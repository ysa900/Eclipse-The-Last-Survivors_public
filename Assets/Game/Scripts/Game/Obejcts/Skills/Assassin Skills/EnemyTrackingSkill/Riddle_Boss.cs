using UnityEngine;
using static Eclipse.Game.DamageHandler;

namespace Eclipse.Game
{
    public class Riddle_Boss : EnemyTrackingSkill
    {
        public override void Init()
        {
            base.Init();
    
            GetComponent<Riddle_Normal>().enabled = false;
        }
    
        protected override void Update()
        {
            bool destroySkill = aliveTimer > aliveTime;
    
            if (destroySkill)
            {
                if (onSkillFinished != null)
                    onSkillFinished(skillIndex); // skillManager에게 delegate로 알려줌
    
                GetComponent<Riddle_Normal>().enabled = true;
                PoolManager.instance.ReturnSkill(this, skillIndex);
                return;
            }
            else
            {
                MoveToEnemy();
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