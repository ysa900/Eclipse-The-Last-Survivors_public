using static Eclipse.Game.DamageHandler;
using UnityEngine;

namespace Eclipse.Game
{
    public class AcidSwamp : CursorTrackingSkill
    {
        protected override void Update()
        {
            bool destroySkill = aliveTimer > aliveTime;
    
            if (destroySkill)
            {
                PoolManager.instance.ReturnSkill(this, returnIndex);
                return;
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