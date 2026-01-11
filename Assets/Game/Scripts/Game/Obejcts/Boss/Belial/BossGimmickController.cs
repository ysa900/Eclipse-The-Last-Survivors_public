using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Eclipse.Game
{
    public class BossGimmickController : MonoBehaviour
    {
        // ======================================================
        // 보스 
        Belial boss;

        // ======================================================
        // UI, BossManager가 할당
        public GUIManager guiManager;

        public float gimmickTimer;

        // ======================================================
        // 맵 및 석상
        int statueNum = 12; // 석상 개수
        List<GameObject> statuePrefab = new List<GameObject>(); // 석상 프리팹들
        GameObject statueObject; // 석상들 캐싱할 게임오브젝트
        GameObject hpBarPrefab; // 석상 HP Bar 프리팹

        List<GameObject> statues = new List<GameObject>(12); // 석상들

        // ======================================================
        // 보스 방어 석상 기믹
        float defenceGimmick_DamageReductionValue = 0.8f; // 기믹 시간 동안 뎀감 80%
        float defenceGimmick_DamageReduction_ReduceValue = 0.2f; // 석상 하나 파괴할때마다 뎀감이 20%씩 줄어듬

        [SerializeField][ReadOnly] int defenceGimmick_StatueCount = 3;
        [SerializeField][ReadOnly] List<DefenceStatue> defenceStatues = new List<DefenceStatue>();

        // ======================================================
        // 보스 공격 석상 기믹
        float attackGimmick_damageIncrementCoefficient = 2f; // 기믹 동안 공격력 2배

        [SerializeField][ReadOnly] int attackGimmick_StatueCount = 3;
        [SerializeField][ReadOnly] List<AttackStatue> attackStatues = new List<AttackStatue>();
        List<AttackStatueLineRenderer> attackStatueLineRenderers = new List<AttackStatueLineRenderer>();

        // ======================================================
        // 보스 정화 석상 기믹
        float purificationGimmick_time = 120f;
        float purificationGimmick_FailedHpRestoreCoefficient = 0.5f;
        float purificationGimmick_FailureEffectTime = 1.5f;
        int purificationGimmick_StatueNum = 6;

        int purificationGimmick_StatueCount = 3;
        List<PurificationStatue> purificationStatues = new List<PurificationStatue>();
        List<PurificationStatueLineRenderer> purificationStatueLineRenderers = new List<PurificationStatueLineRenderer>();
        List<PurificationStatueLineRenderer> triangleGroup = new List<PurificationStatueLineRenderer>();
        List<PurificationStatueLineRenderer> invertedTriangleGroup = new List<PurificationStatueLineRenderer>();

        // ======================================================
        // BossManager로 가는 Action
        public Action<IEnumerable<Enemy>> onStatueChange;

        // ======================================================

        private void Awake()
        {
            // 초기화
            boss = GetComponent<Belial>();
            statuePrefab.Add(Resources.Load<GameObject>("Prefabs/Map Objects/Stage3/The Piper"));
            statuePrefab.Add(Resources.Load<GameObject>("Prefabs/Map Objects/Stage3/The Angel"));
            hpBarPrefab = Resources.Load<GameObject>("Prefabs/Map Objects/Stage3/StatueHpSlider");
        }

        private void Start()
        {
            statueObject = new GameObject("Pipers");
            // 석상들 생성
            for (int i = 0; i < statueNum; i++)
            {
                // 석상 위치 계산
                float angle = 2 * Mathf.PI * i / statueNum;

                Vector3 position = new Vector3(
                    (boss.mapRadius * 0.8f) * Mathf.Cos(angle), // * 0.8f를 해주는 이유는 석상을 안쪽으로 넣어주기 위함 
                    (boss.mapRadius * 0.8f) * Mathf.Sin(angle), 
                    0);

                // 석상 생성
                GameObject statue = Instantiate(statuePrefab[0]);
                statue.transform.position = position;
                statues.Add(statue);
                statue.transform.SetParent(statueObject.transform, false);
            }
        }

        private void Update()
        {
            //if (boss.isDefenceGimmickRunning) DefenceGimmick();
            //if (boss.isAttackGimmickRunning) AttackGimmick();
            if (boss.isPurificationGimmickRunning) PurificationGimmick();
        }

        // ======================================================
        // 방어 석상 기믹
        public void DefenceGimmickStart()
        {
            defenceGimmick_StatueCount = 3;
            // 보스 보호막 뎀감 적용
            boss.damageReduction = defenceGimmick_DamageReductionValue;

            // 석상 3개 선택해서 방어 석상으로 변환
            for(int i = 0; i < statueNum; i += 4)
            {
                // 석상 선택
                int ranNum = UnityEngine.Random.Range(i, i + 4); // i ~ i + 3 중 선택

                // 방어 석상으로 만들기
                DefenceStatue defenceStatue = statues[ranNum].AddComponent<DefenceStatue>();
                string lineRendererObjectName = "LineRenderer";
                GameObject lineRendererObject = new GameObject(lineRendererObjectName);
                lineRendererObject.transform.SetParent(statues[ranNum].transform, false);
                DefenceStatueLineRenderer defenceLineRenderer = lineRendererObject.AddComponent<DefenceStatueLineRenderer>();

                // 클래스 컴포넌트 초기 설정
                defenceStatue.onDestroied = OnDefenceStatueDestoried;
                defenceStatue.lineRendererObjectName = lineRendererObjectName;
                defenceLineRenderer.target = boss.gameObject;

                defenceStatues.Add(defenceStatue); // 리스트에 석상 추가
                
                // 석상 체력바 생성
                GameObject hpBar = Instantiate(hpBarPrefab, guiManager.panelViewer.transform); // hpBar를 panelViewer의 자식으로 생성

                Panels.EnemyHPSlider hpSlider = hpBar.GetComponent<Panels.EnemyHPSlider>();
                hpSlider.enemy = defenceStatue;
                defenceStatue.hpBar = hpBar;




                //defenceStatue.Die();
            }
            onStatueChange(defenceStatues); // 보스 매니저에 석상 전달

            ShowBossMessage(0); // 기믹 보스 메세지 표시
            AudioManager.instance.PlayBgm(AudioManager.Bgm.Boss_1Phase);
        }

        void DefenceGimmick()
        {
            if (gimmickTimer > 0)
            {
                gimmickTimer -= Time.deltaTime;
            }
            else if (gimmickTimer < 0)
            {
                gimmickTimer = 0;
                DefenceGimmickFailed();
            }
        }

        public void DefenceGimmickEnd()
        {
            boss.isDefenceGimmickRunning = false;
        }

        void DefenceGimmickFailed()
        {
            boss.shouldRetryDefenceGimmick = false;
            DefenceGimmickEnd();

            // ToList로 새로운 리스트를 사용하여야 함
            // 이유는 foreach가 순회 중인 리스트를 변경하면, 리스트의 내부 상태(특히 요소의 위치와 개수)가 변경되기 때문에,
            // IEnumerator는 예상치 못한 상태에 빠지게 됨 -> InvalidOperationException 발생
            foreach (var defenceStatue in defenceStatues.ToList())
            {
                defenceStatue.DestroyComponents();
                defenceStatues.Remove(defenceStatue);
            }
            onStatueChange(defenceStatues);
        }

        void OnDefenceStatueDestoried(Statue defenceStatue)
        {
            boss.damageReduction -= defenceGimmick_DamageReduction_ReduceValue;
            defenceGimmick_StatueCount--;
            defenceStatues.Remove(defenceStatue as DefenceStatue);
            Debug.Log(defenceGimmick_StatueCount);
            if(defenceGimmick_StatueCount <= 0 )
            {
                boss.damageReduction = 0; // 초기 뎀감 수치가 있다면 그걸로 적용하게 바꿔야 함
                boss.shouldRetryDefenceGimmick = false;
                DefenceGimmickEnd();
            }

            onStatueChange(defenceStatues); // 보스 매니저에 석상 전달
        }

        // ======================================================
        // 공격 석상 기믹
        public void AttackGimmickStart()
        {
            attackGimmick_StatueCount = 3;
            boss.damage *= attackGimmick_damageIncrementCoefficient;
            boss.onBossChangeDamages(attackGimmick_damageIncrementCoefficient);

            // 석상 3개 선택해서 공격 석상으로 변환
            for (int i = 0; i < statueNum; i += 4)
            {
                // 석상 선택
                int ranNum = UnityEngine.Random.Range(i, i + 4); // i ~ i + 3 중 선택

                // 공격 석상으로 만들기
                AttackStatue attackStatue = statues[ranNum].AddComponent<AttackStatue>();
                string lineRendererObjectName = "LineRenderer";
                GameObject lineRendererObject = new GameObject(lineRendererObjectName);
                lineRendererObject.transform.SetParent(statues[ranNum].transform, false);
                AttackStatueLineRenderer attackLineRenderer = lineRendererObject.AddComponent<AttackStatueLineRenderer>();

                // 클래스 컴포넌트 초기 설정
                attackStatue.onDestroied = OnAttackStatueDestoried;
                attackStatue.lineRendererObjectName = lineRendererObjectName;
                attackLineRenderer.target = boss.gameObject;

                attackStatues.Add(attackStatue); // 리스트에 석상 추가
                attackStatueLineRenderers.Add(attackLineRenderer); // 라인 렌더러도 리스트에 추가

                // 석상 체력바 생성
                GameObject hpBar = Instantiate(hpBarPrefab, guiManager.panelViewer.transform); // hpBar를 panelViewer의 자식으로 생성

                Panels.EnemyHPSlider hpSlider = hpBar.GetComponent<Panels.EnemyHPSlider>();
                hpSlider.enemy = attackStatue;
                attackStatue.hpBar = hpBar;




                //attackStatue.Die();
            }
            onStatueChange(attackStatues); // 보스 매니저에 석상 전달

            ShowBossMessage(1); // 기믹 보스 메세지 표시
            AudioManager.instance.PlayBgm(AudioManager.Bgm.Boss_2Phase);
        }

        void AttackGimmick()
        {

        }

        public void AttackGimmickEnd()
        {
            boss.isAttackGimmickRunning = false;
        }

        void OnAttackStatueDestoried(Statue attackStatue)
        {
            attackGimmick_StatueCount--;
            attackStatues.Remove(attackStatue as AttackStatue);

            if (attackGimmick_StatueCount <= 0)
            {
                boss.damage /= attackGimmick_damageIncrementCoefficient;
                boss.onBossChangeDamages(1f / attackGimmick_damageIncrementCoefficient);
                boss.shouldRetryAttackGimmick = false;
                attackStatueLineRenderers.Clear();
                AttackGimmickEnd();
            }

            onStatueChange(attackStatues); // 보스 매니저에 석상 전달
        }

        public void FadeOutStatueLines(float frameTime)
        {
            if (attackStatueLineRenderers.Count == 0) return; // 라인 렌더러가 없으면 실행하지 않음

            foreach (AttackStatueLineRenderer lineRenderer in attackStatueLineRenderers)
            {
                // Fade out (사라지기)
                if (lineRenderer != null) // 라인 렌더러가 null이 아닐 때만 실행
                { 
                    StartCoroutine(lineRenderer.FadeLineAlpha(1f, 0f, frameTime));
                }
            }
        }

        public void FadeInStatueLines(float frameTime)
        {
            if (attackStatueLineRenderers.Count == 0) return; // 라인 렌더러가 없으면 실행하지 않음

            foreach (AttackStatueLineRenderer lineRenderer in attackStatueLineRenderers)
            {
                // Fade in (등장)
                if (lineRenderer != null) // 라인 렌더러가 null이 아닐 때만 실행
                {
                    StartCoroutine(lineRenderer.FadeLineAlpha(0f, 1f, frameTime));
                }
            }
        }

        // ======================================================
        // 정화 기믹
        public void PurificationGimmickStart()
        {
            Debug.Log("정화 기믹 시작");
            purificationGimmick_StatueCount = 3;
            // 기존 석상들 비활성화
            ControllStatuesActivation(false);

            List<StatueFButtonObject> interactionButtonObjectPrefab = new List<StatueFButtonObject>();
            interactionButtonObjectPrefab.Add(Resources.Load<StatueFButtonObject>("Prefabs/Map Objects/Stage3/FButtonObject_Piper"));
            interactionButtonObjectPrefab.Add(Resources.Load<StatueFButtonObject>("Prefabs/Map Objects/Stage3/FButtonObject_Angle"));

            // 석상들 생성
            for (int i = 0; i < purificationGimmick_StatueNum; i++)
            {
                // 석상 위치 계산
                float angle = 2 * Mathf.PI * i / purificationGimmick_StatueNum + Mathf.PI / 2; // + Mathf.PI / 2로 90도 회전
                Vector3 position = new Vector3(
                    (boss.mapRadius * 0.7f) * Mathf.Cos(angle), // * 0.7f를 해주는 이유는 석상을 안쪽으로 넣어주기 위함
                    (boss.mapRadius * 0.7f) * Mathf.Sin(angle),
                    0);

                // 석상 생성
                GameObject statue = Instantiate(statuePrefab[1 - i % 2]);
                statue.transform.position = position;
                statue.transform.SetParent(statueObject.transform, false);

                // 정화 석상으로 만들기
                PurificationStatue purificationStatue = statue.AddComponent<PurificationStatue>();
                string lineRendererObjectName = "LineRenderer";

                GameObject lineRendererObject = new GameObject(lineRendererObjectName);
                lineRendererObject.transform.SetParent(statue.transform, false);
                PurificationStatueLineRenderer purificationLineRenderer = lineRendererObject.AddComponent<PurificationStatueLineRenderer>();
                purificationLineRenderer.getGimmickTimer = () => gimmickTimer;
                purificationLineRenderer.failureEffectTime = purificationGimmick_FailureEffectTime;

                StatueFButtonObject InteractionButtonObject = Instantiate(interactionButtonObjectPrefab[1 - i % 2]);
                InteractionButtonObject.purificationStatue = purificationStatue;
                InteractionButtonObject.GetComponent<SpriteRenderer>().sortingOrder = statue.GetComponent<SpriteRenderer>().sortingOrder;
                InteractionButtonObject.transform.SetParent(statue.transform, false);

                purificationStatues.Add(purificationStatue);
                purificationStatueLineRenderers.Add(purificationLineRenderer);

                // 삼각형 그룹 분류
                if (i % 2 == 0)
                {
                    triangleGroup.Add(purificationLineRenderer);
                }
                else
                {
                    invertedTriangleGroup.Add(purificationLineRenderer);
                }

                // 클래스 컴포넌트 초기 설정
                if (i % 2 == 1)
                {
                    purificationStatue.onStatuePlacedInside = OnPurificationStatuePlacedCorretly;
                    purificationStatue.onStatuePlacedOutside = OnPurificationStatuePlacedWrongly;
                }
                else
                {
                    purificationStatue.onStatuePlacedInside = OnPurificationStatuePlacedWrongly;
                    purificationStatue.onStatuePlacedOutside = OnPurificationStatuePlacedCorretly;
                }
            }

            // Source와 Target 설정: 정삼각형 그룹
            for (int i = 0; i < triangleGroup.Count; i++)
            {
                PurificationStatueLineRenderer current = triangleGroup[i];
                PurificationStatueLineRenderer next = triangleGroup[(i + 1) % triangleGroup.Count]; // 반시계 방향으로 연결
                current.target = next.GetComponentInParent<PurificationStatue>().gameObject;
                PurificationStatue statue = current.GetComponentInParent<PurificationStatue>();
                statue.isStatueInInvertedTriangleGroup = false;
                statue.movePositions[1] = CalculateMovePositionForTriangleGroup(i);
            }

            // Source와 Target 설정: 역삼각형 그룹
            for (int i = 0; i < invertedTriangleGroup.Count; i++)
            {
                PurificationStatueLineRenderer current = invertedTriangleGroup[i];
                PurificationStatueLineRenderer next = invertedTriangleGroup[(i + 1) % invertedTriangleGroup.Count]; // 반시계 방향으로 연결
                current.target = next.GetComponentInParent<PurificationStatue>().gameObject;
                PurificationStatue statue = current.GetComponentInParent<PurificationStatue>();
                statue.isStatueInInvertedTriangleGroup = true;
                statue.movePositions[1] = CalculateMovePositionForInvertedTriangleGroup(i);
            }

            Vector2 CalculateMovePositionForTriangleGroup(int i)
            {
                Vector2 startPosition = invertedTriangleGroup[(i + invertedTriangleGroup.Count - 1) % invertedTriangleGroup.Count].GetComponentInParent<PurificationStatue>().transform.position;
                Vector2 endPosition = invertedTriangleGroup[i % invertedTriangleGroup.Count].GetComponentInParent<PurificationStatue>().transform.position;
                return (startPosition + endPosition) / 2;
            }
            Vector2 CalculateMovePositionForInvertedTriangleGroup(int i)
            {
                Vector2 startPosition = triangleGroup[i % triangleGroup.Count].GetComponentInParent<PurificationStatue>().transform.position;
                Vector2 endPosition = triangleGroup[(i + 1) % triangleGroup.Count].GetComponentInParent<PurificationStatue>().transform.position;
                return (startPosition + endPosition) / 2;
            }

            // 초기화가 끝난 뒤 Update문을 실행하도록 보장하기 위함
            foreach (PurificationStatueLineRenderer statueLineRenderer in purificationStatueLineRenderers)
            {
                statueLineRenderer.isInitialized = true;
            }

            ShowBossMessage(2); // 기믹 보스 메세지 표시
            AudioManager.instance.PlayBgm(AudioManager.Bgm.Boss_3Phase);
            gimmickTimer = purificationGimmick_time;
            ActivateGimmickTimerText(purificationGimmick_time, "부활까지 남은 시간");
        }

        void PurificationGimmick()
        {
            if (gimmickTimer > 0)
            {
                gimmickTimer -= Time.deltaTime;
            }
            else if(gimmickTimer < 0)
            {
                gimmickTimer = 0;
                StartCoroutine(PurificationGimmickFailed());
            }
        }

        public void PurificationGimmickEnd()
        {
            boss.isPurificationGimmickRunning = false;
            DeActivateGimmickTimer();
        }

        void PurificationGimmickSuccess()
        {
            boss.isPurificationGimmickSuccessed = true;
            PurificationGimmickEnd();
            boss.OnPurificationGimmickSuccessed();
            ShowBossMessage(3);
        }

        IEnumerator PurificationGimmickFailed()
        {
            float waitTime = 0.4f; // 이펙트 완료되고 잠깐 대기하는 시간
            yield return new WaitForSeconds(purificationGimmick_FailureEffectTime + waitTime); 

            PurificationGimmickEnd();
            boss.OnPurificationGimmickFailed(purificationGimmick_FailedHpRestoreCoefficient);
            ResetPurificationStatuesToNormalStatues();
            ShowBossMessage(4);
        }

        void OnPurificationStatuePlacedCorretly()
        {
            purificationGimmick_StatueCount--;

            if (purificationGimmick_StatueCount <= 0)
            {
                boss.shouldRetryPurificationGimmick = false;
                PurificationGimmickSuccess();
            }
        }
        void OnPurificationStatuePlacedWrongly()
        {
            purificationGimmick_StatueCount++;
            Debug.Log(purificationGimmick_StatueCount);
        }

        void ResetPurificationStatuesToNormalStatues()
        {
            foreach (PurificationStatue statue in purificationStatues)
            {
                Destroy(statue.gameObject);
            }
            purificationStatues.Clear();
            purificationStatueLineRenderers.Clear();
            triangleGroup.Clear();
            invertedTriangleGroup.Clear();

            // 기존 석상들 활성화
            ControllStatuesActivation(true);
        }

        // ======================================================
        void ShowBossMessage(int messageNum)
        {
            guiManager.bossMessage.gameObject.SetActive(true);
            guiManager.bossMessage.ChangeGimmickMessage(messageNum);
        }

        void ActivateGimmickTimerText(float time, string timerText)
        {
            guiManager.patternTimer.gameObject.SetActive(true);
            guiManager.patternTimer.Text = timerText;
            guiManager.patternTimer.BossGimmickController = this;
            guiManager.timer.gameObject.SetActive(false);
        }

        void DeActivateGimmickTimer()
        {
            guiManager.patternTimer.gameObject.SetActive(false);
            guiManager.timer.gameObject.SetActive(true);
        }

        void ControllStatuesActivation(bool hasActivated)
        {
            foreach (GameObject gameObject in statues)
            {
                gameObject.SetActive(hasActivated);
            }
        }

        // ======================================================
    }
}