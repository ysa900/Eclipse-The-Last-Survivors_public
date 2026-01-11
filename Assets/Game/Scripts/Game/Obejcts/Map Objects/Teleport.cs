using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Eclipse.Game
{
    public class Teleport : MonoBehaviour
    {
        public Panels.Portal_Indicator indicator;
    
        public Panels.Boss_Portal_Indicator goTextObject;
        FollowCam followCam;
        Vector2 portalPos;
    
        public bool isIndicatorOn;
        bool isPlayerRange;
        bool isTelePortAlreadyStarted;

        // 사용할 Action, delegate들
        public Action<string, bool> onChangeScene;

        string sceneName;
    
        private void Awake()
        {
            isIndicatorOn = true;
        }

        private void OnEnable()
        {
            sceneName = SceneManager.GetActiveScene().name;
        }

        private void Start()
        {
            if (sceneName == "Stage1")
            {
                indicator.gameObject.SetActive(false);
                followCam = FollowCam.instance;
                portalPos = transform.position;
                indicator.portalPos = portalPos;
            }

            isTelePortAlreadyStarted = false;
        }
    
        private void Update()
        {
            TelePortCheck();

            if (sceneName == "Stage1")
            {
                if (portalPos.x >= (followCam.transform.position.x - FollowCam.instance.cameraHalfWidth) &&
                portalPos.x <= followCam.transform.position.x + FollowCam.instance.cameraHalfWidth &&
                portalPos.y >= (followCam.transform.position.y - FollowCam.instance.cameraHalfHeight) &&
                portalPos.y <= followCam.transform.position.y + FollowCam.instance.cameraHalfHeight) // 화면 안
                {
                    // 포털이 화면 안에 있을 때
                    indicator.gameObject.SetActive(false);
                }
                else
                {
                    // 화면 밖
                    if (isIndicatorOn)
                    {
                        indicator.gameObject.SetActive(true);
                    }
                    else
                    {
                        indicator.gameObject.SetActive(false);
                    }
                }
            }
        }

        private void TelePortCheck()
        {
            if (isPlayerRange && !isTelePortAlreadyStarted)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    isTelePortAlreadyStarted = true;
                    switch (SceneManager.GetActiveScene().name)
                    {
                        case "Stage1":
                            AudioManager.instance.bgmPlayer.Stop();
                            onChangeScene("Splash2", false);
                            break;

                        case "Stage2":
                            AudioManager.instance.bgmPlayer.Stop();
                            onChangeScene("Splash3", false);
                            break;
                    }
                }

            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Player")
            {
                isPlayerRange = true;
                transform.Find("Keyboard F").gameObject.SetActive(true);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.tag == "Player")
            {
                isPlayerRange = false;
                transform.Find("Keyboard F").gameObject.SetActive(false);
            }
        }
    }
}