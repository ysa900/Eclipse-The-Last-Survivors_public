namespace Eclipse.Game
{
    public class Touch_Of_Death : RandomSkill
    {
        protected override void Update()
        {
            bool destroySkill = aliveTimer > aliveTime;
    
            if (destroySkill)
            {
                if (onSkillFinished != null)
                    onSkillFinished(skillIndex);
    
                PoolManager.instance.ReturnSkill(this, returnIndex);
            }
    
            base.Update();
        }
    }
}