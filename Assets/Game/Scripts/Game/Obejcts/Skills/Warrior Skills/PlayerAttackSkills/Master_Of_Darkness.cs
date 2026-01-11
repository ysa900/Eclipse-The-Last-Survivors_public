namespace Eclipse.Game
{
    public class Master_Of_Darkness : RandomSkill
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