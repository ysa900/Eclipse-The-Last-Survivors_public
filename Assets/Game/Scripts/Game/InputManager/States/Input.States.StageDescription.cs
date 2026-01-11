using UnityEngine;
using UnityEngine.SceneManagement;

namespace Eclipse.Game
{
    public partial class InputManager
    {
        private void BindStageDescriptionEvents()
        {
            GUIManager gui = client.GetManager<GUIManager>();
            SkillSelectManager skillSelectManager = client.GetManager<SkillSelectManager>();

            void OnEnter()
            {
                //==================================================================
                // ��ư onclick �̺�Ʈ �Ҵ�
                gui.stageDescription_gameStartButton.onClick = () =>
                {
                    stateMachine.Pop();

                    if (SceneManager.GetActiveScene().name == "Stage1")
                    {
                        skillSelectManager.ChooseStartSkill();

                        stateMachine.Push(States.SkillSelect);
                    }
                };

                //==================================================================
                // �������� ���� ������ ��� �ʱ�ȭ
                gui.stageDescriptionPageViewer.Show();

                //==================================================================
                // ���� ������ ��ư �ʱ�ȭ
                gui.settingButton.MakeUnInteractable();
                gui.pauseButton.MakeUnInteractable();
                gui.gameDescriptionButton.MakeUnInteractable();
                gui.damageMetersButton.MakeUnInteractable();

                //==================================================================
                Time.timeScale = 0f;

                //==================================================================
            }

            void OnExit()
            {
                gui.stageDescriptionPageViewer.Hide();

                gui.settingButton.MakeInteractable();
                gui.pauseButton.MakeInteractable();
                gui.gameDescriptionButton.MakeInteractable();
                gui.damageMetersButton.MakeInteractable();

                gui.activeSkillPanel.gameObject.SetActive(true);
                gui.passiveSkillPanel.gameObject.SetActive(true);
                shouldResonancePanelActivated?.Invoke();

                int playerClass = PlayerPrefs.GetInt("PlayerClass");
                if (playerClass == 2)
                {
                    gui.activeMijiSkillPanel.gameObject.SetActive(true);
                    gui.passiveMijiSkillPanel.gameObject.SetActive(true);
                }

                Time.timeScale = 1f;
            }

            var state = new State<States>(States.StageDescription);
            state.onEnter = OnEnter;
            state.onExit = OnExit;

            stateMachine.Add(state);
        }

        private void ActLikeStageDescriptionStartButton()
        {
            Eclipse.Game.AudioManager.instance.PlaySfx(Eclipse.Game.AudioManager.Sfx.Select);

            stateMachine.Pop();

            if (SceneManager.GetActiveScene().name == "Stage1")
            {
                client.GetManager<SkillSelectManager>().ChooseStartSkill();
                stateMachine.Push(States.SkillSelect);
            }
        }
    }
}