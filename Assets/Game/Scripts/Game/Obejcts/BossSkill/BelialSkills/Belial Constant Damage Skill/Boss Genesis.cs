using System.Collections;
using UnityEngine;

namespace Eclipse.Game
{
    public class Boss_Genesis : BossConstantDamageSkill, IPoolingObject
    {
        SpriteRenderer spriteRenderer_child1;
        SpriteRenderer spriteRenderer_child2;
        SpriteRenderer spriteRenderer_child3;
        SpriteRenderer spriteRenderer_child4;
    
        public override void Init()
        {
            base.Init();
    
            UnityEngine.Color col = spriteRenderer_child1.color;
            col.a = 1f;
            spriteRenderer_child1.color = col;
    
            col = spriteRenderer_child2.color;
            col.a = 1f;
            spriteRenderer_child2.color = col;
    
            col = spriteRenderer_child3.color;
            col.a = 1f;
            spriteRenderer_child3.color = col;
    
            col = spriteRenderer_child4.color;
            col.a = 1f;
            spriteRenderer_child4.color = col;
        }
    
        protected override void Awake()
        {
            base.Awake();

            spriteRenderer_child1 = GetComponentsInChildren<SpriteRenderer>()[0];
            spriteRenderer_child2 = GetComponentsInChildren<SpriteRenderer>()[1];
            spriteRenderer_child3 = GetComponentsInChildren<SpriteRenderer>()[2];
            spriteRenderer_child4 = GetComponentsInChildren<SpriteRenderer>()[3];

            aliveTime = 3f;
            safeTime = 1f;
        }

        protected override IEnumerator Disappear()
        {
            animator.SetTrigger("Exit");
    
            isDisappearCoroutineNow = true;
    
            for(float i = 0.99f; i > 0;)
            {
                UnityEngine.Color col = spriteRenderer_child1.color;
                col.a = i;
                spriteRenderer_child1.color = col;
    
                col = spriteRenderer_child2.color;
                col.a = i;
                spriteRenderer_child2.color = col;
    
                col = spriteRenderer_child3.color;
                col.a = i;
                spriteRenderer_child3.color = col;
    
                col = spriteRenderer_child4.color;
                col.a = i;
                spriteRenderer_child4.color = col;
    
                i -= 0.01f;
    
            }
    
            yield return new WaitForSeconds(0.2f); // 지정한 초 만큼 쉬기
    
            PoolManager.instance.ReturnBossSkill(this, index);
    
            isDisappearCoroutineNow = false;
        }
    }
}