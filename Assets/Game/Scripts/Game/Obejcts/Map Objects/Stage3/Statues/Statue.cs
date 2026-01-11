using System;
using UnityEngine;

namespace Eclipse.Game
{
    public class Statue : Enemy
    {
        // StatueHPSlider에게 전달하기 위해 public
        public GameObject hpBar;

        protected bool isStatueAlreadyDead;

        public string lineRendererObjectName; // lineRenderer 오브젝트 이름
        public Action<Statue> onDestroied;

        protected override void Awake()
        {
            base.Awake();

            Vector2 newCenter = (Vector2)transform.position - new Vector2(
                col.offset.x * col.transform.localScale.x,
                col.offset.y * col.transform.localScale.y
            );
            transform.position = newCenter;

            isStatueAlreadyDead = false;

            damage = 0; // 석상은 데미지 X
        }
        
        protected void PlaySoundOnDestroy()
        {
            // 석상 파괴되는 소리 구현
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Collapse);
        }
    }
}
