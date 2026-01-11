using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static Eclipse.Game.DamageHandler;

namespace Eclipse.Game
{
    public class Skill : Object, IPoolingObject
    {
        // 도트 데미지 관련 변수
        public bool isDotDamageSkill;
        protected float dotDelayTime = 0.2f;
        protected Dictionary<int, float> dotDelayTimers = new Dictionary<int, float>(17); // 딕셔너리 capacity는 17로 설정, 무조건 소수로 되어야 함
        protected Dictionary<int, (Collider2D collider, IDamageable damageable, IDamageableSkill damageableSkill)> activeTargets = new Dictionary<int, (Collider2D, IDamageable, IDamageableSkill)>(17); // 활성화된 적 목록

        // 스킬이 꺼질 때 SkillManager의 isSkillsActivated[]를 끄기 위한 delegate
        public delegate void OnSkillFinished(int index);
        public OnSkillFinished onSkillFinished;

        // 스킬 데미지 미터기 기록을 위한 delegate
        public delegate void OnSkillAttack(int index, float damage);
        public OnSkillAttack onSkillAttack;

        public Enemy enemy;

        // 스킬 관련 스탯
        protected float speed;
        protected float attackPower; // 플레이어 공격력
        protected float damage; // 스킬 공격력
        public float Damage { get { return damage; } set { damage = value; } }
        protected float aliveTimer; // 스킬 생존 시간을 체크할 변수
        public float AliveTimer { get { return aliveTimer; } set { aliveTimer = value; } }
        protected float aliveTime; // 스킬 생존 시간
        public float AliveTime { get { return aliveTime; } set { aliveTime = value; } }
        protected float knockbackForce;
        protected float criticalChance;
        public float CriticalChance { get { return criticalChance; } set { criticalChance = value; } }
        protected float criticalMultiplier;
        public float CriticalMultiplier { get { return criticalMultiplier; } set { criticalMultiplier = value; } }

        // 스킬, 풀링 인덱스
        public int skillIndex;
        public int returnIndex;

        // 컴포넌트
        protected Animator animator;
        public SpriteRenderer spriteRenderer;

        protected virtual void Awake()
        {
            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        public virtual void Init()
        {
            aliveTimer = 0;
            dotDelayTime = 0.2f;

            dotDelayTimers.Clear();
            activeTargets.Clear(); // 캐시도 초기화
        }

        public void Initialize(SkillData2 skillData, Server_PlayerData server_PlayerData, PlayerData playerData, SkillData2 assassinPassiveSkillData = null, MijiSkillData mijiPassiveSkillData = null)
        {
            // skillData 할당
            damage = skillData.damage[skillIndex];
            knockbackForce = skillData.knockbackForce[skillIndex];
            aliveTime = skillData.aliveTime[skillIndex]
                * (1 + server_PlayerData.basicPassiveLevels[6] * server_PlayerData.duration);

            if (mijiPassiveSkillData != null && mijiPassiveSkillData.skillSelected[3])
            {
                aliveTime *= (1 + mijiPassiveSkillData.levelCoefficient[3]);
            }

            // server_PlayerData 할당
            attackPower = server_PlayerData.attackPower * server_PlayerData.basicPassiveLevels[0]
                + playerData.additional_Power;

            // assassinPassiveSkillData 할당
            if (assassinPassiveSkillData != null)
            {
                criticalChance = assassinPassiveSkillData.damage[skillIndex % 3] - 1;
                criticalMultiplier = ((AssassinData)playerData).criticalDamageCoefficient;
            }
            else // 도적이 아니라면
            {
                criticalChance = 0;
                criticalMultiplier = 1;
            }
        }

        protected virtual void Update()
        {
            if (!isDotDamageSkill) return;

            // activeTargets 순회 중 수정 방지를 위해 Keys 복사 후 for문 순회
            var keys = new List<int>(activeTargets.Keys);
            for (int i = 0; i < keys.Count; i++)
            {
                int id = keys[i];

                if (!dotDelayTimers.ContainsKey(id))
                    continue;

                var (collider, damageable, damageableSkill) = activeTargets[id];
                
                dotDelayTimers[id] += Time.deltaTime;

                if (dotDelayTimers[id] >= dotDelayTime)
                {
                    var damageResult = DamageHandler.CalculateDamage(attackPower, damage, criticalChance, criticalMultiplier);
                    
                    ApplyDamage(collider, damageable, damageableSkill, damageResult);
                    dotDelayTimers[id] = 0f;
                }
            }
        }

        protected virtual void LateUpdate()
        {
            aliveTimer += Time.deltaTime;
        }

        public Vector2 GetAdjustedEnemyPosition(Enemy enemy)
        {
            if (enemy == null) return Vector2.zero;

            Vector2 adjustedPosition = enemy.transform.position;
            CapsuleCollider2D collider = enemy.GetComponent<CapsuleCollider2D>();

            if (collider != null)
            {
                adjustedPosition += collider.offset;
            }

            return adjustedPosition;
        }

        protected virtual void OnTriggerEnter2D(Collider2D collision)
        {
            int id = collision.GetInstanceID();

            if (!TryGetDamageable(collision, out var damageable, out var damageableSkill))
                return;

            if (!activeTargets.ContainsKey(id))
            {
                activeTargets[id] = (collision, damageable, damageableSkill);
                dotDelayTimers[id] = dotDelayTime;
            }

            // 도트 데미지 스킬이 아닌 경우에만 즉시 데미지
            if (!isDotDamageSkill)
            {
                var damageResult = DamageHandler.CalculateDamage(attackPower, damage, criticalChance, criticalMultiplier);
                ApplyDamage(collision, damageable, damageableSkill, damageResult);
            }
        }

        protected virtual void OnTriggerExit2D(Collider2D collision)
        {
            int id = collision.GetInstanceID();

            activeTargets.Remove(id);
            dotDelayTimers.Remove(id);
        }

        private bool TryGetDamageable(Collider2D collision, out IDamageable damageable, out IDamageableSkill damageableSkill)
        {
            damageable = collision.GetComponent<IDamageable>();
            damageableSkill = null;

            if (damageable == null)
            {
                damageableSkill = collision.GetComponent<IDamageableSkill>();
                if (damageableSkill == null)
                    return false;
            }
            return true;
        }

        private void ApplyDamage(Collider2D collision, IDamageable damageable, IDamageableSkill damageableSkill, DamageResult damageResult)
        {
            if (damageable != null)
            {
                damageable.TakeDamage(tag, damageResult.FinalDamage, damageResult.IsCritical, knockbackForce);
            }
            else
            {
                damageableSkill.TakeDamage(tag, damageResult.FinalDamage, damageResult.IsCritical);
            }
            onSkillAttack?.Invoke(skillIndex, damageResult.FinalDamage);

            OnAfterDamageApplied(collision, damageResult); // Hook method (e.g. Rogue skills)
        }

        protected virtual void OnAfterDamageApplied(Collider2D collision, DamageResult damageResult)
        {
            // 기본은 아무 것도 안 함, 도적 스킬에서 오버라이드하여 사용
        }
    }
}
