using UnityEngine;

namespace Eclipse.Game
{
    public class AttackStatue : Statue, IDamageable
    {
        GameObject statueFlamePrefab;  // 석상 불꽃 프리팹
        GameObject statueFlame;  // 석상 불꽃

        protected override void Awake()
        {
            base.Awake();
            // 시각 표시 오브젝트 초기화
            statueFlamePrefab = Resources.Load<GameObject>("Prefabs/Map Objects/Stage3/StatueFlame");
        }

        private void Start()
        {
            // 공격 석상이 될 시 시각적 표시 추가
            statueFlame = Instantiate(statueFlamePrefab);
            statueFlame.transform.SetParent(transform, false);

            maxHp = 30000;
            hp = maxHp;
        }

        // IDamageable
        public void TakeDamage(string causerTag, float damage, bool isCritical = false, float knockbackForce = 0)
        {
            if (isStatueAlreadyDead) return; // 석상이 이미 죽었으면 데미지 받지 않음
            hp -= damage; // 데미지를 입음
            InGameTextManager.Instance.ShowText(Mathf.RoundToInt(damage).ToString(), causerTag, isCritical, transform.position);

            if (hp <= 0)
            {
                isStatueAlreadyDead = true; // 석상이 이미 죽었음을 표시
                Die();
            }
        }

        public void Die()
        {
            onDestroied(this); // 죽을 때 알려주기 (모든 석상이 다 파괴되면 기믹 파훼)
            DestroyComponents();
            PlaySoundOnDestroy();
        }

        void DestroyComponents()
        {
            Destroy(statueFlame);
            Destroy(transform.Find(lineRendererObjectName).gameObject); // LineRenderer 자식 오브젝트를 제거
            Destroy(GetComponent<AttackStatue>()); // AttackStatue를 제거

            Destroy(hpBar);
        }
    }
}