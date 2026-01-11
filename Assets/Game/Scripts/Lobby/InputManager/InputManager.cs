using UnityEngine;

namespace Eclipse.Lobby
{
    public partial class InputManager : Eclipse.Manager
    {
        public enum States
        {
            // (메인) 로비 화면
            MainPage,
            // 옵션 설정 화면
            SettingPage,
            // 게임 설명 화면
            GameDescriptionPage,
            // 상점 화면
            ShopPage,
            // 캐릭터 선택 화면
            CharacterSelectPage,
            // 캐릭터 설명 화면
            CharacterDescriptionPage,
        };

        [ReadOnly][SerializeField] public StateMachine<States> stateMachine;

        public System.Action<string> onChangeScene;

        Server_PlayerData server_PlayerData;

        private void Awake()
        {
            stateMachine = new StateMachine<States>();
        }

        void Start()
        {
            server_PlayerData = Resources.Load<Server_PlayerData>("Datas/Server_PlayerData");

            BindLobbyInputEvents();
            BindSettingPageInputEvents();
            BindGameDescriptionPageInputEvents();
            BindShopPageInputEvents();
            BindCharacterSelectPageInputEvents();
            BindCharacterDescriptionPageInputEvents();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                HandleBackNavigation();
            }
        }
        public override void Startup()
        {
            stateMachine.Push(States.MainPage);
        }

        private void HandleBackNavigation()
        {
            if (stateMachine.CurrentState.ID == States.SettingPage)
            {
                ActLikeSettingPageBackButton();
            }
            else if (stateMachine.CurrentState.ID == States.CharacterDescriptionPage)
            {
                ActLikeCharacterDescriptionStateBackButton();
            }
            else if (stateMachine.CurrentState.ID == States.ShopPage)
            {
                ActLikeShopPageBackButton();
            }
            else if (stateMachine.CurrentState.ID == States.CharacterSelectPage)
            {
                ActLikeCharacterPageStateBackButton();
            }
            else if (stateMachine.CurrentState.ID == States.GameDescriptionPage)
            {
                ActLikeGameDescriptionBackButton();
            }
        }
    }
}