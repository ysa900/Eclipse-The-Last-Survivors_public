using UnityEngine;
using static Eclipse.Game.DamageHandler;

namespace Eclipse.Game
{
    public class SpiritOfTheWild : RandomSkill
    {
        float frameAliveTime = 29f * AnimationConstants.FrameTime;

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
        }

        public void SetAnimatorSpeed()
        {
            float minSpeedThreshold = 1f;

            // 반복 횟수 계산 (최소 1회 보장)
            // speed = (repeatCount * frameAliveTime) / aliveTime ≥ minSpeedThreshold 이기 때문에
            // repeatCount = aliveTime * minSpeedThreshold / frameAliveTime
            int repeatCount = Mathf.Max(1, Mathf.CeilToInt(aliveTime * minSpeedThreshold / frameAliveTime));

            // 반복 횟수에 맞게 speed 설정
            float speed = (repeatCount * frameAliveTime) / aliveTime;

            animator.speed = speed;
        }
    }
}