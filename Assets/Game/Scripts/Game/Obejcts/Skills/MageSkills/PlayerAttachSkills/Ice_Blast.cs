namespace Eclipse.Game
{
    public class Ice_Blast : PlayerAttachSkill
    {
        protected override void Update()
        {
            bool destroySkill = aliveTimer > aliveTime;
    
            if (destroySkill)
            {
                if (onSkillFinished != null)
                    onSkillFinished(skillIndex); // skillManager에게 delegate로 알려줌
    
                PoolManager.instance.ReturnSkill(this, returnIndex);
                return;
            }
            else
            {
                AttachPlayer();
            }
    
            base.Update();
        }
    }
}