using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Eclipse.Game
{
    public class SkillManager : Eclipse.Manager
    {
        //==================================================================
        protected PoolManager poolManager;
        protected EnemyManager enemyManager;

        //==================================================================
        [ReadOnly] public SkillData2 skillData;
        [ReadOnly] public SkillData2 passiveSkillData;
        [ReadOnly] public Server_PlayerData server_PlayerData;
        [ReadOnly] public PlayerData playerData;

        //==================================================================
        public int MAX_SKILL_NUM;

        // ��ų ���� �迭�� ���� ����
        // ��ų�� index: �� - 3n, ���� - 3n + 1, �� - 3n + 2
        // �� - 0, 3, 6, 9 / ���� - 1, 4, 7, 10 / �� - 2, 5, 8 ,11
        [SerializeField] protected float[] attackDelayTimer;
        [SerializeField] public int[] damageMeters;
        [SerializeField] protected bool[] isSkillsCasted;

        //==================================================================
        protected string sceneName; // 씬 이름을 저장할 변수

        //==================================================================

        protected void Start()
        {
            poolManager = client.GetManager<PoolManager>();
            enemyManager = client.GetManager<EnemyManager>();
        }

        public virtual void Init()
        {

        }

        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        // 체인을 걸어서 이 함수는 매 씬마다 호출된다.
        protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            sceneName = scene.name;
        }

        // ��Ÿ�� �ʱ�ȭ �Լ�
        public void ResetDelayTimer(int index)
        {
            attackDelayTimer[index] = 0;
        }

        protected void OnSkillAttack(int index, float damage)
        {
            damageMeters[index] += (int)damage;
        }

        public int[] ReturnDamageMeters()
        {
            return damageMeters;
        }

        // 스킬 변수 설정
        protected virtual void ApplySkillSettings(Skill skill, int index)
        {
            skill.skillIndex = index;
            SetScale(skill.gameObject, index);
            skill.onSkillAttack = OnSkillAttack;

            skill.Initialize(skillData, server_PlayerData, playerData);
        }

        // 스킬 스케일 설정
        protected void SetScale(GameObject gameObject, int index)
        {
            Vector2 size = gameObject.GetComponent<Collider2D>().bounds.size;
            float scaleMultiplier = server_PlayerData.basicPassiveLevels[5] * server_PlayerData.attackRange;

            Transform parent = gameObject.transform.parent;

            gameObject.transform.parent = null;
            gameObject.transform.localScale = new Vector3(
                skillData.scale[index] * (1 + scaleMultiplier),
                skillData.scale[index] * (1 + scaleMultiplier),
                0);
            gameObject.transform.parent = parent;
        }

        // 스킬 사용중에 또 시전되면 안되는 스킬들 설정
        protected void LockSkillDuringCast(Skill skill, int index)
        {
            skill.onSkillFinished = OnSkillFinished;
            isSkillsCasted[index] = true;
        }

        // 스킬이 꺼질 때 스킬이 delegate를 통해 호출 할 함수
        protected void OnSkillFinished(int index)
        {
            isSkillsCasted[index] = false;
        }

        // PlayerAttachSkill들 Offset, flip 설정
        protected void ApplySkillPositionOffset(Skill skill, int index, float xOffset, float yOffset, bool? isXFlipped = null, bool? isYFlipped = null)
        {
            if (skill is PlayerAttachSkill playerAttachSkill)  // 'is' + 패턴 매칭을 사용한 안전한 캐스팅
            {
                /*
                    플레이어와 스킬의 간격을 유지하려면,
                    새로운 Offset = 기존 Offset + (실제 size * (scaleMultiplier + 1) - scale);
                    즉, 새로운 Offset = 기존 Offset + (실제 size * scaleMultiplier);
                */
                StartCoroutine(WaitForFixedUpdate());

                IEnumerator WaitForFixedUpdate() // bounds.size가 FixedUpdate에서만 정확히 계산되므로, FixedUpdate가 끝날 때까지 기다린다.
                {
                    yield return new WaitForFixedUpdate();

                    float scaleMultiplier = server_PlayerData.basicPassiveLevels[5] * server_PlayerData.attackRange;
                    Vector2 size = skill.GetComponent<Collider2D>().bounds.size; // bounds.size를 하면 scale이 고려된 실제 사이즈가 반환된다.
                    playerAttachSkill.xOffset = xOffset * (1 + size.x * scaleMultiplier);
                    playerAttachSkill.yOffset = yOffset * (1 + size.y * scaleMultiplier);
                    if (isXFlipped.HasValue)
                        playerAttachSkill.isFlipped = isXFlipped.Value;

                    if (isYFlipped.HasValue)
                        playerAttachSkill.isYFlipped = isYFlipped.Value;
                }
            }
            else
            {
                Debug.LogWarning($"ApplySkillPositionOffset: {skill.GetType().Name}은 PlayerAttachSkill이 아닙니다.");
            }
        }
    }
}