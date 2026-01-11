using System.Collections;
using UnityEngine;
using static Eclipse.Game.DamageHandler;

namespace Eclipse.Game
{
    public class SlashBlue : PlayerAttachSkill
    {
        protected override void Update()
        {
            bool destroySkill = aliveTimer > aliveTime;

            if (destroySkill)
            {
                StartCoroutine(Disappear());

                return;
            }
            else
            {
                AttachPlayer();
            }

            base.Update();
        }

        private IEnumerator Disappear()
        {
            animator.SetTrigger("Finish");

            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("SplashBlue_Middle"));

            StartCoroutine(Return());
        }
        private IEnumerator Return()
        {
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

            if (onSkillFinished != null)
                onSkillFinished(skillIndex);

            PoolManager.instance.ReturnSkill(this, skillIndex);
        }

        // 암기류 크확 : A_PassiveSkillData.Damage[0]

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