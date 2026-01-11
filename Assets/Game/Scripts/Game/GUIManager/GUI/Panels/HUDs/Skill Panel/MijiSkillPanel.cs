using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.Game.Panels
{
    public class MijiSkillPanel : MonoBehaviour
    { 
        //==================================================================
        // 자식 오브젝트들
        [SerializeField] protected List<PanelImage> panelSkillIcons = new List<PanelImage>();
        [SerializeField] protected List<PanelText> panelSkillLevelTexts = new List<PanelText>();

        //==================================================================

        private void Awake()
        {
            panelSkillIcons = GetComponentsInChildren<PanelImage>().ToList();
            panelSkillLevelTexts = GetComponentsInChildren<PanelText>().ToList();
        }

        //==================================================================
        // 스킬 패널 리스트 관련 기본 함수들

        // 패널 스킬 아이콘 설정
        public void SetPanelSkillIcon(int listIndex, int skillIndex, MijiSkillData mijiSkilldata)
        {
            // 초기값 -1이므로 안보이게 세팅
            if (skillIndex < 0)
            {
                panelSkillIcons[listIndex].gameObject.SetActive(false);
                return;
            }

            panelSkillIcons[listIndex].SetSprite(mijiSkilldata.skillicon[skillIndex]);
            panelSkillIcons[listIndex].gameObject.SetActive(true);
        }

        // 패널 스킬 레벨 텍스트 설정
        public virtual void SetPanelSkillNameText(int listIndex, int skillIndex, MijiSkillData mijiSkilldata)
        {
            // Index 범위를 벗어나는 경우 예외 방지
            if (listIndex < 0 || listIndex >= panelSkillLevelTexts.Count)
            {
                Debug.LogWarning($"SetPanelSkillLevelText: listIndex {listIndex}가 범위를 벗어났습니다. 패널 텍스트 리스트 크기: {panelSkillLevelTexts.Count}");
                return;
            }

            if (skillIndex < 0)
            {
                panelSkillLevelTexts[listIndex].gameObject.SetActive(false);
                return;
            }

            TextMeshProUGUI text = panelSkillLevelTexts[listIndex].GetComponent<TextMeshProUGUI>();
            text.text = $"{mijiSkilldata.skillName[skillIndex]}";
            panelSkillLevelTexts[listIndex].gameObject.SetActive(true);
        }

    }
}