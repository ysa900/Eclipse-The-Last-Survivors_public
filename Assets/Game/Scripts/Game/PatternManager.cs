using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace Eclipse.Game
{
    public class PatternManager : Eclipse.Manager
    {
        // 사용할 매니저들
        MinionManager minionManager;
        StageManager stageManager;
        RewardManager rewardManager;
        GUIManager gui;

        Light2D playerLight2D;
        Light2D stageLight2D;

        Vector3 prePlayerPosition;
        Vector3 curPlayerPosition;
        Vector3 playerVector;
        Vector3 cumulativeVector;

        int Direction;
        int countIndex;
        int batNum;
        int messageNum;

        string sceneName;

        float angleDegree;
        float weight;
        float CoolTime;
        float CoolTimer;
        float ObstacleCoolTime1;
        [SerializeField] float ObstacleCoolTimer1;
        float ObstacleCoolTime2;
        [SerializeField] float ObstacleCoolTimer2;

        public Obstacle_Setting posionSwamp;
        public Obstacle_Setting obstacle;

        //================================================
        // 박쥐 디버프 상자
        // 시야 디버프 확인 변수
        public bool isChangeVisible = false;
        float gameTime;
        public float patternTimer;

        float changeColorSpeed = 5f;
        Color changeColor;

        Panels.Timer timerObject;
        Panels.PatternTimer patternTimerObject;
        Panels.BossMessage bossMessageObject;

        float patternCoolTime;
        public float patternCoolTimer;
        float debuffTime;
        float debuffTimer;
        float laserPatternCoolTime;
        float laserPatternCoolTimer;

        PatternLaser laserPattern;
        public PatternLaser laserPatternPrefab;

        //==================================================================
        public bool isStage2PatternActivated;

        //==================================================================

        // Start is called before the first frame update
        void Start()
        {
            //==================================================================
            minionManager = client.GetManager<MinionManager>();
            stageManager = client.GetManager<StageManager>();
            rewardManager = client.GetManager<RewardManager>();
            gui = client.GetManager<GUIManager>();

            //==================================================================
            playerLight2D = PlayerManager.player.GetComponentInChildren<Light2D>();
            stageLight2D = GameObject.Find("Light 2D").GetComponent<Light2D>();

            //==================================================================
            // 보스 패턴
            timerObject = gui.timer;
            patternTimerObject = gui.patternTimer;
            bossMessageObject = gui.bossMessage;

            sceneName = SceneManager.GetActiveScene().name;

            //==================================================================
            messageNum = 0;
            weight = 1f;
            prePlayerPosition = Vector3.zero;
            curPlayerPosition = Vector3.zero;
            playerVector = Vector3.zero;
            cumulativeVector = Vector3.zero;
            angleDegree = 0f;
            batNum = 20;

            Direction = 0;
            countIndex = 0;

            CoolTime = 2f;
            CoolTimer = 0f;
            patternCoolTime = 2f;
            patternCoolTimer = 5f;

            ObstacleCoolTime1 = 120f;
            ObstacleCoolTimer1 = 0f;
            ObstacleCoolTime2 = 72f;
            ObstacleCoolTimer2 = 0f;

            debuffTime = 1f;
            debuffTimer = 0f;
            laserPatternCoolTime = 72f;
            laserPatternCoolTimer = 18f;
            //==================================================================
        }

        void Update()
        {
            if (sceneName == "Stage1")
            {
                curPlayerPosition = PlayerManager.player.transform.position;

                CoolTimer += Time.deltaTime;
                ObstacleCoolTimer1 += Time.deltaTime;
                ObstacleCoolTimer2 += Time.deltaTime;

                playerVector = curPlayerPosition - prePlayerPosition;

                CalculateVector(playerVector);
                weight += 0.1f;

                if (CoolTimer >= CoolTime)
                {
                    prePlayerPosition = curPlayerPosition;
                    CoolTimer = 0f;
                }
                // 정규 스테이지 시간의 경우에만 적용
                if (!stageManager.isStageRegularTimeOver)
                {
                    // 장애물 석상 패턴
                    if (ObstacleCoolTimer1 >= ObstacleCoolTime1)
                    {
                        for (int i = 0; i < 20; i++)
                        {
                            CreateObstacle(Direction, 0);
                        }

                        ObstacleCoolTimer1 = 0f;
                        ObstacleCoolTime1 = 10f;
                    }
                    // 몬스터 강화 패턴
                    if (ObstacleCoolTimer2 >= ObstacleCoolTime2)
                    {
                        CreateObstacle(Direction, 2);

                        ObstacleCoolTimer2 = 0f;
                    }

                }
            }
            else if (sceneName == "Stage2")
            {
                if (isStage2PatternActivated)
                {
                    patternCoolTimer += Time.deltaTime;
                    timerObject.gameObject.SetActive(false);
                    patternTimerObject.gameObject.SetActive(true);
                    patternTimerObject.Text = "살아남으세요";
                    patternTimerObject.PatternManager = this;

                    patternTimer -= Time.deltaTime;

                    if (patternCoolTimer >= patternCoolTime)
                    {
                        SetMovePatternEnemy();

                        patternCoolTimer = 0f;
                    }
                }
                else
                {
                    patternTimer = 30f;
                    timerObject.gameObject.SetActive(true);
                    patternTimerObject.gameObject.SetActive(false);
                    playerLight2D.enabled = false;
                    stageLight2D.color = Color.white;
                }

                if (patternTimer <= 0f)
                {
                    isStage2PatternActivated = false;

                    for (int i = 0; i < 60; i++)
                    {
                        rewardManager.ExpSpawn_By_DebuffBox(2, 10);
                        rewardManager.CoinSpawn_By_DebuffBox(1, UnityEngine.Random.Range(30, 50));
                    }

                    for (int i = 0; i < 10; i++)
                        rewardManager.Small_HP_PotionSpawn_By_DebuffBox(2);
                }

            }
            else if (sceneName == "Stage3")
            {
                laserPatternCoolTimer += Time.deltaTime;

                if (!stageManager.isStageRegularTimeOver)
                    if (laserPatternCoolTimer >= laserPatternCoolTime)
                    {
                        bossMessageObject.gameObject.SetActive(true);
                        bossMessageObject.ChangePatternMessage(messageNum++);
                        StartCoroutine(LaserDelay());
                        laserPatternCoolTimer = 0f;
                    }
            }

            if (isChangeVisible)
            {
                changeColor.g = Mathf.Lerp(changeColor.g, 0.05f, Time.deltaTime * changeColorSpeed);
                changeColor.b = Mathf.Lerp(changeColor.b, 0.05f, Time.deltaTime * changeColorSpeed);
                changeColor.r = Mathf.Lerp(changeColor.r, 0.05f, Time.deltaTime * changeColorSpeed);
                stageLight2D.color = changeColor;
                debuffTimer += Time.deltaTime;
            }

            if (debuffTimer >= debuffTime)
            {
                playerLight2D.enabled = false;
                changeColor = Color.white;
                stageLight2D.color = Color.white;
                isChangeVisible = false;
                debuffTimer = 0;
            }

        }

        private void LaserPattern()
        {
            laserPattern = Instantiate(laserPatternPrefab);
            laserPattern.transform.position = PlayerManager.player.transform.position;

        }

        private void OnChangeDebuffVisible()
        {
            playerLight2D.enabled = true;
            isChangeVisible = true;
            debuffTimer = 0f;
        }

        private void SetMovePatternEnemy()
        {
            PatternMinion enemy;

            int batDirection = UnityEngine.Random.Range(1, 9);
            int radius = 4;
            float degree = 360 * batDirection / 8;
            float tmpX = (float)Math.Cos(degree * Mathf.Deg2Rad) * radius;
            float tmpY = (float)Math.Sin(degree * Mathf.Deg2Rad) * radius;
            tmpX += PlayerManager.player.transform.position.x;
            tmpY += PlayerManager.player.transform.position.y;

            float ranX = UnityEngine.Random.Range(-1.6f, 1.6f);
            float ranY = UnityEngine.Random.Range(-1.6f, 1.6f);
            for (int i = 0; i < batNum; i++)
            {
                enemy = PoolManager.instance.GetMinion(13) as PatternMinion;
                
                enemy.X = tmpX + UnityEngine.Random.Range(-0.4f, 0.4f);
                enemy.Y = tmpY + UnityEngine.Random.Range(-0.4f, 0.4f);
                enemy.SetDirection(tmpX, tmpY, ranX, ranY);
                enemy.onChangeDebuffVisible = OnChangeDebuffVisible;
            }
        }


        public void CalculateVector(Vector3 playerVector)
        {
            cumulativeVector += playerVector * weight;
            Direction = DecisionDirection(cumulativeVector);
        }

        public int DecisionDirection(Vector3 cumulativeVector)
        {

            angleDegree = Quaternion.FromToRotation(Vector3.right, cumulativeVector).eulerAngles.z;

            //X�� ������
            if (angleDegree >= 337.5f || angleDegree < 22.5f)
                Direction = 0;
            //��1��и� �밢��
            else if (angleDegree >= 22.5f && angleDegree < 67.5f)
                Direction = 1;
            //Y�� ����
            else if (angleDegree >= 67.5f && angleDegree < 112.5)
                Direction = 2;
            //��2��и� �밢��
            else if (angleDegree >= 112.5 && angleDegree < 157.5)
                Direction = 3;
            //X�� ����
            else if (angleDegree >= 157.5f && angleDegree < 202.5)
                Direction = 4;
            //��3��и� �밢��
            else if (angleDegree >= 202.5f && angleDegree < 247.5f)
                Direction = 5;
            //Y�� �Ʒ���
            else if (angleDegree >= 247.5f && angleDegree < 292.5f)
                Direction = 6;
            //��4��и� �밢��
            else if (angleDegree >= 292.5f && angleDegree < 337.5f)
                Direction = 7;


            return Direction;

        }

        // [고블린 투사용] 원하는 갯수만큼 늪 소환하는 함수
        public void spawnGimmickPosionSwamp(int spawnCounts, Vector2 bossPosition)
        {
            for (int i = 0; i < spawnCounts; i++)
            {
                float radius = UnityEngine.Random.Range(0.5f, 1f);

                float angle = UnityEngine.Random.Range(0, 360);
                float X_Range = bossPosition.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
                float Y_Range = bossPosition.y + radius * Mathf.Sin(angle * Mathf.Deg2Rad);

                obstacle = Instantiate(posionSwamp);
                obstacle.transform.position = new Vector3(X_Range, Y_Range, 0f);
            }
        }

        public void CreateObstacle(int Direction, int playerTend)
        {
            switch (playerTend)
            {
                case 0:
                    MakeObstacleByPlayTendency(Direction);
                    break;
                case 2:
                    MakeEnemyHardByPlayTendency();
                    break;
            }
        }

        public int PlayerTendFun()
        {

            if (countIndex == 4)
            {
                countIndex = 0;
            }

            int playerTend = countIndex;
            countIndex++;

            return playerTend;
        }

        public void MakeObstacleByPlayTendency(int Direction)
        {
            float radius = UnityEngine.Random.Range(1f, 4f);
            float ranDegree = UnityEngine.Random.Range(angleDegree - 45f, angleDegree + 45f);
            float X_Range = 0f;
            float Y_Range = 0f;

            X_Range = curPlayerPosition.x + (float)Mathf.Cos(ranDegree * Mathf.Deg2Rad) * radius;
            Y_Range = curPlayerPosition.y + (float)Mathf.Sin(ranDegree * Mathf.Deg2Rad) * radius;
            obstacle = Instantiate(posionSwamp);
            obstacle.transform.position = new Vector3(X_Range, Y_Range, 0f);
        }

        public void MakeEnemyHardByPlayTendency()
        {
            for (int i = 0; i < minionManager.minions.Count; i++)
            {
                StartCoroutine(((Minion)minionManager.minions[i]).MakeEnemyHardPattern());
            }
        }

        IEnumerator LaserDelay()
        {
            for (int i = 0; i < 20; i++)
            {
                LaserPattern();
                yield return new WaitForSeconds(0.5f);
            }
        }
    }
}