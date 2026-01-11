using TMPro;
using UnityEngine;

namespace Eclipse.Game
{
    public partial class InputManager
    {
        private void BindGameOverEvents()
        {
            GUIManager gui = client.GetManager<GUIManager>();
            StageManager stage = client.GetManager<StageManager>();
            SkillManager skill = client.GetManager<SkillManager>();
            PlayerManager playerManager = client.GetManager<PlayerManager>();
            SavedataManager saveDataManager = client.GetManager<SavedataManager>();

            void OnEnter()
            {
                gui.gameOver_goToLobbyButton.onClick = () =>
                {
                    //데이터 저장 필요
                    saveDataManager.UpdatePlayerGold();
                    saveDataManager.SaveServerData();
                    stage.GoToLobby();
                };

                gui.gameOver_reStartButton.onClick = () =>
                {
                    //데이터 저장 필요
                    saveDataManager.UpdatePlayerGold();
                    saveDataManager.SaveServerData();
                    stage.ResetStage();
                };

                gui.enemyKillText_GameOver.SetText(playerManager.playerData.kill.ToString());
                gui.goldCollectedText_GameOver.SetText(playerManager.server_PlayerData.coin.ToString());

                gui.gameOverPageViewer.Show();

                Time.timeScale = 0f;
            }

            void OnExit()
            {
                gui.gameOverPageViewer.Hide();

                Time.timeScale = 1f;
            }

            var state = new State<States>(States.GameOver);
            state.onEnter = OnEnter;
            state.onExit = OnExit;

            stateMachine.Add(state);
        }


    }
}

