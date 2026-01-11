using System.Collections;
using UnityEngine;

namespace Eclipse.Game
{
    public class PatternLaser : BossConstantDamageSkill, IPoolingObject
    {
    
        public override void Init()
        {
            base.Init();

            float rotate = UnityEngine.Random.Range(0f, 180f);
            Quaternion rotation = Quaternion.Euler(0f, 0f, rotate);
            transform.rotation = rotation;
        }

        protected override void Awake()
        {
            base.Awake();

            aliveTime = 4f;
            safeTime = 1.6f;
            damage = 1.5f;
        }

        private void Start()
        {
            float rotate = UnityEngine.Random.Range(0f, 180f);
            Quaternion rotation = Quaternion.Euler(0f, 0f, rotate);
            transform.rotation = rotation;
        }
    
        protected override IEnumerator Disappear()
        {
            animator.SetTrigger("Exit");
    
            isDisappearCoroutineNow = true;
    
            yield return new WaitForSeconds(0.2f); // 지정한 초 만큼 쉬기

            Destroy(gameObject);
        }
    }
}