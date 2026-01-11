using UnityEngine;

namespace Eclipse.Game
{
    public class PoisonPotion : CursorTrackingSkill
    {
        private float velocity;
        private float time;
        private float elapsedTime;
        private float theta;
        // [포물선 운동 관련]
        public float potionAliveTime; // 독병 날라가는 시간
        /*
         * 포물선 운동
         속도(velocity) :
        - 속도가 커지면 포물선의 길이와 높이가 모두 증가. 즉, 스킬이 더 멀리 날아가고 더 높은 궤적 생성
        - 속도를 줄이면, 포물선의 길이와 높이가 모두 줄어들어서 더 짧고 낮은 궤적 생성
        중력(gravity) :
        - 중력이 커지면 포물선이 더 가파르게 내려오고, 아래로 빨리 떨어짐.
        - 중력을 줄이면, 포물선이 더 완만해지고 천천히 떨어지는 궤적을 그림.
        */
        private float gravity = 14f;  // 중력 가속도 조절 수치
    
        public override void Init()
        {
            base.Init();

            elapsedTime = 0f;
        }
    
        private void FixedUpdate()
        {
            bool destroySkill = aliveTimer > potionAliveTime;
    
            if (destroySkill)
            {
                // 독 장판
                CursorTrackingSkill cursorTrackingSkill = PoolManager.instance.GetSkill(3) as AcidSwamp; // 생성

                // 늪 생성 위치 보정
                cursorTrackingSkill.X = X;
                cursorTrackingSkill.Y = Y + 0.5f;
    
                cursorTrackingSkill.isDotDamageSkill = true; // 스킬 유형 : 도트 데미지 스킬
                cursorTrackingSkill.AliveTime = aliveTime; // 스킬의 지속 시간을 설정
                cursorTrackingSkill.Damage = damage; // 스킬의 데미지를 설정
                cursorTrackingSkill.CriticalChance = criticalChance; // 스킬의 크리티컬 확률을 설정
                cursorTrackingSkill.CriticalMultiplier = criticalMultiplier; // 스킬의 크리티컬 배율을 설정
                cursorTrackingSkill.skillIndex = skillIndex;
    
                Transform parent = cursorTrackingSkill.transform.parent; // 스킬의 크기를 설정
    
                cursorTrackingSkill.transform.parent = null;
                cursorTrackingSkill.transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, 0);
                cursorTrackingSkill.transform.parent = parent;
                cursorTrackingSkill.onSkillAttack = this.onSkillAttack;
    
                elapsedTime = 0f;
    
                // 밸런스 때 생각
                if (onSkillFinished != null)
                    onSkillFinished(skillIndex); // skillManager에게 delegate로 알려줌
    
                PoolManager.instance.ReturnSkill(this, returnIndex);

                AudioManager.instance.PlaySfx(AudioManager.Sfx.PoisonPotion);
                return;
            }
            else
            {
                MoveToCursor();
            }
        }
    
        protected new void MoveToCursor()
        {
            elapsedTime += Time.fixedDeltaTime;
    
            // 포물선 운동 계산
            float x = velocity * Mathf.Cos(theta) * elapsedTime;
            float y = velocity * Mathf.Sin(theta) * elapsedTime - 0.5f * gravity * Mathf.Pow(elapsedTime, 2);
    
            Vector2 vector2 = new Vector2(x, y);
            Vector2 newPosition = startPosition + vector2;
            rigid.MovePosition(newPosition);

            // 회전 모션 추가 (독병 굴러가는 효과)
            float rotationSpeed = 540f; // 회전 속도 조정 (도 단위)
            transform.Rotate(0, 0, rotationSpeed * Time.fixedDeltaTime); // Z축을 기준으로 회전

            X = transform.position.x;
            Y = transform.position.y;
        }
    
        protected override void OnTriggerEnter2D(Collider2D collision)
        {
            // 아무것도 하지 않기
        }
    
        public void CalculateVariable()
        {
            aliveTime = potionAliveTime; // 독병 체공시간 대입
            time = aliveTime;

            // x축과 y축 속도 계산
            float Velo_x = (endPosition.x - startPosition.x) / time;
            float Velo_y = ((endPosition.y - startPosition.y) + (0.5f * gravity * Mathf.Pow(time, 2))) / time;
            
            // 총 속도 계산
            velocity = Mathf.Sqrt(Mathf.Pow(Velo_x, 2) + Mathf.Pow(Velo_y, 2));
            theta = Mathf.Atan2(Velo_y, Velo_x); // 각도를 속도 성분으로부터 계산
        }
    }
}