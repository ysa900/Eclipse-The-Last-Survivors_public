using UnityEngine;

namespace Eclipse.Game
{
    public class ArrangeMinion : Minion
    {
        // Arrange Enemy attackRange
        int[] attackRanges = { 3 };
    
        [SerializeField] float attackRange;
    
        float attackCoolTime = 5f;
        float attackCoolTimer = 5f;
    
        public override void Init()
        {
            attackCoolTimer = 5f;
    
            switch (tag)
            {
                case "Skeleton_Archer":
                    attackRange = attackRanges[0];
                    break;
            }
    
            base.Init();
        }
    
        protected override void FixedUpdate()
        {
            if (isDead)
                return;
    
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
    
            Vector2 enemyPos = transform.position;
            Vector2 targetPos = TargetObject.transform.position;
            float sqrDistance = (enemyPos - targetPos).sqrMagnitude;
    
            bool isInAttackRange = sqrDistance <= attackRange * attackRange; // 플레이어가 사거리 내에 있을때만 공격이 나간다
            bool isAttackOK = attackCoolTime <= attackCoolTimer; // 플레이어가 사거리 내에 있을때만 공격이 나간다

            if (!isInAttackRange)
            {
                MoveToTarget();
            }
            else if (isAttackOK) // 궁수
            {
                Arrange_Attack();
                attackCoolTimer = 0;
            }

            LookAtTarget();
            attackCoolTimer += Time.fixedDeltaTime;
    
            base.FixedUpdate();
        }
    
        void Arrange_Attack()
        {
            switch (tag)
            {
                case "Skeleton_Archer":
                    animator.SetTrigger("Attack");
                    PoolManager.instance.GetArrow(this);
                    break;
            }
            
        }
    }
}