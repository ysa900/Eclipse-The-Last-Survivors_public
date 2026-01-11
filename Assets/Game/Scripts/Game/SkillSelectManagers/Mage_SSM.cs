using System;
using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Eclipse.Game
{
    public class Mage_SSM : SkillSelectManager
    {
        // 싱글톤 패턴을 사용하기 위한 인스턴스 변수
        private static Mage_SSM _instance;

        // 인스턴스에 접근하기 위한 프로퍼티
        public static Mage_SSM instance
        {
            get
            {
                // 인스턴스가 없는 경우에 접근하려 하면 인스턴스를 할당해준다.
                if (!_instance)
                {
                    _instance = FindAnyObjectByType(typeof(Mage_SSM)) as Mage_SSM;

                    if (_instance == null)
                        Debug.Log("no Singleton obj");
                }
                return _instance;
            }
            set { _instance = value; }
        }

        //==================================================================
        // 액션, 델리게이트 정의
        public Action<int, int, int, bool> onDisplayResonancePanel;
        public Action onResonanceSkillSelect;

        //==================================================================

        // 공명 스킬 관련 변수들
        private int resonanceSkillIndex = -1;
        private int resIndex1 = -1;
        private int resIndex2 = -1;

        protected override void Awake()
        {
            //==================================================================
            // 싱글톤 설정
            if (_instance == null)
            {
                _instance = this;
            }
            // 인스턴스가 존재하는 경우 새로생기는 인스턴스를 삭제한다.
            else if (_instance != this)
            {
                Debug.Log($"{gameObject} Destoried because this is not _instance");
                Destroy(gameObject);
                return;
            }

            // 아래의 함수를 사용하여 씬이 전환되더라도 선언되었던 인스턴스가 파괴되지 않는다.
            DontDestroyOnLoad(gameObject);

            //==================================================================
            selectableSkillCount = 6;
            passiveSkillCount = 106;

            max_skill_num = 18;

            // 공명 스킬 관련 변수들
            resonanceSkillIndex = -1;
            resIndex1 = -1;
            resIndex2 = -1;
            isChoosingSpecialSkill = false;

            // guimanager가 싱글톤이라면 필요한 코드
            //if (isSpecialSkillAlreadyChoosed) activeSkillPanel.RestorePanel(5, 6);
            isSpecialSkillAlreadyChoosed = false;

            dotDamageSkills = new int[] { 2, 7, 8, 11, 12, 13, 14, 15, 16 };

            //==================================================================
            

            //==================================================================
            base.Awake();

            //==================================================================
        }

        void OnEnable()
        {
            // 씬 매니저의 sceneLoaded에 체인을 건다.
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        // 체인을 걸어서 이 함수는 매 씬마다 호출된다.
        protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "Lobby" || scene.name == "Stage1")
            {
                // guimanager가 싱글톤이라면 필요한 코드
                //if (isSpecialSkillAlreadyChoosed) activeSkillPanel.RestorePanel(5, 6);
                isSpecialSkillAlreadyChoosed = false;
            }

            base.OnSceneLoaded(scene, mode);
        }

        public override void SetLevelupPanel()
        {
            Time.timeScale = 0;
            CheckResonanceConditions();
            //RerollObejctInteractiveProcess();
            if (resIndex1 > 0 && resIndex2 > 0 && !isSpecialSkillAlreadyChoosed)
            {
                isChoosingSpecialSkill = true;
                DisplayResonancePanel();
            }
            else
            {
                DisplayNormalWay();
            }
        }

        // 공명 조건 검사
        private void CheckResonanceConditions()
        {
            // 최소 스킬 2개이상 이여야 함
            if (selectedSkills[1] < 0 || isSpecialSkillAlreadyChoosed) return;
            for (int i = 0; i < selectedSkills.Count; i++)
            {
                // 가지고 있는 스킬만 검사 
                if (selectedSkills[i] == -1 || !isSkillMaxLevel[selectedSkills[i]]) continue;
                for (int j = i + 1; j < selectedSkills.Count; j++)
                {
                    if (selectedSkills[j] == -1 || !isSkillMaxLevel[selectedSkills[j]]) continue;
                    if (IsResonate(selectedSkills[i], selectedSkills[j]))
                    {
                        resIndex1 = selectedSkills[i];
                        resIndex2 = selectedSkills[j];
                        return;
                    }
                }
            }
        }

        // 스킬 공명 여부 확인
        private bool IsResonate(int skillIndex1, int skillIndex2)
        {
            resonanceSkillIndex = skillIndex1 switch
            {
                9 when skillIndex2 == 5 => 12,
                5 when skillIndex2 == 9 => 12,
                7 when skillIndex2 == 6 => 13,
                6 when skillIndex2 == 7 => 13,
                11 when skillIndex2 == 7 => 14,
                7 when skillIndex2 == 11 => 14,
                3 when skillIndex2 == 8 => 15,
                8 when skillIndex2 == 3 => 15,
                10 when skillIndex2 == 9 => 16,
                9 when skillIndex2 == 10 => 16,
                5 when skillIndex2 == 10 => 17,
                10 when skillIndex2 == 5 => 17,
                _ => 0
            };
            return resonanceSkillIndex != 0;
        }

        // 공명 시 패널 표시
        private void DisplayResonancePanel()
        {
            onDisplayResonancePanel?.Invoke(resIndex1, resIndex2, resonanceSkillIndex, IsDotDamageSkill(resonanceSkillIndex));
        }

        // 두 번째 스킬 선택 버튼 클릭 핸들러
        public override void SkillSelectButton2Clicked()
        {
            if (isChoosingSpecialSkill && !isSpecialSkillAlreadyChoosed)
            {
                HandleResonanceSkillSelect();
                return;
            }
            HandleSkillSelectButton(1);
        }

        // 공명 스킬 선택 처리
        private void HandleResonanceSkillSelect()
        {
            // 공명하는 두 스킬 하단 선택된 스킬 선택 안한 걸로 처리
            skillData.skillSelected[resIndex1] = false;
            skillData.skillSelected[resIndex2] = false;

            // 공명하는 선택은 선택된 스킬에서 제거
            selectedSkills.Remove(resIndex1);
            selectedSkills.Remove(resIndex2);
            selectedSkills.TrimExcess(); // 메모리 최적화
            selectedSkillsPointer -= 2; // 포인터 위치도 조정해 줌
            if (selectedSkillsPointer < 0) selectedSkillsPointer = 0; // 포인터가 음수로 가지 않도록 함

            // 공명 된 두 스킬은 다시 획득할 수 없게 함
            bannedSkills.Add(resIndex1);
            bannedSkills.Add(resIndex2);

            // 공명 스킬 선택함
            skillData.skillSelected[resonanceSkillIndex] = true;
            skillData.level[resonanceSkillIndex] = 5;
            
            isSkillMaxLevel[resonanceSkillIndex] = true;

            isSpecialSkillAlreadyChoosed = true; // 이게 꼭 앞에 와야함, 이게 true가 되어야 ApplyResonanceToSkillPanel이 실행됨
            ChangeActiveSkillPanel(); // 액티브 스킬 패널 변경
            ApplyResonanceToSkillPanel(); // 공명 스킬 패널에 적용

            onResonanceSkillSelect.Invoke();

            isChoosingSpecialSkill = false;

            PlayerManager.player.isSkillSelectComplete = true;

            // 스킬이 선택되었음을 알리는 이벤트를 호출
            onSkillSelected?.Invoke(resonanceSkillIndex);
        }

        public void ApplyResonanceToSkillPanel()
        {
            if (!isSpecialSkillAlreadyChoosed) return;

            specialSkillPanel.gameObject.SetActive(true);
            specialSkillPanel.SetPanelSkillIcon(resonanceSkillIndex);
            specialSkillPanel.SetPanelSkillLevelText(0, "Lv Max", skillData);
        }

        // 속성 스킬 효과 적용
        protected override void MysterySkillEffectApply(int passiveSkillIndex)
        {
            // 속성 스킬 쿨타임 감소
            for (int i = passiveSkillIndex; i < skillData.level.Length; i += 3)
            {
                if (passiveSkillIndex >= 3) break; // 속성 스킬 아니면 실행 안함

                skillData.delay[i] *= 1 - masterySkillIncrementValue;

                switch (i)
                {
                    case 0:
                        skillData.delay[12] *= 1 - masterySkillIncrementValue;
                        skillData.delay[13] *= 1 - masterySkillIncrementValue;
                        skillData.delay[15] *= 1 - masterySkillIncrementValue;
                        skillData.delay[16] *= 1 - masterySkillIncrementValue;
                        break;
                    case 1:
                        skillData.delay[13] *= 1 - masterySkillIncrementValue;
                        skillData.delay[14] *= 1 - masterySkillIncrementValue;
                        skillData.delay[16] *= 1 - masterySkillIncrementValue;
                        skillData.delay[17] *= 1 - masterySkillIncrementValue;
                        break;
                    case 2:
                        skillData.delay[12] *= 1 - masterySkillIncrementValue;
                        skillData.delay[14] *= 1 - masterySkillIncrementValue;
                        skillData.delay[15] *= 1 - masterySkillIncrementValue;
                        skillData.delay[17] *= 1 - masterySkillIncrementValue;
                        break;
                }
            }
        }

        protected override void UpdateSkillStats(int skillIndex, float damageCoefficient, float delayCoefficient, int? frontCoefficient = null, int? backCoefficient = null)
        {
            base.UpdateSkillStats(skillIndex, damageCoefficient, delayCoefficient, frontCoefficient, backCoefficient);

            if (frontCoefficient.HasValue)
            {
                switch (skillIndex)
                {
                    case 0:
                        skillData.skillCount[skillIndex] += frontCoefficient.Value;
                        break;
                    case 1:
                        skillData.skillCount[skillIndex] += frontCoefficient.Value;
                        break;
                    case 2:
                        skillData.scale[skillIndex] *= ((float)(frontCoefficient.Value + 100) / 100f);
                        break;
                    case 3:
                        skillData.scale[skillIndex] *= ((float)(frontCoefficient.Value + 100) / 100f);
                        break;
                    case 4:
                        break;
                    case 5:
                        skillData.delay[skillIndex] *= ((float)(100 - frontCoefficient.Value) / 100f);
                        break;
                    case 6:
                        skillData.scale[skillIndex] *= ((float)(frontCoefficient.Value + 100) / 100f);
                        break;
                    case 7:
                        skillData.scale[skillIndex] *= ((float)(frontCoefficient.Value + 100) / 100f);
                        break;
                    case 8:
                        skillData.aliveTime[skillIndex] *= ((float)(frontCoefficient.Value + 100) / 100f);
                        break;
                    case 9:
                        skillData.skillCount[skillIndex] += frontCoefficient.Value;
                        break;
                    case 10:
                        skillData.scale[skillIndex] *= ((float)(frontCoefficient.Value + 100) / 100f);
                        break;
                    case 11:
                        skillData.scale[skillIndex] *= ((float)(frontCoefficient.Value + 100) / 100f);
                        break;
                }
            }

            if (backCoefficient.HasValue)
            {
                switch (skillIndex)
                {
                    case 0:
                        break;
                    case 1:
                        break;
                    case 2:
                        skillData.aliveTime[skillIndex] *= ((float)(backCoefficient.Value + 100) / 100f);
                        break;
                    case 3:
                        break;
                    case 4:
                        break;
                    case 5:
                        skillData.aliveTime[skillIndex] *= ((float)(backCoefficient.Value + 100) / 100f);
                        break;
                    case 6:
                        break;
                    case 7:
                        break;
                    case 8:
                        break;
                    case 9:
                        break;
                    case 10:
                        break;
                    case 11:
                        break;
                }
            }
        }

    }

}