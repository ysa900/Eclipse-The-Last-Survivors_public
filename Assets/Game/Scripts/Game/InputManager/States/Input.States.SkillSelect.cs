using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.Game
{
    public partial class InputManager
    {
        private void BindSkillSelctEvents()
        {
            GUIManager gui = client.GetManager<GUIManager>();
            SkillSelectManager skillSelectManager = client.GetManager<SkillSelectManager>();

            void OnEnter()
            {
                //==================================================================
                // 버튼 onclick 이벤트 할당

                var skillButtons = gui.skillButtons;
                for (int i = 0; i < skillButtons.Length; i++)
                {
                    skillButtons[i].GetComponent<Button>().onClick.RemoveAllListeners();
                }

                skillButtons[0].onClick = skillSelectManager.SkillSelectButton1Clicked;
                skillButtons[1].onClick = skillSelectManager.SkillSelectButton2Clicked;
                skillButtons[2].onClick = skillSelectManager.SkillSelectButton3Clicked;

                for (int i = 0; i < skillButtons.Length; i++)
                {
                    skillButtons[i].onClick += () =>
                    {
                        OnSkillSelectFinished();
                    };
                }

                var rerollButton = gui.rerollButton;
                rerollButton.GetComponent<Button>().onClick.RemoveAllListeners();
                rerollButton.onClick = () =>
                {
                    rerollButton.RerollNum -= 1; // 리롤 횟수 -1
                    skillSelectManager.SetLevelupPanel();
                };

                //==================================================================
                // 게임 설명 페이지 요소 초기화
                gui.skillSelectPageViewer.Show();

                if (PlayerManager.player.playerData.level != 1)
                {
                    if (rerollButton.RerollNum != 0)
                    {
                        // 버튼 활성화 기다렸다가 애니메이션 시작
                        StartCoroutine(WaitForRerollButtonAndStartAnimation(rerollButton.gameObject));
                    }
                    else
                    {
                        // 버튼 비활성화 및 애니메이션 정지
                        rerollButton.GetComponent<Button>().interactable = false;
                        rerollButton.GetComponentInChildren<UnscaledAnimationController>().StopAnimation();
                    }
                }
                Time.timeScale = 0f;

                //==================================================================
                // 모든 스킬 만렙 찍을 시 처리때문에 이게 가장 나중이 되어야 함
                // 선택할 스킬이 없으면 현재 State를 Pop함
                if (!skillSelectManager.isChoosingStartSkill) skillSelectManager.SetLevelupPanel();

                //==================================================================
            }

            void OnExit()
            {
                gui.skillSelectPageViewer.Hide();
                gui.rerollButton.GetComponentInChildren<UnscaledAnimationController>().StopAnimation(); // 애니메이션 끄기
                Time.timeScale = 1f;
            }

            var state = new State<States>(States.SkillSelect);
            state.onEnter = OnEnter;
            state.onExit = OnExit;

            stateMachine.Add(state);
        }

        private IEnumerator WaitForRerollButtonAndStartAnimation(GameObject rerollButton)
        {
            // 버튼 활성화를 기다림
            while (!rerollButton.activeSelf)
            {
                yield return null;
            }

            // 버튼이 활성화된 뒤 애니메이션 시작
            rerollButton.GetComponentInChildren<UnscaledAnimationController>().StartAnimation();
        }

        public void OnSkillSelectFinished()
        {
            stateMachine.Pop();
        }
    }
}