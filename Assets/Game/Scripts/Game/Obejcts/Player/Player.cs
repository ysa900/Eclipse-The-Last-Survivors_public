using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Eclipse.Game
{
    public class Player : Object, IPlayer
    {
        // ====================================================================
        // 키보드 방향키 입력을 위한 벡터
        public Vector2 inputVec;

        // ====================================================================
        public bool isPlayerDead; // 플레이어가 죽었는지 판별하는 변수
        public bool isPlayerLookLeft; // 플레이어가 보고 있는 방향을 알려주는 변수
        public bool isPlayerShielded; // 플레이어가 보호막의 보호를 받고있냐
        public bool isPlayerHolyShielded; // 전사 보호막
        protected bool isBloodWindowAlreadyDisplayed; // 딸피 때 빨강 이펙트 트리거 불 변수
        protected bool isPlayerInvincible; // 플레이어가 무적이냐

        // ====================================================================
        // 플레이어 피격 관련 변수
        protected float hitDelayTime = 0.1f;
        protected float hitDelayTimer = 0.1f;

        protected bool isHit = false;
        protected Color originalColor;
        Color hitColor = Color.red;
        protected float hitDurationTimer = 0f; // 피격 색깔 유지하는 시간
        float hitDurationTime = 0.2f;
        float hitTransitionTimer = 0f; // 피격 색깔로 변경하는데 걸리는 시간
        float hitTransitionTime = 0.2f;
        protected Coroutine hitColorCoroutine; // 피격 색깔로 변경하는 코루틴

        // 피격 데미지
        public float hitDamage;

        [SerializeField] bool isPlayerTakingDamage; // 플레이어가 데미지를 받고있냐
        [SerializeField] float constantDamge; // 지속적으로 받는 데미지

        // ====================================================================
        // 플레이어 레벨업 함수 실행을 기다리게 하기 위한 변수
        public bool isSkillSelectComplete = true;

        // ====================================================================
        protected Rigidbody2D rigid; // 물리 입력을 받기위한 변수
        protected SpriteRenderer spriteRenderer; // 플레이어 방향을 바꾸기 위해 flipX를 가져오기 위한 변수
        public Animator animator; // 애니메이션 관리를 위한 변수

        // ====================================================================
        // 플레이어가 죽었을 시 PlayerManager에게 알려주기 위한 Action
        public Action onPlayerHasKilled;

        // 플레이어가 레벨업 했을 때 GameManager에게 알려주기 위한 Action
        public Action onPlayerLevelUP;

        public Func<bool> getClinetIsChangingScene;

        // ====================================================================
        [ReadOnly] public PlayerData playerData; // 플레이어 데이터
        [ReadOnly] public Server_PlayerData server_PlayerData; // 서버 플레이어 데이터

        // ====================================================================
        public CursorIndicator cursorIndicator; // 커서 위치 표시기 ( 어쌔신용 )

        // ====================================================================
        // 자식 오브젝트
        protected Effect_Blood bloodEffectObejct;
        protected BloodWindow bloodWindow;
        // 부활 이펙트 프리팹
        private ReviveEffect reviveEffectObjectPrefab;
        // 맥스 레벨 후 공격력 증가 이펙트
        protected PowerUp powerUpEffect;

        // ====================================================================
        // 마그넷 실행 중 재실행 여부를 판단하기 위해 캐싱
        Coroutine magnetRestore;

        // ====================================================================

        private void Awake()
        {
            // 변수 초기화
            rigid = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();

            // 어쌔신인 경우
            int playerClass = PlayerPrefs.GetInt("PlayerClass");
            if (playerClass == 2)
            {
                cursorIndicator = gameObject.GetComponentInChildren<CursorIndicator>();
            }

            bloodEffectObejct = GetComponentInChildren<Effect_Blood>();
            bloodWindow = GetComponentInChildren<BloodWindow>();
            reviveEffectObjectPrefab = Resources.Load<ReviveEffect>("Prefabs/Object/Revive Effect");
            powerUpEffect = GetComponentInChildren<PowerUp>();
        }

        public virtual void Init()
        {
            isPlayerDead = false;
            isPlayerLookLeft = false;
            isPlayerShielded = false;
            isBloodWindowAlreadyDisplayed = false;
            hitTransitionTimer = 0f;
            hitDelayTimer = 0.4f;
            isPlayerTakingDamage = false;
            originalColor = spriteRenderer.color;

            // 어쌔신인 경우
            int playerClass = PlayerPrefs.GetInt("PlayerClass");
            if (playerClass == 2)
            {
                cursorIndicator.gameObject.SetActive(false);
            }

            isSkillSelectComplete = true;

            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;

            transform.Find("Player_Light 2D").GetComponent<Light2D>().enabled = false;
            bloodWindow.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 0); // 빨강 투명으로
        }

        void Update()
        {
            if (isPlayerDead || Time.timeScale == 0) return;

            ReceiveDirectionInput(); // 키보드 방향키 입력을 가져오는 함수

            if (isPlayerTakingDamage) TakeDamage(constantDamge, false);

            RegenHP();
            hitDelayTimer += Time.deltaTime;
        }

        private void FixedUpdate()
        {
            if (isPlayerDead) return;

            MovePlayer();
        }

        // 프레임이 끝나기 직전에 실행되는 함수
        private void LateUpdate()
        {
            if (isPlayerDead) return;

            animator.SetFloat("Speed", inputVec.magnitude); // animator의 float타입인 변수 Speed를 inpuVec의 크기만큼으로 설정한다

            isPlayerLookLeft = inputVec.x < 0; // 플레이어가 왼쪽을 보고 있으면

            if (inputVec.x != 0) // 키를 안눌렀을 때는 실행 안되도록 하기 위해 inputVec.x가 0이 아닌 경우만 실행하게 한다
            {
                Vector3 scale = transform.localScale;
                int signX = isPlayerLookLeft ? -1 : 1;
                scale.x = Mathf.Abs(scale.x) * signX;
                transform.localScale = scale;
            }
            else
            {
                Vector3 scale = transform.localScale;
                isPlayerLookLeft = scale.x < 0;
            }
        }

        // 키보드 방향키 입력을 가져오는 함수
        private void ReceiveDirectionInput()
        {
            // 수평, 수직 방향 입력을 받는다
            // inputmanager에 기본 설정돼있다
            // GetAxisRaw를 해야 더욱 명확한 컨트롤 가능
            inputVec.x = Input.GetAxisRaw("Horizontal");
            inputVec.y = Input.GetAxisRaw("Vertical");
        }

        // 플레이어를 움직이는 함수
        private void MovePlayer()
        {
            // 플레이어의 방향벡터를 가져와서 속도를 설정
            // fixedDeltaTime은 물리 프레임 시간
            Vector2 nextVec = inputVec.normalized * playerData.speed * Time.fixedDeltaTime;

            // 입력받은 방향으로 플레이어 위치 설정
            rigid.MovePosition(rigid.position + nextVec);
        }

        //player 경험치 획득 함수
        public void GetExp(int expAmount)
        {
            if (isPlayerDead) return;

            playerData.Exp += expAmount * (1 + server_PlayerData.specialPassiveLevels[2] * server_PlayerData.expBoost);

            if (playerData.Exp >= playerData.nextExp[playerData.level])
                StartCoroutine(LevelUP(1)); // 레벨 업 함수 실행
        }

        public void GetCoin(int coinAmount)
        {
            if (isPlayerDead) return;

            server_PlayerData.coin += coinAmount * Mathf.CeilToInt(1 + server_PlayerData.specialPassiveLevels[3] * server_PlayerData.goldBoost);
        }

        // 플레이어가 몬스터와 충돌하면 데미지를 입는다
        protected void OnCollisionStay2D(Collision2D collision)
        {
            IDangerousObject dangerousObject = collision.gameObject.GetComponent<IDangerousObject>();

            if (dangerousObject == null)
            {
                return;
            }

            hitDamage = dangerousObject.GetObjectDamage();

            TakeDamage(hitDamage, false);
        }
        
        // 플레이어 데미지 처리 관련 함수들
        public virtual void TakeDamageOneTime(float damage)
        {
            TakeDamage(damage, true);
        }
        public virtual void TakeDamageConstantly(bool isStartTakingDamage, float damage = 0)
        {
            isPlayerTakingDamage = isStartTakingDamage;
            if (damage != 0) constantDamge = damage;
        }
        protected virtual void TakeDamage(float damage, bool isDamageUnavoidable)
        {
            if (isPlayerDead || isPlayerInvincible || isPlayerShielded) return;

            if (isPlayerHolyShielded)
            {
                damage *= playerData.holyReductionValue;
            }

            bool isHitDelayOK = hitDelayTimer >= hitDelayTime;
            if (isHitDelayOK || isDamageUnavoidable)
            {
                playerData.hp -= damage * playerData.damageReductionValue;

                AudioManager.instance.PlaySfx(AudioManager.Sfx.Melee0); // 피격 효과음
                hitDelayTimer = 0;
            }

            if (playerData.hp < playerData.maxHp * 0.3f && !isBloodWindowAlreadyDisplayed)
            {
                StartCoroutine(BloodWindowOn());
                isBloodWindowAlreadyDisplayed = true;
            }

            if (playerData.hp <= 0)
            {
                Die();
            }
            else
            {
                bloodEffectObejct.PlayParticle();

                isHit = true; // 피격 상태로 변경
                hitDurationTimer = 0f; // 피격 색깔로 변경하는 타이머 초기화
                if (hitColorCoroutine == null)
                {
                    hitColorCoroutine = StartCoroutine(ChangeToHitColor()); // 피격 이펙트 색깔로 변경
                }
            }
        }

        protected IEnumerator ChangeToHitColor()
        {
            Color startColor = spriteRenderer.color;
            hitTransitionTimer = 0f; // 피격 색깔로 변경하는 타이머 초기화

            while (hitDurationTimer < hitDurationTime)
            {
                hitDurationTimer += Time.deltaTime;
                hitTransitionTimer += Time.deltaTime;
                if (hitTransitionTimer >= hitTransitionTime)
                {
                    hitTransitionTimer = hitTransitionTime; // 타이머가 넘어가면 최대값으로 고정
                }
                spriteRenderer.color = Color.Lerp(startColor, hitColor, hitTransitionTimer / hitTransitionTime);
                yield return null;
            }

            isHit = false; // 피격 상태 해제
            spriteRenderer.color = hitColor; // 피격 색깔로 설정

            StartCoroutine(ChangeToOriginalColor()); // 원래 색깔로 돌아가기
            hitColorCoroutine = null; // 코루틴이 끝났으므로 null로 설정
        }
        IEnumerator ChangeToOriginalColor()
        {
            Color startColor = spriteRenderer.color;
            hitTransitionTimer = 0f; // 원래 색깔로 돌아가는 타이머 초기화

            while (hitTransitionTimer < hitTransitionTime)
            {
                if (isHit)
                {
                    spriteRenderer.color = hitColor; // 피격 상태면 피격 색깔로 유지
                    yield break; // 다시 피격되면 원래 색깔로 돌아가지 않음
                }

                hitTransitionTimer += Time.deltaTime;
                spriteRenderer.color = Color.Lerp(startColor, originalColor, hitTransitionTimer / hitTransitionTime);
                yield return null;
            }
            spriteRenderer.color = originalColor; // 원래 색깔로 설정
        }

        protected void Die()
        {
            isPlayerDead = true;

            animator.SetTrigger("Dead");
            rigid.constraints = RigidbodyConstraints2D.FreezeAll;

            AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead); // 캐릭터 사망 시 효과음
            StartCoroutine(WaitForPlayerDeadMotion());
        }

        private IEnumerator WaitForPlayerDeadMotion()
        {
            // 죽는 애니메이션 길이만큼 기다림
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Death"));

            float animationClipLength = animator.GetCurrentAnimatorStateInfo(0).length;

            yield return new WaitForSeconds(animationClipLength);

            if (TryRevive(animationClipLength)) yield break;

            onPlayerHasKilled();
        }

        private bool TryRevive(float invincibletime)
        {
            int reviveNum = PlayerPrefs.GetInt("Revive");
            if (reviveNum > 0)
            {
                playerData.hp = playerData.maxHp / 2; // 최대 체력 절반으로 부활
                isPlayerDead = false;
                rigid.constraints = RigidbodyConstraints2D.FreezeRotation;

                reviveNum--;
                PlayerPrefs.SetInt("Revive", reviveNum);

                StartCoroutine(SetPlayerInvincibleForSeconds(invincibletime));
                return true;
            }

            return false;
        }

        protected IEnumerator SetPlayerInvincibleForSeconds(float invincibletime)
        {
            isPlayerInvincible = true; // 무적 활성화
            // 부활 이펙트 생성
            ReviveEffect reviveEffectObject = Instantiate(reviveEffectObjectPrefab);
            reviveEffectObject.playerTransform = transform;

            yield return new WaitForSeconds(invincibletime);

            isPlayerInvincible = false; // 무적 비활성화
            // 부활 이펙트 제거
            Destroy(reviveEffectObject.gameObject);
        }

        // 딸피 빨강 이펙트가 켜져야 되면
        protected IEnumerator BloodWindowOn()
        {
            float targetAlpha = 100f / 255f;
            float changeTransitionValue = 10f;

            SpriteRenderer bloodWindow_spriteRenderer = bloodWindow.GetComponent<SpriteRenderer>();
            Color bloodColor = bloodWindow_spriteRenderer.color;

            while (Mathf.Abs(bloodColor.a - targetAlpha) > 0.01f)
            {
                float alpha = Mathf.Lerp(bloodColor.a, targetAlpha, Time.deltaTime * changeTransitionValue);
                bloodColor = new Color(bloodColor.r, bloodColor.g, bloodColor.b, alpha);
                bloodWindow_spriteRenderer.color = bloodColor;

                yield return null;
            }

            StartCoroutine(BloodWindowOff());
        }

        // 딸피 빨강 이펙트가 꺼져야 되면
        protected IEnumerator BloodWindowOff()
        {
            float targetAlpha = 0;
            float changeTransitionValue = 10f;

            SpriteRenderer bloodWindow_spriteRenderer = bloodWindow.GetComponent<SpriteRenderer>();
            Color bloodColor = bloodWindow_spriteRenderer.color;

            while (Mathf.Abs(bloodColor.a - targetAlpha) > 0.01f)
            {
                float alpha = Mathf.Lerp(bloodColor.a, targetAlpha, Time.deltaTime * changeTransitionValue);
                bloodColor = new Color(bloodColor.r, bloodColor.g, bloodColor.b, alpha);
                bloodWindow_spriteRenderer.color = bloodColor;

                yield return null;
            }
        }

        protected virtual IEnumerator LevelUP(int depth)
        {
            if (Screen.IsTransitioning)  // 읽기 전용 프로퍼티로 상태를 확인
                yield break;  // 화면 전환 중이므로 입력 처리 차단
            if (getClinetIsChangingScene()) yield break;

            isSkillSelectComplete = false;

            playerData.Exp -= playerData.nextExp[playerData.level];
            playerData.Exp = playerData.Exp < 0 ? 0 : playerData.Exp;
            playerData.level++;

            onPlayerLevelUP(); // delegate 호출

            yield return new WaitUntil(() => isSkillSelectComplete);
            // 경험치를 경험치 통보다 많이 갖고있으면 재귀적으로 반복
            bool isAgain = playerData.Exp >= playerData.nextExp[playerData.level];
            if (isAgain) StartCoroutine(LevelUP(depth + 1));
        }

        public void RestoreHP(float restoreAmount)
        {
            playerData.hp += restoreAmount;
            AudioManager.instance.PlaySfx(AudioManager.Sfx.HP_Potion);
            playerData.hp = playerData.hp > playerData.maxHp ? playerData.maxHp : playerData.hp;

            if (playerData.hp >= 30) isBloodWindowAlreadyDisplayed = false; // 30 이상으로 회복되면 다시 빨강 경고 나옴
        }

        private void RegenHP()
        {
            if(playerData.hp < playerData.maxHp)
            {
                playerData.hp += playerData.healthRegen * Time.deltaTime;
            }
            else
            {
                playerData.hp = playerData.maxHp;
            }
        }

        public void PowerUpEffectOn()
        {
            if (powerUpEffect != null)
            {
                powerUpEffect.PlayParticle();
                StartCoroutine(StopPowerEffect());
            }
        }

        private IEnumerator StopPowerEffect()
        {
            yield return new WaitForSeconds(1f);

            if(powerUpEffect != null)
            {
                powerUpEffect.StopParticle();
            }
        }
    }
}