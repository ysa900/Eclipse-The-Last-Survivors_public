namespace Eclipse.Game
{
    public class Thirst_For_Blood : EnemyOnSkill
    {
        protected override void Update()
        {
            bool destroySkill = false;
    
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("obliterate"))
            {
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                {
                    destroySkill = true;
                }
            }

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