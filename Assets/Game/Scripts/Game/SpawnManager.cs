using System;
using System.Collections.Generic;
using UnityEngine;

namespace Eclipse.Game
{
    public class SpawnManager : Eclipse.Manager
    {
        //==================================================================
        // 적 마리수 계수
        [ReadOnly] public int enemyCoefficient = 3;

        // 적 스폰 쿨타임
        private float enemySpawnCoolTime = 3f;
        private float enemySpawnCoolTimer = 0f;

        public static int enemyMaxNum = 400;

        private const float balanceCoefficient = 3 / 5.0f; // 이지 모드 위한 설정 계수

        //==================================================================
        MinionManager minionManager;
        StageManager stageManager;
        PoolManager poolManager;

        //==================================================================
        public bool isBossSpawned = false;

        // 보스전 벽의 안쪽 영역 좌표
        public Vector2 innerMin;
        public Vector2 innerMax;

        //==================================================================
        private enum Stages { Stage1, Stage2, Stage3 };

        private enum Minions
        {
            /* Stage 1 */
            EvilTree,
            Ghost,
            Pumpkin,
            Warlock,
            /* Stage 2 */
            Skeleton_Sword,
            Skeleton_Archer,
            Skeleton_Ghost,
            Skeleton_Horse,
            /* Stage 3 */
            Ghoul,
            Spitter,
            Summoner,
            DarkWarrior,
            BloodKing,
            /* 번외 */
            Bat
        }

        //==================================================================
        public Action SpawnStartEnemies;
        public Action SpawnEnemiesByTime;

        //==================================================================
        public Server_PlayerData server_PlayerData;

        //==================================================================

        private void Start()
        {
            minionManager = client.GetManager<MinionManager>();
            stageManager = client.GetManager<StageManager>();
            poolManager = client.GetManager<PoolManager>();
        }

        public override void Startup()
        {
            SpawnStartEnemies(); // 시작 적 스폰
        }

        private void Update()
        {
            if (stageManager.isGameOver) return;
            
            if (!isBossSpawned)
            {
                bool isEnemiesTooMany = minionManager.minions.Count > enemyMaxNum;
                if (!isEnemiesTooMany && enemySpawnCoolTimer >= enemySpawnCoolTime)
                {
                    SpawnEnemiesByTime(); // 소환할 적을 지정하고 스폰
                    enemySpawnCoolTimer = 0;
                }
            }
            // 정규 스테이지 시간의 경우에만 적용
            if (!stageManager.isStageRegularTimeOver) enemySpawnCoolTimer += Time.deltaTime;
        }

        public void SpawnStage1StartEnemies()
        {
            SpawnEnemies(0, 5 * enemyCoefficient); // 시작 적 소환
        }
        public void SpawnStage2StartEnemies()
        {
            SpawnEnemies(4, 10 * enemyCoefficient); // 시작 적 소환
        }
        public void SpawnStage3StartEnemies()
        {
            SpawnEnemies(8, 20 * enemyCoefficient); // 시작 적 소환
        }

