using System.Collections;
using UnityEngine;

namespace Eclipse.Game
{
    public class Blow_Of_Madness : PlayerAttachSkill
    {
        float frameAliveTime_Start = 12f * AnimationConstants.FrameTime / 1.25f;
        float frameAliveTime_Finish = 20f * AnimationConstants.FrameTime;

        CapsuleCollider2D capsuleCollider;
    
        protected override void Awake()
        {
            base.Awake();
    
            capsuleCollider = GetComponent<CapsuleCollider2D>();
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
                    onSkillFinished(skillIndex); // skillManager���� delegate�� �˷���

                PoolManager.instance.ReturnSkill(this, returnIndex);
                return;
            }
            else
            {
                AttachPlayer();
            }
    
            base.Update();
        }
    
        public override void AttachPlayer()
        {
            base.AttachPlayer();
            if (PlayerManager.player.isPlayerLookLeft)
            {
                X = PlayerManager.player.transform.position.x - xOffset;
                capsuleCollider.offset = new Vector2(-0.2f, -0.05f);
                transform.rotation = Quaternion.Euler(0, 0, 45);
            }
            else
            {
                X = PlayerManager.player.transform.position.x + xOffset;
                capsuleCollider.offset = new Vector2(0.2f, -0.05f);
                transform.rotation = Quaternion.Euler(0, 0, -45);
            }
            Y = PlayerManager.player.transform.position.y + yOffset;
    
    
            if (isFlipped)
            {
                spriteRenderer.flipX = !PlayerManager.player.isPlayerLookLeft;
             
            }
            else if (isYFlipped)
            {
                spriteRenderer.flipY = !PlayerManager.player.isPlayerLookLeft;
            }
            else
            {
                spriteRenderer.flipX = PlayerManager.player.isPlayerLookLeft;
        
            }
        }

        private IEnumerator SetAnimationProcess()
        {
            capsuleCollider.enabled = false;

            // 애니메이션이 시작 될 때까지 기다림
            yield return new WaitUntil(() =>
            {
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0); // 레이어 0 사용
                return stateInfo.IsName("Blood_Normal_Finish");
            });

            capsuleCollider.enabled = true;
        }

        public void SetAnimatorSpeed()
        {
            float frameAnimationTime = frameAliveTime_Start + frameAliveTime_Finish;

            // speed 설정
            float speed = frameAnimationTime / aliveTime;
            animator.speed = speed;
        }
    }
}