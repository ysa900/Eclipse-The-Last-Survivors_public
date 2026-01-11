using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Eclipse.Game
{
    public class Mage_SkillManager : SkillManager
    {
        //==================================================================
        int skyFallQuadrantNum = -1; // Skyfall스킬 랜덤 사분면 번호

        // 스킬이 시전중일 때 또 시전되면 안되는 스킬들
        // 워터샷, 전류 방출, 수호의 방패, 에너지파, 눈보라, 이클립스, 에너지 스매셔,
        // 해일의 권능, 수호의 불꽃
        // isSkillCasted[]로 관리(SkillManager에 있음)

        // 캐싱용 스킬 클래스 객체
        private Skill skill;

        private void Awake()
        {
            MAX_SKILL_NUM = 18;

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

            if (PlayerManager.player.isPlayerDead) return;

            for (int i = 0; i < skillData.damage.Length; i++)
            {
                if (skillData.skillSelected[i]) // 활성화(선택)된 스킬만 실행
                {
                    bool shouldBeAttack = 0 >= attackDelayTimer[i] && !isSkillsCasted[i]; // 쿨타임이 됐는지 확인
                    if (shouldBeAttack)
                    {
                        attackDelayTimer[i] = skillData.delay[i]
                            * (1 - server_PlayerData.basicPassiveLevels[4] * server_PlayerData.cooldown);

                        if (i == 0 || i == 1 || i == 6)
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

            skillData.damage[0] = 80f; //Fire basic
            skillData.damage[1] = 80f; //Eelectric basic
            skillData.damage[2] = 42f; // water basic, dot damage skill
            skillData.damage[3] = 140f; //Fire rare
            skillData.damage[4] = 105f; //Electric rare
            skillData.damage[5] = 74f; // water rare, dot damage skill
            skillData.damage[6] = 44f; // Fire epic, dot damage skill
            skillData.damage[7] = 74f; // Electric epic, dot damage skill
            skillData.damage[8] = 0f; // water epic
            skillData.damage[9] = 257f; // Fire legendary
            skillData.damage[10] = 432f; // electric legendary
            skillData.damage[11] = 130f; // water legendary, dot damage skill
            // 여기부터 공명 스킬
            skillData.damage[12] = 454f; // dot damage skill 이클립스(메테오+얼음송곳)
            skillData.damage[13] = 472f; // dot damage skill 에너지 스매셔(에너지파+인페르노)
            skillData.damage[14] = 816f; // dot damage skill 해일의 권능(눈보라+에너지파)
            skillData.damage[15] = 168f;  // dot damage skill 수호의 불꽃(화염장벽+수호의방패)
            skillData.damage[16] = 827f; // dot damage skill 신의 심판(천벌+메테오)
            skillData.damage[17] = 814f; // 서리의 분노(얼음송곳+천벌)

            skillData.delay[0] = 2f;
            skillData.delay[1] = 1.5f;
            skillData.delay[2] = 2f;
            skillData.delay[3] = 2f;
            skillData.delay[4] = 2f;
            skillData.delay[5] = 2f;
            skillData.delay[6] = 4f;
            skillData.delay[7] = 3f;
            skillData.delay[8] = 5f;
            skillData.delay[9] = 0.5f;
            skillData.delay[10] = 2.5f;
            skillData.delay[11] = 2.5f;
            // 여기부터 공명 스킬
            skillData.delay[12] = 6f;
            skillData.delay[13] = 4f;
            skillData.delay[14] = 3.5f;
            skillData.delay[15] = 6.5f;
            skillData.delay[16] = 2.2f;
            skillData.delay[17] = 0.2f;

            skillData.scale[0] = 1f; //Fire basic
            skillData.scale[1] = 0.2f; // Electric basic
            skillData.scale[2] = 0.5f; // Water basic
            skillData.scale[3] = 0.3f; //Fire rare
            skillData.scale[4] = 0.3f; //Electric rare
            skillData.scale[5] = 0.4f; // Water rare
            skillData.scale[6] = 1.5f; // Fire Epic
            skillData.scale[7] = 0.5f; //Electric Epic
            skillData.scale[8] = 0.3f; // Water Epic
            skillData.scale[9] = 0.3f; // Fire Legend
            skillData.scale[10] = 0.8f; // Electric Legend
            skillData.scale[11] = 0.45f; // Water Legend
            // 여기부터 공명 스킬
            skillData.scale[12] = 5f;
            skillData.scale[13] = 3f;
            skillData.scale[14] = 2f;
            skillData.scale[15] = 2f;
            skillData.scale[16] = 1.5f;
            skillData.scale[17] = 1.8f;

            skillData.aliveTime[0] = 1f;
            skillData.aliveTime[1] = 0.5f;
            skillData.aliveTime[2] = 1f;
            skillData.aliveTime[3] = 0.5f;
            skillData.aliveTime[4] = 3f;
            skillData.aliveTime[5] = 4f;
            skillData.aliveTime[6] = 2f;
            skillData.aliveTime[7] = 2f;
            skillData.aliveTime[8] = 1.5f;
            skillData.aliveTime[9] = 0.5f;
            skillData.aliveTime[10] = 0.7f;
            skillData.aliveTime[11] = 3f;
            // 여기부터 공명 스킬
            skillData.aliveTime[12] = 23f * AnimationConstants.FrameTime;
            skillData.aliveTime[13] = 2.8f;
            skillData.aliveTime[14] = 2.8f;
            skillData.aliveTime[15] = 3.8f;
            skillData.aliveTime[16] = 2.55f;
            skillData.aliveTime[17] = 2f;

            skillData.knockbackForce[0] = 1f;
            skillData.knockbackForce[1] = 1f;
            skillData.knockbackForce[2] = 0.5f;
            skillData.knockbackForce[3] = 1f;
            skillData.knockbackForce[4] = 1f;
            skillData.knockbackForce[5] = 0.1f;
            skillData.knockbackForce[6] = 0f;
            skillData.knockbackForce[7] = 0.5f;
            skillData.knockbackForce[8] = 0f;
            skillData.knockbackForce[9] = 0.1f;
            skillData.knockbackForce[10] = 0.1f;
            skillData.knockbackForce[11] = 0.1f;
            // 여기부터 공명 스킬
            skillData.knockbackForce[12] = 0f;
            skillData.knockbackForce[13] = 0.5f;
            skillData.knockbackForce[14] = 0.1f;
            skillData.knockbackForce[15] = 0f;
            skillData.knockbackForce[16] = 0.1f;
            skillData.knockbackForce[17] = 0.5f;

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

            skyFallQuadrantNum = -1;
#if UNITY_EDITOR
            //skillData.damage[0] = 8000f; //Fire basic
            //skillData.delay[0] = 0.1f; //Fire basic
#endif
        }

        // 시작 스킬을 선택하는 함수 (개발용)
        // num : 스킬 번호 (불 - 0 , 전기 - 1, 물 - 2)
        public void ChooseStartSkill(int num)
        {
            skillData.skillSelected[num] = true;
        }

        // 공격을 시도하는 함수 (사거리 판단)
        // 스킬 관련 배열들 공통 사항
        // 스킬들 index: 불 - 3n, 전기 - 3n + 1, 물 - 3n + 2
        // 불 - 0, 3, 6, 9 / 전기 - 1, 4, 7, 10 / 물 - 2, 5, 8 ,11
        public void TryAttack(int index)
        {
            switch (index)
            {
                case 0:
                    {
                        // 불 공격은 가장 가까운 적이 사거리 내에 있어야지만 나간다
                        Enemy enemy = enemyManager.FindNearestEnemy(); // 가장 가까운 적을 찾는다
                        if (enemy == null) return; // 적이 없으면 공격 X

                        CastSkill(enemy, index); // 스킬을 시전

                        break;
                    }
                case 1: // 라이트닝
                case 6: // 인페르노
                    {
                        Enemy enemy = enemyManager.GetRandomTargetInRange();
                        if (enemy == null) return; // 적이 없으면 공격 X

                        CastSkill(enemy, index);

                        break; // switch 문의 break
                    }
            }
        }

        // 스킬을 시전하는 함수 (Enemy)
        // 스킬 관련 배열들 공통 사항
        // 스킬들 index: 불 - 3n, 전기 - 3n + 1, 물 - 3n + 2
        // 불 - 0, 3, 6, 9 / 전기 - 1, 4, 7, 10 / 물 - 2, 5, 8 ,11
        private void CastSkill(Enemy enemy, int index)
        {
            switch (index)
            {

                case 0:
                    {
                        skill = poolManager.GetSkill(0) as Fireball;

                        // 스킬 방향 설정
                        Vector2 playerPosition = PlayerManager.player.transform.position;
                        Vector2 enemyPosition = skill.GetAdjustedEnemyPosition(enemy);

                        // 파이퍼볼 방향 보정 (적 바라보게)
                        Vector2 direction = new Vector2(enemyPosition.x - playerPosition.x, enemyPosition.y - playerPosition.y);
                        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

                        Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.forward);
                        Quaternion rotation = Quaternion.Slerp(skill.transform.rotation, angleAxis, 5f);
                        skill.transform.rotation = rotation;

                        // 스킬 위치 설정
                        skill.X = playerPosition.x;
                        skill.Y = playerPosition.y;

                        // 스킬 변수 설정
                        skill.enemy = enemy;
                        ((EnemyTrackingSkill)skill).SetTrackingDirection();
                        ApplySkillSettings(skill, index);

                        AudioManager.instance.PlaySfx(AudioManager.Sfx.FireBall);
                        break;
                    }
                case 1:
                    {
                        skill = poolManager.GetSkill(6) as Lightning;

                        Vector2 adjustedPosition = skill.GetAdjustedEnemyPosition(enemy);

                        // 스킬 변수 설정
                        skill.enemy = enemy;
                        // 위치 적용
                        skill.X = adjustedPosition.x;
                        skill.Y = adjustedPosition.y;

                        ApplySkillSettings(skill, index);

                        AudioManager.instance.PlaySfx(AudioManager.Sfx.Lightning);
                        break;
                    }
                case 6:
                    {
                        skill = PoolManager.instance.GetSkill(2) as Inferno;

                        Vector2 adjustedPosition = skill.GetAdjustedEnemyPosition(enemy);

                        // 스킬 변수 할당
                        skill.enemy = enemy;
                        // 위치 적용
                        skill.X = adjustedPosition.x;
                        skill.Y = adjustedPosition.y;

                        skill.isDotDamageSkill = true;
                        ApplySkillSettings(skill, index);

                        AudioManager.instance.PlaySfx(AudioManager.Sfx.Inferno);
                        break;
                    }
            }
        }

        // 스킬을 시전하는 함수
        private void CastSkill(int index)
        {
            switch (index)
            {
                case 2:
                    {
                        skill = poolManager.GetSkill(10) as Water_Shot;
                        skill.X = 999f; skill.Y = 999f; // 초기 위치 설정
                        // 스킬 위치 보정
                        float xPositionNum, yPositionNum;
                        bool isXFlipped = true;

                        if (skillData.level[index] == 5)
                        {
                            xPositionNum = 1 * 1.5f;
                        }
                        else if (skillData.level[index] >= 3)
                        {
                            xPositionNum = 1 * 1.25f;
                        }
                        else
                        {
                            xPositionNum = 1f;
                        }
                        yPositionNum = 0f;

                        // 스킬 변수 할당
                        ((PlayerAttachSkill)skill).isAttachSkill = true;
                        skill.isDotDamageSkill = true;
                        ApplySkillSettings(skill, index);
                        LockSkillDuringCast(skill, index);
                        ApplySkillPositionOffset(skill, index, xPositionNum, yPositionNum, isXFlipped);

                        AudioManager.instance.PlaySfx(AudioManager.Sfx.WaterShot);
                        break;
                    }
                case 3:
                    {
                        skill = poolManager.GetSkill(1) as Explosion;

                        // 스킬 위치 보정
                        float xPositionNum, yPositionNum;
                        bool isXFlipped = false;

                        xPositionNum = 0f;
                        yPositionNum = 0.2f;


                        // 스킬 위치 설정
                        skill.X = PlayerManager.player.transform.position.x;
                        skill.Y = PlayerManager.player.transform.position.y;

                        // 스킬 변수 할당
                        ((PlayerAttachSkill)skill).isAttachSkill = true;
                        ApplySkillSettings(skill, index);
                        ApplySkillPositionOffset(skill, index, xPositionNum, yPositionNum, isXFlipped);

                        AudioManager.instance.PlaySfx(AudioManager.Sfx.Explosion);
                        break;
                    }
                case 4:
                    {
                        CastElectricBall();
                        break;
                    }
                case 5:
                    {
                        skill = poolManager.GetSkill(11) as Ice_Spike;

                        // 스킬 위치 설정
                        float tmpX = PlayerManager.player.transform.position.x;
                        float tmpY = PlayerManager.player.transform.position.y;

                        float ranNum = UnityEngine.Random.Range(-3f, 3f);
                        float ranNum2 = UnityEngine.Random.Range(-2f, 2f);

                        tmpX += ranNum;
                        tmpY += ranNum2;

                        skill.X = tmpX;
                        skill.Y = tmpY;

                        // 스킬 변수 할당
                        skill.isDotDamageSkill = true;
                        ApplySkillSettings(skill, index);

                        AudioManager.instance.PlaySfx(AudioManager.Sfx.IceSpike);
                        break;
                    }
                case 7:
                    {
                        skill = poolManager.GetSkill(8) as Energy_Blast;

                        // 스킬 위치 보정
                        float xPositionNum, yPositionNum;

                        xPositionNum = 1.8f;
                        yPositionNum = -0.1f;

                        // 스킬 변수 할당
                        ((PlayerAttachSkill)skill).isAttachSkill = true;
                        skill.isDotDamageSkill = true;
                        ApplySkillSettings(skill, index);
                        LockSkillDuringCast(skill, index);
                        ApplySkillPositionOffset(skill, index, xPositionNum, yPositionNum);

                        // 스킬 위치 보정 추가
                        Transform parent = skill.transform.parent;

                        skill.transform.parent = null;
                        skill.transform.localScale = new Vector3(
                            1f * (1 + server_PlayerData.basicPassiveLevels[5] * server_PlayerData.attackRange),
                            skillData.scale[index] * (1 + server_PlayerData.basicPassiveLevels[5] * server_PlayerData.attackRange),
                            0);
                        skill.transform.parent = parent;

                        float scaleMultiplier = server_PlayerData.basicPassiveLevels[5] * server_PlayerData.attackRange;
                        ((PlayerAttachSkill)skill).xOffset = xPositionNum + 1f * scaleMultiplier;
                        ((PlayerAttachSkill)skill).yOffset = yPositionNum + skillData.scale[index] * scaleMultiplier;

                        AudioManager.instance.PlaySfx(AudioManager.Sfx.EnergyBlast);
                        break;
                    }
                case 8:
                    {
                        skill = poolManager.GetSkill(12) as Water_Shield;

                        // 스킬 위치 설정
                        skill.X = PlayerManager.player.transform.position.x;
                        skill.Y = PlayerManager.player.transform.position.y;

                        // 스킬 변수 할당
                        ((Water_Shield)skill).onShieldSkillDestroyed = OnShieldSkillOff;
                        ApplySkillSettings(skill, index);
                        LockSkillDuringCast(skill, index);

                        OnShieldSkillOn();

                        AudioManager.instance.PlaySfx(AudioManager.Sfx.HolyShield); // 다른 소리로 변경하면 좋음
                        break;
                    }
                case 9:
                    {
                        skill = poolManager.GetSkill(4) as Meteor;

                        // 스킬 변수 할당
                        ((RandomSkill)skill).scale = skillData.scale[index];
                        ApplySkillSettings(skill, index);
                        skill.Init();

                        AudioManager.instance.PlaySfx(AudioManager.Sfx.Meteor);
                        break;
                    }
                case 10:
                    {
                        StartCoroutine(CastJudgement(index, 9));

                        break;
                    }
                case 11:
                    {
                        skill = poolManager.GetSkill(13) as Ice_Blast;

                        // 스킬 위치 보정
                        float xPositionNum, yPositionNum;
                        bool isYFlipped = true;

                        if (skillData.level[index] == 5)
                        {
                            xPositionNum = 1f * 1.5f;
                        }
                        else if (skillData.level[index] >= 3)
                        {
                            xPositionNum = 1f * 1.25f;
                        }
                        else
                        {
                            xPositionNum = 1f;
                        }
                        yPositionNum = 0f;
                        // 스킬 변수 할당
                        ((PlayerAttachSkill)skill).isAttachSkill = true;
                        skill.isDotDamageSkill = true;
                        ApplySkillSettings(skill, index);
                        LockSkillDuringCast(skill, index);
                        ApplySkillPositionOffset(skill, index, xPositionNum, yPositionNum, null, isYFlipped);

                        AudioManager.instance.PlaySfx(AudioManager.Sfx.IceBlast);
                        break;
                    }
                case 12:
                    {
                        skill = poolManager.GetSkill(14) as HeavensEclipse;

                        // 스킬 변수 할당
                        skill.isDotDamageSkill = true;
                        ((HeavensEclipse)skill).burstDamage = 750f;
                        ApplySkillSettings(skill, index);
                        LockSkillDuringCast(skill, index);

                        AudioManager.instance.PlaySfx(AudioManager.Sfx.HeavenEclipse);
                        break;
                    }
                case 13:
                    {
                        skill = poolManager.GetSkill(15) as EnergySmasher;

                        // 스킬 위치 보정
                        float xPositionNum, yPositionNum;
                        bool isYFlipped = true;

                        xPositionNum = 2.1f;
                        yPositionNum = 0f;
                        // 스킬 변수 할당
                        ((PlayerAttachSkill)skill).isAttachSkill = true;
                        skill.isDotDamageSkill = true;
                        ApplySkillSettings(skill, index);
                        LockSkillDuringCast(skill, index);
                        ApplySkillPositionOffset(skill, index, xPositionNum, yPositionNum, null, isYFlipped);

                        // 스킬 위치 보정 추가
                        Transform parent = skill.transform.parent;

                        skill.transform.parent = null;
                        skill.transform.localScale = new Vector3(
                            6f * (1 + server_PlayerData.basicPassiveLevels[5] * server_PlayerData.attackRange),
                            skillData.scale[index] * (1 + server_PlayerData.basicPassiveLevels[5] * server_PlayerData.attackRange),
                            0);
                        skill.transform.parent = parent;

                        float scaleMultiplier = server_PlayerData.basicPassiveLevels[5] * server_PlayerData.attackRange;
                        ((PlayerAttachSkill)skill).xOffset = xPositionNum + 6f * scaleMultiplier;
                        ((PlayerAttachSkill)skill).yOffset = yPositionNum + skillData.scale[index] * scaleMultiplier;

                        AudioManager.instance.PlaySfx(AudioManager.Sfx.BeamLaser);
                        break;
                    }
                case 14:
                    {
                        skill = poolManager.GetSkill(16) as HydroFlame;

                        // 스킬 변수 할당
                        ((PlayerAttachSkill)skill).isAttachSkill = true;
                        skill.isDotDamageSkill = true;
                        ApplySkillSettings(skill, index);
                        LockSkillDuringCast(skill, index);

                        // 스킬 위치 보정 추가
                        Transform parent = skill.transform.parent;

                        skill.transform.parent = null;
                        skill.transform.localScale = new Vector3(
                            1f * (1 + server_PlayerData.basicPassiveLevels[5] * server_PlayerData.attackRange),
                            skillData.scale[index] * (1 + server_PlayerData.basicPassiveLevels[5] * server_PlayerData.attackRange),
                            0);
                        skill.transform.parent = parent;

                        float scaleMultiplier = server_PlayerData.basicPassiveLevels[5] * server_PlayerData.attackRange;
                        ((HydroFlame)skill).radius = 4.6f + skillData.scale[index] * scaleMultiplier;

                        AudioManager.instance.PlaySfx(AudioManager.Sfx.HydroFlame);
                        break;
                    }
                case 15:
                    {
                        skill = poolManager.GetSkill(17) as Shield_Flame;

                        // 스킬 위치 보정
                        float xPositionNum, yPositionNum;

                        xPositionNum = 0f;
                        yPositionNum = 0f;
                        // 스킬 위치 설정
                        skill.X = PlayerManager.player.transform.position.x;
                        skill.Y = PlayerManager.player.transform.position.y;

                        // 스킬 변수 할당
                        skill.isDotDamageSkill = true;

                        ((Shield_Flame)skill).onShieldSkillDestroyed = OnShieldSkillOff;
                        ApplySkillSettings(skill, index);
                        LockSkillDuringCast(skill, index);
                        ApplySkillPositionOffset(skill, index, xPositionNum, yPositionNum);

                        OnShieldSkillOn();

                        AudioManager.instance.PlaySfx(AudioManager.Sfx.ShieldFlame);
                        break;
                    }
                case 16:
                    {
                        skill = poolManager.GetSkill(18) as Sky_Fall;

                        // 스킬 위치 설정
                        float tmpX = PlayerManager.player.transform.position.x;
                        float tmpY = PlayerManager.player.transform.position.y;

                        // 스킬이 플레이어 기준 1 ~ 4분면 중 어디에 시전될 까
                        // 이전에 떨어졌던 사분면에는 떨어지지 않음
                        int quadrantNum;
                        do
                        {
                            quadrantNum = UnityEngine.Random.Range(1, 5);
                        }
                        while (skyFallQuadrantNum == quadrantNum);

                        skyFallQuadrantNum = quadrantNum;

                        float ranNumX = 0;
                        float ranNumY = 0;
                        switch (quadrantNum)
                        {
                            case 1:
                                ranNumX = UnityEngine.Random.Range(0.4f, 1f);
                                ranNumY = UnityEngine.Random.Range(0.2f, 1f);
                                break;

                            case 2:
                                ranNumX = UnityEngine.Random.Range(-2f, -0.4f);
                                ranNumY = UnityEngine.Random.Range(0.2f, 1f);
                                break;

                            case 3:
                                ranNumX = UnityEngine.Random.Range(-2f, -0.4f);
                                ranNumY = UnityEngine.Random.Range(-1f, -0.2f);
                                break;

                            case 4:
                                ranNumX = UnityEngine.Random.Range(0.4f, 2f);
                                ranNumY = UnityEngine.Random.Range(-1f, -0.2f);
                                break;
                        }

                        tmpX += ranNumX;
                        tmpY += ranNumY;

                        skill.X = tmpX;
                        skill.Y = tmpY;

                        // 스킬 변수 할당
                        skill.isDotDamageSkill = true;
                        ApplySkillSettings(skill, index);

                        AudioManager.instance.PlaySfx(AudioManager.Sfx.SkyFall);
                        break;
                    }
                case 17:
                    {
                        skill = poolManager.GetSkill(19) as Frozen_Spike;

                        // 스킬 위치 설정
                        float tmpX = PlayerManager.player.transform.position.x;
                        float tmpY = PlayerManager.player.transform.position.y;

                        // 스킬이 플레이어 기준 1 ~ 4분면 중 어디에 시전될 까
                        // 이전에 떨어졌던 사분면에는 떨어지지 않음
                        int quadrantNum = -1;

                        List<int> qudrantNums = new List<int> { 1, 2, 3, 4 };

                        for (int i = 0; i < qudrantNums.Count; i++)
                        {
                            quadrantNum = UnityEngine.Random.Range(1, 5);
                            if (skyFallQuadrantNum == quadrantNum)
                                qudrantNums.Remove(quadrantNum);
                            else
                            {
                                skyFallQuadrantNum = quadrantNum;
                                break;
                            }
                        }

                        float ranNumX = 0;
                        float ranNumY = 0;
                        switch (quadrantNum)
                        {
                            case 1:
                                ranNumX = UnityEngine.Random.Range(0, 3.2f);
                                ranNumY = UnityEngine.Random.Range(0, 1.5f);
                                break;

                            case 2:
                                ranNumX = UnityEngine.Random.Range(-3.2f, 0);
                                ranNumY = UnityEngine.Random.Range(0, 1.5f);
                                break;

                            case 3:
                                ranNumX = UnityEngine.Random.Range(-3.2f, 0);
                                ranNumY = UnityEngine.Random.Range(0, -1.5f);
                                break;

                            case 4:
                                ranNumX = UnityEngine.Random.Range(0, 3.2f);
                                ranNumY = UnityEngine.Random.Range(-1.5f, 0);
                                break;
                            default:
                                Debug.Log("사분면을 찾지 못했습니다.");
                                break;
                        }

                        tmpX += ranNumX;
                        tmpY += ranNumY;

                        skill.X = tmpX;
                        skill.Y = tmpY;

                        // 스킬 변수 할당
                        ApplySkillSettings(skill, index);

                        AudioManager.instance.PlaySfx(AudioManager.Sfx.FrozenSpike);
                        break;
                    }
            }
        }

        // 쉴드 스킬이 켜질 때
        private void OnShieldSkillOn()
        {
            PlayerManager.player.isPlayerShielded = true;
        }

        // 쉴드 스킬이 꺼질 때
        private void OnShieldSkillOff()
        {
            PlayerManager.player.isPlayerShielded = false;
        }

        private void CastElectricBall()
        {
            float tmpDegree = 0f;
            int index = 4;

            if (skillData.level[index] == 5)
            {
                for (int i = 0; i < 3; i++)
                {
                    skill = poolManager.GetSkill(7) as Electric_Ball;

                    ((Electric_Ball)skill).degree = tmpDegree;
                    tmpDegree -= 120f;

                    // 스킬 위치 설정
                    skill.X = PlayerManager.player.transform.position.x + 3f;
                    skill.Y = PlayerManager.player.transform.position.y;

                    // 스킬 변수 할당
                    ApplySkillSettings(skill, index);
                    LockSkillDuringCast(skill, index);
                    // 스킬 위치 보정
                    ApplySkillPositionOffset(skill, index, 1, 0);
                }
            }
            else if (skillData.level[index] >= 3)
            {
                for (int i = 0; i < 2; i++)
                {
                    skill = poolManager.GetSkill(7) as Electric_Ball;

                    ((Electric_Ball)skill).degree = tmpDegree;
                    tmpDegree -= 180f;

                    // 스킬 위치 설정
                    skill.X = PlayerManager.player.transform.position.x;
                    skill.Y = PlayerManager.player.transform.position.y;

                    // 스킬 변수 할당
                    ApplySkillSettings(skill, index);
                    LockSkillDuringCast(skill, index);
                    // 스킬 위치 보정
                    ApplySkillPositionOffset(skill, index, 0.7f, 0);
                }
            }
            else
            {
                skill = poolManager.GetSkill(7) as Electric_Ball;

                ((Electric_Ball)skill).degree = tmpDegree;

                // 스킬 위치 설정
                skill.X = PlayerManager.player.transform.position.x + 3f;
                skill.Y = PlayerManager.player.transform.position.y;

                // 스킬 변수 할당
                ApplySkillSettings(skill, index);
                LockSkillDuringCast(skill, index);
                // 스킬 위치 보정
                ApplySkillPositionOffset(skill, index, 0.7f, 0);
            }

            AudioManager.instance.PlaySfx(AudioManager.Sfx.ElectricBall);
        }

        // Judgment 스킬 쓸 때 일정 딜레이로 스킬 cast하기 위함
        IEnumerator CastJudgement(int index, int num)
        {
            for (int i = 0; i < num; i++)
            {
                skill = poolManager.GetSkill(9) as Judgement;

                // 스킬 위치 설정
                float tmpX = PlayerManager.player.transform.position.x;
                float tmpY = PlayerManager.player.transform.position.y;

                float ranNum = UnityEngine.Random.Range(-2f, 2f);
                float ranNum2 = UnityEngine.Random.Range(-1f, 1f);

                tmpX += ranNum;
                tmpY += ranNum2;

                skill.X = tmpX;
                skill.Y = tmpY;

                // 스킬 변수 할당
                ApplySkillSettings(skill, index);
                LockSkillDuringCast(skill, index);

                AudioManager.instance.PlaySfx(AudioManager.Sfx.Judgement);

                yield return new WaitForSeconds(0.2f); // 지정한 초 만큼 쉬기
            }
        }
    }
}