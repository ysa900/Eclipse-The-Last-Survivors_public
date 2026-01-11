using TMPro;

namespace Eclipse.Game.Panels
{
    public class MijiActiveSkillPanel : MijiSkillPanel
    {
        // 스킬 이름 텍스트 설정
        public void SetPanelSkillLevelText(int listIndex, int skillIndex, MijiSkillData mijiSkilldata)
        {
            if (skillIndex < 0)
            {
                panelSkillLevelTexts[listIndex].gameObject.SetActive(false);
                return;
            }

            panelSkillLevelTexts[listIndex].SetText($"{mijiSkilldata.skillName[skillIndex]}");
            panelSkillLevelTexts[listIndex].gameObject.SetActive(true);
        }
    }
}
