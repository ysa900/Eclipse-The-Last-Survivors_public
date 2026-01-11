using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.Game
{
    public partial class InputManager
    {
        private void BindMijiSkillSelctEvents()
        {
            GUIManager gui = client.GetManager<GUIManager>();
            Miji_SSM miji_SSM = client.GetManager<Miji_SSM>();

            void OnEnter()
            {
                //==================================================================
                // ��ư onclick �̺�Ʈ �Ҵ�

                var skillButtons = gui.mijiSkillButtons;
                for (int i = 0; i < skillButtons.Length; i++)
                {
                    skillButtons[i].GetComponent<Button>().onClick.RemoveAllListeners();
                }

                skillButtons[0].onClick = miji_SSM.SkillSelectButton1Clicked;
                skillButtons[1].onClick = miji_SSM.SkillSelectButton2Clicked;
                skillButtons[2].onClick = miji_SSM.SkillSelectButton3Clicked;

                for (int i = 0; i < skillButtons.Length; i++)
                {
                    skillButtons[i].onClick += () =>
                    {
                        OnMijiSkillSelectFinished();
                    };
                }

                // ��޵� ���� ������..
                var mijiRerollButton = gui.mijiRerollButton;

                mijiRerollButton.GetComponent<Button>().onClick.RemoveAllListeners();
                mijiRerollButton.onClick = () =>
                {
                    mijiRerollButton.MijiRerollNum--;
                    gui.rerollButton.RerollNum = mijiRerollButton.MijiRerollNum; // ������Ʈ
                    miji_SSM.SetMijiPanel();
                };

                //==================================================================
                // ���� ���� ������ ��� �ʱ�ȭ
                gui.mijiSkillSelectPageViewer.Show();

                // ��ư Ȱ��ȭ ��ٷȴٰ� �ִϸ��̼� ����
                if (mijiRerollButton.MijiRerollNum != 0)
                {
                    // 버튼 활성화 기다렸다가 애니메이션 시작
                    StartCoroutine(WaitForRerollButtonAndStartAnimation(mijiRerollButton.gameObject));
                }
                else
                {
                    mijiRerollButton.GetComponent<Button>().interactable = false;
                    mijiRerollButton.GetComponentInChildren<UnscaledAnimationController>().StopAnimation();
                }
                StartCoroutine(WaitForMijiRerollButtonAndStartAnimation(mijiRerollButton.gameObject));

                Time.timeScale = 0f;


                //==================================================================

                miji_SSM.SetMijiPanel();
            }

            void OnExit()
            {
                gui.rerollButton.RerollNum = gui.mijiRerollButton.MijiRerollNum; // 리롤 횟수(일반 스킬 패널의 리롤버튼 횟수) 업데이트
                gui.mijiRerollButton.GetComponentInChildren<UnscaledAnimationController>().StopAnimation(); // �ִϸ��̼� ����
                gui.mijiSkillSelectPageViewer.Hide();
            }

            var state = new State<States>(States.MijiSkillSelect);
            state.onEnter = OnEnter;
            state.onExit = OnExit;

            stateMachine.Add(state);
        }
        private IEnumerator WaitForMijiRerollButtonAndStartAnimation(GameObject rerollButton)
        {
            // ��ư Ȱ��ȭ�� ��ٸ�
            while (!rerollButton.activeSelf)
            {
                yield return null;
            }

            // ��ư�� Ȱ��ȭ�� �� �ִϸ��̼� ����
            rerollButton.GetComponentInChildren<UnscaledAnimationController>().StartAnimation();
        }

        public void OnMijiSkillSelectFinished()
        {
            // Miji ��ų ������ ������ SkillSelect ���·� ��ȯ
            stateMachine.Pop(); // MijiSkillSelect ���� Pop
            stateMachine.Push(States.SkillSelect); // SkillSelect ���� Push
        }        
    }
}