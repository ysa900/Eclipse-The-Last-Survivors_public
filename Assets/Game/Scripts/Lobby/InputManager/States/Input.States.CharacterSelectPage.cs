
using UnityEngine;

namespace Eclipse.Lobby
{
    public partial class InputManager
    {
        private void BindCharacterSelectPageInputEvents()
        {
            var gui = client.GetManager<GUIManager>();
            
            //==================================================================

            void OnEnter()
            {
                Debug.Log("Enter Character Select Page");

                if (!Eclipse.Lobby.AudioManager.instance.IsBgmPlaying((int)Eclipse.Lobby.AudioManager.Bgm.CharacterSelectPage))
                {
                    Eclipse.Lobby.AudioManager.instance.playBgm((int)Eclipse.Lobby.AudioManager.Bgm.CharacterSelectPage);
                }
                var characterSelectPageViewer = gui.characterSelectPageViewer;
                
                var characterImage = gui.characterImage;

                var characterSelectPageOptionButton = gui.characterSelectPageOptionButton;
                var characterSelectPageBackButton = gui.characterSelectPageBackButton;
                var mageSelectButton = gui.mageSelectButton;
                var warriorSelectButton = gui.warriorSelectButton;
                var assassinSelectButton = gui.assassinSelectButton;
                var gameStartButton = gui.gameStartButton;

                // 옵션 버튼
                characterSelectPageOptionButton.onClick = () =>
                {
                    stateMachine.Push(States.SettingPage);
                };
                characterSelectPageOptionButton.Show();

                // 뒤로가기 버튼
                characterSelectPageBackButton.onClick = () =>
                {
                    stateMachine.Pop();

                };
                characterSelectPageBackButton.Show();

                // 메이지 선택 버튼
                mageSelectButton.onClick = () =>
                {
                    PlayerPrefs.SetInt("PlayerClass", 0);

                    stateMachine.Push(States.CharacterDescriptionPage);

                };
                mageSelectButton.Show();

                // 워리어 선택 버튼
                warriorSelectButton.onClick = () =>
                {
                    PlayerPrefs.SetInt("PlayerClass", 1);

                    stateMachine.Push(States.CharacterDescriptionPage);

                };
                warriorSelectButton.Show();

                // 어쌔신 선택 버튼
                assassinSelectButton.onClick = () =>
                {
                    PlayerPrefs.SetInt("PlayerClass", 2);

                    stateMachine.Push(States.CharacterDescriptionPage);

                };
                assassinSelectButton.Show();

                // 버튼들 공통적으로 활성화
                characterSelectPageOptionButton.MakeInteractable();
                characterSelectPageBackButton.MakeInteractable();
                mageSelectButton.MakeInteractable();

                // 전사, 도적 우선 비활성화
                warriorSelectButton.MakeUnInteractable();
                assassinSelectButton.MakeUnInteractable();
                gui.lockImages[0].gameObject.SetActive(true);
                gui.lockImages[1].gameObject.SetActive(true);

                // 해금 됐다면 버튼 활성화
                if (server_PlayerData.isWarriorUnlocked)
                {
                    gui.lockImages[0].gameObject.SetActive(false);
                    gui.warriorSelectButton.MakeInteractable();
                }
                if (server_PlayerData.isAssassinUnlocked)
                {
                    gui.lockImages[1].gameObject.SetActive(false);
                    gui.assassinSelectButton.MakeInteractable();
                }

                characterSelectPageViewer.Show();
            }


            //==================================================================

            void OnExit()
            {
                Debug.Log("Exit Character Select Page");

                var gui = client.GetManager<GUIManager>();
                gui.characterSelectPageViewer.Hide();

                // 공통적으로 비활성화
                gui.characterSelectPageOptionButton.MakeUnInteractable();
                gui.characterSelectPageBackButton.MakeUnInteractable();
                gui.mageSelectButton.MakeUnInteractable();
                gui.warriorSelectButton.MakeUnInteractable();
                gui.assassinSelectButton.MakeUnInteractable();
            }

            //==================================================================

            var state = new State<States>(States.CharacterSelectPage);
            state.onEnter = OnEnter;
            state.onExit = OnExit;

            stateMachine.Add(state);
        }

        private void ActLikeCharacterPageStateBackButton()
        {
            Eclipse.Lobby.AudioManager.instance.PlaySfx((int)Eclipse.Lobby.AudioManager.Sfx.Select);
    
            stateMachine.Pop(); // 최상위 스택(CharacterSelect Page) Pop
            Eclipse.Lobby.AudioManager.instance.SwitchBGM((int)Eclipse.Lobby.AudioManager.Bgm.MainPage);
        }
    }
}