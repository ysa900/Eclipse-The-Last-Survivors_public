using System.Collections;
using UnityEngine;

namespace Eclipse.Game
{
    public class AttackHitBox : BossSkill
    {
        Collider2D hitBoxCollider;

        private void Awake()
        {
            hitBoxCollider = GetComponent<Collider2D>();
        }

        public void SetAliveTime(float time)
        {
            aliveTime = time;
        }

        public override void Init()
        {
            if (!gameObject.activeSelf) return;
            StartCoroutine(DisableAfterSeconds(aliveTime));
            hitBoxCollider.enabled = true;
        }

        private IEnumerator DisableAfterSeconds(float time)
        {
            yield return new WaitForSeconds(time);
            hitBoxCollider.enabled = false;
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            IPlayer iPlayer = collision.GetComponent<IPlayer>();

            if (iPlayer == null)
            {
                return;
            }

            iPlayer.TakeDamageOneTime(damage);
        }

        public void DisableHitbox()
        {
            hitBoxCollider.enabled = false;
        }
    }
}