using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Eclipse.Game
{
    public class Assassin_SkillManager : SkillManager
    {
        //==================================================================
        // 비급 데이터
        [ReadOnly] public MijiSkillData buttonActiveMijiSkillData; // 버튼 액티브 스킬
        [ReadOnly] public MijiSkillData passiveMijiSkillData; // 패시브 미지 스킬

        //==================================================================

        // 많이 모인 적 구현 관련 변수
        private float skillRange = 4f; // 스킬 사거리

        // 독운투척 : 독병 날라가는 시간 설정f
        float potionAliveTime = 0.8f;

        /* 비급 */
        private bool isShadowActive = false; // 환영분신 존재 여부 확인

        public bool isBossAppear;

        //=================================================================
        // 캐싱용 스킬 클래스 객체
        private Skill skill;

        //==================================================================

        /* 스킬이 시전중일 때 또 시전되면 안되는 스킬들 - 6개
         * 은조참성, 독운투척, 혼골단참, 염화단도, 연혈상탄, 앵화단도
        */

        //==================================================================
        /* Action 모음 */

        public Action<List<Assassin_Illusion>> allocateIllusionAction;
        public List<Assassin_Illusion> illusionActionList = new List<Assassin_Illusion>();

        //==================================================================

        /*
        어쌔신(Assassin) 스킬 설명

        [스킬 인덱스]
        암기류 : 3n, 인술류 : 3n + 1, 트랩류 : 3n + 2

        */

        /* 일반스킬 목록 (투척류 - 근접류 - 트랩류)
            * 0 : Dagger
            * 1 : ClawSlash / EntropicDecay
            * 2 : AcidSpot
            * 
            * 3 : FireSlash
            * 4 : KillingEdge
            * 5 : VineTrap
            * 
            * 6 : Riddle
            * 7 : BoneSwarm
            * 8 : Desiccation
            * 
            * 9 : ExplosionEnsemble
            * 10 : Blink - 회피율 구현(참고용)
            * 11 : ArcaneHeart   
         */

        /* 리뉴얼된 일반스킬 목록 (암기류 - 인술류 - 트랩류)
           지우지 말 클래스 : Blink - 회피율 구현(참고용)!!
           PoolManager 개수 : 15개
            ** 계열별 1티어 스킬 : 마우스 활용 스킬 **
            * 0 : Dagger - 단검연격(短劍連擊)                               0
            * 1 : EntropicDecay - 화염지구(火焰之球)                        1
            * 2 : AcidSpot - 독운투척(毒雲投擲) Pooling 2개                  2,3
            * 
            * 3 : FireSlash - 화염회귀(火焰回歸)                            4
            * 4 : BlueLotus - 염화매혹(炎華魅惑)                            5
            * 5 : VineTrap - 마령등사(魔靈藤絲) Pooling 2개                  6,7
            * 
            * 6 : KillingEdge -  염화단도(炎火短刀)                         8
            * 7 : Desiccation - 연혈상탄(連血霜彈) // 많은 적                 9
            * 8 : DeathStrikeSeal - 은안지사(隱眼之蛇)                      10
            * 
            * 9 : SpellSword - 파멸각인(滅亡刻印) // 많은 적                         11
            * 10 : SpritOfTheWild - 청시군무(靑翅群舞) // 많은 적                        12
            * 11 : ArcaneHeart - 혼마화진(混魔花陣) Pooling 2개 // 많은 적     13,14
         */

        private void Awake()
        {
            MAX_SKILL_NUM = 12; // 일반 스킬 : 12개

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

            illusionActionList.Clear();
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

                        // 4, 7, 8 : 사거리 내 랜덤한 적
                        // 9, 10, 11 : 많이 모인 적 근방(중앙 위치)
                        if (i == 4 || i == 5 || i == 6 || i == 7 || i == 8 || i == 9 || i == 10 || i == 11)
                        {
                            for (int j = 0; j < skillData.skillCount[i]; j++)
                            {
                                TryAttack(i); // 스킬 쿨타임이 다 됐으면 공격을 시도한다

                                if (isShadowActive)
                                {
                                    // 두 번째 스킬 시전을 0.2초 후에 실행
                                    StartCoroutine(DelayedShadowTryAttack(i, 0.2f));
                                }
                            }

                            enemyManager.ClearNearestEnemies();
                        }
                        else
                        {
                            for (int j = 0; j < skillData.skillCount[i]; j++)
                            {
                                CastSkill(i); // 스킬 쿨타임이 다 됐으면 공격한다
                                if (isShadowActive)
                                {
                                    // 두 번째 스킬 시전을 0.2초 후에 실행
                                    StartCoroutine(DelayedShadowCastSkill(i, 0.2f));
                                }
                            }
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

        // delay만큼 지연 후 공격 시도
        private IEnumerator DelayedShadowTryAttack(int skillIndex, float delay)
        {
            yield return new WaitForSeconds(delay);

            // 원래 데미지 값을 복사하고 레벨 계수를 곱해서 새로운 데미지 계산
            float originalDamage = skillData.damage[skillIndex];
            float modifiedDamage = originalDamage * buttonActiveMijiSkillData.levelCoefficient[0]; // 50% 데미지
            //Debug.Log("나가나요?");
            // 스킬 데미지를 임시로 변경하여 공격
            TryAttack(skillIndex, modifiedDamage);
        }

        // delay만큼 지연 후 스킬 시전
        private IEnumerator DelayedShadowCastSkill(int skillIndex, float delay)
        {
            yield return new WaitForSeconds(delay);

            // 원래 데미지 값을 복사하고 레벨 계수를 곱해서 새로운 데미지 계산
            float originalDamage = skillData.damage[skillIndex];
            float modifiedDamage = originalDamage * buttonActiveMijiSkillData.levelCoefficient[0]; // 50% 데미지

            // 스킬 데미지를 임시로 변경하여 공격
            CastSkill(skillIndex, modifiedDamage);
        }

        // skilldata를 초기화
        public override void Init()
        {
            // [ 액티브 스킬 관련 ] - 수치 임의 지정
            // Skill Data 초기화
            for (int i = 0; i < skillData.level.Length; i++) { skillData.level[i] = 0; }

            skillData.damage[0] = 60f;
            skillData.damage[1] = 20f; // 도트 데미지 스킬, EntropicDecay
            skillData.damage[2] = 24f; // 도트 데미지 스킬
            skillData.damage[3] = 23f; // 도트 데미지 스킬
            skillData.damage[4] = 22f; // 도트 데미지 스킬, BlueLotus
            skillData.damage[5] = 21f; // 도트 데미지 스킬
            skillData.damage[6] = 29f; // 도트 데미지 스킬
            skillData.damage[7] = 28f; //도트 데미지 스킬
            skillData.damage[8] = 26f; // 도트 데미지 스킬
            skillData.damage[9] = 36f; // 도트 데미지 스킬, 도트 데미지 적용은 따로(스킬매니저에서 isdotSkill 해주는 방식 X)
            skillData.damage[10] = 35f; // 도트 데미지 스킬
            skillData.damage[11] = 48f; //도트 데미지 스킬

            // 쿨타임 
            skillData.delay[0] = 2f;
            skillData.delay[1] = 1.5f;
            skillData.delay[2] = 0.5f;
            skillData.delay[3] = 4f;
            skillData.delay[4] = 3.5f;
            skillData.delay[5] = 3.5f;
            skillData.delay[6] = 3f;
            skillData.delay[7] = 3.5f;
            skillData.delay[8] = 3.5f;
            skillData.delay[9] = 3.5f;
            skillData.delay[10] = 4.5f;
            skillData.delay[11] = 6f;

            // 스케일 (크기)
            skillData.scale[0] = 0.0685f;
            skillData.scale[1] = 1f; // EntropicDecay
            skillData.scale[2] = 0.35f;
            skillData.scale[3] = 1.5f;
            skillData.scale[4] = 1.0f;
            skillData.scale[5] = 2.5f;
            skillData.scale[6] = 1f;
            skillData.scale[7] = 1f;
            skillData.scale[8] = 0.8f;
            skillData.scale[9] = 1f;
            skillData.scale[10] = 1f;
            skillData.scale[11] = 1f;

            // 지속 시간
            skillData.aliveTime[0] = 3f;
            skillData.aliveTime[1] = 1.75f; // EntropicDecay
            skillData.aliveTime[2] = 1.2f;
            skillData.aliveTime[3] = 2f;
            skillData.aliveTime[4] = 1.5f;
            skillData.aliveTime[5] = 2f;
            skillData.aliveTime[6] = 2.1f;
            skillData.aliveTime[7] = 2f;
            skillData.aliveTime[8] = 2.2f;
            skillData.aliveTime[9] = 3f;
            skillData.aliveTime[10] = 3f;
            skillData.aliveTime[11] = 3f;

            skillData.knockbackForce[0] = 1f;
            skillData.knockbackForce[1] = 0f;
            skillData.knockbackForce[2] = 0.1f;
            skillData.knockbackForce[3] = 0.5f;
            skillData.knockbackForce[4] = 0f;
            skillData.knockbackForce[5] = 0f;
            skillData.knockbackForce[6] = 1f;
            skillData.knockbackForce[7] = 0.5f;
            skillData.knockbackForce[8] = 1f;
            skillData.knockbackForce[9] = 0.1f;
            skillData.knockbackForce[10] = 0.1f;
            skillData.knockbackForce[11] = -1f;

            // 회피율 초기화 (기본 부여 초기값 = 0%)
            for (int i = 0; i < skillData.sideEffect.Length; i++) skillData.sideEffect[i] = 0;

            // 스킬 나가는 횟수
            for (int i = 0; i < skillData.skillCount.Length; i++) { skillData.skillCount[i] = 1; }
            // 선택됐는 지
            for (int i = 0; i < skillData.skillSelected.Length; i++) { skillData.skillSelected[i] = false; }

            // [ 패시브 스킬 관련 ]
            // Passive Skill Data 초기화
            for (int i = 0; i < passiveSkillData.level.Length; i++) { passiveSkillData.level[i] = 0; }

            // 계열별 크리티컬 확률(기본 : 1(100%))
            passiveSkillData.damage[0] = 1f;
            passiveSkillData.damage[1] = 1f;
            passiveSkillData.damage[2] = 1f;

            passiveSkillData.damage[3] = 0; // 데미지 감소
            passiveSkillData.damage[4] = 1f; // 이동속도
            passiveSkillData.damage[5] = 0; // 마그네틱 사거리

            // 선택됐는 지
            for (int i = 0; i < passiveSkillData.skillSelected.Length; i++) { passiveSkillData.skillSelected[i] = false; }

            illusionActionList.Clear();
            isBossAppear = false;

            isShadowActive = false;
        }

        // 시작 스킬을 선택하는 함수 (개발용)
        public void ChooseStartSkill(int num)
        {
            skillData.skillSelected[num] = true;
        }


        // 공격을 시도하는 함수 - 사거리 판단 : Tracking, EnemyOn 스킬들을 ~
        public void TryAttack(int index, float? modifiedDamage = null)
        {
            switch (index)
            {
                case 4: // 염화매혹(炎華魅惑)
                case 5: // 마령등사(魔靈藤絲)
                case 7: // 연혈상탄(連血霜彈)
                case 8: // 은안지사(隱眼之蛇)
                    {
                        // 사거리(attackRange) 내 랜덤한 적에게 시전된다
                        Enemy enemy = enemyManager.GetRandomTargetInRange();
                        if (enemy == null) return; // 적이 없으면 공격 X

                        CastSkill(enemy, index, modifiedDamage);

                        break; // switch 문의 break
                    }
                case 9:  // 파멸각인(滅亡刻印)
                case 10: // 청시군무(靑翅群舞)
                case 11: // 혼마화진(混魔花陣)
                    {
                        // 스킬 사거리 내 적 중 가장 많이 모인 곳의 적을 찾음
                        Enemy enemy = enemyManager.FindMostClusteredEnemy(skillRange);
                        if (enemy == null) return; // 안전 장치

                        CastSkill(enemy, index, modifiedDamage);

                        break;
                    }
                case 6: // 환표유영(幻鏢游影)
                    {
                        // 사거리(attackRange) 내 가장 가까운 적에게 시전된다
                        Enemy enemy = enemyManager.FindNearestEnemy();
                        if (enemy == null) return; // 적이 없으면 공격 X
                        CastSkill(enemy, index, modifiedDamage);
                        break;
                    }
            }
        }

        // 스킬을 시전하는 함수 - Tracking, EnemyOn 스킬들을 ~
        private void CastSkill(Enemy enemy, int index, float? modifiedDamage = null)
        {
            switch (index)
            {
                // 매커니즘 : 무작위 적에게 시전 뒤 스킬로 끌어당긴다
                case 4: // 염화매혹(炎華魅惑)
                    {
                        if (!isBossAppear)
                        {
                            skill = poolManager.GetSkill(5) as BlueLotus;
                        }
                        else
                        {
                            skill = poolManager.GetSkill(6) as BlueLotus_Boss;
                        }

                        Vector2 adjustedPosition = skill.GetAdjustedEnemyPosition(enemy);
                        skill.enemy = enemy;
                        // 위치 적용
                        if (enemy is Boss)
                        {
                            skill.X = adjustedPosition.x + UnityEngine.Random.Range(-2f, 2f);
                            skill.Y = adjustedPosition.y + UnityEngine.Random.Range(-1.25f, 1.25f);
                        }
                        else
                        {
                            skill.X = adjustedPosition.x;
                            skill.Y = adjustedPosition.y;
                        }
                        skill.isDotDamageSkill = true;
                        ApplySkillSettings(skill, index);
                        if (modifiedDamage.HasValue) skill.Damage = modifiedDamage.Value; // 쉐파 데미지

                        AudioManager.instance.PlaySfx(AudioManager.Sfx.BlueLotus);
                        break;
                    }
                case 5: // 마령등사 (魔靈藤絲), 신규 스킨
                    {
                        skill = poolManager.GetSkill(7) as Snare;

                        Vector2 adjustedPosition = skill.GetAdjustedEnemyPosition(enemy);
                        // 스킬 변수 할당
                        skill.enemy = enemy;
                        // 위치 적용
                        skill.X = adjustedPosition.x;
                        skill.Y = adjustedPosition.y;

                        skill.isDotDamageSkill = true;
                        ApplySkillSettings(skill, index);
                        if (modifiedDamage.HasValue) skill.Damage = modifiedDamage.Value; // 쉐파 데미지

                        AudioManager.instance.PlaySfx(AudioManager.Sfx.Snare);
                        break;
                    }
                case 6: // 환표유영(幻鏢游影)
                    {
                        skill = poolManager.GetSkill(9) as PhantomShuriken;

                        // 스킬 방향 설정
                        Vector2 playerPosition = PlayerManager.player.transform.position;

                        // 스킬 위치 설정
                        skill.X = playerPosition.x;
                        skill.Y = playerPosition.y;

                        // 스킬 변수 할당
                        skill.enemy = enemy;
                        ((EnemyTrackingSkill)skill).SetTrackingDirection();
                        ((PhantomShuriken)skill).InitialzeSpeed();

                        skill.isDotDamageSkill = true;
                        ApplySkillSettings(skill, index);
                        if (modifiedDamage.HasValue) skill.Damage = modifiedDamage.Value; // 쉐파 데미지

                        AudioManager.instance.PlaySfx(AudioManager.Sfx.PhantomShuriken);

                        break;
                    }
                case 7:  // 연혈상탄 (連血霜彈)
                    {
                        skill = poolManager.GetSkill(10) as Desiccation;

                        Vector2 adjustedPosition = skill.GetAdjustedEnemyPosition(enemy);
                        // 위치 적용
                        if (enemy is Boss)
                        {
                            skill.X = adjustedPosition.x + UnityEngine.Random.Range(-0.5f, 01.5f);
                            skill.Y = adjustedPosition.y + UnityEngine.Random.Range(-1.0f, 1.0f);
                        }
                        else
                        {
                            skill.X = adjustedPosition.x + UnityEngine.Random.Range(-0.2f, 0.2f);
                            skill.Y = adjustedPosition.y + UnityEngine.Random.Range(-0.3f, 0.3f);
                        }

                        // 스킬 변수 할당
                        skill.enemy = enemy;
                        skill.isDotDamageSkill = true;
                        ApplySkillSettings(skill, index);
                        if (modifiedDamage.HasValue) skill.Damage = modifiedDamage.Value; // 쉐파 데미지

                        AudioManager.instance.PlaySfx(AudioManager.Sfx.Desiccation);
                        break;
                    }
                case 8: // 은안지사 (隱眼之蛇)
                    {
                        skill = poolManager.GetSkill(11) as DeathStrikeSeal;

                        Vector2 adjustedPosition = skill.GetAdjustedEnemyPosition(enemy);
                        // 위치 적용
                        if (enemy is Boss)
                        {
                            skill.X = adjustedPosition.x + UnityEngine.Random.Range(-1.0f, 1.0f);
                            skill.Y = adjustedPosition.y + UnityEngine.Random.Range(-0.5f, 0.5f);
                        }
                        else
                        {
                            skill.X = adjustedPosition.x + UnityEngine.Random.Range(-1.0f, 1.0f);
                            skill.Y = adjustedPosition.y + UnityEngine.Random.Range(-0.2f, 0.2f);
                        }

                        // 스킬 변수 할당
                        skill.enemy = enemy;
                        skill.isDotDamageSkill = true;
                        ApplySkillSettings(skill, index);
                        if (modifiedDamage.HasValue) skill.Damage = modifiedDamage.Value; // 쉐파 데미지

                        AudioManager.instance.PlaySfx(AudioManager.Sfx.DeathStrikeSeal);
                        break;
                    }
                case 9: // 파멸각인(滅亡刻印)
                    {
                        skill = poolManager.GetSkill(12) as SpellSword;

                        Vector2 adjustedPosition = skill.GetAdjustedEnemyPosition(enemy);
                        // 위치 적용
                        if (enemy is Boss)
                        {
                            skill.X = adjustedPosition.x + UnityEngine.Random.Range(-0.7f, 0.7f);
                            skill.Y = adjustedPosition.y + UnityEngine.Random.Range(-0.3f, 0.3f);
                        }
                        else
                        {
                            skill.X = adjustedPosition.x + UnityEngine.Random.Range(-1.0f, 1.0f);
                            skill.Y = adjustedPosition.y + UnityEngine.Random.Range(-0.3f, 0.3f);
                        }

                        // 스킬 변수 할당
                        skill.enemy = enemy;
                        ((SpellSword)skill).skillLevel = skillData.level[index];
                        if (enemy is not Boss)
                        {
                            ((SpellSword)skill).debuffDamage = modifiedDamage ?? skillData.damage[index];
                            ((SpellSword)skill).debuffTime = skillData.aliveTime[9] + ((SpellSword)skill).frameAliveTime_Start;
                        }

                        ApplySkillSettings(skill, index);
                        if (modifiedDamage.HasValue) skill.Damage = modifiedDamage.Value; // 쉐파 데미지

                        AudioManager.instance.PlaySfx(AudioManager.Sfx.SpellSword);
                        break;
                    }
                // 신 기술 : 많이 모인 적으로 ..
                case 10:
                    {
                        skill = poolManager.GetSkill(13) as SpiritOfTheWild;

                        Vector2 adjustedPosition = skill.GetAdjustedEnemyPosition(enemy);
                        // 위치 적용
                        if (enemy is Boss)
                        {
                            skill.X = adjustedPosition.x + UnityEngine.Random.Range(-1.05f, 1.05f);
                            skill.Y = adjustedPosition.y + UnityEngine.Random.Range(-0.85f, 0.85f);
                        }
                        else
                        {
                            skill.X = adjustedPosition.x + UnityEngine.Random.Range(-1.05f, 1.05f);
                            skill.Y = adjustedPosition.y + UnityEngine.Random.Range(-0.25f, 0.25f);
                        }

                        // 스킬 위치 설정
                        float tmpX = enemy.transform.position.x;
                        float tmpY = enemy.transform.position.y;

                        float ranNum = UnityEngine.Random.Range(-1.05f, 1.05f);
                        float ranNum2 = UnityEngine.Random.Range(-0.25f, 0.25f);

                        tmpX += ranNum;
                        tmpY += ranNum2;

                        skill.X = tmpX;
                        skill.Y = tmpY;

                        // 스킬 변수 할당
                        skill.enemy = enemy;
                        skill.isDotDamageSkill = true;
                        ApplySkillSettings(skill, index);
                        ((SpiritOfTheWild)skill).SetAnimatorSpeed(); // 애니메이션 속도 설정
                        if (modifiedDamage.HasValue) skill.Damage = modifiedDamage.Value; // 쉐파 데미지

                        AudioManager.instance.PlaySfx(AudioManager.Sfx.SpiritOfTheWild);
                        break;
                    }
                case 11: // 혼마화진 (混魔花陣)
                    {
                        skill = poolManager.GetSkill(14) as ArcaneHeart;

                        Vector2 adjustedPosition = skill.GetAdjustedEnemyPosition(enemy);
                        // 위치 적용
                        if (enemy is Boss)
                        {
                            skill.X = adjustedPosition.x + UnityEngine.Random.Range(-1.15f, 1.15f);
                            skill.Y = adjustedPosition.y + UnityEngine.Random.Range(-0.55f, 0.55f);
                        }
                        else
                        {
                            skill.X = adjustedPosition.x + UnityEngine.Random.Range(-0.65f, 0.65f);
                            skill.Y = adjustedPosition.y + UnityEngine.Random.Range(-0.05f, 0.05f);
                        }

                        // 스킬 변수 할당
                        skill.enemy = enemy;
                        if (enemy is Boss boss)
                        {
                            ((ArcaneHeart)skill).level = skillData.level[index];
                        }
                        ((ArcaneHeart)skill).MakeIllusion();
                        ((ArcaneHeart)skill).SettingIllusion();
                        ((ArcaneHeart)skill).onIllusionWasRemoved = OnIllusionWasRemoved;
                        illusionActionList.Add(((ArcaneHeart)skill).assassin_Illusion);
                        allocateIllusionAction.Invoke(illusionActionList);
                        skill.isDotDamageSkill = true;
                        ApplySkillSettings(skill, index);
                        if (modifiedDamage.HasValue) skill.Damage = modifiedDamage.Value; // 쉐파 데미지

                        AudioManager.instance.PlaySfx(AudioManager.Sfx.ArcaneHeart);
                        break;
                    }
            }
        }

        // 스킬을 시전하는 함수
        private void CastSkill(int index, float? modifiedDamage = null)
        {
            switch (index)
            {
                case 0: // 단검연격(短劍連擊)
                    {
                        float angleStep;

                        if (skillData.level[0] == 5)
                        {
                            angleStep = -40f;
                            float angleCoefficient = 20f;
                            for (int i = 0; i < 5; i++)
                            {
                                skill = poolManager.GetSkill(0) as Dagger;

                                // 스킬 flip 보정
                                skill.transform.GetComponent<SpriteRenderer>().flipY = PlayerManager.player.isPlayerLookLeft;

                                // 스킬 방향 설정
                                if(skill is Dagger dagger)
                                {
                                    dagger.direction = Quaternion.Euler(0, 0, angleStep) * dagger.direction;
                                    angleStep += angleCoefficient;
                                    float angle = Mathf.Atan2(dagger.direction.y, dagger.direction.x) * Mathf.Rad2Deg;
                                    Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.forward);
                                    Quaternion rotation = Quaternion.Slerp(skill.transform.rotation, angleAxis, 5f);
                                    skill.transform.rotation = rotation;
                                }

                                // 스킬 변수 할당
                                ApplySkillSettings(skill, index);
                                if (modifiedDamage.HasValue) skill.Damage = modifiedDamage.Value; // 쉐파 데미지

                                if (i % 2 == 0) AudioManager.instance.PlaySfx(AudioManager.Sfx.Dagger);
                            }
                        }
                        else if (skillData.level[0] >= 3)
                        {
                            angleStep = -20f;
                            float angleCoefficient = 20f;
                            for (int i = 0; i < 3; i++)
                            {
                                skill = poolManager.GetSkill(0) as Dagger;

                                // 스킬 flip 보정
                                skill.transform.GetComponent<SpriteRenderer>().flipY = PlayerManager.player.isPlayerLookLeft;

                                // 스킬 방향 설정
                                if (skill is Dagger dagger)
                                {
                                    dagger.direction = Quaternion.Euler(0, 0, angleStep) * dagger.direction;
                                    angleStep += angleCoefficient;
                                    float angle = Mathf.Atan2(dagger.direction.y, dagger.direction.x) * Mathf.Rad2Deg;
                                    Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.forward);
                                    Quaternion rotation = Quaternion.Slerp(skill.transform.rotation, angleAxis, 5f);
                                    skill.transform.rotation = rotation;
                                }

                                // 스킬 변수 할당
                                ApplySkillSettings(skill, index);
                                if (modifiedDamage.HasValue) skill.Damage = modifiedDamage.Value; // 쉐파 데미지

                                if (i % 2 == 0) AudioManager.instance.PlaySfx(AudioManager.Sfx.Dagger);
                            }
                        }
                        else
                        {
                            skill = poolManager.GetSkill(0) as Dagger;

                            // 스킬 flip 보정
                            skill.transform.GetComponent<SpriteRenderer>().flipY = PlayerManager.player.isPlayerLookLeft;

                            // 스킬 방향 설정
                            if (skill is Dagger dagger)
                            {
                                dagger.direction = Quaternion.Euler(0, 0, 0) * dagger.direction;
                                float angle = Mathf.Atan2(dagger.direction.y, dagger.direction.x) * Mathf.Rad2Deg;
                                Quaternion angleAxis = Quaternion.AngleAxis(angle, Vector3.forward);
                                Quaternion rotation = Quaternion.Slerp(skill.transform.rotation, angleAxis, 5f);
                                skill.transform.rotation = rotation;
                            }

                            // 스킬 변수 할당
                            ApplySkillSettings(skill, index);
                            if (modifiedDamage.HasValue) skill.Damage = modifiedDamage.Value; // 쉐파 데미지

                            AudioManager.instance.PlaySfx(AudioManager.Sfx.Dagger);
                        }
                        break;
                    }
                case 1: // 화염지구(火焰之球)
                    {
                        skill = poolManager.GetSkill(1) as EntropicDecay;

                        // 스킬 변수 할당
                        skill.isDotDamageSkill = true;
                        ApplySkillSettings(skill, index);
                        LockSkillDuringCast(skill, index);
                        if (modifiedDamage.HasValue) skill.Damage = modifiedDamage.Value; // 쉐파 데미지

                        AudioManager.instance.PlaySfx(AudioManager.Sfx.EntropicDecay);
                        break;
                    }
                case 2: // 독운투척(毒雲投擲)
                    {
                        int potionNum = skillData.level[index] switch
                        {
                            < 0 => throw new ArgumentOutOfRangeException(skillData.level[index].ToString(), "스킬 레벨이 0미만 입니다."), // 0 미만 체크 추가
                            <= 2 => 1,
                            <= 4 => 2,
                            >= 5 => 3,
                        };

                        for (int i = 0; i < potionNum; i++)
                        {
                            skill = poolManager.GetSkill(2) as PoisonPotion;

                            if (skill is PoisonPotion poisonPotion)
                            {
                                // 스킬 위치 설정
                                float tmpX = poisonPotion.mousePosition.x;
                                float tmpY = poisonPotion.mousePosition.y;
                                float ranNum; float ranNum2;

                                if (i != 0)
                                {
                                    ranNum = UnityEngine.Random.Range(-1.5f, 1.5f); // x 좌표 랜덤 크기
                                    ranNum2 = UnityEngine.Random.Range(-0.85f, 0.85f); // y 좌표 랜덤 크기
                                    tmpX += ranNum; tmpY += ranNum2;
                                }

                                poisonPotion.endPosition = new Vector2(tmpX, tmpY);
                                poisonPotion.potionAliveTime = potionAliveTime;

                                poisonPotion.CalculateVariable();
                            }

                            // 스킬 변수 할당
                            skill.isDotDamageSkill = true;
                            ApplySkillSettings(skill, index);
                            LockSkillDuringCast(skill, index);
                            if (modifiedDamage.HasValue) skill.Damage = modifiedDamage.Value; // 쉐파 데미지
                        }
                        break;
                    }
                case 3: // 청운검격
                    {
                        skill = poolManager.GetSkill(4) as SlashBlue;

                        // 스킬 위치 보정
                        float xPositionNum, yPositionNum;
                        bool isFlipped = true;

                        // 프리팹 위치 보정용
                        if (skillData.level[index] == 5)
                        {
                            xPositionNum = 1.0f;
                        }
                        else if (skillData.level[index] >= 3)
                        {
                            xPositionNum = 0.9f;
                        }
                        else
                        {
                            xPositionNum = 0.8f;
                        }
                        yPositionNum = -0.1f;

                        ApplySkillPositionOffset(skill, index, xPositionNum, yPositionNum, isFlipped);

                        // 스킬 변수 할당
                        ((PlayerAttachSkill)skill).isAttachSkill = true;
                        skill.isDotDamageSkill = true;
                        ApplySkillSettings(skill, index);
                        LockSkillDuringCast(skill, index);
                        if (modifiedDamage.HasValue) skill.Damage = modifiedDamage.Value; // 쉐파 데미지

                        AudioManager.instance.PlaySfx(AudioManager.Sfx.SlashBlue);
                        break;
                    }
            }
        }

        // 스킬 변수 설정
        protected override void ApplySkillSettings(Skill skill, int index)
        {
            skill.skillIndex = index;
            SetScale(skill.gameObject, index);
            skill.onSkillAttack = OnSkillAttack;

            skill.Initialize(skillData, server_PlayerData, playerData, passiveSkillData, passiveMijiSkillData);
        }

        private void OnIllusionWasRemoved(Assassin_Illusion assassin_Illusion)
        {
            illusionActionList.Remove(assassin_Illusion);
        }

        public void SetShadowState(bool isShadowExist)
        {
            isShadowActive = isShadowExist;
        }

        // amount 만큼 쿨타임 감소 (10퍼 감소면 0.1)
        public void ReduceCooldown(float amount)
        {
            for(int i = 0; i < skillData.delay.Length; i++)
            {
                skillData.delay[i] *= 1 - amount; // 쿨타임 줄이기
            }
        }
    }
}