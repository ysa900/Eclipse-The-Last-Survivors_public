using System;

namespace Eclipse.Game
{
    public interface IDebuffable
    {
        void MakeBlind(float aliveTime);
        void MakeBind(float slowTime);

        void ApplyLegendaryDebuff(float damage, float debuffTime, Action onSkillAttack, int? level = null);
    }
}