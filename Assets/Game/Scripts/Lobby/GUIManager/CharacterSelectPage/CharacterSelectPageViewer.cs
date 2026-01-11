using UnityEngine;
using UnityEngine.SceneManagement;

namespace Eclipse.Lobby
{
    public class CharacterSelectPageViewer : Eclipse.Viewer
    {

         //* CharacterSelectPage, Page Num: 1 *

        //// gameStart 버튼
        //[SerializeField] private UnityEngine.UI.Button gameStartButtonObject;
        //// Character Page Option 버튼
        //[SerializeField] private UnityEngine.UI.Button CharacterPageOptionButtonObject;
        //// Character Page Back 버튼
        //[SerializeField] private UnityEngine.UI.Button CharacterPageBackButtonObject;
        //// Characters Button Group
        //[SerializeField] private GameObject CharactersButtonGroup;

        //// 캐릭터 선택창 관련
        //// 캐릭터 선택 및 스토리 패널 ( 어쌔신, 마법사, 전사 )
        //[SerializeField] private GameObject AssassinPanel;
        //[SerializeField] private GameObject MagePanel;
        //[SerializeField] private GameObject WarriorPanel;

        //// 캐릭터(3) 선택
        //[SerializeField] private UnityEngine.UI.Button SelectAssassinButtonObject; // Assasin
        //[SerializeField] private UnityEngine.UI.Button SelectMageButtonObject; // Mage
        //[SerializeField] private UnityEngine.UI.Button SelectWarriorButtonObject; // Warrior

        //bool isCharacterSelect = false;

        //[SerializeField] PlayerData playerData;

        //void Start()
        //{
        //    Hide();

        //    // 클래스 스토리 비활성화
        //    AssassinPanel.SetActive(false);
        //    MagePanel.SetActive(false);
        //    WarriorPanel.SetActive(false);

        //    // Assassin 클래스 선택 시
        //    UnityEngine.UI.Button SelectAssassinButton = SelectAssassinButtonObject.GetComponent<UnityEngine.UI.Button>();
        //    SelectAssassinButton.onClick.AddListener(SelectAssassinButtonClicked);
        //    // Mage 클래스 선택 시
        //    UnityEngine.UI.Button SelectMageButton = SelectMageButtonObject.GetComponent<UnityEngine.UI.Button>();
        //    SelectMageButton.onClick.AddListener(SelectMageButtonClicked);
        //    // Warrior 클래스 선택 시
        //    UnityEngine.UI.Button SelectWarriorButton = SelectWarriorButtonObject.GetComponent<UnityEngine.UI.Button>();
        //    SelectWarriorButton.onClick.AddListener(SelectWarriorButtonClicked);

        //    // Character Page 뒤로가기 버튼 눌렀을 때
        //    UnityEngine.UI.Button CharacterPageBackButton = CharacterPageBackButtonObject.GetComponent<UnityEngine.UI.Button>();
        //    CharacterPageBackButton.onClick.AddListener(CharacterPage_BackButtonClicked);
        //    // Character Page의 Option 버튼 누를 때
        //    UnityEngine.UI.Button CharacterPage_OptionButton = CharacterPageOptionButtonObject.GetComponent<UnityEngine.UI.Button>();
        //    CharacterPage_OptionButton.onClick.AddListener(CharacterPage_OptionButtonClicked);
        //    // GameStart 버튼 눌렀을 때
        //    UnityEngine.UI.Button gameStartButton = gameStartButtonObject.GetComponent<UnityEngine.UI.Button>();
        //    gameStartButton.onClick.AddListener(GameStartButtonClicked);

        //    // 클래스 선택 버튼들 활성화
        //    SelectAssassinButtonObject.enabled = true;
        //    SelectMageButtonObject.enabled = true;
        //    SelectWarriorButtonObject.enabled = true;

        //    gameStartButtonObject.interactable = false;

        //}

