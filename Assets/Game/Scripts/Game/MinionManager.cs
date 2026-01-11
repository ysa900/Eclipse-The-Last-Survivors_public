using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Eclipse.Game
{
    public class MinionManager : Eclipse.Manager
    {
        // ======================================================
        [ReadOnly] public List<Enemy> minions = new List<Enemy>(); // Enemy들을 담을 리스트
        public List<Assassin_Illusion> assassin_Illusions_List;

        // Minion 클래스 객체
        Minion minion;

        // Minion를 구분 짓기 위한 index
        int minionNameIndex = 0;

        // Minion들 체력
        protected float[] minion_HP = { 80, 120, 200, 300,
                                       700, 500, 1000, 2000,
                                       3000, 5000, 9000, 7000, 9000,
                                       1000000 };

        // Minion들 데미지
        protected float[] minion_Damage = { 0.6f, 0.6f, 0.7f, 0.7f,
                                           0.8f, 0.6f, 1.0f, 1.2f,
                                           1.2f, 1.4f, 1.2f, 1.5f, 1.8f,
                                           2.4f };

        // Minion들 데미지
        protected float[] minion_Speed = { 0.5f, 0.3f, 0.6f, 0.7f,
                                           0.7f, 0.5f, 0.8f, 1.0f,
                                           0.7f, 0.8f, 0.6f, 1.0f, 1.2f,
                                           2.5f };

        // Minion들 이름
        string[] minion_Names = { "EvilTree", "Ghost", "Pumpkin","Warlock",
                                "Skeleton_Sword", "Skeleton_Archer", "Skeleton_Ghost", "Skeleton_Horse",
                                "Ghoul", "Spitter", "Summoner", "DarkWarrior", "BloodKing",
                                "Bat" };

        // ======================================================
        string sceneName;

        float hitDelayTime = 0.1f;
        float hitDelayTimer = 0f;

        private float targetUpdateTime = 1.5f; // 타겟 업데이트 주기
        private float targetUpdateTimer = 0f;

        private bool isEnemyAlreadyEnraged = false;

        // ======================================================
        // Minion가 죽었을 때 Client를 통해 RewardManager에게 알려주기 위한 delegate
        public delegate void OnEnemyKilled(Minion killedEnemy);
        public OnEnemyKilled onEnemyKilled;

        // ======================================================
        public Server_PlayerData server_PlayerData;

        // ======================================================

        private void Start()
        {
            sceneName = SceneManager.GetActiveScene().name;
        }

        private void Update()
        {
            hitDelayTimer += Time.deltaTime;
            targetUpdateTimer += Time.deltaTime;

            if (targetUpdateTimer >= targetUpdateTime)
            {
                UpdateTargetForMinions();
                targetUpdateTimer = 0f;
            };

            if (minions.Count >= SpawnManager.enemyMaxNum && !isEnemyAlreadyEnraged && sceneName == "Stage3")
            {
                StartCoroutine(OnMinionMax());
            }
        }

        // 적들의 타겟을 주기적으로 업데이트
        private void UpdateTargetForMinions()
        {
            foreach (var minion in minions)
            {
                if (assassin_Illusions_List.Count == 0)
                {
                    ((Minion)minion).TargetObject = PlayerManager.player.gameObject;
                }
                else
                {
                    int randomIndex = UnityEngine.Random.Range(0, assassin_Illusions_List.Count);
                    ((Minion)minion).TargetObject = assassin_Illusions_List[randomIndex].gameObject;
                }
            }
        }

        public void AllocateIllusionObject()
        {
            for (int i = 0; i < minions.Count; i++)
            {
                // assassin_Illusion_List에서 무작위 인덱스를 생성
                int randomIndex = UnityEngine.Random.Range(0, assassin_Illusions_List.Count);

                // 무작위로 선택된 illusion을 적의 TargetObject에 할당
                ((Minion)minions[i]).TargetObject = assassin_Illusions_List[randomIndex].gameObject;
            }
        }

        public void AllocatePlayerObject(GameObject player)
        {
            for (int i = 0; i < minions.Count; i++)
            {
                ((Minion)minions[i]).TargetObject = player;
            }
        }

        // 적 정보를 입력하는 함수
        public void SetMinionInfo(Minion select, int index)
        {
            minion = select;
            minion.Player = PlayerManager.player;

            minion.index = index;
            minion.maxHp = minion_HP[index] * (1 + server_PlayerData.specialPassiveLevels[4] * server_PlayerData.nightmareMode);
            minion.damage = minion_Damage[index] * (1 + server_PlayerData.specialPassiveLevels[4] * server_PlayerData.nightmareMode);
            minion.speed = minion_Speed[index] * (1 + server_PlayerData.specialPassiveLevels[4] * server_PlayerData.nightmareMode);
            
            minion.name = minion_Names[index] + minionNameIndex; // 적들 이름 구분짓기 위한 방법
            minionNameIndex++;

            // delegate 할당
            minion.onMinionWasKilled = OnMinionWasKilled;
            minion.onMinionHit = OnMinionHit;

            SetPooledMinionInfo(select, index);
        }
        // 풀링된 Minion 정보를 설정하는 함수(공통된 부분)
        public void SetPooledMinionInfo(Minion select, int index)
        {
            minion = select;

            // 타겟 설정
            if (assassin_Illusions_List.Count == 0)
            {
                minion.TargetObject = PlayerManager.player.gameObject;
            }
            else
            {
                // assassin_Illusion_List에서 무작위 인덱스를 생성
                int randomIndex = UnityEngine.Random.Range(0, assassin_Illusions_List.Count);

                // 무작위로 선택된 illusion을 적의 TargetObject에 할당
                minion.TargetObject = assassin_Illusions_List[randomIndex].gameObject;
            }

            // 몬스터가 스테이지에 맞게 소환되게 함
            float degree = 0;

            Vector2 playerPosition = PlayerManager.player.transform.position;
            float playerX = playerPosition.x;
            float playerY = playerPosition.y;

            switch (sceneName)
            {
                case "Stage1": // Stage1은 원형으로 소환 
                    float radius = UnityEngine.Random.Range(4, 6);
                    degree = UnityEngine.Random.Range(0f, 360f);

                    float tmpX = (float)Math.Cos(degree * Mathf.Deg2Rad) * radius;
                    float tmpY = (float)Math.Sin(degree * Mathf.Deg2Rad) * radius;

                    minion.X = tmpX + playerX;
                    minion.Y = tmpY + playerY;

                    if (degree <= -360)
                    {
                        degree %= -360;
                    }
                    break;

                case "Stage2": // Stage2는 좌우로만 소환
                    tmpX = UnityEngine.Random.Range(2, 7);
                    tmpY = UnityEngine.Random.Range(-5, 1.1f); // 맵 y축 범위로 제한

                    if (UnityEngine.Random.Range(0, 2) == 1) tmpX = -tmpX; // 플레이어 기준 왼쪽, 오른쪽 랜덤

                    minion.X = tmpX + playerX;
                    minion.Y = tmpY;
                    break;

                case "Stage3": // Stage3는 정해진 범위 안에 소환 (플레이어와 겹치지 않게)
                    bool isPositionNearWithPlayer;
                    int breakNum = 0; // while문 탈출을 위한 num

                    do
                    {
                        radius = UnityEngine.Random.Range(3, 8);
                        degree = UnityEngine.Random.Range(0f, 360f);

                        tmpX = (float)Math.Cos(degree * Mathf.Deg2Rad) * radius;
                        tmpY = (float)Math.Sin(degree * Mathf.Deg2Rad) * radius;

                        Vector2 playerPos = PlayerManager.player.transform.position;
                        Vector2 myPos = new Vector2(tmpX, tmpY);
                        float sqrDistance = (playerPos - myPos).sqrMagnitude;
                        isPositionNearWithPlayer = sqrDistance < 1.5f * 1.5f;

                        breakNum++;
                        if (breakNum >= 10)// 10회 반복 내에 마땅한 위치를 찾지 못했다면 그냥 break;
                        {
                            break;
                        }
                    }
                    while (isPositionNearWithPlayer);

                    minion.X = tmpX;
                    minion.Y = tmpY;
                    break;
            }

            minions.Add(minion);
        }

        // Minion이 죽었을 때 실행할 것들
        private void OnMinionWasKilled(Minion killedMinion)
        {
            if (StageManager.instance != null && !StageManager.instance.isStageRegularTimeOver)
            {
                onEnemyKilled(killedMinion); // 킬 수 늘리고, 경험치 떨구도록 Client에게 알려주기

                if (!StageManager.instance.isGameOver) //  캐릭터 사망하기 전까지만 실행
                {
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead); // Enemy 사망 시 효과음
                }
            }

            minions.Remove(killedMinion);
        }

        void OnMinionHit()
        {
            bool isDelayOver = hitDelayTimer >= hitDelayTime;
            if (isDelayOver)
            {
                AudioManager.instance.PlaySfx(AudioManager.Sfx.Range); // Enemy 피격 효과음
                hitDelayTimer = 0;
            }
        }

        IEnumerator OnMinionMax()
        {
            isEnemyAlreadyEnraged = true;

            while (minions.Count >= SpawnManager.enemyMaxNum * 0.9f)
            {
                foreach (Minion enemy in minions)
                {
                    enemy.Enrage();
                }
                yield return new WaitForSeconds(30f);
            }

            isEnemyAlreadyEnraged = false;
        }

        public void KillAllMinions()
        {
            StartCoroutine(KillAllMinionsCoroutine(10)); // 프레임당 10마리씩 죽임
        }

        private IEnumerator KillAllMinionsCoroutine(int countPerFrame)
        {
            int counter = 0;

            for (int i = 0; i < minions.Count;)
            {
                ((Minion)minions[i]).Dead();
                counter++;

                if (counter >= countPerFrame)
                {
                    counter = 0;
                    yield return null; // 다음 프레임까지 대기
                }
            }
        }
    }
}