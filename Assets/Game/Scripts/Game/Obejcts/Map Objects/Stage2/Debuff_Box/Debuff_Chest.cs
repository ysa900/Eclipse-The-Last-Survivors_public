using TMPro;
using UnityEngine;

namespace Eclipse.Game
{
    public class Debuff_Chest : Item
    {
        public PatternManager patternManager;

        BoxCollider2D boxCollider2D;

        Animator animator;
        AudioSource audioSource;
        bool isPlayerInRange;
    
        float boxAliveTimer;
        float boxDefaultAliveTime = 20f;

        public float gameTime;
        public float maxGameTime;
    
        GameObject textObject;
        TextMeshPro timeObject;
    
        protected override void Awake()
        {
            base.Awake();
            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            boxCollider2D = GetComponent<BoxCollider2D>();
            isPlayerInRange = false;
    
            textObject = transform.Find("Text").gameObject;
            timeObject = transform.Find("Time Text").GetComponent<TextMeshPro>();
    
            textObject.SetActive(false);
            timeObject.gameObject.SetActive(true);
        }
        protected override void OnEnable()
        {
            // 실행 하지 않음
        }

        private void Start()
        {
            float timeInterval = maxGameTime - gameTime;
            if (timeInterval <= 30f + boxDefaultAliveTime)
            {
                boxAliveTimer = timeInterval - 30f;
            }
            else
            {
                boxAliveTimer = boxDefaultAliveTime;
            }
        }

        void Update()
        {
            // 디버프 상자를 열어서 패턴이 시작된 경우, 디버프 상자가 상호작용 안되게 하기
            if (patternManager.patternTimer >= 30)
                StartStage2Pattern();

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Debuff_Chest_Open"))
            {
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                {
                    animator.ResetTrigger("Open");
    
                    Destroy(gameObject);
                }
            }
    
            if (timeObject.gameObject.activeSelf)
            {
                if (boxAliveTimer < 0)
                {
                    Destroy(gameObject);
    
                    return;
                }
    
                timeObject.text = Mathf.Ceil(boxAliveTimer).ToString();
                boxAliveTimer -= Time.deltaTime;
            }
        }
        
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Player")
            {
                isPlayerInRange = true;
                textObject.SetActive(true);
                timeObject.gameObject.SetActive(false);
            }
        }
    
        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.tag == "Player")
            {
                isPlayerInRange = false;
                textObject.SetActive(false);
                timeObject.gameObject.SetActive(true);
            }
        }
        
        private void StartStage2Pattern()
        {
            if (isPlayerInRange)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    boxCollider2D.enabled = false; // 상자와 충돌하지 않게 하기
                    animator.SetTrigger("Open");
                    audioSource.Play();

                    patternManager.isStage2PatternActivated = true;
                }
            }
        }
    }
}