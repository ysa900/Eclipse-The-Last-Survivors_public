using UnityEngine;
using static Eclipse.Game.DamageHandler;

namespace Eclipse.Game
{
    public class ExplosionEnsemble_Main : Skill
    {
        protected override void Update()
        {
            bool isSkillFinish = aliveTimer >= aliveTime;
    
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Explosion_1"))
            {
                if (isSkillFinish)
                {
                    animator.SetTrigger("Fin");
                }
            }
            else
            { 
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                {
                    if (onSkillFinished != null)
                        onSkillFinished(skillIndex); // skillManager에게 delegate로 알려줌
    
                    PoolManager.instance.ReturnSkill(this, returnIndex);
                }
            }
    
            base.Update();
        }

        protected override void OnAfterDamageApplied(Collider2D collision, DamageResult damageResult)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Boss"))
            {
                var boss = collision.GetComponent<Boss>();
                if (boss != null)
                {
                    boss.compressedDamage += damageResult.FinalDamage;
                }
            }
        }
    }
}