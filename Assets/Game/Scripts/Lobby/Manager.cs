using System.Collections.Generic;
using UnityEngine;

namespace Eclipse.Lobby
{
    public class Manager : Eclipse.Manager
    {
        // * Lobby Main Page, Page Num: 0 *

        public static Manager instance; // 정적 메모리에 담기 위한 instance 변수 선언

        [SerializeField] private CharacterSelectPageViewer charcterPageViewer;

        // Character 버튼
        [SerializeField] private UnityEngine.UI.Button CharacterButtonObject;

        // Exit 버튼
        [SerializeField] private UnityEngine.UI.Button ExitButtonObject;

        // Main Page Option 버튼
        [SerializeField] private UnityEngine.UI.Button MainPageOptionButtonObject;

        // Game Description 버튼
        [SerializeField] private UnityEngine.UI.Button GameDescriptionButtonObject;

        // 페이지 모음
        // Main Page 오브젝트
        public GameObject MainPage;

        // Character Select Page 오브젝트
        public GameObject CharacterSelectPage;

        // Setting Page 오브젝트
        public GameObject SettingPage;

        // GameDescriptionPage 오브젝트
        public GameObject GameDescriptionPage;

        public int currentPageNum = 0;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            //AudioManager.instance.playBgm((int)(AudioManager.Bgm.MainPage)); // 메인로비 배경음
        }

        private void Start()
        {
            // 시작 시 비활성화
            CharacterSelectPage.SetActive(false);
            SettingPage.SetActive(false);
            GameDescriptionPage.SetActive(false);

            // Exit 버튼 눌렀을 때
            UnityEngine.UI.Button ExitButton = ExitButtonObject.GetComponent<UnityEngine.UI.Button>();
            ExitButton.onClick.AddListener(ExitButtonClicked);
            // Character(Page) 버튼 눌렀을 때
            UnityEngine.UI.Button CharacterButton = CharacterButtonObject.GetComponent<UnityEngine.UI.Button>();
            CharacterButton.onClick.AddListener(CharacterButtonClicked);
            // MainPage의 Option 버튼 눌렀을 때
            UnityEngine.UI.Button Main_OptionButton = MainPageOptionButtonObject.GetComponent<UnityEngine.UI.Button>();
            Main_OptionButton.onClick.AddListener(Main_OptionButtonClicked);
            // MainPage의 게임 설명 버튼 눌렀을 때
            UnityEngine.UI.Button GameDescriptionButton = GameDescriptionButtonObject.GetComponent<UnityEngine.UI.Button>();
            GameDescriptionButton.onClick.AddListener(GameDescriptionButtonClicked);
        }

        // Exit 버튼 눌렀을 때
        private void ExitButtonClicked()
        {
            //AudioManager.instance.playSfx((int)AudioManager.Sfx.Select);

            // 유니티 에디터에서 게임 플레이할 땐 종료 위한 #if 문 사용
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else // 실제 빌드 후 게임 플레이 후 종료 할 때 #else 문 사용
            Application.Quit(); // 게임 종료
#endif
        }

        // CharacterPage 버튼 클릭 시
        private void CharacterButtonClicked()
        {
            //AudioManager.instance.playSfx((int)AudioManager.Sfx.Select); // 선택 효과음

            currentPageNum = 1;

            MainPage.SetActive(false);
            CharacterSelectPage.SetActive(true);

            //AudioManager.instance.SwitchBGM((int)AudioManager.Bgm.CharacterSelectPage); // 배경음 변경 
        }

        // Main Page의 Option 버튼 클릭 시
        private void Main_OptionButtonClicked()
        {
            //AudioManager.instance.playSfx((int)AudioManager.Sfx.Select); // 선택 효과음

            // SettingPage 켤 때
            if (!SettingPage.activeSelf)
            {
                SettingPage.SetActive(true);

                // 다른 버튼들 비활성화
                CharacterButtonObject.enabled = false; // Character 버튼 클릭 안되게
                ExitButtonObject.enabled = false; // Exit 버튼 클릭 안되게
            }
            else // SettingPage 끌 때
            {
                SettingPage.SetActive(false);

                // 버튼 재활성화시키기
                CharacterButtonObject.enabled = true;
                ExitButtonObject.enabled = true;
            }
        }

        // Game Description 버튼 클릭 시
        private void GameDescriptionButtonClicked()
        {
            //AudioManager.instance.playSfx((int)AudioManager.Sfx.Select); // 선택 효과음

            // GameDescriptionPage 켤 때
            if (!GameDescriptionPage.activeSelf)
            {
                GameDescriptionPage.SetActive(true);

                // 다른 버튼들 비활성화
                CharacterButtonObject.enabled = false; // Character 버튼 클릭 안되게
                ExitButtonObject.enabled = false; // Exit 버튼 클릭 안되게
                MainPageOptionButtonObject.enabled = false;
                GameDescriptionButtonObject.enabled = false;
            }
        }

        public void GameDescriptionPageBackButtonClicked()
        {
            //AudioManager.instance.playSfx((int)AudioManager.Sfx.Select); // 선택 효과음

            GameDescriptionPage.SetActive(false);

            // 버튼 재활성화시키기
            CharacterButtonObject.enabled = true;
            ExitButtonObject.enabled = true;
            MainPageOptionButtonObject.enabled = true;
            GameDescriptionButtonObject.enabled = true;
        }

        public void SettingPageBackButtonClicked()
        {
            //AudioManager.instance.playSfx((int)AudioManager.Sfx.Select); // 선택 효과음

            switch (currentPageNum)
            {
                case 0: // 메인 로비화면
                    SettingPage.SetActive(false);
                    CharacterButtonObject.enabled = true;
                    ExitButtonObject.enabled = true;
                    break;
                case 1:
                    //charcterPageViewer.CharacterPage_OptionBackButtonClicked();
                    break;
            }
        }
    }
}