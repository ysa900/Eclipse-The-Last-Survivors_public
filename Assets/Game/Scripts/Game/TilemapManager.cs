using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Eclipse.Game
{
    public class TilemapManager : Eclipse.Manager
    {
        //==================================================================
        // 사용할 매니저 클래스들
        PlayerManager playerManager;
        GUIManager guiManager;
        StageManager stageManager;

        //==================================================================
        // Stage1 오브젝트, 변수들
        Teleport teleport_hole;

        bool isStage1End;
        bool isTeleportHoleAlreadySpawned;

        //==================================================================
        // Stage2 오브젝트, 변수들
        Corridor[] Corridors;
        BossRoom bossRoom;

        bool isStage2End;
        bool isBossRoomAlreadyMoved;

        //==================================================================
        // 사용할 Action, delegate들
        public Action<string, bool> onChangeScene;

        //==================================================================

        private void Awake()
        {
            string sceneName = SceneManager.GetActiveScene().name;
            if (sceneName == "Stage1")
            {
                teleport_hole = FindAnyObjectByType<Teleport>();
                teleport_hole.gameObject.SetActive(false);
            }
            else if (sceneName == "Stage2")
            {
                teleport_hole = FindAnyObjectByType<Teleport>();
                Corridors = FindObjectsByType<Corridor>(FindObjectsSortMode.None);
                bossRoom = FindAnyObjectByType<BossRoom>();
            }
            
            bool isStage2 = SceneManager.GetActiveScene().name == "Stage2";
            if (isStage2)
            {
                bossRoom.gameObject.SetActive(false);
                bossRoom.onPlayerTriggerEntered = () =>
                {
                    teleport_hole.goTextObject.gameObject.SetActive(false);
                };


                bossRoom.onPlayerTriggerExited = () =>
                {
                    if (teleport_hole.goTextObject == null) return;
                    teleport_hole.goTextObject.gameObject.SetActive(true);
                };
            }
        }

        private void Start()
        {
            playerManager = client.GetManager<PlayerManager>();
            guiManager = client.GetManager<GUIManager>();
            stageManager = client.GetManager<StageManager>();

            string sceneName = SceneManager.GetActiveScene().name;
            if (sceneName == "Stage1")
            {
                teleport_hole.indicator = guiManager.portal_Indicator;
                teleport_hole.onChangeScene = onChangeScene;
            }
            else if (sceneName == "Stage2")
            {
                teleport_hole.goTextObject = guiManager.boss_Portal_Indicator;
                teleport_hole.onChangeScene = onChangeScene;
            }
        }


        private void Update()
        {
            if (stageManager == null) return; // 싱글톤인 stageManager instance가 파괴된 상황이면 return;

            isStage1End = SceneManager.GetActiveScene().name == "Stage1" && stageManager.isStageClear;

            if (isStage1End && !isTeleportHoleAlreadySpawned)
            {
                Vector3 playerPosition = PlayerManager.player.transform.position;
                Vector3 newPos = new Vector3(playerPosition.x, playerPosition.y + 2, 0);
                teleport_hole.transform.position = newPos;
                
                teleport_hole.gameObject.SetActive(true);

                isTeleportHoleAlreadySpawned = true;
            }
            isStage2End = SceneManager.GetActiveScene().name == "Stage2" && stageManager.isStageClear;

            if (isStage2End && !isBossRoomAlreadyMoved)
            {
                Corridor RightCorridor = Corridors[0].transform.position.x > Corridors[1].transform.position.x ? Corridors[0] : Corridors[1];

                Vector2 newPos = new Vector2(RightCorridor.transform.position.x, RightCorridor.transform.position.y);
                newPos.x += 31.5f; // Corridor나 BossRoom 크기 조절하면 바꿔줘야 함
                newPos.y += -1.9f;

                bossRoom.transform.position = newPos;
                bossRoom.gameObject.SetActive(true);

                teleport_hole.goTextObject.gameObject.SetActive(true);

                isBossRoomAlreadyMoved = true;
            }
        }
    }
}