using UnityEngine;

namespace Eclipse
{
    [CreateAssetMenu(fileName = "Server_PlayerData", menuName = "Scriptable Object/Server_PlayerData")]

    // 해당 클래스는 서버에 이전하기 전 저장용 클래스임
    public class Server_PlayerData : ScriptableObject
    {
        // 유저 아이디
        [Header("유저 아이디")]
        public string Email;
        public string Password;
        public string UserName;

        // 캐릭터 해금 정보
        [Header("해금된 캐릭터")]
        public bool isWarriorUnlocked;  // 전사
        public bool isAssassinUnlocked; // 도적

        // 레벨에 따른 골드량
        [Header("#골드 정보")]
        public int playerGold;
        public int increasedPrice;
        public int[] startPrice = new int[15];
        public int priceIncreseNum;
        public int coin;

        // 기초 패시브 효과
        [Header("# 기초 패시브 효과")]
        public int[] basicPassiveLevels = new int[8];
        public int[] basicPassiveLevelsMaxNum = new int[8];
        public float attackPower;          // 공격력
        public float defense;              // 방어력
        public float maxHealth;            // 최대 체력
        public float regen;                // 회복
        public float cooldown;             // 쿨타임 감소
        public float attackRange;          // 공격 범위
        public float duration;             // 지속시간
        public float moveSpeed;            // 이동속도

        // 특수 패시브 효과
        [Header("# 특수 패시브 효과")]
        public int[] specialPassiveLevels = new int[7];
        public int[] specialPassiveLevelsMaxNum = new int[7];
        public float magnetPower;         // 자석 효과
        public float luck;                // 행운 (경험치, 상자 및 포션 등장 확률, 상태이상 확률 증가)
        public float expBoost;            // 성장 (경험치 획득량 증가)
        public float goldBoost;           // 풍요 (골드 획득량 증가)
        public float nightmareMode;       // 악몽 (난이도 증가)
        public int canRevive;             // 부활 가능 여부
        public int rerollCount;           // 다시 뽑기 가능 횟수
    }
}
