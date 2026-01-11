using UnityEngine;
using UnityEngine.SceneManagement;

namespace Eclipse.Game
{
    public class RePositon : MonoBehaviour
    {
        public GameObject clearWall;

        Transform _playerTransform;
        Transform playerTransform
        {
            get
            {
                if (_playerTransform == null)
                {
                    _playerTransform = PlayerManager.player.transform;
                }
                return _playerTransform;
            }
        }

        int _playerAreaSize;
        int playerAreaSize
        {
            get
            {
                if (_playerAreaSize == 0)
                {
                    _playerAreaSize = (int)PlayerManager.player.GetComponentInChildren<BoxCollider2D>().size.x;
                }
                return _playerAreaSize;
            }
        }

        bool isStage2TimeOver;
        int GroundSize = 32;


        private void Start()
        {
            // clearWall의 오른쪽 끝 좌표를 GameManger를 통해 FollowCam에게 전달 
            if (clearWall != null) SendClearWall_RightX();
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            // 플레이어의 Area와 충돌을 감지해 벗어났다면 실행
            // 플레이어 프리팹 하위에 Area라는게 있음 
            if (!collision.CompareTag("Area"))
            {
                return;
            }

            Vector3 playerPosition = playerTransform.position;
            Vector3 myPosition = transform.position;
            float diffX = Mathf.Abs(playerPosition.x - myPosition.x);
            float diffY = Mathf.Abs(playerPosition.y - myPosition.y);

            float dirtionX = myPosition.x < playerPosition.x ? 1 : -1; // playerDirection이 마이너스면 -1, 플러스면 1
            float dirtionY = myPosition.y < playerPosition.y ? 1 : -1;

            switch (transform.tag)
            {
                case "Ground":
                    if (diffX > playerAreaSize && diffY > playerAreaSize)
                    {
                        transform.Translate(Vector3.right * dirtionX * GroundSize); //대각선 방향으로 오른쪽, 왼쪽 위 아래 방향으로 이동.
                        transform.Translate(Vector3.up * dirtionY * GroundSize);
                        Debug.Log($"이름: {name}, diffX: {diffX}, diffY: {diffY}");
                    }
                    else if (diffX > diffY)
                    {
                        transform.Translate(Vector3.right * dirtionX * GroundSize); // 오른쪽 방향 * (-1 or 1) * 거리
                    }
                    else if (diffX < diffY)
                    {
                        transform.Translate(Vector3.up * dirtionY * GroundSize);// 윗 방향 * (-1 or 1) * 거리
                    }
                    break;

                case "Corridor":
                    // Stage2에서 3분이 지나면 Reposition이 멈추고, 보스 방으로 가는 길이 열려야 함
                    isStage2TimeOver = StageManager.instance?.gameTime >= StageManager.instance?.maxGameTime;

                    if (isStage2TimeOver) break;

                    if (playerPosition.x >= transform.position.x)
                    {
                        float width = GetComponent<BoxCollider2D>().size.x / 5;
                        transform.Translate(Vector3.right * width * 2); // 오른쪽 방향 * 거리
                        clearWall.transform.Translate(Vector3.right * width);
                        dirtionX = 1;
                        SendClearWall_RightX();
                    }
                    break;
            }
        }

        // clearWall의 오른쪽 끝 좌표를 FollowCam에게 전달 
        void SendClearWall_RightX()
        {
            float clearWall_RightX = clearWall.transform.position.x + clearWall.transform.localScale.x / 2 / 5; // 나누기 5는 축소한 grid 사이즈, transform.position에는 반영 됨
            FollowCam.instance.clearWall_RightEndX = clearWall_RightX;
        }
    }
}
