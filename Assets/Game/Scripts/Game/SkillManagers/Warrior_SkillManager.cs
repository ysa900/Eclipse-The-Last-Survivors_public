using UnityEngine;
using UnityEngine.SceneManagement;

namespace Eclipse.Game
{
    public class Warrior_SkillManager : SkillManager
    {
        //==================================================================
        // 캐싱용 스킬 클래스 객체
        private Skill skill;

        //Holy Skill: 0, 3, 6, 9 (Common, Rare, Epic, Legendary)
        //Blood Skill : 1, 4, 7, 10 (Common, Rare, Epic, Legendary)
        //Dark Skill:  2, 5, 8, 11 (Common, Rare, Epic, Legendary)
        //Ultimately Skill : 12 ,13, 14( Holy, Blood, Dark)

        private void Awake()
        {
            MAX_SKILL_NUM = 15;

            isSkillsCasted = new bool[MAX_SKILL_NUM];
            attackDelayTimer = new float[MAX_SKILL_NUM];
        }


        // 체인을 걸어서 이 함수는 매 씬마다 호출된다.
        protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            base.OnSceneLoaded(scene, mode);

            // 쿨타임 초기화
            for (int i = 0; i < attackDelayTimer.Length; i++)
            {
                isSkillsCasted[i] = false;
                attackDelayTimer[i] = skillData.delay[i]
                            * (1 - server_PlayerData.basicPassiveLevels[4] * server_PlayerData.cooldown);
            }
        }

        private void Update()
        {
            bool isSplashScene =
                sceneName == "Lobby" ||
                sceneName == "Splash1" ||
                sceneName == "Splash2" ||
                sceneName == "Splash3";

            if (isSplashScene) return;

            for (int i = 0; i < skillData.damage.Length; i++)
            {
                if (skillData.skillSelected[i]) // 활성화(선택)된 스킬만 실행
                {

                    bool shouldBeAttack = 0 >= attackDelayTimer[i] && !isSkillsCasted[i]; // 쿨타임이 됐는지 확인

                    if (shouldBeAttack)
                    {
                        attackDelayTimer[i] = skillData.delay[i]
                            * (1 - server_PlayerData.basicPassiveLevels[4] * server_PlayerData.cooldown);

                        if (i == 6 || i == 7)
                        {
                            for (int j = 0; j < skillData.skillCount[i]; j++)
                                TryAttack(i); // 스킬 쿨타임이 다 됐으면 공격을 시도한다

                            enemyManager.ClearNearestEnemies();
                        }
                        else
                        {
                            for (int j = 0; j < skillData.skillCount[i]; j++)
                                CastSkill(i); // 스킬 쿨타임이 다 됐으면 공격한다
                        }
                    }
                    else
                    {
                        if (!isSkillsCasted[i])
                            attackDelayTimer[i] -= Time.deltaTime;
                    }
                }
            }
        }

