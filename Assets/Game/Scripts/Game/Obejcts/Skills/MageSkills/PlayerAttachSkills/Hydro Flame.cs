using System.Collections;
using UnityEngine;

namespace Eclipse.Game
{
    public class HydroFlame: PlayerAttachSkill
    {
        Camera cam;

        private float delay = 0.4f;
        private float delayTimer = 0f;
    
        public float radius; // Scale을 통해 조정할 수 있는 반지름

        bool isCorrutineNow = false;

        protected override void Awake()
        {
            base.Awake();

            cam = FollowCam.instance.transform.GetComponent<Camera>();
        }

        public override void Init()
        {
            delayTimer = 0;
            isCorrutineNow = false;

            base.Init();
        }
    
        protected override void Update()
        {
            bool destroySkill = aliveTimer > aliveTime && !isCorrutineNow;

            if (destroySkill)
            {
                StartCoroutine(Disappear());

                return;
            }
            else
            {
                AttachPlayer();
            }

            if (delay <= delayTimer)
            {
                base.Update();
            }
        }

        protected override void LateUpdate()
        {
            base.LateUpdate();

            delayTimer += Time.deltaTime;
        }

        new void AttachPlayer()
        {
            if (Time.timeScale == 0) return;

            Vector2 direction = CalculateMouseDirection();
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
    
            Quaternion angleAxis = Quaternion.AngleAxis(angle - 90f, Vector3.forward);
            Quaternion rotation = Quaternion.Slerp(transform.rotation, angleAxis, 5f);
            transform.rotation = rotation;

            Vector2 playerPosition = PlayerManager.player.transform.position;
            X = playerPosition.x + Mathf.Cos(angle * Mathf.Deg2Rad) * radius;
            Y = playerPosition.y + Mathf.Sin(angle * Mathf.Deg2Rad) * radius;
        }

        private Vector2 CalculateMouseDirection()
        {
            Vector2 mousePosition = Input.mousePosition;
            mousePosition = cam.ScreenToWorldPoint(mousePosition);

            Vector2 direction = (mousePosition - (Vector2)PlayerManager.player.transform.position).normalized;

            return direction;
        }

        private IEnumerator Disappear()
        {
            isCorrutineNow = true;
            animator.SetTrigger("Finish");
    
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).IsName("Hydro Flame_finish"));
    
            StartCoroutine(Return());
        }
    
        private IEnumerator Return()
        {
            yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f);
    
            if (onSkillFinished != null)
                onSkillFinished(skillIndex); // skillManager���� delegate�� �˷���
    
            PoolManager.instance.ReturnSkill(this, returnIndex);
        }
    }
}