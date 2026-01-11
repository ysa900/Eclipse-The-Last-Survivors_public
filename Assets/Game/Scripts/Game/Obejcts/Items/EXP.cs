using UnityEngine;

namespace Eclipse.Game
{
    public class EXP : Item, IPoolingObject
    {
        float followSpeed = 5f;

        public int expAmount;
        private int stageEndExpAmount;
        public int index;
    
        bool isInMagnetRange; // Exp가 자석 범위 안에 있나
        bool oldState; // 이전 상태 (자석 범위 안에 있는지 여부)

        Transform playerTransform; // 플레이어 Transform
        PlayerData playerData;

        protected override void Awake()
        {
            base.Awake();
            playerTransform = PlayerManager.player.transform; // 플레이어의 Transform을 가져옴
            playerData = PlayerManager.instance.playerData;
        }

        public void Init()
        {
            stageEndExpAmount = expAmount / 3;
            if (stageEndExpAmount == 0) stageEndExpAmount = 1;
            oldState = false;
            isInMagnetRange = false; // 초기 상태는 자석 범위 밖
        }

        void Update()
        {
            Vector2 playerPosition = playerTransform.position;
            Vector2 myPosition = transform.position;

            float sqrDistance = (myPosition - playerPosition).sqrMagnitude;
            isInMagnetRange = sqrDistance <= playerData.magnetRange * playerData.magnetRange;
            // 플레이어가 감지됐으면 빨려가기
            if (isInMagnetRange)
            {
                transform.position = Vector2.MoveTowards(transform.position, playerPosition, followSpeed * Time.deltaTime);
            }
            else if (oldState) // 이전 상태가 자석 범위 안에 있었고(oldState == true), 현재는 밖에 있다면(isInMagnetRange == false)
            {
                StartCoroutine(SetNonTriggerForSeconds(0.2f)); // 빨려가다 멈추면, 오브젝트 위에 생성되는 것을 막기 위함
                oldState = false; // 상태를 업데이트
            }
            oldState = isInMagnetRange;
        }
    
        private void OnTriggerEnter2D(Collider2D collision)
        {
            IPlayer iPlayer = collision.GetComponent<IPlayer>();
    
            if (iPlayer == null)
            {
                return;
            }

            if (StageManager.instance == null) return;
            if (StageManager.instance.isStageClear) expAmount = stageEndExpAmount;

            iPlayer.GetExp(expAmount);

            AudioManager.instance.PlaySfx(AudioManager.Sfx.Pickup);
            PoolManager.instance.ReturnExp(this, index);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            IPlayer iPlayer = collision.gameObject.GetComponent<IPlayer>();

            if (iPlayer == null)
            {
                return;
            }

            if (StageManager.instance == null) return;
            if (StageManager.instance.isStageClear) expAmount = stageEndExpAmount;

            iPlayer.GetExp(expAmount);

            AudioManager.instance.PlaySfx(AudioManager.Sfx.Pickup);
            PoolManager.instance.ReturnExp(this, index);
        }
    }
}