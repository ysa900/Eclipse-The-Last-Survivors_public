namespace Eclipse.Game
{
    public class Dark_Slash : PlayerAttachSkill
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
            else
            {
                AttachPlayer();
            }
    
            base.Update();
        }
    }
}