        //// Mage 선택 시
        //private void SelectMageButtonClicked()
        //{
        //    AudioManager.instance.playSfx((int)AudioManager.Sfx.Select); // 선택 효과음

        //    playerData.playerClass = 0;

        //    MagePanel.SetActive(true);
        //    Debug.Log("마법사 선택");
        //    isCharacterSelect = true;
        //    SelectAssassinButtonObject.enabled = false;
        //    SelectWarriorButtonObject.enabled = false;

        //    gameStartButtonObject.interactable = true; // 게임 시작 활성화
        //    gameStartButtonObject.enabled = true;

        //    CharactersButtonGroup.SetActive(false);
        //}

        //// Warrior 선택 시
        //private void SelectWarriorButtonClicked()
        //{
        //    AudioManager.instance.playSfx((int)AudioManager.Sfx.Select); // 선택 효과음

        //    playerData.playerClass = 1;

        //    WarriorPanel.SetActive(true);
        //    Debug.Log("전사 선택");
        //    isCharacterSelect = true;
        //    SelectAssassinButtonObject.enabled = false;
        //    SelectWarriorButtonObject.enabled = false;

        //    gameStartButtonObject.interactable = true; // 게임 시작 활성화
        //    gameStartButtonObject.enabled = true;

        //    CharactersButtonGroup.SetActive(false);
        //}

        //// Assassin 선택 시
        //private void SelectAssassinButtonClicked()
        //{
        //    AudioManager.instance.playSfx((int)AudioManager.Sfx.Select); // 선택 효과음

        //    playerData.playerClass = 2;

        //    AssassinPanel.SetActive(true);
        //    Debug.Log("도적 선택");
        //    isCharacterSelect = true;
        //    SelectAssassinButtonObject.enabled = false;
        //    SelectWarriorButtonObject.enabled = false;

        //    gameStartButtonObject.interactable = true; // 게임 시작 활성화
        //    gameStartButtonObject.enabled = true;

        //    CharactersButtonGroup.SetActive(false);
        //}


        //// Character Page 뒤로가기 버튼 클릭 시
        //private void CharacterPage_BackButtonClicked()
        //{
        //    AudioManager.instance.playSfx((int)AudioManager.Sfx.Select); // 선택 효과음

        //    if (!isCharacterSelect) // 클래스 선택 안한 상태
        //    {
        //        Manager.instance.currentPageNum = 0; // 메인 로비 화면으로
        //        AudioManager.instance.playBgm((int)AudioManager.Bgm.MainPage);

        //        Manager.instance.CharacterSelectPage.SetActive(false);
        //        Manager.instance.MainPage.SetActive(true);
        //    }
        //    // 어떤 클래스를 선택한 상황
        //    else
        //    {
        //        isCharacterSelect = false;
        //        SelectAssassinButtonObject.enabled = true;
        //        SelectWarriorButtonObject.enabled = true;
        //        SelectMageButtonObject.enabled = true;

        //        gameStartButtonObject.interactable = false;

        //        if (AssassinPanel.activeSelf)
        //        {
        //            AssassinPanel.SetActive(false);
        //        }
        //        if (MagePanel.activeSelf)
        //        {
        //            MagePanel.SetActive(false);
        //        }
        //        if (WarriorPanel.activeSelf)
        //        {
        //            WarriorPanel.SetActive(false);
        //        }

        //        CharactersButtonGroup.SetActive(true);
        //    }
        //}

        //public void CharacterPage_OptionBackButtonClicked()
        //{
        //    AudioManager.instance.playSfx((int)AudioManager.Sfx.Select); // 선택 효과음

