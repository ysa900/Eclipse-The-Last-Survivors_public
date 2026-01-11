using UnityEngine;
using System;

namespace Eclipse.Game
{
    public class Shadow : Assassin_Illusion
    {
        public Action<bool> onShadowStateChanged;

        public float speed;
        private bool isShadowActive;

        public new void Init()
        {
            isPlayerLookLeft = false;

            aliveTimer = 0f;

            isShadowActive = true;
            onShadowStateChanged?.Invoke(isShadowActive);

            rigid.constraints = RigidbodyConstraints2D.FreezeRotation;
        }

        // 물리 연산 프레임마다 호출되는 생명주기 함수
        protected new void FixedUpdate()
        {
            MoveWithPlayer();

            bool isFinish = aliveTimer > aliveTime;

            if (isFinish)
            {
                PoolManager.instance.ReturnIllusion(this);

                isShadowActive = false;
                onShadowStateChanged?.Invoke(isShadowActive);
                return;
            }
        }

        // 프레임이 끝나기 직전에 실행되는 함수
        private void LateUpdate()
        {
            // 애니메이션에 속도 반영
            float playerSpeed = PlayerManager.player.inputVec.magnitude;
            GetComponent<Animator>().SetFloat("Speed", playerSpeed);

            // 방향 동기화
            Vector2 inputDirection = PlayerManager.player.inputVec;

            // X축 입력값이 있는 경우에만 방향 변경
            if (inputDirection.x != 0)
            {
                isPlayerLookLeft = inputDirection.x < 0; // 음수면 왼쪽, 양수면 오른쪽
            }

            // Sprite 반전
            spriteRenderer.flipX = isPlayerLookLeft;
        }

        // 환영분신술 용(ex. 쉐도우 파트너) 함수
        private void MoveWithPlayer()
        {
            // 플레이어의 현재 위치 가져오기
            Vector2 playerPosition = PlayerManager.player.transform.position;

            // 분신을 플레이어의 x축 왼쪽으로 이격시키기 위한 오프셋
            float xOffset = -0.3f; // 분신이 얼마나 이격될지를 결정. 필요에 따라 조정 가능
            Vector2 offset = new Vector2(xOffset, 0); // x축으로 이격시키기 위한 오프셋 벡터

            // Shadow 오브젝트의 위치를 플레이어 위치 + 오프셋으로 설정
            rigid.MovePosition(playerPosition + offset);
        }

        public void SetShadowPosition()
        {
            // 플레이어의 현재 위치 가져오기
            Vector2 playerPosition = PlayerManager.player.transform.position;

            // 분신을 플레이어의 x축 왼쪽으로 이격시키기 위한 오프셋
            float xOffset = -0.3f; // 분신이 얼마나 이격될지를 결정. 필요에 따라 조정 가능
            Vector2 offset = new Vector2(xOffset, 0); // x축으로 이격시키기 위한 오프셋 벡터

            // Shadow 오브젝트의 위치 설정
            transform.position = playerPosition + offset;
        }


        protected new void OnCollisionStay2D(Collision2D collision)
        {
            // 지우면 안됨
        }
    }
}