        // skilldata를 초기화
        public override void Init()
        {
            // Skill Data 초기화
            for (int i = 0; i < skillData.level.Length; i++) { skillData.level[i] = 0; }

            skillData.damage[0] = 70f; //Holy Common
            skillData.damage[1] = 70f; //Blood Common
            skillData.damage[2] = 24f;  //Dark Comoon (dot damage Skill)
            skillData.damage[3] = 32f;  //Holy Rare (dot damage Skll)
            skillData.damage[4] = 32f;  //Blood Rare (dot damage Skll)
            skillData.damage[5] = 32f;  //Dark Rare (dot damage Skll)
            skillData.damage[6] = 160f; //Holy Epic
            skillData.damage[7] = 96f; //Blood Epic (dot damage Skill)
            skillData.damage[8] = 48f; //Dark Epic (dot damage Skill)
            skillData.damage[9] = 72f; //Holy Legendary (dot damage Skill)
            skillData.damage[10] = 72f; //Blood Legendary (dot damage Skill)
            skillData.damage[11] = 72f; //Dark Legendary (dot damage Skill)
            skillData.damage[12] = 145f;  //Holy Ulti (dot damage Skill)
            skillData.damage[13] = 145f; //Blood Ulti (dot damage Skill)
            skillData.damage[14] = 145f; //Dark Ulti (dot damage Skill)

            skillData.delay[0] = 1.8f;
            skillData.delay[1] = 1.8f;
            skillData.delay[2] = 2.43f;
            skillData.delay[3] = 3.6f;
            skillData.delay[4] = 3.6f;
            skillData.delay[5] = 3.6f;
            skillData.delay[6] = 0.81f;
            skillData.delay[7] = 1.82f;
            skillData.delay[8] = 1.08f;
            skillData.delay[9] = 3.6f;
            skillData.delay[10] = 3.6f;
            skillData.delay[11] = 2.18f;
            skillData.delay[12] = 3.6f;
            skillData.delay[13] = 3.78f;
            skillData.delay[14] = 3.6f;

            skillData.scale[0] = 1f;    //Holy Common
            skillData.scale[1] = 1f;    //Blood Common
            skillData.scale[2] = 1.2f;    //Dark Comoon (dot damage Skill + general Skill)
            skillData.scale[3] = 1.4f;  //Holy Rare
            skillData.scale[4] = 1.3f;    //Blood Rare (dot damage Skll)
            skillData.scale[5] = 0.7f;    //Dark Rare 
            skillData.scale[6] = 0.8f;    //Holy Epic
            skillData.scale[7] = 0.8f;    //Blood Epic (dot damage Skill)
            skillData.scale[8] = 1.3f;    //Dark Epic (dot damage Skill)
            skillData.scale[9] = 1f;    //Holy Legendary (dot damage Skill)
            skillData.scale[10] = 1.5f;   //Blood Legendary (dot damage Skill)
            skillData.scale[11] = 1f;   //Dark Legendary (dot damage Skill)
            skillData.scale[12] = 5.5f;   //Holy Ulti 
            skillData.scale[13] = 5.5f;   //Blood Ulti 
            skillData.scale[14] = 11.7f; //Dark Ulti 

            skillData.aliveTime[0] = 1f;
            skillData.aliveTime[1] = 1.2f;
            skillData.aliveTime[2] = 3f;
            skillData.aliveTime[3] = 8f;
            skillData.aliveTime[4] = 4f;
            skillData.aliveTime[5] = 3f;
            skillData.aliveTime[6] = 1.2f;
            skillData.aliveTime[7] = 1.78f;
            skillData.aliveTime[8] = 2.3f;
            skillData.aliveTime[9] = 5.5f;
            skillData.aliveTime[10] = 5.5f;
            skillData.aliveTime[11] = 2.6f;
            skillData.aliveTime[12] = 4f;
            skillData.aliveTime[13] = 4.2f;
            skillData.aliveTime[14] = 4f;

            skillData.knockbackForce[0] = 1f;
            skillData.knockbackForce[1] = 1f;
            skillData.knockbackForce[2] = 1f;
            skillData.knockbackForce[3] = 1f;
            skillData.knockbackForce[4] = 0f;
            skillData.knockbackForce[5] = 1f;
            skillData.knockbackForce[6] = 0.5f;
            skillData.knockbackForce[7] = 0f;
            skillData.knockbackForce[8] = 0.1f;
            skillData.knockbackForce[9] = 0.1f;
            skillData.knockbackForce[10] = 0.1f;
            skillData.knockbackForce[11] = 0.1f;
            // 여기부터 각성 스킬
            skillData.knockbackForce[12] = 0f;
            skillData.knockbackForce[13] = 0f;
            skillData.knockbackForce[14] = 0f;

            for (int i = 0; i < skillData.skillCount.Length; i++) { skillData.skillCount[i] = 1; }

            for (int i = 0; i < skillData.skillSelected.Length; i++) { skillData.skillSelected[i] = false; }

            // Passive Skill Data 초기화
            for (int i = 0; i < passiveSkillData.level.Length; i++) { passiveSkillData.level[i] = 0; }

            passiveSkillData.damage[0] = 1f;
            passiveSkillData.damage[1] = 1f;
            passiveSkillData.damage[2] = 1f;

            passiveSkillData.damage[3] = 0f;  // 데미지 감소
            passiveSkillData.damage[4] = 1f;  // 이동속도
            passiveSkillData.damage[5] = 0;   // 마그네틱 사거리

            for (int i = 0; i < passiveSkillData.skillSelected.Length; i++) { passiveSkillData.skillSelected[i] = false; }
        }

