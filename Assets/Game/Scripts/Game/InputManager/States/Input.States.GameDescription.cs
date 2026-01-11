using UnityEngine;

namespace Eclipse.Game
{
    public partial class InputManager
    {
        private void BindGameDescriptionEvents()
        {
            int currentPageNum = 0;
            GUIManager gui = client.GetManager<GUIManager>();

            void OnEnter()
            {
                var gameDescriptionViewer = gui.gameDescriptionViewer;

                var gameDescriptionPageBackButton = gui.gameDescriptionPageBackBotton;
                var gameDescriptionPageNextPageButton = gui.gameDescriptionPageNextPageButton;
                var gameDescriptionPagePreviousPageButton = gui.gameDescriptionPagePreviousPageButton;

                // 어떤 Viewer가 켜져있던 GameDescription Page 나가기
                gameDescriptionPageBackButton.onClick = () =>
                {
                    stateMachine.Pop(); // 최상위 스택(Setting Page) Pop
                                        // stateMachine.Change(States.MainPage);

                };

                gameDescriptionPageNextPageButton.onClick = () =>
                {
                    gui.gameDescriptionNextPage.Show();
                    gui.gameDescriptionNextPage.GetComponent<Animator>().SetTrigger("Left");

                    gui.gameDescriptionPages[currentPageNum++].Hide();
                    gui.gameDescriptionPages[currentPageNum].Show();

                    gameDescriptionPageNextPageButton.Hide();
                    gameDescriptionPagePreviousPageButton.Show();
                };

                gameDescriptionPagePreviousPageButton.onClick = () =>
                {
                    gui.gameDescriptionNextPage.Show();
                    gui.gameDescriptionNextPage.GetComponent<Animator>().SetTrigger("Right");

                    gui.gameDescriptionPages[currentPageNum--].Hide();
                    gui.gameDescriptionPages[currentPageNum].Show();

                    gameDescriptionPageNextPageButton.Show();
                    gameDescriptionPagePreviousPageButton.Hide();
                };

                //==================================================================
                // ���� ���� ������ ��� �ʱ�ȭ
                currentPageNum = 0;
                gui.gameDescriptionViewer.Show();

                gui.gameDescriptionAppearPage.Show();
                gui.gameDescriptionNextPage.Hide();

                gui.gameDescriptionPages[currentPageNum].Show();
                for (int i = 1; i < gui.gameDescriptionPages.Length; i++) gui.gameDescriptionPages[i].Hide();

                gameDescriptionPageBackButton.Show();
                gameDescriptionPageNextPageButton.Show();
                gameDescriptionPagePreviousPageButton.Hide();

                //==================================================================
                // ���� ������ ��ư �ʱ�ȭ
                gui.settingButton.MakeUnInteractable();
                gui.pauseButton.MakeUnInteractable();
                gui.gameDescriptionButton.MakeUnInteractable();

                //==================================================================
                Time.timeScale = 0f;

                //==================================================================
            }

            void OnExit()
            {
                gui.gameDescriptionViewer.Hide();

                gui.settingButton.MakeInteractable();
                gui.pauseButton.MakeInteractable();
                gui.gameDescriptionButton.MakeInteractable();

                Time.timeScale = 1;
            }

            var state = new State<States>(States.GameDescription);
            state.onEnter = OnEnter;
            state.onExit = OnExit;

            stateMachine.Add(state);
        }

        private void ActLikeGameDescriptionStateBackButton()
        {
            Eclipse.Game.AudioManager.instance.PlaySfx(Eclipse.Game.AudioManager.Sfx.Select);
            
            stateMachine.Pop();
        }
    }
}