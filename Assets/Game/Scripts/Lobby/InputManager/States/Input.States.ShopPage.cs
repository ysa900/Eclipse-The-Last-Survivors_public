using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Eclipse.Lobby
{
    public partial class InputManager
    {
        [SerializeField] Material goldMaterial;

        public void BindShopPageInputEvents()
        {
            var gui = client.GetManager<GUIManager>();
            var saveDataManager = client.GetManager<SavedataManager>();

            Image[] passiveUpgradeButtonImages = gui.passiveUpgradeButtons
                .Select(button => button.GetComponent<Image>())
                .ToArray();
            Shop.SkullEyeController skullEyeController = gui
                .passiveUpgradeButtons[12]
                .GetComponentInChildren<Shop.SkullEyeController>();

            //==================================================================

            void OnEnter()
            {
                //==================================================================
                /* 초기화 */
                Debug.Log("Enter Shop Page");

                // 업그레이드 누른 스킬들의 index(key)와 업글 횟수(value)로 이루어진 Dictionary
                Dictionary<int, int> upgradeMap = new Dictionary<int, int>();

                int startNum = 0;

                gui.selectButton.Hide();
                gui.passiveUpgradePanels[startNum].gameObject.SetActive(true);

                gui.passiveUpgradePanelTapButtons[startNum].Show();
                gui.passiveUpgradePanelTapClosedButtons[startNum].Hide();

                for (int i = startNum + 1; i < gui.passiveUpgradePanels.Length; i++)
                {
                    gui.passiveUpgradePanels[i].gameObject.SetActive(false);

                    gui.passiveUpgradePanelTapButtons[i].Hide();
                    gui.passiveUpgradePanelTapClosedButtons[i].Show();
                }

                gui.tooltipPanel.Hide();

                int basicLength = server_PlayerData.basicPassiveLevels.Length;

                // 업그레이드 버튼들 저장된 패시브 레벨, 골드 설정
                server_PlayerData.increasedPrice = GetIncreasedPriceInData();

                for (int i = 0; i < gui.levelTexts.Length; i++)
                {
                    int level;
                    if (i < basicLength)
                    {
                        level = server_PlayerData.basicPassiveLevels[i];
                    }
                    else
                    {
                        int index = i - basicLength;
                        level = server_PlayerData.specialPassiveLevels[index];
                    }

                    SetLevelString(i, level);
                    SetPriceString(i);

                    // Max레벨인 업그레이드 버튼 설정
                    UpdateUpgradeButtonState(i, level);
                    SetTooltipContent(i, upgradeMap);
                }

                // 플레이어 보유 골드 표시 설정
                gui.goldText.SetText(server_PlayerData.playerGold.ToString() + " G");

                //==================================================================
                /* 버튼 OnClick 할당 */

                foreach (Shop.PassiveUpgradePanelTapClosedButton item in gui.passiveUpgradePanelTapClosedButtons)
                {
                    item.onClick = () =>
                    {
                        /*
                             OnEnter에서 for문을 돌릴 때 index별로 onClick을 설정해 주지만,
                            해당 버튼의 OnClick이 불릴 때에는 이미 for문이 끝난 이후라 index값은 버튼의 개수+1(현재는 2)가 되는 것이다.
                            따라서 아래 for문에서 if(i == index)로 검사하면 누른 버튼의 index가 들어가는게 아니라 위 for문이 끝난 결과인 index(현재는2)가 들어간다.
                            결론적으로, 의도대로 현재 누른 버튼의 index를 의미하고 싶으면 foreach문을 돌려서 item의 index를 검사해야 한다.
                         */

                        int thisIndex = Array.IndexOf(gui.passiveUpgradePanelTapClosedButtons, item);

                        for (int i = 0; i < gui.passiveUpgradePanels.Length; i++)
                        {
                            if (i == thisIndex)
                            {
                                gui.passiveUpgradePanels[i].gameObject.SetActive(true);

                                gui.passiveUpgradePanelTapButtons[i].Show();

                                gui.passiveUpgradePanelTapClosedButtons[i].Hide();
                            }
                            else
                            {
                                gui.passiveUpgradePanels[i].gameObject.SetActive(false);

                                gui.passiveUpgradePanelTapButtons[i].Hide();

                                gui.passiveUpgradePanelTapClosedButtons[i].Show();
                            }
                        }
                    };
                }

                foreach (Shop.PassiveUpgradeButton item in gui.passiveUpgradeButtons)
                {
                    item.onClick = () =>
                    {
                        int thisIndex = Array.IndexOf(gui.passiveUpgradeButtons, item);
                        UpgradePassive(thisIndex);
                        SetTooltipContent(thisIndex, upgradeMap);
                    };

                    item.onRightClick = () =>
                    {
                        int thisIndex = Array.IndexOf(gui.passiveUpgradeButtons, item);
                        DowngradePassive(thisIndex);
                        SetTooltipContent(thisIndex, upgradeMap);
                    };
                }

                gui.refundButton.onClick = () =>
                {
                    // 환불해야 할 골드 총합 계산
                    int totalUpgrades = 0; // 전체 업그레이드 횟수
                    int refundTotal = 0;
                    int cost;

                    for (int i = 0; i < server_PlayerData.startPrice.Length; i++)
                    {
                        int currentLevel = GetLevelIncludingMap(upgradeMap, i);

                        for (int level = 0; level < currentLevel; level++)
                        {
                            cost = server_PlayerData.startPrice[i] + (totalUpgrades * server_PlayerData.priceIncreseNum); // 다음 업그레이드 비용 증가 반영
                            refundTotal += cost; // 현재 비용 추가
                            totalUpgrades++; // 업그레이드 횟수 증가
                        }
                    }

                    // 현재 플레이어의 골드량 int로 변환
                    string[] guiGoldSplit = gui.goldText.Text.text.Split(' ');
                    int currentGold = int.Parse(guiGoldSplit[0]);

                    // 플레이어의 골드에 적용
                    server_PlayerData.playerGold = currentGold + refundTotal;
                    gui.goldText.SetText(server_PlayerData.playerGold.ToString() + " G");

                    // 업그레이드 가격 증가치 초기화
                    server_PlayerData.increasedPrice = 0;

                    // GUI Clear
                    for (int i = 0; i < gui.passiveUpgradeButtons.Length; i++)
                    {
                        SetLevelString(i, 0);
                        SetPriceString(i);
                        UpdateUpgradeButtonState(i, 0); // 업그레이드 버튼 상태 초기화
                    }

                    // 딕셔너리, 저장된 레벨 배열 Clear
                    upgradeMap.Clear();
                    Array.Clear(server_PlayerData.basicPassiveLevels, 0, server_PlayerData.basicPassiveLevels.Length);
                    Array.Clear(server_PlayerData.specialPassiveLevels, 0, server_PlayerData.specialPassiveLevels.Length);

                    //데이터 저장 필요
                    saveDataManager.SaveServerData();
                };

                gui.selectButton.onClick = () =>
                {
                    foreach (var item in upgradeMap)
                    {
                        int thisIndex = item.Key;
                        if (item.Key < basicLength)
                        {
                            server_PlayerData.basicPassiveLevels[item.Key] = item.Value;
                        }
                        else
                        {
                            int index = item.Key - basicLength;
                            server_PlayerData.specialPassiveLevels[index] = item.Value;
                        }

                        string[] guiGoldSplit = gui.goldText.Text.text.Split(' ');
                        int currentGold = int.Parse(guiGoldSplit[0]);
                        server_PlayerData.playerGold = currentGold;

                        gui.selectButton.Hide();
                    }

                    //데이터 저장 필요
                    saveDataManager.SaveServerData();
                };

                gui.backButton.onClick = () =>
                {
                    stateMachine.Pop();
                };

                gui.backButton.Show();

                //==================================================================
                gui.shopPageViewer.Show();

                //==================================================================

                void UpgradePassive(int index)
                {
                    int currentLevel = GetLevelIncludingMap(upgradeMap, index);

                    int basicLength = server_PlayerData.basicPassiveLevels.Length;
                    int maxLevel = index < basicLength
                                ? server_PlayerData.basicPassiveLevelsMaxNum[index]
                                : server_PlayerData.specialPassiveLevelsMaxNum[index - basicLength];

                    // 최대 레벨 도달 시 업그레이드 차단
                    if (currentLevel >= maxLevel)
                        return;

                    int needGoldAmount = server_PlayerData.startPrice[index] + GetIncreasedPriceIncludingMap(upgradeMap);

                    if (upgradeMap.ContainsKey(index))
                    {
                        upgradeMap[index] += 1;
                    }
                    else
                    {
                        upgradeMap[index] = currentLevel + 1;
                    }

                    string[] guiGoldSplit = gui.goldText.Text.text.Split(' ');
                    int currentGold = int.Parse(guiGoldSplit[0]);

                    if (needGoldAmount > currentGold)
                    {
                        upgradeMap[index] -= 1;
                        return;
                    }

                    currentGold -= needGoldAmount;
                    gui.goldText.SetText(currentGold + " G");

                    SetLevelString(index, upgradeMap[index]);

                    int increasedPrice = GetIncreasedPriceIncludingMap(upgradeMap);
                    for (int i = 0; i < gui.priceTexts.Length; i++)
                    {
                        gui.priceTexts[i].SetText((server_PlayerData.startPrice[i] + increasedPrice).ToString());
                    }

                    UpdateUpgradeButtonState(index, upgradeMap[index]);

                    gui.selectButton.Show();
                }

                void DowngradePassive(int index)
                {
                    int basicLength = server_PlayerData.basicPassiveLevels.Length;
                    int baseLevel = index < basicLength
                        ? server_PlayerData.basicPassiveLevels[index]
                        : server_PlayerData.specialPassiveLevels[index - basicLength];

                    int currentLevel = GetLevelIncludingMap(upgradeMap, index);

                    // 회수 불가: 기본 레벨이 0이고, 업그레이드도 없을 때
                    if (currentLevel <= 0)
                        return;

                    // upgradeMap에 추가되지 않았지만 회수 대상이면 추가
                    if (!upgradeMap.ContainsKey(index))
                    {
                        upgradeMap[index] = currentLevel - 1;
                    }
                    else
                    {
                        upgradeMap[index] -= 1;
                    }

                    int newLevel = upgradeMap[index];

                    // 골드 환급 계산
                    int refundAmount = server_PlayerData.startPrice[index] + GetIncreasedPriceIncludingMap(upgradeMap);

                    // 골드 증가
                    string[] guiGoldSplit = gui.goldText.Text.text.Split(' ');
                    int currentGold = int.Parse(guiGoldSplit[0]);
                    currentGold += refundAmount;

                    gui.goldText.SetText(currentGold + " G");

                    // UI 갱신
                    SetLevelString(index, newLevel);

                    int increasedPrice = GetIncreasedPriceIncludingMap(upgradeMap);
                    for (int i = 0; i < gui.priceTexts.Length; i++)
                    {
                        gui.priceTexts[i].SetText((server_PlayerData.startPrice[i] + increasedPrice).ToString());
                    }

                    UpdateUpgradeButtonState(index, upgradeMap[index]);

                    gui.selectButton.Show();
                }
            };

            int GetIncreasedPriceIncludingMap(Dictionary<int, int> upgradeMap)
            {
                int levelSum = GetTotalLevelIncludingMap(upgradeMap);

                int increasedPrice = levelSum * server_PlayerData.priceIncreseNum;
                return increasedPrice;
            }

            int GetLevelIncludingMap(Dictionary<int, int> upgradeMap, int index)
            {
                int level;

                if (upgradeMap.ContainsKey(index))
                {
                    level = upgradeMap[index];
                }
                else
                {
                    int basicLengh = server_PlayerData.basicPassiveLevels.Length;
                    level = index < basicLengh
                                ? server_PlayerData.basicPassiveLevels[index]
                                : server_PlayerData.specialPassiveLevels[index - basicLengh];
                }

                return level;
            }

            int GetTotalLevelIncludingMap(Dictionary<int, int> upgradeMap)
            {
                int levelSum = 0;

                for (int i = 0; i < gui.levelTexts.Length; i++)
                {
                    levelSum += GetLevelIncludingMap(upgradeMap, i);
                }

                return levelSum;
            }

            int GetIncreasedPriceInData()
            {
                int levelSum = 0;

                for (int i = 0; i < server_PlayerData.basicPassiveLevels.Length; i++)
                {
                    levelSum += server_PlayerData.basicPassiveLevels[i];
                }
                for (int i = 0; i < server_PlayerData.specialPassiveLevels.Length; i++)
                {
                    levelSum += server_PlayerData.specialPassiveLevels[i];
                }

                int increasedPrice = levelSum * server_PlayerData.priceIncreseNum;
                return increasedPrice;
            }

            void SetLevelString(int index, int level)
            {
                string levelText = "Lv.";
                int basicLength = server_PlayerData.basicPassiveLevels.Length;
                int maxLevel = index < basicLength
                            ? server_PlayerData.basicPassiveLevelsMaxNum[index]
                            : server_PlayerData.specialPassiveLevelsMaxNum[index - basicLength];
                levelText += level == maxLevel
                            ? "Max"
                            : $"{level} / {maxLevel}";
                gui.levelTexts[index].SetText(levelText);
            }

            void SetPriceString(int index)
            {
                /*//이거는 업그레이드 골드량이 각 스킬 레벨별로 따로 처리될 때 코드
                string priceText = surver_PlayerData.goldIncreseNum[level].ToString();*/

                // 이거는 업그레이드 골드량이 공통으로 처리될 때 코드
                string priceText = (server_PlayerData.startPrice[index] + server_PlayerData.increasedPrice).ToString();
                gui.priceTexts[index].SetText(priceText);
            }

            void UpdateUpgradeButtonState(int index, int level)
            {
                int basicLength = server_PlayerData.basicPassiveLevels.Length;
                int maxLevel = index < basicLength
                            ? server_PlayerData.basicPassiveLevelsMaxNum[index]
                            : server_PlayerData.specialPassiveLevelsMaxNum[index - basicLength];

                bool isMax = level >= maxLevel;

                // 이미지 설정
                if (index == 12) // 악몽은 별도 처리
                {
                    skullEyeController.SetGlowIntensity(level); // 스컬 아이 효과 업데이트
                }
                else
                {
                    passiveUpgradeButtonImages[index].material = isMax ? goldMaterial : null;
                }
            }

            void SetTooltipContent(int index, Dictionary<int, int> upgradeMap)
            {
                int basicLength = server_PlayerData.basicPassiveLevels.Length;
                int level = GetLevelIncludingMap(upgradeMap, index);
                int maxLevel = index < basicLength
                            ? server_PlayerData.basicPassiveLevelsMaxNum[index]
                            : server_PlayerData.specialPassiveLevelsMaxNum[index - basicLength];

                bool isMax = level >= maxLevel;

                string nameText = passiveUpgradeButtonImages[index].GetComponentsInChildren<TextMeshProUGUI>()[0].text;
                float increasePerLevel;
                switch (nameText)
                {
                    case "공격력":
                        nameText = "공격력 증가";
                        increasePerLevel = server_PlayerData.attackPower;
                        break;
                    case "방어력":
                        nameText = "방어력 증가";
                        increasePerLevel = server_PlayerData.defense;
                        break;
                    case "최대 체력":
                        nameText = "최대 체력 증가";
                        increasePerLevel = server_PlayerData.maxHealth;
                        break;
                    case "회복":
                        nameText = "회복 효율 증가";
                        increasePerLevel = server_PlayerData.regen;
                        break;
                    case "쿨타임 감소":
                        nameText = "쿨타임 감소";
                        increasePerLevel = server_PlayerData.cooldown;
                        break;
                    case "공격 범위":
                        nameText = "공격 범위 증가";
                        increasePerLevel = server_PlayerData.attackRange;
                        break;
                    case "이동속도":
                        nameText = "이동 속도 증가";
                        increasePerLevel = server_PlayerData.moveSpeed;
                        break;
                    case "지속시간":
                        nameText = "스킬 지속 시간";
                        increasePerLevel = server_PlayerData.duration;
                        break;
                    case "자석":
                        nameText = "자석 효과";
                        increasePerLevel = server_PlayerData.magnetPower;
                        break;
                    case "행운":
                        nameText = "행운 증가";
                        increasePerLevel = server_PlayerData.luck;
                        break;
                    case "성장":
                        nameText = "경험치 획득량 증가";
                        increasePerLevel = server_PlayerData.expBoost;
                        break;
                    case "풍요":
                        nameText = "골드 획득량 증가";
                        increasePerLevel = server_PlayerData.goldBoost;
                        break;
                    case "악몽":
                        nameText = "난이도 증가";
                        increasePerLevel = server_PlayerData.nightmareMode;
                        break;
                    case "부활":
                        nameText = "부활 가능 여부";
                        increasePerLevel = server_PlayerData.canRevive;
                        break;
                    case "다시 뽑기":
                        nameText = "다시 뽑기 가능 횟수";
                        increasePerLevel = server_PlayerData.rerollCount;
                        break;
                    default:
                        nameText = "알 수 없는 업그레이드";
                        increasePerLevel = 0f;
                        break;
                }
                int currentLevel = GetLevelIncludingMap(upgradeMap, index);
                bool isPercent = increasePerLevel < 1f;

                string increaseText = isPercent
                    ? $"{increasePerLevel * 100f}%"
                    : $"{increasePerLevel}";

                string currentEffectValue = isPercent
                    ? $"{increasePerLevel * 100f * currentLevel}%"
                    : $"{increasePerLevel * currentLevel}";

                string currentEffect = $"현재: {currentEffectValue}";

                string tooltipContent = $"레벨당 {nameText} + {increaseText}\n{currentEffect}";

                if (!isMax)
                {
                    string nextEffectValue = isPercent
                        ? $"{increasePerLevel * 100f * (currentLevel + 1)}%"
                        : $"{increasePerLevel * (currentLevel + 1)}";

                    string nextEffect = $"다음: {nextEffectValue}";

                    tooltipContent += $" -> {nextEffect}";
                }

                var trigger = gui.tooltipTriggers[index];
                trigger.tooltipContent = tooltipContent;

                // 툴팁이 현재 떠있다면 바로 갱신
                trigger.Refresh();
            }

            //==================================================================

            void OnExit()
            {
                gui.shopPageViewer.Hide();
            }

            //==================================================================

            State<States> state = new State<States>(States.ShopPage);
            state.onEnter = OnEnter;
            state.onExit = OnExit;

            stateMachine.Add(state);
        }

        private void ActLikeShopPageBackButton()
        {
            Eclipse.Lobby.AudioManager.instance.PlaySfx((int)Eclipse.Lobby.AudioManager.Sfx.Select);

            stateMachine.Pop();
        }
    }
}