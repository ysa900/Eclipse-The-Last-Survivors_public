using UnityEngine;
namespace Eclipse.Game
{
    public class CursorTrackingSkill : Skill, IPoolingObject
    {
        protected Camera cam;
        protected Transform targetTransform;

        public Rigidbody2D rigid;
        public Vector2 direction;
        public Vector2 mousePosition;

        protected Vector2 startPosition;
        public Vector2 endPosition;
    
        protected override void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();
            cam = FollowCam.instance.transform.GetComponent<Camera>();
            targetTransform = PlayerManager.player.transform; // 플레이어의 Transform을 가져옴

            base.Awake();
        }

        public override void Init()
        {
            base.Init();

            // 현재 마우스 위치를 가져옴
            GetMousePosition();

            // 시작 위치는 플레이어 위치
            startPosition = targetTransform.position;
            transform.position = startPosition;
        }

        protected void GetMousePosition()
        {
            // Step 1: 현재 마우스 위치 (스크린 좌표)
            Vector3 screenPos = Input.mousePosition;

            // Step 2: 뷰포트 좌표로 변환 (0~1 사이로 정규화)
            Vector3 viewportPos = cam.ScreenToViewportPoint(screenPos);

            // Step 3: clamp to viewport range
            viewportPos.x = Mathf.Clamp01(viewportPos.x);
            viewportPos.y = Mathf.Clamp01(viewportPos.y);

            // Step 4: 다시 월드 좌표로 변환
            mousePosition = cam.ViewportToWorldPoint(viewportPos);
        }

        protected virtual void MoveToCursor()
        {
            Vector2 targetPosition = rigid.position + (direction * speed * Time.fixedDeltaTime);
            rigid.MovePosition(targetPosition);
    
            X = transform.position.x;
            Y = transform.position.y;
        }
    }
}