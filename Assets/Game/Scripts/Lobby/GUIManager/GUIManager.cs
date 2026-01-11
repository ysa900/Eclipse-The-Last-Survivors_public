using UnityEngine;

namespace Eclipse.Lobby
{
    public class GUIManager : Eclipse.Manager
    {
        // ===============================================================================
        /* Main Page */
        [Header("Main Page")]
        [ReadOnly] public MainPageViewer mainPageViewer;

        [ReadOnly] public CharacterButton characterButton;
        [ReadOnly] public ExitButton exitButton;
        [ReadOnly] public GameDescriptionButton gameDescriptionButton;

        [ReadOnly] public MainPageOptionButton mainPageOptionButton;
        [ReadOnly] public ShopButton shopButton;

        // ===============================================================================
        /* Character Select Page */
        [Header("Character Select Page")]
        [ReadOnly] public CharacterSelectPageViewer characterSelectPageViewer;

        [ReadOnly] public GameStartButton gameStartButton;
        [ReadOnly] public AssassinSelectButton assassinSelectButton;
        [ReadOnly] public WarriorSelectButton warriorSelectButton;
        [ReadOnly] public MageSelectButton mageSelectButton;

        [ReadOnly] public LockImage[] lockImages;

        [ReadOnly] public CharacterSelectPageBackButton characterSelectPageBackButton;
        [ReadOnly] public CharacterSelectPageOptionButton characterSelectPageOptionButton;

        // ===============================================================================
        /* Shop Page */
        [Header("Shop Page")]
        [ReadOnly] public Shop.ShopPageViewer shopPageViewer;

        [ReadOnly] public Shop.PassiveUpgradePanel[] passiveUpgradePanels;

        [ReadOnly] public Shop.PassiveUpgradePanelTapImage[] passiveUpgradePanelTapButtons;
        [ReadOnly] public Shop.PassiveUpgradePanelTapClosedButton[] passiveUpgradePanelTapClosedButtons;

        [ReadOnly] public Shop.PassiveUpgradeButton[] passiveUpgradeButtons;
        [ReadOnly] public TooltipTrigger[] tooltipTriggers;
        [ReadOnly] public TooltipPanel tooltipPanel;

        [ReadOnly] public Shop.RefundButton refundButton;
        [ReadOnly] public Shop.SelectButton selectButton;
        [ReadOnly] public Shop.BackButton backButton;

        [ReadOnly] public Shop.LevelText[] levelTexts;
        [ReadOnly] public Shop.PriceText[] priceTexts;
        [ReadOnly] public Shop.GoldText goldText;

        // ===============================================================================
        /* Character Description Page */
        [Header("Character Description Page")]
        // [ReadOnly] public AssassinDescrptionViewer assassinDescriptionViewer;
        // [ReadOnly] public MageDescriptionViewer mageDescriptionViewer;
        // [ReadOnly] public WarriorDescriptionViewer warriorDescriptionViewer;

        [ReadOnly] public CharacterDescrptionViewer characterDescrptionViewer;

        [ReadOnly] public AbilityDescriptionText abilityDescriptionText;
        [ReadOnly] public AbilityNameText abilityNameText;
        [ReadOnly] public CharacterNameText characterNameText;
        [ReadOnly] public CharacterStoryText characterStoryText;

        [ReadOnly] public AbilityImage abilityImage;
        [ReadOnly] public CharacterPanelImage characterPanelImage;
     


        // ===============================================================================
        /* Setting Page */
        [Header("Setting Page")]
        [ReadOnly] public SettingPageViewer settingPageViewer;

        [ReadOnly] public SettingPageBackButton settingPageBackButton;

        [ReadOnly] public MasterSlider masterSlider;
        [ReadOnly] public BGMSlider bgmSlider;
        [ReadOnly] public SFXSlider sfxSlider;

        [ReadOnly] public MuteToggle muteToggle;
        [ReadOnly] public FullScreenToggle fullScreenToggle;

        [ReadOnly] public MasterSoundLabel masterSoundLabel;
        [ReadOnly] public BGMSoundLabel bgmSoundLabel;
        [ReadOnly] public SFXSoundLabel sfxSoundLabel;

        [ReadOnly] public MasterFillImage masterFillImage;
        [ReadOnly] public BGMFillImage bgmFillImage;
        [ReadOnly] public SFXFillImage sfxFillImage;

        // ===============================================================================
        /* Game Discription Page */
        [Header("Game Discription Page")]
        [ReadOnly] public GameDescriptionViewer gameDescriptionViewer;

        [ReadOnly] public GameDescriptionPage[] gameDescriptionPages;

        [ReadOnly] public GameDescriptionPageBackBotton gameDescriptionPageBackBotton;
        [ReadOnly] public GameDescriptionPageNextPageButton gameDescriptionPageNextPageButton;
        [ReadOnly] public GameDescriptionPagePreviousPageButton gameDescriptionPagePreviousPageButton;
        [ReadOnly] public GameDescriptionNextPage gameDescriptionNextPage;
        [ReadOnly] public GameDescriptionAppearPage gameDescriptionAppearPage;

        // ===============================================================================
        /* Loading Page */
        [Header("Loading Page")]
        [ReadOnly] public CharacterImage characterImage;

