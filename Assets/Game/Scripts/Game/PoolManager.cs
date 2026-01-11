using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Eclipse.Game
{
    public class PoolManager : Eclipse.Manager
    {
        // 인스턴스 변수
        private static PoolManager _instance;
        public static PoolManager instance
        {
            get
            {
                // 인스턴스가 없는 경우에 접근하려 하면 인스턴스를 할당해준다.
                if (!_instance)
                {
                    _instance = FindAnyObjectByType(typeof(PoolManager)) as PoolManager;

                    if (_instance == null)
                        Debug.Log("no Singleton obj");
                }
                return _instance;
            }
        }

        [ReadOnly] public PlayerData playerData;

        const int ENEMY_NUM = 14;
        const int EXP_NUM = 3;
        const int MAGE_SKILL_NUM = 20; // 불: 0 ~ 5, 전기: 6 ~ 9, 물: 10 ~ 13, 공명 스킬: 14 ~ 19
        const int WARRIOR_SKILL_NUM = 15;
        const int ASSASSIN_SKILL_NUM = 18; // 일반 스킬용 16개 + 버튼 액티브 비급(연막탄) 1개 + 액티브 비급(사출기) 1개 
        const int COIN_NUM = 3;

        int playerClass;

        // 사용할 클래스 객체들
        private MinionManager minionManager;

        // 프리팹 보관할 변수
        [SerializeField] Minion[] enemy_Prefabs = new Minion[ENEMY_NUM]; // ENEMY_NUM = 12
        [SerializeField] EXP[] exp_Prefabs = new EXP[EXP_NUM]; // EXP_NUM = 3
        [SerializeField] List<Skill> mage_Skill_Prefabs = new List<Skill>(MAGE_SKILL_NUM); // MAGE_SKILL_NUM = 21
        [SerializeField] List<Skill> warrior_Skill_Prefabs = new List<Skill>(WARRIOR_SKILL_NUM);
        [SerializeField] List<Skill> assassin_Skill_Prefabs = new List<Skill>(ASSASSIN_SKILL_NUM);
        [SerializeField] List<Skill>[] skills_Prefabs = new List<Skill>[3];
        [SerializeField] List<BossSkill> boss_Skill_Prefabs = new List<BossSkill>();
        [SerializeField] Assassin_Illusion[] assassin_Illusion_Prefabs = new Assassin_Illusion[2];
        [SerializeField] Coin[] coin_Prefabs = new Coin[COIN_NUM]; // COIN_NUM = 3
        [SerializeField] InGameText inGameText;
        [SerializeField] Arrow arrow;

        // 풀 담당을 하는 리스트들
        [SerializeField] List<Minion>[] enemy_Pools;
        [SerializeField] List<EXP>[] exp_Pools;
        [SerializeField] List<List<Skill>> mage_Skill_Pools = new List<List<Skill>>(MAGE_SKILL_NUM);
        [SerializeField] List<List<Skill>> warrior_Skill_Pools = new List<List<Skill>>(WARRIOR_SKILL_NUM);
        [SerializeField] List<List<Skill>> assassin_Skill_Pools = new List<List<Skill>>(ASSASSIN_SKILL_NUM);
        [SerializeField] List<List<Skill>>[] skills_Pools = new List<List<Skill>>[3];
        [SerializeField] List<List<BossSkill>> boss_Skill_Pool;
        [SerializeField] List<InGameText> inGame_Text_Pool = new List<InGameText>();
        [SerializeField] List<Arrow> arrow_Pool = new List<Arrow>();
        [SerializeField] List<Assassin_Illusion>[] assassin_Illusion_Pools;
        [SerializeField] List<Coin>[] coin_Pools;

        private void Awake()
        {
            enemy_Pools = new List<Minion>[enemy_Prefabs.Length];
            for (int index = 0; index < enemy_Pools.Length; index++)
            {
                enemy_Pools[index] = new List<Minion>();
            }

            exp_Pools = new List<EXP>[exp_Prefabs.Length];
            for (int index = 0; index < exp_Pools.Length; index++)
            {
                exp_Pools[index] = new List<EXP>();
            }

            for (int index = 0; index < MAGE_SKILL_NUM; index++)
            {
                mage_Skill_Pools.Add(new List<Skill>());
            }

            for (int index = 0; index < WARRIOR_SKILL_NUM; index++)
            {
                warrior_Skill_Pools.Add(new List<Skill>());
            }

            for (int index = 0; index < ASSASSIN_SKILL_NUM; index++)
            {
                assassin_Skill_Pools.Add(new List<Skill>());
            }

            boss_Skill_Pool = new List<List<BossSkill>>(boss_Skill_Prefabs.Count);
            for (int index = 0; index < boss_Skill_Prefabs.Count; index++)
            {
                boss_Skill_Pool.Add(new List<BossSkill>());
            }

            coin_Pools = new List<Coin>[coin_Prefabs.Length];
            for (int index = 0; index < coin_Pools.Length; index++)
            {
                coin_Pools[index] = new List<Coin>();
            }

            assassin_Illusion_Pools = new List<Assassin_Illusion>[assassin_Illusion_Prefabs.Length];
            for (int index = 0; index < assassin_Illusion_Prefabs.Length; index++)
            {
                assassin_Illusion_Pools[index] = new List<Assassin_Illusion>();
            }

            skills_Prefabs[0] = mage_Skill_Prefabs;
            skills_Prefabs[1] = warrior_Skill_Prefabs;
            skills_Prefabs[2] = assassin_Skill_Prefabs;

            skills_Pools[0] = mage_Skill_Pools;
            skills_Pools[1] = warrior_Skill_Pools;
            skills_Pools[2] = assassin_Skill_Pools;

            playerClass = PlayerPrefs.GetInt("PlayerClass");
        }

        private void Start()
        {
            minionManager = client.GetManager<MinionManager>();
        }

        public Minion GetMinion(int index)
        {
            Minion select = null;

            // 선택한 풀의 놀고있는(비활성화된) 게임 오브젝트 접근    
            foreach (Minion item in enemy_Pools[index])
            {
                if (!item.gameObject.activeSelf)
                {
                    // 발견하면 select 변수에 할당
                    select = item;

                    if (select.GetComponent<IPoolingObject>() != null)
                    {
                        minionManager.SetPooledMinionInfo(select, index);
                        select.gameObject.SetActive(true);
                        select.GetComponent<IPoolingObject>().Init();
                    }
                    break;
                }
            }

            // 못 찾았으면?      
            if (!select)
            {
                // 새롭게 생성하고 select 변수에 할당
                // 자기 자신(transform) 추가 이유: hierarchy창 지저분해지는 거 방지
                select = Instantiate(enemy_Prefabs[index]);

                minionManager.SetMinionInfo(select, index);

                select.Init();

                select.transform.SetParent(this.gameObject.transform.GetChild(0));
                enemy_Pools[index].Add(select);
            }

            return select;
        }

        public void ReturnMinion(Minion obj, int index)
        {
            obj.gameObject.SetActive(false);
        }

        // Enemy를 새롭게 생성
        void CreateEnemies(int index, int num)
        {
            for (int i = 0; i < num; i++)
            {
                Minion enemy = Instantiate(enemy_Prefabs[index]);

                enemy.transform.SetParent(this.gameObject.transform.GetChild(0)); // 자기 자신(transform) 추가 이유: hierarchy창 지저분해지는 거 방지
                enemy_Pools[index].Add(enemy);

                enemy.gameObject.SetActive(false);
            }
        }

        public EXP GetExp(int index, int expAmount)
        {
            EXP select = null;

            // 선택한 풀의 놀고있는(비활성화된) 게임 오브젝트 접근    
            foreach (EXP item in exp_Pools[index])
            {
                if (!item.gameObject.activeSelf)
                {
                    // 발견하면 select 변수에 할당
                    select = item;
                    select.expAmount = expAmount;

                    if (select.GetComponent<IPoolingObject>() != null)
                        select.GetComponent<IPoolingObject>().Init();

                    select.gameObject.SetActive(true);
                    break;
                }
            }

            // 못 찾았으면?      
            if (!select)
            {
                if (exp_Pools[index].Count > 300) return null; // 너무 많이 나와서 렉걸리는 거 방지

                // 새롭게 생성하고 select 변수에 할당
                // 자기 자신(transform) 추가 이유: hierarchy창 지저분해지는 거 방지
                select = Instantiate(exp_Prefabs[index]);
                select.expAmount = expAmount;

                select.Init();

                select.transform.SetParent(this.gameObject.transform.GetChild(1));
                exp_Pools[index].Add(select);
            }

            return select;
        }
        public void ReturnExp(EXP obj, int index)
        {
            obj.gameObject.SetActive(false);
        }

        // Exp를 새롭게 생성
        void CreateExps(int index, int num)
        {
            for (int i = 0; i < num; i++)
            {
                EXP exp = Instantiate(exp_Prefabs[index]);

                exp.transform.SetParent(this.gameObject.transform.GetChild(1)); // 자기 자신(transform) 추가 이유: hierarchy창 지저분해지는 거 방지
                exp_Pools[index].Add(exp);

                exp.gameObject.SetActive(false);
            }
        }

        public Coin GetCoin(int index, int coinAmount)
        {
            Coin select = null;

            // 선택한 풀의 놀고있는(비활성화된) 게임 오브젝트 접근    
            foreach (Coin item in coin_Pools[index])
            {
                if (!item.gameObject.activeSelf)
                {
                    // 발견하면 select 변수에 할당
                    select = item;
                    select.coinAmount = coinAmount;

                    if (select.GetComponent<IPoolingObject>() != null)
                        select.GetComponent<IPoolingObject>().Init();

                    select.gameObject.SetActive(true);
                    break;
                }
            }

            // 못 찾았으면?      
            if (!select)
            {
                if (coin_Pools[index].Count > 300) return null; // 너무 많이 나와서 렉걸리는 거 방지

                // 새롭게 생성하고 select 변수에 할당
                // 자기 자신(transform) 추가 이유: hierarchy창 지저분해지는 거 방지
                select = Instantiate(coin_Prefabs[index]);
                select.coinAmount = coinAmount;

                if (select.GetComponent<IPoolingObject>() != null)
                    select.GetComponent<IPoolingObject>().Init();

                select.transform.SetParent(this.gameObject.transform.GetChild(1));
                coin_Pools[index].Add(select);
            }

            return select;
        }

        public void ReturnCoin(Coin obj, int index)
        {
            obj.gameObject.SetActive(false);

        }

        public Skill GetSkill(int index)
        {
            Skill select = null;

            // 선택한 풀의 놀고있는(비활성화된) 게임 오브젝트 접근    
            foreach (Skill item in skills_Pools[playerClass][index])
            {
                if (!item.gameObject.activeSelf)
                {
                    // 발견하면 select 변수에 할당
                    select = item;

                    if (select.GetComponent<IPoolingObject>() != null)
                    {
                        select.returnIndex = index; // return을 위해 index 부여

                        select.gameObject.SetActive(true);
                        select.GetComponent<IPoolingObject>().Init();
                    }
                    break;
                }
            }

            // 못 찾았으면?      
            if (!select)
            {
                // 새롭게 생성하고 select 변수에 할당
                select = Instantiate(skills_Prefabs[playerClass][index]);

                if (select.GetComponent<IPoolingObject>() != null)
                {
                    select.returnIndex = index; // return을 위해 index 부여

                    select.GetComponent<IPoolingObject>().Init();
                }

                select.transform.SetParent(this.gameObject.transform.GetChild(2));
                skills_Pools[playerClass][index].Add(select);
            }

            return select;
        }

        public void ReturnSkill(Skill obj, int index)
        {
            obj.gameObject.SetActive(false);
        }

        // Skill을 새롭게 생성
        void CreateSkills(int index, int num)
        {
            for (int i = 0; i < num; i++)
            {
                Skill skill = Instantiate(skills_Prefabs[playerClass][index]);

                skill.transform.SetParent(this.gameObject.transform.GetChild(2)); // 자기 자신(transform) 추가 이유: hierarchy창 지저분해지는 거 방지
                skills_Pools[playerClass][index].Add(skill);

                skill.gameObject.SetActive(false);
            }
        }

        public BossSkill GetBossSkill(int index, Boss boss)
        {
            BossSkill select = null;

            // 선택한 풀의 놀고있는(비활성화된) 게임 오브젝트 접근
            foreach (BossSkill item in boss_Skill_Pool[index])
            {
                if (!item.gameObject.activeSelf)
                {
                    // 발견하면 select 변수에 할당
                    select = item;

                    if (select.GetComponent<IPoolingObject>() != null)
                    {
                        select.index = index; // return을 위해 index 부여
                        select.boss = boss;

                        select.gameObject.SetActive(true);
                        select.GetComponent<IPoolingObject>().Init();
                    }
                    break;
                }
            }
            // 못 찾았으면?      
            if (!select)
            {
                // 새롭게 생성하고 select 변수에 할당
                select = Instantiate(boss_Skill_Prefabs[index]);

                if (select.GetComponent<IPoolingObject>() != null)
                {
                    select.index = index; // return을 위해 index 부여
                    select.boss = boss;

                    select.GetComponent<IPoolingObject>().Init();
                }

                select.transform.SetParent(this.gameObject.transform.GetChild(3));
                boss_Skill_Pool[index].Add(select);
            }

            return select;
        }

        public BossSkill GetBossSkill(int index, Boss boss, float? num) // Boss Laser 때문에 만든 것
        {
            BossSkill select = null;

            // 선택한 풀의 놀고있는(비활성화된) 게임 오브젝트 접근
            foreach (BossSkill item in boss_Skill_Pool[index])
            {
                if (!item.gameObject.activeSelf)
                {
                    // 발견하면 select 변수에 할당
                    select = item;

                    if (select.GetComponent<IPoolingObject>() != null)
                    {
                        select.index = index; // return을 위해 index 부여
                        select.boss = boss;
                        if (num.HasValue)
                        {
                            ((Boss_Laser)select).laserTurnNum = num.Value;
                        }

                        select.gameObject.SetActive(true);
                        select.GetComponent<IPoolingObject>().Init();
                    }
                    break;
                }
            }

            // 못 찾았으면?      
            if (!select)
            {
                // 새롭게 생성하고 select 변수에 할당
                select = Instantiate(boss_Skill_Prefabs[index]);

                if (select.GetComponent<IPoolingObject>() != null)
                {
                    select.index = index; // return을 위해 index 부여
                    select.boss = boss;
                    if (num.HasValue)
                    {
                        ((Boss_Laser)select).laserTurnNum = num.Value;
                    }
                    select.GetComponent<IPoolingObject>().Init();
                }

                select.transform.SetParent(this.gameObject.transform.GetChild(3));
                boss_Skill_Pool[index].Add(select);
            }

            return select;
        }
        public BossSkill GetBossSkill(int index, Boss boss, float x, float y, bool? isRightTop) // Grid laser 때문에 만든 것
        {
            BossSkill select = null;

            // 선택한 풀의 놀고있는(비활성화된) 게임 오브젝트 접근
            foreach (BossSkill item in boss_Skill_Pool[index])
            {
                if (!item.gameObject.activeSelf)
                {
                    // 발견하면 select 변수에 할당
                    select = item;

                    if (select.GetComponent<IPoolingObject>() != null)
                    {
                        select.index = index; // return을 위해 index 부여
                        select.boss = boss;
                        select.X = x;
                        select.Y = y;
                        if (isRightTop.HasValue)
                        {
                            ((Boss_Grid_Laser)select).isLeftTop = isRightTop.Value;
                        }

                        select.gameObject.SetActive(true);
                        select.GetComponent<IPoolingObject>().Init();
                    }
                    break;
                }
            }

            // 못 찾았으면?      
            if (!select)
            {
                // 새롭게 생성하고 select 변수에 할당
                select = Instantiate(boss_Skill_Prefabs[index]);

                if (select.GetComponent<IPoolingObject>() != null)
                {
                    select.index = index; // return을 위해 index 부여
                    select.boss = boss;
                    select.X = x;
                    select.Y = y;
                    if (isRightTop.HasValue)
                    {
                        ((Boss_Grid_Laser)select).isLeftTop = isRightTop.Value;
                    }
                    select.GetComponent<IPoolingObject>().Init();
                }

                select.transform.SetParent(this.gameObject.transform.GetChild(3));
                boss_Skill_Pool[index].Add(select);
            }

            return select;
        } 
        public void ReturnBossSkill(BossSkill obj, int index)
        {
            obj.gameObject.SetActive(false);
        }

        public InGameText GetInGameText(string text, bool isCritical)
        {
            InGameText select = null;

            // 선택한 풀의 놀고있는(비활성화된) 게임 오브젝트 접근    
            foreach (InGameText item in inGame_Text_Pool)
            {
                if (!item.gameObject.activeSelf)
                {
                    // 발견하면 select 변수에 할당
                    select = item;
                    if (select.GetComponent<IPoolingObject>() != null)
                    {
                        select.gameObject.SetActive(true);
                        select.showText = text;
                        select.isCritical = isCritical;
                        select.GetComponent<IPoolingObject>().Init();
                    }
                    break;
                }
            }

            // 못 찾았으면?      
            if (!select)
            {
                // 새롭게 생성하고 select 변수에 할당
                // 자기 자신(transform) 추가 이유: hierarchy창 지저분해지는 거 방지
                if (inGame_Text_Pool.Count > 100) return null; // 너무 많이 나와서 렉걸리는 거 방지
                
                select = Instantiate(inGameText);
                select.showText = text;
                select.isCritical = isCritical;
                select.GetComponent<IPoolingObject>().Init();

                select.transform.SetParent(this.gameObject.transform.GetChild(4));
                inGame_Text_Pool.Add(select);
            }

            return select;
        }

        public void ReturnText(GameObject obj)
        {
            obj.SetActive(false);
        }

        void CreateTexts(int num)
        {
            for (int i = 0; i < num; i++)
            {
                InGameText tmpObject = Instantiate(inGameText);

                tmpObject.transform.SetParent(this.gameObject.transform.GetChild(5)); // 자기 자신(transform) 추가 이유: hierarchy창 지저분해지는 거 방지
                inGame_Text_Pool.Add(tmpObject);

                tmpObject.gameObject.SetActive(false);
            }
        }

        public Arrow GetArrow(Minion enemy)
        {
            Arrow select = null;

            // 선택한 풀의 놀고있는(비활성화된) 게임 오브젝트 접근    
            foreach (Arrow item in arrow_Pool)
            {
                if (!item.gameObject.activeSelf)
                {
                    // 발견하면 select 변수에 할당
                    select = item;

                    select.X = enemy.X;
                    select.Y = enemy.Y;

                    if (select.GetComponent<IPoolingObject>() != null)
                        select.GetComponent<IPoolingObject>().Init();

                    select.gameObject.SetActive(true);
                    break;
                }
            }

            // 못 찾았으면?      
            if (!select)
            {
                // 새롭게 생성하고 select 변수에 할당
                // 자기 자신(transform) 추가 이유: hierarchy창 지저분해지는 거 방지
                select = Instantiate(arrow);

                select.X = enemy.X;
                select.Y = enemy.Y;
                select.GetComponent<IPoolingObject>().Init();

                select.transform.SetParent(this.gameObject.transform.GetChild(5));
                arrow_Pool.Add(select);
            }

            return select;
        }
        public void ReturnArrow(Arrow obj)
        {
            obj.gameObject.SetActive(false);
        }

        void CreateArrows(int num)
        {
            for (int i = 0; i < num; i++)
            {
                Arrow tmpObject = Instantiate(arrow);

                tmpObject.transform.SetParent(this.gameObject.transform.GetChild(6)); // 자기 자신(transform) 추가 이유: hierarchy창 지저분해지는 거 방지
                arrow_Pool.Add(tmpObject);

                tmpObject.gameObject.SetActive(false);
            }
        }

        public Assassin_Illusion GetIllusion(int index)
        {
            Assassin_Illusion select = null;

            // 선택한 풀의 놀고있는(비활성화된) 게임 오브젝트 접근
            foreach (Assassin_Illusion item in assassin_Illusion_Pools[index])
            {
                if (!item.gameObject.activeSelf)
                {
                    // 발견하면 select 변수에 할당
                    select = item;

                    if (select.GetComponent<IPoolingObject>() != null)
                        select.GetComponent<IPoolingObject>().Init();

                    select.gameObject.SetActive(true);
                    break;
                }
            }

            // 못 찾았으면?      
            if (!select)
            {
                // 새롭게 생성하고 select 변수에 할당
                // 자기 자신(transform) 추가 이유: hierarchy창 지저분해지는 거 방지
                select = Instantiate(assassin_Illusion_Prefabs[index]);

                select.transform.SetParent(this.gameObject.transform.GetChild(6));
                assassin_Illusion_Pools[index].Add(select);
            }

            return select;
        }

        public void ReturnIllusion(Assassin_Illusion obj)
        {
            obj.gameObject.SetActive(false);
        }
    }
}