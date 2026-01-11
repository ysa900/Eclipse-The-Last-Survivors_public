using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Eclipse.Game.Panels
{
    public class SkillPanel : MonoBehaviour
    {
        //==================================================================
        // 사용할 스킬 데이터
        protected SkillData2 _skillData;
        public SkillData2 SkillData
        {
            get { return _skillData; }
            set { _skillData = value; }
        }

        //==================================================================
        // 리스트 타입 검사를 위한 딕셔너리
        Dictionary<Type, IList> typeListMap = new Dictionary<Type, IList>();

        //==================================================================
        // 자식 오브젝트들
        [SerializeField][ReadOnly] protected List<PanelImage> panelSkillIcons = new List<PanelImage>();
        [SerializeField][ReadOnly] protected List<PanelText> panelSkillLevelTexts = new List<PanelText>();

        //==================================================================

        private void Awake()
        {
            panelSkillIcons = GetComponentsInChildren<PanelImage>().ToList();
            panelSkillLevelTexts = GetComponentsInChildren<PanelText>().ToList();
        }

        //==================================================================
        // 스킬 패널 리스트 관련 기본 함수들

        // 스킬 아이콘 설정
        public virtual void SetPanelSkillIcon(int listIndex, int skillIndex, SkillData2 skilldata)
        {
            if (skillIndex < 0)
            {
                panelSkillIcons[listIndex].gameObject.SetActive(false);
                return;
            }
            
            panelSkillIcons[listIndex].SetSprite(skilldata.skillicon[skillIndex]);
            panelSkillIcons[listIndex].gameObject.SetActive(true);
        }

        // 스킬 레벨 텍스트 설정
        public void SetPanelSkillLevelText(int listIndex, int skillIndex, SkillData2 skillData)
        {
            if (skillIndex < 0)
            {
                panelSkillLevelTexts[listIndex].gameObject.SetActive(false);
                return;
            }

            int level = skillData.level[skillIndex];
            panelSkillLevelTexts[listIndex].SetText($"Lv {level}");
            panelSkillLevelTexts[listIndex].gameObject.SetActive(true);
        }

        public void SetPanelSkillLevelText(int listIndex, string levelText, SkillData2 skillData)
        {
            panelSkillLevelTexts[listIndex].SetText(levelText);
        }

        // 딕셔너리를 이용해 리스트마다 함수를 만들지 않아도 됨
        public void InsertToList<T>(int listIndex, T item)
        {
            Type itemType = typeof(T);

            if (typeListMap.TryGetValue(itemType, out IList list))
            {
                list.Insert(listIndex, item);
            }
            else
            {
                Debug.LogWarning($"{itemType}에 해당하는 리스트가 존재하지 않습니다.");
            }
        }

        // 스킬 패널 크기 조절 함수
        public void ChangeSkillPanelSize(int currentSize ,int targetSize)
        {
            RectTransform skillPanelRect = gameObject.GetComponent<RectTransform>();
            skillPanelRect.sizeDelta = new Vector2(skillPanelRect.sizeDelta.x / currentSize * targetSize, skillPanelRect.sizeDelta.y);
        }

        //==================================================================
    }

}