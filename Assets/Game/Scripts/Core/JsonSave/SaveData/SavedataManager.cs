using Newtonsoft.Json;
using System.IO;
using UnityEngine;

namespace Eclipse
{
    public class SavedataManager : Eclipse.Manager
    {
        //==================================================================
        // Singleton part
        // 인스턴스에 접근하기 위한 프로퍼티
        public static SavedataManager instance
        {
            get
            {
                // 인스턴스가 없는 경우에 접근하려 하면 인스턴스를 할당해준다.
                if (!_instance)
                {
                    _instance = FindAnyObjectByType(typeof(SavedataManager)) as SavedataManager;

                    if (_instance == null)
                        Debug.Log("no Singleton obj");
                }
                return _instance;
            }
        }

        // 싱글톤 패턴을 사용하기 위한 인스턴스 변수
        private static SavedataManager _instance;

        //==================================================================
        public Server_PlayerData server_PlayerData;
        [SerializeField] Server_PlayerData load_PlayerData;

        [SerializeField] SettingData settingData;
        [SerializeField] SettingData load_SettingData;

        //==================================================================
        private string filePath_Server_PlayerData;
        private string filePath_SettingData;
        private const string encryptionKey = "0123456789abcdef0123456789abcdef"; // 32바이트 (AES-256)

        int basicSkillLevelLength = 8;
        int specialPassiveLevelLength = 7;

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
            DontDestroyOnLoad(_instance);

            //==================================================================
            SetForLoadServerData();
            SetForLoadSettingData();
        }

        public void SaveServerData()
        {
            //=================================================================================
            // 데이터 객체를 생성후 Json 형식으로 변환 후 저장

            string json = JsonConvert.SerializeObject(server_PlayerData, Formatting.Indented);

            // JSON 데이터 암호화
            string encryptedJson = EncryptionUtility.Encrypt(json, encryptionKey);

            Debug.Log(json);
            File.WriteAllText(filePath_Server_PlayerData, encryptedJson);
            Debug.Log("Json 파일로 저장됨: " + filePath_Server_PlayerData);
        }
        private void SaveSettingData()
        {
            //=================================================================================
            // 데이터 객체를 생성후 Json 형식으로 변환 후 저장

            string json = JsonConvert.SerializeObject(settingData, Formatting.Indented);

            // JSON 데이터 암호화
            string encryptedJson = EncryptionUtility.Encrypt(json, encryptionKey);

            Debug.Log(json);
            File.WriteAllText(filePath_SettingData, encryptedJson);
            Debug.Log("Json 파일로 저장됨: " + filePath_SettingData);
        }

        public void LoadServerData()
        {
            //=================================================================================
            SetForLoadServerData();

            //=================================================================================
            // 해당 경로에 파일이 저장되어 있을 경우에 파일 로드

            if (!File.Exists(filePath_Server_PlayerData))
            {
                Debug.Log($"파일이 존재하지 않습니다: {filePath_Server_PlayerData}");
                InitServer_PlayerDataInfo();
                SaveServerData();
                return;
            }
            // 암호화된 JSON 읽기
            string encryptedJson = File.ReadAllText(filePath_Server_PlayerData);

            // 복호화
            string decryptedJson = EncryptionUtility.Decrypt(encryptedJson, encryptionKey);

            JsonConvert.PopulateObject(decryptedJson, load_PlayerData);

            Debug.Log("서버 플레이어 데이터 로드!");

            UpdatePlayerInfo();
        }
        private void SetForLoadServerData()
        {
            //================================================================================
            // 데이터 저장하기 위한 경로 
            filePath_Server_PlayerData = Application.persistentDataPath + "/Server_PlayerData.json";
        }
        public void LoadSettingData()
        {
            //=================================================================================
            SetForLoadSettingData();

            //=================================================================================
            // 해당 경로에 파일이 저장되어 있을 경우에 파일 로드

            if (!File.Exists(filePath_SettingData))
            {
                Debug.Log($"파일이 존재하지 않습니다: {filePath_SettingData}");
                InitSettingDataInfo();
                SaveSettingData();
                return;
            }
            // 암호화된 JSON 읽기
            string encryptedJson = File.ReadAllText(filePath_SettingData);

            // 복호화
            string decryptedJson = EncryptionUtility.Decrypt(encryptedJson, encryptionKey);

            JsonConvert.PopulateObject(decryptedJson, load_SettingData);

            Debug.Log("세팅 데이터 로드!");

            UpdateSettingInfo();
        }
        private void SetForLoadSettingData()
        {
            //================================================================================
            // 데이터 저장하기 위한 경로 
            filePath_SettingData = Application.persistentDataPath + "/SettingData.json";
        }

        public void UpdatePlayerGold()
        {
            Debug.Log(server_PlayerData);
            server_PlayerData.playerGold += server_PlayerData.coin;
            server_PlayerData.coin = 0;
        }

