using Eclipse.Game.SkillSelect;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.Game
{
    public partial class GUIManager
    {
        public void OnDisplayUltimatePanel(int ultimateSkillIndex)
        {
            rerollButton.gameObject.SetActive(false);

            //onSkillSelectObjectDisplayed?.Invoke();
            skillSelectPageViewer.Show();

            string color = (ultimateSkillIndex) switch
            {
                0 => "#FFF2A6",
                1 => "#D82626",
                _ => "#1F1F1F"
            };

            title.GetComponentInChildren<TextMeshProUGUI>().text = $"<color={color}>{"각성"}</color>";

            // 스킬 패널 개수에 따른 UI 설정
            skillPanels[2].gameObject.SetActive(false);
            skillPanels[4].gameObject.SetActive(false);
            levels[2].gameObject.SetActive(false);

            skillPanels[1].gameObject.SetActive(true);
            levels[1].gameObject.SetActive(false);

            skillPanels[0].gameObject.SetActive(false);
            skillPanels[3].gameObject.SetActive(false);
            levels[0].gameObject.SetActive(false);

            SetUltimatedSkillPanel(ultimateSkillIndex);
        }

        private void SetUltimatedSkillPanel(int ultimateSkillIndex)
        {
            Image icon = skillIcons[1].GetComponent<Image>();
            icon.sprite = skillData.skillicon[ultimateSkillIndex];

            string color = (ultimateSkillIndex) switch
            {
                0 => "#FFF2A6",
                1 => "#D82626",
                _ => "#1F1F1F"
            };

            TextMeshProUGUI textName = skillTextNames[1].GetComponent<TextMeshProUGUI>();
            textName.text = $"<color={color}>{skillData.skillName[ultimateSkillIndex]}</color>";
            TextMeshProUGUI textDescription = skillTextDescriptions[1].GetComponent<TextMeshProUGUI>();

            if (typeDescriptions[1].GetComponentInParent<TypeFrame>() == null)
            {
                typeDescriptions[1].transform.parent.gameObject.SetActive(true);
            }

            TextMeshProUGUI skillTypeText = typeDescriptions[1].GetComponent<TextMeshProUGUI>();
            skillTypeText.text = "각성\n"; // 유저 친화적인 '스킬 설명' 인터페이스 위한 카테고리 분류

            string description = "";

            description += skillData.skillDescription[ultimateSkillIndex] + "\n";

            textDescription.text = $"<color={skillDescriptionColor}>{description}</color>"; // 스킬 설명 글씨색 흰색으로 통일

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

        public void OnUltimateSkillSelect()
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