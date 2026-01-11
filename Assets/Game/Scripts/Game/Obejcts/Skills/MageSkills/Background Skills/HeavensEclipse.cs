using UnityEngine;

namespace Eclipse.Game
{
    public class HeavensEclipse : Skill
    {
        FollowCam followCam;
    
        float BurstTime = 18f * AnimationConstants.FrameTime;
        float BurstTimer = 0;
    
        public float burstDamage;
    
        SpriteRenderer eclipseSky_Renderer;
        Color eclipseSky_color;
    
        [SerializeField] float alphaSpeed;
    
        public override void Init()
        {
            BurstTimer = 0;
            eclipseSky_color.a = 0f;
            eclipseSky_Renderer.color = eclipseSky_color;
    
            base.Init();
        }
    
        protected override void Awake()
        {
            base.Awake();
            eclipseSky_Renderer = transform.Find("EclipseSky").GetComponent<SpriteRenderer>();
        }
        
        protected void Start()
        {
            followCam = FollowCam.instance;
            eclipseSky_color = eclipseSky_Renderer.color;
        }
    
        protected override void Update()
        {
            bool destroySkill = aliveTimer > aliveTime;
    
            if (destroySkill)
            {
                if (onSkillFinished != null)
                    onSkillFinished(skillIndex);
    
                PoolManager.instance.ReturnSkill(this, returnIndex);
    
                return;
            }
            else
            {
                AttachCamera();
    
                eclipseSky_color.a = Mathf.Lerp(eclipseSky_color.a, 0.5f, Time.fixedDeltaTime * alphaSpeed);
                eclipseSky_Renderer.color = eclipseSky_color;
            }
    
            bool isBurstTimeNow = BurstTimer > BurstTime;
            if (isBurstTimeNow)
            {
                damage = burstDamage;
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
    }
}