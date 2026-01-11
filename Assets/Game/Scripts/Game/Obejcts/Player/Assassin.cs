using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Eclipse.Game
{
    public class Assassin : Player
    {
        int probability = 101;

        bool isDodge;

        // 어쌔신이 10레벨이 돼서 비급 찍어야된다고 알려주기 위한 delegate
        public Action onMijiSkillMustSelect;


        public override void Init()
        {
            base.Init();


            if(SceneManager.GetActiveScene().name == "Stage1")
            {
                ((AssassinData)playerData).dodgeRate = 1f;
                ((AssassinData)playerData).criticalDamageCoefficient = 2f; // 기본 치피 200%
            }
        }

        protected override IEnumerator LevelUP(int depth)
        {
            if (getClinetIsChangingScene()) yield break;

            isSkillSelectComplete = false;

            playerData.Exp -= playerData.nextExp[playerData.level];
            playerData.Exp = playerData.Exp < 0 ? 0 : playerData.Exp;
            playerData.level++;

            if (playerData.level % 10 == 0 && playerData.level <= 50)
            {
                // 10레벨에 도달했을 때 Miji 스킬 선택 상태로 전환
                onMijiSkillMustSelect?.Invoke(); // 어쌔신 비급 스킬 선택 알림
            }
            else
            {
                onPlayerLevelUP(); // delegate 호출
            }
            
            yield return new WaitUntil(() => isSkillSelectComplete);
            // 경험치를 경험치 통보다 많이 갖고 있으면 재귀적으로 반복
            bool isAgain = playerData.Exp >= playerData.nextExp[playerData.level];
            if (isAgain) StartCoroutine(LevelUP(depth + 1));
        }

        protected override void TakeDamage(float damage, bool isDamageUnavoidable)
        {
            if (!isPlayerDead)
            {
                float dodgeCoefficient = (((AssassinData)playerData).dodgeRate * 100) - 100;

                float ranNum = UnityEngine.Random.Range(1, probability);
                isDodge = ranNum <= dodgeCoefficient;

                if (isDodge)
                {
                    bool isDodgeDelayOK = hitDelayTimer >= hitDelayTime;
                    if (isDodgeDelayOK)
                    {
                        hitDelayTimer = 0;
                    }
                    return;
                }
                else
                {
                    bool isHitDelayOK = hitDelayTimer >= hitDelayTime;
                    if (isHitDelayOK || isDamageUnavoidable)
                    {
                        playerData.hp -= damage * playerData.damageReductionValue;

                        AudioManager.instance.PlaySfx(AudioManager.Sfx.Melee0); // 피격 효과음
                        hitDelayTimer = 0;
                    }
                }

                if (playerData.hp < playerData.maxHp * 0.3f && !isBloodWindowAlreadyDisplayed)
                {
                    StartCoroutine(BloodWindowOn());
                    isBloodWindowAlreadyDisplayed = true;
                }

                if (playerData.hp <= 0)
                {
                    Die();
                }
                else
                {
                    bloodEffectObejct.PlayParticle();

                    isHit = true; // 피격 상태로 변경
                    hitDurationTimer = 0f; // 피격 색깔로 변경하는 타이머 초기화
                    if (hitColorCoroutine == null)
                    {
                        hitColorCoroutine = StartCoroutine(ChangeToHitColor()); // 피격 이펙트 색깔로 변경
                    }
                }
            }
        }
    }
}
