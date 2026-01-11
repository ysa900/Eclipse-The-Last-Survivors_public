using System.Collections;
using UnityEngine;

namespace Eclipse.Game
{
    public class Boss_Bullet : BossSkill, IPoolingObject, IDamageableSkill
    {
        public Transform target;
    
        bool isDead;
        bool isOnLeftSide;
    
        private float hp = 750f;
        private float speed = 3;
    
        Animator animator;
        Rigidbody2D rigid; // 물리 입력을 받기위한 변수
        SpriteRenderer spriteRenderer;
    
        private void Awake()
        {
            animator = GetComponent<Animator>();
            rigid = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();

            aliveTime = 10f;
        }
    
        private void Start()
        {
            damage = 5f;
    
            Init();
        }
    
        public override void Init()
        {
            if (!(boss == null))
            {
                isDead = false;
                aliveTimer = 0f;
                rigid.constraints = RigidbodyConstraints2D.None;
                GetComponent<CapsuleCollider2D>().enabled = true;
    
                bool isBossLookLeft = boss.isBossLookLeft;
    
                float bulletX = boss.transform.position.x;
                float bulletY = boss.transform.position.y;
    
                if (isBossLookLeft)
                {
                    bulletX -= 2.5f;
                }
                else
                {
                    bulletX += 2.5f;
                }
    
                bulletY -= 3f;
    
                transform.position = new Vector2(bulletX, bulletY);
    
                target = PlayerManager.player.GetComponent<Transform>();
            }
    
        }
    
        private void FixedUpdate()
        {
            if (!isDead)
            {
                target = PlayerManager.player.GetComponent<Transform>();
                Vector2 direction = new Vector2(transform.position.x - target.position.x, transform.position.y - target.position.y);
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    
                Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.forward);
                Quaternion rotation = Quaternion.Slerp(transform.rotation, angleAxis, 5f);
                transform.rotation = rotation;
    
                isOnLeftSide = Mathf.Cos(angle * Mathf.Deg2Rad) < 0; // cos값이 -면 플레이어를 기준으로 왼쪽에 있는 것
    
                spriteRenderer.flipY = isOnLeftSide;
    
                rigid.MovePosition(rigid.position - direction.normalized * speed * Time.fixedDeltaTime); // 플레이어 방향으로 위치 변경
    
                X = transform.position.x;
                Y = transform.position.y;
    
                if (aliveTimer > aliveTime)
                {
                    StartCoroutine(Dead());
                }
    
                aliveTimer += Time.fixedDeltaTime;
            }
    
        }
    
        IEnumerator Dead()
        {
            animator.SetTrigger("Hit");
    
            isDead = true;
    
            rigid.constraints = RigidbodyConstraints2D.FreezeAll;
            GetComponent<CapsuleCollider2D>().enabled = false;
    
            yield return new WaitForSeconds(0.35f); // 지정한 초 만큼 쉬기
    
            PoolManager.instance.ReturnBossSkill(this, index);
        }
    
        private void OnTriggerEnter2D(Collider2D collision)
        {
            IPlayer iPlayer = collision.GetComponent<IPlayer>();
    
            if (iPlayer == null)
            {
                return;
            }
    
            iPlayer.TakeDamageOneTime(damage);
    
            StartCoroutine(Dead());
        }
    
        public void TakeDamage(string causerTag, float damage, bool isCritical = false)
        {
            hp -= damage;
            InGameTextManager.Instance.ShowText(Mathf.RoundToInt(damage).ToString(), causerTag, isCritical, transform.position); // damageText 출력
    
            if (hp <= 0)
            {
                PoolManager.instance.ReturnBossSkill(this, index);
            }
        }
    }
}