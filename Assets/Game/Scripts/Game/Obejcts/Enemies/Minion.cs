using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Eclipse.Game
{
    public class Minion : Enemy, IDamageable, IPoolingObject, IDebuffable, IDangerousObject
    {
        // Minion 규칙: 모든 Minion 프리팹은 오른쪽을 보는 게 기본

        // Minion 종류
        public int index;

        // 스탯
        public float speed;

        // 스탯 저장용 변수
        private float savedHp;
        private float savedSpeed;

        // 넉백 관련
        private bool isKnockback = false;
        private float knockbackDuration = 0.2f; // 넉백 지속 시간

        // 기타
        public bool isMinionLookLeft; // 적이 보고 있는 방향을 알려주는 변수

        protected bool isDead;

        // 미니언 피격 모션용 변수
        private float hitReactionDuration = 2f;
        protected float hitReactionTimer = 0;

        protected string sceneName;

        // ======================================================
        private float savedDamage; // 실명 시간 계산용

        private Coroutine blindCorountine; // 실명 시간 저장 코루틴
        private Coroutine bindCoroutine; // 속박 시간 저장 코루틴

        // 레전더리 디버프 시간 계산용
        private float legendaryTimer;

        private float dotDamageTimer;
        private float dotDamageInterval = 0.5f;  // 도트 데미지 간격 설정

        // 도트 데미지를 입히는 코루틴
        private Coroutine legendaryDebuffCoroutine;

        // ======================================================

        // 피격 처리 관련 변수들
        Color redCol;
        float tmpcol = 1;
        float alphaSpeed = 10;
        bool isHitted;

        private bool isBound; // 속박 상태를 나타내는 변수
        private bool isBlinded; // 실명 상태를 나타내는 변수

        public Rigidbody2D rigid; // 물리 입력을 받기위한 변수
        SpriteRenderer spriteRenderer; // 적 방향을 바꾸기 위해 flipX를 가져오기 위한 변수
        protected Animator animator; // 애니메이션 관리를 위한 변수

        // ======================================================
        // delegate 모음

        // enemy가 죽었을 때 EnemyManager에게 알려주기 위한 delegate
        public delegate void OnEnemyWasKilled(Minion killedEnemy);
        public OnEnemyWasKilled onMinionWasKilled;

        // enemy 피격음을 EnemyManager가 총괄하도록 하는 delegate
        public delegate void OnEnemyHit();
        public OnEnemyHit onMinionHit;

        // ======================================================
        Player player;

        public Player Player
        {
            get => player;
            set => player = value;
        }

        public virtual void Init()
        {
            hp = maxHp;

            if (savedSpeed != 0)
            {
                speed = savedSpeed;
            }
            if (savedDamage != 0)
            {
                damage = savedDamage;
            }
            savedSpeed = 0;
            isDead = false;
            isBound = false;
            isBlinded = false;

            hitReactionTimer = 0;

            tmpcol = 1;
            redCol = Color.white;
            spriteRenderer.color = redCol;
            isHitted = false;

            rigid.velocity = Vector2.zero;
            isKnockback = false;

            rigid.constraints = RigidbodyConstraints2D.None;
            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
            GetComponent<CapsuleCollider2D>().enabled = true;
        }

        protected override void Awake()
        {
            base.Awake();

            rigid = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();

            sceneName = SceneManager.GetActiveScene().name;
        }

        protected virtual void Update()
        {
            hitReactionTimer += Time.deltaTime;
        }

        protected virtual void FixedUpdate()
        {
            MoveIfTooFar(); // 플레이어와의 거리가 너무 멀면 위치
        }

        private void LateUpdate()
        {
            if (isHitted)
            {
                HitReaction();
            }
        }

        // 타겟을 바라보는 함수
        protected void LookAtTarget()
        {
            Vector2 targetPosition = targetObject.transform.position;
            Vector2 myPosition = transform.position;
            Vector2 direction = targetPosition - myPosition;

            if (Math.Abs(direction.x) >= 0.3f)
            {
                isMinionLookLeft = direction.x < 0;
            }

            Vector3 scale = transform.localScale;
            int signX = isMinionLookLeft ? -1 : 1;
            scale.x = Mathf.Abs(scale.x) * signX;
            transform.localScale = scale;
        }

        // 타겟 방향으로 이동하는 함수
        protected void MoveToTarget()
        {
            if (isKnockback) return; // 넉백 중에는 이동 금지

            Vector2 targetPosition = targetObject.transform.position;
            Vector2 myPosition = transform.position;

            Vector2 direction = targetPosition - myPosition;

            direction = direction.normalized;
            rigid.MovePosition(rigid.position + direction * speed * Time.fixedDeltaTime);
        }

        // 플레이어와의 거리가 너무 멀면 이동시키는 메서드
        private void MoveIfTooFar()
        {
            Vector2 playerPosition = PlayerManager.player.transform.position;
            Vector2 myPosition = transform.position;

            float sqrDistance = (myPosition - playerPosition).sqrMagnitude;

            bool isTooFar = sqrDistance > 10 * 10;

            if (isTooFar)
            {
                switch (sceneName)
                {
                    case "Stage1": // 플레이어가 너무 멀리 가면 enemy를 플레이어를 중심으로 점 대칭 이동
                        float xDiff = myPosition.x - playerPosition.x;
                        float yDiff = myPosition.y - playerPosition.y;

                        // tooFar 범위에 계속 걸치는 문제를 방지하기 위해 안쪽으로 넣어줌
                        xDiff = xDiff > 0 ? xDiff - 1 : xDiff + 1;
                        yDiff = yDiff > 0 ? yDiff - 1 : yDiff + 1;

                        Vector2 vector2 = new Vector2(playerPosition.x - xDiff, playerPosition.y - yDiff);
                        transform.position = vector2;

                        break;

                    case "Stage2": // 플레이어가 너무 멀리 가면 enemy를 플레이어를 중심으로 y축 대칭 이동
                        xDiff = myPosition.x - playerPosition.x;

                        // toFar 범위에 계속 걸치는 문제를 방지하기 위해 안쪽으로 넣어줌
                        xDiff = xDiff > 0 ? xDiff - 1 : xDiff + 1;

                        vector2 = new Vector2(playerPosition.x - xDiff, Y);
                        transform.position = vector2;

                        break;
                }

            }
        }

        // IDamageable의 함수 TakeDamage
        public void TakeDamage(string causerTag, float damage, bool isCritical = false, float knockbackForce = 0)
        {
            hp = hp - (int)damage;

            onMinionHit(); // delegate 호출
            InGameTextManager.Instance.ShowText(Mathf.RoundToInt(damage).ToString(), causerTag, isCritical, transform.position); // damageText 출력

            isHitted = true;

            if (hp <= 0 && !isDead)
            {
                Dead();
            }
            else
            {
                if (hitReactionDuration <= hitReactionTimer)
                {
                    animator.SetTrigger("Hit");
                    hitReactionTimer = 0;
                }
            }

            ApplyKnockback(knockbackForce); // 넉백 적용
        }

        public virtual void Dead()
        {
            if (!gameObject.activeSelf || isDead) return; // 이미 죽었거나 비활성화된 경우 무시
            isDead = true;

            animator.SetTrigger("Dead");

            onMinionWasKilled(this); // 대리자 호출

            rigid.constraints = RigidbodyConstraints2D.FreezeAll;
            GetComponent<CapsuleCollider2D>().enabled = false;

            StartCoroutine(WaitForAnimation());
        }

        protected IEnumerator WaitForAnimation()
        {
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Die"));

            StartCoroutine(WaitForDead());

        }

        protected virtual IEnumerator WaitForDead()
        {
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f);

            bool isStage1 = gameObject.tag == "EvilTree" || gameObject.tag == "Ghost" || gameObject.tag == "Pumpkin" || gameObject.tag == "WarLock";

            if (isStage1)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (transform.GetChild(i).gameObject.activeSelf)
                    {
                        transform.GetChild(i).gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    if (transform.GetChild(i).gameObject.activeSelf)
                    {
                        transform.GetChild(i).gameObject.SetActive(false);
                    }
                }
            }

            PoolManager.instance.ReturnMinion(this, index);
            animator.ResetTrigger("Dead");
        }

        public void ApplyKnockback(float knockbackForce)
        {
            if (isKnockback) return; // 넉백 중일 때 또 호출되지 않도록 방지

            isKnockback = true;
            Vector2 knockbackDirection = (transform.position - player.transform.position).normalized;
            Vector2 force = knockbackDirection * knockbackForce;

            rigid.velocity = Vector2.zero; // 기존 속도 제거
            rigid.AddForce(force * rigid.mass, ForceMode2D.Impulse); // 질량 고려한 넉백 적용

            if (isDead) return; // 죽은 적은 넉백 적용 안함
            StartCoroutine(EndKnockback(knockbackDuration)); // 일정 시간 후 이동 재개
        }

        private IEnumerator EndKnockback(float duration)
        {
            yield return new WaitForSeconds(duration);

            rigid.velocity = Vector2.zero; // 넉백 이후 바로 멈춤 (더 자연스럽게)
            isKnockback = false;
        }

        public IEnumerator MakeEnemyHardPattern()
        {
            transform.GetChild(0).gameObject.SetActive(true);
            savedHp = hp;
            hp *= 2f;

            yield return new WaitForSeconds(30f); // 지정한 초 뒤에 패턴 끄기

            transform.GetChild(0).gameObject.SetActive(false);

            // Enemy Hp 보정
            if (tag == "EvilTree")
            {
                if (hp >= savedHp)
                {
                    hp = savedHp;
                }
            }
            else if (tag == "Pumpkin")
            {
                if (hp >= savedHp)
                {
                    hp = savedHp;
                }
            }
            else if (tag == "Warlock")
            {
                if (hp >= savedHp)
                {
                    hp = savedHp;
                }
            }
        }

        void HitReaction()
        {
            float redAmount = 0.5f;

            tmpcol = Mathf.Lerp(tmpcol, redAmount, Time.deltaTime * alphaSpeed);
            if (tmpcol < redAmount) tmpcol = redAmount;

            redCol.g = tmpcol;
            redCol.b = tmpcol;

            spriteRenderer.color = redCol;

            if (Math.Abs(tmpcol - redAmount) < 0.01f)
            {
                redCol = Color.white;
                spriteRenderer.color = redCol;
                isHitted = false;
                tmpcol = 1;
            }
        }

        public float GetEnemyDamage()
        {
            return damage;
        }

        public virtual void MakeBlind(float blindTime)
        {
            // 개선 방법 생각
            bool isStage1 = gameObject.tag == "EvilTree" || gameObject.tag == "Ghost" || gameObject.tag == "Pumpkin" || gameObject.tag == "WarLock";

            if (!isBlinded)
            {
                // Blind 이펙트 켜기
                if (isStage1)
                {
                    transform.GetChild(1).gameObject.SetActive(true);
                }
                else
                {
                    transform.GetChild(0).gameObject.SetActive(true);
                }

                savedDamage = damage;
                damage = 0;
                isBlinded = true;

                if (gameObject.activeSelf)
                {
                    blindCorountine = StartCoroutine(WaitUntilBlindTimeAndRestoreDamage(blindTime, savedDamage));
                }
            }
            else
            {
                if (blindCorountine != null)
                {
                    StopCoroutine(blindCorountine);
                }

                if (gameObject.activeSelf)
                {
                    blindCorountine = StartCoroutine(WaitUntilBlindTimeAndRestoreDamage(blindTime, savedDamage));
                }
            }
        }

        public virtual void MakeBind(float slowTime)
        {

            bool isStage1 = gameObject.tag == "EvilTree" || gameObject.tag == "Ghost" || gameObject.tag == "Pumpkin" || gameObject.tag == "WarLock";

            if (!isBound) // 속도가 이미 0이 아닐 때만 처리
            {
                // Bind 이펙트 켜기
                if (isStage1)
                {
                    transform.GetChild(2).gameObject.SetActive(true);
                }
                else
                {
                    transform.GetChild(1).gameObject.SetActive(true);
                }

                savedSpeed = speed; // 원래 속도 저장
                speed = 0; // 속도를 0으로
                isBound = true;

                if (gameObject.activeSelf)
                {
                    bindCoroutine = StartCoroutine(DelaySpeedRestore(slowTime));
                }
            }
            else
            {
                if (bindCoroutine != null)
                {
                    StopCoroutine(bindCoroutine);
                }

                if (gameObject.activeSelf)
                {
                    bindCoroutine = StartCoroutine(DelaySpeedRestore(slowTime));
                }
            }
        }

        // 속박 상태에서 스킬 종료돼도 delay 만큼 이후에 속도 정상화
        private IEnumerator DelaySpeedRestore(float delay)
        {
            yield return new WaitForSeconds(delay);

            speed = savedSpeed; // 원래 속도로 정상화

            bool isStage1 = gameObject.tag == "EvilTree" || gameObject.tag == "Ghost" || gameObject.tag == "Pumpkin" || gameObject.tag == "WarLock";

            // Bind 이펙트 끄기
            if (isStage1)
            {
                transform.GetChild(2).gameObject.SetActive(false);
            }
            else
            {
                transform.GetChild(1).gameObject.SetActive(false);
            }

            isBound = false; // 속박 상태 해제
        }

        private IEnumerator WaitUntilBlindTimeAndRestoreDamage(float blindTime, float damage)
        {
            yield return new WaitForSeconds(blindTime);

            this.damage = damage; // 원래 데미지로 정상화

            bool isStage1 = gameObject.tag == "EvilTree" || gameObject.tag == "Ghost" || gameObject.tag == "Pumpkin" || gameObject.tag == "WarLock";

            // Blind 이펙트 끄기
            if (isStage1)
            {
                transform.GetChild(1).gameObject.SetActive(false);
            }
            else
            {
                transform.GetChild(0).gameObject.SetActive(false);
            }
            isBlinded = false; // 실명 상태 해제
        }

        public virtual void ApplyLegendaryDebuff(float damage, float debuffTime, Action onSkillAttack, int? level = null)
        {
            if (level == null)
            {
                // 기존 debuffTime이 덮어씌워지는지 확인하고, 코루틴이 이미 실행 중이면 중단
                if (legendaryDebuffCoroutine != null)
                {
                    StopCoroutine(legendaryDebuffCoroutine);
                }

                // 새로운 debuffTime 동안 도트 데미지 적용
                legendaryDebuffCoroutine = StartCoroutine(ApplyLegendaryDebuff(damage, debuffTime, onSkillAttack));
            }
        }

        // 코루틴: debuffTime 동안 도트 데미지를 주기적으로 적용
        private IEnumerator ApplyLegendaryDebuff(float damage, float debuffTime, Action onSkillAttack)
        {
            legendaryTimer = 0f;
            dotDamageTimer = 0f;

            bool isStage1 = gameObject.tag == "EvilTree" || gameObject.tag == "Pumpkin" || gameObject.tag == "WarLock";

            // SpellSword 이펙트 켜기
            if (isStage1)
            {
                transform.GetChild(3).gameObject.SetActive(true); // SpellSword 이펙트 켜기
            }
            else // 스테이지 2, 3
            {
                transform.GetChild(2).gameObject.SetActive(true); // SpellSword 이펙트 켜기
            }

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

            if (hp > 0)
            {
                // debuffTime이 끝났는데 죽지 않으면 이펙트 비활성화
                if (isStage1)
                {
                    transform.GetChild(3).gameObject.SetActive(false); // SpellSword 이펙트 끄기
                }
                else
                {
                    transform.GetChild(2).gameObject.SetActive(false); // SpellSword 이펙트 끄기
                }
            }
        }

        public float GetObjectDamage()
        {
            return damage;
        }

        public void Enrage()
        {
            damage *= 1.5f;
            speed *= 1.5f;
        }
    }
}