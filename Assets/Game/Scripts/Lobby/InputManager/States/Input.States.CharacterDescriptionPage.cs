
using UnityEngine;
namespace Eclipse.Lobby
{
    public partial class InputManager
    {
        private void BindCharacterDescriptionPageInputEvents()
        {
            var gui = client.GetManager<GUIManager>();

            //==================================================================

            void OnEnter()
            {
                Debug.Log("Enter Character Description Page");

                int playerClass = PlayerPrefs.GetInt("PlayerClass");

                SetCharacterDescriptionPage(gui, playerClass);


                var characterSelectPageBackButton = gui.characterSelectPageBackButton;
                var mageSelectButton = gui.mageSelectButton;
                var warriorSelectButton = gui.warriorSelectButton;
                var assassinSelectButton = gui.assassinSelectButton;
                var gameStartButton = gui.gameStartButton;
                var characterSelectPageOptionButton = gui.characterSelectPageOptionButton;

                characterSelectPageBackButton.onClick = () =>
                {
                    stateMachine.Pop();
                };
                characterSelectPageBackButton.Show();

                // 옵션 버튼
                characterSelectPageOptionButton.onClick = () =>
                {
                    stateMachine.Push(States.SettingPage);
                };
                characterSelectPageOptionButton.Show();

                // GameStart 버튼
                gameStartButton.onClick = () =>
                {
                    onChangeScene("Splash1"); // Splash1 씬 전환
                };
                gameStartButton.Show();

                // 옵션 버튼
                characterSelectPageOptionButton.onClick = () =>
                {
                    stateMachine.Push(States.SettingPage);

                };
                characterSelectPageOptionButton.Show();

                gui.characterSelectPageViewer.Show();
                gameStartButton.MakeInteractable();
                characterSelectPageOptionButton.MakeInteractable();
                characterSelectPageBackButton.MakeInteractable();

                gui.characterDescrptionViewer.Show();
            }


            //==================================================================

            void OnExit()
            {
                Debug.Log("Exit Character Description Page");

                var gui = client.GetManager<GUIManager>();

                gui.characterDescrptionViewer.Hide();
                gui.gameStartButton.MakeUnInteractable();
                gui.characterSelectPageOptionButton.MakeUnInteractable();
            }


            //==================================================================

            var state = new State<States>(States.CharacterDescriptionPage);
            state.onEnter = OnEnter;
            state.onExit = OnExit;

            stateMachine.Add(state);
        }

        void SetCharacterDescriptionPage(GUIManager gui, int playerClass)
        {
            gui.abilityDescriptionText.ChangeAbilityDescription(playerClass);
            gui.abilityNameText.ChangeAbilityName(playerClass);
            gui.characterNameText.ChangeCharacterName(playerClass);
            gui.characterStoryText.ChangeCharacterStory(playerClass);

            gui.abilityImage.ChangeAbilityImage(playerClass);
            gui.characterPanelImage.ChangeCharacterPanelImage(playerClass);
        }

        private void ActLikeCharacterDescriptionStateBackButton()
        {
            Eclipse.Lobby.AudioManager.instance.PlaySfx((int)Eclipse.Lobby.AudioManager.Sfx.Select);

            stateMachine.Pop();
        }
    }
}