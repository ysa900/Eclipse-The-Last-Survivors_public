using UnityEngine;
using static Eclipse.Game.DamageHandler;

namespace Eclipse.Game
{
    public class DebuffFloor : Skill
    {
        private bool isDamagePeriod = false;

        protected override void Update()
        {
            bool destroySkill = aliveTimer > aliveTime;

            // 스킬이 생성된 후 0.15초 이내에 데미지 적용
            // 스킬이 종료되기 전 0.15초부터 데미지 적용
            if (aliveTimer > 0.15f && aliveTimer < aliveTime - 0.15f)
            {
                isDamagePeriod = false;
            }
            else
            {
                isDamagePeriod = true;
            }

            if (destroySkill)
            {
                PoolManager.instance.ReturnSkill(this, returnIndex);
                return;
            }

            base.Update();
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (!isDamagePeriod) return; // 데미지 적용 기간이 아니면 리턴

            criticalChance = 0;
            criticalMultiplier = 1;

            base.OnTriggerEnter2D(other); // 데미지 적용, 크리티컬 데미지는 안들어가게..
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
                collision.gameObject.GetComponent<IDebuffable>().MakeBind(aliveTime - aliveTimer);
            }
        }
    }
}
