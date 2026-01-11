using Eclipse.Game.Panels;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Eclipse.Game
{
    public class BossManager : Eclipse.Manager
    {
        //==================================================================
        BossGimmickController gimmickController;

        public Boss boss;
        Boss bossPrefab;

        EnemyHPSlider hpSliderPrefab;

        //==================================================================
        GUIManager guiManager;

        string sceneName;
        //==================================================================
        // using Actions
        public Action<Boss> onBossHasKilled;
        public Action onBossSpawned;
        public Action<IEnumerable<Enemy>> onCreatureChange;

        //==================================================================

        private void Awake()
        {
            sceneName = SceneManager.GetActiveScene().name;

            switch (sceneName)
            {
                case "Stage1":
                    // 고블린 투사
                    bossPrefab = Resources.Load<FighterGoblin>("Prefabs/Char_Eclipse/Boss/FighterGoblin/FighterGoblin");
                    hpSliderPrefab = Resources.Load<EnemyHPSlider>("Prefabs/Char_Eclipse/Boss/FighterGoblin/FighterGoblinHpSlider");
                    break;
                case "Stage2":
                    // 스켈레톤 킹
                    bossPrefab = Resources.Load<RuinedKing>("Prefabs/Char_Eclipse/Boss/RuinedKing/RuinedKing");
                    hpSliderPrefab = Resources.Load<EnemyHPSlider>("Prefabs/Char_Eclipse/Boss/RuinedKing/RuinedKingHpSlider");
                    break;
                case "Stage3":
                    // 벨리알(보스)
                    bossPrefab = Resources.Load<Belial>("Prefabs/Char_Eclipse/Boss/Belial/Belial");
                    break;
            }
        }

        private void Start()
        {
            guiManager = client.GetManager<GUIManager>();
        }

        // 보스 소환
        public void SpawnBoss()
        {
            SetBossInfoNSummon();

            onBossSpawned();
        }

        // 보스 Instatiate 및 세팅
        private void SetBossInfoNSummon()
        {
            Server_PlayerData server_PlayerData = PlayerManager.player.server_PlayerData; // 플레이어 데이터 가져오기
            
            // 스테이지 별 해당 보스 Instantiate
            boss = Instantiate(bossPrefab);
            boss.onBossDead = OnBossDead;
            boss.server_PlayerData = server_PlayerData; // 플레이어 데이터 설정

            // 쫄몹 소환 관련) 추후에 부모 BossSkillHandler로 통합 시 수정할 것
            SpawnManager spawnManager = client.GetManager<SpawnManager>();
            PatternManager patternManager = client.GetManager<PatternManager>();

            float nightmareDamageCoefficient = (1 + server_PlayerData.specialPassiveLevels[4] * server_PlayerData.nightmareMode); // nightmare 난이도 데미지 계수
            if (sceneName == "Stage1")
            {
                // 보스 위치 초기화
                Vector2 playerPosition = PlayerManager.player.transform.position;
                Vector2 spawnPosition = new Vector2(playerPosition.x + 8f, playerPosition.y);
                boss.transform.position = spawnPosition;

                ((FighterGoblin)boss).fighterGoblinSkillHandler.onSpawnGimmicEnemies = spawnManager.SpawnConstantEnemiesForStage;
                ((FighterGoblin)boss).fighterGoblinSkillHandler.onSpawnGimmicPoisonSwamp = patternManager.spawnGimmickPosionSwamp;
                ((FighterGoblin)boss).fighterGoblinSkillHandler.nightmareDamageCoefficient = nightmareDamageCoefficient;

                // 보스 체력바 생성
                EnemyHPSlider hpSlider = Instantiate(hpSliderPrefab, guiManager.panelViewer.transform); // hpBar를 panelViewer의 자식으로 생성
                hpSlider.enemy = boss;
                ((FighterGoblin)boss).hpSlider = hpSlider;
            }
            else if (sceneName == "Stage2")
            {
                // 보스 위치 초기화
                Vector2 playerPosition = PlayerManager.player.transform.position;
                Vector2 spawnPosition = new Vector2(playerPosition.x + 2f, -1.5f);
                boss.transform.position = spawnPosition;

                // Action 설정
                ((RuinedKing)boss).skillHandler.onSpawnGimmicEnemies = spawnManager.SpawnConstantEnemiesForStage;
                ((RuinedKing)boss).skillHandler.nightmareDamageCoefficient = nightmareDamageCoefficient;

                // 보스 체력바 생성
                EnemyHPSlider hpSlider = Instantiate(hpSliderPrefab, guiManager.panelViewer.transform); // hpSlider를 panelViewer의 자식으로 생성
                hpSlider.enemy = boss;
                ((RuinedKing)boss).hpSlider = hpSlider;
            }
            else if (sceneName == "Stage3")
            {
                GUIManager gui = client.GetManager<GUIManager>();
                gimmickController = boss.GetComponent<BossGimmickController>();
                gimmickController.guiManager = gui;

                ((Belial)boss).skillHandler.nightmareDamageCoefficient = nightmareDamageCoefficient;

                gimmickController.onStatueChange = onCreatureChange;
            }
        }

        private void OnBossDead()
        {
            onBossHasKilled(boss);
        }

        //==================================================================
    }
}