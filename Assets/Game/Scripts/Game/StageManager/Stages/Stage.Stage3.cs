using UnityEngine;

namespace Eclipse.Game
{
    public partial class StageManager
    {
        private void BindStage3Events()
        {
            void OnEnter() // 스테이지 시작할 때
            {
                //==================================================================
                PlayerManager playerManager = client.GetManager<PlayerManager>();
                SpawnManager spawnManager = client.GetManager<SpawnManager>();
                BossManager bossManager = client.GetManager<BossManager>();
                SkillManager skillManager = client.GetManager<SkillManager>();

                //==================================================================
                sceneGameTime = gameTime;

                //==================================================================
                maxGameTime = sceneGameTime + 3 * 60f; // 이지 모드

                //==================================================================
                client.GetManager<PlayerManager>().SetPlayerStage3Info();

                //==================================================================
                spawnManager.SpawnStartEnemies = spawnManager.SpawnStage3StartEnemies;
                spawnManager.SpawnEnemiesByTime = spawnManager.Stage3Spawn;
                spawnManager.enemyCoefficient += 2;

                //==================================================================
                // 오디오 클립을 미리 로드해서 렉 방지
                AudioManager.instance.LoadBgm(AudioManager.Bgm.Boss_1Phase);
                AudioManager.instance.LoadBgm(AudioManager.Bgm.Boss_2Phase);
                AudioManager.instance.LoadBgm(AudioManager.Bgm.Boss_3Phase);

                AudioManager.instance.PlayBgm(AudioManager.Bgm.Stage3);

                //==================================================================
            }

            void OnExit() // 스테이지가 끝날 때 (스테이지 나갈 때 X)
            {
                //==================================================================
                BossManager bossManager = client.GetManager<BossManager>();
                SkillManager skillManager = client.GetManager<SkillManager>();

                //==================================================================
                isStageRegularTimeOver = true;
                onStageOver.Invoke();

                Debug.Log("Stage3 Exit");
                //==================================================================
            }

            var state = new State<Stages>(Stages.Stage3);
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