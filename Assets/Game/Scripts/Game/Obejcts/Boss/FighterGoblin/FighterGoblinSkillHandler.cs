using System;
using UnityEngine;

namespace Eclipse.Game
{
    public class FighterGoblinSkillHandler : MonoBehaviour
    {
        //==================================================================
        FighterGoblin boss;

        //==================================================================
        // 고블린 투사 스킬들
        AttackHitBox fighterGoblinPunchFirstAttack;    // 0번, 펀치 첫번째 공격
        AttackHitBox fighterGoblinPunchSecondAttack;   // 1번, 펀치 두번째 공격
        AttackHitBox fighterGoblinSmashAttack;         // 2번, 찍기 공격
        AttackHitBox fighterGoblinJumpAttack;          // 3번, 점프 후 착지 (범위)공격

        /*
         0, 1번 : 양손 펀치(2타) 각 데미지 = 2
         2번 :    찍기 데미지 = 5
         3번 :    점프 후 착지 데미지 = 10
        */

        // 우선 임의 지정, 애니메이션 프레임 확인해서 하나씩 맞추기
        float[] hitBoxAliveTime =
        {
            4 * AnimationConstants.FrameTime, // 0번, 펀치 첫번째 공격
            6 * AnimationConstants.FrameTime, // 1번, 펀치 두번째 공격
            3 * AnimationConstants.FrameTime, // 2번, 찍기 공격
            3 * AnimationConstants.FrameTime  // 3번, 점프 후 착지 공격
        };
        float[] hitBoxDamages = { 2f, 2f, 5f, 10f };
        public float nightmareDamageCoefficient; // nightmare 난이도 데미지 계수

        //==================================================================
        // 액션 모음
        public Action<int, int, Vector2> onSpawnGimmicEnemies;
        public Action<int, Vector2> onSpawnGimmicPoisonSwamp;

        private void Awake()
        {
            boss = GetComponent<FighterGoblin>();
        }

        private void Start()
        {
            AttackHitBox[] hitBoxes = GetComponentsInChildren<AttackHitBox>();

            // 히트박스 공통 변수 초기화
            InitializeAttackHitBox(out fighterGoblinPunchFirstAttack, hitBoxes, 0);
            InitializeAttackHitBox(out fighterGoblinPunchSecondAttack, hitBoxes, 1);
            InitializeAttackHitBox(out fighterGoblinSmashAttack, hitBoxes, 2);
            InitializeAttackHitBox(out fighterGoblinJumpAttack, hitBoxes, 3);

            FighterGoblinInit();
        }

        private void InitializeAttackHitBox(out AttackHitBox hitBox, AttackHitBox[] hitBoxes, int index)
        {
            hitBox = hitBoxes[index];
            hitBox.SetAliveTime(hitBoxAliveTime[index]);
            hitBox.damage = hitBoxDamages[index] * nightmareDamageCoefficient;
            hitBox.boss = boss;
            hitBox.DisableHitbox();
        }

        private void FighterGoblinInit()
        {
            boss.onFirstPunchAttack = OnFirstPunchAttack;
            boss.onSecondPunchAttack = OnSecondPunchAttack;
            boss.onSmashAttack = OnSmashAttack;
            boss.onJumpAttack = OnJumpAttack;
            boss.onBossChangeDamages = OnBossChangeDamages;       
        }

        void OnFirstPunchAttack()
        {
            fighterGoblinPunchFirstAttack.Init();
        }
        void OnSecondPunchAttack()
        {
            fighterGoblinPunchSecondAttack.Init();
        }
        void OnSmashAttack()
        {
            fighterGoblinSmashAttack.Init();
        }
        void OnJumpAttack()
        {
            fighterGoblinJumpAttack.Init();
        }

        void OnBossChangeDamages(float changeCoefficient)
        {
            for (int i = 0; i < hitBoxDamages.Length; i++)
            {
                hitBoxDamages[i] *= changeCoefficient;
            }
        }
    }
}
