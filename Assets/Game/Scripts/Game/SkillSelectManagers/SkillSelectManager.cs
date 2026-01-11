using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Eclipse.Game
{
    public class SkillSelectManager : Eclipse.Manager
    {
        //==================================================================
        // GUI 패널들
        [ReadOnly] public Panels.ActiveSkillPanel[] activeSkillPanels;
        [ReadOnly] public Panels.ActiveSkillPanel activeSkillPanel; // 현재 활성화된 액티브 스킬 패널
        [ReadOnly] public Panels.PassiveSkillPanel passiveSkillPanel;
        [ReadOnly] public Panels.SpecialSkillPanel specialSkillPanel;

        //==================================================================
        // 액션, 델리게이트 정의
        public Action onChoosingStartSkill;
        public Action<int, int, bool> onChoosingTestSkill;
        public Action<int> onDisplayPanelNormalWay;
        public Action onSkillAllMax;
        public Action<int, int, bool> onSetActiveSkillPanel;
        public Action<int, int, int> onSetPassiveSkillPanel;
        public Action onMaxLevelIncrease;

        public delegate void OnSkillSelectObjectDisplayed();
        public OnSkillSelectObjectDisplayed onSkillSelectObjectDisplayed;
        public delegate void OnSkillSelectObjectHided();
        public OnSkillSelectObjectDisplayed onSkillSelectObjectHided;
        public delegate void OnPlayerHealed();
        public OnPlayerHealed onPlayerHealed;
        public delegate void OnSkillSelected(int num);
        public OnSkillSelected onSkillSelected;

        //==================================================================
        // 개발용 테스트 플래그 및 테스트 스킬 인덱스
        [SerializeField] protected bool isSkillTest;
        [SerializeField] protected int testSkillIndex;

        // 최대 스킬 및 패시브 스킬 수 상수 정의
        protected int max_skill_num;
        protected int max_passiveSkill_num = 6;

        // 스킬 최대 레벨 정의
        protected int maxSkillLevel = 5;
        protected int maxPassiveSkillLevel = 5;

        // 현재 고를 수 있는 스킬 수와 패시브 스킬 범위 설정
        protected int selectableSkillCount = 6;
        protected const int PASSIVESKILLNUM = 100;
        protected int passiveSkillCount = 106;

        // 데이터 클래스 참조
        [ReadOnly] public SkillData2 skillData;
        [ReadOnly] public SkillData2 passiveSkillData;
        [ReadOnly] public PlayerData playerData;
        [ReadOnly] public Server_PlayerData server_PlayerData;

        // 랜덤으로 고를 스킬 인덱스
        protected int[] ranNum = new int[3];

        // 선택 가능한 스킬들이 들어갈 리스트
        [SerializeField] List<int> list;

        // 현재 상태를 나타내는 변수들
        public bool isChoosingStartSkill;
        public bool isChoosingSpecialSkill;
        public bool isSpecialSkillAlreadyChoosed;

        // 스킬 및 패시브 스킬 최대 레벨 여부를 저장하는 배열
        [SerializeField][ReadOnly] protected bool[] isSkillMaxLevel;
        [SerializeField][ReadOnly] protected bool[] isPassiveSkillMaxLevel;

        // 도트 데미지 스킬들
        protected int[] dotDamageSkills;

        // 선택된 스킬 및 패시브 스킬 리스트
        [SerializeField][ReadOnly] protected List<int> selectedSkills = new List<int>() { -1, -1, -1, -1, -1, -1 };
        [SerializeField][ReadOnly] protected int selectedSkillsPointer = 0;
        [SerializeField][ReadOnly] protected int[] selectedPassiveSkills = new int[] { -1, -1, -1 };
        [SerializeField][ReadOnly] protected int selectedPassiveSkillsPointer = 0;

        // 금지된 스킬 리스트
        [SerializeField][ReadOnly] protected List<int> bannedSkills = new List<int>();

        // 모든 스킬이 선택되었는지 및 만렙인지 판단하는 변수들
        protected bool isSkillAllSelected;
        protected bool isPassiveSkillAllSelected;
        protected bool isSkillAllMax;

        // 스킬 데미지 미터기
        public int[] damageMeters;

        //==================================================================
        // 여기 값들은 scriptableObject로 옮겨야 함, GUIManager에도 있음
        // 스킬 레벨업 시 계수
        public float normalDamageCoefficient = 1.15f;
        public float normalDelayCoefficient = 0.95f;
        public float maxDamageCoefficient = 1.30f;
        public float maxDelayCoefficient = 0.80f;

        // 패시브 스킬 초기값 및 증가값
        public float masterySkillStartValue = 1.1f;
        public float damageReductionSkillStartValue = 0.05f;
        public float speedUpSkillStartValue = 1.03f;
        public float magnetSkillStartValue = 0.5f;

        public float masterySkillIncrementValue = 0.1f;
        public float damageReductionSkillIncrementValue = 0.05f;
        public float speedUpSkillIncrementValue = 0.03f;
        public float magnetSkillIncrementValue = 0.3f;

        //==================================================================

        // 3, 5레벨 시 증가를 말하는 것
        // 스케일 증가 스킬
        protected List<float> sacleUpSkills = new List<float>();

        // 개수 증가 스킬
        protected List<float> countUpSkills = new List<float>();

        // 지속시간 증가 스킬
        protected List<float> aliveTimeUpSkills = new List<float>();

        // 둔화 스킬
        protected List<float> slowUpSkills = new List<float>();

        protected virtual void Awake()
        {
            // 변수들 초기화
            ranNum = new int[3];

            // 현재 상태를 나타내는 변수들
            isChoosingStartSkill = false;

            // 스킬 및 패시브 스킬 최대 레벨 여부를 저장하는 배열
            isSkillMaxLevel = new bool[max_skill_num];
            isPassiveSkillMaxLevel = new bool[max_passiveSkill_num];

            // 선택된 스킬 및 패시브 스킬 리스트
            selectedSkills = new List<int>() { -1, -1, -1, -1, -1, -1 };
            selectedSkillsPointer = 0;
            selectedPassiveSkills = new int[] { -1, -1, -1 };
            selectedPassiveSkillsPointer = 0;

            // 금지된 스킬 리스트
            bannedSkills.Clear();

            // 모든 스킬이 선택되었는지 및 만렙인지 판단하는 변수들
            isSkillAllSelected = false;
            isPassiveSkillAllSelected = false;
            isSkillAllMax = false;
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
        protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (this == null) return;

            string sceneName = scene.name;
            if (sceneName == "Stage1" || sceneName == "Stage2" || sceneName == "Stage3")
                Init();
        }

        public virtual void Init()
        {
            //==================================================================
            // HUD의 패널 설정
            StartCoroutine(WaitforActivePanel());

            //==================================================================
        }

        IEnumerator WaitforActivePanel()
        {
            // activeSkillPanel이 null이 아닐때까지 대기
            yield return new WaitUntil(() =>  activeSkillPanels != null);

            activeSkillPanel = isSpecialSkillAlreadyChoosed ? activeSkillPanels[1] : activeSkillPanels[0];
            for (int i = 0; i < selectedSkills.Count; i++)
            {
                if (selectedSkills[i] >= 12) continue;
                activeSkillPanel.SetPanelSkillIcon(i, selectedSkills[i], skillData);

                int level = -1;
                if (selectedSkills[i] >= 0)
                {
                    level = skillData.level[selectedSkills[i]];
                }
                if (level >= maxSkillLevel)
                {
                    activeSkillPanel.SetPanelSkillLevelText(i, "Lv Max", skillData);
                }
                else
                {
                    activeSkillPanel.SetPanelSkillLevelText(i, selectedSkills[i], skillData);
                }
            }

            for (int i = 0; i < selectedPassiveSkills.Length; i++)
            {
                passiveSkillPanel.SetPanelSkillIcon(i, selectedPassiveSkills[i], passiveSkillData);
                
                int level = -1;
                if (selectedPassiveSkills[i] >= 0)
                {
                    level = passiveSkillData.level[selectedPassiveSkills[i]];
                }
                if (level >= maxPassiveSkillLevel)
                {
                    passiveSkillPanel.SetPanelSkillLevelText(i, "Lv Max", passiveSkillData);
                }
                else
                {
                    passiveSkillPanel.SetPanelSkillLevelText(i, selectedPassiveSkills[i], passiveSkillData);
                }
            }
        }

        // 시작 스킬 선택
        public void ChooseStartSkill()
        {
            isChoosingStartSkill = true;

            if (isSkillTest)
            {
                SetSkillPanelForTest();
                return;
            }

            onChoosingStartSkill();
        }

        // 테스트 스킬 패널 설정
        private void SetSkillPanelForTest()
        {
            int i = 2; // 테스트 스킬은 3번째 패널에 나오도록
            onChoosingTestSkill?.Invoke(i, testSkillIndex, IsDotDamageSkill(testSkillIndex));

            for (i = 0; i < 2; i++)
            {
                onChoosingTestSkill?.Invoke(i, i, IsDotDamageSkill(testSkillIndex));
            }
        }

        // 레벨업 패널 표시
        public virtual void SetLevelupPanel()
        {
            Time.timeScale = 0;

            DisplayNormalWay();
        }

        // 일반 레벨업 패널 표시
        protected void DisplayNormalWay()
        {
            int playerLevel = playerData.level;
            selectableSkillCount = playerLevel switch
            {
                <= 10 => 6,
                <= 20 => 9,
                _ => 12
            };

            list = new List<int>();

            // 스킬이 전부 선택됐는지 검사
            isSkillAllSelected = true;

            for (int i = 0; i < selectedSkills.Count; i++)
            {
                if (selectedSkills[i] == -1)
                {
                    isSkillAllSelected = false;
                    break;
                }
            }

            // 패시브 스킬이 전부 선택됐는지 검사
            isPassiveSkillAllSelected = true;

            for (int i = 0; i < selectedPassiveSkills.Length; i++)
            {
                if (selectedPassiveSkills[i] == -1)
                {
                    isPassiveSkillAllSelected = false;
                    break;
                }
            }

            if (!isSkillAllSelected)
            {
                AddNonBannedSkills(list);
            }
            else
            {
                AddSelectedNonMaxLevelSkills(list);
            }

            if (!isPassiveSkillAllSelected)
            {
                AddNonPassiveSkills(list);
            }
            else
            {
                AddSelectedNonMaxLevelPassiveSkills(list);
            }
            DisplaySkillPanel(list);
        }

        // 금지되지 않은 스킬 추가
        private void AddNonBannedSkills(List<int> list)
        {
            for (int i = 0; i < selectableSkillCount; i++)
            {
                if (bannedSkills.Contains(i) || i >= 12 || isSkillMaxLevel[i]) continue;

                list.Add(i);
            }
        }

        // 선택된 만렙이 아닌 스킬 추가
        private void AddSelectedNonMaxLevelSkills(List<int> list)
        {
            foreach (int skill in selectedSkills)
            {
                if (bannedSkills.Contains(skill) || skill >= 12 || isSkillMaxLevel[skill]) continue;
                list.Add(skill);
            }
        }

        // 만렙이 아닌 패시브 스킬 추가
        private void AddNonPassiveSkills(List<int> list)
        {
            for (int i = PASSIVESKILLNUM; i < passiveSkillCount; i++)
            {
                if (bannedSkills.Contains(i) || isPassiveSkillMaxLevel[i - PASSIVESKILLNUM]) continue;
                list.Add(i);

            }
        }

        // 선택된 만렙이 아닌 패시브 스킬 추가
        private void AddSelectedNonMaxLevelPassiveSkills(List<int> list)
        {
            foreach (int skill in selectedPassiveSkills)
            {
                if (!isPassiveSkillMaxLevel[skill])
                    list.Add(skill + PASSIVESKILLNUM);
            }
        }

        // 스킬 패널 표시
        private void DisplaySkillPanel(List<int> list)
        {
            if (list.Count == 0)
            {
                onSkillAllMax?.Invoke();

                return;
            }

            onDisplayPanelNormalWay?.Invoke(list.Count);

            // 50% 확률로 첫 번째 패널 선택
            float ranValue = UnityEngine.Random.value;
            int selectedPanelIndex = ranValue < 0.2f ? 0 : -1;

            // 행운 확률을 반영한 두 번째 패널 선택
            ranValue = UnityEngine.Random.value;
            int advancedPanelIndex = ranValue < (server_PlayerData.luck * server_PlayerData.specialPassiveLevels[1]) ? 1 : -1;

            Array.Fill(ranNum, -1); // ranNum 초기화
            for (int i = 0; i < ranNum.Length && list.Count > 0; i++)
            {
                int ran = UnityEngine.Random.Range(0, list.Count);

                // 1개만 남으면 중앙(1번 패널)에 표시
                if (i == 0 && list.Count == 1)
                {
                    ranNum[i + 1] = list[ran];
                    SetSkillPanel(i + 1);
                }
                else
                {
                    ranNum[i] = GetSkillForPanel(i, ran);
                    SetSkillPanel(i);
                }

                list.Remove(ranNum[i]);
            }

            // 선택된 패널에 맞는 스킬을 결정하는 메서드
            int GetSkillForPanel(int panelIndex, int randomIndex)
            {
                if (panelIndex == selectedPanelIndex)
                {
                    List<int> filteredSkills = GetSelectedSkills();
                    filteredSkills.RemoveAll(skill => ranNum.Contains(skill)); // 중복 제거

                    return filteredSkills.Count > 0 
                        ? filteredSkills[UnityEngine.Random.Range(0, filteredSkills.Count)] 
                        : list[randomIndex];
                }

                if (panelIndex == advancedPanelIndex)
                {
                    int lastSkillIndex = selectableSkillCount - 1;
                    List<int> filteredSkills = new List<int>(){ lastSkillIndex - 2, lastSkillIndex - 1, lastSkillIndex };
                    
                    while (filteredSkills.Count > 0)
                    {
                        int selectedIndex = UnityEngine.Random.Range(0, filteredSkills.Count);
                        int chosenSkill = filteredSkills[selectedIndex];

                        // 골라도 되는 스킬에 포함 되지 않고, 이미 선택된 스킬이면 제거
                        if (!list.Contains(chosenSkill) || ranNum.Contains(chosenSkill))
                        {
                            filteredSkills.RemoveAt(selectedIndex);
                        }
                        else
                        {
                            return chosenSkill; // 선택된 스킬 반환
                        }
                    }
                }

                return list[randomIndex]; // 기본 선택
            }

            // 선택된 스킬들을 가져오는 메서드
            List<int> GetSelectedSkills()
            {
                List<int> skillList = new List<int>();

                for (int i = 0; i < skillData.skillSelected.Length; i++)
                {
                    if (skillData.skillSelected[i] && !isSkillMaxLevel[i])
                        skillList.Add(i);
                }

                for (int i = 0; i < passiveSkillData.skillSelected.Length; i++)
                {
                    if (passiveSkillData.skillSelected[i] && !isPassiveSkillMaxLevel[i])
                        skillList.Add(i + 100);
                }

                return skillList;
            }
        }

        // 스킬 패널 설정
        private void SetSkillPanel(int i)
        {
            if (isSkillAllMax) return;
            bool isPassiveSkill = ranNum[i] >= PASSIVESKILLNUM;
            if (!isPassiveSkill)
            {
                onSetActiveSkillPanel?.Invoke(i, ranNum[i], IsDotDamageSkill(ranNum[i]));
            }
            else
            {
                onSetPassiveSkillPanel?.Invoke(i, ranNum[i], PASSIVESKILLNUM);
            }
        }

        // 첫 번째 스킬 선택 버튼 클릭 핸들러
        public void SkillSelectButton1Clicked()
        {
            HandleSkillSelectButton(0);
        }

        // 두 번째 스킬 선택 버튼 클릭 핸들러
        public virtual void SkillSelectButton2Clicked()
        {
            HandleSkillSelectButton(1);
        }

        // 세 번째 스킬 선택 버튼 클릭 핸들러
        public void SkillSelectButton3Clicked()
        {
            HandleSkillSelectButton(2, true);
        }

        // 스킬 선택 버튼 핸들러
        protected virtual void HandleSkillSelectButton(int index, bool isThirdButton = false)
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

                // 시작 스킬들은 고른 거 빼고 절대 안뜨게
                for (int i = 0; i < 3; i++)
                {
                    if (i != index)
                    {
                        bannedSkills.Add(i);
                    }
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
        protected virtual void HandleActiveSkillSelect(int index)
        {
            if (!skillData.skillSelected[ranNum[index]])
            {
                skillData.skillSelected[ranNum[index]] = true;
                skillData.level[ranNum[index]] = 1;
                activeSkillPanel.SetPanelSkillIcon(selectedSkillsPointer, ranNum[index], skillData);

                activeSkillPanel.SetPanelSkillLevelText(selectedSkillsPointer, ranNum[index], skillData);
                selectedSkills[selectedSkillsPointer++] = ranNum[index];
                onSkillSelected?.Invoke(ranNum[index]);
            }
            else
            {
                IncrementSkillLevel(index);
            }
        }

        // 스킬 레벨 증가 처리 재정의
        protected void IncrementSkillLevel(int index)
        {
            skillData.level[ranNum[index]]++;  // 스킬 레벨 증가
            isSkillMaxLevel[ranNum[index]] = skillData.level[ranNum[index]] >= maxSkillLevel; // 최대 레벨 여부 확인
            int skillListIndex = selectedSkills.FindIndex(i => i == ranNum[index]); // 스킬 리스트에서 현재 스킬의 인덱스 찾기
            activeSkillPanel.SetPanelSkillLevelText(skillListIndex, ranNum[index], skillData); // 스킬 레벨 텍스트 업데이트

            if (isSkillMaxLevel[ranNum[index]])
            {
                // 최대 레벨(5 레벨)에 도달했을 때 처리
                BecomeMaxLevel(ranNum[index]);
            }
            else if (skillData.level[ranNum[index]] == 3)
            {
                // 3레벨에 도달했을 때 처리
                BecomeLevelThree(ranNum[index]);
            }
            else
            {
                UpdateSkillStats(ranNum[index], normalDamageCoefficient, normalDelayCoefficient); // 스킬 스탯 업데이트
            }

            // 스킬이 선택되었음을 알리는 이벤트를 호출
            onSkillSelected?.Invoke(ranNum[index]);
        }

        // 3레벨에 도달했을 때의 처리
        protected void BecomeLevelThree(int skillIndex)
        {
            // levelMaxIncrementValue : String형
            var values = skillData.levelThreeIncrementValue[skillIndex].Split(','); // 쉼표를 기준으로 문자열을 분리

            int front_levelThreeIncrementValue = int.Parse(values[0].TrimEnd('%')); // '%' 붙어있으면 떼고 정수로 변환
            int back_levelThreeIncrementValue = values.Length > 1 ? int.Parse(values[1].TrimEnd('%')) : 0; // 두 번째 값이 있으면 정수로 변환하고 없으면 0

            // 스킬 통계 업데이트
            UpdateSkillStats(skillIndex, normalDamageCoefficient, normalDelayCoefficient, front_levelThreeIncrementValue, back_levelThreeIncrementValue == 0 ? null : (int?)back_levelThreeIncrementValue);
        }

        // 최대 레벨에 도달했을 때의 처리
        protected void BecomeMaxLevel(int skillIndex)
        {
            var values = skillData.levelMaxIncrementValue[skillIndex].Split(','); // 쉼표를 기준으로 문자열을 분리
            int front_levelMaxIncrementValue = int.Parse(values[0].TrimEnd('%')); // '%' 붙어있으면 떼고 정수로 변환
            int back_levelMaxIncrementValue = values.Length > 1 ? int.Parse(values[1].TrimEnd('%')) : 0; // 두 번째 값이 있으면 정수로 변환하고 없으면 0

            int skillListIndex = selectedSkills.IndexOf(skillIndex);
            UpdateSkillStats(skillIndex, maxDamageCoefficient, maxDelayCoefficient, front_levelMaxIncrementValue, back_levelMaxIncrementValue == 0 ? null : (int?)back_levelMaxIncrementValue);
            activeSkillPanel.SetPanelSkillLevelText(skillListIndex, "Lv Max", passiveSkillData);
        }

        // 스킬 스탯 업데이트
        protected virtual void UpdateSkillStats(int skillIndex, float damageCoefficient, float delayCoefficient, int? frontCoefficient = null, int? backCoefficient = null)
        {
            skillData.damage[skillIndex] = (float)Math.Round(skillData.damage[skillIndex] * damageCoefficient, 2);
            skillData.delay[skillIndex] = (float)Math.Round(skillData.delay[skillIndex] * delayCoefficient, 2);
        }

        // 패시브 스킬 선택 처리
        protected void HandlePassiveSkillSelect(int index)
        {
            int passiveSkillIndex = ranNum[index] - PASSIVESKILLNUM;
            if (!passiveSkillData.skillSelected[passiveSkillIndex])
            {
                passiveSkillData.skillSelected[passiveSkillIndex] = true;
                passiveSkillData.level[passiveSkillIndex] = 1;

                MysterySkillEffectApply(passiveSkillIndex);

                selectedPassiveSkillsPointer = 0;

                for (int i = 0; i < selectedPassiveSkills.Length; i++)
                {
                    if (selectedPassiveSkills[i] != -1)
                    {
                        selectedPassiveSkillsPointer++;
                        continue;
                    }
                    // 패널 적용
                    passiveSkillPanel.SetPanelSkillIcon(selectedPassiveSkillsPointer, passiveSkillIndex, passiveSkillData);
                    passiveSkillPanel.SetPanelSkillLevelText(selectedPassiveSkillsPointer, passiveSkillIndex, passiveSkillData);
                    // 선택된 패시브 스킬 배열에 추가
                    selectedPassiveSkills[selectedPassiveSkillsPointer++] = passiveSkillIndex;
                    break;
                }

                passiveSkillData.damage[passiveSkillIndex] = GetPassiveSkillStartValue(passiveSkillIndex);
            }
            else
            {
                IncrementPassiveSkillLevel(index, passiveSkillIndex);
            }
        }

        // 속성 스킬 효과 적용
        protected virtual void MysterySkillEffectApply(int passiveSkillIndex)
        {
            // 이거 상속받아서 마스터리 스킬 찍었을 때 효과 적용하면 됨
        }

        // 패시브 스킬 레벨 증가 처리
        protected void IncrementPassiveSkillLevel(int index, int passiveSkillIndex)
        {
            passiveSkillData.level[passiveSkillIndex]++;
            isPassiveSkillMaxLevel[passiveSkillIndex] = passiveSkillData.level[passiveSkillIndex] == maxPassiveSkillLevel;
            int passiveSkillListIndex = Array.IndexOf(selectedPassiveSkills, passiveSkillIndex);
            passiveSkillPanel.SetPanelSkillLevelText(passiveSkillListIndex, passiveSkillIndex, passiveSkillData);
            UpdatePassiveSkillStats(passiveSkillIndex);

            MysterySkillEffectApply(passiveSkillIndex);

            if (isPassiveSkillMaxLevel[passiveSkillIndex]) passiveSkillPanel.SetPanelSkillLevelText(passiveSkillListIndex, "Lv Max", passiveSkillData);
        }

        // 패시브 스킬 수치 업데이트
        protected void UpdatePassiveSkillStats(int passiveSkillIndex)
        {
            switch (passiveSkillIndex)
            {
                case 3: passiveSkillData.damage[passiveSkillIndex] += damageReductionSkillIncrementValue; break;
                case 4: passiveSkillData.damage[passiveSkillIndex] += speedUpSkillIncrementValue; break;
                case 5: passiveSkillData.damage[passiveSkillIndex] += magnetSkillIncrementValue; break;
                default: passiveSkillData.damage[passiveSkillIndex] += masterySkillIncrementValue; break;
            }
        }

        // 패시브 스킬 시작 값 가져오기
        protected float GetPassiveSkillStartValue(int passiveSkillIndex)
        {
            return passiveSkillIndex switch
            {
                3 => damageReductionSkillStartValue,
                4 => speedUpSkillStartValue,
                5 => magnetSkillStartValue,
                _ => masterySkillStartValue
            };
        }

        protected void ChangeActiveSkillPanel()
        {
            activeSkillPanel.gameObject.SetActive(false); // 기존 액티브 스킬 패널 비활성화
            activeSkillPanel = activeSkillPanels[1];      // 두 번째 패널을 선택
            activeSkillPanel.gameObject.SetActive(true);  // 새로운 액티브 스킬 패널 활성화

            for (int i = 0; i < selectedSkills.Count; i++)
            {
                if (selectedSkills[i] >= 12) continue;
                activeSkillPanel.SetPanelSkillIcon(i, selectedSkills[i], skillData);

                int level = -1;
                if (selectedSkills[i] >= 0)
                {
                    level = skillData.level[selectedSkills[i]];
                }
                if (level >= maxSkillLevel)
                {
                    activeSkillPanel.SetPanelSkillLevelText(i, "Lv Max", skillData);
                }
                else
                {
                    activeSkillPanel.SetPanelSkillLevelText(i, selectedSkills[i], skillData);
                }
            }
        }


        //==================================================================
        // 도구
        protected bool IsDotDamageSkill(int skillIndex)
        {
            return Array.Exists(dotDamageSkills, e => e == skillIndex);
        }

        //==================================================================
    }
}