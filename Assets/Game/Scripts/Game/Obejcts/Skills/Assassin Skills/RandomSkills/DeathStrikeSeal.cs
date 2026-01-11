using UnityEngine;
using static Eclipse.Game.DamageHandler;

namespace Eclipse.Game
{
    public class DeathStrikeSeal : RandomSkill
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

            // LayerMask가 "Enemy", "Boss"인 경우
            if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy")
                || collision.gameObject.layer == LayerMask.NameToLayer("Boss"))
            {
                collision.gameObject.GetComponent<IDebuffable>().MakeBlind(aliveTime);
            }
        }
    }
}