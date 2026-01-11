using Eclipse.Game.Panels;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.Game
{
    public partial class GUIManager
    {
        private Coroutine[] buttonActiveCooldownCoroutines = new Coroutine[4]; // 버튼 액티브 스킬의 개수만큼 초기화
        private Coroutine activeSkillCooldownCoroutine;
        private Coroutine stealthRemainTimeCoroutine;

        //==================================================================
        // 비급 스킬 선택 시 처리
        public void OnDisplayMijiPanel()
        {
            // 스킬 패널 개수 3개 이상 무조건 고정됨(비급 총 : 10개, 최대 보유: 5개)
            title.GetComponentInChildren<TextMeshProUGUI>().text = "비급";

            // 스킬 패널 개수에 따른 UI 설정
            skillPanels[2].gameObject.SetActive(true);
            levels[2].gameObject.SetActive(true);

            skillPanels[1].gameObject.SetActive(true);
            levels[1].gameObject.SetActive(true);

            skillPanels[0].gameObject.SetActive(true);
            levels[0].gameObject.SetActive(true);
        }

        //==================================================================
        // 액티브 비급 패널 세팅
        public void OnSetActiveMijiSkillPanel(int index, int skillIndex, MijiSkillData mijiSkillData)
        {
            Image icon = mijiSkillIcons[index].GetComponent<Image>();
            icon.sprite = mijiSkillData.skillicon[skillIndex];

            string color = mijiSkillColorCodeString[0];

            TextMeshProUGUI textName = mijiSkillTextNames[index].GetComponent<TextMeshProUGUI>();
            textName.text = $"<color={color}>{mijiSkillData.skillName[skillIndex]}</color>";

            TextMeshProUGUI textDescription = mijiSkillTextDescriptions[index].GetComponent<TextMeshProUGUI>();

            string description = "";

            TextMeshProUGUI mijiSkillTypeText = mijiTypeDescriptions[index].GetComponent<TextMeshProUGUI>();
            // 유저 친화적인 '스킬 설명' 인터페이스 위한 카테고리 분류
            mijiSkillTypeText.text = "액티브";

            description += mijiSkillData.skillDescription[skillIndex] + "\n";
            textDescription.text = $"<color=#FFFFFF>{description}</color>"; // 스킬 설명 글씨색 흰색으로 통일

            // 버튼 액티브 비급
            // mijiTypeImages[0]의 Sprite 설정
            Image[] levelImages = mijiLevels[index].GetComponentsInChildren<Image>();
            foreach (var img in levelImages)
            {
                img.sprite = mijiTypeImages[0];
            }

            mijiSkillBoundaries[index].GetComponent<Image>().color = HexToColor(miji_Boundary_Color[0]);
        }

        // 패시브 비급 패널 세팅
        public void OnSetPassiveMijiSkillPanel(int index, int skillIndex, int CONSTNUM, MijiSkillData mijiSkillData)
        {
            Image icon = mijiSkillIcons[index].GetComponent<Image>();
            icon.sprite = mijiSkillData.skillicon[skillIndex - CONSTNUM];

            string color = mijiSkillColorCodeString[1];

            TextMeshProUGUI textName = mijiSkillTextNames[index].GetComponent<TextMeshProUGUI>();
            textName.text = $"<color={color}>{mijiSkillData.skillName[skillIndex - CONSTNUM]}</color>";

            TextMeshProUGUI mijiSkillTypeText = mijiTypeDescriptions[index].GetComponent<TextMeshProUGUI>();
            // 유저 친화적인 '스킬 설명' 인터페이스 위한 카테고리 분류
            mijiSkillTypeText.text = "패시브";

            TextMeshProUGUI textDescription = mijiSkillTextDescriptions[index].GetComponent<TextMeshProUGUI>();

            string description = "";

            description += mijiSkillData.skillDescription[skillIndex - CONSTNUM] + "\n";
            textDescription.text = $"<color=#FFFFFF>{description}</color>"; // 스킬 설명 글씨색 흰색으로 통일

            // 패시브 비급
            // mijiTypeImages[1]의 Sprite 설정
            Image[] levelImages = mijiLevels[index].GetComponentsInChildren<Image>();
            foreach (var img in levelImages)
            {
                img.sprite = mijiTypeImages[1];
            }

            mijiSkillBoundaries[index].GetComponent<Image>().color = HexToColor(miji_Boundary_Color[1]);
        }

        // 버튼 액티브 스킬 쿨타임 UI 갱신
        public void UpdateButtonActiveSkillCooldownUI(int index, float coolTime)
        {
            if (buttonActiveCooldownCoroutines[index] != null)
            {
                StopCoroutine(buttonActiveCooldownCoroutines[index]);
            }

            buttonActiveCooldownCoroutines[index] = 
                StartCoroutine(ShowButtonActiveCooldown(
                    buttonActiveSkillCooldownImages[index],
                    buttonActiveSkillCooldownTexts[index], coolTime
                    ));
        }


        // 액티브 스킬 쿨타임 UI 갱신
        public void UpdateActiveSkillCooldownUI(int index, float coolTime)
        {
            //StartCoroutine(ShowActiveCooldown(activeSkillCooldownImages[index], coolTime));

            if (activeSkillCooldownCoroutine != null)
            {
                StopCoroutine(activeSkillCooldownCoroutine);
            }

            activeSkillCooldownCoroutine = StartCoroutine(ShowActiveCooldown(activeSkillCooldownImages[index], coolTime));
        }

        // 버튼 액티브 스킬 쿨타임 처리
        private IEnumerator ShowButtonActiveCooldown(ButtonActiveSkillImage cooldownImage, MijiCoolTimeText cooldownText, float coolTime)
        {
            cooldownImage.Show();
            //cooldownText.Hide(); // 처음에는 쿨타임 텍스트를 숨김

            float elapsed = 0f;
            while (elapsed < coolTime)
            {
                elapsed += Time.deltaTime;
                float fillAmount = Mathf.Clamp01(elapsed / coolTime);
                cooldownImage.SetFillAmount(fillAmount);

                // 남은 시간이 5초 이하일 때만 쿨타임 텍스트를 표시
                float remainingTime = coolTime - elapsed;
                if (remainingTime <= 5f)
                {
                    cooldownText.Show();
                    int remainingTimeInt = Mathf.CeilToInt(remainingTime);
                    cooldownText.SetText(remainingTimeInt > 0 ? remainingTimeInt.ToString() : string.Empty);
                }

                yield return null;
            }

            cooldownText.Hide();
        }

        // 액티브 스킬 쿨타임 처리
        private IEnumerator ShowActiveCooldown(ActiveSkillImage cooldownImage, float coolTime)
        {
            cooldownImage.Show();

            float elapsed = 0f;
            while (elapsed < coolTime)
            {
                elapsed += Time.deltaTime;
                float fillAmount = Mathf.Clamp01(elapsed / coolTime);
                cooldownImage.SetFillAmount(fillAmount);

                yield return null;
            }
        }

        // 코루틴 중단 메서드
        public void StopAllCooldownCoroutines()
        {
            // 모든 버튼 액티브 스킬 코루틴 중단
            for (int i = 0; i < buttonActiveCooldownCoroutines.Length; i++)
            {
                if (buttonActiveCooldownCoroutines[i] != null)
                {
                    StopCoroutine(buttonActiveCooldownCoroutines[i]);
                    buttonActiveCooldownCoroutines[i] = null;
                }
            }

            // 액티브 스킬 코루틴 중단
            if (activeSkillCooldownCoroutine != null)
            {
                StopCoroutine(activeSkillCooldownCoroutine);
                activeSkillCooldownCoroutine = null;
            }

            // 흑야은신 남은 시간 코루틴 중단
            if (stealthRemainTimeCoroutine != null)
            {
                StopCoroutine(stealthRemainTimeCoroutine);
                stealthRemainTimeCoroutine = null;
            }
        }
 
        public void ContinueButtonActiveSkillCooldownUI(int index, float originalCoolTime, float remainingCoolTime)
        {
            // Index가 배열 범위를 벗어나지 않도록 검사
            if (index < 0 || index >= buttonActiveCooldownCoroutines.Length)
            {
                return; // 오류가 발생하지 않도록 함수 종료
            }

            // 기존 코루틴이 있다면 중단
            if (buttonActiveCooldownCoroutines[index] != null)
            {
                StopCoroutine(buttonActiveCooldownCoroutines[index]);
            }

            // 코루틴 시작
            buttonActiveCooldownCoroutines[index] = StartCoroutine(ShowButtonActiveCooldown(
                buttonActiveSkillCooldownImages[index],
                buttonActiveSkillCooldownTexts[index],
                originalCoolTime,
                remainingCoolTime));
        }

        // 저장된 쿨타임으로부터 액티브 스킬 쿨타임 이어서 감소
        public void ContinueActiveSkillCooldownUI(int index, float originalCoolTime, float remainingCoolTime)
        {
            if (activeSkillCooldownCoroutine != null)
            {
                StopCoroutine(activeSkillCooldownCoroutine);
            }

            // index가 배열의 유효한 범위 내에 있는지 확인
            if (index >= 0 && index < activeSkillCooldownImages.Length)
            {
                activeSkillCooldownCoroutine = StartCoroutine(ShowActiveCooldown(activeSkillCooldownImages[index], originalCoolTime, remainingCoolTime));
            }
        }

        // 남은 시간 버튼 액티브 스킬 쿨타임 처리
        private IEnumerator ShowButtonActiveCooldown(ButtonActiveSkillImage cooldownImage, MijiCoolTimeText cooldownText, float originalCoolTime, float remainingCoolTime)
        {
            cooldownImage.Show();
            float elapsed = originalCoolTime - remainingCoolTime;
            while (elapsed < originalCoolTime)
            {
                elapsed += Time.deltaTime;
                float fillAmount = Mathf.Clamp01(elapsed / originalCoolTime);
                cooldownImage.SetFillAmount(fillAmount);

                // 남은 시간이 5초 이하일 때만 쿨타임 텍스트를 표시
                float remainingTime = originalCoolTime - elapsed;
                if (remainingTime <= 5f)
                {
                    cooldownText.Show();
                    int remainingTimeInt = Mathf.CeilToInt(remainingTime);
                    cooldownText.SetText(remainingTimeInt > 0 ? remainingTimeInt.ToString() : string.Empty);
                }

                yield return null;
            }

            cooldownText.Hide();
        }

        // 남은 시간 액티브 스킬 쿨타임 처리
        private IEnumerator ShowActiveCooldown(ActiveSkillImage cooldownImage, float originalCoolTime, float remainingCoolTime)
        {
            cooldownImage.Show();
            float elapsed = originalCoolTime - remainingCoolTime;
            while (elapsed < originalCoolTime)
            {
                elapsed += Time.deltaTime;
                float fillAmount = Mathf.Clamp01(elapsed / originalCoolTime);
                cooldownImage.SetFillAmount(fillAmount);

                yield return null;
            }
        }

    }
}