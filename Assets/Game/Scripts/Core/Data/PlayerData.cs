using Eclipse.Game;
using UnityEngine;

namespace Eclipse
{
    [CreateAssetMenu(fileName = "PlayerData", menuName = "Scriptable Object/PlayerData")]

    public class PlayerData : ScriptableObject
    {
        private Server_PlayerData server_PlayerData;
        private SkillData2 passiveSkillData;
        private DefaultPlayerData defaultPlayerData;

        public void Initialize(Server_PlayerData server_PlayerData,
            SkillData2 passiveSkillData, DefaultPlayerData defaultPlayerData)
        {
            this.server_PlayerData = server_PlayerData;
            this.passiveSkillData = passiveSkillData;
            this.defaultPlayerData = defaultPlayerData;
        }

        [Header("# 계산식에 사용되지 않은 플레이어 변수")]
        public float hp;
        public float Exp;
        public int level;
        public int[] nextExp;
        public int kill; // 몬스터 킬 수

        // 전사 패시브 효과 관련 변수
        [Header("# 전사 패시브 효과")]
        public float holyReductionValue;

        // 플레이어 정보
        [Header("# 플레이어 정보")]

        [SerializeField] private float _maxHp;
        public float maxHp
        {
            get
            {
                _maxHp = defaultPlayerData.maxHp
                    + server_PlayerData.basicPassiveLevels[2] * server_PlayerData.maxHealth;
                return _maxHp;
            }
        }

        [SerializeField] private float _healthRegen;
        public float healthRegen
        {
            get
            {
                _healthRegen = defaultPlayerData.healthRegen
                    + server_PlayerData.basicPassiveLevels[3] * server_PlayerData.regen;
                return _healthRegen;
            }
        }

        // 플레이어 패시브 효과 관련 변수
        [Header("# 플레이어 공통 패시브 효과")]

        public float speed_Additional;
        [SerializeField] private float _speed;
        public float speed
        {
            get
            {
                _speed = defaultPlayerData.speed * passiveSkillData.damage[4]
                    + server_PlayerData.basicPassiveLevels[7] * server_PlayerData.moveSpeed
                    + speed_Additional;

                return _speed;
            }
        }

        [SerializeField] private float _damageReductionValue;
        public float damageReductionValue
        {
            get
            {
                _damageReductionValue = 1 - (defaultPlayerData.damageReductionValue + passiveSkillData.damage[3]
                    + server_PlayerData.basicPassiveLevels[1] * server_PlayerData.defense);
                return _damageReductionValue;
            }
        }

        public float magnetRange_Additional;
        [SerializeField] private float _magnetRange;
        public float magnetRange
        {
            get
            {
                _magnetRange = defaultPlayerData.magnetRange + passiveSkillData.damage[5]
                    + server_PlayerData.specialPassiveLevels[0] * server_PlayerData.magnetPower
                    + magnetRange_Additional;
                return _magnetRange;
            }
        }

        [SerializeField] private float _additional_Power;
        public float additional_Power
        {
            get
            {
                return _additional_Power;
            }
            set
            {
                _additional_Power = value;
            }
        }
    }
}