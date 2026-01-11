using UnityEngine;
using UnityEngine.SceneManagement;

namespace Eclipse.Game
{
    public class Normal_Chest : Item
    {
        public RewardManager rewardManager;

        BoxCollider2D boxCollider2D;

        Animator animator;
        AudioSource audioSource;
        bool isPlayerRange;
        string sceneName;
        
        protected override void Awake()
        {
            base.Awake();

            animator = GetComponent<Animator>();
            audioSource = GetComponent<AudioSource>();
            boxCollider2D = GetComponent<BoxCollider2D>();
            isPlayerRange = false;
            rewardManager = FindAnyObjectByType<RewardManager>();
            sceneName = SceneManager.GetActiveScene().name;
        }

        protected override void OnEnable()
        {
            // 실행 하지 않음
        }

        void Update()
        {
            NormalChestOpen();

            if (animator.GetCurrentAnimatorStateInfo(0).IsName("Normal_Chest_Open"))
            {
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
                {
                    animator.ResetTrigger("Open");

                    Destroy(gameObject);
                    CreatePotionEXPCoin();
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.tag == "Player")
            {
                isPlayerRange = true;
                transform.Find("Text").gameObject.SetActive(true);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.tag == "Player")
            {
                isPlayerRange = false;
                transform.Find("Text").gameObject.SetActive(false);
            }
        }

        private void NormalChestOpen()
        {
            if (isPlayerRange)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    boxCollider2D.enabled = false; // 상자와 충돌하지 않게 하기
                    animator.SetTrigger("Open");
                    audioSource.Play();
                }
            }
        }

        private void CreatePotionEXPCoin()
        {
            switch (sceneName)
            {
                case "Stage1":
                    for (int i = 0; i < 10; i++)
                        rewardManager.ExpSpawn_By_GameObject(0, 6, gameObject);
                    for (int i = 0; i < 10; i++)
                        rewardManager.CoinSpawn_By_GameObject(0, UnityEngine.Random.Range(10, 50), gameObject);
                    break;
                case "Stage2":
                    for (int i = 0; i < 20; i++)
                        rewardManager.ExpSpawn_By_GameObject(1, 10, gameObject);
                    for (int i = 0; i < 10; i++)
                        rewardManager.CoinSpawn_By_GameObject(1, UnityEngine.Random.Range(50, 70), gameObject);
                    break;
                case "Stage3":
                    for (int i = 0; i < 20; i++)
                        rewardManager.ExpSpawn_By_GameObject(2, 20, gameObject);
                    for (int i = 0; i < 10; i++)
                        rewardManager.CoinSpawn_By_GameObject(2, UnityEngine.Random.Range(70, 100), gameObject);
                    break;
            }

            for (int i = 0; i < 5; i++)
                rewardManager.Small_HP_Potion_By_GameObject(2, gameObject);
        }
    }
}