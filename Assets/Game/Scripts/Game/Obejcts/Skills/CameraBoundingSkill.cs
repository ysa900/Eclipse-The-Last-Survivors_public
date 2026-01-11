using UnityEngine;

namespace Eclipse.Game
{
    public class CameraBoundingSkill : Skill
    {
        FollowCam followCam;
        private Vector2 direction; // 현재 이동 방향

        public override void Init()
        {
            base.Init();
            followCam = FollowCam.instance;

            // 카메라 중심 좌표로 위치 설정
            Vector2 cameraCenter = followCam.transform.position;
            transform.position = cameraCenter;

            // 랜덤한 방향으로 이동 시작
            direction = Random.insideUnitCircle.normalized;
        }

        protected override void Update()
        {
            base.Update();

            BoundingCameraRange();
        }

        protected void BoundingCameraRange()
        {
            // 카메라 중심 좌표와 크기 가져오기
            Vector2 cameraCenter = followCam.transform.position;
            float cameraHalfWidth = followCam.cameraHalfWidth;
            float cameraHalfHeight = followCam.cameraHalfHeight;

            // 현재 위치
            Vector2 currentPosition = transform.position;

            // 범위를 벗어났는지 확인
            if (currentPosition.x < cameraCenter.x - cameraHalfWidth || currentPosition.x > cameraCenter.x + cameraHalfWidth)
            {
                // X축 방향 반사
                direction.x = -direction.x;
                currentPosition.x = Mathf.Clamp(currentPosition.x, cameraCenter.x - cameraHalfWidth, cameraCenter.x + cameraHalfWidth);
            }

            if (currentPosition.y < cameraCenter.y - cameraHalfHeight || currentPosition.y > cameraCenter.y + cameraHalfHeight)
            {
                // Y축 방향 반사
                direction.y = -direction.y;
                currentPosition.y = Mathf.Clamp(currentPosition.y, cameraCenter.y - cameraHalfHeight, cameraCenter.y + cameraHalfHeight);
            }

            // 위치 갱신
            transform.position = currentPosition + direction * speed * Time.deltaTime;
        }
    }
}
