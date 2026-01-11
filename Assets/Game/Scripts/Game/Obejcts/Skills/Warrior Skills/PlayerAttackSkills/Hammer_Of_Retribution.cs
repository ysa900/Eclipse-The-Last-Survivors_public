using UnityEngine;

namespace Eclipse.Game
{
    public class Hammer_Of_Retribution : PlayerAttachSkill
    {
        CapsuleCollider2D capsuleCollider;

        protected override void Awake()
        {
            base.Awake();
            capsuleCollider = GetComponent<CapsuleCollider2D>();

        }

        protected override void Update()
        {
            bool destroySkill = false;

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Holy_Common_Start"))
            {
                capsuleCollider.enabled = false; // 콜라이더 끄기

                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
                {
                    capsuleCollider.enabled = true; // 내려칠 때 콜라이더 켜기

                    AudioManager.instance.PlaySfx(AudioManager.Sfx.HolyHammer); // 내려칠 때 SFX 켜기
                    animator.SetTrigger("Finish");
                }
            }
            else if (animator.GetCurrentAnimatorStateInfo(0).IsName("Holy_Common_Finish"))
            {
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f)
                {
                    destroySkill = true;
                }
            }

            if (destroySkill)
            {
                if (onSkillFinished != null)
                    onSkillFinished(skillIndex); // skillManager     delegate    ˷   

                PoolManager.instance.ReturnSkill(this, returnIndex);
                return;
            }
            else
            {
                AttachPlayer();
            }

            base.Update();
        }
    }
}