        private void Awake()
        {
            // ===============================================================================
            /* Main Page */

            mainPageViewer = GetWidget<MainPageViewer>();

            characterButton = GetWidget<CharacterButton>();
            exitButton = GetWidget<ExitButton>();
            gameDescriptionButton = GetWidget<GameDescriptionButton>();

            mainPageOptionButton = GetWidget<MainPageOptionButton>();
            shopButton = GetWidget<ShopButton>();

            // ===============================================================================
            /* Character Select Page */

            characterSelectPageViewer = GetWidget<CharacterSelectPageViewer>();

            gameStartButton = GetWidget<GameStartButton>();
            mageSelectButton = GetWidget<MageSelectButton>();
            warriorSelectButton = GetWidget<WarriorSelectButton>();
            assassinSelectButton = GetWidget<AssassinSelectButton>();

            lockImages = GetComponentsInChildren<LockImage>();

            characterSelectPageBackButton = GetWidget<CharacterSelectPageBackButton>();
            characterSelectPageOptionButton = GetWidget<CharacterSelectPageOptionButton>();

            // ===============================================================================
            /* Shop Page */
            shopPageViewer = GetWidget<Shop.ShopPageViewer>();

            passiveUpgradePanels = GetComponentsInChildren<Shop.PassiveUpgradePanel>();

            passiveUpgradePanelTapButtons = GetComponentsInChildren<Shop.PassiveUpgradePanelTapImage>();
            passiveUpgradePanelTapClosedButtons = GetComponentsInChildren<Shop.PassiveUpgradePanelTapClosedButton>();

            passiveUpgradeButtons = GetComponentsInChildren<Shop.PassiveUpgradeButton>();
            tooltipTriggers = GetComponentsInChildren<TooltipTrigger>();
            tooltipPanel = GetWidget<TooltipPanel>();

            refundButton = GetWidget<Shop.RefundButton>();
            selectButton = GetWidget<Shop.SelectButton>();
            backButton = GetWidget<Shop.BackButton>();
            
            levelTexts = GetComponentsInChildren<Shop.LevelText>();
            priceTexts = GetComponentsInChildren<Shop.PriceText>();
            goldText = GetWidget<Shop.GoldText>();

            // ============================================================
            /* Character Description Page */

            characterDescrptionViewer = GetWidget<CharacterDescrptionViewer>();

            abilityDescriptionText = GetWidget<AbilityDescriptionText>();
            abilityNameText = GetWidget<AbilityNameText>();
            characterNameText = GetWidget<CharacterNameText>();
            characterStoryText = GetWidget<CharacterStoryText>();

            abilityImage = GetWidget<AbilityImage>();
            characterPanelImage = GetWidget<CharacterPanelImage>();
         

            // ===============================================================================
            /* Setting Page */

            settingPageViewer = GetWidget<SettingPageViewer>();

            settingPageBackButton = GetWidget<SettingPageBackButton>();

            masterSlider = GetWidget<MasterSlider>();
            bgmSlider = GetWidget<BGMSlider>();
            sfxSlider = GetWidget<SFXSlider>();

            muteToggle = GetWidget<MuteToggle>();
            fullScreenToggle = GetWidget<FullScreenToggle>();

            masterSoundLabel = GetWidget<MasterSoundLabel>();
            bgmSoundLabel = GetWidget<BGMSoundLabel>();
            sfxSoundLabel = GetWidget<SFXSoundLabel>();

            masterFillImage = GetWidget<MasterFillImage>();
            bgmFillImage = GetWidget<BGMFillImage>();
            sfxFillImage = GetWidget<SFXFillImage>();


            // ===============================================================================
            /* Game Discription Page */

            gameDescriptionViewer = GetWidget<GameDescriptionViewer>();
            gameDescriptionPages = GetComponentsInChildren<GameDescriptionPage>();

            gameDescriptionPageBackBotton = GetWidget<GameDescriptionPageBackBotton>();
            gameDescriptionPageNextPageButton = GetWidget<GameDescriptionPageNextPageButton>();
            gameDescriptionPagePreviousPageButton = GetWidget<GameDescriptionPagePreviousPageButton>();
            gameDescriptionNextPage = GetWidget<GameDescriptionNextPage>();
            gameDescriptionAppearPage = GetWidget<GameDescriptionAppearPage>();

            // ===============================================================================
            /* Loading Page */

            characterImage = GetWidget<CharacterImage>();


            // ===============================================================================
        }

        private void Start() {
            // ===============================================================================
            /* Character Select Page */

            characterDescrptionViewer.Hide();

            // ===============================================================================
            /* Character Select Page */

            characterSelectPageViewer.Hide();


            // ===============================================================================
            /* Shop Page */

            shopPageViewer.Hide();


            // ===============================================================================
            /* Game Discription Page */

            gameDescriptionViewer.Hide();

            foreach(var gameDescriptionPage in gameDescriptionPages)
            {
                gameDescriptionPage.Hide();
            }
            gameDescriptionPagePreviousPageButton.Hide();
            gameDescriptionNextPage.Hide();

            // ===============================================================================
            /* Setting Page */

            settingPageViewer.Hide();


            // ===============================================================================
        }

        // GetWidget<T>() = gameObject.GetComponentInChildren<T>() 같은 의미, 편리하게 ..
        public T GetWidget<T>() where T : UnityEngine.Component
        {
            return gameObject.GetComponentInChildren<T>();
        }
    }

}