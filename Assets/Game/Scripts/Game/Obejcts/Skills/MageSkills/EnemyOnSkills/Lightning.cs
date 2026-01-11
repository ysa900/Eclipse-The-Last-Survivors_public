namespace Eclipse.Game
{
    public class Lightning : EnemyOnSkill
    {
        protected override void Update()
        {
            bool destroySkill;
    
            if (!isBossAppear) { destroySkill = aliveTimer > aliveTime; }
            else { destroySkill = aliveTimer > aliveTime; }
    
            if (destroySkill)
            {
                if (onSkillFinished != null)
                    onSkillFinished(skillIndex); 
    
                PoolManager.instance.ReturnSkill(this, returnIndex);
    
                return;
            }
    
            base.Update();
        }
    
    }
}