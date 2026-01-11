using UnityEngine;

namespace Eclipse.Game
{
    public class Assassin_SSM : SkillSelectManager
    {
        // 싱글톤 패턴을 사용하기 위한 인스턴스 변수
        private static Assassin_SSM _instance;

        // 인스턴스에 접근하기 위한 프로퍼티
        public static Assassin_SSM instance
        {
            get
            {
                // 인스턴스가 없는 경우에 접근하려 하면 인스턴스를 할당해준다.
                if (!_instance)
                {
                    _instance = FindAnyObjectByType(typeof(Assassin_SSM)) as Assassin_SSM;

                    if (_instance == null)
                        Debug.Log("no Singleton obj");
                }
                return _instance;
            }
            set { _instance = value; }
        }

        //==================================================================

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
            /*
             * (현재) 최대 가질 수 있는 스킬 수 : 6
             * 일반 스킬 : 12 ( 3 * 4 ) + 비급(@)
             * 패시브 스킬 개수 : 6 ( 계열별 크리티컬 + 댐감 / 이속 증가 / 자석 ) + 비급
             * 도트 대미지 스킬 : AcidSpot(독운투척), VineTrap(마령등사), 앵화단도 (櫻花短刀)
             */

            selectableSkillCount = 6; // 보유할 수 있는 일반 액티브 개수
            passiveSkillCount = 106; // 공용 패시브 + 계열 패시브 3종
            max_skill_num = 12; // 일반 스킬 12개 + 액티브 비급 1개
            isPassiveSkillMaxLevel = new bool[max_passiveSkill_num];

            // 도트 대미지 스킬 목록
            // 화염지구, 독운투척, 염화매혹, 마령등사, 혼마화진
            dotDamageSkills = new int[] { 1, 2, 4, 5, 11 };

            // 패시브 스킬 초기값 및 증가값
            masterySkillStartValue = 1.15f;
            masterySkillIncrementValue = 0.15f;

            //==================================================================
            base.Awake();

            //==================================================================
        }

        // 계열별 패시브 스킬 올리면 크리티컬 효과 적용
        // 패시브 스킬 레벨에 따라 크리티컬 확률을 증가시키는 메서드

        // MysterySkillEffectApply 메서드 재정의
        // 패시브 스킬 효과를 적용하는 메서드
        protected override void MysterySkillEffectApply(int passiveSkillIndex)
        {

        }

        // 스킬 선택 버튼 핸들러 재정의
        protected override void HandleSkillSelectButton(int index, bool isThirdButton = false)
        {
            if (isChoosingStartSkill)
            {
                if (index == 0 || index == 1 || index == 2) // 비검투척, 화염지구, 독운투척
                {
                    PlayerManager.player.cursorIndicator.gameObject.SetActive(true);
                    PlayerManager.player.cursorIndicator.Init();
                }

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

        // 액티브 스킬 선택 처리 - 재정의
        protected override void HandleActiveSkillSelect(int index)
        {
            if (!skillData.skillSelected[ranNum[index]])
            {
                if (ranNum[index] == 0 || index == 1 || ranNum[index] == 2) // 비검투척, 화염지구, 독운투척
                {
                    PlayerManager.player.cursorIndicator.gameObject.SetActive(true);
                    PlayerManager.player.cursorIndicator.Init();
                }
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

        // 스킬 레벨업 시 올라가는 수치 조정 재정의
        protected override void UpdateSkillStats(int skillIndex, float damageCoefficient, float delayCoefficient, int? frontCoefficient = null, int? backCoefficient = null)
        {
            base.UpdateSkillStats(skillIndex, damageCoefficient, delayCoefficient, frontCoefficient, backCoefficient);

            if (frontCoefficient.HasValue) // 레벨업 시 첫번째 증가효과( 무조건 있음 )
            {
                switch (skillIndex)
                {
                    case 0: // 개수
                            //skillData.skillCount[skillIndex] += frontCoefficient.Value;
                        break;
                    case 1: // 범위
                        skillData.scale[skillIndex] *= ((float)(frontCoefficient.Value + 100) / 100f);
                        break;
                    case 2: // 개수
                            //skillData.skillCount[skillIndex] += frontCoefficient.Value;
                        break;
                    case 3: // 범위
                        skillData.scale[skillIndex] *= ((float)(frontCoefficient.Value + 100) / 100f);
                        break;
                    case 4: // 범위
                        skillData.scale[skillIndex] *= ((float)(frontCoefficient.Value + 100) / 100f);
                        break;
                    case 5: // 범위
                        skillData.scale[skillIndex] *= ((float)(frontCoefficient.Value + 100) / 100f);
                        break;
                    case 6: // 개수 증가
                        skillData.skillCount[skillIndex] += frontCoefficient.Value;
                        break;
                    case 7: // 범위
                        skillData.scale[skillIndex] *= ((float)(frontCoefficient.Value + 100) / 100f);
                        break;
                    case 8: // 범위
                        skillData.scale[skillIndex] *= ((float)(frontCoefficient.Value + 100) / 100f);
                        break;
                    case 9: // 범위
                        skillData.scale[skillIndex] *= ((float)(frontCoefficient.Value + 100) / 100f);
                        break;
                    case 10: // 범위
                        skillData.scale[skillIndex] *= ((float)(frontCoefficient.Value + 100) / 100f);
                        break;
                    case 11: // 범위
                        skillData.scale[skillIndex] *= ((float)(frontCoefficient.Value + 100) / 100f);
                        break;
                }
            }

            if (backCoefficient.HasValue) // 레벨업 시 두번째 효과도 있다면 ( 조건부 존재 )
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
                        break;
                    case 4:
                        break;
                    case 5: // 지속 시간
                        skillData.aliveTime[skillIndex] *= ((float)(backCoefficient.Value + 100) / 100f);
                        break;
                    case 6:
                        break;
                    case 7:
                        break;
                    case 8: // 지속 시간
                        skillData.aliveTime[skillIndex] *= ((float)(backCoefficient.Value + 100) / 100f);
                        break;
                    case 9:
                        break;
                    case 10:
                        break;
                    case 11: // 개수 증가
                        skillData.skillCount[skillIndex] += backCoefficient.Value;
                        break;
                }
            }
        }
    }

}