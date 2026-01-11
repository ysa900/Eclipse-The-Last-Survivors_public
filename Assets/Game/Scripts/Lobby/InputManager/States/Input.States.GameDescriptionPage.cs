using UnityEngine;

namespace Eclipse.Lobby
{
    public partial class InputManager
    {
        private int currentPageNum = 0;

        private void BindGameDescriptionPageInputEvents()
        {
            var gui = client.GetManager<GUIManager>();

            //==================================================================

            void OnEnter()
            {
                Debug.Log("Enter GameDescription Page");

                var gameDescriptionViewer = gui.gameDescriptionViewer;

                var gameDescriptionPageBackButton = gui.gameDescriptionPageBackBotton;
                var gameDescriptionPageNextPageButton = gui.gameDescriptionPageNextPageButton;
                var gameDescriptionPagePreviousPageButton = gui.gameDescriptionPagePreviousPageButton;

                gui.gameDescriptionAppearPage.Show();

                // 어떤 Viewer가 켜져있던 GameDescription Page 나가기
                gameDescriptionPageBackButton.onClick = () =>
                {
                    stateMachine.Pop(); // 최상위 스택(Setting Page) Pop
                                        // stateMachine.Change(States.MainPage);

                };
                gameDescriptionPageBackButton.Show();


                gameDescriptionPageNextPageButton.onClick = () =>
                {
                    gui.gameDescriptionNextPage.Show();
                    gui.gameDescriptionNextPage.GetComponent<Animator>().SetTrigger("Left");

                    gui.gameDescriptionPages[currentPageNum++].Hide();
                    gui.gameDescriptionPages[currentPageNum].Show();

                    gameDescriptionPageNextPageButton.Hide();
                    gameDescriptionPagePreviousPageButton.Show();
                };

                if (gui.gameDescriptionPages[1].gameObject.activeSelf == false)
                   gameDescriptionPageNextPageButton.Show();

                gameDescriptionPagePreviousPageButton.onClick = () =>
                {
                    gui.gameDescriptionNextPage.Show();
                    gui.gameDescriptionNextPage.GetComponent<Animator>().SetTrigger("Right");

                    gui.gameDescriptionPages[currentPageNum--].Hide();
                    gui.gameDescriptionPages[currentPageNum].Show();

                    gameDescriptionPageNextPageButton.Show();
                    gameDescriptionPagePreviousPageButton.Hide();
                };

                gameDescriptionViewer.Show();
                gui.gameDescriptionPages[currentPageNum].Show();
            }

            //==================================================================

            void OnExit()
            {
                Debug.Log("Exit GameDescription Page");

                var gui = client.GetManager<GUIManager>();

                gui.gameDescriptionViewer.Hide();

                // 나머지 버튼들 정상화
                gui.exitButton.MakeInteractable();
                gui.characterButton.MakeInteractable();
                gui.gameDescriptionButton.MakeInteractable();
                gui.mainPageOptionButton.MakeInteractable();
            }

            //==================================================================

            var state = new State<States>(States.GameDescriptionPage);
            state.onEnter = OnEnter;
            state.onExit = OnExit;

            stateMachine.Add(state);
        }

        private void ActLikeGameDescriptionBackButton()
        {
            Eclipse.Lobby.AudioManager.instance.PlaySfx((int)Eclipse.Lobby.AudioManager.Sfx.Select);

            stateMachine.Pop();
        }
    }
}