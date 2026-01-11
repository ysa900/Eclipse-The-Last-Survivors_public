using UnityEngine;
using static Eclipse.Game.DamageHandler;

namespace Eclipse.Game
{
    public class ArcaneHeart_Main : RandomSkill
    {
        private CapsuleCollider2D capsuleCollider2D;

        protected override void Awake()
        {
            base.Awake();
            capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        }

        protected override void Update()
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("ArcaneHeart_Main_Start"))
            {
                capsuleCollider2D.enabled = false; // 처음 시전 시에는 Collider 끄기
            }
            else
            {
                capsuleCollider2D.enabled = true;

                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
                {
                    if (onSkillFinished != null)
                        onSkillFinished(skillIndex); // skillManager에게 delegate로 알려줌

                    PoolManager.instance.ReturnSkill(this, returnIndex);
                    return;
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