using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Eclipse.Game
{
    public class PlayerManager : Eclipse.Manager
    {
        //==================================================================
        // Singleton part
        // 인스턴스에 접근하기 위한 프로퍼티
        public static PlayerManager instance
        {
            get
            {
                // 인스턴스가 없는 경우에 접근하려 하면 인스턴스를 할당해준다.
                if (!_instance)
                {
                    _instance = FindAnyObjectByType(typeof(PlayerManager)) as PlayerManager;

                    if (_instance == null)
                        Debug.Log("no Singleton obj");
                }
                return _instance;
            }
        }

        // 싱글톤 패턴을 사용하기 위한 인스턴스 변수
        private static PlayerManager _instance;

        //==================================================================
        // 플레이어 프리팹들
        Player magePrefab;
        Player warriorPrefab;
        Player assassinPrefab;

        //==================================================================
        // 플레이어 데이터들
        // player Area Size
        public float playerAreaSize = 0;

        private float playerSpeedSave;

        [ReadOnly] public PlayerData playerData; // 플레이어 데이터 객체
        [ReadOnly] public SkillData2 skillData; // 플레이어 데이터 객체
        [ReadOnly] public Server_PlayerData server_PlayerData; // 서버 플레이어 데이터 객체
        [ReadOnly] public DefaultPlayerData defaultPlayerData; // 플레이어 디폴트 데이터 객체

        //==================================================================
        // 플레이어 객체
        public static Player player;

        //==================================================================
        // 사용할 클래스 객체
        StageManager stageManager;
        SkillSelectManager skillSelectManager;
        GUIManager guiManager;

        //==================================================================
        private const int LEVEL_FOR_REROLL_INCREMENT = 5;

        //==================================================================
        // Action 객체들
        public Action onGameOver;
        public Action onLevelUp;

        public Action onMijiSkillMustSelect;

        //==================================================================
        // 비급 이펙트(어쌔신 전용)
        private GameObject mijiEffect;

        private void Awake()
        {
            // Singleton part
            //==================================================================
            if (_instance == null)
            {
                _instance = this;
            }
            // 인스턴스가 존재하는 경우 새로생기는 인스턴스를 삭제한다.
            else if (_instance != this)
            {
                Destroy(gameObject);
                return;
            }

            // 씬이 전환되더라도 선언되었던 인스턴스가 파괴되지 않게 함
            DontDestroyOnLoad(gameObject);

            //==================================================================
        }

        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            string sceneName = scene.name;
            if (sceneName == "Stage1" || sceneName == "Stage2" || sceneName == "Stage3")
            {
                Init();
            }
        }

        void Init()
        {
            stageManager = client.GetManager<StageManager>();
            skillSelectManager = client.GetManager<SkillSelectManager>();
            guiManager = client.GetManager<GUIManager>();
        }

        // 플레이어 스폰 함수
        public void SpawnPlayer()
        {
            int playerClass = PlayerPrefs.GetInt("PlayerClass");

            if (SceneManager.GetActiveScene().name == "Stage1")
                PlayerDataInit();

            InstantiatePlayer(playerClass);
            player.Init();
        }

        // 플레이어 스테이지 설정 관련 함수들
        //==================================================================
        public void SetPlayerStage1Info()
        {
            playerData.speed_Additional = 0; // 플레이어 속도 원래대로
            playerAreaSize = 120 / 5;

            SetPlayerAreaSize(playerAreaSize);
            SetPlayerToDefalutPosition();
        }

        public IEnumerator SetPlayerStage2Info()
        {
            yield return new WaitUntil(() => FollowCam.instance.cameraHalfWidth != 5); // PixelPerfectCamera는 Start 이후 프레임에 적용되기 때문에 한 프레임 대기

            playerData.speed_Additional = 0; // 플레이어 속도 원래대로
            playerAreaSize = FollowCam.instance.cameraHalfWidth * 2;

            SetPlayerAreaSize(playerAreaSize);
            SetPlayerToDefalutPosition();

            // 도적 커서 표시
            int playerClass = PlayerPrefs.GetInt("PlayerClass");
            if (playerClass == 2) MakeCursorVisible();
        }

        public void SetPlayerStage3Info()
        {
            playerData.speed_Additional = 0; // 플레이어 속도 원래대로

            SetPlayerToDefalutPosition();

            // 도적 커서 표시
            int playerClass = PlayerPrefs.GetInt("PlayerClass");
            if (playerClass == 2) MakeCursorVisible();
        }

        // Player AreaSize 설정 함수
        private void SetPlayerAreaSize(float areaSize)
        {
            Vector2 AreaSize = new Vector2(areaSize, areaSize);
            player.gameObject.GetComponentInChildren<BoxCollider2D>().size = AreaSize;
        }

        private void SetPlayerToDefalutPosition()
        {
            Vector2 PlayerPos = new Vector2(0, 0);
            player.transform.position = PlayerPos;
        }

        //==================================================================

        void InstantiatePlayer(int playerclass)
        {
            string playerPrefabPath = "Prefabs/Char_Eclipse/Player/";

            // 플레이어 생성
            switch (playerclass)
            {
                case 0:
                    magePrefab = Resources.Load<Player>(playerPrefabPath + "Mage/Mage");
                    player = Instantiate(magePrefab);
                    break;

                case 1:
                    warriorPrefab = Resources.Load<Player>(playerPrefabPath + "Warrior/Warrior");
                    player = Instantiate(warriorPrefab);
                    break;

                case 2:
                    assassinPrefab = Resources.Load<Assassin>(playerPrefabPath + "Assassin/Assassin");
                    player = Instantiate(assassinPrefab);

                    // 어쌔신인 경우에만 델리게이트 초기화
                    ((Assassin)player).onMijiSkillMustSelect = HandleMijiSkillSelect; // 비급 선택 창 나오게 연결
                    // 비급 선택 시 획득 이펙트 관련..
                    mijiEffect = player.transform.GetChild(6).gameObject;

                    break;
            }

            player.playerData = playerData; // player에 playerData 할당
            playerData.additional_Power = 0f; // 추가 공격력 초기화
            player.server_PlayerData = server_PlayerData;
            playerData.magnetRange_Additional = 0; // 플레이어 자석 범위 정상화

            player.onPlayerHasKilled = OnPlayerHasKilled;
            player.onPlayerLevelUP = OnPlayerLevelUP;
            player.getClinetIsChangingScene = () => { return client.isChangingScene; };
        }

        void PlayerDataInit()
        {
            playerData.hp = playerData.maxHp;
            playerData.Exp = 0;
            playerData.level = 1;
            playerData.kill = 0;
            playerData.holyReductionValue = 0.3f;

            playerData.nextExp = new int[999];
            int expNum = 5;
            int expWeight = 5;
            int expWeightCount = 1;

            for (int i = 1; i < playerData.nextExp.Length; i++)
            {
                if (i % 10 == 0) expWeightCount++;

                expNum += expWeight * expWeightCount;
                playerData.nextExp[i] = expNum;
            }

            PlayerPrefs.SetInt("Revive", server_PlayerData.specialPassiveLevels[5]);
        }

        public void PlayerSpeedUp()
        {
            playerData.speed_Additional = 3f;
        }

        // 도적 커서 활성화 함수
        public void MakeCursorVisible()
        {
            bool isCursorSkillSelected = skillData.skillSelected[0] || skillData.skillSelected[1] || skillData.skillSelected[2];
            player.cursorIndicator.gameObject.SetActive(isCursorSkillSelected);
            player.cursorIndicator.GetComponent<CursorIndicator>().CursorInit();
        }

        // 플레이어가 죽었을 시 실행됨
        private void OnPlayerHasKilled()
        {
            stageManager.isGameOver = true;

            onGameOver();

            AudioManager.instance.bgmPlayer.Stop(); // 배경음 멈추기
            Time.timeScale = 0; // 화면 멈추기
        }

        // 플레이어가 레벨 업 했을 시 실행
        private void OnPlayerLevelUP()
        {
            int playerClass = PlayerPrefs.GetInt("PlayerClass");

            // 5레벨 단위로 리롤 횟수 1회 추가
            if (skillSelectManager.playerData.level % LEVEL_FOR_REROLL_INCREMENT == 0)
            {
                // 기본적으로 리롤 횟수 증가
                guiManager.rerollButton.RerollNum += 1; // 리롤 횟수 +1
            }

            onLevelUp();

            AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp); // 레벨업 시 효과음
        }

        // [어쌔신]비급 선택해야할 레벨에 도달했을 때 실행(10레벨 당)
        private void HandleMijiSkillSelect()
        {
            if (onMijiSkillMustSelect != null)
            {
                guiManager.rerollButton.RerollNum += 1; // 비급 선택 전에 리롤 횟수 +1
                // 비급 선택창 리롤 횟수 할당
                guiManager.mijiRerollButton.MijiRerollNum = guiManager.rerollButton.RerollNum;

                onMijiSkillMustSelect?.Invoke(); // MijiSkillSelect 상태 전환
            }
        }

        // dodgeRate를 증가시키는 함수
        public void ApplyDodgeRate(float rate)
        {
            ((AssassinData)playerData).dodgeRate += rate; // AssassinData에 반영
        }
    
        // dodgeRate를 감소시키는 함수
        public void RemoveDodgeRate(float rate)
        {
            ((AssassinData)playerData).dodgeRate -= rate;
        }
    }
}
