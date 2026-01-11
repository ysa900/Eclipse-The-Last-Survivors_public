using System.Collections;
using UnityEngine;

namespace Eclipse.Game
{
    public class Item : Object
    {
        protected Collider2D col;
        protected Rigidbody2D rb;

        protected virtual void Awake()
        {
            col = GetComponent<Collider2D>();
            rb = GetComponent<Rigidbody2D>();
        }

        protected virtual void OnEnable()
        {
            if (gameObject.activeSelf)
            {
                StartCoroutine(SetNonTriggerForSeconds(0.2f)); // 오브젝트 위에 생성되는 것을 막기 위함
            }
        }

        protected IEnumerator SetNonTriggerForSeconds(float seconds)
        {
            rb.WakeUp(); // Rigidbody를 깨워서 물리 엔진이 작동하도록 함
            col.isTrigger = false;
            rb.bodyType = RigidbodyType2D.Dynamic; // Rigidbody를 동적 모드로 설정
            yield return new WaitForSeconds(seconds);
            col.isTrigger = true;
            rb.bodyType = RigidbodyType2D.Kinematic; // Rigidbody를 키네마틱 모드로 설정
            rb.velocity = Vector2.zero; // 움직임을 멈추기 위해 속도를 0으로 설정
            rb.Sleep(); // 완전 정지 + 성능 최적화
        }
    }
}
