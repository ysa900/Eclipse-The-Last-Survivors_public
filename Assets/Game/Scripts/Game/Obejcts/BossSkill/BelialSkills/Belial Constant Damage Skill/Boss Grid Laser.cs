using System.Collections;
using UnityEngine;

namespace Eclipse.Game
{
    public class Boss_Grid_Laser : BossConstantDamageSkill, IPoolingObject
    {
        public bool isLeftTop;
        private bool isLeftBottom;
    
        public override void Init()
        {
            base.Init();

            isLeftBottom = !isLeftTop;
    
            float angle = 0f;
            if (isLeftTop)
            {
                angle = 45f;
            }
            else if (isLeftBottom)
            {
                angle = -45f;
            }
    
            Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.forward);
            Quaternion rotation = Quaternion.Slerp(transform.rotation, angleAxis, 5f);
            transform.rotation = rotation;
        }

        protected override void Awake()
        {
            base.Awake();

            aliveTime = 4f; 
            safeTime = 1.6f;
        }
        
        protected override IEnumerator Disappear()
        {
            animator.SetTrigger("Exit");
    
            isDisappearCoroutineNow = true;
    
            yield return new WaitForSeconds(0.2f); // 지정한 초 만큼 쉬기

            PoolManager.instance.ReturnBossSkill(this, index);
        }
    }
}