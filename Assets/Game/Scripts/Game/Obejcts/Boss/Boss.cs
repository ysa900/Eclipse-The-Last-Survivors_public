using System;
using System.Collections;
using UnityEngine;

namespace Eclipse.Game
{
    public class Boss : Enemy
    {
        // ======================================================
        // Behavior Tree의 Root Node
        public Node rootNode;

        // ======================================================
        // Boss Status
        [ReadOnly] public float damageReduction;
        public Server_PlayerData server_PlayerData;

        // ======================================================
        // Boss Components
        protected Animator animator;
        protected SpriteRenderer spriteRenderer;
        protected Rigidbody2D rigid;

        // ======================================================
        // 보스 광폭화 판단
        protected bool enrageAlreadyActivated; 
        protected float enrage_DamageIncrementCoefficient = 1.25f;
        protected float enrage_CoolDownDecrementCoefficient = 1.5f;
        protected float enrage_SpeedIncrementCoefficient = 1.25f;

        // ======================================================
        // 보스 설정 관련 변수들
        [ReadOnly] public bool isBossLookLeft; // 보스가 보고 있는 방향을 알려주는 변수
        [SerializeField] protected bool isDirectionLocked; // 공격이 실행중인지 체크하는 변수

        // ======================================================
        // 혼란 상태
        protected bool isBlinded;
        protected float blindTimer = 0;

        // 슬로우 상태
        protected float slowDownCoefficient = 0.75f; // 보스 움직임을 slowDownCoefficient만큼 슬로우
        protected float slowTimer;

        // [어쌔신용] 레전더리 디버프 시간 계산용
        protected float legendaryTimer;
        [ReadOnly] public float compressedDamage = 0f; // 누적 피해 저장

        protected float dotDamageTimer;
        protected float dotDamageInterval = 0.5f;  // 도트 데미지 간격 설정

        // 도트 데미지를 입히는 코루틴
        protected Coroutine legendaryDebuffCoroutine;
        protected Coroutine slowDownCoroutine;

        // ======================================================
        // BossManager에게 보스가 죽었다고 알려주기 위한 Action
        protected bool isDead;
        public Action onBossDead;

        // ======================================================

        protected override void Awake()
        {
            base.Awake();

            animator = GetComponent<Animator>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            rigid = GetComponent<Rigidbody2D>();
            col = GetComponent<Collider2D>();
        }

        protected virtual void Start()
        {
            // 변수 초기화
            enrageAlreadyActivated = false;
            isDirectionLocked = false;
            isBlinded = false;
            isDead = false;

            targetObject = PlayerManager.player.gameObject; // 초기 Target 설정
            StatusInit(); // 보스 스탯 초기화
            rootNode = SetupBehaviorTree(); // rootNode에 BehaviorTree 설정
        }

        protected virtual void StatusInit()
        {

        }

        protected virtual void Update()
        {
            if (isDead) return;

            SetBossDirection(); // 보스 방향 설정
            rootNode.Evaluate(); // 보스 AI 실행

            UpdateTimers();
        }

        protected virtual void UpdateTimers()
        {
            //targetUpdateTimer += Time.deltaTime; // 타겟 재설정 시간 타이머, 현재는 분신을 타겟에서 제외하면서 사용하지 않음
        }

        protected virtual Node SetupBehaviorTree()
        {
            return null;
        }

        protected void SetBossDirection()
        {
            if (isDirectionLocked) return; // 공격 중일 때는 방향 설정을 하지 않음

            Vector2 targetPosition = targetObject.transform.position;
            Vector2 myPosition = transform.position;

            Vector2 direction = targetPosition - myPosition;
            direction = direction.normalized;

            if (Math.Abs(direction.x) >= 0.3f)
            {
                isBossLookLeft = direction.x < 0;
            }

            Vector3 scale = transform.localScale;
            int signX = isBossLookLeft ? -1 : 1;
            scale.x = Mathf.Abs(scale.x) * signX;
            transform.localScale = scale;
        }

        // IDamageable
        public virtual void TakeDamage(string causerTag, float damage, bool isCritical = false, float knockbackForce = 0)
        {

        }

        protected void OnCollisionExit2D(Collision2D collision)
        {
            rigid.velocity = Vector2.zero; // 기존 속도 제거
        }

        //==================================================================
        // [어쌔신용] 보스용 파멸각인
        // 5 레벨 아니면 기존 매커니즘(도트 데미지)
        // 5 레벨 : 압축딜
        public void ApplyLegendaryDebuff(float damage, float debuffTime, Action onSkillAttack, int? level = null)
        {
            if (level != 5) // 기존 도트 데미지처럼
            {
                // 기존 debuffTime이 덮어씌워지는지 확인하고, 코루틴이 이미 실행 중이면 중단
                if (legendaryDebuffCoroutine != null)
                {
                    StopCoroutine(legendaryDebuffCoroutine);
                }

                // 새로운 debuffTime 동안 도트 데미지 적용
                if (gameObject.activeSelf)
                {
                    legendaryDebuffCoroutine = StartCoroutine(ApplyLegendaryDotDamage(damage, debuffTime, onSkillAttack));
                }
            }
            else if (level == 5) // 압축 딜
            {
                if (legendaryDebuffCoroutine != null)
                {
                    StopCoroutine(legendaryDebuffCoroutine);
                }

                compressedDamage = 0f; // 피해 누적 변수 초기화// 새로운 debuffTime 동안 도트 데미지 적용
                if (gameObject.activeSelf)
                {
                    legendaryDebuffCoroutine = StartCoroutine(ApplyCompressedDamage(damage, debuffTime, onSkillAttack));
                }
            }
        }

