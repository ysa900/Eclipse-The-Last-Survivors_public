using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Eclipse.Game
{
    public class Client : Eclipse.Client
    {
        //==================================================================
        // 플레이어 테스트용
        [SerializeField] bool isPlayerTest;
        [SerializeField] int classNum;

        //==================================================================
        // 데이터들
        PlayerData _playerData;
        public PlayerData PlayerData { get { return _playerData; } }

        DefaultPlayerData _defaultPlayerData;
        public DefaultPlayerData DefaultPlayerData { get { return _defaultPlayerData; } }

        SkillData2 _skillData;
        public SkillData2 SkillData { get { return _skillData; } }

        SkillData2 _passiveSkillData;
        public SkillData2 PassiveSkillData { get { return _passiveSkillData; } }

        Server_PlayerData _server_PlayerData;
        public Server_PlayerData Server_PlayerData {  get { return _server_PlayerData; } }

        /* 비급 관련 데이터들 */
        MijiSkillData _buttonActiveMijiSkillData;
        public MijiSkillData ButtonActiveMijiSkillData { get { return _buttonActiveMijiSkillData; } }

        MijiSkillData _activeMijiSkillData;
        public MijiSkillData ActiveMijiSKillData { get { return _activeMijiSkillData; } }

        MijiSkillData _passiveMijiSkillData;
        public MijiSkillData PassiveMijiSkillData { get { return _passiveMijiSkillData; } }

        IllusionData _illusionData;
        public IllusionData IllusionData { get { return _illusionData; } }

        Shadow _shadow;
        public Shadow Shadow { get { return _shadow; } }

        //==================================================================
        // 매니저 클래스 객체들
        //==================================================================
        // 스테이지 관련 매니저들
        StageManager stageManager;
        RewardManager rewardManager;
        PatternManager patternManager;
        TilemapManager tilemapManager;

        //==================================================================
        // 캐릭터, 적들 관련 매니저들
        BossManager bossManager;
        EnemyManager enemyManager;
        MinionManager minionManager;
        PlayerManager playerManager;
        SpawnManager spawnManager;
        PoolManager poolManager;

        //==================================================================
        // 스킬 관련 매니저들
        SkillManager skillManager;
        SkillSelectManager skillSelectManager;

        Miji_SkillManager mijiSkillManager;
        Miji_SSM miji_SSM;

        //==================================================================
        // Input, GUI, Sound 등을 처리하는 매니저들
        InputManager inputManager;
        GUIManager guiManager;
        AudioManager audioManager;
        //==================================================================
        // 파일 저장 관련 매니저
        SavedataManager saveDataManager;

        //==================================================================

        string sceneName;
        
        protected override void Awake()
        {
            if (isPlayerTest) PlayerPrefs.SetInt("PlayerClass", classNum);

            base.Awake();

            //==================================================================

            sceneName = SceneManager.GetActiveScene().name; // 씬 이름 가져오기

            // 데이터들
            int playerClass = PlayerPrefs.GetInt("PlayerClass");
            string className = "";
            switch (playerClass)
            {
                case 0:
                    {
                        className = "Mage";
                        _playerData = Resources.Load<PlayerData>("Datas/PlayerData");
                        break;
                    }
                case 1:
                    {
                        className = "Warrior";
                        _playerData = Resources.Load<PlayerData>("Datas/PlayerData");
                        break;
                    }
                case 2:
                    {
                        className = "Assassin";

                        _buttonActiveMijiSkillData = Resources.Load<MijiSkillData>($"Datas/Skill Datas/{className}/Miji Button Active Skills Data");
                        _activeMijiSkillData = Resources.Load<MijiSkillData>($"Datas/Skill Datas/{className}/Miji Active Skills Data");
                        _passiveMijiSkillData = Resources.Load<MijiSkillData>($"Datas/Skill Datas/{className}/Miji Passive Skills Data");

                        _illusionData = Resources.Load<IllusionData>($"Datas/Skill Datas/{className}/Illusion Data");
                        _shadow = Resources.Load<Shadow>("Prefabs/Skills/AssassinSkills/Miji/Shadow").GetComponent<Shadow>();
                        _playerData = Resources.Load<AssassinData>("Datas/AssassinData");
                        break;
                    }
            }

            _skillData = Resources.Load<SkillData2>($"Datas/Skill Datas/{className}/{className} Skills Data");
            _passiveSkillData = Resources.Load<SkillData2>($"Datas/Skill Datas/{className}/{className} Passive Skills Data");
            _server_PlayerData = Resources.Load<Server_PlayerData>("Datas/Server_PlayerData");

            _defaultPlayerData = Resources.Load<DefaultPlayerData>("Datas/DefaultPlayerData");

            // PlayerData에 필요한 데이터 초기화
            _playerData.Initialize(_server_PlayerData, _passiveSkillData, _defaultPlayerData);

            //==================================================================
            // 매니저 클래스 객체들 초기화
            // Client는 예외적으로 다른 클래스들을 Awake에서 초기화 (가장 빨라야 하기 때문)
            //==================================================================
            // 스테이지 관련 매니저들
            stageManager = FindAnyObjectByType<StageManager>();
            stageManager.SetClient(this);
            stageManager.onChangeScene = ChangeScene;
            stageManager.playerData = _playerData;

            rewardManager = FindAnyObjectByType<RewardManager>();
            rewardManager.SetClient(this);
            rewardManager.playerData = _playerData;
            RewardManager.server_PlayerData = _server_PlayerData;

            patternManager = FindAnyObjectByType<PatternManager>();
            patternManager.SetClient(this);

            tilemapManager = FindAnyObjectByType<TilemapManager>();
            tilemapManager.SetClient(this);
            tilemapManager.onChangeScene = ChangeScene;

            //==================================================================
            // Input, GUI 관련 매니저들
            inputManager = FindAnyObjectByType<InputManager>();
            inputManager.SetClient(this);
            inputManager.onChangeScene = ChangeScene;

            guiManager = FindAnyObjectByType<GUIManager>();
            guiManager.SetClient(this);
            guiManager.onChangeScene = ChangeScene;

            guiManager.playerData = _playerData;
            guiManager.skillData = _skillData;
            guiManager.passiveSkillData = _passiveSkillData;

            guiManager.buttonActiveMijiSkillData = _buttonActiveMijiSkillData;
            guiManager.activeMijiSkillData = _activeMijiSkillData;
            guiManager.passiveMijiSkillData = _passiveMijiSkillData;

            audioManager = FindAnyObjectByType<AudioManager>();
            audioManager.SetClient(this);

            //==================================================================
            // 캐릭터, 적들 관련 매니저들

            playerManager = FindAnyObjectByType<PlayerManager>();
            playerManager.SetClient(this);
            playerManager.playerData = _playerData;
            playerManager.skillData = _skillData;
            playerManager.server_PlayerData = _server_PlayerData;
            playerManager.defaultPlayerData = _defaultPlayerData;

            playerManager.onGameOver = inputManager.OnGameOver;
            playerManager.onLevelUp = inputManager.OnLevelUp;
            if (playerClass == 2)
            {
                playerManager.onMijiSkillMustSelect = inputManager.OnMijiSkillMustSelect;
            }

            enemyManager = FindAnyObjectByType<EnemyManager>();
            enemyManager.SetClient(this);
            enemyManager.InitializeEnemies(3);
            enemyManager.allocateEnemy = () =>
            {
                enemyManager.enemies[0] = minionManager.minions;
                enemyManager.debugForEnemies0 = minionManager.minions;
            };

            minionManager = FindAnyObjectByType<MinionManager>();
            minionManager.SetClient(this);
            minionManager.onEnemyKilled = rewardManager.OnEnemyKilled;
            minionManager.server_PlayerData = _server_PlayerData;
            
            spawnManager = FindAnyObjectByType<SpawnManager>();
            spawnManager.SetClient(this);
            spawnManager.server_PlayerData = _server_PlayerData;

            poolManager = FindAnyObjectByType<PoolManager>();
            poolManager.SetClient(this);
            poolManager.playerData = _playerData;

            //==================================================================
            // 스킬 관련 매니저들
            skillManager = FindAnyObjectByType<SkillManager>();
            skillManager = SkillManagerFactory.Create(_playerData, skillManager.gameObject);
            Destroy(skillManager.GetComponent<SkillManager>());
            if (playerClass == 2)
            {
                ((Assassin_SkillManager)skillManager).buttonActiveMijiSkillData = _buttonActiveMijiSkillData;
                ((Assassin_SkillManager)skillManager).passiveMijiSkillData = _passiveMijiSkillData;
            }
            skillManager.SetClient(this);
            skillManager.skillData = _skillData;
            skillManager.passiveSkillData = _passiveSkillData;
            skillManager.server_PlayerData = _server_PlayerData;
            skillManager.playerData = _playerData;
            if (sceneName == "Stage1")
            {
                skillManager.Init(); // skillData 초기화
            }

            if (playerClass == 2)
            {
                mijiSkillManager = FindAnyObjectByType<Miji_SkillManager>();

                // 필요한 데이터 설정
                mijiSkillManager.buttonActiveMijiSkillData = _buttonActiveMijiSkillData;
                mijiSkillManager.activeMijiSkillData = _activeMijiSkillData;
                mijiSkillManager.passiveMijiSkillData = _passiveMijiSkillData;

                mijiSkillManager.illusionData = _illusionData;
                //mijiSkillManager.shadow = _shadow;

                mijiSkillManager.SetClient(this);
            }
            else
            {
                // playerClass가 2가 아닌 경우 Miji_SkillManager 객체를 찾아서 Destroy
                Miji_SkillManager mijiSkillManagerToDestroy = FindAnyObjectByType<Miji_SkillManager>();
                if (mijiSkillManagerToDestroy != null)
                {
                    Destroy(mijiSkillManagerToDestroy.gameObject);
                }
            }

            // 싱글톤 클래스이기 때문에 가져온 SkillSelectManager가 정확이 SkillSelectManager인지 자식 클래스타입 인지 검사
            skillSelectManager = FindAnyObjectByType<SkillSelectManager>();
            bool isExactType = skillSelectManager.GetType() == typeof(SkillSelectManager);

            if (isExactType)
            {
                skillSelectManager = SkillSelectManagerFactory.Create(_playerData, skillSelectManager.gameObject);
                Destroy(skillSelectManager.GetComponent<SkillSelectManager>());
                skillSelectManager.playerData = _playerData;
                skillSelectManager.skillData = _skillData;
                skillSelectManager.passiveSkillData = _passiveSkillData;
                skillSelectManager.server_PlayerData = _server_PlayerData;
            }
            // Factory가 끝난 뒤에 해줘야 함
            skillSelectManager.SetClient(this);

            //=============================================================================================
            saveDataManager = FindAnyObjectByType<SavedataManager>();
            saveDataManager.SetClient(this);


            // 이렇게도 정확히 해당 클래스타입인지 검사할 수 있음
            /*if (skillSelectManager is SkillSelectManager exactType)
            {
                // This is either SkillSelectManager or a derived type.
                if (exactType.GetType() == typeof(SkillSelectManager))
                {
                    // This is exactly SkillSelectManager, not a derived type.
                    Debug.Log("This is exactly a SkillSelectManager.");
                    skillSelectManager = SkillSelectManagerFactory.Create(_playerData, skillSelectManager.gameObject);
                    Destroy(skillSelectManager.GetComponent<SkillSelectManager>());
                    skillSelectManager.SetClient(this);
                    skillSelectManager.playerData = _playerData;
                    skillSelectManager.skillData = _skillData;
                    skillSelectManager.passiveSkillData = _passiveSkillData;
                }
                else
                {
                    // This is a derived type of SkillSelectManager.
                    Debug.Log("This is a derived type of SkillSelectManager.");
                    skillSelectManager.SetClient(this);
                }
            }
            else
            {
                Debug.Log("No SkillSelectManager found.");
            }*/


            //==================================================================
            // Miji_SSM 싱글톤
            // 어쌔신의 경우
            if (playerClass == 2)
            {
                //miji_SSM = skillSelectManager.gameObject.AddComponent<Miji_SSM>();
                miji_SSM = FindAnyObjectByType<Miji_SSM>();

                miji_SSM.buttonActiveMijiSkillData = _buttonActiveMijiSkillData;
                miji_SSM.activeMijiSkillData = _activeMijiSkillData;
                miji_SSM.passiveMijiSkillData = _passiveMijiSkillData;
                miji_SSM.SetClient(this);
            }
            else
            {
                // playerClass가 2가 아닌 경우 Miji_SSM 싱글톤 객체를 찾아서 Destroy
                Miji_SSM miji_SSMToDestroy = FindAnyObjectByType<Miji_SSM>();
                if (miji_SSMToDestroy != null)
                {
                    Destroy(miji_SSMToDestroy.gameObject);
                }
            }

            //==================================================================
            // 보스 매니저
            bossManager = FindAnyObjectByType<BossManager>();
            bossManager.SetClient(this);
            bossManager.onBossSpawned = OnBossSpawned;
            bossManager.onBossHasKilled = // 공통
                (boss) =>
                {
                    stageManager.isStageClear = true;
                    playerManager.PlayerSpeedUp(); // 플레이어 속도 증가
                    rewardManager.BossReward(boss);
                    _playerData.magnetRange_Additional = 999f; // 플레이어 자석 범위 늘려주기
                };
            bossManager.onBossHasKilled +=
                sceneName == "Stage3" ? // 스테이지 별
                (boss) =>
                {
                    IEnumerator WaitForReward()
                    {
                        yield return new WaitForSeconds(1.5f);
                        inputManager.OnGameClear();
                    }
                    StartCoroutine(WaitForReward());
                } :
                (boss) => 
                {
                    stageManager.DestroyWalls();
                    enemyManager.enemies[1].Remove(boss);
                    minionManager.KillAllMinions();

                    switch (boss)
                    {
                        case FighterGoblin:
                            audioManager.SwitchBGM((int)AudioManager.Bgm.Stage1_Clear); // Stage1_Clear BGM
                            break;
                        case RuinedKing:
                            audioManager.SwitchBGM((int)AudioManager.Bgm.Stage2_Clear); // Stage2_Clear BGM
                            break;
                    }
                };
            // 보스가 생성하는 피조물(쫄몹, 박쥐, 석상 등) 액션
            bossManager.onCreatureChange = list =>
            {
                // enmies[2] : 잠시 생성되는 Enemy 리스트
                enemyManager.enemies[2].Clear();
                enemyManager.debugForEnemies2.Clear();

                foreach (var item in list)
                {
                    enemyManager.enemies[2].Add(item);
                    enemyManager.debugForEnemies2.Add(item);
                }
            };

            //==================================================================
            playerManager.SpawnPlayer();
            Init();

            //==================================================================
        }

        private void Init()
        {
            //==================================================================
            int playerClass = PlayerPrefs.GetInt("PlayerClass");
            if (playerClass == 2)
            {
                // [분신 관련 Action 모음]
                //==================================================================
                // 환영 리스트를 가져오는 Action 할당
                ((Assassin_SkillManager)skillManager).allocateIllusionAction = (List<Assassin_Illusion> assassin_Illusion_List) => {
                    minionManager.assassin_Illusions_List = assassin_Illusion_List;
                    minionManager.AllocateIllusionObject();
                };
            }

            //==================================================================
            // SkillSelectMAnager
            /*// GUI 패널 할당, 현재 GUIManager의 Awake에서 해주고 있음, SkillSelectManager의 OnEnable보다 빨라야 하기 때문, 더 나은 구조로 수정 필요
            skillSelectManager.activeSkillPanel = guiManager.activeSkillPanel;
            skillSelectManager.passiveSkillPanel = guiManager.passiveSkillPanel;
            skillSelectManager.specialSkillPanel = guiManager.specialSkillPanel;*/

            // skillSelectManager Action 할당
            skillSelectManager.onChoosingStartSkill = guiManager.OnChoosingStartSkill;
            skillSelectManager.onChoosingTestSkill = guiManager.OnChoosingTestSkill;
            skillSelectManager.onDisplayPanelNormalWay = guiManager.OnDisplayPanelNormalWay;
            skillSelectManager.onSkillAllMax = inputManager.OnSkillSelectFinished;
            skillSelectManager.onSkillAllMax += () =>
            {
                _playerData.additional_Power += 0.005f; // 스킬을 모두 선택했을 때 추가 파워 증가 (0.5% = 0.005f)
                PlayerManager.player.PowerUpEffectOn(); // 플레이어 파워 업 이펙트 켜기
            };
            skillSelectManager.onSetActiveSkillPanel = guiManager.OnSetActiveSkillPanel;
            skillSelectManager.onSetPassiveSkillPanel = guiManager.OnSetPassiveSkillPanel;
            skillSelectManager.onMaxLevelIncrease = guiManager.OnMaxLevelIncrease;
            skillSelectManager.onSkillSelected = skillManager.ResetDelayTimer;

            // 각 SSM에 따라 다르게 처리
            switch (skillSelectManager)
            {
                case Mage_SSM mage:
                    mage.onDisplayResonancePanel = guiManager.OnDisplayResonancePanel;
                    mage.onResonanceSkillSelect = guiManager.OnResonanceSkillSelect;
                    inputManager.shouldResonancePanelActivated = mage.ApplyResonanceToSkillPanel;
                    break;

                case Warrior_SSM warrior:
                    warrior.onDisplayUltimatePanel = guiManager.OnDisplayUltimatePanel;
                    warrior.onUltimateSkillSelect = guiManager.OnUltimateSkillSelect;
                    inputManager.shouldResonancePanelActivated = warrior.ApplyAltimateToSkillPanel;
                    break;

                /*case Assassin_SSM assassin:

                    break;*/

                default:
                    Debug.LogWarning("알 수 없는 SkillSelectManager입니다.");
                    break;
            }

            // 비급 관련 Action 할당
            if (playerClass == 2)
            {
                // 암연장막 비급 - 회피율 증감
                mijiSkillManager.onDodgeOn = playerManager.ApplyDodgeRate;
                mijiSkillManager.onDodgeOff = playerManager.RemoveDodgeRate;

                miji_SSM.onButtonActiveSkillSelected = mijiSkillManager.AddButtonActiveSkillIndex;
                miji_SSM.onActiveSkillSelected = mijiSkillManager.AddActiveSkillIndex;
                miji_SSM.onPassiveSkillSelected = mijiSkillManager.OnSelectPassiveMijiSkil;
                miji_SSM.onSkillIndicesAssigned = mijiSkillManager.AssignSkillIndices;

                //miji_SSM.onChoosingTestSkill = guiManager.OnChoosingTestSkill;
                miji_SSM.onDisplayMijiPanel = guiManager.OnDisplayMijiPanel;
                miji_SSM.onSetActiveMijiSkillPanel = guiManager.OnSetActiveMijiSkillPanel;
                miji_SSM.onSetPassiveMijiSkillPanel = guiManager.OnSetPassiveMijiSkillPanel;

                mijiSkillManager.onSelectPassiveMijiSkil = ((Assassin_SkillManager)skillManager).ReduceCooldown;
                mijiSkillManager.onButtonActiveSkillCasted = guiManager.UpdateButtonActiveSkillCooldownUI;
                mijiSkillManager.onActiveSkillCasted = guiManager.UpdateActiveSkillCooldownUI;
                mijiSkillManager.onStopAllCooldownCoroutines = guiManager.StopAllCooldownCoroutines;
                mijiSkillManager.onContinueButtonActiveSkillCooldown = guiManager.ContinueButtonActiveSkillCooldownUI;
                mijiSkillManager.onContinueActiveSkillCooldown = guiManager.ContinueActiveSkillCooldownUI;

                mijiSkillManager.onShadowStateChanged = ((Assassin_SkillManager)skillManager).SetShadowState;
            }

            //==================================================================
            // 스테이지 종료 이후, 실행할 동작이 다수 있을 수 있기에 += 으로 연결
            stageManager.onStageOver = minionManager.KillAllMinions;
            stageManager.onStageOver += () =>
            {
                bossManager.SpawnBoss();
                if (PlayerPrefs.GetInt("PlayerClass") == 2)
                    ((Assassin_SkillManager)skillManager).isBossAppear = true;
            };
            if (sceneName == "Stage2")
            {
                stageManager.onStageOver += () =>
                {
                    Player player = PlayerManager.player;
                    if (player.Y < -4.5f)
                        player.Y = -1.5f;
                };
            }
            stageManager.onSpawnWalls = (Vector2 innerMin, Vector2 innerMax) =>
            {
                spawnManager.innerMin = innerMin;
                spawnManager.innerMax = innerMax;
            };
        }

        IEnumerator Start()
        {
            yield return StartCoroutine(LoadingScreenOut());

            var managers = GetManagers();
            foreach (var manager in managers)
            {
                manager.Startup();
            }

            DoAfterStartUp();
        }

        void DoAfterStartUp()
        {
            if (sceneName == "Stage1")
            {
                skillSelectManager.damageMeters = new int[skillManager.MAX_SKILL_NUM];
                skillManager.damageMeters = skillSelectManager.damageMeters;
            }
            else
            {
                skillManager.damageMeters = skillSelectManager.damageMeters;
            }
        }

        void ChangeScene(string sceneName, bool isExitGameScene = false)
        {
            IEnumerator Process()
            {
                yield return StartCoroutine(LoadingScreenIn());

                // 비동기로 씬 로드 시작
                var ao = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(sceneName);
                ao.allowSceneActivation = false;

                // 씬이 로딩될 동안 대기
                while (ao.progress < 0.9f)
                {
                    yield return null;
                }

                if (isExitGameScene) DestroySingletonInstances();

                ao.allowSceneActivation = true;
            }

            isChangingScene = true;
            StartCoroutine(Process());
        }

        void DestroySingletonInstances()
        {
            // 씬이 완료되었으니 인스턴스 파괴 (로딩 완료 후 파괴)
            if (StageManager.instance != null)
            {
                Destroy(StageManager.instance.gameObject);
            }

            if (PlayerManager.instance != null)
            {
                Destroy(PlayerManager.instance.gameObject);
            }

            if(SavedataManager.instance != null)
            {
                Destroy(SavedataManager.instance.gameObject);   
            }

            int playerClass = PlayerPrefs.GetInt("PlayerClass");
            switch (playerClass)
            {
                case 0:
                    Destroy(Mage_SSM.instance?.gameObject); // 참조 여부 확인 후 파괴
                    break;
                case 1:
                    Destroy(Warrior_SSM.instance?.gameObject);
                    break;
                case 2:
                    Destroy(Assassin_SSM.instance?.gameObject);
                    Destroy(Miji_SSM.instance?.gameObject);
                    break;
            }
        }

        private void OnBossSpawned()
        {
            int playerClass = PlayerPrefs.GetInt("PlayerClass");

            enemyManager.enemies[1].Clear();
            enemyManager.debugForEnemies1.Clear();

            enemyManager.enemies[1].Add(bossManager.boss);
            enemyManager.debugForEnemies1.Add(bossManager.boss);

            // 어쌔신인 경우
            if (playerClass == 2)
            {
                ((Assassin_SkillManager)skillManager).isBossAppear = true;
                mijiSkillManager.isBossAppear = true;
            }

            switch (sceneName)
            {
                case "Stage1":
                    // 보스 머리 위 HP바(고블린 투사 전용) active하는 건 자체 보스 클래스에서 Instantiate하기
                    // Stage BGM 종료 후 보스 BGM ON
                    audioManager.SwitchBGM((int)AudioManager.Bgm.FighterGoblin); // 고블린 투사 전용 BGM
                    break;
                case "Stage2":
                    // 보스 머리 위 HP바(고블린 투사, 스켈레톤 킹 전용) active하는 건 자체 보스 클래스에서 Instantiate하기
                    // Stage BGM 종료 후 보스 BGM ON
                    audioManager.SwitchBGM((int)AudioManager.Bgm.RuinedKing); // 스켈레톤 킹 전용 BGM
                    break;

                case "Stage3":
                    // Stage BGM 종료 후 보스 BGM ON
                    audioManager.SwitchBGM((int)AudioManager.Bgm.Boss); // 벨리알 전용 BGM
                                                                        // 보스 상단 HP바(벨리알 전용) active
                    guiManager.bossHPStatus.gameObject.SetActive(true);
                    break;
            }

            spawnManager.isBossSpawned = true;
        }
    }
}