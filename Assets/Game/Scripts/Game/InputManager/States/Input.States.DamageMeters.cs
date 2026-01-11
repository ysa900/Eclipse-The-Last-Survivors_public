using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.Game
{
    public partial class InputManager
    {
        private void BindDamageMetersEvent()
        {
            GUIManager gui = client.GetManager<GUIManager>();
            SkillManager skillManager = client.GetManager<SkillManager>();

            void OnEnter()
            {
                gui.damageMeters_playButton.onClick = () =>
                {
                    stateMachine.Pop();
                };

                SetDamageMeters(); // 미터기 세팅
                gui.damageMetersViewer.Show();

                Time.timeScale = 0f;
            }

            void OnExit()
            {
                gui.damageMetersViewer.Hide();

                Time.timeScale = 1f;
            }

            var state = new State<States>(States.DamageMeters);
            state.onEnter = OnEnter;
            state.onExit = OnExit;

            stateMachine.Add(state);

            // ==================================================================
            void SetDamageMeters()
            {
                //==================================================================
                SkillData2 skillData = skillManager.skillData;

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

                Image[] skillIcons = gui.damageMeterSkills.GetComponentsInChildren<Image>(true);
                foreach (Image icon in skillIcons)
                {
                    icon.gameObject.SetActive(true);
                }
                TextMeshProUGUI[] tempText = gui.damageMeterSkills.GetComponentsInChildren<TextMeshProUGUI>();

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
                for (int skillIndex = 0; count < skillIcons.Count(); skillIndex++)
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

                    if (skillIndex >= damageMeters.Length)
                    {
                        skillIcons[count].gameObject.SetActive(false);
                        count++;
                        continue;
                    }

                    if (damageMeters[skillIndex] > 0)
                    {
                        Image icon = skillIcons[count].GetComponent<Image>();
                        icon.sprite = skillData.skillicon[skillIndex];

                        TextMeshProUGUI levelText = skillLevelTexts[count].GetComponent<TextMeshProUGUI>();
                        levelText.text = $"Lv.{skillData.level[skillIndex].ToString()}";

                        TextMeshProUGUI damageText = skillDamageTexts[count].GetComponent<TextMeshProUGUI>();
                        damageText.text = damageMeters[skillIndex].ToString();
                        count++;
                    }
                }
            }

        }
        private void ActLikeDamageMetersStateStartButton()
        {
            Eclipse.Game.AudioManager.instance.PlaySfx(Eclipse.Game.AudioManager.Sfx.Select);

            stateMachine.Pop();
        }
    }
}