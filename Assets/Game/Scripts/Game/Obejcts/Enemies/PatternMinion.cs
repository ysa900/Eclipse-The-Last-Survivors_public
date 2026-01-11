using System;
using System.Collections;
using UnityEngine;

namespace Eclipse.Game
{
    public class PatternMinion : Minion
    {
        float batTime;
        float batTimer;
        Vector2 direction;
    
        public delegate void OnChangeDebuffVisible();
        public OnChangeDebuffVisible onChangeDebuffVisible;
    
        public delegate bool OnReturnDebuffVisible();
        public OnReturnDebuffVisible onReturnDebuffVisible;
        
        public override void Init()
        {
            base.Init();

            isDead = false;
            
            batTimer = 0f;

            rigid.constraints = RigidbodyConstraints2D.None;
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
            GetComponent<CapsuleCollider2D>().enabled = true;
        }
    
        void Start()
        {
            batTime = 5f;
        }

        protected override void Update()
        {
            base.Update();

            batTimer += Time.deltaTime;

            if (batTimer >= batTime)
            {
                Dead();

                batTimer = 0f;
            }
        }

        new void FixedUpdate()
        {
            MoveToPoint();
        }
    
        public void SetDirection(float startX, float startY, float ranX, float ranY)
        {
            Vector2 playerPosition = PlayerManager.player.transform.position;
            Vector2 myPosition = new Vector2((int)startX, (int)startY);
    
            playerPosition.x += ranX + UnityEngine.Random.Range(-1f, 1f);
            playerPosition.y += ranY + UnityEngine.Random.Range(-1f, 1f);
    
            direction = playerPosition - myPosition;
            direction = direction.normalized;
            
        }
    
        private void MoveToPoint()
        {
            rigid.MovePosition(rigid.position + direction * speed * Time.fixedDeltaTime);
        }
    
        private void OnTriggerEnter2D(Collider2D collision)
        {
            IPlayer iPlayer = collision.GetComponent<IPlayer>();
    
            if (iPlayer == null)
            {
                return;
            }

            onChangeDebuffVisible();
            iPlayer.TakeDamageConstantly(true, damage);
        }
    
        private void OnTriggerExit2D(Collider2D collision)
        {
            IPlayer iPlayer = collision.GetComponent<IPlayer>();
    
            if (iPlayer == null)
            {
                return;
            }

            iPlayer.TakeDamageConstantly(false);
        }

        protected override IEnumerator WaitForDead()
        {
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f);

            PoolManager.instance.ReturnMinion(this, index);
            animator.ResetTrigger("Dead");
        }

        public override void MakeBlind(float blindTime)
        {
            // 비워두기 (아무것도 안해야 함)
        }

        public override void MakeBind(float slowTime)
        {
            // 비워두기 (아무것도 안해야 함)
        }

        public override void ApplyLegendaryDebuff(float damage, float debuffTime, Action onSkillAttack, int? level = null)
        {
            // 비워두기 (아무것도 안해야 함)
        }
    }
}