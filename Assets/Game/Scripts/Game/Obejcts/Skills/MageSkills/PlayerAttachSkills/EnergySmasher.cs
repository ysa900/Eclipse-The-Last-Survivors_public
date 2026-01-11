using System.Collections;
using UnityEngine;

namespace Eclipse.Game
{    
    public class EnergySmasher : PlayerAttachSkill
    {
        private float delay = 12f * AnimationConstants.FrameTime / 1.25f; // 12프레임 * 1.25배속
        private float delayTimer = 0f;

        bool isCorrutineNow = false;

        Collider2D col;

        protected override void Awake()
        {
            base.Awake();
            col = GetComponent<Collider2D>();
        }

        public override void Init()
        {
            delayTimer = 0;
            col.enabled = true;
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
            else if (delayTimer < delay)
            {
                AttachPlayer();
            }
            else
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
            col.enabled = false; // Finish때는 데미지 안들어가게 하기 위해서
            animator.SetTrigger("Finish");

            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Energy Smasher_Finish"));

            StartCoroutine(Return());
        }

        private IEnumerator Return()
        {
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.99f);
            
            if (onSkillFinished != null)
                onSkillFinished(skillIndex); // skillManager에게 delegate로 알려줌

            PoolManager.instance.ReturnSkill(this, returnIndex);
        }
    }
}