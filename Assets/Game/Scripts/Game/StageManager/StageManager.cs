using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Eclipse.Game
{
    public partial class StageManager : Eclipse.Manager
    {
        //==================================================================
        // Singleton part
        // 인스턴스에 접근하기 위한 프로퍼티
        public static StageManager instance
        {
            get
            {
                // 인스턴스가 없는 경우에 접근하려 하면 인스턴스를 할당해준다.
                if (!_instance)
                {
                    _instance = FindAnyObjectByType(typeof(StageManager)) as StageManager;
                    
                    if (_instance == null)
                        Debug.Log("no Singleton obj");
                }
                return _instance;
            }
        }
        
        // 싱글톤 패턴을 사용하기 위한 인스턴스 변수
        private static StageManager _instance;
        
        //==================================================================
        // State 관리 part
        public enum Stages
        {
            Stage1,
            Stage2,
            Stage3,
        }

        [SerializeField] [ReadOnly] StateMachine<Stages> stateMachine;

        //==================================================================
        // 데이터들
        [ReadOnly] public PlayerData playerData;
        Server_PlayerData server_PlayerData;

        //==================================================================
        // 중간보스전 벽 생성하기 위한 변수들
        [SerializeField] GameObject wallPrefab;
        List<GameObject> spawnedWalls = new List<GameObject>();
        Camera mainCamera;
        

        //==================================================================
        // 게임 시간
        public float gameTime;
        public float sceneGameTime;
        public float maxGameTime = 3 * 60f; // 초기(Stage1) maxGameTime

        //==================================================================
        // bool 변수들
        public bool isGameOver = false;
        public bool isStageRegularTimeOver = false; // 스테이지 정규 시간 끝났는지 판단
        public bool isStageClear = true; // Stage들 클리어
        public bool shouldStopGameTime = false; // 스테이지 2에서 패턴 시작할 때 전체 stageManager.gameTime 멈추기 위해 필요한 변수
        bool isStatePoped = false;

        //==================================================================
        // 사용할 Action, delegate들
        public Action<string, bool> onChangeScene;
        public Action onStageOver;
        public Action<Vector2, Vector2> onSpawnWalls; // 벽 생성 시 호출되는 액션 (Stage1, Stage2에서 사용)

        //==================================================================
        // 씬 이름 캐싱
        string sceneName;

        //==================================================================

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
            // State 관리 part
            stateMachine = new StateMachine<Stages>();

            //==================================================================
            // 변수 초기화 part
            gameTime = 0;
            maxGameTime = 3 * 60; // 이지 모드

            //==================================================================
            server_PlayerData = Resources.Load<Server_PlayerData>("Datas/Server_PlayerData");

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
            sceneName = scene.name;
            if (sceneName == "Stage1" || sceneName == "Stage2" || sceneName == "Stage3")
            {
                Init();
            }
            if (sceneName == "Stage1" || sceneName == "Stage2")
            {
                onStageOver += () => SpawnWalls(); // 이건 Client에서 onStageOver 할당 이후임
            }
        }

        void Init()
        {
            //==================================================================
            switch (sceneName)
            {
                case "Stage1":
                    server_PlayerData.coin = 0;
                    break;
                case "Stage2":
                    stateMachine.Push(Stages.Stage2);
                    break;

                case "Stage3":
                    stateMachine.Push(Stages.Stage3);
                    break;
            }
            mainCamera = Camera.main;

            //==================================================================
            isGameOver = false;
            isStageRegularTimeOver = false;
            isStageClear = false;
            isStatePoped = false;

            //==================================================================
        }

        void SpawnWalls()
        {
            GameObject levelObject = GameObject.Find("Level");
            float camHeight = 2f * mainCamera.orthographicSize;
            float camWidth = camHeight * mainCamera.aspect;

            int cols = Mathf.CeilToInt(camWidth) + 1;
            int rows = Mathf.CeilToInt(camHeight) + 1;

            Vector2 center = mainCamera.transform.position;
            float left = center.x - cols / 2f;
            float right = center.x + cols / 2f;
            float bottom = center.y - rows / 2f;
            float top = center.y + rows / 2f;

            bool isStage2 = sceneName == "Stage2";

            float topY = isStage2 ? 2f : top - 1f;
            float bottomY = isStage2 ? -5f : bottom;

            // 위/아래 벽
            for (int x = 0; x < cols; x++)
            {
                float posX = left + x;

                spawnedWalls.Add(Instantiate(wallPrefab, new Vector3(posX, bottomY, 0f), Quaternion.identity));
                spawnedWalls.Add(Instantiate(wallPrefab, new Vector3(posX, topY, 0f), Quaternion.identity));
            }

            // 좌/우 벽
            int yCount = isStage2 ? 7 - 1 : rows - 1; // 8칸 (2.0 ~ -5.0), 위/아래 제외
            float yStart = isStage2 ? -4f : bottom + 1f;

            for (int i = 0; i < yCount; i++)
            {
                float posY = yStart + i;
                float leftX = left;
                float rightX = right - 1f;

                spawnedWalls.Add(Instantiate(wallPrefab, new Vector3(leftX, posY, 0f), Quaternion.identity));
                spawnedWalls.Add(Instantiate(wallPrefab, new Vector3(rightX, posY, 0f), Quaternion.identity));
            }

            // 생성된 벽들을 levelObject의 자식으로 설정
            foreach (var wall in spawnedWalls)
            {
                if (wall != null)
                {
                    wall.transform.SetParent(levelObject.transform);
                }
            }

            // 안쪽 영역 좌표 전달
            float innerLeft = left + 0.5f;
            float innerRight = right - 1.5f;
            float innerBottom = bottomY + 0.5f;
            float innerTop = topY - 0.5f;
            
            onSpawnWalls?.Invoke(new Vector2(innerLeft, innerBottom), new Vector2(innerRight, innerTop));
        }
        
        public void DestroyWalls()
        {
            foreach (var wall in spawnedWalls)
            {
                if (wall != null)
                    Destroy(wall);
            }

            spawnedWalls.Clear();
        }

        private void Start()
        {
            //==================================================================
            // State 관리 part
            BindStage1Events();
            BindStage2Events();
            BindStage3Events();

            //==================================================================
        }

        public override void Startup()
        {
            if (sceneName == "Stage1")
            {
                stateMachine.Push(Stages.Stage1);
            }
        }

        void Update()
        {
            if (gameTime >= maxGameTime && !isStatePoped) // 이지 모드 : 3분
            {
                stateMachine.Pop();

                isStatePoped = true;
            }

            if (!isStageClear && !shouldStopGameTime)
            {
                gameTime += Time.deltaTime; // 게임 시간 증가
            }
        }

        public void ResetStage()
        {
            server_PlayerData.playerGold += server_PlayerData.coin;
            Time.timeScale = 1;
            onChangeScene("Stage1", true);
        }

        public void GoToLobby()
        {
            server_PlayerData.playerGold += server_PlayerData.coin;
            Time.timeScale = 1;
            onChangeScene("Lobby", true);
        }
#if UNITY_EDITOR
        public void GoToStage2()
        {
            stateMachine.Pop();
            onChangeScene("Stage2", false);
        }
        public void GoToStage3()
        {
            stateMachine.Pop();
            onChangeScene("Stage3", false);
        }

        public void MakeRegularStageTimeOver()
        {
            gameTime = maxGameTime - 1f; // 정규 시간 끝나기 1초 전으로 설정
        }
#endif
    }
}