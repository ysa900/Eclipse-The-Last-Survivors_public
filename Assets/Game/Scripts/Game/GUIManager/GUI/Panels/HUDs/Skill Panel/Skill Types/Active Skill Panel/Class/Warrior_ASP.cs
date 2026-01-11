using System.Collections.Generic;
using UnityEngine;

namespace Eclipse.Game.Panels
{
    public class Warrior_ASP : ActiveSkillPanel
    {
/*
        //==================================================================
        // 전사 각성 시 필요한 변수들

        private List<int> removedIndexes = new List<int>();
        private List<PanelImage> removedPanelSkillIcons = new List<PanelImage>();
        private List<PanelText> removedPanelSkillLevelTexts = new List<PanelText>();
        
        //==================================================================
        // 각성 스킬 패널 복원
        public override void RestorePanel(int currentSize, int targetSize)
        {
            RectTransform skillPanelRect = gameObject.GetComponent<RectTransform>();
            skillPanelRect.sizeDelta = new Vector2(skillPanelRect.sizeDelta.x / currentSize * targetSize, skillPanelRect.sizeDelta.y);

            for (int i = 0; i < removedIndexes.Count; i++)
            {
                panelSkillIcons.Insert(removedIndexes[i], removedPanelSkillIcons[i]);
                panelSkillLevelTexts.Insert(removedIndexes[i], removedPanelSkillLevelTexts[i]);
            }

            removedIndexes.Clear();
            removedPanelSkillIcons.Clear();
            removedPanelSkillLevelTexts.Clear();
        }

        // 각성 시 패널 스킬 비활성화 처리
        public override void DeactivatePanelSkills(List<int> indexes)
        {
            for (int i = 0; i < indexes.Count; i++)
            {
                panelSkillIcons[indexes[i]].gameObject.SetActive(false);

                removedIndexes.Add(indexes[i]);
                removedPanelSkillIcons.Add(panelSkillIcons[indexes[i]]);
                removedPanelSkillLevelTexts.Add(panelSkillLevelTexts[indexes[i]]);

                panelSkillIcons.RemoveAt(indexes[i]);
                panelSkillLevelTexts.RemoveAt(indexes[i]);
            }
        }*/
    }
}

