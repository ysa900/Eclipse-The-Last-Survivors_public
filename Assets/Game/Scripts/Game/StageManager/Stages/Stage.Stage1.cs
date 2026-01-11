using UnityEngine;

namespace Eclipse.Game
{
    public partial class StageManager
    {
        private void BindStage1Events()
        {
            void OnEnter() // 스테이지 시작할 때
            {
                //==================================================================
                SpawnManager spawnManager = client.GetManager<SpawnManager>();
                PlayerManager playerManager = client.GetManager<PlayerManager>();
                SkillSelectManager skillSelectManager = client.GetManager<SkillSelectManager>();

                //==================================================================
                isGameOver = false;
                sceneGameTime = gameTime;

                //==================================================================
                AudioManager.instance.PlayBgm(AudioManager.Bgm.Stage1);

                //==================================================================
                playerManager.SetPlayerStage1Info();

                //==================================================================
                spawnManager.SpawnStartEnemies = spawnManager.SpawnStage1StartEnemies;
                spawnManager.SpawnEnemiesByTime = spawnManager.Stage1Spawn;

                //==================================================================
            }

            void OnExit() // 스테이지가 끝날 때 (스테이지 나갈 때 X)
            {
                //==================================================================
                isStageRegularTimeOver = true;
                onStageOver.Invoke();

                Debug.Log("Stage1 Exit");
                //==================================================================
            }

            var state = new State<Stages>(Stages.Stage1);
            state.onEnter = OnEnter;
            state.onExit = OnExit;

            stateMachine.Add(state);

            //==================================================================
            // GUIManager Action 할당
            /*client.GetManager<GUIManager>().onSDVGameStartButtonClicked = () =>
            {
                client.GetManager<SkillSelectManager>().ChooseStartSkill();
            };*/
        }
    }
}
