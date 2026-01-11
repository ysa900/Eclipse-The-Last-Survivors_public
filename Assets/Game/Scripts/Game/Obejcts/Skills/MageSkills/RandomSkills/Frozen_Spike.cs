using UnityEngine;

namespace Eclipse.Game
{
    public class Frozen_Spike : RandomSkill
    {
        private float delay = 7f * AnimationConstants.FrameTime;
        private float delayTimer = 0f;

        CapsuleCollider2D capsuleCollider2D;

        protected override void Awake()
        {
            base.Awake();
            capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        }

        public override void Init()
        {
            delayTimer = 0f;
    
            base.Init();
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

            if (delayTimer >= delay)
            {
                // 스킬이 발동된 후에 콜라이더를 활성화
                capsuleCollider2D.enabled = true;
            }
            else
            {
                // 스킬이 발동되기 전에는 콜라이더를 비활성화
                capsuleCollider2D.enabled = false;
            }
    
            base.Update();
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();

            delayTimer += Time.deltaTime;
        }

        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            if (delayTimer < delay) return;

            base.OnTriggerEnter2D(collision);
        }
    }
}