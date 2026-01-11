using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Eclipse.Game
{
    public class Miji_SSM : Eclipse.Manager
    {
        // 싱글톤 패턴
        private static Miji_SSM _instance;

        public static Miji_SSM instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = FindAnyObjectByType(typeof(Miji_SSM)) as Miji_SSM;

                    if (_instance == null)
                    {
                        Debug.Log("no Singleton obj");
                    }
                }
                return _instance;
            }

            set { _instance = value; }
        }

        //==================================================================
        // GUI 패널들
        [ReadOnly] public Panels.MijiActiveSkillPanel activeMijiSkillPanel;
        [ReadOnly] public Panels.MijiPassiveSkillPanel passiveMijiSkillPanel;

        //==================================================================
        // 액션, 델리게이트 정의
        // GUI Manager
        public Action<int, int, MijiSkillData> onChoosingTestSkill;
        public Action onDisplayMijiPanel;

        public Action<int, int, MijiSkillData> onSetActiveMijiSkillPanel;
        public Action<int, int, int, MijiSkillData> onSetPassiveMijiSkillPanel;

        public Action<int, int> onButtonActiveSkillSelected; // 버튼 액티브 스킬 인덱스 넘겨주는 용도
        public Action<int, int> onActiveSkillSelected; // 액티브 스킬 인덱스 넘겨주는 용도
        public Action<int> onPassiveSkillSelected; // 액티브 스킬 인덱스 넘겨주는 용도

        public Action onMijiSkillSelected; // 비급 선택했음을 알려주는 용도

        // 데이터 전달을 위한 액션 델리게이트
        public Action<List<int>, List<int>> onSkillIndicesAssigned;

        //==================================================================
        // 개발용 테스트 플래그 및 테스트 스킬 인덱스
        [SerializeField] private bool isSkillTest;
        [SerializeField] private int testSkillIndex;

        // 최대 액티브 및 패시브 비급 수 상수 정의
        /*int max_ButtonActiceMijiSkill_num = 4; // 버튼 액티브 스킬
        int max_ActiveMijiSkill_num = 1; // 액티브 스킬(사출기)
        int max_PassiveMijiSkill_num = 5; // 패시브 스킬*/

        // 버튼 액티브 스킬
        int buttonActiveSkillCount = 4; // 4개

        // 액티브 스킬 범위 설정
        const int ACTIVESKILLNUM = 100;
        int activeSkillCount = 101; // 1개

        // 패시브 스킬 범위 설정
        const int PASSIVESKILLNUM = 200;
        int passiveSkillCount = 205; // 5개

        // 데이터 클래스 참조
        [ReadOnly] public MijiSkillData buttonActiveMijiSkillData; // 버튼 액티브 스킬
        [ReadOnly] public MijiSkillData activeMijiSkillData; // 사출기(액티브) 스킬
        [ReadOnly] public MijiSkillData passiveMijiSkillData; // 찐 패시브 스킬

        // 랜덤으로 고를 스킬 인덱스
        protected int[] ranNum = new int[3];

        [SerializeField] private List<int> list; // 무작위 선택할 스킬 리스트

        // 선택된 액티브/패시브 비급 리스트
        [SerializeField][ReadOnly] List<int> selectedActiveMijiSkills;
        [SerializeField][ReadOnly] int selectedActiveMijiSkillsPointer = 0;
        [SerializeField][ReadOnly] List<int> selectedPassiveMijiSkills;
        [SerializeField][ReadOnly] int selectedPassiveMijiSkillsPointer = 0;

        bool isActiveAllSelected; // 최대 보유 갯수(2)까지 찍었는 지 확인하는 변수
        bool isPassiveAllSelected; // 최대 보유 갯수(3)까지 찍었는 지 확인하는 변수

        //==================================================================
        /* 비급 스킬 데이터
            
            - 버튼 액티브 스킬 : 액티브 비급 패널
            - 액티브 / 찐 패시브 스킬 : 패시브 비급 패널
         */
        // 버튼 액티브 스킬 데이터 설정
        float[] buttonActiveCoolTimes = { 30f, 10f, 25f, 15f };
        float[] buttonActiveAliveTimes = { 15f, 3f, 5f, 5f };
        float[] buttonActiveCoefficients = { 0.50f, 200f, 0.5f, 0f }; // 순보는 레벨 비례 데미지

        // 액티브 스킬 데이터 설정
        float activeCoolTime = 2f; // 사출기 쿨타임
        float activeLifes = 3f; // 사출기 팅기는 횟수
        float activeDamage = 500f; // 사출기 딜, 레벨 비례 데미지

        // 찐 패시브 스킬 데이터 설정
        private float[] passiveApplyCoefficients = { 0.5f, 0.1f, 0.1f, 0.2f, 0.25f };

        
        private void Awake()
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


            // 변수들 초기화
            ranNum = new int[3];
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

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (this == null) return;

            string sceneName = scene.name;

            if (sceneName == "Stage1")
                InitSkillData();

            if (sceneName == "Stage1" || sceneName == "Stage2" || sceneName == "Stage3")
                Init();
        }

        private void InitSkillData()
        {
            // 변수들 초기화
            ranNum = new int[3];

            // 선택된 스킬 리스트 초기화
            selectedActiveMijiSkills = new List<int>() { -1, -1 };
            selectedActiveMijiSkillsPointer = 0;
            selectedPassiveMijiSkills = new List<int>() { -1, -1, -1 };
            selectedPassiveMijiSkillsPointer = 0;

            for (int i = 0; i < buttonActiveMijiSkillData.skillSelected.Length; i++)
            {
                buttonActiveMijiSkillData.skillSelected[i] = false;
            }

            for (int i = 0; i < activeMijiSkillData.skillSelected.Length; i++)
            {
                activeMijiSkillData.skillSelected[i] = false;
            }

            for (int i = 0; i < passiveMijiSkillData.skillSelected.Length; i++)
            {
                passiveMijiSkillData.skillSelected[i] = false;
            }

            list.Clear();
        }

        public void Init()
        {
            //==================================================================
            // HUD의 패널 설정
            StartCoroutine(WaitforMijiSkillPanel());
        }

        IEnumerator WaitforMijiSkillPanel()
        {
            yield return new WaitUntil(() => (activeMijiSkillPanel != null && passiveMijiSkillPanel != null));

            // 버튼 액티브 비급 패널 초기화
            for (int i = 0; i < selectedActiveMijiSkills.Count; i++)
            {
                if (selectedActiveMijiSkills[i] != -1)
                {
                    activeMijiSkillPanel.SetPanelSkillIcon(i, selectedActiveMijiSkills[i], buttonActiveMijiSkillData);
                    activeMijiSkillPanel.SetPanelSkillLevelText(i, selectedActiveMijiSkills[i], buttonActiveMijiSkillData);
                }
            }

            // 패시브 비급 패널 초기화
            for (int i = 0; i < selectedPassiveMijiSkills.Count; i++)
            {
                if (selectedPassiveMijiSkills[i] != -1)
                {
                    int skillIndex = selectedPassiveMijiSkills[i];
                    Debug.Log($"Processing skillIndex: {skillIndex} at list position {i}");

                    if (skillIndex >= PASSIVESKILLNUM)
                    {
                        int adjustedIndex = skillIndex - PASSIVESKILLNUM;
                        if (adjustedIndex < 0 || adjustedIndex >= passiveMijiSkillData.skillicon.Length)
                        {
                            continue; // 잘못된 인덱스는 무시
                        }

                        passiveMijiSkillPanel.SetPanelSkillIcon(i, adjustedIndex, passiveMijiSkillData);
                        passiveMijiSkillPanel.SetPanelSkillNameText(i, adjustedIndex, passiveMijiSkillData);
                    }
                    else if (skillIndex >= ACTIVESKILLNUM && skillIndex < PASSIVESKILLNUM)
                    {
                        int adjustedIndex = skillIndex - ACTIVESKILLNUM;
                        if (adjustedIndex < 0 || adjustedIndex >= activeMijiSkillData.skillicon.Length)
                        {
                            continue; // 잘못된 인덱스는 무시
                        }

                        passiveMijiSkillPanel.SetPanelSkillIcon(i, adjustedIndex, activeMijiSkillData);
                        passiveMijiSkillPanel.SetPanelSkillNameText(i, adjustedIndex, activeMijiSkillData);
                    }
                }
            }

            onSkillIndicesAssigned?.Invoke(selectedActiveMijiSkills, selectedPassiveMijiSkills);

        }
        
        //===============================================================

        // 레벨업 패널 표시
        public void SetMijiPanel()
        {
            Time.timeScale = 0;

            DisplayMijiSkills();
        }

        private void DisplayMijiSkills()
        {
            list = new List<int>();

            // 현재 플레이어 레벨 가져오기
            int playerLevel = PlayerManager.player.playerData.level;

            // 모든 값이 -1이 아닌 경우에만 true로 설정
            isActiveAllSelected = true;
            for (int i = 0; i < selectedActiveMijiSkills.Count; i++)
            {
                if (selectedActiveMijiSkills[i] == -1)
                {
                    isActiveAllSelected = false;
                    break;
                }
            }

            isPassiveAllSelected = true;
            for (int i = 0; i < selectedPassiveMijiSkills.Count; i++)
            {
                if (selectedPassiveMijiSkills[i] == -1)
                {
                    isPassiveAllSelected = false;
                    break;
                }
            }

            // 액티브 패널이 다 찼으면 패시브 비급 + 액티브 비급 스킬 추가
            if (isActiveAllSelected)
            {
                AddPassiveMijiSkills(list);
                AddActiveMijiSkills(list);
            }
            // 패시브 패널이 다 찼으면 버튼 액티브 비급 스킬만 추가
            else if (isPassiveAllSelected)
            {
                AddButtonActiveMijiSkills(list);
            }
            // 패시브, 액티브 비급 패널둘 다 차지 않은 경우
            else
            {
                // 패시브 비급패널 관련 비급만 list에 추가
                if (playerLevel % 20 != 0)
                {
                    AddActiveMijiSkills(list);
                    AddPassiveMijiSkills(list);
                }
                // 모두 list에 추가
                else
                {
                    AddButtonActiveMijiSkills(list);
                    AddActiveMijiSkills(list);
                    AddPassiveMijiSkills(list);
                }
            }

            if (list.Count > 0)
            {
                // 무작위로 선택된 스킬 인덱스 3개를 ranNum에 할당
                ShuffleRandomSkills();

                // 스킬 패널 표시
                DisplaySkillPanel(list);
            }
        }

        // 선택되지 않은 버튼 액티브 스킬 추가
        private void AddButtonActiveMijiSkills(List<int> list)
        {
            for (int i = 0; i < buttonActiveSkillCount; i++)
            {
                //if (!isButtonActiveMijilSelected[i] && !buttonActiveMijiSkillData.skillSelected[i])
                //{
                //    list.Add(i); // 버튼 액티브 스킬 인덱스 추가
                //}

                if (!buttonActiveMijiSkillData.skillSelected[i])
                {
                    list.Add(i); // 버튼 액티브 스킬 인덱스 추가
                }
            }
        }

        // 선택되지 않은 액티브 스킬 추가
        private void AddActiveMijiSkills(List<int> list)
        {
            for (int i = ACTIVESKILLNUM; i < activeSkillCount; i++) // 액티브 스킬 인덱스는 100부터 시작
            {
                int adjustedIndex = i - ACTIVESKILLNUM; // 인덱스 보정

                //if (!isActiveMijiSelected[adjustedIndex] && !activeMijiSkillData.skillSelected[adjustedIndex])
                //{
                //    list.Add(i); // 액티브 스킬 인덱스 추가
                //}

                if (!activeMijiSkillData.skillSelected[adjustedIndex])
                {
                    list.Add(i); // 액티브 스킬 인덱스 추가
                }
            }
        }

        // 선택되지 않은 찐 패시브 스킬 추가
        private void AddPassiveMijiSkills(List<int> list)
        {
            for (int i = PASSIVESKILLNUM; i < passiveSkillCount; i++) // 패시브 스킬 인덱스는 200부터 시작
            {
                int adjustedIndex = i - PASSIVESKILLNUM; // 인덱스 보정

                //if (!isPassiveMijilSelected[adjustedIndex] && !passiveMijiSkillData.skillSelected[adjustedIndex])
                //{
                //    list.Add(i); // 패시브 스킬 인덱스 추가
                //}

                if (!passiveMijiSkillData.skillSelected[adjustedIndex])
                {
                    list.Add(i); // 패시브 스킬 인덱스 추가
                }
            }
        }


        // 3개의 무작위 스킬 선택
        private void ShuffleRandomSkills()
        {
            List<int> availableSkills = new List<int>(list);
            System.Random random = new System.Random();

            for (int i = 0; i < ranNum.Length; i++)
            {
                int randomIndex = random.Next(availableSkills.Count);
                ranNum[i] = availableSkills[randomIndex];
                availableSkills.RemoveAt(randomIndex);
            }
        }

        // 스킬 패널 표시
        private void DisplaySkillPanel(List<int> list)
        {
            if (list.Count == 0)
            {
                return;
            }

            // 안전장치: list를 다시 정렬
            // "버튼 액티브 스킬 -> 액티브 스킬 -> 찐 패시브 스킬" 순서 유지
            List<int> orderedList = new List<int>();

            AddButtonActiveMijiSkills(orderedList);
            AddActiveMijiSkills(orderedList);
            AddPassiveMijiSkills(orderedList);

            // 기존 list를 대체
            list.Clear();
            list.AddRange(orderedList);

            onDisplayMijiPanel?.Invoke();


            for (int i = 0; i < ranNum.Length; i++)
            {
                // 인덱스 범위에 따라 적절한 스킬데이터로 매칭하기
                if (ranNum[i] < buttonActiveSkillCount)
                {
                    // 버튼 액티브 스킬
                    SetSkillPanel(i, buttonActiveMijiSkillData, ranNum[i]);
                }
                else if (ranNum[i] >= ACTIVESKILLNUM && ranNum[i] < activeSkillCount)
                {
                    // 액티브 스킬
                    int adjustedIndex = ranNum[i] - ACTIVESKILLNUM;
                    SetSkillPanel(i, activeMijiSkillData, adjustedIndex % activeMijiSkillData.skillicon.Length, isActiveSkill: true);
                }
                else if (ranNum[i] >= PASSIVESKILLNUM)
                {
                    // 찐 패시브 스킬
                    int adjustedIndex = ranNum[i] - PASSIVESKILLNUM;
                    SetSkillPanel(i, passiveMijiSkillData, adjustedIndex % passiveMijiSkillData.skillicon.Length, isActiveSkill: false);
                }
            }
        }

        // 스킬 패널 설정
        private void SetSkillPanel(int panelIndex, MijiSkillData mijiSkillData, int skillIndex, bool isActiveSkill = false)
        {
            if (mijiSkillData == buttonActiveMijiSkillData)
            {
                onSetActiveMijiSkillPanel?.Invoke(panelIndex, skillIndex, mijiSkillData);
            }
            else
            {
                int baseIndex = isActiveSkill ? ACTIVESKILLNUM : PASSIVESKILLNUM;
                int adjustedSkillIndex = skillIndex + baseIndex;

                onSetPassiveMijiSkillPanel?.Invoke(panelIndex, adjustedSkillIndex, baseIndex, mijiSkillData);
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
        private void HandleSkillSelectButton(int index, bool isThirdButton = false)
        {
            // ranNum 배열을 사용하여 무작위로 선택된 스킬 인덱스를 가져옴
            int selectedSkillIndex = ranNum[index];

            // 추가 조건: 모든 스킬이 이미 선택된 경우 반환
            if (isActiveAllSelected && selectedActiveMijiSkills.Contains(selectedSkillIndex))
            {
                return;
            }
            if (isPassiveAllSelected && selectedPassiveMijiSkills.Contains(selectedSkillIndex))
            {
                return;
            }

            // 세 번째 버튼이 클릭된 경우
            if (isThirdButton)
            {
                if (isSkillTest)
                {
                    // 테스트 모드에서의 처리
                    bool isActiveTestSkill = testSkillIndex >= ACTIVESKILLNUM && testSkillIndex < PASSIVESKILLNUM;
                    bool isPassiveTestSkill = testSkillIndex >= PASSIVESKILLNUM;

                    // testSkillIndex가 패시브 비급인 경우
                    if (isPassiveTestSkill)
                    {
                        HandlePassiveMijiSkillSelect(testSkillIndex);
                    }
                    // testSkillIndex가 액티브 비급인 경우
                    else if (isActiveTestSkill)
                    {
                        HandlePassiveMijiSkillSelect(testSkillIndex);
                    }
                    // testSkillIndex가 버튼 액티브 비급인 경우
                    else
                    {
                        HandleActiveMijiSkillSelect(testSkillIndex);
                    }
                }
                else
                {
                    // isThirdButton이 참이고 isSkillTest가 거짓인 경우
                    bool isActiveSkill = selectedSkillIndex >= ACTIVESKILLNUM && selectedSkillIndex < PASSIVESKILLNUM;
                    bool isPassiveSkill = selectedSkillIndex >= PASSIVESKILLNUM;

                    // selectedSkillIndex가 패시브 비급인 경우
                    if (isPassiveSkill)
                    {
                        HandlePassiveMijiSkillSelect(selectedSkillIndex);
                    }
                    // selectedSkillIndex가 액티브 비급인 경우
                    else if (isActiveSkill)
                    {
                        HandlePassiveMijiSkillSelect(selectedSkillIndex);
                    }
                    // selectedSkillIndex가 버튼 액티브 비급인 경우
                    else
                    {
                        HandleActiveMijiSkillSelect(selectedSkillIndex);
                    }
                }
            }
            else
            {
                // 일반적인 경우 (세 번째 버튼이 아닌 경우)
                bool isActiveSkill = selectedSkillIndex >= ACTIVESKILLNUM && selectedSkillIndex < PASSIVESKILLNUM;
                bool isPassiveSkill = selectedSkillIndex >= PASSIVESKILLNUM;

                // selectedSkillIndex가 패시브 비급인 경우
                if (isPassiveSkill)
                {
                    HandlePassiveMijiSkillSelect(selectedSkillIndex);
                }
                // selectedSkillIndex가 액티브 비급인 경우
                else if (isActiveSkill)
                {
                    HandlePassiveMijiSkillSelect(selectedSkillIndex);
                }
                // selectedSkillIndex가 버튼 액티브 비급인 경우
                else
                {
                    HandleActiveMijiSkillSelect(selectedSkillIndex);
                }
            }

            //PlayerManager.player.isSkillSelectComplete = true;
        }

        // 버튼 액티브 비급 스킬 처리
        private void HandleActiveMijiSkillSelect(int index)
        {
            if (!buttonActiveMijiSkillData.skillSelected[index])
            {
                buttonActiveMijiSkillData.skillSelected[index] = true;
                buttonActiveMijiSkillData.level[index] = 1;

                // 필요 수치 초기화
                buttonActiveMijiSkillData.Delay[index] = buttonActiveCoolTimes[index];
                buttonActiveMijiSkillData.aliveTime[index] = buttonActiveAliveTimes[index];
                buttonActiveMijiSkillData.levelCoefficient[index] = buttonActiveCoefficients[index];

                // HUD 패널에 버튼 액티브 스킬 적용
                for (int i = 0; i < selectedActiveMijiSkills.Count; i++)
                {
                    if (selectedActiveMijiSkills[i] != -1)
                    {
                        continue;
                    }

                    // 포인터 값이 리스트의 크기를 초과하지 않도록 제한
                    if (selectedActiveMijiSkillsPointer < selectedActiveMijiSkills.Count)
                    {
                        activeMijiSkillPanel.SetPanelSkillIcon(selectedActiveMijiSkillsPointer, index, buttonActiveMijiSkillData);
                        activeMijiSkillPanel.SetPanelSkillLevelText(selectedActiveMijiSkillsPointer, index, buttonActiveMijiSkillData);
                        selectedActiveMijiSkills[selectedActiveMijiSkillsPointer] = index;
                        //isButtonActiveMijilSelected[index] = true;

                        onButtonActiveSkillSelected?.Invoke(index, selectedActiveMijiSkillsPointer);

                        selectedActiveMijiSkillsPointer++;

                        break;
                    }
                }
            }
            else
            {
                return;
            }
        }

        // 패시브 비급 스킬 처리
        private void HandlePassiveMijiSkillSelect(int index)
        {
            bool isActiveSkill = index >= ACTIVESKILLNUM && index < PASSIVESKILLNUM;
            bool isPassiveSkill = index >= PASSIVESKILLNUM;

            int realPassiveSkillIndex;

            // 찐 패시브 비급 처리
            if (isPassiveSkill)
            {
                realPassiveSkillIndex = index - PASSIVESKILLNUM;

                if (!passiveMijiSkillData.skillSelected[realPassiveSkillIndex])
                {
                    passiveMijiSkillData.skillSelected[realPassiveSkillIndex] = true;
                    passiveMijiSkillData.level[realPassiveSkillIndex] = 1;
                    passiveMijiSkillData.levelCoefficient[realPassiveSkillIndex] = passiveApplyCoefficients[realPassiveSkillIndex];

                    // 패시브 비급 슬롯에 스킬이 정상적으로 배치되도록 조건 추가
                    for (int i = 0; i < selectedPassiveMijiSkills.Count; i++)
                    {
                        // 빈 슬롯(-1)이 있는 경우에만 스킬 할당
                        if (selectedPassiveMijiSkills[i] == -1)
                        {
                            passiveMijiSkillPanel.SetPanelSkillIcon(i, realPassiveSkillIndex, passiveMijiSkillData);
                            passiveMijiSkillPanel.SetPanelSkillNameText(i, realPassiveSkillIndex, passiveMijiSkillData);
                            selectedPassiveMijiSkills[i] = index;
                            //isPassiveMijilSelected[realPassiveSkillIndex] = true;

                            onPassiveSkillSelected(realPassiveSkillIndex);
                            selectedPassiveMijiSkillsPointer = i + 1; // 포인터를 다음 빈 슬롯으로 설정
                            break;
                        }
                    }
                }
            }
            // 액티브 비급 처리
            else if (isActiveSkill)
            {
                realPassiveSkillIndex = index - ACTIVESKILLNUM;

                if (!activeMijiSkillData.skillSelected[realPassiveSkillIndex])
                {
                    activeMijiSkillData.skillSelected[realPassiveSkillIndex] = true;
                    activeMijiSkillData.level[realPassiveSkillIndex] = 1;
                    activeMijiSkillData.Delay[realPassiveSkillIndex] = activeCoolTime;
                    activeMijiSkillData.aliveTime[realPassiveSkillIndex] = activeLifes;
                    activeMijiSkillData.levelCoefficient[realPassiveSkillIndex] = activeDamage;

                    // 패시브 비급 슬롯에 액티브 비급 스킬이 정상적으로 배치되도록 조건 추가
                    for (int i = 0; i < selectedPassiveMijiSkills.Count; i++)
                    {
                        // 빈 슬롯(-1)이 있는 경우에만 스킬 할당
                        if (selectedPassiveMijiSkills[i] == -1)
                        {
                            passiveMijiSkillPanel.SetPanelSkillIcon(i, realPassiveSkillIndex, activeMijiSkillData);
                            passiveMijiSkillPanel.SetPanelSkillNameText(i, realPassiveSkillIndex, activeMijiSkillData);
                            selectedPassiveMijiSkills[i] = index;
                            //isActiveMijiSelected[realPassiveSkillIndex] = true;

                            onActiveSkillSelected?.Invoke(realPassiveSkillIndex, selectedPassiveMijiSkillsPointer);
                            selectedPassiveMijiSkillsPointer = i + 1; // 포인터를 다음 빈 슬롯으로 설정

                            break;
                        }
                    }
                }
            }
        }
    }
}