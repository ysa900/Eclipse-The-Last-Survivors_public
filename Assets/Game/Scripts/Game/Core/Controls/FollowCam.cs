using UnityEngine;
using UnityEngine.SceneManagement;

namespace Eclipse.Game
{
    public class FollowCam : MonoBehaviour
    {
        public static FollowCam instance;

        private Transform playerTransform;

        string sceneName;
        public float cameraHalfWidth, cameraHalfHeight;
        [SerializeField][ReadOnly] bool isClearWallDetected;
        public float clearWall_RightEndX = 0;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            cameraHalfWidth = Camera.main.aspect * Camera.main.orthographicSize;
            cameraHalfHeight = Camera.main.orthographicSize;
        }

        private void Start()
        {
            sceneName = SceneManager.GetActiveScene().name;
            playerTransform = PlayerManager.player.transform;
        }

        private void FixedUpdate()
        {
            // 실시간 화면비에 따라 반영되도록 업데이트
            cameraHalfWidth = Camera.main.aspect * Camera.main.orthographicSize;
            cameraHalfHeight = Camera.main.orthographicSize;

            if (sceneName == "Stage2")
            {
                isClearWallDetected = (transform.position.x - cameraHalfWidth <= clearWall_RightEndX) &&
                    (playerTransform.position.x - cameraHalfWidth < clearWall_RightEndX);
            }

            Vector2 newCamPosition;
            if (isClearWallDetected)
            {
                newCamPosition = new Vector2(transform.position.x, playerTransform.position.y);
            }
            else
            {
                newCamPosition = new Vector2(playerTransform.position.x, playerTransform.position.y);
            }

            transform.position = new Vector3(newCamPosition.x, newCamPosition.y, transform.position.z);
        }
    }
}