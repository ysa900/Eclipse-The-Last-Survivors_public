using System.Collections;
using UnityEngine;

namespace Eclipse.Game
{
    public class Water_Shot : PlayerAttachSkill
    {
        bool isCorrutineNow = false;
    
        public override void Init()
        {
            base.Init();
    
            isCorrutineNow = false;
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
    
            base.Update();
        }
    
        private IEnumerator Disappear()
        {
            isCorrutineNow = true;
            animator.SetTrigger("Finish");
    
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("WaterShot_finish"));
    
            StartCoroutine(Return());
        }
    
        private IEnumerator Return()
        {
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
    
            if (onSkillFinished != null)
                onSkillFinished(skillIndex); // skillManager���� delegate�� �˷���
    
            PoolManager.instance.ReturnSkill(this, returnIndex);
        }
    
    }
}