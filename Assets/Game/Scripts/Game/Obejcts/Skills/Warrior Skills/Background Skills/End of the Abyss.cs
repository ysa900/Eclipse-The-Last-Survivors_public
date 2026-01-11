using UnityEngine;

namespace Eclipse.Game
{
    public class EndoftheAbyss : BackGroundSkill
    {
        FollowCam followCam;
        BoxCollider2D boxColider2D;
    
        protected override void Awake()
        {
            base.Awake();
    
            boxColider2D = GetComponent<BoxCollider2D>();
            animator = GetComponent<Animator>();
        }
    
        public override void Init()
        {
            boxColider2D.enabled = false;   
            base.Init();
        }
    
        protected void Start()
        {
            followCam = FollowCam.instance;
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
                AttachCamera();
            }
    
            boxColider2D.enabled = true;
    
            animator.SetTrigger("Attack");
            base.Update();
        }

        // Animation Event로 호출
        void ChangeAngle()
        {
            float randomAngle = UnityEngine.Random.Range(0f, -120f);

            transform.rotation = Quaternion.Euler(0f, 0f, randomAngle);
        }

        void AttachCamera()
        {
            X = followCam.transform.position.x;
            Y = followCam.transform.position.y;
        }
    }
}