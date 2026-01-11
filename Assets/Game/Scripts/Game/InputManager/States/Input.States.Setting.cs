using UnityEngine;

namespace Eclipse.Game
{
    public partial class InputManager
    {
        private void BindSettingEvents()
        {
            GUIManager gui = client.GetManager<GUIManager>();

            void OnEnter()
            {
                gui.settingPage_backButton.onClick = () =>
                {
                    stateMachine.Pop();
                };

                gui.settingPageViewer.Show();

                Time.timeScale = 0f;
            }

            void OnExit()
            {
                gui.settingPageViewer.Hide();

                Time.timeScale = 1f;
            }

            var state = new State<States>(States.Setting);
            state.onEnter = OnEnter;
            state.onExit = OnExit;

            stateMachine.Add(state);
        }

        private void ActLikeSettingStateBackButton()
        {
            Eclipse.Game.AudioManager.instance.PlaySfx(Eclipse.Game.AudioManager.Sfx.Select);

            stateMachine.Pop();
        }
    }
}