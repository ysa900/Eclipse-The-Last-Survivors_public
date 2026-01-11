using System.Collections;
using UnityEngine;

namespace Eclipse.Game
{
    public class DarkBond : PlayerAttachSkill
    {
        bool isCorrutineNow = false;
        float finishFrameTime = 5f * AnimationConstants.FrameTime;

        public override void Init()
        {
            isCorrutineNow = false;

            base.Init();
        }

        protected override void Update()
        {
            bool destroySkill = aliveTimer > (aliveTime - finishFrameTime) && !isCorrutineNow;

            if (destroySkill)
            {
                StartCoroutine(Disappear());

                return;
            }
            else
            {
                X = PlayerManager.player.transform.position.x;
                Y = PlayerManager.player.transform.position.y;
            }
    
            base.Update();
        }

        private IEnumerator Disappear()
        {
            isCorrutineNow = true;
            animator.SetTrigger("Finish");

            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Dark_Common_Finish"));

            StartCoroutine(Return());
        }

        private IEnumerator Return()
        {
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);

            if (onSkillFinished != null)
                onSkillFinished(skillIndex);

            PoolManager.instance.ReturnSkill(this, returnIndex);
        }
    }
}