using UnityEngine;

namespace Eclipse.Game
{
    public partial class StageManager
    {
        private void BindStage2Events()
        {
            void OnEnter() // 스테이지 시작할 때
            {
                //==================================================================
                SpawnManager spawnManager = client.GetManager<SpawnManager>();

                //==================================================================
                sceneGameTime = gameTime;

                //==================================================================
                maxGameTime = sceneGameTime + 3 * 60f; // 이지 모드
                
                AudioManager.instance.PlayBgm(AudioManager.Bgm.Stage2);

                //==================================================================
                StartCoroutine(client.GetManager<PlayerManager>().SetPlayerStage2Info());

                //==================================================================
                spawnManager.SpawnStartEnemies = spawnManager.SpawnStage2StartEnemies;
                spawnManager.SpawnEnemiesByTime = spawnManager.Stage2Spawn;
                spawnManager.enemyCoefficient += 2;

                //==================================================================
            }

            void OnExit() // 스테이지가 끝날 때 (스테이지 나갈 때 X)
            {
                isStageRegularTimeOver = true;
                onStageOver.Invoke();

                Debug.Log("Stage2 Exit");
            }

            var state = new State<Stages>(Stages.Stage2);
            state.onEnter = OnEnter;
            state.onExit = OnExit;

            stateMachine.Add(state);

            //==================================================================
            // GUIManager Action 할당
            /*client.GetManager<GUIManager>().onSDVGameStartButtonClicked = () =>
            {
                Time.timeScale = 1;
            };*/
        }
    }
}