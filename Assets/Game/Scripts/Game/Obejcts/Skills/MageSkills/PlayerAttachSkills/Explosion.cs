namespace Eclipse.Game
{
    public class Explosion : PlayerAttachSkill
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
                X = PlayerManager.player.transform.position.x;
                Y = PlayerManager.player.transform.position.y;
            }
    
            base.Update();
        }
    }
}