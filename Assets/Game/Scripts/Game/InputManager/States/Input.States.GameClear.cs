using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.Game
{
    public partial class InputManager
    {
        private void BindGameClearEvents()
        {
            GUIManager gui = client.GetManager<GUIManager>();
            StageManager stageManager = client.GetManager<StageManager>();
            SkillManager skillManager = client.GetManager<SkillManager>();
            PlayerManager playerManager = client.GetManager<PlayerManager>();
            SavedataManager saveDataManager = client.GetManager<SavedataManager>();
            Server_PlayerData server_PlayerData = saveDataManager.server_PlayerData;

            void OnEnter()
            {
                AudioManager.instance.SwitchBGM((int)AudioManager.Bgm.Game_Clear);

                gui.gameClear_goToLobbyButton.onClick = () =>
                {
                    saveDataManager.UpdatePlayerGold();
                    saveDataManager.SaveServerData();
                    stageManager.GoToLobby();
                };

                SetClearPage(); // 클리어 데이터 설정
                gui.gameClearPageViewer.Show();

                Time.timeScale = 0f;
            }

            void OnExit()
            {
                gui.gameClearPageViewer.Hide();

                Time.timeScale = 1f;
            }

            var state = new State<States>(States.GameClear);
            state.onEnter = OnEnter;
            state.onExit = OnExit;

            stateMachine.Add(state);

            // 클리어 데이터 설정
            void SetClearPage()
            {
                //==================================================================
                PlayerData playerData = playerManager.playerData;
                SkillData2 skillData = skillManager.skillData;

                //==================================================================
                // 클리어 시간, 처치한 적 수, 획득한 골드 표시
                int min = Mathf.FloorToInt(stageManager.gameTime / 60);
                int sec = Mathf.FloorToInt(stageManager.gameTime % 60);
                gui.clearTimeText.GetComponent<TextMeshProUGUI>().text = string.Format("{0:D2}:{1:D2}", min, sec);
                gui.enemyKillText_GameClear.GetComponent<TextMeshProUGUI>().text = playerData.kill.ToString();
                gui.goldCollectedText_GameClear.GetComponent<TextMeshProUGUI>().text = server_PlayerData.coin.ToString();
                int nightmareLevel = server_PlayerData.specialPassiveLevels[4];
                string text = nightmareLevel > 0 
                    ? $"<color=#FF3030>악몽</color><size={120f}%><color=#9B30FF> {nightmareLevel}단계</color></size><color=#FF3030> 클리어</color>"
                    : $"<color=#FFD700>클리어</color>";
                gui.clearMainText.SetText(text);

                //==================================================================
                // gui 정보 가져오기
                int[] damageMeters = skillManager.ReturnDamageMeters();
                int playerClass = PlayerPrefs.GetInt("PlayerClass");
                /*int[] mijiMeters = null;
                MijiSkillData activeMijiSkillData = null;
                if (playerClass == 2)
                {
                    Miji_SkillManager miji_skillManager = client.GetManager<Miji_SkillManager>();
                    mijiMeters = miji_skillManager.ReturnDamageMeters();
                    activeMijiSkillData = miji_skillManager.activeMijiSkillData;
                }*/

                Image[] skillIcons = gui.skills.GetComponentsInChildren<Image>();
                TextMeshProUGUI[] tempText = gui.skills.GetComponentsInChildren<TextMeshProUGUI>();

                // tempText 배열에서 짝수 인덱스의 요소를 추출하여 skillDamageTexts 배열에 할당
                TextMeshProUGUI[] skillDamageTexts = tempText
                    .Where((_, index) => index % 2 == 0)
                    .ToArray();

                // tempText 배열에서 홀수 인덱스의 요소를 추출하여 skillLevelTexts 배열에 할당
                TextMeshProUGUI[] skillLevelTexts = tempText
                    .Where((_, index) => index % 2 != 0)
                    .ToArray();

                //==================================================================
                // 미터기 띄우기
                int maxIndex = damageMeters.Length;
                /*int maxIndex = mijiMeters == null 
                    ? damageMeters.Length 
                    : damageMeters.Length + mijiMeters.Length;*/

                int count = 0;
                for (int i = 0; count < skillIcons.Count(); i++)
                {
                    /*if (i >= damageMeters.Length)
                    {
                        if (mijiMeters != null && mijiMeters[i] > 0 && i < maxIndex)
                        {
                            // 여기서 어떤 미지 데이터의 아이콘과 텍스트를 가져올 지 작성해야 함.
                            // 현재 데미지 주는 비급 스킬은 슈리켄, 쉐파, 쉬프트
                            // 두개는 버튼액티브미지스킬(쉐파, 쉬프트), 하나는 액티브미지스킬(슈리켄)
                            // 쉐파는 현재 따로 기록하는 기능이 없음, 만들어야 함
                            *//*
                             * MijiSkillData mijiSkillData = ???
                             * ~~~~~~~~~~~~~~
                             *//*
                            

                            Image icon = skillIcons[count].GetComponent<Image>();
                            icon.sprite = activeMijiSkillData.skillicon[i - damageMeters.Length];

                            TextMeshProUGUI levelText = skillLevelTexts[count].GetComponent<TextMeshProUGUI>();
                            levelText.text = $"Lv.{activeMijiSkillData.level[i - damageMeters.Length].ToString()}";

                            TextMeshProUGUI damageText = skillDamageTexts[count].GetComponent<TextMeshProUGUI>();
                            damageText.text = mijiMeters[i - damageMeters.Length].ToString();
                        }
                        else
                        {
                            skillIcons[count].gameObject.SetActive(false);
                        }

                        count++;
                        continue;
                    }*/

                    if (i >= damageMeters.Length)
                    {
                        skillIcons[count].gameObject.SetActive(false);
                        count++;
                        continue;
                    }

                    if (damageMeters[i] > 0)
                    {
                        Image icon = skillIcons[count].GetComponent<Image>();
                        icon.sprite = skillData.skillicon[i];

                        TextMeshProUGUI levelText = skillLevelTexts[count].GetComponent<TextMeshProUGUI>();
                        levelText.text = $"Lv.{skillData.level[i].ToString()}";

                        TextMeshProUGUI damageText = skillDamageTexts[count].GetComponent<TextMeshProUGUI>();
                        damageText.text = damageMeters[i].ToString();
                        count++;
                    }
                }

                gui.characterUnlockImage.gameObject.SetActive(false);
                if (!server_PlayerData.isWarriorUnlocked)
                {
                    gui.characterUnlockImage.gameObject.SetActive(true);
                    gui.characterUnlockImage.ShowCharacterUnlock("전사");
                    server_PlayerData.isWarriorUnlocked = true;
                }
                else if (!server_PlayerData.isAssassinUnlocked && playerClass == 1) // 전사로 클리어 했을 때
                {
                    gui.characterUnlockImage.gameObject.SetActive(true);
                    gui.characterUnlockImage.ShowCharacterUnlock("도적");
                    server_PlayerData.isAssassinUnlocked = true;
                }
            }
        }
    }
}

