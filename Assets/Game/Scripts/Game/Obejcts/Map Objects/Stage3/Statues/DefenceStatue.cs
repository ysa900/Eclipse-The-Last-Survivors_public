using UnityEngine;

namespace Eclipse.Game
{
    public class DefenceStatue : Statue, IDamageable
    {
        private bool isStatueInvincible; // 석상의 무적 상태를 나타내는 변수

        GameObject statueShieldPrefab; // 석상 쉴드 프리팹
        GameObject statueShield; // 석상 쉴드

        protected override void Awake()
        {
            base.Awake();
            // 시각 표시 오브젝트 초기화
            statueShieldPrefab = Resources.Load<GameObject>("Prefabs/Map Objects/Stage3/StatueShield");
        }

        private void Start()
        {
            // 방어 석상이 될 시 시각적 표시 추가
            statueShield = Instantiate(statueShieldPrefab);
            statueShield.transform.SetParent(transform, false);

            isStatueInvincible = true; // 초기 무적상태 On
            maxHp = 30000;
            hp = maxHp;
        }

        // IDamageable
        public void TakeDamage(string causerTag, float damage, bool isCritical = false, float knockbackForce = 0)
        {
            if (isStatueAlreadyDead) return; // 이미 죽은 석상이라면 데미지 입지 않음

            if (isStatueInvincible)
            {
                damage = 0; // 무적 상태일때는 0데미지만 들어감
                InGameTextManager.Instance.ShowText("무적", "Default", isCritical, transform.position);
                return; // 무적 상태라면 데미지 입지 않음
            }

            hp -= damage; // 데미지를 입음
            InGameTextManager.Instance.ShowText(Mathf.RoundToInt(damage).ToString(), causerTag, isCritical, transform.position);

            if (hp <= 0)
            {
                isStatueAlreadyDead = true; // 석상이 죽었음을 표시
                Die();
            }
        }

        public void Die()
        {
            onDestroied(this); // 죽을 때 보스 보호막 중첩 감소
            DestroyComponents();
            PlaySoundOnDestroy();
        }

        public void DestroyComponents()
        {
            Destroy(statueShield);
            Destroy(transform.Find(lineRendererObjectName).gameObject); // LineRenderer 자식 오브젝트를 제거
            Destroy(GetComponent<DefenceStatue>()); // DefenceStatue를 제거

            Destroy(hpBar);
        }

        public void ActivateShield()
        {
            statueShield.gameObject.SetActive(true); // 석상 쉴드 활성화
            isStatueInvincible = true; // 무적 상태 활성화
        }
        public void DeactivateShield()
        {
            statueShield.gameObject.SetActive(false); // 석상 쉴드 비활성화
            isStatueInvincible = false; // 무적 상태 비활성화
        }
    }
}