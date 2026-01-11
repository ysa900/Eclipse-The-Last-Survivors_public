using UnityEngine;
using static Eclipse.Game.DamageHandler;

namespace Eclipse.Game
{
    public class Dagger : CursorTrackingSkill
    {
        private void Start()
        {
            speed = 5f;
        }

        public override void Init()
        {
            base.Init();

            endPosition = new Vector2(mousePosition.x, mousePosition.y);
            CalculateDirection();
        }
    
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
                MoveToCursor();
            }

            base.Update();
        }
    
        private void CalculateDirection()
        {
            direction = (endPosition - startPosition).normalized;
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