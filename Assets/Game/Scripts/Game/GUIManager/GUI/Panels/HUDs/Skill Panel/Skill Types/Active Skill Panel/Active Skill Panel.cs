using System.Collections.Generic;

namespace Eclipse.Game.Panels
{
    public class ActiveSkillPanel : SkillPanel
    {
        //==================================================================
        // 오버라이드용 함수

        // 패널 복원 함수
        public virtual void RestorePanel(int currentSize, int targetSize) { }
        public virtual void DeactivatePanelSkills(int index1, int index2) { }
        public virtual void DeactivatePanelSkills(List<int> indexes) { }

        //==================================================================
    }
}