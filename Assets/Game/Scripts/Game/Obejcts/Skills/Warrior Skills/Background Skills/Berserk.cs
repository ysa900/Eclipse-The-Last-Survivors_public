using System.Collections;
using UnityEngine;

namespace Eclipse.Game
{
    public class Berserk : BackGroundSkill
    {
        FollowCam followCam;
        float BurstTime = 1.1f;
        float BurstTimer = 0;
        public float burstDamage;
    
        public override void Init()
        {
            BurstTimer = 0;
            base.Init();
        }
    
        protected void Start()
        {
            followCam = FollowCam.instance;
            animator = GetComponent<Animator>();
        }

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
                AttachCamera();
            }
    
            bool isBurstTimeNow = BurstTimer > BurstTime;
            if (isBurstTimeNow)
            {
                damage = burstDamage;
                dotDelayTime = 0.05f;
            }
    
            base.Update();
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();

            BurstTimer += Time.deltaTime;
        }

        void AttachCamera()
        {
            X = followCam.transform.position.x;
            Y = followCam.transform.position.y;
        }
    
        private IEnumerator Disappear()
        {
            animator.SetTrigger("Finish");
    
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Blood_Ultimated_finish"));
    
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