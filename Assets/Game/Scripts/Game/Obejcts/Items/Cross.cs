using UnityEngine;

namespace Eclipse.Game
{
    public class Cross : Item
    {
        CircleCollider2D damageCollider;
        SpriteRenderer spriteRenderer;

        bool isPlayerTriggered;
        float damage;
        float aliveTime = 0.1f;
        float aliveTimer = 0;

        protected override void Awake()
        {
            base.Awake();
            damageCollider = GetComponentsInChildren<CircleCollider2D>()[1];
            damageCollider.gameObject.SetActive(false);
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (isPlayerTriggered)
            {
                aliveTimer += Time.deltaTime;
                if (aliveTimer > aliveTime) Destroy(gameObject);
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (!isPlayerTriggered)
            {
                IPlayer iPlayer = collision.GetComponent<IPlayer>();

                if (iPlayer == null)
                {
                    return;
                }
                isPlayerTriggered = true;

                col.enabled = false;
                damageCollider.gameObject.SetActive(true);
                spriteRenderer.enabled = false;

                AudioManager.instance.PlaySfx(AudioManager.Sfx.Pickup);
            }
            else
            {
                IDamageable damageable = collision.GetComponent<IDamageable>();

                if (damageable == null)
                {
                    return;
                }

                Enemy enemy = collision.GetComponent<Enemy>();
                
                if (enemy == null)
                {
                    return;
                }
                switch (enemy)
                {
                    case Boss boss:
                        damage = boss.maxHp * 0.1f; // Boss는 적의 최대 체력의 10%만큼 데미지를 입힘
                        break;
                    case Minion minion:
                        damage = minion.maxHp; // Minion은 적의 최대 체력만큼 데미지를 입힘
                        break;
                }
                damageable.TakeDamage(tag, damage);
            }
        }
    }
}