using UnityEngine;

namespace Eclipse.Game
{
    public class Sky_Fall : RandomSkill
    {
        private float delay = 8f * AnimationConstants.FrameTime;
        private float delayTimer = 0f;
    
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
    }
}