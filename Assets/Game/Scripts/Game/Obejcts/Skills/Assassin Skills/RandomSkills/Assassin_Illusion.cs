using UnityEngine;

namespace Eclipse.Game
{
    public class Assassin_Illusion : MonoBehaviour, IPoolingObject
    {
        // 싱글톤 패턴을 사용하기 위한 인스턴스 변수
        private static Assassin_Illusion _instance;
    
        public bool isIllusionDead; // 분신이 사라졌는지 판별하는 변수
        public bool isPlayerLookLeft; // 플레이어가 보고 있는 방향을 알려주는 변수

        private int hitCount = 0; // 충돌 횟수를 저장할 변수
        private const int hitsPerShowDamage = 5; // 5번마다 데미지 텍스트를 표시

        private float hitDelayTime = 0.2f;
        private float hitDelayTimer = 0f;
    
        public float aliveTime;
        public float aliveTimer;
    
        public Rigidbody2D rigid; // 물리 입력을 받기위한 변수
        public SpriteRenderer spriteRenderer; // 플레이어 방향을 바꾸기 위해 flipX를 가져오기 위한 변수
        public Animator animator; // 애니메이션 관리를 위한 변수
        
        // 환영이 HP가 닳아 끝났을 시, ArcaneHeart에 알려주기 위한 delegate
        public delegate void OnIllusionWasDead(Assassin_Illusion assassin_Illusion);
        public OnIllusionWasDead onIllusionWasDead;
    
        public IllusionData illusionData; // 환영 데이터
        [SerializeField] protected float hp;

        private void Awake()
        {
            if (_instance == null)
            {
                _instance = this;
            }
    
            // 변수 초기화
            rigid = GetComponent<Rigidbody2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            animator = GetComponent<Animator>();
        }
    
        public virtual void Init()
        {
            hp = illusionData.hp;

            isIllusionDead = false;
            isPlayerLookLeft = false;
            hitDelayTimer = 0f;
            aliveTimer = 0f;

            //rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
            rigid.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        protected virtual void Update()
        {
            aliveTimer += Time.deltaTime;
            hitDelayTimer += Time.deltaTime;
        }

        // 물리 연산 프레임마다 호출되는 생명주기 함수
        protected virtual void FixedUpdate()
        {
            bool destroySkill = aliveTimer > aliveTime;
    
            if (destroySkill)
            {
                PoolManager.instance.ReturnIllusion(this);
                onIllusionWasDead(this);
    
                return;
            }
        }
    
    
        // 환영과 몬스터와 충돌하면 데미지를 입는다
        protected virtual void OnCollisionStay2D(Collision2D collision)
        {
            bool isNotDamageObject = collision.gameObject.tag == "Obstacle" ||
                collision.gameObject.tag == "ClearWall" || collision.gameObject.tag == "Upper_Wall";
    
            if (isNotDamageObject) // 장애물과 충돌한거면 데미지 안입음 
                return;

            if (collision.gameObject.CompareTag("Player"))
            {
                // 플레이어와의 충돌은 무시
                Physics2D.IgnoreCollision(collision.collider, GetComponent<Collider2D>());
                return;
            }

            if (!isIllusionDead)
            {
                bool isHitDelayOK = hitDelayTimer >= hitDelayTime;
                if (isHitDelayOK)
                {
                    hp -= 1; // Damage 수동으로 1 로 설정
                    hitCount++; // 충돌 횟수 증가

                    // 5번 충돌마다 데미지 텍스트를 표시
                    if (hitCount % hitsPerShowDamage == 0)
                    {
                        InGameTextManager.Instance.ShowText("속았지?", "Legendary", false, transform.position);
                    }

                    AudioManager.instance.PlaySfx(AudioManager.Sfx.Melee0);
                    hitDelayTimer = 0;
                }

                if (hp <= 0)
                {
                    isIllusionDead = true;
                    PoolManager.instance.ReturnIllusion(this);
                    onIllusionWasDead(this);
                }
            }
        }
    }
}