        public void Stage1Spawn()
        {
            if (stageManager.gameTime <= 30 * balanceCoefficient)
            {
                SpawnEnemies(0, 1 * enemyCoefficient); // EvilTree 몬스터 소환
            }
            else if (stageManager.gameTime <= 60 * balanceCoefficient)
            {
                SpawnEnemies(0, 2 * enemyCoefficient); // EvilTree 몬스터 소환
                SpawnEnemies(1, 1 * enemyCoefficient); // Ghost 몬스터 소환
            }
            else if (stageManager.gameTime <= 90 * balanceCoefficient)
            {
                SpawnEnemies(0, 2 * enemyCoefficient); // EvilTree 몬스터 소환
                SpawnEnemies(1, 1 * enemyCoefficient); // Ghost 몬스터 소환
            }
            else if (stageManager.gameTime < 120 * balanceCoefficient)
            {
                SpawnEnemies(0, 2 * enemyCoefficient); // EvilTree 몬스터 소환
                SpawnEnemies(1, 1 * enemyCoefficient); // Ghost 몬스터 소환
                SpawnEnemies(2, 1 * enemyCoefficient); // Pumpkin 몬스터 소환
            }
            else if (stageManager.gameTime < 150 * balanceCoefficient)
            {
                SpawnEnemies(0, 2 * enemyCoefficient); // EvilTree 몬스터 소환
                SpawnEnemies(1, 1 * enemyCoefficient); // Ghost 몬스터 소환
                SpawnEnemies(2, 2 * enemyCoefficient); // Pumpkin 몬스터 소환
            }
            else if (stageManager.gameTime <= 180 * balanceCoefficient)
            {
                SpawnEnemies(0, 1 * enemyCoefficient); // EvilTree 몬스터 소환
                SpawnEnemies(1, 1 * enemyCoefficient); // Ghost 몬스터 소환
                SpawnEnemies(2, 4 * enemyCoefficient); // Pumpkin 몬스터 소환
            }
            else if (stageManager.gameTime <= 210 * balanceCoefficient)
            {
                SpawnEnemies(0, 1 * enemyCoefficient); // EvilTree 몬스터 소환
                SpawnEnemies(1, 1 * enemyCoefficient); // Ghost 몬스터 소환
                SpawnEnemies(2, 3 * enemyCoefficient); // Pumpkin 몬스터 소환
                SpawnEnemies(3, 2 * enemyCoefficient); // Warlock 몬스터 소환
            }
            else if (stageManager.gameTime < 240 * balanceCoefficient)
            {
                SpawnEnemies(1, 1 * enemyCoefficient); // Ghost 몬스터 소환
                SpawnEnemies(2, 2 * enemyCoefficient); // Pumpkin 몬스터 소환
                SpawnEnemies(3, 4 * enemyCoefficient); // Warlock 몬스터 소환
            }
            else if (stageManager.gameTime < 270 * balanceCoefficient)
            {
                SpawnEnemies(2, 2 * enemyCoefficient); // Pumpkin 몬스터 소환
                SpawnEnemies(3, 5 * enemyCoefficient); // Warlock 몬스터 소환
            }
            else if (stageManager.gameTime < 300 * balanceCoefficient)
            {
                SpawnEnemies(2, 3 * enemyCoefficient); // Pumpkin 몬스터 소환
                SpawnEnemies(3, 6 * enemyCoefficient); // Warlock 몬스터 소환
            }
        }
        public void Stage2Spawn()
        {
            if (stageManager.gameTime <= stageManager.sceneGameTime + 30 * balanceCoefficient)
            {
                SpawnEnemies(4, 2 * enemyCoefficient); // Skeleton_Sword 몬스터 소환
            }
            else if (stageManager.gameTime <= stageManager.sceneGameTime + 60 * balanceCoefficient)
            {
                SpawnEnemies(4, 4 * enemyCoefficient); // Skeleton_Sword 몬스터 소환
            }
            else if (stageManager.gameTime <= stageManager.sceneGameTime + 90 * balanceCoefficient)
            {
                SpawnEnemies(4, 4 * enemyCoefficient); // Skeleton_Sword 몬스터 소환
                SpawnEnemies(5, 1 * enemyCoefficient); // Skeleton_Archor 몬스터 소환
            }
            else if (stageManager.gameTime < stageManager.sceneGameTime + 120 * balanceCoefficient)
            {
                SpawnEnemies(4, 3 * enemyCoefficient); // Skeleton_Sword 몬스터 소환
                SpawnEnemies(5, 2 * enemyCoefficient); // Skeleton_Archor 몬스터 소환
            }
            else if (stageManager.gameTime < stageManager.sceneGameTime + 150 * balanceCoefficient)
            {
                SpawnEnemies(4, 2 * enemyCoefficient); // Skeleton_Sword 몬스터 소환
                SpawnEnemies(5, 2 * enemyCoefficient); // Skeleton_Archor 몬스터 소환
                SpawnEnemies(6, 2 * enemyCoefficient); // Skeleton_Ghost 몬스터 소환
            }
            else if (stageManager.gameTime <= stageManager.sceneGameTime + 180 * balanceCoefficient)
            {
                SpawnEnemies(4, 2 * enemyCoefficient); // Skeleton_Sword 몬스터 소환
                SpawnEnemies(5, 2 * enemyCoefficient); // Skeleton_Archor 몬스터 소환
                SpawnEnemies(6, 3 * enemyCoefficient); // Skeleton_Ghost 몬스터 소환
            }
            else if (stageManager.gameTime <= stageManager.sceneGameTime + 210 * balanceCoefficient)
            {
                SpawnEnemies(4, 1 * enemyCoefficient); // Skeleton_Sword 몬스터 소환
                SpawnEnemies(5, 2 * enemyCoefficient); // Skeleton_Archor 몬스터 소환
                SpawnEnemies(6, 2 * enemyCoefficient); // Skeleton_Ghost 몬스터 소환
                SpawnEnemies(7, 3 * enemyCoefficient); // Skeleton_Horse 몬스터 소환
            }
            else if (stageManager.gameTime < stageManager.sceneGameTime + 240 * balanceCoefficient)
            {
                SpawnEnemies(5, 2 * enemyCoefficient); // Skeleton_Archor 몬스터 소환
                SpawnEnemies(6, 3 * enemyCoefficient); // Skeleton_Ghost 몬스터 소환
                SpawnEnemies(7, 6 * enemyCoefficient); // Skeleton_Horse 몬스터 소환
            }
            else if (stageManager.gameTime < stageManager.sceneGameTime + 270 * balanceCoefficient)
            {
                SpawnEnemies(5, 1 * enemyCoefficient); // Skeleton_Archor 몬스터 소환
                SpawnEnemies(6, 4 * enemyCoefficient); // Skeleton_Ghost 몬스터 소환
                SpawnEnemies(7, 7 * enemyCoefficient); // Skeleton_Horse 몬스터 소환
            }
            else if (stageManager.gameTime < stageManager.sceneGameTime + 300 * balanceCoefficient)
            {
                SpawnEnemies(6, 6 * enemyCoefficient); // Skeleton_Ghost 몬스터 소환
                SpawnEnemies(7, 8 * enemyCoefficient); // Skeleton_Horse 몬스터 소환
            }
        }

