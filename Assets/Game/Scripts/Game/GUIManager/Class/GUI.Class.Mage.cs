using Eclipse.Game.SkillSelect;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.Game
{
    public partial class GUIManager
    {
        // 공명 시 패널 표시
        public void OnDisplayResonancePanel(int resIndex1, int resIndex2, int resonanceSkillIndex, bool isDotDamageSkill)
        {
            rerollButton.gameObject.SetActive(false);

            skillSelectPageViewer.Show();
            string color1 = (resIndex1 % 3) switch
            {
                0 => "#F7570B",
                1 => "#B4E9F0",
                _ => "#2C82C9"
            };
            string color2 = (resIndex2 % 3) switch
            {
                0 => "#F7570B",
                1 => "#B4E9F0",
                _ => "#2C82C9"
            };
            title.GetComponentInChildren<TextMeshProUGUI>().text = $"<color={color1}>{"원소"}</color><color={color2}>{"공명"}</color>";

            // 스킬 패널 개수에 따른 UI 설정
            skillPanels[2].gameObject.SetActive(false);
            skillPanels[4].gameObject.SetActive(false);
            levels[2].gameObject.SetActive(false);

            skillPanels[1].gameObject.SetActive(true);
            levels[1].gameObject.SetActive(false);

            skillPanels[0].gameObject.SetActive(false);
            skillPanels[3].gameObject.SetActive(false);
            levels[0].gameObject.SetActive(false);

            SetResonanceSkillPanel(resonanceSkillIndex, isDotDamageSkill);
        }

        // 공명 패널 설정
        private void SetResonanceSkillPanel(int resonanceSkillIndex, bool isDotDamageSkill)
        {
            Image icon = skillIcons[1].GetComponent<Image>();
            icon.sprite = skillData.skillicon[resonanceSkillIndex];
            string color = (resonanceSkillIndex % 3) switch
            {
                0 => "#F7570B",
                1 => "#B4E9F0",
                _ => "#2C82C9"
            };
            TextMeshProUGUI textName = skillTextNames[1].GetComponent<TextMeshProUGUI>();
            textName.text = $"<color={color}>{skillData.skillName[resonanceSkillIndex]}</color>";
            TextMeshProUGUI textDescription = skillTextDescriptions[1].GetComponent<TextMeshProUGUI>();

            if (typeDescriptions[1].GetComponentInParent<TypeFrame>() == null)
            {
                typeDescriptions[1].transform.parent.gameObject.SetActive(true);
            }

            TextMeshProUGUI skillTypeText = typeDescriptions[1].GetComponent<TextMeshProUGUI>();
            skillTypeText.text = "공명\n"; // 유저 친화적인 '스킬 설명' 인터페이스 위한 카테고리 분류

            string description = "";

            description += skillData.skillDescription[resonanceSkillIndex] + "\n";

            // 공명 스킬 설명 빼는걸로
           /* if (isDotDamageSkill)
                description += "도트 데미지: ";
            else description += "데미지: ";

            if (resonanceSkillIndex == 12) description += 240;
            else description += skillData.damage[resonanceSkillIndex];

            float delay = skillData.delay[resonanceSkillIndex];
            description += $"\n쿨타임: {delay:F2}초";*/

            //textDescription.text = $"<color={color}>{description}</color>";
            textDescription.text = $"<color={skillDescriptionColor}>{description}</color>"; // 스킬 설명 글씨색 흰색으로 통일
            //SetLevelObjectAlpha(1, 5, true);

            skillBoundaries[1].gameObject.SetActive(true);
            skillBoundaries[1].GetComponent<Image>().color = HexToColor(boundary_Color[4]);

            // 스킬 패널 크기 증가
            skillPanels[1].transform.localScale = new Vector3(1.3f, 1.3f, 0);
            HoverEffect hover = skillPanels[1].GetComponent<HoverEffect>();
            hover.SetOriginalScale(skillPanels[1].transform.localScale);

            // 스킬 패널 위치 변경
            Vector2 vector2 = skillPanels[1].transform.localPosition;
            vector2.y /= 4f;
            skillPanels[1].transform.localPosition = vector2;
        }

        public void OnResonanceSkillSelect()
        {
            // 스킬 패널 크기 감소
            skillPanels[1].transform.localScale = new Vector3(1, 1, 0);
            HoverEffect hover = skillPanels[1].GetComponent<HoverEffect>();
            hover.SetOriginalScale(skillPanels[1].transform.localScale);

            // 스킬 패널 위치 변경
            Vector2 vector2 = skillPanels[1].transform.localPosition;
            vector2.y *= 4f;
            skillPanels[1].transform.localPosition = vector2;

            skillSelectPageViewer.Hide();
        }
    }
}

