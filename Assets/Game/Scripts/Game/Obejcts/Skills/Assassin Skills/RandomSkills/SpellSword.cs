using System.Collections;
using System.Threading;
using UnityEngine;
using static Eclipse.Game.DamageHandler;

namespace Eclipse.Game
{
    public class SpellSword : RandomSkill
    {
        public float frameAliveTime_Start = 17f * AnimationConstants.FrameTime / 1.5f; // Start 애니메이션의 프레임 시간 (1.5배속)
        float frameAliveTime_Finish = 13f * AnimationConstants.FrameTime;

        public float debuffDamage;
        public float debuffTime;
        public int skillLevel;

        CapsuleCollider2D capsuleCollider2D;

        protected override void Awake()
        {
            base.Awake();
            capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        }

        public override void Init()
        {
            base.Init();

            StartCoroutine(SetAnimationProcess());
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

            base.Update();
        }

        private IEnumerator SetAnimationProcess()
        {
            animator.speed = 1;
            capsuleCollider2D.enabled = false;

            // 애니메이션이 시작 될 때까지 기다림
            yield return new WaitUntil(() =>
            {
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0); // 레이어 0 사용
                return stateInfo.IsName("SpellSword_Normal_Finish");
            });

            SetAnimatorSpeed();
            capsuleCollider2D.enabled = true;
        }

        private void SetAnimatorSpeed()
        {
            float minSpeedThreshold = 1f;
            float finishAliveTime = aliveTime - frameAliveTime_Start;

            // 반복 횟수 계산 (최소 1회 보장)
            // speed = (repeatCount * frameAliveTime) / finishAliveTime ≥ minSpeedThreshold 이기 때문에
            // repeatCount = finishAliveTime * minSpeedThreshold / frameAliveTime
            int repeatCount = Mathf.Max(1, Mathf.CeilToInt(finishAliveTime * minSpeedThreshold / frameAliveTime_Finish));

            // 반복 횟수에 맞게 speed 설정
            float speed = (repeatCount * frameAliveTime_Finish) / finishAliveTime;  
            animator.speed = speed;
        }

        // 해당 스킬 데미지는 딜 압축 포함 X - 딜 압축 시작하는 기폭 역할만..
        protected override void OnAfterDamageApplied(Collider2D collision, DamageResult damageResult)
        {
            // LayerMask가 "Enemy", "Boss"인 경우
            if (collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                collision.gameObject.GetComponent<IDebuffable>().ApplyLegendaryDebuff(debuffDamage, debuffTime, () => onSkillAttack(skillIndex, damageResult.FinalDamage));
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("Boss"))
            {
                collision.gameObject.GetComponent<IDebuffable>().ApplyLegendaryDebuff(debuffDamage, debuffTime, () => onSkillAttack(skillIndex, damageResult.FinalDamage), skillLevel);
            }
        }
    }
}