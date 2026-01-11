using UnityEngine;

namespace Eclipse.Game
{
    public class CursorIndicator : MonoBehaviour
    {
        Vector2 mousePosition;
        Vector2 playerPosition;
        Vector2 direction;
    
        float angle;
        float circleRadius; // 플레이어와의 이격 거리( 원 )
    
        public void Init()
        {
            // 현재 마우스 커서의 위치를 화면 좌표(Screen Coordinates)로 가져오는 역할
            mousePosition = Input.mousePosition; // 화면 좌표계에서 마우스 위치를 가져온다
            mousePosition = FollowCam.instance.transform.GetComponent<Camera>().ScreenToWorldPoint(mousePosition); // 이 위치를 월드 좌표계로 변환
    
            playerPosition = transform.parent.position;
            transform.position = new Vector2(playerPosition.x, playerPosition.y);
            circleRadius = 0.35f;
        }
    
        private void FixedUpdate()
        {
            if (Time.timeScale == 0) return;

            var x = circleRadius * Mathf.Cos(angle);
            var y = circleRadius * Mathf.Sin(angle);
            CalculateDirection();
    
            transform.position = playerPosition + new Vector2(x, y);
            transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg - 90);
        }
    
        private void CalculateDirection()
        {
            // 현재 마우스 커서의 위치를 화면 좌표(Screen Coordinates)로 가져오는 역할
            mousePosition = Input.mousePosition; // 화면 좌표계에서 마우스 위치를 가져온다
            mousePosition = FollowCam.instance.transform.GetComponent<Camera>().ScreenToWorldPoint(mousePosition); // 이 위치를 월드 좌표계로 변환
    
            playerPosition = PlayerManager.player.transform.position;
            direction = (mousePosition - playerPosition).normalized;
            angle = Mathf.Atan2(direction.y, direction.x);
        }
    
        public void CursorInit()
        {
            transform.position = new Vector2(playerPosition.x, playerPosition.y);
            circleRadius = 0.35f;
        }
    }
}