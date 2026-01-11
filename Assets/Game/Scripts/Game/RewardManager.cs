using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Eclipse.Game
{
    public class RewardManager : Eclipse.Manager
    {
        private HP_Potion hp_Potion;
        public HP_Potion small_HP_Potion;
        private Debuff_Chest debuff_Box;
        private Normal_Chest normal_Box;
        private Magnet magnet;
        private Cross cross;

        public PlayerData playerData;
        public static Server_PlayerData server_PlayerData; // DropItem 클래스에서 가져다 쓸 수 있또록 staic으로 함.

        [SerializeField] private HP_Potion hp_PotionPrefab;
        [SerializeField] private HP_Potion small_HP_PotionPrefab;
        [SerializeField] private Debuff_Chest debuff_BoxPrefab;
        [SerializeField] private Normal_Chest normal_BoxPrefab;
        [SerializeField] private Magnet magnetPrefab;
        [SerializeField] private Cross crossPrefab;

        // 클래스 변수
        PatternManager patternManager;
        PoolManager poolManager;
        StageManager stageManager;

        // 확률 변수
        float expDropChance = 40f; // EXP 드랍 확률
        float coinDropChance = 0.5f; // Coin 드랍 확률
        float hpPotionDropChance = 0.25f; // HP Potion 드랍 확률
        float normalBoxDropChance = 0.083f; // Normal Box 드랍 확률 (1/1200)
        float magnetDropChance = 0.05f; // Magnet 드랍 확률 (1/2000)
        float crossDropChance = 0.05f; // Cross 드랍 확률
        float debuffBoxDropChance = 0.05f; // Debuff Box 드랍 확률 (스테이지 2에서만 적용)

        // 캐싱용 변수
        EXP exp;
        Coin coin;
        string sceneName;

        [Serializable]
        public class DropItem
        {
            public string itemName;
            public float dropChance; // 확률 %
            public Action<Enemy> dropAction; // 드랍 실행 함수

            public DropItem(string name, float chance, Action<Enemy> action)
            {
                itemName = name;
                dropChance = chance * (1 + server_PlayerData.specialPassiveLevels[1] * server_PlayerData.luck);
                dropAction = action;
            }
        }

        private List<DropItem> dropTable;

        void Start()
        {
            poolManager = PoolManager.instance;
            patternManager = client.GetManager<PatternManager>();
            stageManager = client.GetManager<StageManager>();
            sceneName = SceneManager.GetActiveScene().name;
            InitializeDropTable();
        }

        private void InitializeDropTable()
        {
            dropTable = new List<DropItem>
            {
                new DropItem("EXP", expDropChance, (enemy) => ExpDrop(enemy)),
                new DropItem("Coin", coinDropChance, (enemy) => CoinDrop(enemy)),
                new DropItem("HP Potion", hpPotionDropChance, (enemy) => HP_PotionSpawn(10, enemy)),
                new DropItem("Normal Box", normalBoxDropChance, (enemy) => Normal_BoxSpawn(enemy)), // 1/1200 = 0.083%
                new DropItem("Magnet", magnetDropChance, (enemy) => MagentSpawn(enemy)), // 1/2000 = 0.05%
                new DropItem("Cross", crossDropChance, (enemy) => CrossSpawn(enemy)),
                new DropItem("Debuff Box", debuffBoxDropChance, (enemy) =>
                {
                    if (sceneName == "Stage2" && enemy.tag != "Bat")
                        Debuff_BoxSpawn(enemy, stageManager.maxGameTime, stageManager.gameTime);
                })
            };
        }

        // 적이 죽었을 때 킬수를 올리고, 아이템을 드롭함.
        public void OnEnemyKilled(Enemy killedEnemy)
        {
            if (!PlayerManager.player.isPlayerDead)
            {
                playerData.kill++;
            }

            DropItems(killedEnemy);
        }

        // 각각의 아이템들이 드롭될 지 계산해서 드롭시킴.(여러 아이템이 동시에 드랍 가능)
        private void DropItems(Enemy killedEnemy)
        {
            foreach (var item in dropTable)
            {
                float randomValue = UnityEngine.Random.Range(0f, 100f); // 각 아이템마다 새로운 랜덤값 생성
                if (randomValue < item.dropChance)
                {
                    item.dropAction.Invoke(killedEnemy);
                }
            }
        }

        //==================================================================
        private void ExpDrop(Enemy enemy)
        {
            switch (enemy.tag)
            {
                case "EvilTree":
                case "Ghost":
                case "Pumpkin":
                    ExpSpawn(0, 2, enemy);
                    break;
                case "WarLock":
                case "Skeleton_Sword":
                    ExpSpawn(0, 3, enemy);
                    break;
                case "Skeleton_Archer":
                case "Skeleton_Ghost":
                case "Skeleton_Horse":
                    ExpSpawn(1, 5, enemy);
                    break;
                case "Ghoul":
                    ExpSpawn(1, 6, enemy);
                    break;
                case "Spitter":
                    ExpSpawn(2, 6, enemy);
                    break;
                case "Summoner":
                case "DarkWarrior":
                case "BloodKing":
                    ExpSpawn(2, 8, enemy);
                    break;
            }
        }

        private void CoinDrop(Enemy enemy)
        {
            switch (enemy.tag)
            {
                case "EvilTree":
                case "Ghost":
                case "Pumpkin":
                case "WarLock":
                case "Skeleton_Sword":
                    CoinSpawn(0, UnityEngine.Random.Range(10, 50), enemy);
                    break;
                case "Skeleton_Archer":
                case "Skeleton_Ghost":
                case "Skeleton_Horse":
                case "Ghoul":
                    CoinSpawn(1, UnityEngine.Random.Range(50, 70), enemy);
                    break;
                case "Spitter":
                case "Summoner":
                case "DarkWarrior":
                case "BloodKing":
                    CoinSpawn(2, UnityEngine.Random.Range(70, 100), enemy);
                    break;
            }
        }

        //==================================================================
        public void ExpSpawn(int index, int expAmount, Enemy killedEnemy)
        {
            float ranNum = UnityEngine.Random.Range(-0.2f, 0.2f);

            //exp.expAmount = 200;
            exp = poolManager.GetExp(index, expAmount);
            if (exp == null) return;

            exp.index = index;

            exp.X = killedEnemy.transform.position.x + ranNum;
            exp.Y = killedEnemy.transform.position.y + ranNum;
        }

        public void CoinSpawn(int index, int coinAmount, Enemy killedEnemy)
        {
            float ranNum = UnityEngine.Random.Range(-0.2f, 0.2f);

            coin = poolManager.GetCoin(index, coinAmount);
            if (coin == null) return;

            coin.index = index;

            coin.X = killedEnemy.transform.position.x + ranNum;
            coin.Y = killedEnemy.transform.position.y + ranNum;

        }

        public void HP_PotionSpawn(float hpAmount, Enemy killedEnemy)
        {
            hp_Potion = Instantiate(hp_PotionPrefab);

            hp_Potion.hpAmount = hpAmount * (1 + server_PlayerData.regen * server_PlayerData.basicPassiveLevels[3]);

            hp_Potion.X = killedEnemy.transform.position.x;
            hp_Potion.Y = killedEnemy.transform.position.y + 0.5f;
        }

        //==================================================================
        public void Normal_BoxSpawn(Enemy killedEnemy)
        {
            float ranNum = UnityEngine.Random.Range(-1f, 1f);
            normal_Box = Instantiate(normal_BoxPrefab);

            normal_Box.rewardManager = this;
            normal_Box.X = killedEnemy.transform.position.x + ranNum;
            normal_Box.Y = killedEnemy.transform.position.y + ranNum;
        }

        public void Debuff_BoxSpawn(Enemy killedEnemy, float MaxGameTime, float GameTime)
        {
            float ranNum = UnityEngine.Random.Range(-1f, 1f);
            debuff_Box = Instantiate(debuff_BoxPrefab);

            debuff_Box.maxGameTime = MaxGameTime;
            debuff_Box.gameTime = GameTime;

            debuff_Box.X = killedEnemy.transform.position.x + ranNum;
            debuff_Box.Y = killedEnemy.transform.position.y + ranNum;

            debuff_Box.patternManager = patternManager;
            debuffBoxDropChance /= 2f; // 드랍 될 때마다 확률을 반으로 줄임. (스테이지 2에서만 적용)
        }

        public void MagentSpawn(Enemy killedEnemy)
        {
            float ranNum = UnityEngine.Random.Range(-1f, 1f);
            magnet = Instantiate(magnetPrefab);

            magnet.X = killedEnemy.transform.position.x + ranNum;
            magnet.Y = killedEnemy.transform.position.y + ranNum;
        }

        public void CrossSpawn(Enemy killedEnemy)
        {
            float ranNum = UnityEngine.Random.Range(-1f, 1f);
            cross = Instantiate(crossPrefab);

            cross.X = killedEnemy.transform.position.x + ranNum;
            cross.Y = killedEnemy.transform.position.y + ranNum;
        }

        //==================================================================
        public void ExpSpawn_By_GameObject(int index, int expAmount, GameObject gameObject)
        {
            float radius = gameObject.layer == LayerMask.NameToLayer("Boss")
                ? UnityEngine.Random.Range(0.6f, 3f)
                : UnityEngine.Random.Range(0f, 1.5f);
            float angle = UnityEngine.Random.Range(0, 360);

            exp = poolManager.GetExp(index, expAmount);
            if (exp == null) return;

            exp.index = index;

            exp.X = gameObject.transform.position.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            exp.Y = gameObject.transform.position.y + radius * Mathf.Sin(angle * Mathf.Deg2Rad);
        }

        public void CoinSpawn_By_GameObject(int index, int coinAmount, GameObject gameObject)
        {
            float radius = gameObject.layer == LayerMask.NameToLayer("Boss")
                ? UnityEngine.Random.Range(0.6f, 3f)
                : UnityEngine.Random.Range(0f, 1.5f);
            float angle = UnityEngine.Random.Range(0, 360);

            coin = poolManager.GetCoin(index, coinAmount);
            if (coin == null) return;

            coin.index = index;

            coin.X = gameObject.transform.position.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            coin.Y = gameObject.transform.position.y + radius * Mathf.Sin(angle * Mathf.Deg2Rad);
        }

        public void Small_HP_Potion_By_GameObject(float hpAmount, GameObject gameObject)
        {
            small_HP_Potion = Instantiate(small_HP_PotionPrefab);

            small_HP_Potion.hpAmount = hpAmount;

            float radius = UnityEngine.Random.Range(0f, 1.2f);
            float angle = UnityEngine.Random.Range(0, 360);

            small_HP_Potion.X = gameObject.transform.position.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            small_HP_Potion.Y = gameObject.transform.position.y + radius * Mathf.Sin(angle * Mathf.Deg2Rad);
        }

        //==================================================================
        public void ExpSpawn_By_DebuffBox(int index, int expAmount)
        {
            float radius = UnityEngine.Random.Range(0.6f, 2f);
            float angle = UnityEngine.Random.Range(0, 360);

            exp = poolManager.GetExp(index, expAmount);
            if (exp == null) return;

            exp.index = index;

            exp.X = PlayerManager.player.transform.position.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            exp.Y = PlayerManager.player.transform.position.y + radius * Mathf.Sin(angle * Mathf.Deg2Rad);
        }

        public void CoinSpawn_By_DebuffBox(int index, int coinAmount)
        {
            float radius = UnityEngine.Random.Range(0.6f, 2f);
            float angle = UnityEngine.Random.Range(0, 360);

            coin = poolManager.GetCoin(index, coinAmount);
            if (coin == null) return;

            coin.index = index;

            coin.X = PlayerManager.player.transform.position.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            coin.Y = PlayerManager.player.transform.position.y + radius * Mathf.Sin(angle * Mathf.Deg2Rad);

        }
        public void Small_HP_PotionSpawn_By_DebuffBox(float hpAmount)
        {
            small_HP_Potion = Instantiate(small_HP_PotionPrefab);

            small_HP_Potion.hpAmount = hpAmount;

            float radius = UnityEngine.Random.Range(0.6f, 1f);
            float angle = UnityEngine.Random.Range(0, 360);

            small_HP_Potion.X = PlayerManager.player.transform.position.x + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            small_HP_Potion.Y = PlayerManager.player.transform.position.y + radius * Mathf.Sin(angle * Mathf.Deg2Rad);
        }

        //==================================================================
        public void BossReward(Boss boss)
        {
            int rewardCoefficient;
            int stageEndCoefficient = 3; // Exp, Coin은 스테이지 끝나면 3배로 줄어드니까 3배 더 줘야 함.
            if (boss is FighterGoblin) // 200 ~ 1000골
            {
                rewardCoefficient = 2;
                for (int i = 0; i < 10 * rewardCoefficient; i++)
                    ExpSpawn_By_GameObject(0, 6 * stageEndCoefficient, boss.gameObject);
                for (int i = 0; i < 10 * rewardCoefficient; i++)
                    CoinSpawn_By_GameObject(0, UnityEngine.Random.Range(10, 50) * stageEndCoefficient, boss.gameObject);
                HP_PotionSpawn(10, boss);
            }
            else if (boss is RuinedKing) // 1500 ~ 2100골
            {
                rewardCoefficient = 3;
                for (int i = 0; i < 10 * rewardCoefficient; i++)
                    ExpSpawn_By_GameObject(1, 10 * stageEndCoefficient, boss.gameObject);
                for (int i = 0; i < 10 * rewardCoefficient; i++)
                    CoinSpawn_By_GameObject(1, UnityEngine.Random.Range(50, 70) * stageEndCoefficient, boss.gameObject);
                HP_PotionSpawn(10, boss);
            }
            else if (boss is Belial) // 3500 ~ 5000골
            {
                rewardCoefficient = 5;
                for (int i = 0; i < 10 * rewardCoefficient; i++)
                    CoinSpawn_By_GameObject(2, UnityEngine.Random.Range(70, 100) * stageEndCoefficient, boss.gameObject);
            }
        }

        //==================================================================
    }
}