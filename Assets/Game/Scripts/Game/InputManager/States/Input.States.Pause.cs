using Eclipse.Lobby;
using UnityEngine;

namespace Eclipse.Game
{
    public partial class InputManager
    {
        private void BindPauseEvents()
        {
            GUIManager gui = client.GetManager<GUIManager>();
            StageManager stageManager = client.GetManager<StageManager>();
            SavedataManager saveDataManager = client.GetManager<SavedataManager>();

            void OnEnter()
            {
                gui.pause_goToLobbyButton.onClick = () =>
                {
                    //데이터 저장 필요
                    saveDataManager.UpdatePlayerGold();
                    saveDataManager.SaveServerData();
                    Debug.Log("게임 중간에 멈추고 로비로 갈 경우 데이터 저장");
                    stageManager.GoToLobby();

                };

                gui.pause_reStartButton.onClick = () =>
                {
                    //데이터 저장 필요
                    saveDataManager.UpdatePlayerGold();
                    saveDataManager.SaveServerData();
                    Debug.Log("게임 중간에 멈추고 재시작할 경우 데이터 저장");
                    stageManager.ResetStage();

                };

                gui.pause_playButton.onClick = () =>
                {
                    stateMachine.Pop();
                };

                gui.pausePageViewer.Show();


                if (gui.specialSkillPanel.gameObject.activeSelf)
                {
                    //gui.specialSkillPanel.gameObject.SetActive(false);
                }

                Time.timeScale = 0f;
            }

            void OnExit()
            {
                gui.pausePageViewer.Hide();

                Time.timeScale = 1f;
            }


            var state = new State<States>(States.Pause);
            state.onEnter = OnEnter;
            state.onExit = OnExit;

            stateMachine.Add(state);
        }

        private void ActLikePauseStateStartButton()
        {
            Eclipse.Game.AudioManager.instance.PlaySfx(Eclipse.Game.AudioManager.Sfx.Select);

            stateMachine.Pop();
        }


        private void ActLikePauseStateReStartButton()
        {
            Eclipse.Game.AudioManager.instance.PlaySfx(Eclipse.Game.AudioManager.Sfx.Select);

            client.GetManager<StageManager>().ResetStage();
        }
    }
}