        private void UpdatePlayerInfo()
        {
            //=================================================================================
            // 캐릭터 해금 정보
            server_PlayerData.isWarriorUnlocked = load_PlayerData.isWarriorUnlocked;  // 전사
            server_PlayerData.isAssassinUnlocked = load_PlayerData.isAssassinUnlocked; // 도적

            //=================================================================================
            // 플레이어 상점관련 정보
            server_PlayerData.playerGold = load_PlayerData.playerGold;
            server_PlayerData.increasedPrice = load_PlayerData.increasedPrice;
            server_PlayerData.startPrice = load_PlayerData.startPrice;
            server_PlayerData.priceIncreseNum = load_PlayerData.priceIncreseNum;
            server_PlayerData.coin = load_PlayerData.coin;

            //=================================================================================
            // 기초 패시브 효과 
            server_PlayerData.basicPassiveLevels = load_PlayerData.basicPassiveLevels;
            server_PlayerData.basicPassiveLevelsMaxNum = load_PlayerData.basicPassiveLevelsMaxNum;
            server_PlayerData.attackPower = load_PlayerData.attackPower;
            server_PlayerData.defense = load_PlayerData.defense;
            server_PlayerData.maxHealth = load_PlayerData.maxHealth;
            server_PlayerData.regen = load_PlayerData.regen;
            server_PlayerData.cooldown = load_PlayerData.cooldown;
            server_PlayerData.attackRange = load_PlayerData.attackRange;
            server_PlayerData.duration = load_PlayerData.duration;
            server_PlayerData.moveSpeed = load_PlayerData.moveSpeed;

            //=================================================================================
            // 특수 패시브 효과 
            server_PlayerData.specialPassiveLevels = load_PlayerData.specialPassiveLevels;
            server_PlayerData.specialPassiveLevelsMaxNum = load_PlayerData.specialPassiveLevelsMaxNum;
            server_PlayerData.magnetPower = load_PlayerData.magnetPower;
            server_PlayerData.luck = load_PlayerData.luck;
            server_PlayerData.expBoost = load_PlayerData.expBoost;
            server_PlayerData.goldBoost = load_PlayerData.goldBoost;
            server_PlayerData.nightmareMode = load_PlayerData.nightmareMode;
            server_PlayerData.canRevive = load_PlayerData.canRevive;
            server_PlayerData.rerollCount = load_PlayerData.rerollCount;
        }
        private void UpdateSettingInfo()
        {
            //=================================================================================
            // 플레이어 사운드 관련 정보
            settingData.masterSound = load_SettingData.masterSound;
            settingData.bgmSound = load_SettingData.bgmSound;
            settingData.sfxSound = load_SettingData.sfxSound;
            settingData.isMute = load_SettingData.isMute;

            //=================================================================================
            // 그래픽 관련 정보
            settingData.isFullScreen = load_SettingData.isFullScreen;
        }

        private void InitServer_PlayerDataInfo()
        {
            //=================================================================================
            // 캐릭터 해금 정보
            server_PlayerData.isWarriorUnlocked = false;  // 전사
            server_PlayerData.isAssassinUnlocked = false; // 도적

            //=================================================================================
            // 골드 및 상점 관련 정보
            server_PlayerData.playerGold = 0;
            server_PlayerData.increasedPrice = 0;
            server_PlayerData.startPrice = new int[]
            {
                200, 300, 200, 200, 900, 300, 300, 300,
                300, 600, 900, 200, 1500, 10000, 1000
            };
            server_PlayerData.priceIncreseNum = 100;
            server_PlayerData.coin = 0;

            //=================================================================================
            // 기초 패시브 효과 
            for (int i = 0; i < basicSkillLevelLength; i++)
                server_PlayerData.basicPassiveLevels[i] = 0;
            server_PlayerData.basicPassiveLevelsMaxNum = new int[]
            {
                10, 5, 5, 5, 2, 2, 2, 2
            };
            server_PlayerData.attackPower = 0.05f;
            server_PlayerData.defense = 0.05f;
            server_PlayerData.maxHealth = 10;
            server_PlayerData.regen = 0.025f;
            server_PlayerData.cooldown = 0.05f;
            server_PlayerData.attackRange = 0.05f;
            server_PlayerData.moveSpeed = 0.05f;
            server_PlayerData.duration = 0.1f;

            //=================================================================================
            // 특수 패시브 효과 
            for (int i = 0; i < specialPassiveLevelLength; i++)
                server_PlayerData.specialPassiveLevels[i] = 0;
            server_PlayerData.specialPassiveLevelsMaxNum = new int[]
            {
                2, 3, 5, 5, 5, 1, 5
            };
            server_PlayerData.magnetPower = 0.2f;
            server_PlayerData.luck = 0.1f;
            server_PlayerData.expBoost = 0.025f;
            server_PlayerData.goldBoost = 0.05f;
            server_PlayerData.nightmareMode = 0.15f;
            server_PlayerData.canRevive = 1;
            server_PlayerData.rerollCount = 1;
        }
        private void InitSettingDataInfo()
        {
            settingData.masterSound = 0.8f;
            settingData.bgmSound = 1f;
            settingData.sfxSound = 1f;
            settingData.isMute = false;

            settingData.isFullScreen = true;
        }

        private void OnApplicationQuit()
        {
            // 애플리케이션 종료 시 데이터 저장
            SaveServerData();
            SaveSettingData();
        }
    }
}
