using System.Collections;
using UnityEngine;

namespace Eclipse.Game
{
    public class Holy_Shield : PlayerAttachSkill
    {
        public delegate void OnHolyShieldSkillDestroyed();
        public OnHolyShieldSkillDestroyed onHolyShieldSkillDestroyed;
    
        protected override void Update()
        {
            bool destroySkill = aliveTimer > aliveTime;
    
            StartCoroutine(ShieldAnimOff());
    
            if (destroySkill)
            {
                if (onSkillFinished != null)
                    onSkillFinished(skillIndex); // skillManager���� delegate�� �˷���
    
                onHolyShieldSkillDestroyed();
    
                PoolManager.instance.ReturnSkill(this, returnIndex);
                transform.GetChild(0).gameObject.SetActive(true);
                return;
            }
            else
            {
                X = PlayerManager.player.transform.position.x;
                Y = PlayerManager.player.transform.position.y + 0.2f;
            }
            base.Update();
        }
    
        IEnumerator ShieldAnimOff()
        {
            float animFrameTime = 26f * AnimationConstants.FrameTime;
            yield return new WaitForSeconds(animFrameTime);
            transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}