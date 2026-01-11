using UnityEngine;
using static Eclipse.Game.DamageHandler;

namespace Eclipse.Game
{
    public class EntropicDecay : CursorTrackingSkill
    {
        [SerializeField] private float followSpeed = 3f; // 마우스 위치를 따라가는 속도값
        
        public override void Init()
        {
            base.Init();

            transform.position = mousePosition; // 마우스 위치로 스킬 이동
        }

        protected override void Update()
        {
            bool destroySkill = aliveTimer > aliveTime;

            if (destroySkill)
            {
                if (onSkillFinished != null)
                    onSkillFinished(skillIndex); // skillManager에게 delegate로 알려줌

                PoolManager.instance.ReturnSkill(this, returnIndex);
                return;
            }
            else
            {
                TrackingCursor();
            }

            base.Update();
        }

        private void TrackingCursor()
        {
            // 매 프레임마다 마우스의 최신 좌표를 가져옴
            GetMousePosition();

            // 현재 위치에서 마우스 위치로 부드럽게 이동 (속도 적용)
            Vector2 currentPos = transform.position;
            Vector2 targetPos = mousePosition;

            // Lerp 함수로 일정 속도로 마우스 위치로 이동
            // Lerp : 선형보간 함수
            // 두 점 사이를 부드럽게 이동시키는 방법으로, 매 프레임마다 대상 위치에 가까워지도록 해주지만, 목표 위치에 도달하는 데는 시간이 걸리게 만듦
            Vector2 newPos = Vector2.Lerp(currentPos, targetPos, followSpeed * Time.deltaTime);

            transform.position = newPos; // 새로운 위치로 업데이트

            X = transform.position.x;
            Y = transform.position.y;
        }

        protected override void OnAfterDamageApplied(Collider2D collision, DamageResult damageResult)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Boss"))
            {
                var boss = collision.GetComponent<Boss>();
                if (boss != null)
                {
                    boss.compressedDamage += damageResult.FinalDamage;
                }
            }
        }
    }
}
