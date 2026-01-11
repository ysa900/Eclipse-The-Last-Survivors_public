using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eclipse.Game
{
    public class RuinedKing : Boss, IDamageable, IDebuffable, IDangerousObject
    {
        // ======================================================
        // 스킬 핸들러
        public RuinedKingSkillHandler skillHandler;

        // 보스 Hp 슬라이더
        public Panels.EnemyHPSlider hpSlider;

        // ======================================================
        // 스탯 관련 변수들
        [SerializeField] private float moveSpeed;

        // ======================================================
        // 스폰 애니메이션 실행 변수
        private bool hasSpawnAnimationPlayed = false;

        // ======================================================
        // 스킬 관련 변수들
        // Jump Attack, Defense, Walking Defense, Spawn Monster
        private List<float> cooldowns = new List<float> { 5, 8, 10, 20 };
        private List<float> cooltimers = new List<float> { 3, 8, 10, 15 };

        private float attackRange; // 공격 범위
        private bool isAttackRunning; // 공격 중인지 체크하는 변수

        [SerializeField] private bool isDefensing = false; // 방어 중인지 체크하는 변수

        // ======================================================
        // BossManager에게 스킬 사용을 알려주기 위한 Action들
        public Action onLightAttack1;
        public Action onLightAttack2;
        public Action onHeavyAttack;
        public Action onJumpAttack;
        public Action<float> onBossChangeDamages;

        protected override void Awake()
        {
            base.Awake();

            skillHandler = GetComponent<RuinedKingSkillHandler>();

            // 딕셔너리를 이용한 AnimationClip 캐싱
            Dictionary<string, AnimationClip> clipCache = new Dictionary<string, AnimationClip>();

            foreach (var clip in animator.runtimeAnimatorController.animationClips)
            {
                if (!clipCache.ContainsKey(clip.name))
                    clipCache.Add(clip.name, clip);
            }

            // 애니메이션 클립 연결
            clipCache.TryGetValue("LightAttack", out AnimationClip lightAttackAnimationClip);
            clipCache.TryGetValue("HeavyAttack", out AnimationClip heavyAttackAnimationClip);

            // Animation Event 추가
            AddAnimationEvent(lightAttackAnimationClip, nameof(EnableLightAttackHitBox1), 6 * AnimationConstants.FrameTime);
            AddAnimationEvent(lightAttackAnimationClip, nameof(EnableLightAttackHitBox2), 12 * AnimationConstants.FrameTime);
            AddAnimationEvent(heavyAttackAnimationClip, nameof(EnableHeavyAttackHitBox), 6 * AnimationConstants.FrameTime);
        }

        protected override void Start()
        {
            base.Start();

            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        protected override void StatusInit()
        {
            // 스탯 설정
            maxHp = 300000 * (1 + server_PlayerData.specialPassiveLevels[4] * server_PlayerData.nightmareMode); ; // 몰락한 왕 풀피
            hp = maxHp; // 최대 hp 적용
            moveSpeed = 1.5f * (1 + server_PlayerData.specialPassiveLevels[4] * server_PlayerData.nightmareMode); ; // 이동 속도
            damage = 1f * (1 + server_PlayerData.specialPassiveLevels[4] * server_PlayerData.nightmareMode); ; // 몸박뎀
            damageReduction = 0f; //데미지 감소 수치
            attackRange = 1f; // 평타 사거리

            // 변수 초기화
            hasSpawnAnimationPlayed = false;
            isDefensing = false;
        }

        protected override void UpdateTimers()
        {
            for (int i = 0; i < cooltimers.Count; i++)
            {
                cooltimers[i] -= Time.deltaTime;
            }
        }

        protected override Node SetupBehaviorTree()
        {
            // ======================================================
            // 1. Spawn 처리
            Node spawnTask = new CoroutineTask(this, PlaySpawnAnimation, "SpawnAnimation");
            Node spawnDoneCheck = new BoolCheckTask(() => hasSpawnAnimationPlayed, true, "SpawnDoneCheck");

            // ======================================================
            // 2. 스킬 관련 Task
            Node jumpAttack = new CoroutineTask(this, JumpAttack, "JumpAttack 코루틴");
            Node defense = new CoroutineTask(this, Defense, "Defense 코루틴");
            Node walkingDefense = new CoroutineTask(this, WalkingDefense, "WalkingDefense 코루틴");
            Node spawnMonster = new CoroutineTask(this, SpawnMonster, "SpawnMonster 코루틴");

            // 쿨타임 체크 Task
            Node jumpAttackCoolDownCheck = new CheckTask<float>(() => cooltimers[0], 0, "JumpAttack 쿨타임 체크");
            Node defenseCoolDownCheck = new CheckTask<float>(() => cooltimers[1], 0, "Defense 쿨타임 체크");
            Node walkingDefenseCoolDownCheck = new CheckTask<float>(() => cooltimers[2], 0, "WalkingDefense 쿨타임 체크");
            Node spawnMonsterCoolDownCheck = new CheckTask<float>(() => cooltimers[3], 0, "SpawnMonster 쿨타임 체크");

            // ======================================================
            // 3. 사거리 체크 및 이동/공격 Task

            // 랜덤 공격 실행 (LightAttack 또는 HeavyAttack)
            Node randomAttack = new CoroutineTask(this, (onComplete) =>
            {
                return UnityEngine.Random.value < 0.5f ? LightAttack(onComplete) : HeavyAttack(onComplete);
            }, "랜덤 공격 실행");

            // 현재 공격이 실행중인지 체크
            Node attackRunningCheck = new BoolCheckTask(() => isAttackRunning, true, "랜덤 공격 실행중 체크");

            // 사거리 안에 있는지 체크
            Node isTargetInRangeCheck = new BoolCheckTask(() => IsTargetInRange(), true, "사거리 체크");

            Node isRandomAttackAvailableCheck = new Selector(new List<Node>
            {
                attackRunningCheck,     // 공격이 실행중일 때
                isTargetInRangeCheck,   // 사거리 안에 있을 때
            }, "랜덤 공격 가능 체크");

            // ======================================================
            // 4. 메인 행동 묶기 (MutexTask)
            Node actionMutexTask = new MutexTask(new List<Node>
            {
                new Sequence(new List<Node> { defenseCoolDownCheck, defense }, "DefenseSequence"),
                new Sequence(new List<Node> { walkingDefenseCoolDownCheck, walkingDefense }, "WalkingDefenseSequence"),
                new Sequence(new List<Node> { spawnMonsterCoolDownCheck, spawnMonster }, "SpawnMonsterSequence"),
                new Sequence(new List<Node> { isRandomAttackAvailableCheck, randomAttack }, "AttackSequence"), // 사거리 안이면 랜덤 공격
                new Sequence(new List<Node> { jumpAttackCoolDownCheck, jumpAttack }, "JumpAttackSequence"),
                new ActionTask(MoveToTarget, "MoveToTarget") // 사거리 밖이면 이동
            }, "ActionMutexTask");

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
                new Parallel(new List<Node> { enrageSequence, actionMutexTask })  // 이후는 전투 패턴 반복
            });
        }

        // 타겟이 사거리 안에 있는지 체크하는 함수
        private bool IsTargetInRange()
        {
            float angleThreshold = 60f;

            Vector2 targetPosition = targetObject.transform.position;
            Vector2 myPosition = (Vector2)transform.position + col.offset;
            Vector2 directionToTarget = (targetPosition - myPosition).normalized;
            Vector2 bossForward = isBossLookLeft ? Vector2.left : Vector2.right;

            // 거리 판단
            float sqrDistanceToTarget = (targetPosition - myPosition).sqrMagnitude;
            bool isInRange = sqrDistanceToTarget <= attackRange * attackRange;

            // Dot Product 계산
            float dot = Vector2.Dot(bossForward, directionToTarget); // 보스 정면과 타겟 방향의 내적 (둘 사이의 각도)
            float angleDotThreshold = Mathf.Cos(angleThreshold * Mathf.Deg2Rad);

            bool isInAngle = Mathf.Abs(dot) >= angleDotThreshold; // 정면 또는 후면

            return isInRange && isInAngle;
        }


        // 타겟 방향으로 이동하는 함수
        private void MoveToTarget()
        {
            Vector2 targetPosition = targetObject.transform.position;
            Vector2 myPosition = transform.position;

            Vector2 direction = targetPosition - myPosition;

            if (IsTargetInRange())
            {
                // 멈춤 처리
                animator.SetFloat("Speed", 0f);
                return;
            }

            animator.SetFloat("Speed", moveSpeed); // 일정 속도로 이동 애니메이션
            direction = direction.normalized;
            rigid.MovePosition(rigid.position + direction * moveSpeed * Time.fixedDeltaTime);
        }

        IEnumerator PlaySpawnAnimation(Action<bool> onComplete)
        {
            Debug.Log("몰락한 왕 스폰");
            animator.SetTrigger("Spawn");
            float animationThreshold = 0.99f; // 애니메이션이 끝났다고 판단할 기준 (99% 이상)

            // 애니메이션이 끝날 때까지 기다림
            yield return WaitForAnimationEnd("Spawn", animationThreshold);

            hasSpawnAnimationPlayed = true; // 스폰 애니메이션이 재생되었음을 표시
            onComplete?.Invoke(true); // 성공적으로 마쳤다고 판단
        }

        // 강공격 구현
        IEnumerator HeavyAttack(Action<bool> onComplete)
        {
            Debug.Log("HeavyAttack");
            animator.SetTrigger("HeavyAttack");
            isAttackRunning = true;
            isDirectionLocked = true;
            float animationThreshold = 0.8f; // 애니메이션이 끝났다고 판단할 기준 (80% 이상)

            // 애니메이션이 끝날 때까지 기다림
            yield return WaitForAnimationEnd("HeavyAttack", animationThreshold);

            isAttackRunning = false;
            isDirectionLocked = false;
            onComplete?.Invoke(true); // 성공적으로 마쳤다고 판단
        }

        // 약공격 구현
        IEnumerator LightAttack(Action<bool> onComplete)
        {
            Debug.Log("LightAttack");
            animator.SetTrigger("LightAttack");
            isAttackRunning = true;
            isDirectionLocked = true;
            float animationThreshold = 0.8f; // 애니메이션이 끝났다고 판단할 기준 (80% 이상)

            // 애니메이션이 끝날 때까지 기다림
            yield return WaitForAnimationEnd("LightAttack", animationThreshold);

            isAttackRunning = false;
            isDirectionLocked = false;
            onComplete?.Invoke(true); // 성공적으로 마쳤다고 판단
        }

        // 점프 공격
        IEnumerator JumpAttack(Action<bool> onComplete)
        {
            Debug.Log("JumpAttack");
            animator.SetTrigger("Jumping_Start");
            float animationThreshold = 0.8f; // 애니메이션이 끝났다고 판단할 기준 (80% 이상)

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
                if (stateInfo.IsName("Falling") && stateInfo.normalizedTime >= animationThreshold)
                    break;

                timer += Time.fixedDeltaTime;
                yield return new WaitForFixedUpdate();
            }

            col.isTrigger = false; // 점프 공격이 끝나면 콜라이더를 다시 일반으로 설정

            onJumpAttack(); // 점프 공격 충격파 생성

            // 애니메이션이 끝날 때까지 기다림
            yield return WaitForAnimationEnd("Land", animationThreshold);

            onComplete?.Invoke(true); // 성공적으로 마쳤다고 판단
            StartCoroutine(ResetCoolTimer(0)); // 쿨타임 초기화
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
            time = AnimationConstants.FrameTime * 10f / animator.speed; // 점프 시간
            gravity = 9.81f; // 중력 가속도

            // 위치
            Vector2 targetPosition = (Vector2)targetObject.transform.position - col.offset * 2;
            startPosition = transform.position;

            // x축과 y축 속도 계산
            float Velo_x = (targetPosition.x - startPosition.x) / time;
            float Velo_y = ((targetPosition.y - startPosition.y) + (0.5f * gravity * Mathf.Pow(time, 2))) / time;

            // 총 속도 계산
            velocity = Mathf.Sqrt(Mathf.Pow(Velo_x, 2) + Mathf.Pow(Velo_y, 2));
            theta = Mathf.Atan2(Velo_y, Velo_x); // 각도를 속도 성분으로부터 계산
        }

        // 방어 패턴
        IEnumerator Defense(Action<bool> onComplete)
        {
            Debug.Log("Defense");
            animator.ResetTrigger("Defense_Finish");
            animator.SetBool("IsWalkingDefensing", false);
            animator.SetTrigger("Defense_Start");
            isDefensing = true;
            float animationThreshold = 0.99f; // 애니메이션이 끝났다고 판단할 기준 (99% 이상)

            // 방어 시간동안 대기
            float defenseTime = 1.5f; // 방어 시간
            yield return new WaitForSeconds(defenseTime);
            animator.SetTrigger("Defense_Finish");

            float timeout = 5f * AnimationConstants.FrameTime / animator.speed; // 애니메이션이 끝날 때까지 기다리는 최대 시간(안전장치)
            float timer = 0f;
            // 애니메이션이 끝날 때까지 기다림
            yield return new WaitUntil(() =>
            {
                timer += Time.deltaTime;
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0); // 레이어 0 사용
                return 
                    (stateInfo.IsName("Defensing")) ||
                    (stateInfo.IsName("Defense_Success") && stateInfo.normalizedTime >= 0.99f)
                    || (timer > timeout);
            });

            isDefensing = false;
            if(timer > timeout)
            {
                // 방어 애니메이션이 끝나지 않았을 때
                animator.Play("Defensing_End", 0, 1f); // 방어 애니메이션을 강제로 끝냄
            }

            animationThreshold = 0.8f;
            // 애니메이션이 끝날 때까지 기다림
            yield return WaitForAnimationEnd("Defensing_End", animationThreshold);

            animator.ResetTrigger("Defense_Success");
            onComplete?.Invoke(true); // 성공적으로 마쳤다고 판단
            StartCoroutine(ResetCoolTimer(1)); // 쿨타임 초기화
        }

        // 방어 이동 패턴
        IEnumerator WalkingDefense(Action<bool> onComplete)
        {
            Debug.Log("WalkingDefensing");
            animator.SetTrigger("Defense_Start");
            animator.SetBool("IsWalkingDefensing", true);
            isDirectionLocked = true;
            isDefensing = true;
            
            yield return new WaitForSeconds(0.3f); // 방어 애니메이션 대기

            animator.SetTrigger("WalkingDefensing_Start");
            float speedSave = moveSpeed;
            moveSpeed = 8f;

            Vector2 direction = SetDirection();

            float dashTime = 0.6f;
            float dashtimer = 0f;

            while (true)
            {
                dashtimer += Time.fixedDeltaTime;
                if (dashtimer > dashTime)
                {
                    break;
                }

                Dash(direction); // 타겟 방향으로 이동
                yield return new WaitForFixedUpdate();
            }

            animator.SetTrigger("WalkingDefensing_Finish");
            isDirectionLocked = false;
            isDefensing = false;
            moveSpeed = speedSave;
            float animationThreshold = 0.8f; // 애니메이션이 끝났다고 판단할 기준 (80% 이상)

            // 애니메이션이 끝날 때까지 기다림
            yield return WaitForAnimationEnd("Defensing_End", animationThreshold);

            animator.SetBool("IsWalkingDefensing", false);
            animator.ResetTrigger("Defense_Success");
            onComplete?.Invoke(true); // 성공적으로 마쳤다고 판단
            StartCoroutine(ResetCoolTimer(2)); // 쿨타임 초기화
        }
        private Vector2 SetDirection()
        {
            Vector2 targetPosition = (Vector2)targetObject.transform.position - col.offset * 2;
            Vector2 myPosition = transform.position;
            Vector2 direction = targetPosition - myPosition;

            return direction = direction.normalized;
        }
        // 타겟 방향으로 이동하는 함수
        private void Dash(Vector2 direction)
        {
            rigid.MovePosition(rigid.position + direction * moveSpeed * Time.fixedDeltaTime);
        }

        // 몬스터 소환
        IEnumerator SpawnMonster(Action<bool> onComplete)
        {
            Debug.Log("SpawnMonster");

            float savedMoveSpeed = moveSpeed; // 원래 이동속도 저장
            animator.SetFloat("Speed", 0f); // 정지
            animator.Play("Idle");

            float waitTime = enrageAlreadyActivated ? 0.75f : 1.5f; // 광폭화 상태일 때 대기 시간
            yield return new WaitForSeconds(waitTime); // 잠시 대기

            int currentStage = 2; // 현재 스테이지
            int spawnCount = 10; // 소환할 몬스터 수
            skillHandler.onSpawnGimmicEnemies(currentStage, spawnCount, transform.position); // 몬스터 소환 액션 실행
            animator.SetFloat("Speed", moveSpeed); // 정지 // 이동속도 복구

            onComplete?.Invoke(true); // 성공적으로 마쳤다고 판단
            StartCoroutine(ResetCoolTimer(3)); // 쿨타임 초기화
        }

        // 프레임 지연 상태 전이를 위해 한 프레임 대기하고 쿨타임을 초기화 하는 코루틴
        IEnumerator ResetCoolTimer(int index)
        {
            yield return null;
            cooltimers[index] = cooldowns[index];
        }

        // 광폭화 동작
        protected void Enrage()
        {
            Debug.Log("Enrage");
            // 광폭화 구현, 데미지, 속도 증가, 쿨타임 감소
            damage *= enrage_DamageIncrementCoefficient;
            onBossChangeDamages(enrage_DamageIncrementCoefficient);

            animator.speed *= enrage_SpeedIncrementCoefficient; // 애니메이션 속도 증가
            moveSpeed *= enrage_SpeedIncrementCoefficient; // 이동 속도 증가

            for (int i = 0; i < cooldowns.Count; i++)
            {
                cooldowns[i] /= enrage_CoolDownDecrementCoefficient;
            }

            spriteRenderer.color = new Color(1f, 0.6f, 0.6f, 1f); // 색상 변경
            enrageAlreadyActivated = true;
        }

        public override void TakeDamage(string causerTag, float damage, bool isCritical = false, float knockbackForce = 0)
        {
            float reduedDamage = damage * (1f - damageReduction); // 데미지 감소량만큼 데미지를 덜 입음 (현재 기본 뎀감은 0%)

            bool canBlockAttack = isDefensing && IsAttackFromFront();
            if (canBlockAttack)
            {
                animator.SetTrigger("Defense_Success");
                reduedDamage = 0; // 가드 시에는 데미지 0 받기
            }

            hp -= reduedDamage;
            if (hp <= 0)
            {
                hp = 0;
                StartCoroutine(Die());
            }

            // 데미지 텍스트 출력
            InGameTextManager.Instance.ShowText(Mathf.RoundToInt(reduedDamage).ToString(), causerTag, isCritical, transform.position);
        }

        bool IsAttackFromFront()
        {
            float angleThreshold = 45f; // 각도 임계값

            Vector2 attackerPosition = targetObject.transform.position;
            Vector2 forward = isBossLookLeft ? Vector2.left : Vector2.right;
            Vector2 dirFromAttacker = ((Vector2)transform.position - attackerPosition).normalized;

            return Vector2.Dot(forward, -dirFromAttacker) > Mathf.Cos(angleThreshold * Mathf.Deg2Rad);
        }

        //죽음
        IEnumerator Die()
        {
            isDead = true;
            isDirectionLocked = true; // 방향 고정
            rigid.constraints = RigidbodyConstraints2D.FreezeAll;
            col.enabled = false;

            animator.speed = 1f; // 애니메이션 속도 복구
            spriteRenderer.color = Color.white; // 색상 복구
            animator.Play("Death", 0, 0f); // 죽음 애니메이션 재생
            animator.SetBool("Uninterruptible", true);
            float animationThreshold = 0.8f; // 애니메이션이 끝났다고 판단할 기준 (80% 이상)

            // 애니메이션이 끝날 때까지 기다림
            yield return WaitForAnimationEnd("Death", animationThreshold);

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
        public void EnableLightAttackHitBox1()
        {
            onLightAttack1();
        }
        public void EnableLightAttackHitBox2()
        {
            onLightAttack2();
        }
        public void EnableHeavyAttackHitBox()
        {
            onHeavyAttack();
        }
    }
}