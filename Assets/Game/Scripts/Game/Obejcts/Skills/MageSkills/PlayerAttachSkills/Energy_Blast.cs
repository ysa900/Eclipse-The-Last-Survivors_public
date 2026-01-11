using System.Collections;
using UnityEngine;

namespace Eclipse.Game
{
    public class Energy_Blast : PlayerAttachSkill
    {
        private float delay = 5f * AnimationConstants.FrameTime;
        private float delayTimer = 0f;
    
        bool isCorrutineNow = false;
    
        public override void Init()
        {
            delayTimer = 0;
            isCorrutineNow = false;
    
            base.Init();
        }
    
        protected override void Update()
        {
            bool destroySkill = aliveTimer > aliveTime && !isCorrutineNow;
    
            if (destroySkill)
            {
                StartCoroutine(Disappear());
    
                return;
            }
            else
            {
                AttachPlayer();
            }

            if (delay <= delayTimer)
            {
                base.Update();
            }
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();

            delayTimer += Time.deltaTime;
        }

        private IEnumerator Disappear()
        {
            isCorrutineNow = true;
            delayTimer = -999f; // Finish때는 데미지 안들어가게 하기 위해서
            animator.SetTrigger("Finish");
    
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Energy Blast_finish"));
    
            StartCoroutine(Return());
        }
    
        private IEnumerator Return()
        {
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f);
    
            if (onSkillFinished != null)
                onSkillFinished(skillIndex); // skillManager에게 delegate로 알려줌
    
            PoolManager.instance.ReturnSkill(this, returnIndex);
        }
    }
}