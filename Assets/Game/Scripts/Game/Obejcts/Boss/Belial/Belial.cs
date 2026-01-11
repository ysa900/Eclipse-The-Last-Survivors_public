using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eclipse.Game
{
    public class Belial : Boss, IDamageable, IDebuffable, IDangerousObject
    {
        // ======================================================
        public BelialSkillHandler skillHandler; // 벨리알 스킬 핸들러

        // ======================================================
        // 벨리알 피격 모션 쿨타임
        private float hitAnimationDelay = 3f;
        private float hitAnimationDelayTimer = 0;

        // ======================================================
        // 공격 쿨타임 타이머와 설정 값
        // 기본 공격, 쿨타임 4초
        // 레이저, 쿨타임 8초
        // 그리드 레이저, 쿨타임 10초
        // 제네시스, 쿨타임 15초
        // 텔레포트, 쿨타임 3초
        private List<float> cooldowns = new List<float> { 4, 8, 10, 15, 3 };
        private List<float> cooltimers = new List<float> { 4, 6, 0, 0, 0 };

        // ======================================================
        // 기믹 관련

        public bool isDefenceGimmickRunning = false; // 벨리알 방어 석상 기믹 실행중 판단
        public bool isAttackGimmickRunning = false; // 벨리알 공격 석상 기믹 실행중 판단
        public bool isPurificationGimmickRunning = false; // 벨리알 정화 기믹 실행중 판단
        public bool isPurificationGimmickSuccessed = false; // 벨리알 정화 기믹 성공 판단

        public bool shouldRetryDefenceGimmick = false; // 벨리알 방어 석상 기믹 실패 후, 재실행 필요 판단
        public bool shouldRetryAttackGimmick = false; // 벨리알 공격 석상 기믹 실패 후, 재실행 필요 판단
        public bool shouldRetryPurificationGimmick = false; // 벨리알 정화 기믹 실패 후, 재실행 필요 판단

        // 벨리알 기믹 컨트롤러
        BossGimmickController bossGimmickController;

        // 벨리알 스킬 모음
        GameObject bossShieldPrefab; // 벨리알 쉴드 프리팹
        GameObject bossFlamePrefab;  // 벨리알 불꽃 프리팹
        GameObject bossShield; // 벨리알 쉴드
        GameObject bossFlame;  // 벨리알 불꽃

        // ======================================================
        // 맵 관련
        Stage3Map stage;
        [ReadOnly] public float mapRadius; // Stage3Map 콜라이더 radius 값 * scale 값
        [ReadOnly] public Vector2 mapCenter; // 맵의 중앙

        // BossManager에게 스킬 사용을 알려주기 위한 Action들
        public Action onBossTryBasicAttack;
        public Action<float> onBossTryLaserAttack;
        public Action<float, float, bool> onBossTryGridLaserAttack;
        public Action onBossTryGenesisAttack;
        public Action<float> onBossChangeDamages;

        //=======================================================
        private System.Random random = new System.Random();

        //=======================================================
        protected override void Awake()
        {
            base.Awake();

            skillHandler = GetComponent<BelialSkillHandler>();

            // 딕셔너리를 이용한 AnimationClip 캐싱
            Dictionary<string, AnimationClip> clipCache = new Dictionary<string, AnimationClip>();

            foreach (var clip in animator.runtimeAnimatorController.animationClips)
            {
                if (!clipCache.ContainsKey(clip.name))
                    clipCache.Add(clip.name, clip);
            }

            // 애니메이션 클립 연결
            clipCache.TryGetValue("Attack", out AnimationClip attackAnimationClip);
            clipCache.TryGetValue("teleport", out AnimationClip teleportAnimationClip);
            clipCache.TryGetValue("teleport_arrive", out AnimationClip teleport_arriveAnimationClip);

            // Animation Event 추가
            AddAnimationEvent(attackAnimationClip, nameof(EnableAttack), 3 * AnimationConstants.FrameTime);
            AddAnimationEvent(teleportAnimationClip, nameof(OnTeleportStart), 0 * AnimationConstants.FrameTime);
            AddAnimationEvent(teleport_arriveAnimationClip, nameof(OnTeleportEnd), 0 * AnimationConstants.FrameTime);

            bossGimmickController = GetComponent<BossGimmickController>();
            bossShieldPrefab = Resources.Load<GameObject>("Prefabs/Char_Eclipse/Boss/Belial/BossShield");
            bossFlamePrefab = Resources.Load<GameObject>("Prefabs/Char_Eclipse/Boss/Belial/BossFlame");
        }

        protected override void Start()
        {
            base.Start();

            // 맵 관련 변수 설정
            stage = FindAnyObjectByType<Stage3Map>();
            CircleCollider2D mapCircleCollider = stage.GetComponent<CircleCollider2D>();
            mapRadius = mapCircleCollider.radius * stage.transform.localScale.x;
            mapCenter = (Vector2)stage.transform.position / stage.transform.localScale.x + mapCircleCollider.offset;

            // 플레이어가 밀 때 밀리지 않게 좌표를 고정
            rigid.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        protected override void StatusInit()
        {
            maxHp = 666666f * (1 + server_PlayerData.specialPassiveLevels[4] * server_PlayerData.nightmareMode);
            hp = maxHp; // 최대 hp 적용
            damage = 2.4f * (1 + server_PlayerData.specialPassiveLevels[4] * server_PlayerData.nightmareMode); ;
            damageReduction = 0f;
        }

        protected override void UpdateTimers()
        {
            base.UpdateTimers();

            // 쿨타임 타이머 업데이트
            for (int i = 0; i < cooltimers.Count; i++)
            {
                cooltimers[i] -= Time.deltaTime;
            }
        }

        protected override Node SetupBehaviorTree()
        {
            // 기본 공격 및 스킬 Task에 쿨타임 적용
            // CoroutineTask는 반복적으로 실행할 작업에 대해 사용
            Node basicAttack = new CoroutineTask(this, CastBasicAttack, "기본 공격 코루틴");               // 기본 공격, 쿨타임 8초
            Node lineLaser = new CoroutineTask(this, CastLineLaser, "직선 레이저 코루틴");                 // 직선형 레이저, 쿨타임 15초
            Node gridLaser = new CoroutineTask(this, CastGridLaser, "그리드 레이저 코루틴");               // 그리드 레이저, 쿨타임 20초
            Node tripleLineLaser = new CoroutineTask(this, CastTripleLineLaser, "트리플 레이저 코루틴");   // 강화된 레이저, 쿨타임 15초
            Node genesis = new CoroutineTask(this, CastGenesis, "제네시스 코루틴");                        // 제네시스, 쿨타임 30초
            Node teleport = new CoroutineTask(this, CastTelePort, "텔레포트 코루틴");                      // 텔레포트

            // 쿨타임 체크 Task
            Node basicAttackCoolDownCheck = new CheckTask<float>(() => cooltimers[0], 0, "기본공격 쿨타임 체크");
            Node lineLaserCoolDownCheck = new CheckTask<float>(() => cooltimers[1], 0, "레이저 쿨타임 체크");
            Node gridCoolDownCheck = new CheckTask<float>(() => cooltimers[2], 0, "그리드 레이저 쿨타임 체크");
            Node genesisCoolDownCheck = new CheckTask<float>(() => cooltimers[3], 0, "제네시스 쿨타임 체크");
            Node teleportCoolDownCheck = new CheckTask<float>(() => cooltimers[4], 0, "텔레포트 쿨타임 체크");

            // 체력 비율 체크 Task
            Node hpCheckTask100 = new CheckTask<float>(() => hp / maxHp, 1.0f);  // 체력 100% 이하
            Node hpCheckTask80 = new CheckTask<float>(() => hp / maxHp, 0.8f);   // 체력 80% 이하
            Node hpCheckTask50 = new CheckTask<float>(() => hp / maxHp, 0.5f);   // 체력 50% 이하
            Node hpCheckTask30 = new CheckTask<float>(() => hp / maxHp, 0.3f);   // 체력 30% 이하

            // 기믹 체력 체크 Task
            Node gimmick70Check = new CheckTask<float>(() => hp / maxHp, 0.7f);
            Node gimmick30Check = new CheckTask<float>(() => hp / maxHp, 0.3f);
            Node finalGimmickCheck = new CheckTask<float>(() => hp / maxHp, 0f);

            // 기믹 작업 Task
            // WaitForCompletionTask는 한번 실행이 완료되면 계속 Success를 리턴
            Node defenseGimmick = new WaitForCompletionTask(this, DefenseGimmickCoroutine, () => shouldRetryDefenceGimmick, "defenseGimmick");
            Node attackGimmick = new WaitForCompletionTask(this, AttackGimmickCoroutine, () => shouldRetryAttackGimmick, "attackGimmick");
            Node purificationGimmick = new WaitForCompletionTask(this, PurificationGimmickCoroutine, () => shouldRetryPurificationGimmick, "purificationGimmick");

            // 시간 제한 광폭화 Task
            /*Node enrageActivationCheck = new BoolCheckTask(() => enrageAlreadyActivated, false);
            Node enrageTimeCheck = new CheckTask<float>(() => elapsedTimer, 0); // 5분 경과 시
            Node enrageTask = new ActionTask(Enrage);*/

            // 직선형 레이저 셀렉터 생성
            Selector lineLaserSelector = new Selector(new List<Node>
            {
                new Sequence(new List<Node> { hpCheckTask50.Not(), lineLaserCoolDownCheck, lineLaser }),  // 체력 50% 초과일 때 직선형 레이저
                new Sequence(new List<Node> { hpCheckTask50, lineLaserCoolDownCheck, tripleLineLaser }), // 체력 50% 이하일 때 강화된 직선형 레이저
            }, "lineLaserSequnce");

            // 행동 뮤텍스 테스크 생성
            MutexTask actionMutexTask = new MutexTask(new List<Node>
            {
                new Sequence(new List<Node> { hpCheckTask100, basicAttackCoolDownCheck, basicAttack }, "basicAttackSequence"),  // 체력 100% 이하일 때 기본 공격
                new Sequence(new List<Node> { hpCheckTask80, gridCoolDownCheck, gridLaser }, "gridLaserSequence"),              // 체력 80% 이하일 때 그리드 레이저
                lineLaserSelector,                                                                                              // 체력 50%를 기준으로 직선형 레이저 셀렉트
                new Sequence(new List<Node> { hpCheckTask30, genesisCoolDownCheck, genesis }, "genesisSequence"),               // 체력 30% 이하일 때 제네시스
                new Sequence(new List<Node> { teleportCoolDownCheck, teleport }, "teleportSequence")                            // 공격이 모두 쿨타임이면 teleport를 수행한다. (추후에 텔레포트 이외의 이동이 추가되면 수정하면 됨)
            }, "actionMutexTask");

            // 기믹 뮤텍스 테스크 생성
            // ※뮤텍스: 상호 배제(Mutual Exclusion) - 운영체제에서 배운 것
            MutexTask gimmickMutexTask = new MutexTask(new List<Node>
            {
                new Sequence(new List<Node> { gimmick70Check, defenseGimmick }, "방어 석상 기믹 시퀀스"), // 체력 70% 이하에서 방어 기믹
                new Sequence(new List<Node> { gimmick30Check, attackGimmick }, "공격 석상 기믹 시퀀스"),  // 체력 30% 이하에서 공격 기믹
                new Sequence(new List<Node> { finalGimmickCheck, purificationGimmick }, "정화 기믹 시퀀스") // 체력 0%에서 최종 기믹
            }, "gimmickMutexTask");

            /*Sequence enrageSequence = new Sequence(new List<Node>
            {
                enrageActivationCheck,  // 광폭화가 이미 진행되었는지 판단
                enrageTimeCheck,        // 광폭화 조건 판단
                enrageTask              // 광폭화 로직 실행
            }, "enrageSequence");*/

            // 메인 행동 트리 구성 (Parallel로 모든 상태를 병렬로 평가)
            return new Parallel(new List<Node>
            {
                //enrageSequence,  // 5분 경과 시 광폭화, 현재 광폭화 빼는걸로 기획 수정
                new ConditionalDecorator(
                    () => !actionMutexTask.IsRunning(), // 조건: actionMutexTask가 실행 중이 아닐 때만
                    gimmickMutexTask                    // 기믹 뮤텍스 테스크 실행
                ),
                actionMutexTask, // 행동 뮤텍스 테스크 수행
            });
        }

        // 기본 스킬 동작
        IEnumerator CastBasicAttack(Action<bool> onComplete)
        {
            Debug.Log("CastBasicAttack");
            animator.SetTrigger("Teleport");
            float animationThreshold = 0.9f; // 애니메이션이 끝났다고 판단할 기준 (90% 이상)

            // "Teleport" 애니메이션이 끝날 때까지 기다림
            yield return WaitForAnimationEnd("Teleport", animationThreshold);

            if (isBlinded)
            {
                // 엉뚱한 위치를 지정
                TeleportBoss(3f, 4f); // 보스를 텔레포트
            }
            else
            {
                TeleportBoss(1f, 1.5f); // 보스를 텔레포트
            }

            // "Teleport_arrive" 애니메이션이 끝날 때까지 기다림
            yield return WaitForAnimationEnd("Teleport_arrive", animationThreshold);

            animator.SetTrigger("Attack");
            isDirectionLocked = true;

            // "Attack" 애니메이션이 끝날 때까지 기다림
            yield return WaitForAnimationEnd("Attack", animationThreshold);

            isDirectionLocked = false;
            cooltimers[0] = cooldowns[0];
            onComplete?.Invoke(true); // 성공적으로 마쳤다고 판단
        }

        IEnumerator CastGridLaser(Action<bool> onComplete)
        {
            Debug.Log("CastGridLaser");
            animator.SetTrigger("Casting");
            animator.SetBool("Casting_Stay", true);
            isDirectionLocked = true;

            Vector2 center = targetObject.transform.position;

            if (isBlinded)
            {
                // 혼란 상태일 때 랜덤 오프셋 적용
                float randomOffsetX = UnityEngine.Random.Range(-2, 2); // X축 랜덤 오프셋
                float randomOffsetY = UnityEngine.Random.Range(-2, 2); // Y축 랜덤 오프셋

                center.x += randomOffsetX;
                center.y += randomOffsetY;
            }

            float root2 = MathF.Sqrt(2); // 루트2
            float laserWidth = 0.8f; // 레이저 폭, Boss Grid Laser 프리팹 참고, 플레이어 회피 공간 생각해서 여유롭게 잡을 것
            float squareDiagonallength = mapRadius * 2 * root2; // 맵을 사각형으로 봤을 때 대각선 길이
            float laserAbleNum = squareDiagonallength / laserWidth / 2; // 대각선 안에 레이저가 몇개 들어가냐 나누기 2

            float laserX = center.x - mapRadius; // 맵 왼쪽 끝
            float laserY = center.y + mapRadius; // 맵 위쪽 끝
            for (int i = 0; i < laserAbleNum; i++)
            {
                onBossTryGridLaserAttack(laserX, laserY, true);
                laserX += (laserWidth * 2) / root2;
                laserY -= (laserWidth * 2) / root2;
            }

            laserX = center.x - mapRadius; // 맵 왼쪽 끝
            laserY = center.y - mapRadius; // 맵 위쪽 끝
            for (int i = 0; i < laserAbleNum; i++)
            {
                onBossTryGridLaserAttack(laserX, laserY, false);
                laserX += (laserWidth * 2) / root2;
                laserY += (laserWidth * 2) / root2;
            }

            yield return new WaitForSeconds(4f); // 지정한 초 만큼 쉬기

            animator.SetBool("Casting_Stay", false);
            isDirectionLocked = false;
            cooltimers[2] = cooldowns[2];
            onComplete?.Invoke(true); // 성공적으로 마쳤다고 판단
        }

        IEnumerator CastLineLaser(Action<bool> onComplete)
        {
            Debug.Log("CastLineLaser");
            yield return new WaitForSeconds(0.2f); // 지정한 초 만큼 쉬기
            isDirectionLocked = true;

            onBossTryLaserAttack(0f);

            yield return new WaitForSeconds(3.2f); // 지정한 초 만큼 쉬기
            isDirectionLocked = false;
            cooltimers[1] = cooldowns[1];
            onComplete?.Invoke(true); // 성공적으로 마쳤다고 판단
        }

        IEnumerator CastTripleLineLaser(Action<bool> onComplete)
        {
            Debug.Log("CastTripleLineLaser");
            yield return new WaitForSeconds(0.2f); // 지정한 초 만큼 쉬기
            isDirectionLocked = true;

            onBossTryLaserAttack(30f);
            onBossTryLaserAttack(0f);
            onBossTryLaserAttack(-30f);

            yield return new WaitForSeconds(3.2f); // 지정한 초 만큼 쉬기
            isDirectionLocked = false;
            cooltimers[1] = cooldowns[1];
            onComplete?.Invoke(true); // 성공적으로 마쳤다고 판단
        }

        IEnumerator CastGenesis(Action<bool> onComplete)
        {
            Debug.Log("CastGenesis");
            animator.SetTrigger("Casting");
            isDirectionLocked = true;
            float animationThreshold = 0.9f; // 애니메이션이 끝났다고 판단할 기준 (90% 이상)

            // 애니메이션이 끝날 때까지 기다림
            yield return WaitForAnimationEnd("Casting", animationThreshold);

            animator.SetBool("Casting_Stay", true);

            yield return new WaitForSeconds(0.75f); // 지정한 초 만큼 쉬기

            onBossTryGenesisAttack();

            yield return new WaitForSeconds(2.8f); // 지정한 초 만큼 쉬기

            animator.SetBool("Casting_Stay", false);
            isDirectionLocked = false;
            cooltimers[3] = cooldowns[3];
            onComplete?.Invoke(true); // 성공적으로 마쳤다고 판단
        }

        IEnumerator CastTelePort(Vector2 pos)
        {
            Debug.Log("CastTelePort");
            animator.SetTrigger("Teleport");
            float animationThreshold = 0.9f; // 애니메이션이 끝났다고 판단할 기준 (90% 이상)

            // 애니메이션이 끝날 때까지 기다림
            yield return WaitForAnimationEnd("Teleport", animationThreshold);

            X = pos.x;
            Y = pos.y;

            col.isTrigger = true; // 텔레포트 중에는 충돌을 막기 위해 트리거로 설정
            yield return WaitForAnimationEnd("Teleport_arrive", animationThreshold); // 두 번째 애니메이션이 끝날 때까지 기다림
            col.isTrigger = false; // 충돌을 다시 활성화

            cooltimers[4] = cooldowns[4];
        }

        IEnumerator CastTelePort(Action<bool> onComplete)
        {
            Debug.Log("CastTelePort");
            animator.SetTrigger("Teleport");
            float animationThreshold = 0.9f; // 애니메이션이 끝났다고 판단할 기준 (90% 이상)

            // 애니메이션이 끝날 때까지 기다림
            yield return WaitForAnimationEnd("Teleport", animationThreshold);

            TeleportBoss(1.5f, 2f); // 보스를 텔레포트

            // 맵 밖으로 벗어났으면, 위치를 맵 경계 안으로 고정
            bool hasBossExitedMap = MathF.Sqrt(X * X + Y * Y) > mapRadius;
            if (hasBossExitedMap)
            {
                Vector2 position = transform.position;
                Vector2 direction = (position - Vector2.zero).normalized; // 여기서 Vector2.zero는 맵 중심 (변경되면 수정해야 함, 지금은 맵에서 오망성의 중심이 0,0)
                transform.position = Vector2.zero + direction * mapRadius;
            }

            col.isTrigger = true; // 텔레포트 중에는 충돌을 막기 위해 트리거로 설정
            yield return WaitForAnimationEnd("Teleport_arrive", animationThreshold); // 두 번째 애니메이션이 끝날 때까지 기다림
            col.isTrigger = false; // 충돌을 다시 활성화

            cooltimers[4] = cooldowns[4];
            onComplete?.Invoke(true); // 성공적으로 마쳤다고 판단
        }

        void TeleportBoss(float minRadius, float maxRadius)
        {
            // 랜덤 반지름 계산
            float randomRadius = UnityEngine.Random.Range(minRadius, maxRadius);

            Vector2 bossPosition = transform.position;

            // 현재 보스 위치를 맵 중심 기준으로 각도로 변환
            float currentAngle = Mathf.Atan2(bossPosition.y - mapCenter.y, bossPosition.x - mapCenter.x) * Mathf.Rad2Deg;

            // 정규 분포를 적용하여 새로운 각도 샘플링
            // 표준 편차 (σ) = 90일 때, 4분위 확률:
            // - 180도 ± 45도 (135도 ~ 225도) 확률 ~~ 27%
            // - 0도 ± 45도 (315도 ~ 45도) 확률 ~~ 9%
            // - 90도 ± 45도 (225도 ~ 315도) 확률 ~~ 24%
            // - 270도 ± 45도 (45도 ~ 135도) 확률 ~~ 24%
            float sigma = 90f; // 정규 분포 표준 편차 90 사용
            float calculatedAngle = GetGaussianAngle(currentAngle, sigma); // 각도 계산

            // 새로운 위치 계산
            Vector2 newPosition = (Vector2)targetObject.transform.position
                + new Vector2(Mathf.Cos(calculatedAngle * Mathf.Deg2Rad), Mathf.Sin(calculatedAngle * Mathf.Deg2Rad)) * randomRadius;

            // 보스 위치 이동
            transform.position = newPosition;

            // 맵 밖으로 벗어났으면, 위치를 맵 경계 안으로 고정
            bossPosition = transform.position;
            float sqrDistanceFromCenter = (bossPosition - mapCenter).sqrMagnitude;

            if (sqrDistanceFromCenter > mapRadius * mapRadius)
            {
                // 보스가 맵 밖으로 나갔으면, 맵 경계선으로 이동
                Vector2 direction = (bossPosition - mapCenter).normalized;
                transform.position = mapCenter + direction * mapRadius;
            }
        }

        // 정규 분포를 이용해 각도를 반환해주는 함수
        float GetGaussianAngle(float currentAngle, float sigma)
        {
            // Box-Muller 변환을 이용해 정규 분포 난수 생성
            double u1 = 1.0 - random.NextDouble(); // random.NextDouble()은 0.0 ~ 1.0 사이의 난수를 반환하는데,
            double u2 = 1.0 - random.NextDouble(); // Math.Log()는 0을 입력받으면 오류가 나기 때문에, 0을 피하기 위해 1에서 뺌.
            double randStdNormal = Math.Sqrt(-2.0 * Math.Log(u1)) * Math.Sin(2.0 * Math.PI * u2);

            // 정규 분포 값 적용 (currentAngle + 180도를 중심으로 샘플링)
            float oppositeAngle = (currentAngle + 180) % 360;
            float newAngle = oppositeAngle + (float)(sigma * randStdNormal);

            // 각도를 0~360도 사이로 정리
            return (newAngle + 360) % 360;
        }

        // 기믹 동작
        IEnumerator DefenseGimmickCoroutine()
        {
            Debug.Log("DefenseGimmick");
            yield return CastTelePort(new Vector2(0, 0)); // 중앙으로 텔레포트

            // 보스 쉴드 생성
            bossShield = Instantiate(bossShieldPrefab);
            bossShield.transform.SetParent(transform, false);

            /* 방어의 석상 기믹 구현 */
            isDefenceGimmickRunning = true;
            bossGimmickController.DefenceGimmickStart();

            yield return new WaitWhile(() => isDefenceGimmickRunning);

            if (damageReduction <= 0) // 기본 뎀감이 있다면 기준을 해당 수치로 바꿔줘야 함
            {
                Destroy(bossShield);
            }
            Debug.Log("DefenseGimmick Finished");
        }

        IEnumerator AttackGimmickCoroutine()
        {
            Debug.Log("AttackGimmick");
            yield return CastTelePort(new Vector2(0, 0)); // 중앙으로 텔레포트

            // 보스 불꽃 생성
            bossFlame = Instantiate(bossFlamePrefab);
            bossFlame.transform.SetParent(transform, false);

            /* 공격의 석상 기믹 구현 */
            isAttackGimmickRunning = true;
            bossGimmickController.AttackGimmickStart();

            yield return new WaitWhile(() => isAttackGimmickRunning);

            Destroy(bossFlame);
            Debug.Log("AttackGimmick Finished");
        }

        IEnumerator PurificationGimmickCoroutine()
        {
            Debug.Log("PurificationGimmick");
            yield return CastTelePort(new Vector2(0, 0)); // 중앙으로 텔레포트

            /* 정화의 석상 기믹 구현 */
            isPurificationGimmickRunning = true;
            bossGimmickController.PurificationGimmickStart();

            yield return new WaitWhile(() => isPurificationGimmickRunning);

            Debug.Log("PurificationGimmick Finished");
        }

        public void OnPurificationGimmickSuccessed()
        {
            isDead = true;
            StartCoroutine(Die());
        }

        public void OnPurificationGimmickFailed(float purificationGimmickFailedHpRestoreCoefficient)
        {
            hp = maxHp * purificationGimmickFailedHpRestoreCoefficient;

            damage *= enrage_DamageIncrementCoefficient;
            onBossChangeDamages(enrage_DamageIncrementCoefficient);
            for (int i = 0; i < cooldowns.Count; i++)
            {
                cooldowns[i] /= enrage_CoolDownDecrementCoefficient;
            }

            shouldRetryPurificationGimmick = true;
        }

        // 광폭화 동작
        /*protected void Enrage()
        {
            Debug.Log("Enrage");
            *//* 광폭화 구현, 체력 회복, 데미지 2배, 쿨타임 절반 *//*
            hp += MathF.Ceiling(maxHp * 0.3f);
            damage *= enrage_DamageIncrementCoefficient;
            onBossChangeDamages(enrage_DamageIncrementCoefficient);
            for (int i = 0; i < cooldowns.Count; i++)
            {
                cooldowns[i] /= enrage_CoolDownDecrementCoefficient;
            }

            enrageAlreadyActivated = true;
        }*/

        // IDamageable
        public override void TakeDamage(string causerTag, float damage, bool isCritical = false, float knockbackForce = 0)
        {
            base.TakeDamage(causerTag, damage, isCritical, knockbackForce);

            float reduedDamage = damage * (1f - damageReduction); // 데미지 감소량만큼 데미지를 덜 입음 (현재 기본 뎀감은 0%)

            // 기믹 중 체력 제한 (방어 석상 기믹: 50%, 공격 석상 기믹: 20%)
            bool isDefenceGimmickHpLimited = isDefenceGimmickRunning && hp <= (maxHp * 0.5f);
            bool isAttackGimmickHpLimited = isAttackGimmickRunning && hp <= (maxHp * 0.2f);
            if (isDefenceGimmickHpLimited || isAttackGimmickHpLimited)
            {
                reduedDamage = 0;
                InGameTextManager.Instance.ShowText("무적", "Default", isCritical, transform.position);
                return;
            }

            hp -= reduedDamage;
            if (hp <= 0) hp = 0;
            InGameTextManager.Instance.ShowText(Mathf.RoundToInt(reduedDamage).ToString(), causerTag, isCritical, transform.position);

            HandleDamageReaction(); // 데미지 받을 시 행동 처리
        }

        private void HandleDamageReaction()
        {
            if (hitAnimationDelay <= hitAnimationDelayTimer)
            {
                animator.SetTrigger("Hit");
                hitAnimationDelayTimer = 0;
            }
        }

        IEnumerator Die()
        {
            Debug.Log("중앙으로 텔포 시작");
            yield return CastTelePort(new Vector2(0, 0)); // 중앙으로 텔레포트
            Debug.Log("중앙으로 텔포 완");
            animator.SetTrigger("DeadLoop");
            isDirectionLocked = true;

            rigid.constraints = RigidbodyConstraints2D.FreezeAll;
            col.enabled = false;

            yield return new WaitForSeconds(2f); // 지정한 초 만큼 쉬기

            animator.SetTrigger("Dead");
            yield return new WaitForSeconds(0.5f); // 지정한 초 만큼 쉬기

            Destroy(gameObject);
            onBossDead();
        }

        protected override IEnumerator SlowDownRoutine(float duration)
        {
            float originalSpeed = animator.speed; // 원래 애니메이션 속도 저장
            animator.speed = originalSpeed * slowDownCoefficient; // 애니메이션 속도 감소
            transform.GetChild(1).gameObject.SetActive(true);

            yield return new WaitForSeconds(duration);

            animator.speed = originalSpeed; // 원래 속도로 복구
            transform.GetChild(1).gameObject.SetActive(false);

            slowDownCoroutine = null; // 코루틴 종료
        }

        //==================================================================
        // EnableAttack 메서드 (애니메이션 이벤트에서 호출됨)
        public void EnableAttack()
        {
            onBossTryBasicAttack();
        }

        public void OnTeleportStart()
        {
            // 1.25는 애니메이션 속도 계수, 0.9f는 animationThreshold 값
            float frameTime = 5f * AnimationConstants.FrameTime / 1.25f * 0.9f;
            bossGimmickController.FadeOutStatueLines(frameTime); // 텔레포트 시작 시 석상 선 제거
        }

        public void OnTeleportEnd()
        {
            // 1.25는 애니메이션 속도 계수, 0.9f는 animationThreshold 값
            float frameTime = 5f * AnimationConstants.FrameTime / 1.25f * 0.9f;
            bossGimmickController.FadeInStatueLines(frameTime); // 텔레포트 끝나면 석상 선 다시 표시
        }
    }
}