        //    //캐릭터 선택되지 않은 상황에서 옵션창 끄기
        //    if (!isCharacterSelect)
        //    {
        //        // 버튼 재활성화시키기
        //        SelectAssassinButtonObject.enabled = true;
        //        SelectWarriorButtonObject.enabled = true;
        //        SelectMageButtonObject.enabled = true;
        //        CharacterPageOptionButtonObject.enabled = true;
        //        CharacterPageBackButtonObject.enabled = true;
        //    }
        //    // 캐릭터 선택되어 있는 상황에서 옵션창 끄기
        //    else
        //    {
        //        // 캐릭선택 후 설명화면만 남기고 캐릭선택 버튼 비활성화하고 GameStart 버튼은 활성화
        //        SelectAssassinButtonObject.enabled = false;
        //        SelectWarriorButtonObject.enabled = false;
        //        SelectMageButtonObject.enabled = false;

        //        CharacterPageBackButtonObject.enabled = true;
        //        CharacterPageOptionButtonObject.enabled = true;

        //        gameStartButtonObject.interactable = true;
        //        gameStartButtonObject.enabled = true;
        //    }

        //    Manager.instance.SettingPage.SetActive(false);
        //}

        //public void CharacterPage_OptionButtonClicked()
        //{
        //    AudioManager.instance.playSfx((int)AudioManager.Sfx.Select); // 선택 효과음

        //    // 옵션창 안켜진 상황에서 누르기
        //    if (!Manager.instance.SettingPage.activeSelf)
        //    {
        //        Manager.instance.SettingPage.SetActive(true);
        //        CharacterPageBackButtonObject.enabled = false; // CharacterPage의 뒤로가기 버튼 안눌리게

        //        // 캐릭터 선택 버튼들 무효화
        //        SelectAssassinButtonObject.enabled = false;
        //        SelectWarriorButtonObject.enabled = false;
        //        SelectMageButtonObject.enabled = false;

        //        // GameStart 버튼 비활성화
        //        if (isCharacterSelect)
        //        {
        //            gameStartButtonObject.enabled = false;
        //        }
        //        //else
        //        //{
        //        //    gameStartButtonObject.interactable = false;
        //        //}
        //    }
        //    // 옵션창에서 켜진 상황에서 또 누르기
        //    else
        //    {
        //        Manager.instance.SettingPage.SetActive(false);
        //        CharacterPageBackButtonObject.enabled = true;

        //        // 캐릭터 선택하지 않은 상황
        //        if (!isCharacterSelect)
        //        {
        //            // 캐릭터 선택 버튼들 활성화
        //            SelectAssassinButtonObject.enabled = true;
        //            SelectWarriorButtonObject.enabled = true;
        //            SelectMageButtonObject.enabled = true;

        //            gameStartButtonObject.interactable = false;
        //        }
        //        else
        //        {
        //            // Assassin 클래스 선택한 상황
        //            if (AssassinPanel.activeSelf)
        //            {
        //                SelectAssassinButtonObject.enabled = true;
        //                SelectWarriorButtonObject.enabled = false;
        //                SelectMageButtonObject.enabled = false;

        //                gameStartButtonObject.enabled = true;
        //            }
        //            // Mage 클래스 선택한 상황
        //            else if (MagePanel.activeSelf)
        //            {
        //                SelectAssassinButtonObject.enabled = false;
        //                SelectWarriorButtonObject.enabled = true;
        //                SelectMageButtonObject.enabled = true;

        //                gameStartButtonObject.enabled = true;
        //            }
        //            // Warrior 클래스 선택한 상황
        //            else if (WarriorPanel.activeSelf)
        //            {
        //                SelectAssassinButtonObject.enabled = false;
        //                SelectWarriorButtonObject.enabled = true;
        //                SelectMageButtonObject.enabled = false;

        //                gameStartButtonObject.enabled = true;
        //            }
        //        }
        //    }
        //}

        //// GameStart 버튼 클릭 시
        //private void GameStartButtonClicked()
        //{
        //    AudioManager.instance.playSfx((int)AudioManager.Sfx.Select); // 선택 효과음
        //    AudioManager.instance.bgmPlayer.Stop(); // 현재 배경음 종료
        //    SceneManager.LoadScene("Splash1");
        //}
    }

}