        public void Stage3Spawn()
        {
            if (stageManager.gameTime <= stageManager.sceneGameTime + 30 * balanceCoefficient)
            {
                SpawnEnemies(8, 4 * enemyCoefficient); // Ghoul 몬스터 소환
            }
            else if (stageManager.gameTime <= stageManager.sceneGameTime + 60 * balanceCoefficient)
            {
                SpawnEnemies(8, 6 * enemyCoefficient); // Ghoul 몬스터 소환
            }
            else if (stageManager.gameTime <= stageManager.sceneGameTime + 90 * balanceCoefficient)
            {
                SpawnEnemies(8, 6 * enemyCoefficient); // Ghoul 몬스터 소환
                SpawnEnemies(9, 2 * enemyCoefficient); // Spitter 몬스터 소환
            }
            else if (stageManager.gameTime < stageManager.sceneGameTime + 120 * balanceCoefficient)
            {
                SpawnEnemies(8, 4 * enemyCoefficient); // Ghoul 몬스터 소환
                SpawnEnemies(9, 6 * enemyCoefficient); // Spitter 몬스터 소환
            }
            else if (stageManager.gameTime < stageManager.sceneGameTime + 150 * balanceCoefficient)
            {
                SpawnEnemies(8, 3 * enemyCoefficient); // Ghoul 몬스터 소환
                SpawnEnemies(9, 8 * enemyCoefficient); // Spitter 몬스터 소환
            }
            else if (stageManager.gameTime <= stageManager.sceneGameTime + 180 * balanceCoefficient)
            {
                SpawnEnemies(8, 2 * enemyCoefficient); // Ghoul 몬스터 소환
                SpawnEnemies(9, 6 * enemyCoefficient); // Spitter 몬스터 소환
                SpawnEnemies(10, 4 * enemyCoefficient); // Summoner 몬스터 소환
            }
            else if (stageManager.gameTime <= stageManager.sceneGameTime + 210 * balanceCoefficient)
            {
                SpawnEnemies(8, 2 * enemyCoefficient); // Ghoul 몬스터 소환
                SpawnEnemies(9, 3 * enemyCoefficient); // Spitter 몬스터 소환
                SpawnEnemies(10, 4 * enemyCoefficient); // Summoner 몬스터 소환
                SpawnEnemies(11, 4 * enemyCoefficient); // DarkWarrior 몬스터 소환
            }
            else if (stageManager.gameTime < stageManager.sceneGameTime + 240 * balanceCoefficient)
            {
                SpawnEnemies(9, 2 * enemyCoefficient); // Spitter 몬스터 소환
                SpawnEnemies(10, 4 * enemyCoefficient); // Summoner 몬스터 소환
                SpawnEnemies(11, 4 * enemyCoefficient); // DarkWarrior 몬스터 소환
                SpawnEnemies(12, 6 * enemyCoefficient); // BloodKing 몬스터 소환
            }
            else if (stageManager.gameTime < stageManager.sceneGameTime + 270 * balanceCoefficient)
            {
                SpawnEnemies(10, 2 * enemyCoefficient); // Summoner 몬스터 소환
                SpawnEnemies(11, 4 * enemyCoefficient); // DarkWarrior 몬스터 소환
                SpawnEnemies(12, 4 * enemyCoefficient); // BloodKing 몬스터 소환
            }
            else if (stageManager.gameTime < stageManager.sceneGameTime + 300 * balanceCoefficient)
            {
                SpawnEnemies(10, 2 * enemyCoefficient); // Summoner 몬스터 소환
                SpawnEnemies(11, 5 * enemyCoefficient); // DarkWarrior 몬스터 소환
                SpawnEnemies(12, 5 * enemyCoefficient); // BloodKing 몬스터 소환
            }
        }