        public void ChooseStartSkill(int num)
        {
            skillData.skillSelected[num] = true;
        }

        // 공격을 시도하는 함수 (사거리 판단)
        public void TryAttack(int index)
        {
            switch (index)
            {
                case 6:
                case 7:
                    {
                        Enemy enemy = enemyManager.FindNearestEnemy(); // 가장 가까운 적을 찾는다
                        if (enemy == null) return; // 적이 없으면 공격 X

                        CastSkill(enemy, index); // 스킬을 시전

                        break;
                    }
            }
        }

        private void CastSkill(Enemy enemy, int index)
        {
            switch (index)
            {

                case 6: //Holy Epic
                    {
                        skill = poolManager.GetSkill(6) as Cleansing_Strike;

                        Vector2 adjustedPosition = skill.GetAdjustedEnemyPosition(enemy);

                        // 스킬 위치 설정
                        skill.X = adjustedPosition.x;
                        skill.Y = adjustedPosition.y;

                        // 스킬 변수 설정
                        skill.enemy = enemy;
                        ApplySkillSettings(skill, index);

                        AudioManager.instance.PlaySfx(AudioManager.Sfx.CleaningStrike);
                        break;
                    }
                case 7: //Blood Epic
                    {
                        skill = poolManager.GetSkill(7) as Thirst_For_Blood;

                        Vector2 adjustedPosition = skill.GetAdjustedEnemyPosition(enemy);

                        skill.X = adjustedPosition.x;
                        skill.Y = adjustedPosition.y;

                        // 스킬 변수 설정
                        skill.isDotDamageSkill = true;
                        skill.enemy = enemy;
                        ApplySkillSettings(skill, index);
                        LockSkillDuringCast(skill, index);

                        AudioManager.instance.PlaySfx(AudioManager.Sfx.ThirstForBlood);
                        break;
                    }
            }
        }

