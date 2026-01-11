using System.Collections;
using UnityEngine;

namespace Eclipse.Game
{
    public class Inferno : EnemyOnSkill
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
    
            base.Update();
        }
    
        private IEnumerator Disappear()
        {
            isCorrutineNow = true;
            animator.SetTrigger("Finish");
    
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Inferno_finish"));
    
            StartCoroutine(Return());
        }
    
        private IEnumerator Return()
        {
            yield return new WaitUntil(()=> animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
    
            if (onSkillFinished != null)
                onSkillFinished(skillIndex);
    
            PoolManager.instance.ReturnSkill(this, returnIndex);
        }
    }
}