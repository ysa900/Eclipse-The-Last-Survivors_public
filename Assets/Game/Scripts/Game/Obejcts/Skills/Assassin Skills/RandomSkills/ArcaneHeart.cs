using UnityEngine;

namespace Eclipse.Game
{
    public class ArcaneHeart : RandomSkill
    {
        //=========================================================
        public Assassin_Illusion assassin_Illusion;
        [SerializeField] private IllusionData illusionData;
        public int level;

        //=========================================================
        private CapsuleCollider2D capsuleCollider2D;

        //=========================================================
        // 환영이 사라질 시, A_SkillManager에 알려주기 위한 delegate
        public delegate void OnIllusionWasRemoved(Assassin_Illusion assassin_Illusion);
        public OnIllusionWasRemoved onIllusionWasRemoved;
        //=========================================================

        protected override void Awake()
        {
            base.Awake();
            capsuleCollider2D = GetComponent<CapsuleCollider2D>();
        }

        protected override void Update()
        {
            if (animator.GetCurrentAnimatorStateInfo(0).IsName("ArcaneHeart_Start"))
            {
                capsuleCollider2D.enabled = false; // 처음 시전 시에는 Collider 끄기
            }
            else
            {
                if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.9f)
                {
                    capsuleCollider2D.enabled = false; // 환영 설치기 모션 끝나면 다시 Collider 끄기
                    PoolManager.instance.ReturnSkill(this, returnIndex);
                    return;
                }
                else
                {
                    capsuleCollider2D.enabled = true; // 환영 설치기 모션 동안 Collider 켜기
                }
            }
    
            base.Update();
        }
    
        // 분신 사라질 때 하는 함수
        private void OnIllusionWasDead(Assassin_Illusion assassin_Illusion)
        {
            // 환영 HP 다 닳면 해당 자리에서 폭발하게 만들기
            MakeExplosion();
    
            PoolManager.instance.ReturnSkill(this, returnIndex);
            
            onIllusionWasRemoved(assassin_Illusion);
        }
    
        public void MakeIllusion()
        {
            illusionData.speed = 0f;
            illusionData.hp = 50f;
            illusionData.maxHp = 50f;
    
            // 환영 생성 및 초기화
            assassin_Illusion = PoolManager.instance.GetIllusion(0); // 환영 허수아비
            assassin_Illusion.transform.localScale = new Vector3(1.2f, 1.2f, 0);
            assassin_Illusion.aliveTime = aliveTime;
            assassin_Illusion.Init();

            assassin_Illusion.onIllusionWasDead = OnIllusionWasDead;
        }
    
        private void MakeExplosion()
        {
            RandomSkill randomSkill = PoolManager.instance.GetSkill(15) as ArcaneHeart_Main;
    
            // 분신 위치에서 터지게 ..
            randomSkill.X = X;
            randomSkill.Y = Y;

            randomSkill.skillIndex = skillIndex;
            randomSkill.isDotDamageSkill = true;
            randomSkill.AliveTime = aliveTime - 0.5f;
            randomSkill.Damage = damage * 2; // 데미지 정상화, 환영 생성 시 설정된 데미지에 절반 들어가게 해놨기에 ..
            randomSkill.CriticalChance = criticalChance;
            randomSkill.CriticalMultiplier = criticalMultiplier;

            Transform parent = randomSkill.transform.parent; // 환영 크기 설정
    
            randomSkill.transform.parent = null;
            randomSkill.transform.localScale = new Vector3(1f, 1f, 0);
            randomSkill.transform.parent = parent;
    
            randomSkill.onSkillAttack = onSkillAttack;
        }
    
        public void SettingIllusion()
        {
            if (level == 5)
            {
                assassin_Illusion.transform.position = new Vector2(X, Y - 0.75f);
            }
            else
            {
                assassin_Illusion.transform.position = new Vector2(X, Y - 0.5f);
            }
        }
    }
}