        // Enemy 소환 함수
        private void SpawnEnemies(int index, float num)
        {
            num *= 1 + server_PlayerData.specialPassiveLevels[4] * server_PlayerData.nightmareMode;
            for (int i = 0; i < num; i++)
            {
                poolManager.GetMinion(index); // 몬스터 소환
            }
        }
        private void SpawnEnemies(int index, float num, Vector2 bossPosition)
        {
            float spawnMinRange = 0.5f;
            float spawnMaxRange = 2f;

            for (int i = 0; i < num; i++)
            {
                Minion minion = poolManager.GetMinion(index);

                // 1. 보스 주변 랜덤 위치 생성
                float randomRange = UnityEngine.Random.Range(spawnMinRange, spawnMaxRange);
                Vector2 randomOffset = UnityEngine.Random.insideUnitCircle.normalized * randomRange;
                Vector2 spawnPos = bossPosition + randomOffset;

                // 2. 내부 경계 안으로 강제 클램핑
                spawnPos.x = Mathf.Clamp(spawnPos.x, innerMin.x, innerMax.x);
                spawnPos.y = Mathf.Clamp(spawnPos.y, innerMin.y, innerMax.y);

                // 3. 위치 설정
                minion.transform.position = spawnPos;
            }
        }

        private List<Minions> GetMinionFor(Stages stage)
        {
            List<Minions> selected_MinionList = new List<Minions>();

            switch (stage)
            {
                case Stages.Stage1:
                    selected_MinionList.Add(Minions.EvilTree);
                    selected_MinionList.Add(Minions.Ghost);
                    selected_MinionList.Add(Minions.Pumpkin);
                    selected_MinionList.Add(Minions.Warlock);

                    break;
                case Stages.Stage2:
                    selected_MinionList.Add(Minions.Skeleton_Sword);
                    selected_MinionList.Add(Minions.Skeleton_Archer);
                    selected_MinionList.Add(Minions.Skeleton_Ghost);
                    selected_MinionList.Add(Minions.Skeleton_Horse);

                    break;
                case Stages.Stage3:
                    selected_MinionList.Add(Minions.Ghoul);
                    selected_MinionList.Add(Minions.Spitter);
                    selected_MinionList.Add(Minions.Summoner);
                    selected_MinionList.Add(Minions.DarkWarrior);
                    selected_MinionList.Add(Minions.BloodKing);

                    break;

                default:
                    selected_MinionList.Add(Minions.Bat);

                    break;
            }

            return selected_MinionList;
        }

        public void SpawnConstantEnemiesForStage(int stage, int spawnCounts, Vector2 bossPosition)
        {
            List<Minions> selected_MinionList = GetMinionFor((Stages)stage - 1); // Stage1이 0이므로 -1 해줌

            for (int i = 0; i < spawnCounts; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, selected_MinionList.Count);
                SpawnEnemies((int)selected_MinionList[randomIndex], 1, bossPosition);
            }
        }
    }
}
