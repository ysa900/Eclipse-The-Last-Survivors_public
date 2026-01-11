using UnityEngine;

namespace Eclipse.Lobby
{
    public partial class InputManager
    {
        private void BindLobbyInputEvents()
        {
            var gui = client.GetManager<GUIManager>();

            //==================================================================

            void OnEnter()
            {
                Debug.Log("Enter Lobby Main Page");

                if (!Eclipse.Lobby.AudioManager.instance.IsBgmPlaying((int)Eclipse.Lobby.AudioManager.Bgm.MainPage))
                {
                    Eclipse.Lobby.AudioManager.instance.playBgm((int)Eclipse.Lobby.AudioManager.Bgm.MainPage);
                }

                var mainPageOptionButton = gui.mainPageOptionButton;
                mainPageOptionButton.onClick = () =>
                {
                    stateMachine.Push(States.SettingPage);
                    
                };
                mainPageOptionButton.Show();

                // CharacterSelectPage로 ..
                var characterButton = gui.characterButton;
                characterButton.onClick = () =>
                {
                    stateMachine.Push(States.CharacterSelectPage);
                };
                characterButton.Show();

                var shopButton = gui.shopButton;
                shopButton.onClick = () =>
                {
                    stateMachine.Push(States.ShopPage);
                };
                shopButton.Show();
                //==================================================================

                // Game 나가기 ..
                var exitButton = gui.exitButton;
                exitButton.onClick = () =>
                {
                    // 유니티 에디터에서 게임 플레이할 땐 종료 위한 #if 문 사용
                    #if UNITY_EDITOR
                        UnityEditor.EditorApplication.isPlaying = false;
                    #else // 실제 빌드 후 게임 플레이 후 종료 할 때 #else 문 사용
                    Application.Quit(); // 게임 종료
                    #endif
                };
                exitButton.Show();

                // GameDescriptionPage로 ..
                var gameDescriptionButton = gui.gameDescriptionButton;
                gameDescriptionButton.onClick = () =>
                {
                    stateMachine.Push(States.GameDescriptionPage);
                };
                gameDescriptionButton.Show();

                // 다른 버튼들 활성화
                gui.exitButton.MakeInteractable();
                gui.characterButton.MakeInteractable();
                gui.gameDescriptionButton.MakeInteractable();
                gui.mainPageOptionButton.MakeInteractable();
            }

            void OnExit()
            {
                // 다른 버튼들 비활성화
                gui.exitButton.MakeUnInteractable();
                gui.characterButton.MakeUnInteractable();
                gui.gameDescriptionButton.MakeUnInteractable();
                gui.mainPageOptionButton.MakeUnInteractable();
            }


            //==================================================================

            var state = new State<States>(States.MainPage);
            state.onEnter = OnEnter;
            state.onExit = OnExit;

            stateMachine.Add(state);
        }
    }
}