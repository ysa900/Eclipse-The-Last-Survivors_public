using UnityEngine;

namespace Eclipse.Game.Panels
{
    public class Portal_Indicator : MonoBehaviour
    {
        //==================================================================
        FollowCam _followCam;

        //==================================================================
        private RectTransform indicatorRectTransform;

        //==================================================================
        public Vector2 portalPos;
        float lengthX, lengthY;
        Vector2 direction;

        //==================================================================
        const int FHD_X = 1920;
        const int FHD_Y = 1080;

        //==================================================================

        void Start()
        {
            // 초기화
            _followCam = FollowCam.instance;

            indicatorRectTransform = GetComponent<RectTransform>();

            indicatorRectTransform.localPosition = PlayerManager.player.transform.position;
        }
        
        void Update()
        {
            // 실제 게임 내 포탈 위치 카메라 위치로 가져오기
            direction = new Vector2(portalPos.x - _followCam.transform.position.x, portalPos.y - _followCam.transform.position.y);
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    
            float labelAngle = Mathf.Atan2(FHD_Y, FHD_X) * Mathf.Rad2Deg;
    
            int signX = portalPos.x - _followCam.transform.position.x >= 0 ? 1 : -1;
            int signY = portalPos.y - _followCam.transform.position.y >= 0 ? 1 : -1;
    
            if ((angle <= 180 - labelAngle && angle > labelAngle) ||
                (angle <= -labelAngle && angle > -180 + labelAngle))
            {
                lengthX = (Mathf.Abs(direction.x) * _followCam.cameraHalfHeight) / Mathf.Abs(direction.y);
                lengthY = _followCam.cameraHalfHeight; // y축 부호 결정
            }
            else
            {
                lengthX = _followCam.cameraHalfWidth;
                lengthY = (_followCam.cameraHalfWidth * Mathf.Abs(direction.y)) / Mathf.Abs(direction.x);
            }
    
            lengthX *= signX;
            lengthY *= signY;
    
            // 비율 계산하기
            lengthX = lengthX / (2 * _followCam.cameraHalfWidth); // cameraHalfWidth: 19.2
            lengthY = lengthY / (2 * _followCam.cameraHalfHeight); // cameraHalfHeight: 10.2
    
            lengthX *= FHD_X;
            lengthY *= FHD_Y;

            float[] diffs = new float[4];
            diffs[0] = Mathf.Abs(lengthX + FHD_X / 2);
            diffs[1] = Mathf.Abs(lengthX - FHD_X / 2);
            diffs[2] = Mathf.Abs(lengthY + FHD_Y / 2);
            diffs[3] = Mathf.Abs(lengthY - FHD_Y / 2);
    
            float minDiff = Mathf.Min(diffs);

            if (minDiff == diffs[0]) lengthX += 64f;
            if (minDiff == diffs[1]) lengthX -= 64f;
            if (minDiff == diffs[2]) lengthY += 64f;
            if (minDiff == diffs[3]) lengthY -= 64f;
    
            // 캔버스 기준으로 변환
            Vector2 myPos = new Vector2(lengthX, lengthY);
    
            indicatorRectTransform.localPosition = myPos;
    
        }
    }
}