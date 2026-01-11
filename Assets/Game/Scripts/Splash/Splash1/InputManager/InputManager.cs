using System.Collections;
using UnityEngine;

namespace Eclipse.Splash.Splash1
{
    public class InputManager : Eclipse.Manager
    {
        public System.Action<string> onChangeScene;
        GUIManager gui;
        bool isSetChangeScene;

        void Start()
        {
            // AudioManager가 완전히 초기화될 때까지 기다리기 위한 안전장치
            gui = client.GetManager<GUIManager>();
            
            Eclipse.Splash.AudioManager.instance.playBgm((int)Eclipse.Splash.AudioManager.Bgm.Splash1);

            GotoStage1();

            isSetChangeScene = false;
        }

        private void Update()
        {
            if (Screen.IsTransitioning)  // 읽기 전용 프로퍼티로 상태를 확인
                return;  // 화면 전환 중이므로 입력 처리 차단

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                onChangeScene("Stage1");
            }

            if (gui.guideText.gameObject.activeSelf && !isSetChangeScene)
            {
                StartCoroutine(WaitforAnyKeyDown());
                isSetChangeScene = true;
            }
        }


        private void GotoStage1()
        {
            gui.skipButton.onClick = () =>
            {
                Eclipse.Splash.AudioManager.instance.playSfx((int)Eclipse.Splash.AudioManager.Sfx.Select);

                onChangeScene("Stage1");
            };
        }

        IEnumerator WaitforAnyKeyDown()
        {
            yield return new WaitUntil(() => Input.anyKeyDown);  // 아무 키나 입력 받을 때까지 대기
            onChangeScene("Stage1");
        }
    }
}
