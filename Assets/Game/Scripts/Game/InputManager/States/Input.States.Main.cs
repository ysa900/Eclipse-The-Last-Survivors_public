using Eclipse.Game.Panels;
using Eclipse.Lobby;
using UnityEngine;

namespace Eclipse.Game
{
    public partial class InputManager
    {
        private void BindStageMainEvents()
        {
            GUIManager gui = client.GetManager<GUIManager>();

            void OnEnter()
            {
                gui.pauseButton.onClick = () =>
                {
                    stateMachine.Push(States.Pause);
                };

                gui.settingButton.onClick = () =>
                {
                    stateMachine.Push(States.Setting);
                };

                gui.gameDescriptionButton.onClick = () =>
                {
                    stateMachine.Push(States.GameDescription);
                };

                gui.damageMetersButton.onClick = () =>
                {
                    stateMachine.Push(States.DamageMeters);
                };

                gui.settingButton.MakeInteractable();
                gui.pauseButton.MakeInteractable();
                gui.gameDescriptionButton.MakeInteractable();
                gui.damageMetersButton.MakeInteractable();

                gui.settingButton.Show();
                gui.pauseButton.Show();
                gui.gameDescriptionButton.Show();
                gui.damageMetersButton.Show();
                gui.goldPanel.Show();
                gui.playerHpStatus.gameObject.SetActive(true);
                gui.hpSlider.gameObject.SetActive(true);
            }

            void OnExit()
            {
                gui.settingButton.MakeUnInteractable();
                gui.pauseButton.MakeUnInteractable();
                gui.gameDescriptionButton.MakeUnInteractable();
                gui.damageMetersButton.MakeUnInteractable();
                gui.hpSlider.gameObject.SetActive(false);
            }

            var state = new State<States>(States.Main);
            state.onEnter = OnEnter;
            state.onExit = OnExit;

            stateMachine.Add(state);
        }

        private void ActLikeMainStatePauseButton()
        {
            Debug.Log(stateMachine);
            Eclipse.Game.AudioManager.instance.PlaySfx(Eclipse.Game.AudioManager.Sfx.Select);

            stateMachine.Push(States.Pause);
        }
    }
}