using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Eclipse.Game
{
    public partial class InputManager : Eclipse.Manager
    {
        //==================================================================
        public enum States
        {
            Main,
            GameDescription,
            StageDescription,
            Pause,
            Setting,
            DamageMeters,
            GameOver,
            GameClear,
            SkillSelect,
            MijiSkillSelect,
        }

        [ReadOnly][SerializeField] StateMachine<States> stateMachine;


        SkillSelectManager skillSelectManager;
        Miji_SSM mijiSkillSelectManager;
        GUIManager guiManager;

        //==================================================================
        // 사용할 Action들
        public Action<string, bool> onChangeScene;
        public Action shouldResonancePanelActivated;

        //==================================================================

        private void Awake()
        {
            stateMachine = new StateMachine<States>();
        }

        private void Start()
        {
            BindStageMainEvents();
            BindGameClearEvents();
            BindGameDescriptionEvents();
            BindGameOverEvents();
            BindPauseEvents();
            BindDamageMetersEvent();
            BindSettingEvents();
            BindSkillSelctEvents();
            BindStageDescriptionEvents();

            int playerClass = PlayerPrefs.GetInt("PlayerClass");
            if (playerClass == 2)
            {
                BindMijiSkillSelctEvents();
            }

            StartCoroutine(ExecuteAfterAllStarts());
        }

        private IEnumerator ExecuteAfterAllStarts()
        {
            // 모든 Start()가 끝나고 마지막에 실행
            yield return new WaitForEndOfFrame();

            skillSelectManager = client.GetManager<SkillSelectManager>();
            if (PlayerPrefs.GetInt("PlayerClass") == 2)
            {
                yield return null;
                mijiSkillSelectManager = client.GetManager<Miji_SSM>();
            }
          
            guiManager = client.GetManager<GUIManager>();

        }

        private void Update()
        {
            if (Screen.IsTransitioning) // 읽기 전용 프로퍼티로 상태를 확인
            {
                Time.timeScale = 1f; // 화면 전환 중에는 게임 시간을 멈춤
                return;  // 화면 전환 중이므로 입력 처리 차단
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                HandleBackNavigation();
            }
            else if (Input.GetKeyDown(KeyCode.R) && stateMachine.CurrentState.ID == States.Pause)
            {
                ActLikePauseStateReStartButton();
            }
            else if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && stateMachine.CurrentState.ID == States.StageDescription)
            {
                ActLikeStageDescriptionStartButton();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
            {
                ActLikeSelect1Button();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
            {
                ActLikeSelect2Button();
            }
            else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
            {
                ActLikeSelect3Button();
            }
            else if (Input.GetKeyDown(KeyCode.R) && !client.GetManager<SkillSelectManager>().isChoosingSpecialSkill)
            {
                ActLikeRerollButton();
            }
#if UNITY_EDITOR
            else if (Input.GetKeyDown(KeyCode.Tab))
            {
                StageManager stage = client.GetManager<StageManager>();
                if (SceneManager.GetActiveScene().name == "Stage2")
                {
                    stage.gameTime = stage.maxGameTime - 1f; // 개발 편의를 위해 만들어놓음
                }
                else
                {
                    stage.GoToStage2(); // 개발 편의를 위해 만들어놓음
                }
            }
            else if (Input.GetKeyDown(KeyCode.CapsLock))
            {
                UnityEditor.EditorApplication.isPlaying = false; // 개발 편의를 위해 만들어놓음
            }
            else if (Input.GetKeyDown(KeyCode.F1))
            {
                client.GetManager<BossManager>().boss.hp *= 0.7f; // 개발 편의를 위해 만들어놓음
            }
            else if (Input.GetKeyDown(KeyCode.F2))
            {
                client.GetManager<BossManager>().boss.hp *= 0.3f; // 개발 편의를 위해 만들어놓음
            }
            else if (Input.GetKeyDown(KeyCode.F3))
            {
                client.GetManager<BossManager>().boss.hp *= 0f; // 개발 편의를 위해 만들어놓음

            }
            else if (Input.GetKeyDown(KeyCode.Delete))
            {
                client.GetManager<StageManager>().MakeRegularStageTimeOver(); // 개발 편의를 위해 만들어놓음
            }
#endif
        }

        public override void Startup()
        {
            stateMachine.Push(States.Main);
            stateMachine.Push(States.StageDescription);
        }

        public void OnGameClear()
        {
            stateMachine.Push(States.GameClear);
        }

        public void OnGameOver()
        {
            stateMachine.Push(States.GameOver);
        }

        public void OnLevelUp()
        {
            stateMachine.Push(States.SkillSelect);
        }

        public void OnMijiSkillMustSelect()
        {
            stateMachine.Push(States.MijiSkillSelect);
        }

        private void HandleBackNavigation()
        {
            if (stateMachine.CurrentState.ID == States.Main)
            {
                ActLikeMainStatePauseButton();
            }
            else if (stateMachine.CurrentState.ID == States.Pause)
            {
                ActLikePauseStateStartButton();
            }
            else if (stateMachine.CurrentState.ID == States.GameDescription)
            {
                ActLikeGameDescriptionStateBackButton();
            }
            else if (stateMachine.CurrentState.ID == States.Setting)
            {
                ActLikeSettingStateBackButton();
            }
            else if (stateMachine.CurrentState.ID == States.DamageMeters)
            {
                ActLikeDamageMetersStateStartButton();
            }
        }

        private void ActLikeSelect1Button()
        {
            if (stateMachine.CurrentState.ID == States.SkillSelect)
            {
                bool hasPanelActivated = guiManager.skillPanels[0].gameObject.activeSelf;
                if (!hasPanelActivated) return;

                skillSelectManager.SkillSelectButton1Clicked();
                OnSkillSelectFinished();
            }
            else if (stateMachine.CurrentState.ID == States.MijiSkillSelect)
            {
                bool hasPanelActivated = guiManager.mijiSkillPanels[0].gameObject.activeSelf;
                if (!hasPanelActivated) return;

                mijiSkillSelectManager.SkillSelectButton1Clicked();
                OnMijiSkillSelectFinished();
            }
            else
            {
                return;
            }
        }

        private void ActLikeSelect2Button()
        {
            if (stateMachine.CurrentState.ID == States.SkillSelect)
            {
                bool hasPanelActivated = guiManager.skillPanels[1].gameObject.activeSelf;
                if (!hasPanelActivated) return;

                skillSelectManager.SkillSelectButton2Clicked();
                OnSkillSelectFinished();
            }
            else if (stateMachine.CurrentState.ID == States.MijiSkillSelect)
            {
                bool hasPanelActivated = guiManager.mijiSkillPanels[1].gameObject.activeSelf;
                if (!hasPanelActivated) return;

                mijiSkillSelectManager.SkillSelectButton2Clicked();
                OnMijiSkillSelectFinished();
            }
            else
            {
                return;
            }
        }

        private void ActLikeSelect3Button()
        {
            if (stateMachine.CurrentState.ID == States.SkillSelect)
            {
                bool hasPanelActivated = guiManager.skillPanels[2].gameObject.activeSelf;
                if (!hasPanelActivated) return;

                skillSelectManager.SkillSelectButton3Clicked();
                OnSkillSelectFinished();
            }
            else if (stateMachine.CurrentState.ID == States.MijiSkillSelect)
            {
                bool hasPanelActivated = guiManager.mijiSkillPanels[2].gameObject.activeSelf;
                if (!hasPanelActivated) return;

                mijiSkillSelectManager.SkillSelectButton3Clicked();
                OnMijiSkillSelectFinished();
            }
            else
            {
                return;
            }
        }

        private void ActLikeRerollButton()
        {
            if (stateMachine.CurrentState.ID == States.SkillSelect)
            {
                if (guiManager.rerollButton.RerollNum != 0)
                {
                    guiManager.rerollButton.RerollNum -= 1; // 리롤 횟수 -1
                    skillSelectManager.SetLevelupPanel();
                }
                else
                {
                    return;
                }

            }
            else if (stateMachine.CurrentState.ID == States.MijiSkillSelect)
            {
                if (guiManager.mijiRerollButton.MijiRerollNum != 0)
                {
                    guiManager.mijiRerollButton.MijiRerollNum--;
                    guiManager.rerollButton.RerollNum = guiManager.mijiRerollButton.MijiRerollNum; // 업데이트
                    mijiSkillSelectManager.SetMijiPanel();
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
            }
        }
    }
}