        private void CastSkill(int index)
        {
            switch (index)
            {
                case 0: //Holy Common
                    {
                        skill = poolManager.GetSkill(0) as Hammer_Of_Retribution;

                        // 스킬 위치 보정
                        float xPositionNum, yPositionNum;
                        bool isFlipped = false;

                        if (skillData.level[index] == 5)
                        {
                            xPositionNum = 0.7f;
                            yPositionNum = 0.7f;
                        }
                        else if (skillData.level[index] >= 3)
                        {
                            xPositionNum = 0.6f;
                            yPositionNum = 0.6f;
                        }
                        else
                        {
                            xPositionNum = 0.5f;
                            yPositionNum = 0.5f;
                        }
                        ApplySkillPositionOffset(skill, index, xPositionNum, yPositionNum, isFlipped);

                        // 스킬 변수 설정
                        ((PlayerAttachSkill)skill).isAttachSkill = true;
                        ApplySkillSettings(skill, index);
                        LockSkillDuringCast(skill, index);

                        break;
                    }
                case 1: //Blood Common
                    {
                        skill = poolManager.GetSkill(1) as Blow_Of_Madness;

                        // 스킬 위치 보정
                        float xPositionNum, yPositionNum;
                        bool isFlipped = true;

                        if (skillData.level[index] == 5)
                        {
                            xPositionNum = 0.5f;
                            yPositionNum = 0.4f;
                        }
                        else if (skillData.level[index] >= 3)
                        {
                            xPositionNum = 0.4f;
                            yPositionNum = 0.4f;
                        }
                        else
                        {
                            xPositionNum = 0.3f;
                            yPositionNum = 0.4f;
                        }

                        ApplySkillPositionOffset(skill, index, xPositionNum, yPositionNum, isFlipped);

                        // 스킬 변수 설정
                        ((PlayerAttachSkill)skill).isAttachSkill = true;
                        ApplySkillSettings(skill, index);
                        LockSkillDuringCast(skill, index);
                        ((Blow_Of_Madness)skill).SetAnimatorSpeed();

                        AudioManager.instance.PlaySfx(AudioManager.Sfx.BlowOfMadness);
                        break;
                    }
                case 2: //Dark Common
                    {
                        skill = poolManager.GetSkill(2) as DarkBond;

                        // 스킬 위치 보정
                        float xPositionNum, yPositionNum;
                        xPositionNum = 0f;
                        yPositionNum = 0f;
                        bool isFlipped = true;

                        ApplySkillPositionOffset(skill, index, xPositionNum, yPositionNum, isFlipped);

                        // 스킬 변수 설정
                        ((PlayerAttachSkill)skill).isAttachSkill = true;
                        skill.isDotDamageSkill = true;
                        ApplySkillSettings(skill, index);
                        LockSkillDuringCast(skill, index);

                        AudioManager.instance.PlaySfx(AudioManager.Sfx.StrikeOfDarkness);
                        break;
                    }
                case 3: //Holy Rare
                    {
                        skill = poolManager.GetSkill(3) as Holy_Shield;

                        // 스킬 변수 설정
                        ((PlayerAttachSkill)skill).isAttachSkill = true;
                        skill.isDotDamageSkill = true;
                        ApplySkillSettings(skill, index);
                        LockSkillDuringCast(skill, index);
                        ((Holy_Shield)skill).onHolyShieldSkillDestroyed = OnHolyShieldSkillOff;

                        OnHolyShieldSkillOn();

                        AudioManager.instance.PlaySfx(AudioManager.Sfx.HolyShield);
                        break;
                    }
                case 4: //Blood Rare
                    {
                        skill = poolManager.GetSkill(4) as Storm_Of_Madness;

                        // 스킬 변수 설정
                        ((PlayerAttachSkill)skill).isAttachSkill = true;
                        skill.isDotDamageSkill = true;
                        ApplySkillSettings(skill, index);
                        LockSkillDuringCast(skill, index);

                        AudioManager.instance.PlaySfx(AudioManager.Sfx.StormOfMadness);
                        break;
                    }
                case 5: //Dark Rare
                    {
                        skill = poolManager.GetSkill(5) as Dark_Slash;

                        // 스킬 위치 보정
                        float xPositionNum, yPositionNum;
                        bool isFlipped = true;

                        if (skillData.level[index] == 5)
                        {
                            yPositionNum = 0.4f;
                        }
                        else if (skillData.level[index] >= 3)
                        {
                            yPositionNum = 0.3f;
                        }
                        else
                        {
                            yPositionNum = 0.2f;
                        }
                        xPositionNum = 0f;

                        ApplySkillPositionOffset(skill, index, xPositionNum, yPositionNum, isFlipped);

                        // 스킬 변수 설정
                        ((PlayerAttachSkill)skill).isAttachSkill = true;
                        skill.isDotDamageSkill = true;
                        ApplySkillSettings(skill, index);
                        LockSkillDuringCast(skill, index);

                        AudioManager.instance.PlaySfx(AudioManager.Sfx.DarkSlash);
                        break;
                    }
                case 8: //Dark Epic
                    {
                        skill = poolManager.GetSkill(8) as Touch_Of_Death;

                        // 스킬 위치 설정
                        float tmpX = PlayerManager.player.transform.position.x;
                        float tmpY = PlayerManager.player.transform.position.y;

                        float ranNum = UnityEngine.Random.Range(-3f, 3f);
                        float ranNum2 = UnityEngine.Random.Range(-1.5f, 1.5f);

                        tmpX += ranNum;
                        tmpY += ranNum2;

                        skill.X = tmpX;
                        skill.Y = tmpY;

                        // 스킬 변수 설정
                        skill.isDotDamageSkill = true;
                        ApplySkillSettings(skill, index);

                        AudioManager.instance.PlaySfx(AudioManager.Sfx.TouchOfDeath);
                        break;
                    }
                case 9: //Holy Legendary
                    {
                        skill = poolManager.GetSkill(9) as Heavenly_Sword;

                        // 스킬 위치 보정
                        float xPositionNum, yPositionNum;
                        bool isFlipped = true;

                        if (skillData.level[index] == 5)
                        {
                            yPositionNum = 0.6f;
                        }
                        else
                        {
                            yPositionNum = 0.4f;
                        }
                        xPositionNum = 0;

                        ApplySkillPositionOffset(skill, index, xPositionNum, yPositionNum, isFlipped);

                        // 스킬 변수 설정
                        ((PlayerAttachSkill)skill).isAttachSkill = true;
                        skill.isDotDamageSkill = true;
                        ApplySkillSettings(skill, index);
                        LockSkillDuringCast(skill, index);

                        AudioManager.instance.PlaySfx(AudioManager.Sfx.HolySword);
                        break;
                    }
                case 10: //Blood Legendary
                    {
                        skill = poolManager.GetSkill(10) as Incarnation_Of_Range;

                        float xPositionNum, yPositionNum;
                        bool isFlipped = true;

                        xPositionNum = 0.5f;
                        yPositionNum = 0.5f;

                        ApplySkillPositionOffset(skill, index, xPositionNum, yPositionNum, isFlipped);

                        // 스킬 변수 설정
                        ((PlayerAttachSkill)skill).isAttachSkill = true;
                        ((PlayerAttachSkill)skill).shouldNotBeFlipped = true;
                        skill.isDotDamageSkill = true;
                        ApplySkillSettings(skill, index);
                        LockSkillDuringCast(skill, index);

                        AudioManager.instance.PlaySfx(AudioManager.Sfx.IncarnationOfRange);
                        break;
                    }
                case 11: //Dark Legendary
                    {
                        skill = poolManager.GetSkill(11) as Master_Of_Darkness;

                        // 스킬 위치 설정
                        float tmpX = PlayerManager.player.transform.position.x;
                        float tmpY = PlayerManager.player.transform.position.y;

                        float ranNum = UnityEngine.Random.Range(-3f, 3f);
                        float ranNum2 = UnityEngine.Random.Range(-1.5f, 1.5f);

                        tmpX += ranNum;
                        tmpY += ranNum2;

                        skill.X = tmpX;
                        skill.Y = tmpY;

                        // 스킬 변수 설정
                        skill.isDotDamageSkill = true;
                        ApplySkillSettings(skill, index);

                        AudioManager.instance.PlaySfx(AudioManager.Sfx.MasterOfDarness);
                        break;
                    }
                case 12: //Holy Ultimate
                    {
                        skill = poolManager.GetSkill(12) as LionHeart;

                        // 스킬 변수 설정
                        skill.isDotDamageSkill = true;
                        ((LionHeart)skill).burstDamage = 120f;
                        ApplySkillSettings(skill, index);
                        LockSkillDuringCast(skill, index);

                        AudioManager.instance.PlaySfx(AudioManager.Sfx.HolyUltimated);
                        break;
                    }
                case 13: //Blood Ultimate
                    {
                        skill = poolManager.GetSkill(13) as Berserk;

                        // 스킬 변수 설정
                        skill.isDotDamageSkill = true;
                        ((Berserk)skill).burstDamage = 120f;
                        ApplySkillSettings(skill, index);
                        LockSkillDuringCast(skill, index);

                        AudioManager.instance.PlaySfx(AudioManager.Sfx.BloodUltimated);
                        break;
                    }
                case 14: //Dark Ultimate
                    {
                        skill = poolManager.GetSkill(14) as EndoftheAbyss;

                        // 스킬 변수 설정
                        skill.isDotDamageSkill = true;
                        ApplySkillSettings(skill, index);
                        LockSkillDuringCast(skill, index);

                        // 스킬 위치 보정 추가
                        Transform parent = skill.transform.parent;

                        skill.transform.parent = null;
                        skill.transform.localScale = new Vector3(
                            2f * (1 + server_PlayerData.basicPassiveLevels[5] * server_PlayerData.attackRange),
                            skillData.scale[index] * (1 + server_PlayerData.basicPassiveLevels[5] * server_PlayerData.attackRange),
                            0);
                        skill.transform.parent = parent;

                        AudioManager.instance.PlaySfx(AudioManager.Sfx.DarkUltimated);
                        break;
                    }
            }
        }

        private void OnHolyShieldSkillOn()
        {
            PlayerManager.player.isPlayerHolyShielded = true;
        }

        // 쉴드 스킬이 꺼질 때
        private void OnHolyShieldSkillOff()
        {
            PlayerManager.player.isPlayerHolyShielded = false;
        }
    }
}