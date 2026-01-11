namespace Eclipse.Game
{
    public class Judgement : RandomSkill
    {
        protected override void Update()
        {
            bool destroySkill = aliveTimer > aliveTime;
    
            if (destroySkill)
            {
                if (onSkillFinished != null)
                    onSkillFinished(skillIndex); // skillManager���� delegate�� �˷���
    
                PoolManager.instance.ReturnSkill(this, returnIndex);
    
                return;
            }
    
            base.Update();
        }
    }
}