        // debuffTime 동안 도트 데미지를 주기적으로 적용
        private IEnumerator ApplyLegendaryDotDamage(float damage, float debuffTime, Action onSkillAttack)
        {
            legendaryTimer = 0f;
            dotDamageTimer = 0f;

            // SpellSword 이펙트 켜기
            transform.GetChild(2).gameObject.SetActive(true);

            // debuffTime 동안 지속적으로 도트 데미지 적용
            while (legendaryTimer < debuffTime)
            {
                legendaryTimer += Time.deltaTime;
                dotDamageTimer += Time.deltaTime;

                if (dotDamageTimer >= dotDamageInterval)
                {
                    // 도트 데미지 적용
                    TakeDamage("Legendary", damage);
                    onSkillAttack.Invoke();
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Range); // 도트 딜 효과음으로 대체 예정

                    // 타이머 초기화
                    dotDamageTimer = 0f;
                }

                yield return null;
            }

            // SpellSword 이펙트 끄기
            transform.GetChild(2).gameObject.SetActive(false);
        }

        // 5레벨일 때의 압축 딜 코루틴
        IEnumerator ApplyCompressedDamage(float damage, float debuffTime, Action onSkillAttack)
        {
            legendaryTimer = 0f;

            // SpellSword 이펙트 켜기
            transform.GetChild(2).gameObject.SetActive(true);

            while (legendaryTimer < debuffTime)
            {
                legendaryTimer += Time.deltaTime;

                yield return null;
            }

            // debuffTime이 끝나면 누적된 피해를 한 번에 적용
            ApplyFinalDamage(damage);
            onSkillAttack.Invoke();

            // SpellSword 이펙트 끄기
            transform.GetChild(2).gameObject.SetActive(false);
        }

        // 누적된 피해를 한 번에 적용
        protected void ApplyFinalDamage(float damage)
        {
            damage += compressedDamage; // 압축 딜
            damage *= 1.1f; // 기존 도트 데미지 1.1배 피해
            TakeDamage("Legendary", damage);

            AudioManager.instance.PlaySfx(AudioManager.Sfx.Range); // 압축 딜 효과음으로 대체 예정
        }

        //==================================================================
        // IDebuffable 구현
        public void MakeBlind(float blindTime)
        {
            if (isBlinded)
            {
                blindTimer = 0f;
                return;
            }

            transform.GetChild(0).gameObject.SetActive(true);

            float tempDamage = damage;
            damage = 0;
            isBlinded = true;

            if (gameObject.activeSelf)
            {
                StartCoroutine(WaitUntilBlindTimeAndRestoreDamage(blindTime, tempDamage));
            }
        }

        IEnumerator WaitUntilBlindTimeAndRestoreDamage(float blindTime, float damage)
        {
            blindTimer = 0;
            while (blindTime <= blindTimer)
            {
                blindTimer += Time.deltaTime;
                yield return null;
            }

            yield return new WaitUntil(() => blindTime <= blindTimer);

            this.damage = damage; // 원래 데미지로 정상화
            transform.GetChild(0).gameObject.SetActive(false);
            isBlinded = false; // 실명 상태 해제
        }

        //==================================================================
        public void MakeBind(float slowTime)
        {
            if (slowDownCoroutine == null && gameObject.activeSelf)
            {
                slowDownCoroutine = StartCoroutine(SlowDownRoutine(slowTime));
            }
        }

        // 자식 클래스에서 오버라이드하여 슬로우 효과를 구현
        protected virtual IEnumerator SlowDownRoutine(float duration)
        {
            yield return null;
        }

        //==================================================================
        public float GetObjectDamage()
        {
            return damage;
        }
        
        //==================================================================
        // Animation Event를 추가하는 메서드
        protected void AddAnimationEvent(AnimationClip clip, string functionName, float time)
        {
            AnimationEvent animationEvent = new AnimationEvent
            {
                functionName = functionName,
                time = time
            };
            clip.AddEvent(animationEvent);
        }
        //==================================================================
        // 애니메이션이 끝날 때까지 기다리는 코루틴 (최대 3초 대기)
        protected IEnumerator WaitForAnimationEnd(string animationName, float animationThreshold)
        {
            float timeout = 3f;
            float timer = 0f;

            // 애니메이션이 끝날 때까지 또는 3초가 지나기 전까지 대기
            while (timer < timeout)
            {
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0); // 레이어 0 사용

                if (stateInfo.IsName(animationName) && stateInfo.normalizedTime >= animationThreshold)
                {
                    break; // 애니메이션 완료
                }

                timer += Time.deltaTime;
                yield return null;
            }

            Debug.LogWarning($"WaitForAnimationEnd: 애니메이션 '{animationName}'이(가) 3초 내에 끝나지 않았습니다.");
        }
    }
}
