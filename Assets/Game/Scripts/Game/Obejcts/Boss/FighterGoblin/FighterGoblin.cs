using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eclipse.Game
{
    public class FighterGoblin : Boss, IDamageable, IDebuffable, IDangerousObject
    {
        // ======================================================
        // 고블린 투사 스킬 핸들러
        public FighterGoblinSkillHandler fighterGoblinSkillHandler;

        // 고블린 투사 Hp 슬라이더
        public Panels.EnemyHPSlider hpSlider;

        // ======================================================
        // 스탯 및 상태 관련 변수들
        [SerializeField] private float moveSpeed;

        // ======================================================
        // 스폰 애니메이션 실행 변수
        private bool hasSpawnRoutinePlayed = false;

        // ======================================================
        // 공격 패턴 쿨타임 타이머와 설정 값
        private float cooldown = 2f;

        // ======================================================
        // 공격 및 기믹 관련 변수들
        private bool isFirstJump; // 첫번째 점프 공격인지 체크하는 변수
        private float punch_dashDistance = 1.0f; // 펀치 대시 거리
        private float attackRange = 0.8f; // 펀치 공격 범위
        private float smash_dashDistance = 0.75f; // 찍기 대시 거리

        private int skillStep = 0;
        private int moveStep = 0;

        // ======================================================
        // 액션 및 델리게이트 모음
        public Action onFirstPunchAttack;
        public Action onSecondPunchAttack;
        public Action onSmashAttack;
        public Action onJumpAttack;

        public Action onBossEnrageStart;
        public Action onBossEnrageFinish;

        public Action<float> onBossChangeRanges;
        public Action<float> onBossChangeDamages;

        //=======================================================
        protected override void Awake()
        {
            base.Awake();

            fighterGoblinSkillHandler = GetComponent<FighterGoblinSkillHandler>();

            // 딕셔너리를 이용한 AnimationClip 캐싱
            Dictionary<string, AnimationClip> clipCache = new Dictionary<string, AnimationClip>();

            foreach (var clip in animator.runtimeAnimatorController.animationClips)
            {
                if (!clipCache.ContainsKey(clip.name))
                    clipCache.Add(clip.name, clip);
            }
            // 애니메이션 클립 연결
            clipCache.TryGetValue("LightFirstAttack_End", out AnimationClip firstPunchAttackAnimationClip);
            clipCache.TryGetValue("LightSecondAttack", out AnimationClip secondPunchAttackAnimationClip);
            clipCache.TryGetValue("AirHeavyAttack_End", out AnimationClip smashAttackAnimationClip);
            clipCache.TryGetValue("Land", out AnimationClip jumpAttackAnimationClip);

            // Animation Event 추가 : 해당 프레임 이후 히트박스 작동시키기
            AddAnimationEvent(firstPunchAttackAnimationClip, nameof(EnableFirstPunchAttackHitBox), 2 * AnimationConstants.FrameTime);
            AddAnimationEvent(secondPunchAttackAnimationClip, nameof(EnableSecondPunchAttackHitBox), 1 * AnimationConstants.FrameTime);
            AddAnimationEvent(smashAttackAnimationClip, nameof(EnableSmashAttackHitBox), 3 * AnimationConstants.FrameTime);
            AddAnimationEvent(jumpAttackAnimationClip, nameof(EnableJumpAttackHitBox), 1 * AnimationConstants.FrameTime);
        }

        protected override void Start()
        {
            base.Start();

            // 회전 방지
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        }
        protected override void StatusInit()
        {
            hasSpawnRoutinePlayed = false;
            isFirstJump = true;

            maxHp = 38000 * (1 + server_PlayerData.specialPassiveLevels[4] * server_PlayerData.nightmareMode); ; // 고블린 투사 풀피
            hp = maxHp; // 풀피 적용
            damage = 1f * (1 + server_PlayerData.specialPassiveLevels[4] * server_PlayerData.nightmareMode); ; // 몸박뎀
            damageReduction = 0.1f; // 데미지 감소 10%
            moveSpeed = 1.5f * (1 + server_PlayerData.specialPassiveLevels[4] * server_PlayerData.nightmareMode); ; // 이동 속도
        }

        // BehaviorTree 생성 시 구현 예정
        protected override Node SetupBehaviorTree()
        {
            // ======================================================
            // 1. Spawn 처리
            Node spawnTask = new CoroutineTask(this, PlaySpawnRoutine, "PlaySpawnRoutine");
            Node spawnDoneCheck = new BoolCheckTask(() => hasSpawnRoutinePlayed, true, "SpawnDoneCheck");

            // ======================================================
            // 2. 스킬 관련 Task
            Node LightAttackTask(int step) => new WaitForCompletionTask(this, LightAttack, () => skillStep == step, "LightAttack");
            Node HeavyAttackTask(int step) => new WaitForCompletionTask(this, HeavyAttack, () => skillStep == step, "HeavyAttack");
            Node JumpInPlaceTask(int step) => new WaitForCompletionTask(this, JumpInPlace, () => skillStep == step, "JumpInPlace");

            // ======================================================
            // 3. 이동 Task
            int count = 0;
            Node MoveForSec(int step)
            {
                count++;
                return new WaitForCompletionTask(this, () => MoveForSeconds(cooldown), () => moveStep == step, $"MoveForSec_{count}");
            }
            
            // ======================================================
            // 4. 메인 행동 묶기
            Node resetTask = new ActionTask(() =>
            {
                skillStep = 0; // 스킬 순서 초기화
                moveStep = 0; // 이동 순서 초기화
            }, "ResetTask");

            Node skillMutexTask = new MutexTask(new List<Node>
            {
                LightAttackTask(0), MoveForSec(0),
                LightAttackTask(1), HeavyAttackTask(2), MoveForSec(1),
                LightAttackTask(3), MoveForSec(2),
                JumpInPlaceTask(4), MoveForSec(3),
                resetTask
            }, "SkillComboMutex");

            // ======================================================
            // 5. 광폭화 체크
            Node enrageActivationCheck = new BoolCheckTask(() => enrageAlreadyActivated, true);
            Node enrageHpCheckTask30 = new CheckTask<float>(() => hp / maxHp, 0.3f);
            Node enrageTask = new ActionTask(Enrage);

            Sequence enrageSequence = new Sequence(new List<Node>
            {
                enrageActivationCheck.Not(), // 아직 광폭화 안 했을 때만
                enrageHpCheckTask30,         // 체력 30% 이하
                enrageTask                   // 광폭화 발동
            }, "EnrageSequence");

            // ======================================================
            // 6. 최종 루트 트리 구성
            return new Selector(new List<Node>
            {
                new Sequence(new List<Node> { spawnDoneCheck.Not(), spawnTask }), // 스폰 애니메이션 먼저
                enrageSequence, // 광폭화 체크
                skillMutexTask,  // 이후는 전투 패턴 반복
            });
        }

        IEnumerator MoveForSeconds(float seconds)
        {
            float timer = 0f;
            while (true)
            {
                MoveToTarget(); // 타겟 방향으로 이동

                timer += Time.fixedDeltaTime;
                if (timer >= seconds) break;

                yield return new WaitForFixedUpdate();
            }
            moveStep++; // 이동 순서 증가
        }

        // 타겟 방향으로 이동하는 함수
        protected void MoveToTarget()
        {
            Vector2 targetPosition = targetObject.transform.position;
            Vector2 bossPosition = transform.position;

            Vector2 direction = targetPosition - bossPosition;

            animator.SetFloat("Speed", moveSpeed); // 일정 속도로 이동 애니메이션
            direction = direction.normalized;
            rigid.MovePosition(rigid.position + direction * moveSpeed * Time.fixedDeltaTime);
        }

        IEnumerator PlaySpawnRoutine(Action<bool> onComplete)
        {
            Debug.Log("고블린 투사 스폰");

            animator.SetTrigger("Jump");
            float animationThreshold = 0.8f; // 애니메이션이 끝나는 시점 (80% 지점)

            col.isTrigger = true; // 점프 공격 중에는 콜라이더를 트리거로 설정
            CalculateVariable(out Vector2 startPosition, out float velocity, out float theta, out float time, out float gravity);
            float elapsedTime = 0f;

            float timeout = 3f;
            float timer = 0f;

            // 애니메이션이 끝날 때까지 기다림
            while (timer < timeout)
            {
                JumpMove(startPosition, elapsedTime, velocity, theta, time, gravity); // 점프 이동
                elapsedTime += Time.fixedDeltaTime;

                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.IsName("Jump_End") && stateInfo.normalizedTime >= animationThreshold)
                {
                    break; // 애니메이션 완료
                }

                timer += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            col.isTrigger = false; // 점프 공격이 끝나면 콜라이더를 다시 일반으로 설정

            onJumpAttack(); // 점프 공격 충격파 생성

            // 애니메이션이 끝날 때까지 기다림
            yield return WaitForAnimationEnd("Land", animationThreshold);

            hasSpawnRoutinePlayed = true; // 스폰 루틴 완료 되었음을 표시
            onComplete?.Invoke(true); // 성공적으로 마쳤다고 판단
        }
        // 점프 이동
        void JumpMove(Vector2 startPosition, float elapsedTime, float velocity, float theta, float time, float gravity)
        {
            // 포물선 운동 계산
            float x = velocity * Mathf.Cos(theta) * elapsedTime;
            float y = velocity * Mathf.Sin(theta) * elapsedTime - 0.5f * gravity * Mathf.Pow(elapsedTime, 2);

            Vector2 vector2 = new Vector2(x, y);
            Vector2 newPosition = startPosition + vector2;
            rigid.MovePosition(newPosition);
        }
        // 포물선 운동 관련 변수들 계산
        void CalculateVariable(out Vector2 startPosition, out float velocity, out float theta, out float time, out float gravity)
        {
            // 시간, 중력은 임의 설정
            time = AnimationConstants.FrameTime * 9f / 1.25f / animator.speed; // 점프 시간 (1.25f는 애니메이션 자체 speed)
            gravity = 9.81f / 2f; // 중력 가속도 (점프력 세게 보이게 하려고 2로 나눔)

            // 위치
            Vector2 targetAheadPos = (Vector2)targetObject.transform.position + new Vector2(2.4f, 0.2f); // 타겟 앞쪽 위치 (약간 앞으로 이동)
            startPosition = transform.position;

            // x축과 y축 속도 계산
            float Velo_x = (targetAheadPos.x - startPosition.x) / time;
            float Velo_y = ((targetAheadPos.y - startPosition.y) + (0.5f * gravity * Mathf.Pow(time, 2))) / time;

            // 총 속도 계산
            velocity = Mathf.Sqrt(Mathf.Pow(Velo_x, 2) + Mathf.Pow(Velo_y, 2));
            theta = Mathf.Atan2(Velo_y, Velo_x); // 각도를 속도 성분으로부터 계산
        }

        /*
            펀치 (이동)공격
            플레이어와 일정 거리(punchAttackRange)만큼 떨어져있는 경우 이동하면서 펀치 공격
            그렇지 않을 경우, 그 자리에서 펀치 공격
        */
        // 약공격 구현
        IEnumerator LightAttack()
        {
            Debug.Log("LightAttack");
            animator.SetTrigger("LightAttack");
            isDirectionLocked = true;
            float animationThreshold = 0.8f; // 애니메이션이 끝나는 시점 (80% 지점)

            Vector2 direction; // 타겟 방향
            float dashSpeed; // 대쉬 속도
            IEnumerator DashRoutine(string anmationName)
            {
                float timeout = 3f;
                float timer = 0f;

                // 애니메이션이 끝날 때까지 기다림
                while (timer < timeout)
                {
                    Dash(direction, dashSpeed); // 타겟 방향으로 대쉬
                    AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                    if (stateInfo.IsName(anmationName) && stateInfo.normalizedTime >= animationThreshold)
                    {
                        break; // 애니메이션 완료
                    }

                    timer += Time.fixedDeltaTime;
                    yield return new WaitForFixedUpdate();
                }
            }

            // 애니메이션이 끝날 때까지 기다림
            yield return new WaitUntil(() =>
            {
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0); // 레이어 0 사용
                return stateInfo.IsName("LightFirstAttack_Start") && stateInfo.normalizedTime >= animationThreshold;
            });

            // 첫번째 펀치 애니메이션 실행
            if (IsTargetInRange())
            {
                // 타겟과 일정 거리 이하일 때 그냥 공격
                yield return StartCoroutine(WaitForAnimationEnd("LightFirstAttack_End", animationThreshold));
            }
            else
            {
                direction = SetDirection(); // 대시 타겟 방향 설정
                dashSpeed = CalculateDashSpeed(animationThreshold, punch_dashDistance, 4f); // 대쉬 속도 계산
                animationThreshold = 4f / 6f; // 애니메이션 프레임에 맞춰 대쉬 애니메이션 끝나는 시점 (4/6 지점)
                // 타겟과 일정 거리 이상 떨어져 있을 때 대쉬
                yield return StartCoroutine(DashRoutine("LightFirstAttack_End"));

                animationThreshold = 0.8f; // 다시 애니메이션 프레임에 맞춰 대쉬 애니메이션 끝나는 시점 (80% 지점)
                yield return StartCoroutine(WaitForAnimationEnd("LightFirstAttack_End", animationThreshold));
            }

            isDirectionLocked = false; // 방향 고정 해제
            SetBossDirection(); // 보스 바라보는 방향 재설정
            isDirectionLocked = true; // 방향 고정

            // 두번째 펀치 애니메이션 실행
            if (IsTargetInRange())
            {
                // 타겟과 일정 거리 이하일 때 그냥 공격
                yield return StartCoroutine(WaitForAnimationEnd("LightSecondAttack", animationThreshold));
            }
            else
            {
                direction = SetDirection(); // 대시 타겟 방향 설정
                dashSpeed = CalculateDashSpeed(animationThreshold, punch_dashDistance, 6f); // 대쉬 속도 계산
                animationThreshold = 6f / 10f; // 애니메이션 프레임에 맞춰 대쉬 애니메이션 끝나는 시점 (6/10 지점)
                // 타겟과 일정 거리 이상 떨어져 있을 때 대쉬
                yield return StartCoroutine(DashRoutine("LightSecondAttack"));

                animationThreshold = 0.8f; // 다시 애니메이션 프레임에 맞춰 대쉬 애니메이션 끝나는 시점 (80% 지점)
                yield return StartCoroutine(WaitForAnimationEnd("LightSecondAttack", animationThreshold));
            }

            isDirectionLocked = false;
            skillStep++; // 스킬 순서 증가
        }
        private Vector2 SetDirection()
        {
            Vector2 targetPosition = (Vector2)targetObject.transform.position;
            Vector2 myPosition = transform.position;
            Vector2 direction = targetPosition - myPosition;

            return direction = direction.normalized;
        }
        private float CalculateDashSpeed(float animationThreshold, float dashDistance, float dashFrame)
        {
            float dashTime = dashFrame * AnimationConstants.FrameTime / animator.speed * animationThreshold; // 대쉬 시간 (애니메이션 프레임에 맞춰 조정)
            float dashSpeed = dashDistance / dashTime; // 대쉬 속도

            return dashSpeed;
        }
        // 타겟 방향으로 이동하는 함수
        private void Dash(Vector2 direction, float dashSpeed)
        {
            rigid.MovePosition(rigid.position + direction * dashSpeed * Time.fixedDeltaTime);
        }
        // 타겟이 사거리 안에 있는지 체크하는 함수
        private bool IsTargetInRange()
        {
            float sqrDistance = (transform.position - targetObject.transform.position).sqrMagnitude;
            return sqrDistance <= attackRange * attackRange;
        }

        IEnumerator HeavyAttack()
        {
            Debug.Log("HeavyAttack");
            animator.SetTrigger("HeavyAttack");
            isDirectionLocked = true;
            float animationThreshold = 0.8f; // 애니메이션이 끝나는 시점 (80% 지점)
            col.isTrigger = true; // HeavyAttack 중에는 콜라이더를 트리거로 설정

            // 애니메이션이 끝날 때까지 기다림
            yield return WaitForAnimationEnd("AirHeavyAttack_Start", animationThreshold);

            Vector2 direction = SetDirection(); // 타겟 방향 설정
            float dashSpeed = CalculateDashSpeed(animationThreshold, smash_dashDistance, 3f); // 대쉬 속도 계산

            animationThreshold = 3f / 15f; // 애니메이션 프레임에 맞춰 대쉬 애니메이션 끝나는 시점 (3/15 지점)
            
            float timeout = 3f;
            float timer = 0f;
            // 애니메이션이 끝날 때까지 기다림
            while (timer < timeout)
            {
                Dash(direction, dashSpeed); // 타겟 방향으로 대쉬
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                if (stateInfo.IsName("AirHeavyAttack_End") && stateInfo.normalizedTime >= animationThreshold)
                {
                    break; // 애니메이션 완료
                }

                timer += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }
            col.isTrigger = false; // HeavyAttack이 끝나면 콜라이더를 다시 일반으로 설정

            animationThreshold = 0.8f; // 다시 애니메이션 프레임에 맞춰 대쉬 애니메이션 끝나는 시점 (80% 지점)
            // 애니메이션이 끝날 때까지 기다림
            yield return WaitForAnimationEnd("AirHeavyAttack_End", animationThreshold);

            isDirectionLocked = false;
            skillStep++; // 스킬 순서 증가
        }

        IEnumerator JumpInPlace()
        {
            Debug.Log("JumpInPlace");
            animator.SetTrigger("Jump");
            isDirectionLocked = true;
            float animationThreshold = 0.8f; // 애니메이션이 끝나는 시점 (80% 지점)

            // 애니메이션이 끝날 때까지 기다림
            yield return WaitForAnimationEnd("Jump_End", animationThreshold);

            if (isFirstJump)
            {
                SpawnMonster(); // 첫번째 점프 공격 시 몬스터 소환
            }
            else
            {
                SpawnPoisonSwamp(); // 두번째 점프 공격 시 늪 소환
            }
            isFirstJump = !isFirstJump; // 다음 점프 공격은 반대 행동

            // 애니메이션이 끝날 때까지 기다림
            yield return WaitForAnimationEnd("Land", animationThreshold);

            isDirectionLocked = false;
            skillStep++; // 스킬 순서 증가
        }

        // 몬스터 소환
        private void SpawnMonster()
        {
            Debug.Log("SpawnMonster");

            int currentStage = 1; // 현재 스테이지
            int spawnCount = 10; // 소환할 몬스터 수
            fighterGoblinSkillHandler.onSpawnGimmicEnemies(currentStage, spawnCount, transform.position); // 몬스터 소환 액션 실행
        }

        // 늪 소환
        private void SpawnPoisonSwamp()
        {
            Debug.Log("SpawnPoisonSwamp");

            int spawnCount = enrageAlreadyActivated ? 4 : 2; // 소환할 늪 수
            fighterGoblinSkillHandler.onSpawnGimmicPoisonSwamp(spawnCount, transform.position); // 늪 소환 액션 실행
        }

        // 광폭화 동작
        private void Enrage()
        {
            Debug.Log("Enrage");
            // 광폭화 구현, 데미지, 속도 증가, 쿨타임 감소
            damage *= enrage_DamageIncrementCoefficient;
            onBossChangeDamages(enrage_DamageIncrementCoefficient);

            animator.speed *= enrage_SpeedIncrementCoefficient; // 애니메이션 속도 증가
            moveSpeed *= enrage_SpeedIncrementCoefficient; // 이동 속도 증가

            cooldown /= enrage_CoolDownDecrementCoefficient;

            spriteRenderer.color = new Color(1f, 0.6f, 0.6f, 1f); // 색상 변경
            enrageAlreadyActivated = true;
        }

        // IDamageable
        public override void TakeDamage(string causerTag, float damage, bool isCritical = false, float knockbackForce = 0)
        {
            base.TakeDamage(causerTag, damage, isCritical, knockbackForce);

            float reduedDamage = damage * (1f - damageReduction); // 데미지 감소량만큼 데미지를 덜 입음 (현재 기본 뎀감은 0%)

            hp -= reduedDamage;
            if (hp <= 0)
            {
                hp = 0;
                StartCoroutine(Die());
            }

            InGameTextManager.Instance.ShowText(Mathf.RoundToInt(reduedDamage).ToString(), causerTag, isCritical, transform.position);
        }

        IEnumerator Die()
        {
            isDead = true;
            isDirectionLocked = true; // 방향 고정

            rigid.constraints = RigidbodyConstraints2D.FreezeAll;
            col.enabled = false;

            animator.speed = 1f; // 애니메이션 속도 복구
            spriteRenderer.color = Color.white; // 색상 복구
            animator.Play("Dead"); // 죽는 애니메이션 실행
            float animationThreshold = 0.8f; // 애니메이션이 끝났다고 판단할 기준 (80% 이상)

            // 애니메이션이 끝날 때까지 기다림
            yield return WaitForAnimationEnd("Dead", animationThreshold);

            Destroy(hpSlider.gameObject); // hp 슬라이더 제거
            Destroy(gameObject);
            onBossDead(); // 죽었을 때, 텔레포트 이정표 뜨고 텔레포트 스폰시키기
        }

        protected override IEnumerator SlowDownRoutine(float duration)
        {
            float originalSpeed = animator.speed; // 원래 애니메이션 속도 저장
            animator.speed = originalSpeed * slowDownCoefficient; // 애니메이션 속도 감소

            float savedMoveSpeed = moveSpeed; // 원래 이동속도 저장
            moveSpeed *= slowDownCoefficient; // 이동속도 감소
            transform.GetChild(1).gameObject.SetActive(true);

            yield return new WaitForSeconds(duration);

            animator.speed = originalSpeed; // 원래 속도로 복구
            moveSpeed = savedMoveSpeed; // 이동속도 복구
            transform.GetChild(1).gameObject.SetActive(false);

            slowDownCoroutine = null; // 코루틴 종료
        }

        //==================================================================
        // 애니메이션 이벤트에서 호출되는 메서드들
        public void EnableFirstPunchAttackHitBox()
        {
            onFirstPunchAttack();
        }
        public void EnableSecondPunchAttackHitBox()
        {
            onSecondPunchAttack();
        }
        public void EnableSmashAttackHitBox()
        {
            onSmashAttack();
        }

        public void EnableJumpAttackHitBox()
        {
            onJumpAttack();
        }
    }
}