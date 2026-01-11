using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Eclipse.Game
{
    public class Warrior_SSM : SkillSelectManager
    {
        // 싱글톤 패턴을 사용하기 위한 인스턴스 변수
        private static Warrior_SSM _instance;

        // 인스턴스에 접근하기 위한 프로퍼티
        public static Warrior_SSM instance
        {
            get
            {
                // 인스턴스가 없는 경우에 접근하려 하면 인스턴스를 할당해준다.
                if (!_instance)
                {
                    _instance = FindAnyObjectByType(typeof(Warrior_SSM)) as Warrior_SSM;

                    if (_instance == null)
                        Debug.Log("no Singleton obj");
                }
                return _instance;
            }
            set { _instance = value; }
        }

        //==================================================================
        // 액션, 델리게이트 정의
        public Action<int> onDisplayUltimatePanel;
        public Action onUltimateSkillSelect;

        //==================================================================
        //각성 스킬 관련 변수들
        private int ultimatedSkillindex; // 각성 스킬의 인덱스
        private int ultimatedTypeIndex; // 각성 스킬 계열 인덱스
        private List<int> indexesToDeactivate = new List<int>();

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
            max_skill_num = 15;

            dotDamageSkills = new int[] { 2, 3, 4, 7, 8, 9, 10, 11, 12, 13, 14 };

            isChoosingSpecialSkill = false;
            ultimatedTypeIndex = -1;
            ultimatedSkillindex = -1;

            // guimanager가 싱글톤이라면 필요한 코드
            //if (isSpecialSkillAlreadyChoosed) activeSkillPanel.RestorePanel(5, 6);

            isSpecialSkillAlreadyChoosed = false;

            //==================================================================
            base.Awake();

            //==================================================================
        }

        public override void Init()
        {
            if (isSpecialSkillAlreadyChoosed)
            {
                StartCoroutine(WaitforSpecialPanel());
            }

            base.Init();
        }

        IEnumerator WaitforSpecialPanel()
        {
            // specialSkillPanel이 null이 아닐때까지 대기
            yield return new WaitUntil(() => specialSkillPanel != null);

            onMaxLevelIncrease();
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
            if (scene.name == "Lobby")
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
            CheckUltimatedConditions();
            //RerollObejctInteractiveProcess();

            if (ultimatedTypeIndex != -1 && !isSpecialSkillAlreadyChoosed)
            {
                isChoosingSpecialSkill = true;  
                DisplayUltimatePanel();
            }
            else
            {
                DisplayNormalWay();
            }
        }

        private void CheckUltimatedConditions()
        {
            int[] elementCount = new int[] { 0, 0, 0 };

            // 공명 조건: 속성 패시브 스킬 만렙 + 단일 속성 스킬 4개를 소유하고 있어야 함
            if (selectedSkills[3] < 0 || isSpecialSkillAlreadyChoosed) return;

            for (int i = 0; i < selectedSkills.Count; i++)
            {

                //속성 스킬이 만렙이고 해당 스킬 또한 만렙일 경우에만 해당 속성 카운트
                if (selectedSkills[i] == -1 || !isSkillMaxLevel[selectedSkills[i]] || !isPassiveSkillMaxLevel[selectedSkills[i] % 3]) continue;

                elementCount[selectedSkills[i] % 3] += 1;
            }

            for (int i = 0; i < elementCount.Length; i++)
            {
                if (elementCount[i] == 4)
                {
                    ultimatedTypeIndex = i;
                }
            }

            if (ultimatedTypeIndex != -1)
            {
                ultimatedSkillindex = ultimatedTypeIndex switch
                {
                    0 => 12,
                    1 => 13,
                    _ => 14
                };
            }
        }

        private void DisplayUltimatePanel()
        {
            onDisplayUltimatePanel?.Invoke(ultimatedSkillindex);
        }

        public override void SkillSelectButton2Clicked()
        {
            if (isChoosingSpecialSkill && !isSpecialSkillAlreadyChoosed)
            {
                HandleUltimatedSkillSelect();
                return;
            }
            HandleSkillSelectButton(1);
        }

        // 스킬 선택 버튼 핸들러
        protected override void HandleSkillSelectButton(int index, bool isThirdButton = false)
        {
            if (isChoosingStartSkill)
            {
                if (isThirdButton && isSkillTest)
                {
                    skillData.skillSelected[testSkillIndex] = true;
                    skillData.level[testSkillIndex] = 1;
                    activeSkillPanel.SetPanelSkillIcon(selectedSkillsPointer, testSkillIndex, skillData);

                    activeSkillPanel.SetPanelSkillLevelText(selectedSkillsPointer, testSkillIndex, skillData);
                    selectedSkills[selectedSkillsPointer++] = testSkillIndex;
                    onSkillSelected?.Invoke(testSkillIndex);
                }
                else
                {
                    skillData.skillSelected[index] = true;
                    skillData.level[index] = 1;
                    activeSkillPanel.SetPanelSkillIcon(selectedSkillsPointer, index, skillData);

                    activeSkillPanel.SetPanelSkillLevelText(selectedSkillsPointer, index, skillData);
                    selectedSkills[selectedSkillsPointer++] = index;
                }

                isChoosingStartSkill = false;
            }
            else
            {
                bool isPassiveSkill = ranNum[index] >= PASSIVESKILLNUM;
                if (!isPassiveSkill)
                {
                    HandleActiveSkillSelect(index);
                }
                else
                {
                    HandlePassiveSkillSelect(index);
                }
            }

            PlayerManager.player.isSkillSelectComplete = true;
        }

        // 액티브 스킬 선택 처리
        protected override void HandleActiveSkillSelect(int index)
        {
            if (!skillData.skillSelected[ranNum[index]])
            {
                skillData.skillSelected[ranNum[index]] = true;
                skillData.level[ranNum[index]] = 1;
                activeSkillPanel.SetPanelSkillIcon(selectedSkillsPointer, ranNum[index], skillData  );

                activeSkillPanel.SetPanelSkillLevelText(selectedSkillsPointer, ranNum[index], skillData);
                selectedSkills[selectedSkillsPointer++] = ranNum[index];
                onSkillSelected?.Invoke(ranNum[index]);

                if (ranNum[index] == 3) // holyShield 스킬 이면
                    playerData.holyReductionValue = 0.9f;
            }
            else
            {
                IncrementSkillLevel(index);
            }
        }

        private void HandleUltimatedSkillSelect()
        {
            // 각성 속성 스킬 인덱스가 없을 경우 return
            if (ultimatedTypeIndex == -1) return;

            for (int i = 0; i < selectedSkills.Count; i++)
            {
                // 공명한 스킬 계열이 아니거나 비어있는 스킬은 비활성화
                if (selectedSkills[i] % 3 != ultimatedTypeIndex || selectedSkills[i] == -1)
                {
                    indexesToDeactivate.Add(i);
                    if (selectedSkills[i] != -1) skillData.skillSelected[selectedSkills[i]] = false;
                    selectedSkills.RemoveAt(i);
                    i--; // selectedSkills.Count가 줄어듦에 따라 index도 한칸씩 앞으로 당겨져야 함
                }
            }
            selectedSkills.TrimExcess(); // 메모리 최적화를 위해 리스트의 용량을 줄임

            // 각성속성 패시브 스킬외에 다른 패시브 스킬들은 미선택으로 변경
            for (int i = 0; i < 3; i++)
            {
                if (i != ultimatedTypeIndex)
                    passiveSkillData.skillSelected[i] = false;
            }

            // 패시브 스킬 패널 아이콘 제거
            for (int i = 0; i < selectedPassiveSkills.Length; i++)
            {
                if (selectedPassiveSkills[i] == -1) continue;

                if (selectedPassiveSkills[i] < 3 && selectedPassiveSkills[i] != ultimatedTypeIndex)
                {
                    selectedPassiveSkills[i] = -1;
                    selectedPassiveSkillsPointer = 0;
                    isPassiveSkillAllSelected = false;
                }
            }

            // 각성된 속성외의 스킬들은 모두 bannedSkills에 추가함
            for (int i = 0; i < skillData.level.Length; i++)
            {
                if (i % 3 != ultimatedTypeIndex)
                {
                    bannedSkills.Add(i);
                }
            }

            // 다른 속성 패시브 스킬 또한 bannedskills에 추가함
            for (int i = PASSIVESKILLNUM; i <= 102; i++)
            {
                if (i != ultimatedTypeIndex + 100)
                {
                    bannedSkills.Add(i);
                }
            }

            // MaxLevel 초기화
            for (int i = 0; i < isSkillMaxLevel.Length; i++)
            {
                if (isSkillMaxLevel[i] == false) continue;

                if (i % 3 == ultimatedTypeIndex)
                {
                    isSkillMaxLevel[i] = false;
                }
            }

            // maxskill 증가
            maxSkillLevel = 7;

            // GUIManager의 hasMaxLevelIncreased도 true로 변경
            // SetLevelObejctAlpha에서 쓰는 hasMaxLevelIncreased 때문, 추후 더 좋은 구조를 생각하면 변경해도 됨
            onMaxLevelIncrease();

            skillData.skillSelected[ultimatedSkillindex] = true;
            skillData.level[ultimatedSkillindex] = 7;
            isSkillMaxLevel[ultimatedSkillindex] = true;

            isSpecialSkillAlreadyChoosed = true; // 이게 꼭 앞에 와야함, 이게 true가 되어야 ApplyAltimateToSkillPanel이 실행됨
            ChangeActiveSkillPanel(); // 액티브 스킬 패널 변경
            ApplyAltimateToSkillPanel(); // 각성 스킬 패널에 적용

            onUltimateSkillSelect.Invoke();

            isChoosingSpecialSkill = false;

            PlayerManager.player.isSkillSelectComplete = true;

            // 스킬이 선택되었음을 알리는 이벤트를 호출
            onSkillSelected?.Invoke(ultimatedSkillindex);
        }

        
        public void ApplyAltimateToSkillPanel()
        {
            if (!isSpecialSkillAlreadyChoosed) return;
            
            specialSkillPanel.gameObject.SetActive(true);
            specialSkillPanel.SetPanelSkillIcon(ultimatedSkillindex);
            specialSkillPanel.SetPanelSkillLevelText(0, "Lv Max", skillData);
        }

        protected override void MysterySkillEffectApply(int passiveSkillIndex)
        {
            // 계열 스킬 데미지 증가
            for (int i = passiveSkillIndex; i < skillData.level.Length; i += 3)
            {
                if (passiveSkillIndex >= 3) break; // 계열 스킬 아니면 실행 안함

                skillData.damage[i] *= 1 + masterySkillIncrementValue;

                switch (i)
                {
                    case 0:
                        skillData.damage[12] *= 1 + masterySkillIncrementValue;
                        break;
                    case 1:
                        skillData.damage[13] *= 1 + masterySkillIncrementValue;
                        break;
                    case 2:
                        skillData.damage[14] *= 1 + masterySkillIncrementValue;
                        break;
                }
            }
        }

        protected override void UpdateSkillStats(int skillIndex, float damageCoefficient, float delayCoefficient, int? frontCoefficient = null, int? backCoefficient = null)
        {
            skillData.damage[skillIndex] = (float)Math.Round(skillData.damage[skillIndex] * damageCoefficient, 2);
            skillData.delay[skillIndex] = (float)Math.Round(skillData.delay[skillIndex] * delayCoefficient, 2);

            if (frontCoefficient.HasValue)
            {
                switch (skillIndex)
                {
                    case 0:
                        skillData.scale[skillIndex] *= ((float)(frontCoefficient.Value + 100) / 100f);
                        break;
                    case 1:
                        skillData.scale[skillIndex] *= ((float)(frontCoefficient.Value + 100) / 100f);
                        break;
                    case 2:
                        skillData.scale[skillIndex] *= ((float)(frontCoefficient.Value + 100) / 100f);
                        break;
                    case 3:
                        skillData.aliveTime[skillIndex] *= ((float)(frontCoefficient.Value + 100) / 100f);
                        break;
                    case 4:
                        skillData.scale[skillIndex] *= ((float)(frontCoefficient.Value + 100) / 100f);
                        break;
                    case 5:
                        skillData.scale[skillIndex] *= ((float)(frontCoefficient.Value + 100) / 100f);
                        break;
                    case 6:
                        skillData.scale[skillIndex] *= ((float)(frontCoefficient.Value + 100) / 100f);
                        break;
                    case 7:
                        skillData.scale[skillIndex] *= ((float)(frontCoefficient.Value + 100) / 100f);
                        break;
                    case 8:
                        skillData.scale[skillIndex] *= ((float)(frontCoefficient.Value + 100) / 100f);
                        break;
                    case 9:
                        skillData.scale[skillIndex] *= ((float)(frontCoefficient.Value + 100) / 100f);
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
                        break;
                    case 3:
                        playerData.holyReductionValue = 1 - (float)backCoefficient.Value / 100;
                        break;
                    case 4:
                        break;
                    case 5:
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

