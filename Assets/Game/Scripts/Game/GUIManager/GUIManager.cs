using Eclipse.Game.Panels;
using Eclipse.Game.SkillSelect;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Eclipse.Game
{
    public partial class GUIManager : Eclipse.Manager
    {
        // 클래스 구분 짓기 위한 변수 
        [SerializeField] private int playerClass;

        //================================================================================
        [Header("SkillSelectManager 관련 변수들")]
        // SkillSelectManager 관련 변수들
        SkillSelectManager ssm;

        // 사용할 Scriptable Ojbect들
        [ReadOnly] public PlayerData playerData;
        [ReadOnly] public SkillData2 skillData;
        [ReadOnly] public SkillData2 passiveSkillData;
        [ReadOnly] public Server_PlayerData server_PlayerData;

        // 비급들
        [ReadOnly] public MijiSkillData buttonActiveMijiSkillData;
        [ReadOnly] public MijiSkillData activeMijiSkillData;
        [ReadOnly] public MijiSkillData passiveMijiSkillData;

        // 스킬 레벨 최대치가 증가했냐를 판단하는 변수
        [SerializeField][ReadOnly] bool hasMaxLevelIncreased;

        //================================================================================
        [Header("클래스 별 스킬 텍스트 색상")]
        // 클래스 별 스킬 텍스트 색상
        //================================================================================
        string[] mage_PassiveSkillEffectString = { "불 속성 쿨타임 감소", "전기 속성 쿨타임 감소", "물 속성 쿨타임 감소",
                                                    "데미지 감소", "이동속도 증가", "자석 범위 증가"};
        string[] mage_SkillColorCodeString = { "#F7570B", "#B4E9F0", "#4FA3C7" };
        string[] mage_PassiveSkillColorCodeString = { "#F7570B", "#B4E9F0", "#4FA3C7", "#FFFFFF" };

        //================================================================================
        string[] warrior_PassiveSkillEffectString = { "신성 계열 데미지 증가", "광기 계열 데미지 증가", "어둠 계열 데미지 증가",
                                                    "데미지 감소", "이동속도 증가", "자석 범위 증가"};
        string[] warrior_SkillColorCodeString = { "#FFF2A6", "#D82626", "#1F1F1F" };
        string[] warrior_PassiveSkillColorCodeString = { "#FFF2A6", "#D82626", "#1F1F1F", "#FFFFFF" };

        //================================================================================
        string[] assassin_PassiveSkillEffectString = { "암기류 크리티컬 확률 증가", "인술류 크리티컬 확률 증가", "함정류 크리티컬 확률 증가",
                                                    "데미지 감소", "이동속도 증가", "자석 범위 증가"};
        string[] assassin_SkillColorCodeString = { "#8A4EBF", "#F0F0F0", "#32860E" };
        string[] assassin_PassiveSkillColorCodeString = { "#8A4EBF", "#F0F0F0", "#32860E", "#FFFFFF" };

        string skillDescriptionColor = "#2E1B05"; // 클래스 별 스킬 설명 색상 지정

        string[][] passiveSkillEffectStrings;
        string[][] skillColorCodeStrings;
        string[][] passiveSkillColorCodeStrings;

        //================================================================================
        // 비급 관련

        // 비급 스킬 글자 색상
        string[] miji_SkillColorCodeStrings = { "#AF1740", "#9A7E6F" }; // 버튼 액티브, 찐 패시브
        // 비급 스킬 테두리 색상
        string[] miji_Boundary_Color = { "#740938", "#54473F" }; // 버튼 액티브, 찐 패시브

        // 비급 유형 2개 색 코드
        string[] mijiSkillColorCodeString = new string[2];

        //================================================================================
        string[] passiveSkillEffectString; // 패시브 스킬 효과 텍스트
        string[] skillColorCodeString = new string[3]; // 계열 3개 색 코드
        string[] passiveSkillColorCodeString = new string[4]; // 계열 3개 + 일반 패시브 스킬들 색 코드

        // 스킬 테두리 색상
        string[] boundary_Color = { "#BEBEBE", "#89F0FF", "#B634E0", "#F17F07", "#77EE00" };

        //================================================================================
        [Header("스테이지 설명 페이지")]
        // 스테이지 설명 페이지
        [ReadOnly] public StageDescription.StageDescriptionPageViewer stageDescriptionPageViewer;

        [ReadOnly] public StageDescription.StageImage stageImage;
        [ReadOnly] public StageDescription.StageName stageName;
        [ReadOnly] public StageDescription.StageDescription stageDescription;

        [ReadOnly] public StageDescription.AppearingEnemies appearingEnemies;

        [ReadOnly] public StageDescription.GimmickIcon gimmickIcon;
        [ReadOnly] public StageDescription.GimmickName gimmickName;
        [ReadOnly] public StageDescription.GimmickDescription gimmickDescription;

        [ReadOnly] public StageDescription.GameStartButton stageDescription_gameStartButton;

        //================================================================================
        //================================================================================
        [Header("패널 설정")]
        // 패널 설정
        [ReadOnly] public Panels.PanelViewer panelViewer;

        //================================================================================
        [Header("패널 버튼들")]
        // 패널 버튼들
        [ReadOnly] public SettingButton settingButton;
        [ReadOnly] public PauseButton pauseButton;
        [ReadOnly] public GameDescriptionButton gameDescriptionButton;
        [ReadOnly] public DamageMetersButton damageMetersButton;

        //================================================================================
        [Header("패널 HUD들")]
        // 패널 HUD들
        [ReadOnly] public Panels.ActiveSkillPanel activeSkillPanel; // 현재 선택된 액티브 스킬 패널
        [ReadOnly] public Panels.ActiveSkillPanel[] activeSkillPanels;
        [ReadOnly] public Panels.PassiveSkillPanel passiveSkillPanel;
        [ReadOnly] public Panels.SpecialSkillPanel specialSkillPanel;

        [ReadOnly] public Panels.BossHPStatus bossHPStatus;
        [ReadOnly] public Panels.CharacterProfile characterProfile;
        [ReadOnly] public Panels.ExpBar expBar;
        [ReadOnly] public Panels.PlayerHPSlider hpSlider;
        [ReadOnly] public Panels.PlayerHpStatus playerHpStatus;
        [ReadOnly] public Panels.PlayerLevel level;
        [ReadOnly] public Panels.KillText killText;
        [ReadOnly] public Panels.Timer timer;
        [ReadOnly] public Panels.GoldText goldText;
        [ReadOnly] public Panels.GoldPanel goldPanel;

        /* 비급용 */
        [ReadOnly] public Panels.MijiActiveSkillPanel activeMijiSkillPanel;
        [ReadOnly] public Panels.MijiPassiveSkillPanel passiveMijiSkillPanel;

        // 쿨타임 표시용
        [SerializeField] private ButtonActiveSkillImage[] buttonActiveSkillCooldownImages; // 버튼 액티브 스킬의 쿨타임 Image
        [SerializeField] private MijiCoolTimeText[] buttonActiveSkillCooldownTexts;   // 버튼 액티브 스킬의 쿨타임 Text
        [SerializeField] private ActiveSkillImage[] activeSkillCooldownImages;      // 액티브 스킬의 쿨타임 Image

        //================================================================================
        [Header("패널 Indicator들")]
        // 패널 Indicator들
        [ReadOnly] public Panels.Boss_Portal_Indicator boss_Portal_Indicator;
        [ReadOnly] public Panels.Portal_Indicator portal_Indicator;

        //================================================================================
        [Header("패널 UI들")]
        // 패턴 UI들
        [ReadOnly] public Panels.BossMessage bossMessage;
        [ReadOnly] public Panels.PatternTimer patternTimer;

        //================================================================================
        //================================================================================
        [Header("게임 설명 페이지")]
        // 게임 설명 페이지
        [ReadOnly] public GameDescriptionViewer gameDescriptionViewer;

        [ReadOnly] public GameDescriptionPage[] gameDescriptionPages;

        [ReadOnly] public GameDescriptionPageBackBotton gameDescriptionPageBackBotton;
        [ReadOnly] public GameDescriptionPageNextPageButton gameDescriptionPageNextPageButton;
        [ReadOnly] public GameDescriptionPagePreviousPageButton gameDescriptionPagePreviousPageButton;
        [ReadOnly] public GameDescriptionNextPage gameDescriptionNextPage;
        [ReadOnly] public GameDescriptionAppearPage gameDescriptionAppearPage;

        //================================================================================
        [Header("스킬 선택 페이지")]
        // 스킬 선택 페이지
        [ReadOnly] public SkillSelect.SkillSelectPageViewer skillSelectPageViewer;

        [ReadOnly] public SkillSelect.Title title;
        [ReadOnly] public SkillSelect.SkillPanel[] skillPanels;
        [ReadOnly] public SkillSelect.Level[] levels;
        [ReadOnly] public SkillSelect.RerollButton rerollButton;

        [ReadOnly] public SkillSelect.SkillButton[] skillButtons;

        [ReadOnly] public SkillSelect.Icon[] skillIcons;
        [ReadOnly] public SkillSelect.Name[] skillTextNames;
        [ReadOnly] public SkillSelect.Description[] skillTextDescriptions;
        [ReadOnly] public SkillSelect.Boundary[] skillBoundaries;
        [ReadOnly] public SkillSelect.TypeDescription[] typeDescriptions;

        //================================================================================
        [Header("비급 선택 페이지")]
        // 스킬 선택 페이지
        [ReadOnly] public SkillSelect.MijiSkillSelectPageViewer mijiSkillSelectPageViewer;

        [ReadOnly] public SkillSelect.MijiTitle mijiTitle;
        [ReadOnly] public SkillSelect.MijiSkillPanel[] mijiSkillPanels;
        [ReadOnly] public SkillSelect.MijiLevel[] mijiLevels;
        [SerializeField] private Sprite[] mijiTypeImages = new Sprite[2]; // 비급 계열 별 이미지 모음
        [ReadOnly] public SkillSelect.MijiRerollButton mijiRerollButton;

        [ReadOnly] public SkillSelect.MijiSkillButton[] mijiSkillButtons;

        [ReadOnly] public SkillSelect.MijiIcon[] mijiSkillIcons;
        [ReadOnly] public SkillSelect.MijiName[] mijiSkillTextNames;
        [ReadOnly] public SkillSelect.MijiDescription[] mijiSkillTextDescriptions;
        [ReadOnly] public SkillSelect.MijiBoundary[] mijiSkillBoundaries;
        [ReadOnly] public SkillSelect.MijiTypeDescription[] mijiTypeDescriptions;

        //================================================================================
        [Header("Pause 페이지")]
        // Pause 페이지
        [ReadOnly] public Pause.PausePageViewer pausePageViewer;
        [ReadOnly] public Pause.GoToLobbyButton pause_goToLobbyButton;
        [ReadOnly] public Pause.ReStartButton pause_reStartButton;
        [ReadOnly] public Pause.PlayButton pause_playButton;

        //================================================================================
        [Header("Setting 페이지")]
        // Setting 페이지
        [ReadOnly] public SettingPageViewer settingPageViewer;

        [ReadOnly] public SettingPageBackButton settingPage_backButton;

        [ReadOnly] public MasterSlider masterSlider;
        [ReadOnly] public BGMSlider bgmSlider;
        [ReadOnly] public SFXSlider sfxSlider;

        [ReadOnly] public MasterSoundLabel masterSoundLabel;
        [ReadOnly] public BGMSoundLabel bgmSoundLabel;
        [ReadOnly] public SFXSoundLabel sfxSoundLabel;

        [ReadOnly] public MasterFillImage masterFillImage;
        [ReadOnly] public BGMFillImage bgmFillImage;
        [ReadOnly] public SFXFillImage sfxFillImage;

        [ReadOnly] public MuteToggle muteToggle;
        [ReadOnly] public FullScreenToggle fullScreenToggle;

        //================================================================================
        [Header("Damage Meters 페이지")]
        [ReadOnly] public DamageMeters.DamageMetersViewer damageMetersViewer;
        [ReadOnly] public DamageMeters.Skills damageMeterSkills;
        [ReadOnly] public DamageMeters.PlayButton damageMeters_playButton;

        //================================================================================
        [Header("Game Over 페이지")]
        // Game Over 페이지
        [ReadOnly] public GameOver.GameOverPageViewer gameOverPageViewer;
        [ReadOnly] public GameOver.EnemyKillText enemyKillText_GameOver;
        [ReadOnly] public GameOver.GoldCollectedText goldCollectedText_GameOver;
        [ReadOnly] public GameOver.GoToLobbyButton gameOver_goToLobbyButton;
        [ReadOnly] public GameOver.ReStartButton gameOver_reStartButton;

        //================================================================================
        [Header("Game Clear 페이지")]
        // Game Clear 페이지
        [ReadOnly] public GameClear.GameClearPageViewer gameClearPageViewer;
        [ReadOnly] public GameClear.Skills skills;
        [ReadOnly] public GameClear.CharacterUnlockImage characterUnlockImage;
        [ReadOnly] public GameClear.ClearTimeText clearTimeText;
        [ReadOnly] public GameClear.EnemyKillText enemyKillText_GameClear;
        [ReadOnly] public GameClear.GoldCollectedText goldCollectedText_GameClear;
        [ReadOnly] public GameClear.ClearMainText clearMainText;
        [ReadOnly] public GameClear.GoToLobbyButton gameClear_goToLobbyButton;

        //================================================================================
        // 필요한 Action, delegate들
        public Action<string, bool> onChangeScene;

        //================================================================================

        private void Awake()
        {
            //================================================================================
            // 클래스 별 스킬 텍스트 색상 초기화
            passiveSkillEffectStrings = new string[][]
            {
                mage_PassiveSkillEffectString,
                warrior_PassiveSkillEffectString,
                assassin_PassiveSkillEffectString
            };

            skillColorCodeStrings = new string[][]
            {
                mage_SkillColorCodeString,
                warrior_SkillColorCodeString,
                assassin_SkillColorCodeString
            };

            passiveSkillColorCodeStrings = new string[][]
            {
                mage_PassiveSkillColorCodeString,
                warrior_PassiveSkillColorCodeString,
                assassin_PassiveSkillColorCodeString
            };


            //================================================================================
            // 스테이지 설명 페이지
            stageDescriptionPageViewer = GetComponentInChildren<StageDescription.StageDescriptionPageViewer>();

            stageImage = GetComponentInChildren<StageDescription.StageImage>();
            stageName = GetComponentInChildren<StageDescription.StageName>();
            stageDescription = GetComponentInChildren<StageDescription.StageDescription>();

            appearingEnemies = GetComponentInChildren<StageDescription.AppearingEnemies>();

            gimmickIcon = GetComponentInChildren<StageDescription.GimmickIcon>();
            gimmickName = GetComponentInChildren<StageDescription.GimmickName>();
            gimmickDescription = GetComponentInChildren<StageDescription.GimmickDescription>();

            stageDescription_gameStartButton = GetComponentInChildren<StageDescription.GameStartButton>();

            //================================================================================
            // 패널 설정
            panelViewer = GetComponentInChildren<Panels.PanelViewer>();

            //================================================================================
            // 패널 버튼들
            settingButton = GetComponentInChildren<SettingButton>();
            pauseButton = GetComponentInChildren<PauseButton>();
            gameDescriptionButton = GetComponentInChildren<GameDescriptionButton>();
            damageMetersButton = GetComponentInChildren<DamageMetersButton>();

            //================================================================================
            // 패널 HUD들
            bossHPStatus = GetComponentInChildren<Panels.BossHPStatus>();
            characterProfile = GetComponentInChildren<Panels.CharacterProfile>();
            expBar = GetComponentInChildren<Panels.ExpBar>();
            hpSlider = GetComponentInChildren<Panels.PlayerHPSlider>();
            playerHpStatus = GetComponentInChildren<Panels.PlayerHpStatus>();
            level = GetComponentInChildren<Panels.PlayerLevel>();
            killText = GetComponentInChildren<Panels.KillText>();
            timer = GetComponentInChildren<Panels.Timer>();
            goldText = GetComponentInChildren<Panels.GoldText>();
            goldPanel = GetComponentInChildren<Panels.GoldPanel>();

            //================================================================================
            // 패널 Indicator들
            boss_Portal_Indicator = GetComponentInChildren<Panels.Boss_Portal_Indicator>();
            portal_Indicator = GetComponentInChildren<Panels.Portal_Indicator>();

            //================================================================================
            // 패턴 UI들
            bossMessage = GetComponentInChildren<Panels.BossMessage>();
            patternTimer = GetComponentInChildren<Panels.PatternTimer>();

            //================================================================================
            //================================================================================
            // 게임 설명 페이지
            gameDescriptionViewer = GetComponentInChildren<GameDescriptionViewer>();
            gameDescriptionPages = GetComponentsInChildren<GameDescriptionPage>();

            gameDescriptionPageBackBotton = GetComponentInChildren<GameDescriptionPageBackBotton>();
            gameDescriptionPageNextPageButton = GetComponentInChildren<GameDescriptionPageNextPageButton>();
            gameDescriptionPagePreviousPageButton = GetComponentInChildren<GameDescriptionPagePreviousPageButton>();
            gameDescriptionNextPage = GetComponentInChildren<GameDescriptionNextPage>();
            gameDescriptionAppearPage = GetComponentInChildren<GameDescriptionAppearPage>();

            //================================================================================
            // 스킬 선택 페이지 설정
            skillSelectPageViewer = GetComponentInChildren<SkillSelect.SkillSelectPageViewer>();

            title = GetComponentInChildren<SkillSelect.Title>();
            skillPanels = GetComponentsInChildren<SkillSelect.SkillPanel>();
            levels = GetComponentsInChildren<SkillSelect.Level>();
            rerollButton = GetComponentInChildren<SkillSelect.RerollButton>();

            skillButtons = GetComponentsInChildren<SkillSelect.SkillButton>();

            skillIcons = GetComponentsInChildren<SkillSelect.Icon>();
            skillTextNames = GetComponentsInChildren<SkillSelect.Name>();
            skillTextDescriptions = GetComponentsInChildren<SkillSelect.Description>();
            skillBoundaries = GetComponentsInChildren<SkillSelect.Boundary>();
            typeDescriptions = GetComponentsInChildren<SkillSelect.TypeDescription>();
            
            //================================================================================
            // 비급 선택 페이지(어쌔신 전용) 설정
            mijiSkillSelectPageViewer = GetComponentInChildren<SkillSelect.MijiSkillSelectPageViewer>();

            mijiTitle = GetComponentInChildren<SkillSelect.MijiTitle>();
            mijiSkillPanels = GetComponentsInChildren<SkillSelect.MijiSkillPanel>();
            mijiLevels = GetComponentsInChildren<SkillSelect.MijiLevel>();
            mijiRerollButton = GetComponentInChildren<SkillSelect.MijiRerollButton>();

            mijiSkillButtons = GetComponentsInChildren<SkillSelect.MijiSkillButton>();

            mijiSkillIcons = GetComponentsInChildren<SkillSelect.MijiIcon>();
            mijiSkillTextNames = GetComponentsInChildren<SkillSelect.MijiName>();
            mijiSkillTextDescriptions = GetComponentsInChildren<SkillSelect.MijiDescription>();
            mijiSkillBoundaries = GetComponentsInChildren<SkillSelect.MijiBoundary>();
            mijiTypeDescriptions = GetComponentsInChildren<SkillSelect.MijiTypeDescription>();

            mijiTypeImages[0] = Resources.Load<Sprite>("Sprites/Skill Icons/Assassin Skill Icon/Miji/Active Miji Icon");
            mijiTypeImages[1] = Resources.Load<Sprite>("Sprites/Skill Icons/Assassin Skill Icon/Miji/Passive Miji Icon");

            //================================================================================
            // Pause 페이지
            pausePageViewer = GetComponentInChildren<Pause.PausePageViewer>();
            pause_goToLobbyButton = GetComponentInChildren<Pause.GoToLobbyButton>();
            pause_reStartButton = GetComponentInChildren<Pause.ReStartButton>();
            pause_playButton = GetComponentInChildren<Pause.PlayButton>();

            //================================================================================
            // Setting 페이지
            settingPageViewer = GetComponentInChildren<SettingPageViewer>();

            settingPage_backButton = GetComponentInChildren<SettingPageBackButton>();

            masterSlider = GetComponentInChildren<MasterSlider>();
            bgmSlider = GetComponentInChildren<BGMSlider>();
            sfxSlider = GetComponentInChildren<SFXSlider>();

            masterSoundLabel = GetComponentInChildren<MasterSoundLabel>();
            bgmSoundLabel = GetComponentInChildren<BGMSoundLabel>();
            sfxSoundLabel = GetComponentInChildren<SFXSoundLabel>();

            masterFillImage = GetComponentInChildren<MasterFillImage>();
            bgmFillImage = GetComponentInChildren<BGMFillImage>();
            sfxFillImage = GetComponentInChildren<SFXFillImage>();

            muteToggle = GetComponentInChildren<MuteToggle>();
            fullScreenToggle = GetComponentInChildren<FullScreenToggle>();

            //================================================================================
            // Damage Meters 페이지
            damageMetersViewer = GetComponentInChildren<DamageMeters.DamageMetersViewer>();
            damageMeterSkills = GetComponentInChildren<DamageMeters.Skills>();
            damageMeters_playButton = GetComponentInChildren<DamageMeters.PlayButton>();

            //================================================================================
            // Game Over 페이지
            gameOverPageViewer = GetComponentInChildren<GameOver.GameOverPageViewer>();
            enemyKillText_GameOver = GetComponentInChildren<GameOver.EnemyKillText>();
            goldCollectedText_GameOver = GetComponentInChildren<GameOver.GoldCollectedText>();
            gameOver_goToLobbyButton = GetComponentInChildren<GameOver.GoToLobbyButton>();
            gameOver_reStartButton = GetComponentInChildren<GameOver.ReStartButton>();

            //================================================================================
            // Game Clear 페이지
            gameClearPageViewer = GetComponentInChildren<GameClear.GameClearPageViewer>();
            skills = GetComponentInChildren<GameClear.Skills>();
            characterUnlockImage = GetComponentInChildren<GameClear.CharacterUnlockImage>();
            clearTimeText = GetComponentInChildren<GameClear.ClearTimeText>();
            enemyKillText_GameClear = GetComponentInChildren<GameClear.EnemyKillText>();
            goldCollectedText_GameClear = GetComponentInChildren<GameClear.GoldCollectedText>();
            clearMainText = GetComponentInChildren<GameClear.ClearMainText>();
            gameClear_goToLobbyButton = GetComponentInChildren<GameClear.GoToLobbyButton>();

            //================================================================================
            server_PlayerData = Resources.Load<Server_PlayerData>("Datas/Server_PlayerData");
        }

        private void OnEnable()
        {
            activeMijiSkillPanel = GetComponentInChildren<Panels.MijiActiveSkillPanel>();
            passiveMijiSkillPanel = GetComponentInChildren<Panels.MijiPassiveSkillPanel>();

            buttonActiveSkillCooldownImages = GetComponentsInChildren<Panels.ButtonActiveSkillImage>();
            activeSkillCooldownImages = GetComponentsInChildren<Panels.ActiveSkillImage>();
            buttonActiveSkillCooldownTexts = GetComponentsInChildren<Panels.MijiCoolTimeText>();
        }

        private void Start()
        {
            //================================================================================
            // playerData 할당 이후 초기화 부분
            //================================================================================
            // 플레이어 클래스에 맞는 스킬 데이터 할당
            playerClass = PlayerPrefs.GetInt("PlayerClass");
            passiveSkillEffectString = passiveSkillEffectStrings[playerClass];  // 스킬 효과
            skillColorCodeString = skillColorCodeStrings[playerClass];  // 스킬 색상 코드
            passiveSkillColorCodeString = passiveSkillColorCodeStrings[playerClass];  // 패시브 스킬 색상 코드

            //================================================================================
            // 패널 HUD들 초기화
            activeSkillPanels = GetComponentsInChildren<Panels.ActiveSkillPanel>();
            for (int i = 0; i < activeSkillPanels.Length; i++)
            {
                activeSkillPanels[i] = ActiveSkillPanelFactory.Create(playerData, activeSkillPanels[i].gameObject);
                Destroy(activeSkillPanels[i].gameObject.GetComponent<Panels.ActiveSkillPanel>());
            }

            if (activeSkillPanels == null) Debug.LogError($"{playerClass}는 유효하지 않는 PlayerClass입니다.");
            passiveSkillPanel = GetComponentInChildren<Panels.PassiveSkillPanel>();
            specialSkillPanel = GetComponentInChildren<Panels.SpecialSkillPanel>();

            // 순서 차이 때문에 이 때 초기화
            ssm = client.GetManager<SkillSelectManager>();
            ssm.activeSkillPanels = activeSkillPanels;
            ssm.passiveSkillPanel = passiveSkillPanel;
            ssm.specialSkillPanel = specialSkillPanel;
            
            //================================================================================
            // 비급 패널 HUD도 마찬가지로 초기화
            // 어쌔신인 경우에만 비급 세팅

            mijiSkillColorCodeString = miji_SkillColorCodeStrings;

            if (playerClass == 2)
            {
                // 순서 차이 때문에 이 때 초기화
                Miji_SSM miji_SSM = client.GetManager<Miji_SSM>();

                miji_SSM.activeMijiSkillPanel = activeMijiSkillPanel;
                miji_SSM.passiveMijiSkillPanel = passiveMijiSkillPanel;
            }

            //================================================================================
            //================================================================================
            // 스테이지 설명 페이지
            int sceneIndex = SceneManager.GetActiveScene().name switch
            {
                "Stage1" => 0,
                "Stage2" => 1,
                "Stage3" => 2,
                _ => -1
            };
            if (sceneIndex == -1) { Debug.LogError($"현재 Stage Scene이 아닙니다."); }
            ChangeStageSetting(sceneIndex);

            stageDescriptionPageViewer.Hide();

            //================================================================================
            //================================================================================
            // 패널 설정
            hpSlider.gameObject.SetActive(false);

            //================================================================================
            // 패널 버튼들
            settingButton.Hide();
            pauseButton.Hide();
            gameDescriptionButton.Hide();
            damageMetersButton.Hide();

            //================================================================================
            // 패널 HUD들
            level.PlayerData = playerData;
            expBar.PlayerData = playerData;
            hpSlider.PlayerData = playerData;
            playerHpStatus.PlayerData = playerData;
            killText.PlayerData = playerData;
            goldText.Server_PlayerData = server_PlayerData;
            timer.StageManager = client.GetManager<StageManager>();

            passiveSkillPanel.SkillData = skillData;
            specialSkillPanel.SkillData = skillData;

            goldPanel.Hide();
            playerHpStatus.gameObject.SetActive(false);
            SkillSelectManager skillSelectManager = client.GetManager<SkillSelectManager>();
            if (skillSelectManager.isSpecialSkillAlreadyChoosed)
            {
                activeSkillPanel = activeSkillPanels[1];
            }
            else
            {
                activeSkillPanel = activeSkillPanels[0];
            }
            for (int i = 0; i < activeSkillPanels.Length; i++)
            {
                activeSkillPanels[i].SkillData = skillData;
                activeSkillPanels[i].gameObject.SetActive(false);
            }
            passiveSkillPanel.gameObject.SetActive(false);
            specialSkillPanel.gameObject.SetActive(false);

            // 비급 패시브/액티브 패널
            activeMijiSkillPanel.gameObject.SetActive(false);
            passiveMijiSkillPanel.gameObject.SetActive(false);

            // 쿨타임 표시 이미지/텍스트
            for (int i = 0; i < buttonActiveSkillCooldownImages.Length; i++)
            {
                buttonActiveSkillCooldownImages[i].Hide();
            }

            for (int i = 0; i < activeSkillCooldownImages.Length; i++)
            {
                activeSkillCooldownImages[i].Hide();
            }

            for (int i = 0; i < buttonActiveSkillCooldownTexts.Length; i++)
            {
                buttonActiveSkillCooldownTexts[i].Hide();
            }
            
            bossHPStatus.gameObject.SetActive(false);
            bossHPStatus.BossManager = client.GetManager<BossManager>();

            //================================================================================
            // 패널 Indicator들
            boss_Portal_Indicator.gameObject.SetActive(false);
            portal_Indicator.gameObject.SetActive(false);

            //================================================================================
            // 패턴 UI들
            bossMessage.gameObject.SetActive(false);
            patternTimer.gameObject.SetActive(false);

            //================================================================================
            //================================================================================
            // 게임 설명 페이지
            gameDescriptionViewer.Hide();

            //================================================================================
            // 스킬 선택 페이지
            skillSelectPageViewer.Hide();
            
            // Open Scroll은 true, Closed Scroll은 false
            for (int i = 0; i < skillPanels.Length; i++)
            {
                if (i < 3) 
                    skillPanels[i].gameObject.SetActive(true);
                else 
                    skillPanels[i].gameObject.SetActive(false);
            }
            // 레벨 오브젝트 활성화
            for (int i = 0; i < levels.Length; i++) levels[i].gameObject.SetActive(true);

            // 리롤 오브젝트 enabled, SetActive true
            rerollButton.server_PlayerData = server_PlayerData;
            rerollButton.enabled = true;
            rerollButton.gameObject.SetActive(true);
            rerollButton.SetRerollNum();

            //================================================================================
            // 비급 선택 페이지
            mijiSkillSelectPageViewer.Hide();

            // Open Scroll은 true
            for (int i = 0; i < mijiSkillPanels.Length; i++)
            {
                mijiSkillPanels[i].gameObject.SetActive(true);
            }
            // 레벨 오브젝트 활성화
            for (int i = 0; i < mijiLevels.Length; i++) mijiLevels[i].gameObject.SetActive(true);

            //================================================================================
            // Pause 페이지
            pausePageViewer.Hide();

            //================================================================================
            // Setting 페이지
            settingPageViewer.Hide();

            //================================================================================
            // Damage Meters 페이지
            damageMetersViewer.Hide();

            //================================================================================
            // Game Over 페이지
            gameOverPageViewer.Hide();

            //================================================================================
            // Game Clear 페이지
            gameClearPageViewer.Hide();

            //================================================================================
        }

        // 싱글톤 하려면 필요한 함수
        /*private void Init()
        {
            //==================================================================
            // 변수 초기화
            hasMaxLevelIncreased = false;

            //==================================================================
            // 스킬 패널 크기 조정
            skillPanels[1].transform.localScale = new Vector3(1, 1, 1);

            // 스킬 패널 위치 변경
            Vector2 vector2 = skillPanels[1].transform.localPosition;
            vector2.y = -10;
            skillPanels[1].transform.localPosition = vector2;

            //==================================================================
        }
        // 로비, 스플레쉬에서는 안보이게, 당장은 안씀 9월에 개선할 때 쓰려면 쓸 것
        public void JudgeActivateCanvas(GameObject canvasObject)
        {
            bool isSplashScene =
                SceneManager.GetActiveScene().name == "Lobby" ||
                SceneManager.GetActiveScene().name == "Splash1" ||
                SceneManager.GetActiveScene().name == "Splash2" ||
                SceneManager.GetActiveScene().name == "Splash3";

            if (isSplashScene) { canvasObject.SetActive(false); }
            else { canvasObject.SetActive(true); }
        }*/

        //================================================================================
        // 스테이지 설명 페이지 관련 함수

        // 스테이지 설명 설정
        void ChangeStageSetting(int stageIndex)
        {
            stageImage.ChangeImage(stageIndex);
            stageName.ChangeImage(stageIndex);
            stageDescription.ChangeImage(stageIndex);
            appearingEnemies.ChangeImage(stageIndex);
            gimmickIcon.ChangeImage(stageIndex);
            gimmickName.ChangeImage(stageIndex);
            gimmickDescription.ChangeImage(stageIndex);
        }

        //================================================================================
        // SkillSelectManager 관련 함수들

        // 시작 스킬 선택 상황 시 처리
        public void OnChoosingStartSkill()
        {
            rerollButton.gameObject.SetActive(false);

            title.GetComponentInChildren<TextMeshProUGUI>().text = "스킬 선택";

            for (int i = 0; i < 3; i++)
            {
                Image icon = skillIcons[i].GetComponent<Image>();
                icon.sprite = skillData.skillicon[i];
                string color = skillColorCodeString[i % 3];

                TextMeshProUGUI textName = skillTextNames[i].GetComponent<TextMeshProUGUI>();
                textName.text = $"<color={color}>{skillData.skillName[i]}</color>";
                TextMeshProUGUI textDescription = skillTextDescriptions[i].GetComponent<TextMeshProUGUI>();

                TextMeshProUGUI skillTypeText = typeDescriptions[i].GetComponent<TextMeshProUGUI>();

                // 유저 친화적인 '스킬 설명' 인터페이스 위한 카테고리 분류
                switch (playerClass)
                {
                    case 0: // "Mage"
                        if (i == 0)
                        {
                            skillTypeText.text = "불\n";
                        }
                        else if (i == 1)
                        {
                            skillTypeText.text = "전기\n";
                        }
                        else
                        {
                            skillTypeText.text = "물\n";
                        }
                        break;
                    case 1: // "Warrior"
                        if (i == 0)
                        {
                            skillTypeText.text = "신성\n";
                        }
                        else if (i == 1)
                        {
                            skillTypeText.text = "광기\n";
                        }
                        else
                        {
                            skillTypeText.text = "암흑\n";
                        }
                        break;
                    case 2: // "Assassin"
                        if (i == 0)
                        {
                            skillTypeText.text = "암기\n";
                        }
                        else if (i == 1)
                        {
                            skillTypeText.text = "인술\n";
                        }
                        else
                        {
                            skillTypeText.text = "함정\n";
                        }
                        break;
                }

                string description = "";

                description += skillData.skillDescription[i] + "\n";

                textDescription.text = $"<color={skillDescriptionColor}>{description}</color>"; // 스킬 설명 글씨색 흰색으로 통일

                levels[i].gameObject.SetActive(true);
                StartCoroutine(SetLevelObjectAlpha(i, skillData.level[i]));

                // 여기에 스킬 바운더리 색 설정
                skillBoundaries[i].gameObject.SetActive(true);
                skillBoundaries[i].GetComponent<Image>().color = HexToColor(boundary_Color[0]);
            }
        }

        // 스킬 아이콘 및 설명 설정
        public void OnChoosingTestSkill(int panelIndex, int testSkillIndex, bool isDotDamageSkill)
        {
            Image icon = skillIcons[panelIndex].GetComponent<Image>();
            icon.sprite = skillData.skillicon[testSkillIndex];
            string color = skillColorCodeString[panelIndex % 3];
            TextMeshProUGUI textName = skillTextNames[panelIndex].GetComponent<TextMeshProUGUI>();
            textName.text = $"<color={color}>{skillData.skillName[testSkillIndex]}</color>";
            TextMeshProUGUI textDescription = skillTextDescriptions[panelIndex].GetComponent<TextMeshProUGUI>();

            if (typeDescriptions[panelIndex].GetComponentInParent<TypeFrame>() == null)
            {
                typeDescriptions[panelIndex].transform.parent.gameObject.SetActive(true);
            }

            TextMeshProUGUI skillTypeText = typeDescriptions[panelIndex].GetComponent<TextMeshProUGUI>();
            // 유저 친화적인 '스킬 설명' 인터페이스 위한 카테고리 분류
            switch (playerClass)
            {
                case 0: // "Mage"
                    if (testSkillIndex % 3 == 0)
                    {
                        skillTypeText.text = "불\n";
                    }
                    else if (testSkillIndex % 3 == 1)
                    {
                        skillTypeText.text = "전기\n";
                    }
                    else
                    {
                        skillTypeText.text = "물\n";
                    }
                    break;
                case 1: // "Warrior"
                    if (testSkillIndex % 3 == 0)
                    {
                        skillTypeText.text = "신성\n";
                    }
                    else if (testSkillIndex % 3 == 1)
                    {
                        skillTypeText.text = "광기\n";
                    }
                    else
                    {
                        skillTypeText.text = "암흑\n";
                    }
                    break;
                case 2: // "Assassin"
                    if (testSkillIndex % 3 == 0)
                    {
                        skillTypeText.text = "암기\n";
                    }
                    else if (testSkillIndex % 3 == 1)
                    {
                        skillTypeText.text = "인술\n";
                    }
                    else
                    {
                        skillTypeText.text = "함정\n";
                    }
                    break;
            }

            string description = "";

            description += skillData.skillDescription[testSkillIndex] + "\n";

            if (isDotDamageSkill)
                description += "도트 데미지: ";
            else description += "데미지: ";

            if (testSkillIndex == 12) description += 60;
            else description += skillData.damage[testSkillIndex];

            description += "\n쿨타임: " + skillData.delay[testSkillIndex] + "초";

            textDescription.text = $"<color={skillDescriptionColor}>{description}</color>"; // 스킬 설명 글씨색 흰색으로 통일

            levels[panelIndex].gameObject.SetActive(true);
            Image[] img = levels[panelIndex].GetComponentsInChildren<Image>();

            for (int num = 4 - skillData.level[testSkillIndex]; num >= 0; num--)
            {
                var col = img[num].color;
                col.a = 0.3f;
                img[num].color = col;
            }
            for (int num = 0; num < skillData.level[testSkillIndex]; num++)
            {
                var col = img[num].color;
                col.a = 1f;
                img[num].color = col;
            }

            // 여기에 스킬 바운더리 색 설정
            skillBoundaries[panelIndex].GetComponent<Image>().color = testSkillIndex switch
            {
                < 3 => HexToColor(boundary_Color[0]),
                < 6 => HexToColor(boundary_Color[1]),
                < 9 => HexToColor(boundary_Color[2]),
                < 12 => HexToColor(boundary_Color[3]),
                _ => HexToColor(boundary_Color[4])
            };
        }

        // 일반적인 스킬 선택 시 처리
        public void OnDisplayPanelNormalWay(int displayCount)
        {
            rerollButton.gameObject.SetActive(true);

            title.GetComponentInChildren<TextMeshProUGUI>().text = "레벨업!";

            // 스킬 패널 개수에 따른 UI 설정
            skillPanels[2].gameObject.SetActive(displayCount >= 3);
            skillPanels[4].gameObject.SetActive(displayCount < 3);
            levels[2].gameObject.SetActive(displayCount >= 3);

            skillPanels[1].gameObject.SetActive(true);
            levels[1].gameObject.SetActive(true);

            skillPanels[0].gameObject.SetActive(displayCount >= 2);
            skillPanels[3].gameObject.SetActive(displayCount < 2);
            levels[0].gameObject.SetActive(displayCount >= 2);
        }

        // 액티브 스킬 레벨 오브젝트 알파값 설정
        private IEnumerator SetLevelObjectAlpha(int index, int level)
        {
            yield return null; // 프레임 대기, 전사 각성 후 레벨 오브젝트 활성화가 프레임 딜레이로 인해 제대로 안되는 문제 해결
            
            if (hasMaxLevelIncreased)
            {
                for (int i = 5; i < 7; i++)
                {
                    GameObject circle = levels[index].transform.GetChild(i).gameObject;
                    circle.SetActive(true);
                    }
            }

            Image[] img = levels[index].GetComponentsInChildren<Image>();

            if (hasMaxLevelIncreased)
            {
                for (int num = 6; num >= level; num--)
                {
                    Color col = img[num].color;
                    col.a = 0.3f;
                    img[num].color = col;
                }
                for (int num = 0; num < level; num++)
                {
                    Color col = img[num].color;
                    col.a = 1f;
                    img[num].color = col;
                }
            }
            else
            {
                for (int num = 4; num >= level; num--)
                {
                    Color col = img[num].color;
                    col.a = 0.3f;
                    img[num].color = col;
                }
                for (int num = 0; num < level; num++)
                {
                    Color col = img[num].color;
                    col.a = 1f;
                    img[num].color = col;
                }
            }
        }

        // 패시브 스킬 레벨 오브젝트 알파값 설정 (각성 후)
        private void SetPassiveSkillLevelObjectAlpha(int index, int level)
        {
            if (hasMaxLevelIncreased)
            {
                for (int i = 5; i < 10; i++)
                    levels[index].transform.GetChild(i).gameObject.SetActive(false);
            }

            Image[] img = levels[index].GetComponentsInChildren<Image>();

            for (int num = 4; num >= level; num--)
            {
                Color col = img[num].color;
                col.a = 0.3f;
                img[num].color = col;
            }
            for (int num = 0; num < level; num++)
            {
                Color col = img[num].color;
                col.a = 1f;
                img[num].color = col;
            }
        }

        // 일반 스킬 패널 설정 (SkillSelectPageViewer)
        public void OnSetActiveSkillPanel(int index, int skillIndex, bool isDotDamageSkill)
        {
            Image icon = skillIcons[index].GetComponent<Image>();
            icon.sprite = skillData.skillicon[skillIndex];

            string color = skillColorCodeString[skillIndex % 3];
            TextMeshProUGUI textName = skillTextNames[index].GetComponent<TextMeshProUGUI>();
            textName.text = $"<color={color}>{skillData.skillName[skillIndex]}</color>";

            TextMeshProUGUI textDescription = skillTextDescriptions[index].GetComponent<TextMeshProUGUI>();

            if (typeDescriptions[index].GetComponentInParent<TypeFrame>() == null)
            {
                typeDescriptions[index].transform.parent.gameObject.SetActive(true);
            }

            TextMeshProUGUI skillTypeText = typeDescriptions[index].GetComponent<TextMeshProUGUI>();
            // 유저 친화적인 '스킬 설명' 인터페이스 위한 카테고리 분류
            switch (playerClass)
            {
                case 0: // "Mage"
                    if (skillIndex % 3 == 0)
                    {
                        skillTypeText.text = "불\n";
                    }
                    else if (skillIndex % 3 == 1)
                    {
                        skillTypeText.text = "전기\n";
                    }
                    else
                    {
                        skillTypeText.text = "물\n";
                    }
                    break;
                case 1: // "Warrior"
                    if (skillIndex % 3 == 0)
                    {
                        skillTypeText.text = "신성\n";
                    }
                    else if (skillIndex % 3 == 1)
                    {
                        skillTypeText.text = "광기\n";
                    }
                    else
                    {
                        skillTypeText.text = "암흑\n";
                    }
                    break;
                case 2: // "Assassin"
                    if (skillIndex % 3 == 0)
                    {
                        skillTypeText.text = "암기\n";
                    }
                    else if (skillIndex % 3 == 1)
                    {
                        skillTypeText.text = "인술\n";
                    }
                    else
                    {
                        skillTypeText.text = "함정\n";
                    }
                    break;
            }

            string description = "";

            description += skillData.level[skillIndex] == 0 ?
                SetActiveSkillDescription(skillIndex) : SetActiveSkillIncrementValue(skillIndex, isDotDamageSkill);

            //textDescription.text = $"<color={color}>{description}</color>";
            textDescription.text = $"<color={skillDescriptionColor}>{description}</color>"; // 스킬 설명 글씨색 흰색으로 통일

            StartCoroutine(SetLevelObjectAlpha(index, skillData.level[skillIndex]));

            skillBoundaries[index].gameObject.SetActive(true);
            // 여기에 스킬 바운더리 색 설정
            skillBoundaries[index].GetComponent<Image>().color = skillIndex switch
            {
                < 3 => HexToColor(boundary_Color[0]),
                < 6 => HexToColor(boundary_Color[1]),
                < 9 => HexToColor(boundary_Color[2]),
                < 12 => HexToColor(boundary_Color[3]),
                _ => HexToColor(boundary_Color[4])
            };
        }

        string SetActiveSkillDescription(int skillIndex)
        {
            return skillData.skillDescription[skillIndex];
        }

        string SetActiveSkillIncrementValue(int skillIndex, bool isDotDamageSkill)
        {
            int playerClass = PlayerPrefs.GetInt("PlayerClass"); // 플레이어 직업 알기 위해서..

            float damage = skillData.damage[skillIndex];
            float delay = skillData.delay[skillIndex];
            float normalDamage = Mathf.Floor(damage * ssm.normalDamageCoefficient * 100) / 100;
            float normalDelay = Mathf.Floor(delay * ssm.normalDelayCoefficient * 100) / 100;
            bool isSelected = skillData.skillSelected[skillIndex];
            int skillLevel = skillData.level[skillIndex];

            // 스킬 별 올라갈 항목
            var valueNames = skillData.levelIncrementType[skillIndex].Split(','); // 쉼표를 기준으로 문자열을 분리

            string front_levelIncrementType = valueNames[0];
            string back_levelIncrementType = valueNames.Length > 1 ? valueNames[1] : "";

            // 3레벨에 스킬 별 올라갈 수치
            var threeValues = skillData.levelThreeIncrementValue[skillIndex].Split(','); // 쉼표를 기준으로 문자열을 분리

            string front_levelThreeIncrementValue = threeValues[0];
            string back_levelThreeIncrementValue = threeValues.Length > 1 ? threeValues[1] : "";

            // 5레벨에 스킬 별 올라갈 수치
            var maxValues = skillData.levelMaxIncrementValue[skillIndex].Split(','); // 쉼표를 기준으로 문자열을 분리

            string front_levelMaxIncrementValue = maxValues[0];
            string back_levelMaxIncrementValue = maxValues.Length > 1 ? maxValues[1] : "";

            string description = "";

            // Description based on skill type
            if (!(playerClass == 0 && skillIndex == 8))
            {
                description = isDotDamageSkill ? "도트 데미지: " : "데미지: ";
                description += isSelected ? $"{damage:F2} + {normalDamage - damage:F2}\n" : $"{damage:F0}\n";
            }

            // Cooldown description
            description += $"쿨타임: {delay:F2}초";
            if (isSelected)
            {
                description += $" - {(delay - normalDelay):F2}초";
            }

            // 레벨업 수치 표기
            if (skillLevel == 2)
            {
                if (back_levelIncrementType == "")
                {
                    description += "\n스킬 " + front_levelIncrementType + " 증가 : " + front_levelThreeIncrementValue;
                }
                else // 두번째 수치 있는 경우
                {
                    // 청운검격, 염화단도, 혼마화진 - 3레벨 경우 범위만 증가, 두번째 효과 표시 X
                    if (playerClass == 2 && (skillIndex == 3 || skillIndex == 6 || skillIndex == 11))
                    {
                        description += "\n스킬 " + front_levelIncrementType + " 증가: " + front_levelThreeIncrementValue;
                    }
                    else
                    {
                        description += "\n스킬 " + front_levelIncrementType + " 증가: " + front_levelThreeIncrementValue +
                            "\n" + back_levelIncrementType + " 증가: " + back_levelThreeIncrementValue;
                    }
                }
            }
            else if (skillLevel == 4)
            {
                if (back_levelIncrementType == "")
                {
                    description += "\n스킬 " + front_levelIncrementType + " 증가 : " + front_levelMaxIncrementValue;

                    if (playerClass == 2)
                    {
                        if (skillIndex == 9)
                        {
                            description += "\n보스전 돌입시 변화합니다.";
                        }
                    }
                }
                else // 두번째 수치 있는 경우
                {
                    // 염화단도, 혼마화진
                    if (playerClass == 2 && (skillIndex == 6 || skillIndex == 11))
                    {
                        back_levelMaxIncrementValue += "개"; // 5 레벨 경우 개수도 증가 및 표시 O
                    }

                    description += "\n스킬 " + front_levelIncrementType + " 증가: " + front_levelMaxIncrementValue +
                            "\n" + back_levelIncrementType + " 증가: " + back_levelMaxIncrementValue;
                }
            }

            return description;
        }


        // 패시브 스킬 패널 설정 (SkillSelectPageViewer)
        public void OnSetPassiveSkillPanel(int index, int skillIndex, int PASSIVESKILLNUM)
        {
            if (typeDescriptions[index].GetComponentInParent<TypeFrame>() != null)
            {
                typeDescriptions[index].GetComponentInParent<TypeFrame>().gameObject.SetActive(false);
            }

            //if (typeDescriptions[index].transform.parent.gameObject.activeSelf)
            //{
            //    typeDescriptions[index].transform.parent.gameObject.SetActive(false);
            //}

            Image icon = skillIcons[index].GetComponent<Image>();
            icon.sprite = passiveSkillData.skillicon[skillIndex - PASSIVESKILLNUM];

            string color = (skillIndex - PASSIVESKILLNUM) switch
            {
                0 => passiveSkillColorCodeString[0],
                1 => passiveSkillColorCodeString[1],
                2 => passiveSkillColorCodeString[2],
                _ => passiveSkillColorCodeString[3]
            };
            TextMeshProUGUI textName = skillTextNames[index].GetComponent<TextMeshProUGUI>();
            textName.text = $"<color={color}>{passiveSkillData.skillName[skillIndex - PASSIVESKILLNUM]}</color>";
            
            TextMeshProUGUI textDescription = skillTextDescriptions[index].GetComponent<TextMeshProUGUI>();
            string description = "";

            int passiveIndex = skillIndex - PASSIVESKILLNUM;
            float damage = passiveSkillData.damage[passiveIndex];
            bool isSkillSelected = passiveSkillData.skillSelected[passiveIndex];

            if (isSkillSelected)
            {
                description = GetSelectedSkillDescription(skillIndex, damage, PASSIVESKILLNUM);
            }
            else
            {
                description = GetUnselectedSkillDescription(skillIndex, PASSIVESKILLNUM);
            }

            //textDescription.text = $"<color={color}>{description}</color>";
            textDescription.text = $"<color={skillDescriptionColor}>{description}</color>"; // 스킬 설명 글씨색 흰색으로 통일

            SetPassiveSkillLevelObjectAlpha(index, passiveSkillData.level[skillIndex - PASSIVESKILLNUM]);

            skillBoundaries[index].gameObject.SetActive(false);
        }

        string GetSelectedSkillDescription(int skillNum, float damage, int PASSIVESKILLNUM)
        {
            float incrementValue = 0;
            string attributeType = passiveSkillEffectString[skillNum - PASSIVESKILLNUM];

            switch (skillNum)
            {
                case 100:
                case 101:
                case 102:
                    incrementValue = ssm.masterySkillIncrementValue;
                    break;

                case 103:
                    incrementValue = ssm.damageReductionSkillIncrementValue;
                    break;

                case 104:
                    incrementValue = ssm.speedUpSkillIncrementValue;
                    break;

                case 105:
                    incrementValue = ssm.magnetSkillIncrementValue;
                    break;
            }

            string calculatedValue = "";
            switch (skillNum)
            {
                case 103:
                case 105:
                    calculatedValue = "" + Mathf.RoundToInt(damage * 100) + " + " + Mathf.RoundToInt(incrementValue * 100);
                    break;
                default:
                    calculatedValue = "" + (Mathf.RoundToInt(damage * 100) - 100) + " + " + Mathf.RoundToInt(incrementValue * 100);
                    break;
            }

            return $"{attributeType}: {calculatedValue}%";
        }

        string GetUnselectedSkillDescription(int skillNum, int PASSIVESKILLNUM)
        {
            float startValue = 0;
            string attributeType = passiveSkillEffectString[skillNum - PASSIVESKILLNUM];

            switch (skillNum)
            {
                case 100:
                case 101:
                case 102:
                    startValue = ssm.masterySkillStartValue;
                    break;

                case 103:
                    startValue = ssm.damageReductionSkillStartValue;
                    break;

                case 104:
                    startValue = ssm.speedUpSkillStartValue;
                    break;

                case 105:
                    startValue = ssm.magnetSkillStartValue;
                    break;
            }

            string calculatedValue = "";
            switch (skillNum)
            {
                case 103:
                    calculatedValue = "" + Mathf.RoundToInt(startValue * 100);
                    break;
                case 105:
                    calculatedValue = "" + Mathf.RoundToInt((startValue - 0.25f) * 100); // 플레이어 Asborber radius 기본 값 바뀌면 수정 해줘야 함
                    break;
                default:
                    calculatedValue = "" + (Mathf.RoundToInt(startValue * 100) - 100);
                    break;
            }

            return $"{attributeType}: +{calculatedValue}%";
        }

        public void OnMaxLevelIncrease()
        {
            hasMaxLevelIncreased = true;
        }

        //==================================================================
        // 도구
        private Color HexToColor(string hexCode)
        {
            Color color;
            if (UnityEngine.ColorUtility.TryParseHtmlString(hexCode, out color))
            {
                return color;
            }

            Debug.LogError("[UnityExtension::HexColor]invalid hex code - " + hexCode);
            return Color.white;
        }
        //==================================================================
    }
}

