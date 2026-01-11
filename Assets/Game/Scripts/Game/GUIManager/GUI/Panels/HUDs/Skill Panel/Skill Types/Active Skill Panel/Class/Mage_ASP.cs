using UnityEngine;

namespace Eclipse.Game.Panels
{
    public class Mage_ASP : ActiveSkillPanel
    {/*
        //==================================================================
        // 마법사 공명 시 필요한 변수들
        private int removedIndex; // 선택된 스킬 패널에서 제거된 스킬의 배열 index
        private PanelImage removedPanelSkillIcon;
        private PanelText removedPanelSkillLevelText;


        //==================================================================
        // 마법사 공명 시 필요한 함수들

        // 공명 시 패널 스킬 비활성화 처리
        public override void DeactivatePanelSkills(int listResIndex1, int listResIndex2)
        {
            panelSkillIcons[listResIndex1].gameObject.SetActive(false);
            panelSkillIcons[listResIndex2].gameObject.SetActive(false);

            removedIndex = listResIndex2;
            removedPanelSkillIcon = panelSkillIcons[listResIndex2];
            removedPanelSkillLevelText = panelSkillLevelTexts[listResIndex2];

            panelSkillIcons.RemoveAt(listResIndex2);
            panelSkillLevelTexts.RemoveAt(listResIndex2);
        }

        // 공명 스킬 패널 복원
        public override void RestorePanel(int currentSize, int targetSize)
        {
            RectTransform skillPanelRect = gameObject.GetComponent<RectTransform>();
            skillPanelRect.sizeDelta = new Vector2(skillPanelRect.sizeDelta.x / currentSize * targetSize, skillPanelRect.sizeDelta.y);

            InsertToList(removedIndex, removedPanelSkillIcon);
            InsertToList(removedIndex, removedPanelSkillLevelText);
        }
*/
    }
}

