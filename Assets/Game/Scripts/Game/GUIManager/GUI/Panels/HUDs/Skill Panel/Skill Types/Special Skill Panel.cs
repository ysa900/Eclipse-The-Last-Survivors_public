using UnityEngine.UI;

namespace Eclipse.Game.Panels
{
    public class SpecialSkillPanel : SkillPanel
    {
        public void SetPanelSkillIcon(int skillIndex)
        {
            Image icon = panelSkillIcons[0].GetComponent<Image>();
            icon.sprite = _skillData.skillicon[skillIndex];
            gameObject.SetActive(true);
        }
    }
}