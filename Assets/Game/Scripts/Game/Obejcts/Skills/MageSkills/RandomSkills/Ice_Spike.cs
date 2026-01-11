using System.Collections;
using UnityEngine;

namespace Eclipse.Game
{
    public class Ice_Spike : RandomSkill
    {
        bool isCoroutineNow; // ���� �ڷ�ƾ�� �����ϰ� �ִ����� üũ�� ����
    
        Animator animator_ground;
    
        public override void Init()
        {
            base.Init();
            animator_ground = transform.Find("ground").GetComponent<Animator>();
            
        }
    
    
        // Update is called once per frame
        protected override void Update()
        {
            bool destroySkill = aliveTimer > aliveTime;
    
            if (destroySkill && !isCoroutineNow)
            {
                StartCoroutine(Disappear());
            }
    
            base.Update();
        }
    
        IEnumerator Disappear()
        {
            animator.SetTrigger("Finish");
            animator_ground.SetTrigger("Finish");
    
            isCoroutineNow = true;
    
            yield return new WaitForSeconds(0.2f); // ������ �� ��ŭ ����
    
            if (onSkillFinished != null)
                onSkillFinished(skillIndex); // skillManager���� delegate�� �˷���
    
            PoolManager.instance.ReturnSkill(this, returnIndex);
    
            isCoroutineNow = false;
        }
    }
}