using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Eclipse.Game
{
    public class Miji_SkillManager : Eclipse.Manager
    {
        public static Miji_SkillManager instance
        {
            get
            {
                // 인스턴스가 없는 경우에 접근하려 하면 인스턴스를 할당해준다.
                if (!_instance)
                {
                    _instance = FindAnyObjectByType(typeof(Miji_SkillManager)) as Miji_SkillManager;
                }
                return _instance;
            }
        }

        // 싱글톤 패턴을 사용하기 위한 인스턴스 변수
        private static Miji_SkillManager _instance;

        //==================================================================
        PoolManager poolManager;
        StageManager stageManager;

        Transform _playerTransform; // 플레이어의 Transform
        Transform playerTransform
        {
            get
            {
                if (_playerTransform == null)
                {
                    _playerTransform = PlayerManager.player.transform;
                }
                return _playerTransform;
            }
            set
            {
                _playerTransform = value;
            }
        }

        private GameObject _stealthEffect; // 은신 이펙트 오브젝트
        private GameObject stealthEffect
        {
            get
            {
                if (_stealthEffect == null)
                {
                    _stealthEffect = playerTransform.GetChild(6).gameObject; // 플레이어의 자식 오브젝트 중 6번째
                }
                return _stealthEffect;
            }
            set
            {
                _stealthEffect = value;
            }
        }
        private Stealth _stealth; // 은신 스킬
        private Stealth stealth
        {
            get
            {
                if (_stealth == null)
                {
                    _stealth = stealthEffect.GetComponent<Stealth>();
                }
                return _stealth;
            }
            set
            {
                _stealth = value;
            }
        }

        //==================================================================
        // 비급 데이터
        [ReadOnly] public MijiSkillData buttonActiveMijiSkillData; // 버튼 액티브 스킬
        [ReadOnly] public MijiSkillData activeMijiSkillData; // 액티브 스킬
        [ReadOnly] public MijiSkillData passiveMijiSkillData; // 찐 패시브 스킬

        [SerializeField] private Assassin_Illusion shadow;
        [ReadOnly] public IllusionData illusionData;

        //=================================================================
        // 스킬 클래스 객체들
        private Skill skill;
        private CameraBoundingSkill cameraBoundingSkill;

        //==================================================================
        // 캐스팅 가능한 스킬들 개수(버튼 액티브, 액티브 비급들)
        const int MAX_CASTABLE_SKILLS = 5;

        const int MAX_BUTTONACTIVE_SKILLS = 4;
        const int MAX_ACTIVE_SKILLS = 1;

        // 스킬 시전 중인 지 판단
        [SerializeField] private bool[] isButtonActiveSkillsCasted;
        private bool[] isActiveSkillCasted;

        // 패널 별 위치 인덱스
        [SerializeField] int[] buttonActiveSkillsPosIndices; // 버튼 액티브 스킬들 위치 인덱스 모음
        [SerializeField] int activeSkillPosIndex; // 사출기 스킬 위치 인덱스

        /* [사출기 관련] */
        List<Enemy> firstTimeNearestEnemies = new List<Enemy>();
        List<Enemy> nearestEnemies = new List<Enemy>();

        public bool isBossAppear;

        //==================================================================

        [SerializeField] private int[] damageMeters;

        // 쿨타임
        [SerializeField] private float[] buttonActiveCoolTimes; // 버튼 액티브 비급
        [SerializeField] private float[] activeCoolTimes; // 액티브 비급 : 사출기 1개여도 일단 리스트로 ..
        // 스킬 인덱스 리스트
        [SerializeField] private List<int> buttonActiveSkillIndices; // 보유 가능한 버튼 액티브 비급 인덱스 리스트
        [SerializeField] private List<int> activeSkillIndices; // 자동 시전할 액티브 스킬(사출기) 인덱스 리스트

        [SerializeField] private float[] buttonActiveAliveTimes;
        [SerializeField] private float[] buttonActiveCoefficients;

        [SerializeField] private float[] activeDamages;
        [SerializeField] private float[] activeLifes;

        private bool isStealthActive = false; // 은신 활성화 여부
        private float stealthRemainingTime; // 남은 은신 시간

        private bool isShadowActive = false; // 환영분신 활성화 여부
        private float shadowRemainingTime; // 남은 환영분신 시간

        //==================================================================
        // 비급 이펙트(어쌔신 전용)
        private GameObject mijiEffect;
        private Animator mijiEffectAnimator;

        //==================================================================
        /* Action 모음 */

        public Action<float> onDodgeOn;
        public Action<float> onDodgeOff;
        public Action<float> onSelectPassiveMijiSkil;

        // 스킬 시전 정보를 전달하기 위한 액션
        public Action<int, float> onButtonActiveSkillCasted; // (스킬 인덱스, 쿨타임)
        public Action<int, float> onActiveSkillCasted; // (스킬 인덱스, 쿨타임)

        public Action<bool> onStealthStateChanged; // 은신 상태인지 알려주는 액션
        public Action<bool> onShadowStateChanged; // 환영분신 존재 여부 알려주는 액션

        public Action onStopAllCooldownCoroutines; // 스테이지 클리어(3분, 6분) 시 쿨타임 멈추게 하기
        public Action<int, float, float> onContinueButtonActiveSkillCooldown;
        public Action<int, float, float> onContinueActiveSkillCooldown;


        //==================================================================
        /* 비급 목록
         * 
         * 버튼 액티브 비급
         * [buttonActiveSkillData가 담당]
         * (Index : 0) 환영분신
         * (Index : 1) 질풍순보
         * (Index : 2) 암연장막
         * (Index : 3) 흑야은신
         * 
         * 액티브 비급
         * [activeSkillData가 담당]
         * (Index : 0) 
         * 
         * 찐 패시브 비급
         * [passiveSkillData가 담당]
         * (Index : 0) 살인귀혼
         * (Index : 1) 풍신잠행
         * (Index : 2) 암흑기습
         * 
         */

        //==================================================================

        private void Awake()
        {
            isButtonActiveSkillsCasted = new bool[MAX_BUTTONACTIVE_SKILLS];
            isActiveSkillCasted = new bool[MAX_ACTIVE_SKILLS];

            buttonActiveCoolTimes = new float[MAX_BUTTONACTIVE_SKILLS];
            activeCoolTimes = new float[MAX_ACTIVE_SKILLS];

            buttonActiveSkillsPosIndices = new int[MAX_BUTTONACTIVE_SKILLS];

            buttonActiveAliveTimes = new float[MAX_BUTTONACTIVE_SKILLS];
            buttonActiveCoefficients = new float[MAX_BUTTONACTIVE_SKILLS];

            activeDamages = new float[MAX_ACTIVE_SKILLS];
            activeLifes = new float[MAX_ACTIVE_SKILLS];

            damageMeters = new int[MAX_CASTABLE_SKILLS];
        }


        private void Start()
        {
            poolManager = client.GetManager<PoolManager>();
            stageManager = client.GetManager<StageManager>();
        }

        void OnEnable()
        {
            // 씬 매니저의 sceneLoaded에 체인을 건다.
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            // 씬 매니저의 sceneLoaded에 체인을 푼다.
            SceneManager.sceneLoaded -= OnSceneLoaded;

        }

        // 체인을 걸어서 이 함수는 매 씬마다 호출된다.
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // Stage1 - Stage2, Stage2 - Stage3 넘어갈 때 쿨타임 저장 후 Stage2, Stage3일때 저장한 쿨타임 할당

            string sceneName = scene.name;

            if (sceneName == "Stage1")
                InitSkillData();

            if (sceneName == "Stage2" || sceneName == "Stage3")
                Init();
        }

        private void InitSkillData()
        {
            // 버튼 액티브 스킬 인덱스 초기화
            buttonActiveSkillIndices = new List<int> { -1, -1 };
            // 자동 시전 액티브 스킬 인덱스 초기화
            activeSkillIndices = new List<int> { -1 }; // 최대 1개의 스킬만 보유 가능

            stealthRemainingTime = 0;

            for (int i = 0; i < buttonActiveSkillsPosIndices.Length; i++)
            {
                buttonActiveSkillsPosIndices[i] = -1;
            }
            activeSkillPosIndex = -1;


            for (int i = 0; i < buttonActiveMijiSkillData.skillName.Length; i++)
            {
                isButtonActiveSkillsCasted[i] = false;
                buttonActiveCoolTimes[i] = 0;
                buttonActiveAliveTimes[i] = 0;
                buttonActiveCoefficients[i] = 0;
            }

            for (int i = 0; i < activeMijiSkillData.skillName.Length; i++)
            {
                isActiveSkillCasted[i] = false;
                activeCoolTimes[i] = 0;
                activeDamages[i] = 0;
                activeLifes[i] = 0;
            }

            isBossAppear = false;

            firstTimeNearestEnemies.Clear();
            nearestEnemies.Clear();
        }

        private void Init()
        {
            //==================================================================
            // 버튼 액티브 스킬 상태 복원

            for (int i = 0; i < MAX_BUTTONACTIVE_SKILLS; i++)
            {
                // 이전에 저장된 버튼 액티브 스킬 쿨타임을 불러옵니다. 기본값은 0.
                buttonActiveCoolTimes[i] = PlayerPrefs.GetFloat($"ButtonActiveCoolTime_{i}", 0);
                // 이전에 저장된 스킬이 시전 중인지 여부를 불러옵니다. 1이면 true, 0이면 false.
                isButtonActiveSkillsCasted[i] = PlayerPrefs.GetInt($"ButtonActiveSkillCasted_{i}", 0) == 1;
                // 이전에 저장된 스킬의 액티브 비급 패널의 위치를 불러옵니다. 기본 값은 -1
                buttonActiveSkillsPosIndices[i] = PlayerPrefs.GetInt($"ButtonActiveSkillPosIndex_{i}", -1);

                // 버튼 액티브 스킬 계수와 시전 시간 복원
                buttonActiveAliveTimes[i] = PlayerPrefs.GetFloat($"buttonActiveAliveTimes_{i}", 0);
                buttonActiveCoefficients[i] = PlayerPrefs.GetFloat($"buttonActiveCoefficients{i}", 0);
            }

            // 선택된 버튼 액티브 스킬 중 시전 중 넘어온 스킬 쿨타임 UI 이어서 하기
            for (int i = 0; i < buttonActiveSkillsPosIndices.Length; i++) // 배열 크기만큼만 반복
            {
                if (isButtonActiveSkillsCasted[i] && buttonActiveCoolTimes[i] > 0)
                {
                    if (Array.Exists(buttonActiveSkillsPosIndices, posIndex => posIndex == i))
                    {
                        int index = Array.IndexOf(buttonActiveSkillsPosIndices, i);
                        if (index >= 0 && index < buttonActiveCoolTimes.Length)
                        {
                            onContinueButtonActiveSkillCooldown?.Invoke(index, buttonActiveMijiSkillData.Delay[i], buttonActiveCoolTimes[i]);
                        }
                    }
                }
            }

            //==================================================================
            // 액티브 스킬 인덱스 위치 복원

            // 액티브 스킬의 패널 위치를 불러옵니다. 기본값은 0.
            activeSkillPosIndex = PlayerPrefs.GetInt("ActiveSkillIndex", 0);

            // 액티브 스킬 상태 복원
            for (int i = 0; i < MAX_ACTIVE_SKILLS; i++)
            {
                // 이전에 저장된 액티브 스킬 쿨타임을 불러옵니다. 기본값은 0.
                activeCoolTimes[i] = PlayerPrefs.GetFloat($"ActiveCoolTime_{i}", 0);
                // 이전에 저장된 스킬이 시전 중인지 여부를 불러옵니다. 1이면 true, 0이면 false.
                isActiveSkillCasted[i] = PlayerPrefs.GetInt($"ActiveSkillCasted_{i}", 0) == 1;

                // 액티브 스킬 데미지와 횟수 복원
                activeDamages[i] = PlayerPrefs.GetFloat($"ActiveDamage_{i}", 0);
                activeLifes[i] = PlayerPrefs.GetFloat($"ActiveLife_{i}", 0);

                if (isActiveSkillCasted[i] && activeCoolTimes[i] > 0)
                {
                    if (activeSkillPosIndex >= 0)
                    {
                        onContinueActiveSkillCooldown?.Invoke(activeSkillPosIndex, activeMijiSkillData.Delay[i], activeCoolTimes[i]);
                    }
                }
            }

            isBossAppear = false;

            firstTimeNearestEnemies.Clear();
            nearestEnemies.Clear();

            // 은신 상태 복원
            isStealthActive = PlayerPrefs.GetInt("IsStealthActive", 0) == 1; // 1 이면  T 아니면 F
            stealthRemainingTime = PlayerPrefs.GetFloat("StealthRemainingTime", 0);

            // 만약 은신 상태였고 남은 시간이 있다면 은신 스킬을 다시 지속시킴
            if (isStealthActive && stealthRemainingTime > 0)
            {
                ResumeStealthSkill(stealthRemainingTime);
            }

            // 환영분신 복원
            isShadowActive = PlayerPrefs.GetInt("IsShadowActive", 0) == 1; // 1 이면 T 아니면 F
            shadowRemainingTime = PlayerPrefs.GetFloat("ShadowRemainingTime", 0);

            // 만약 환영분신 상태였고 남은 시간이 있다면 환영분신 스킬을 다시 지속시킴
            if (isShadowActive && shadowRemainingTime > 0)
            {
                ResumeShadowSkill(shadowRemainingTime);
            }
        }

        void Update()
        {
            bool isSplashScene =
                SceneManager.GetActiveScene().name == "Lobby" ||
                SceneManager.GetActiveScene().name == "Splash1" ||
                SceneManager.GetActiveScene().name == "Splash2" ||
                SceneManager.GetActiveScene().name == "Splash3";

            if (isSplashScene) return;

            string sceneName = SceneManager.GetActiveScene().name;

            // Stage가 클리어된 경우 : Stage1과 Stage2에만 저장되게.. 
            if (stageManager.isStageClear
                && (sceneName == "Stage1" || sceneName == "Stage2"))
            {

                if (isStealthActive)
                {
                    // 남아있는 은신 시간을 저장
                    float elapsedTime = stealth.aliveTimer;
                    stealthRemainingTime = buttonActiveAliveTimes[3] - elapsedTime;
                }

                if (isShadowActive)
                {
                    // 남아있는 환영분신 시간을 저장
                    shadowRemainingTime = shadow.aliveTime - shadow.aliveTimer;
                }


                SaveSkillState();
                onStopAllCooldownCoroutines?.Invoke(); // 버튼 액티브/액티브 스킬의 패널 쿨타임 지속시간 멈추게 하기
                return;
            }

            bool isPlayNow = Time.timeScale == 1;

            if (isPlayNow) // Q/E 키 입력이 가능한 경우
            {
                if (Input.GetKeyDown(KeyCode.Q) && buttonActiveSkillIndices[0] != -1)
                {
                    TryCastButtonActiveSkill(0, buttonActiveSkillIndices[0]);
                }
                if (Input.GetKeyDown(KeyCode.E) && buttonActiveSkillIndices[1] != -1)
                {
                    TryCastButtonActiveSkill(1, buttonActiveSkillIndices[1]);
                }
            }

            /* 비급 스킬 쿨타임 관리 */
            // 버튼 액티브 스킬
            for (int i = 0; i < buttonActiveMijiSkillData.skillName.Length; i++)
            {
                if (buttonActiveMijiSkillData.skillSelected[i])
                {
                    if (buttonActiveCoolTimes[i] > 0 && isButtonActiveSkillsCasted[i])
                    {

                        buttonActiveCoolTimes[i] -= Time.deltaTime;

                        if (buttonActiveCoolTimes[i] <= 0)
                        {
                            buttonActiveCoolTimes[i] = 0;

                            //  쿨타임 다 되면 재시전 가능하게..
                            isButtonActiveSkillsCasted[i] = false;
                        }
                    }
                }
            }
        }

        // 버튼 액티브 스킬 처리
        private void TryCastButtonActiveSkill(int index, int skillIndex)
        {
            // 스킬 시전 가능 여부 확인
            // 시전 중이 아닌 경우
            if (!isButtonActiveSkillsCasted[skillIndex])
            {
                Debug.Log($"Casting button active skill: {buttonActiveMijiSkillData.skillName[skillIndex]}"); // 디버깅

                // 쿨타임 초기화 및 스킬 상태 설정
                buttonActiveCoolTimes[skillIndex] = buttonActiveMijiSkillData.Delay[skillIndex];
                buttonActiveAliveTimes[skillIndex] = buttonActiveMijiSkillData.aliveTime[skillIndex];
                buttonActiveCoefficients[skillIndex] = buttonActiveMijiSkillData.levelCoefficient[skillIndex];
                isButtonActiveSkillsCasted[skillIndex] = true;

                // 스킬 시전 후 패널 쿨타임 돌 게 하는 액션
                onButtonActiveSkillCasted?.Invoke(index, buttonActiveMijiSkillData.Delay[skillIndex]);

                CastButtonActiveMijiSkill(skillIndex);
            }
        }

        public void JudgeSelectedAndCastInfiniteActiveSkill()
        {
            for (int i = 0; i < activeSkillIndices.Count; i++)
            {
                int skillIndex = activeSkillIndices[i];

                // activeSkillIndex : 패시브 비급패널에서의 위치 인덱스
                // 현재 사출기 인덱스(skillIndex) = Stage 1 : 0 , 그 외 : 100
                if (skillIndex != -1)
                {
                    switch (skillIndex)
                    {
                        // skillIndex 한자리 수 인 경우 : Stage1
                        case 0:
                            cameraBoundingSkill = poolManager.GetSkill(17) as Shuriken;

                            cameraBoundingSkill.enemy = null;
                            activeDamages[skillIndex] = activeMijiSkillData.levelCoefficient[skillIndex];
                            cameraBoundingSkill.Damage = activeDamages[skillIndex];
                            cameraBoundingSkill.skillIndex = skillIndex;
                            cameraBoundingSkill.onSkillAttack = OnSkillAttack;
                            break;
                        // skillIndex 100부터 출발 : Stage 2, 3인 경우(해당 스킬이 액티브 스킬인지 알기위한 조치)
                        case 100:
                            skillIndex -= 100;
                            cameraBoundingSkill = poolManager.GetSkill(17) as Shuriken;

                            cameraBoundingSkill.enemy = null;
                            activeDamages[skillIndex] = activeMijiSkillData.levelCoefficient[skillIndex];
                            cameraBoundingSkill.Damage = activeDamages[skillIndex];
                            cameraBoundingSkill.skillIndex = skillIndex;
                            cameraBoundingSkill.onSkillAttack = OnSkillAttack;
                            break;
                    }
                }
            }
        }

        private void CastButtonActiveMijiSkill(int skillIndex)
        {
            switch (skillIndex)
            {
                case 0: // 환영분신(幻影分身) 
                    shadow = PoolManager.instance.GetIllusion(1) as Shadow; // 환영분신

                    shadow.aliveTime = buttonActiveAliveTimes[skillIndex];
                    ((Shadow)shadow).onShadowStateChanged = OnShadowStateChanaged;
                    ((Shadow)shadow).Init();

                    ((Shadow)shadow).SetShadowPosition();

                    break;
                case 1: // 질풍순보(疾風瞬步)
                    PerformDash(skillIndex);

                    break;
                case 2: // 암연장막(暗煙障幕) 
                    skill = poolManager.GetSkill(16) as SmokeScreen;

                    ((SmokeScreen)skill).SetSkillInformation(playerTransform.position.x, playerTransform.position.y + 0.5f);
                    
                    skill.AliveTime = buttonActiveAliveTimes[skillIndex];
                    skill.skillIndex = skillIndex;
                    ((SmokeScreen)skill).dodgeRate = buttonActiveCoefficients[skillIndex];
                    ((SmokeScreen)skill).Init();
                    ((SmokeScreen)skill).onDodgeOn = OnDodgeOn;
                    ((SmokeScreen)skill).onDodgeOff = OnDodgeOff;

                    break;
                case 3: // 흑야은신(黑夜隱身)
                    stealthEffect.SetActive(true);
                    stealth.MakePlayerStealth();
                    stealth.aliveTime = buttonActiveAliveTimes[skillIndex];

                    stealth.onStealthStateChanged = OnStealthStateChanged;

                    stealth.AttachPlayer(); // 플레이어 위치로 이펙트 위치
                    break;
            }
        }

        private void PerformDash(int skillIndex)
        {
            // 사거리 및 데미지 계수 가져오기
            float dashDistance = buttonActiveAliveTimes[skillIndex];
            float hitDamage = buttonActiveCoefficients[skillIndex] * PlayerManager.player.playerData.level;

            // 플레이어의 위치 및 입력 방향 가져오기
            Player player = PlayerManager.player;
            Vector2 playerPosition = player.transform.position;
            Vector2 dashDirection;
            if (player.inputVec.magnitude == 0)
            {
                dashDirection = player.transform.right * Mathf.Sign(player.transform.localScale.x);
            }
            else
            {
                dashDirection = player.inputVec.normalized; // 입력된 방향을 정규화
            }

            // CapsuleCast 설정: 감지 범위 설정
            float capsuleWidth = 1.0f; // 원통의 너비 (값을 조정하여 범위를 넓힐 수 있습니다)
            float capsuleHeight = dashDistance; // 원통의 높이 (도약 거리와 동일)
            Vector2 capsulePoint1 = playerPosition; // 캡슐의 시작 지점
            Vector2 capsulePoint2 = playerPosition + dashDirection * dashDistance; // 캡슐의 끝 지점

            RaycastHit2D[] hits = Physics2D.CapsuleCastAll(
                (capsulePoint1 + capsulePoint2) / 2, // 캡슐의 중심 위치
                new Vector2(capsuleWidth, capsuleHeight), // 캡슐의 크기
                CapsuleDirection2D.Horizontal, // 캡슐 방향
                0f, // 회전 각도
                dashDirection, // 캐스트 방향
                dashDistance // 캐스트 거리
            );

            // 적 감지 및 데미지 처리
            foreach (RaycastHit2D hit in hits)
            {
                if (hit.collider != null)
                {
                    // 감지된 Collider가 적 또는 보스인지 확인
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Enemy") || hit.collider.gameObject.layer == LayerMask.NameToLayer("Boss"))
                    {
                        IDamageable damageable = hit.collider.GetComponent<IDamageable>();
                        if (damageable != null)
                        {
                            damageable.TakeDamage(tag, hitDamage); // 데미지 처리
                        }
                    }
                }
            }

            // 레이어 마스크 설정
            int layerMask = LayerMask.GetMask("Boundary"); // Boundary만 감지

            // 이동 경로에 장애물이 있는지 Raycast로 체크
            Rigidbody2D rigidbody2D = player.GetComponent<Rigidbody2D>();
            Vector2 startPosition = rigidbody2D.position;
            float backMargin = 0.1f;
            startPosition += dashDirection * (-backMargin); // 뒤로 좀 뺌
            dashDistance += backMargin;
            RaycastHit2D hit2 = Physics2D.Raycast(startPosition, dashDirection, dashDistance, layerMask);

            if (hit2.collider != null)
            {
                // 장애물이 있다면 그 위치까지 이동
                Vector2 newPosition = hit2.point;
                newPosition += dashDirection * (-backMargin);
                player.transform.position = newPosition;
                Debug.Log(hit2.point);
            }
            else
            {
                // 장애물이 없다면 목표 위치로 이동
                Vector2 dashEndPosition = playerPosition + dashDirection * dashDistance;
                player.transform.position = dashEndPosition;
                Debug.Log(dashEndPosition);
            }
        }


        // 은신 이어서 계속 진행
        private void ResumeStealthSkill(float remainingTime)
        {
            stealthEffect.SetActive(true);
            stealth.MakePlayerStealth();
            stealth.aliveTime = remainingTime; // 남은 시간

            stealth.onStealthStateChanged = OnStealthStateChanged;

            stealth.AttachPlayer(); // 플레이어 위치로 이펙트 위치시키기
        }

        // 환영분신 이어서 계속 진행
        private void ResumeShadowSkill(float remainingTime)
        {
            shadow = PoolManager.instance.GetIllusion(1) as Shadow;

            shadow.aliveTime = remainingTime; // 남은 시간
            ((Shadow)shadow).onShadowStateChanged = OnShadowStateChanaged;
            ((Shadow)shadow).Init();
            ((Shadow)shadow).SetShadowPosition();
        }

        private void OnStealthStateChanged(bool isStealthActive)
        {
            this.isStealthActive = isStealthActive;
        }

        private void OnShadowStateChanaged(bool isShadowActive)
        {
            this.isShadowActive = isShadowActive;

            onShadowStateChanged(isShadowActive);
        }

        void OnSkillAttack(int index, float damage)
        {
            damageMeters[index] += (int)damage;
        }

        public int[] ReturnDamageMeters()
        {
            return damageMeters;
        }

        public void OnDodgeOn(float dodgeRate)
        {
            onDodgeOn?.Invoke(dodgeRate);
        }

        public void OnDodgeOff(float dodgeRate)
        {
            onDodgeOff?.Invoke(dodgeRate);
        }

        // 버튼 액티브 비급스킬 인덱스 추가 함수
        public void AddButtonActiveSkillIndex(int index, int pointerIndex)
        {
            // 중복된 인덱스를 추가하지 않음
            if (!buttonActiveSkillIndices.Contains(index))
            {
                for (int i = 0; i < buttonActiveSkillIndices.Count; i++)
                {
                    if (buttonActiveSkillIndices[i] == -1)
                    {
                        buttonActiveSkillIndices[i] = index; // 스킬 인덱스
                        buttonActiveSkillsPosIndices[i] = pointerIndex; // 패널 위치 인덱스
                        OnMijiSkillSelected();
                        break;
                    }
                }
            }
        }

        // 액티브 비급스킬 인덱스 추가 함수
        public void AddActiveSkillIndex(int index, int pointerIndex)
        {
            // 중복된 인덱스를 추가하지 않음
            if (!activeSkillIndices.Contains(index))
            {
                for (int i = 0; i < activeSkillIndices.Count; i++)
                {
                    if (activeSkillIndices[i] == -1)
                    {
                        activeSkillIndices[i] = index;
                        activeSkillPosIndex = pointerIndex;
                        OnMijiSkillSelected();
                        break;
                    }
                }
            }

            JudgeSelectedAndCastInfiniteActiveSkill();
        }

        // 패시브 비급스킬 선택 처리 함수
        public void OnSelectPassiveMijiSkil(int index)
        {
            switch (index)
            {
                case 0:
                    // DamageHandler에서 data 참조해서 사용함
                    break;
                case 1:
                    onSelectPassiveMijiSkil(0.1f); // 모든 스킬 쿨타임 10% 감소
                    break;
                case 2:
                    // DamageHandler에서 data 참조해서 사용함
                    break;
                case 3:
                    onDodgeOn?.Invoke(passiveMijiSkillData.levelCoefficient[3]);
                    break;
                case 4:
                    // DamageHandler에서 data 참조해서 사용함
                    break;
            }
        }

        public void AssignSkillIndices(List<int> selectedActiveMijiSkills, List<int> selectedPassiveMijiSkills)
        {
            //==================================================================
            // buttonActiveSkillIndices는 직접 할당
            buttonActiveSkillIndices = new List<int>(selectedActiveMijiSkills);


            //==================================================================
            // activeSkillIndices 초기화
            activeSkillIndices.Clear();

            // selectedPassiveMijiSkills에서 '100'이 있는지 확인하고 할당
            foreach (int skillIndex in selectedPassiveMijiSkills)
            {
                if (skillIndex == 100) // 100이 있는 경우만 추가
                {
                    activeSkillIndices.Add(skillIndex);
                    break; // 최대 1개의 스킬만 추가하므로 break
                }
            }

            // 만약 '100'이 없다면, -1을 추가하여 비어 있음을 표시
            if (activeSkillIndices.Count == 0)
            {
                activeSkillIndices.Add(-1);
            }

            // 사출기는 무한히 지속되기에 처음부터 동작
            JudgeSelectedAndCastInfiniteActiveSkill();
        }

        public void OnMijiSkillSelected()
        {
            mijiEffect = playerTransform.GetChild(5).gameObject;

            mijiEffect.SetActive(true);

            if (mijiEffect != null)
            {
                StartCoroutine(DeactivateAfterAnimation());
            }
        }

        private IEnumerator DeactivateAfterAnimation()
        {
            mijiEffectAnimator = mijiEffect.GetComponent<Animator>();

            // Animator가 유효한지 확인
            if (mijiEffectAnimator == null)
            {
                yield break; // Animator가 파괴되었으면 코루틴을 종료
            }

            // Animator가 다시 null인지 확인
            if (mijiEffectAnimator != null)
            {
                // 애니메이션 종료될 때까지 대기
                yield return new WaitUntil(() => mijiEffectAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f);

                mijiEffect.SetActive(false); // 비활성화
            }
        }

        private void SaveSkillState()
        {
            //==================================================================
            // 버튼 액티브 스킬 상태 저장

            for (int i = 0; i < MAX_BUTTONACTIVE_SKILLS; i++)
            {
                if (isButtonActiveSkillsCasted[i]) // 스킬이 시전 중일 경우
                {
                    PlayerPrefs.SetFloat($"ButtonActiveCoolTime_{i}", buttonActiveCoolTimes[i]);
                    PlayerPrefs.SetInt($"ButtonActiveSkillCasted_{i}", 1); // 1은 true 의미
                }
                else
                {
                    PlayerPrefs.SetFloat($"ButtonActiveCoolTime_{i}", 0);
                    PlayerPrefs.SetInt($"ButtonActiveSkillCasted_{i}", 0); // 0은 false 의미
                }

                // 버튼 액티브 시전 시간과 계수 저장
                PlayerPrefs.SetFloat($"buttonActiveAliveTimes_{i}", buttonActiveAliveTimes[i]);
                PlayerPrefs.SetFloat($"buttonActiveCoefficients_{i}", buttonActiveCoefficients[i]);
                
                // 버튼 액티브 스킬 위치 인덱스 저장
                PlayerPrefs.SetInt($"ButtonActiveSkillPosIndex_{i}", buttonActiveSkillsPosIndices[i]);
            }

            //==================================================================
            // 액티브 스킬 상태 저장

            for (int i = 0; i < MAX_ACTIVE_SKILLS; i++)
            {
                if (isActiveSkillCasted[i]) // 스킬이 시전 중일 경우
                {
                    PlayerPrefs.SetFloat($"ActiveCoolTime_{i}", activeCoolTimes[i]);
                    PlayerPrefs.SetInt($"ActiveSkillCasted_{i}", 1); // 1은 true 의미
                }
                else
                {
                    PlayerPrefs.SetFloat($"ActiveCoolTime_{i}", 0);
                    PlayerPrefs.SetInt($"ActiveSkillCasted_{i}", 0); // 0은 false 의미
                }

                // 액티브 스킬 데미지와 횟수 저장
                PlayerPrefs.SetFloat($"ActiveDamage_{i}", activeDamages[i]);
                PlayerPrefs.SetFloat($"ActiveLife_{i}", activeLifes[i]);
            }

            // 액티브 스킬 인덱스 위치 저장
            PlayerPrefs.SetInt("ActiveSkillIndex", activeSkillPosIndex);

            // 은신 상태 및 남은 시간 저장
            PlayerPrefs.SetInt("IsStealthActive", isStealthActive ? 1 : 0); // 1이면 true, 0이면 false
            PlayerPrefs.SetFloat("StealthRemainingTime", stealthRemainingTime);

            // 환영분신 상태 및 남은 시간 저장
            PlayerPrefs.SetInt("IsShadowActive", isShadowActive ? 1 : 0); // 1이면 true, 0이면 false
            PlayerPrefs.SetFloat("ShadowRemainingTime", shadowRemainingTime);
        }
    }
}