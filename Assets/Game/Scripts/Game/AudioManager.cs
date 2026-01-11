using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace Eclipse.Game
{
    public class AudioManager : Eclipse.Manager
    {
        public static AudioManager instance; // 싱글톤 패턴을 위한 정적 instance 변수 선언

        [SerializeField] private SettingData _settingData;
        // SettingData를 리소스 폴더에서 로드하는 속성. 사운드 설정을 데이터로 관리
        public SettingData SettingData
        {
            get
            {
                if (_settingData == null)
                {
                    _settingData = Resources.Load<SettingData>("Datas/SettingData");
                }
                return _settingData;
            }
        }

        [SerializeField] private AudioMixer m_AudioMixer;
        // AudioMixer를 리소스에서 로드하는 속성
        public AudioMixer M_AudioMixer
        {
            get
            {
                if (m_AudioMixer == null)
                {
                    m_AudioMixer = Resources.Load<AudioMixer>("Audio/MasterAudioMixer");
                }
                return m_AudioMixer;
            }
        }

        // BGM
        [Header("#BGM")]
        [ReadOnly] public List<AudioClip> bgmClips; // BGM 관련 클립 리스트
        [ReadOnly] public AudioSource bgmPlayer;    // BGM을 재생하는 AudioSource

        //private Coroutine fadeCoroutine; // 페이드 효과 관리

        // SFX(효과음)
        [Header("#SFX")]
        [ReadOnly] public List<AudioClip> sfxClips; // SFX 관련 클립 리스트
        private const int lowPriorityChannelLimit = 3; // 0,1,2번 채널만 사용
        private const int totalSfxChannels = 12; // // 기본 채널 수를 설정 (동시 재생 개수)
        private float sfxCheckCooldown = 0.05f;
        private float lastSfxCheckTime = -1f;
        private int channelIndex;    // 현재 재생 중인 채널의 인덱스
        private AudioSource[] sfxPlayers; // SFX 재생용 AudioSource 배열

        // 최근 재생된 SFX를 추적하기 위한 큐
        private Queue<Sfx> recentSFXQueue = new Queue<Sfx>();
        private const int RecentSFXLimit = 6; // 기록할 SFX 개수

        // 그룹과 우선순위 설정
        public class FixedSizeQueue<T> : Queue<T>
        {
            public int MaxSize { get; }

            public FixedSizeQueue(int maxSize)
            {
                MaxSize = maxSize;
            }

            public new void Enqueue(T item)
            {
                while (Count >= MaxSize)
                {
                    Dequeue(); // 오래된 항목 제거
                }
                base.Enqueue(item);
            }
        }

        private enum SfxGroup { UI, Object, Combat, Skill, Ultimate }
        private const int MaxSfxQueueSize = 20;
        private Dictionary<SfxGroup, FixedSizeQueue<Sfx>> sfxQueues = new();
        private HashSet<SfxGroup> processingGroups = new();
        private HashSet<Sfx> lowPrioritySfx = new HashSet<Sfx> { Sfx.Dead, Sfx.Melee0, Sfx.Melee1, Sfx.Hit, Sfx.Range };

        // UI 관련 변수들
        CustomSlider m_MusicMasterSlider;
        CustomSlider m_MusicBGMSlider;
        CustomSlider m_MusicSFXSlider;

        SliderText masterSoundLabel;
        SliderText bgmSoundLabel;
        SliderText sfxSoundLabel;

        SliderImage masterFillImage;
        SliderImage sfxFillImage;
        SliderImage bgmFillImage;

        MuteToggle soundMuteToggle; // 음소거 토글
        FullScreenToggle fullScreenToggle; // 전체 화면 토글

        // BGM, SFX Enum 정의. 각 Clip의 인덱스를 명시적으로 관리 가능
        public enum Bgm { Stage1, Stage1_Clear, Stage2, Stage2_Clear, Stage3, Boss, Boss_1Phase, Boss_2Phase, Boss_3Phase, Game_Clear, FighterGoblin, RuinedKing }
        public enum Sfx
        {
            // 공용, Melee1, Hit는 호출하지 말 것(Melee0의 랜덤 클립으로 사용됨)
            Dead, Melee0, Melee1, Hit, LevelUp, Range, Select, Exp, HP_Potion, Pickup, SFX_Check, Collapse,
            /* 마법사 클래스 */
            // 전기 속성
            Lightning,
            ElectricBall,
            EnergyBlast,
            Judgement,
            // 불 속성
            FireBall,
            Explosion,
            Inferno,
            Meteor,
            // 물 속성
            WaterShot,
            IceSpike,
            IceBlast,
            // 각성
            BeamLaser,
            FrozenSpike,
            HeavenEclipse,
            HydroFlame,
            ShieldFlame,
            SkyFall,

            /* 전사 클래스 */
            // 광기 계열
            BlowOfMadness,
            StormOfMadness,
            ThirstForBlood,
            IncarnationOfRange,
            // 암흑 계열
            StrikeOfDarkness,
            DarkSlash,
            TouchOfDeath,
            MasterOfDarness,
            // 신성 계열
            HolyHammer,
            HolyShield,
            CleaningStrike,
            HolySword,
            // 각성
            BloodUltimated,
            DarkUltimated,
            HolyUltimated,

            /* 어쌔신 클래스 */
            // 암기
            Dagger,
            SlashBlue,
            PhantomShuriken,
            SpellSword,
            // 인술
            EntropicDecay,
            BlueLotus,
            Desiccation,
            SpiritOfTheWild,
            // 트랩
            PoisonPotion,
            Snare,
            DeathStrikeSeal,
            ArcaneHeart,
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            // BGM 및 SFX 리소스 로드
            string bgmPath = "Audio/Background Effect/";
            bgmClips.Add(Resources.Load<AudioClip>(bgmPath + "Stage1"));
            bgmClips.Add(Resources.Load<AudioClip>(bgmPath + "Stage1 Clear"));
            bgmClips.Add(Resources.Load<AudioClip>(bgmPath + "Stage2"));
            bgmClips.Add(Resources.Load<AudioClip>(bgmPath + "Stage2 Clear"));
            bgmClips.Add(Resources.Load<AudioClip>(bgmPath + "Stage3"));
            bgmClips.Add(Resources.Load<AudioClip>(bgmPath + "Boss Audio Source/Belial/Belial"));
            bgmClips.Add(Resources.Load<AudioClip>(bgmPath + "Boss 1Phase"));
            bgmClips.Add(Resources.Load<AudioClip>(bgmPath + "Boss 2Phase"));
            bgmClips.Add(Resources.Load<AudioClip>(bgmPath + "Boss 3Phase"));
            bgmClips.Add(Resources.Load<AudioClip>(bgmPath + "Ending Credit_Edit"));
            bgmClips.Add(Resources.Load<AudioClip>(bgmPath + "Boss Audio Source/FighterGoblin/FighterGoblin"));
            bgmClips.Add(Resources.Load<AudioClip>(bgmPath + "Boss Audio Source/RuinedKing/RuinedKing"));

            string audioPath = "Audio/Sound Effect/";
            sfxClips.Add(Resources.Load<AudioClip>(audioPath + "Dead_Denoise"));
            sfxClips.Add(Resources.Load<AudioClip>(audioPath + "Melee0"));
            sfxClips.Add(Resources.Load<AudioClip>(audioPath + "Melee1"));
            sfxClips.Add(Resources.Load<AudioClip>(audioPath + "Hit"));
            sfxClips.Add(Resources.Load<AudioClip>(audioPath + "LevelUp"));
            sfxClips.Add(Resources.Load<AudioClip>(audioPath + "Range"));
            sfxClips.Add(Resources.Load<AudioClip>(audioPath + "Select_Denoise"));
            sfxClips.Add(Resources.Load<AudioClip>(audioPath + "Exp"));
            sfxClips.Add(Resources.Load<AudioClip>(audioPath + "HP_Potion_Drink"));
            sfxClips.Add(Resources.Load<AudioClip>(audioPath + "Pickup"));
            sfxClips.Add(Resources.Load<AudioClip>(audioPath + "SFX_Check"));
            sfxClips.Add(Resources.Load<AudioClip>(audioPath + "Collapse"));

            /* 스킬 사운드 모음 */

            // 마법사 클래스
            string mageSkillSoundPath = "Audio/Skill Sound/MageSkillSound/";

            // 전기 속성
            sfxClips.Add(Resources.Load<AudioClip>(mageSkillSoundPath + "Electric Skill/Lightning(Electric_Common)"));
            sfxClips.Add(Resources.Load<AudioClip>(mageSkillSoundPath + "Electric Skill/ElectricBall(Electric_Rare)"));
            sfxClips.Add(Resources.Load<AudioClip>(mageSkillSoundPath + "Electric Skill/EnergyBlast(Electric_Epic)"));
            sfxClips.Add(Resources.Load<AudioClip>(mageSkillSoundPath + "Electric Skill/Judgement(Eletric_Legendary)"));

            // 불 속성
            sfxClips.Add(Resources.Load<AudioClip>(mageSkillSoundPath + "Fire Skill/FilreBall(Fire_Common)_HughSound"));
            sfxClips.Add(Resources.Load<AudioClip>(mageSkillSoundPath + "Fire Skill/Explosion(Fire_Rare)_HugeSound"));
            sfxClips.Add(Resources.Load<AudioClip>(mageSkillSoundPath + "Fire Skill/Inferno(Fire_Epic)"));
            sfxClips.Add(Resources.Load<AudioClip>(mageSkillSoundPath + "Fire Skill/Meteor(Fire_Legendary)"));

            // 물 속성
            sfxClips.Add(Resources.Load<AudioClip>(mageSkillSoundPath + "Water Skill/WaterShot(Water_Common)"));
            sfxClips.Add(Resources.Load<AudioClip>(mageSkillSoundPath + "Water Skill/IceSpike(Water_Rare)"));
            sfxClips.Add(Resources.Load<AudioClip>(mageSkillSoundPath + "Water Skill/IceBlast(Water_Legendary)"));

            // 공명
            sfxClips.Add(Resources.Load<AudioClip>(mageSkillSoundPath + "Resonance Skill/BeamLaser"));
            sfxClips.Add(Resources.Load<AudioClip>(mageSkillSoundPath + "Resonance Skill/Frozen_Spike"));
            sfxClips.Add(Resources.Load<AudioClip>(mageSkillSoundPath + "Resonance Skill/HeavenEclipse"));
            sfxClips.Add(Resources.Load<AudioClip>(mageSkillSoundPath + "Resonance Skill/HydroFlame"));
            sfxClips.Add(Resources.Load<AudioClip>(mageSkillSoundPath + "Resonance Skill/ShieldFlame"));
            sfxClips.Add(Resources.Load<AudioClip>(mageSkillSoundPath + "Resonance Skill/SkyFall"));

            // 전사 클래스
            string warriorSkillSoundPath = "Audio/Skill Sound/WarriorSkillSound/";

            // 광기 계열
            sfxClips.Add(Resources.Load<AudioClip>(warriorSkillSoundPath + "Blood Skill/BlowOfMadness(Blood_Common)"));
            sfxClips.Add(Resources.Load<AudioClip>(warriorSkillSoundPath + "Blood Skill/StormOfMadness(Blood_Rare)"));
            sfxClips.Add(Resources.Load<AudioClip>(warriorSkillSoundPath + "Blood Skill/ThirstForBlood(Blood_Epic)"));
            sfxClips.Add(Resources.Load<AudioClip>(warriorSkillSoundPath + "Blood Skill/IncarnationOfRange(Blood_Legendary)"));

            // 암흑 계열
            sfxClips.Add(Resources.Load<AudioClip>(warriorSkillSoundPath + "Dark Skill/StrikeOfDarkness(Dark_Common)"));
            sfxClips.Add(Resources.Load<AudioClip>(warriorSkillSoundPath + "Dark Skill/DarkSlash(Dark_Rare)"));
            sfxClips.Add(Resources.Load<AudioClip>(warriorSkillSoundPath + "Dark Skill/TouchOfDeath(Dark_Epic)"));
            sfxClips.Add(Resources.Load<AudioClip>(warriorSkillSoundPath + "Dark Skill/Master_Of_Darkness(Dark_Legendary)"));

            // 신성 계열
            sfxClips.Add(Resources.Load<AudioClip>(warriorSkillSoundPath + "Holy Skill/Holy_Hammer(Holy_common)"));
            sfxClips.Add(Resources.Load<AudioClip>(warriorSkillSoundPath + "Holy Skill/HolyShield(Holy_rare)"));
            sfxClips.Add(Resources.Load<AudioClip>(warriorSkillSoundPath + "Holy Skill/PurifyingStrike(Holy_Epic)"));
            sfxClips.Add(Resources.Load<AudioClip>(warriorSkillSoundPath + "Holy Skill/HolySword(Holy_Legendary)"));

            // 각성
            sfxClips.Add(Resources.Load<AudioClip>(warriorSkillSoundPath + "Ultimated Skill/Blood_Ultimated"));
            sfxClips.Add(Resources.Load<AudioClip>(warriorSkillSoundPath + "Ultimated Skill/Dark_Ultimated"));
            sfxClips.Add(Resources.Load<AudioClip>(warriorSkillSoundPath + "Ultimated Skill/Holy_Ultimated"));

            // 어쌔신 클래스
            string assassinSkillSoundPath = "Audio/Skill Sound/AssassinSkillSound/";

            // 암기류
            sfxClips.Add(Resources.Load<AudioClip>(assassinSkillSoundPath + "Amgi Skill/Dagger(Amgi_Common)"));
            sfxClips.Add(Resources.Load<AudioClip>(assassinSkillSoundPath + "Amgi Skill/FireSlah(Amgi_Rare)"));
            sfxClips.Add(Resources.Load<AudioClip>(assassinSkillSoundPath + "Amgi Skill/KillingEdge(Amgi_Epic)"));
            sfxClips.Add(Resources.Load<AudioClip>(assassinSkillSoundPath + "Amgi Skill/SpellSword(Amgi_Legendary)"));

            // 인술류
            sfxClips.Add(Resources.Load<AudioClip>(assassinSkillSoundPath + "Ninshu Skill/FlameHell(Common)"));
            sfxClips.Add(Resources.Load<AudioClip>(assassinSkillSoundPath + "Ninshu Skill/FlameFlower(Rare)"));
            sfxClips.Add(Resources.Load<AudioClip>(assassinSkillSoundPath + "Ninshu Skill/BloodCharm(Epic)"));
            sfxClips.Add(Resources.Load<AudioClip>(assassinSkillSoundPath + "Ninshu Skill/Raikiri(Legendary)"));

            // 트랩류
            sfxClips.Add(Resources.Load<AudioClip>(assassinSkillSoundPath + "Trap Skill/PoisonTrap(Common)"));
            sfxClips.Add(Resources.Load<AudioClip>(assassinSkillSoundPath + "Trap Skill/VineTrap(Rare)"));
            sfxClips.Add(Resources.Load<AudioClip>(assassinSkillSoundPath + "Trap Skill/BlindTrap(Epic)"));
            sfxClips.Add(Resources.Load<AudioClip>(assassinSkillSoundPath + "Trap Skill/DevilParade(Legendary)"));


            string outputMixer = "Master";

            // BGM 플레이어 초기화
            GameObject bgmObject = new GameObject("BgmPlayer");
            bgmObject.transform.parent = transform;
            bgmPlayer = bgmObject.AddComponent<AudioSource>();
            bgmPlayer.outputAudioMixerGroup = M_AudioMixer.FindMatchingGroups(outputMixer)[1];
            bgmPlayer.playOnAwake = false;
            bgmPlayer.loop = true;

            // SFX 플레이어 초기화
            GameObject sfxObject = new GameObject("SfxPlayer");
            sfxObject.transform.parent = transform;

            // AudioSource 배열을 채널 수에 맞게 생성
            sfxPlayers = new AudioSource[totalSfxChannels];

            // 각 채널에 대한 오브젝트와 AudioSource를 개별적으로 추가
            for (int index = 0; index < totalSfxChannels; index++)
            {
                GameObject sfxChannel = new GameObject("SFX_Channel_" + index);
                sfxChannel.transform.parent = sfxObject.transform; // sfxObject의 자식으로 설정

                AudioSource sfxSource = sfxChannel.AddComponent<AudioSource>(); // AudioSource 추가
                sfxSource.playOnAwake = false;  // 즉시 재생 비활성화
                sfxSource.outputAudioMixerGroup = M_AudioMixer.FindMatchingGroups(outputMixer)[2]; // 적절한 믹서 그룹 설정

                // AudioSource 배열에 추가
                sfxPlayers[index] = sfxSource; // 배열에 직접 할당 (Add 필요 없음)
            }
        }

        // Start에서 GUI와 슬라이더, 라벨 등을 초기화
        private void Start()
        {
            GUIManager gui = client.GetManager<GUIManager>();

            m_MusicMasterSlider = gui.masterSlider;
            m_MusicBGMSlider = gui.bgmSlider;
            m_MusicSFXSlider = gui.sfxSlider;
            soundMuteToggle = gui.muteToggle;

            fullScreenToggle = gui.fullScreenToggle;

            masterSoundLabel = gui.masterSoundLabel;
            bgmSoundLabel = gui.bgmSoundLabel;
            sfxSoundLabel = gui.sfxSoundLabel;

            masterFillImage = gui.masterFillImage;
            bgmFillImage =  gui.bgmFillImage;
            sfxFillImage = gui.sfxFillImage;

            // 사운드 데이터에 저장된 값 불러와서 적용
            SetMasterVolume(SettingData.masterSound);
            SetBGMVolume(SettingData.bgmSound);
            SetSFXVolume(SettingData.sfxSound);
            SetMuteSetting(SettingData.isMute);
            FullScreenSetting(SettingData.isFullScreen);

            // 슬라이더에 이벤트 리스너 추가
            m_MusicMasterSlider.onValueChanged = (value) =>
            {
                PlaySfxWithCooldown(Sfx.SFX_Check);

                SettingData.masterSound = value;
                value = SettingData.masterSound;

                if (SettingData.masterSound == 0.001f)
                    M_AudioMixer.SetFloat("Master", -80);
                else
                    M_AudioMixer.SetFloat("Master", Mathf.Log10(value) * 20);

                masterFillImage.SetFillAmount(value); // FillAmount 변경
                masterSoundLabel.SetLabelText(value); // Label 변경
            };

            m_MusicBGMSlider.onValueChanged = (value) =>
            {
                PlaySfxWithCooldown(Sfx.SFX_Check);

                SettingData.bgmSound = value;
                value = SettingData.bgmSound;

                if (SettingData.bgmSound == 0.001f)
                    M_AudioMixer.SetFloat("BGM", -80);
                else
                    M_AudioMixer.SetFloat("BGM", Mathf.Log10(value) * 20);

                bgmFillImage.SetFillAmount(value); // FillAmount 변경
                bgmSoundLabel.SetLabelText(value); // Label 변경
            };

            m_MusicSFXSlider.onValueChanged = (value) =>
            {
                PlaySfxWithCooldown(Sfx.SFX_Check);

                SettingData.sfxSound = value;
                value = SettingData.sfxSound;

                if (SettingData.sfxSound == 0.001f)
                    M_AudioMixer.SetFloat("SFX", -80);
                else
                    M_AudioMixer.SetFloat("SFX", Mathf.Log10(value) * 20);

                sfxFillImage.SetFillAmount(value); // FillAmount 변경
                sfxSoundLabel.SetLabelText(value); // Label 변경
            };

            // 음소거 토글 값 변경 시 이벤트 처리
            soundMuteToggle.onToggleChanged = (isOn) =>
            {
                SettingData.isMute = isOn;
                AudioListener.volume = isOn ? 0 : 1;

                soundMuteToggle.SetToggleValue(isOn);
            };

            // 전체 화면 토글 값 변경 시 이벤트 처리
            fullScreenToggle.onToggleChanged = (isOn) =>
            {
                SettingData.isFullScreen = isOn;
                fullScreenToggle.SetFullScreen(isOn);
            };
        }

        // 배경음 플레이
        public void PlayBgm(Bgm bgm)
        {
            bgmPlayer.clip = bgmClips[(int)bgm];
            bgmPlayer.Play();
            bgmPlayer.playOnAwake = false;
            bgmPlayer.loop = true; // 무한 반복
        }

        public void LoadBgm(Bgm bgm)
        {
            bgmPlayer.clip = bgmClips[(int)bgm];
            bgmPlayer.clip.LoadAudioData();
        }

        public void PlaySfxWithCooldown(Sfx sfx)
        {
            // 슬라이더 SFX만 쿨타임 적용
            if (sfx == Sfx.SFX_Check)
            {
                float now = Time.realtimeSinceStartup;
                if (now - lastSfxCheckTime < sfxCheckCooldown)
                {
                    return;
                }

                lastSfxCheckTime = now;
            }

            PlaySfx(sfx); // 원래 함수 호출
        }

        // 효과음 플레이
        public void PlaySfx(Sfx sfx)
        {
            SfxGroup group = GetGroupForSfx(sfx);

            if (!sfxQueues.ContainsKey(group))
                sfxQueues[group] = new FixedSizeQueue<Sfx>(MaxSfxQueueSize);

            sfxQueues[group].Enqueue(sfx);

            if (!processingGroups.Contains(group))
            {
                processingGroups.Add(group);
                StartCoroutine(ProcessSfxQueue(group));
            }
        }

        // SFX 그룹 큐를 처리하는 코루틴
        // 그룹 큐에 있는 SFX를 하나씩 꺼내 적절한 채널에 재생 시도
        private IEnumerator ProcessSfxQueue(SfxGroup group)
        {
            while (sfxQueues[group].Count > 0)
            {
                var sfx = sfxQueues[group].Dequeue();

                // 낮은 우선순위 SFX는 전용 채널(0~2)에서만 재생
                if (lowPrioritySfx.Contains(sfx))
                {
                    for (int i = 0; i < lowPriorityChannelLimit; i++)
                    {
                        if (!sfxPlayers[i].isPlaying)
                        {
                            PlayClipOnChannel(i, sfx);
                            break;
                        }
                    }

                    // 낮은 우선순위 SFX는 프레임 단위로 처리
                    yield return null;
                }
                else
                {
                    bool played = false;
                    // 일반/중요 SFX는 나머지 채널(3~11)에서 재생
                    for (int i = lowPriorityChannelLimit; i < totalSfxChannels; i++)
                    {
                        int loopIndex = (i + channelIndex) % (totalSfxChannels - lowPriorityChannelLimit) + lowPriorityChannelLimit;

                        if (!sfxPlayers[loopIndex].isPlaying)
                        {
                            PlayClipOnChannel(loopIndex, sfx);
                            played = true;
                            break;
                        }
                    }

                    if (!played)
                    {
                        // 모든 채널이 재생 중인 경우, 중복 재생이 적은 SFX를 우선 처리
                        int leastUsedChannel = GetLeastUsedChannel(sfx);
                        PlayClipOnChannel(leastUsedChannel, sfx);
                    }

                    // Skill 그룹만 딜레이 처리
                    if (group == SfxGroup.Skill) yield return new WaitForSeconds(0.1f);
                    else yield return null;
                }
            }

            // 큐가 비면 처리 중 상태 해제
            processingGroups.Remove(group);
        }

        // SFX를 지정된 채널에서 재생
        private void PlayClipOnChannel(int index, Sfx sfx)
        {
            int randIndex = 0;

            // Hit, Melee는 랜덤 클립 선택
            if (sfx == Sfx.Melee0)
            {
                randIndex = Random.Range(0, 3);
            }

            // 클립 설정 및 재생
            sfxPlayers[index].PlayOneShot(sfxClips[(int)sfx + randIndex]);

            // 최근 재생 기록에 추가
            AddToRecentSFXQueue(sfx);

            // 채널 인덱스 갱신 (순환 재생용)
            channelIndex = index;
        }

        // 각 SFX를 속성에 따라 그룹으로 분류
        private SfxGroup GetGroupForSfx(Sfx sfx)
        {
            switch (sfx)
            {
                case Sfx.Select:
                case Sfx.LevelUp:
                case Sfx.SFX_Check:
                    return SfxGroup.UI;

                case Sfx.Pickup:
                case Sfx.Exp:
                case Sfx.HP_Potion:
                    return SfxGroup.Object;

                case Sfx.Dead:
                case Sfx.Melee0:
                case Sfx.Melee1:
                case Sfx.Hit:
                case Sfx.Range:
                case Sfx.Collapse:
                    return SfxGroup.Combat;

                case Sfx.BloodUltimated:
                case Sfx.DarkUltimated:
                case Sfx.HolyUltimated:
                    return SfxGroup.Ultimate;

                default:
                    return SfxGroup.Skill;
            }
        }

        // 최근 재생된 SFX 기록 큐에 추가
        // 중복 판단 또는 통계용으로 활용 가능
        private void AddToRecentSFXQueue(Sfx sfx)
        {
            recentSFXQueue.Enqueue(sfx);

            if (recentSFXQueue.Count > RecentSFXLimit)
            {
                recentSFXQueue.Dequeue(); // 초과 시 오래된 항목 제거
            }
        }

        private int GetLeastUsedChannel(Sfx sfx)
        {
            int leastUsedChannel = 0;
            int leastOccurrences = int.MaxValue;

            for (int index = 0; index < sfxPlayers.Length; index++)
            {
                // 현재 채널에서 재생 중인 SFX의 중복 횟수를 계산
                int occurrences = CountRecentSFXOccurrences(sfxPlayers[index].clip);
                if (occurrences < leastOccurrences)
                {
                    leastOccurrences = occurrences;
                    leastUsedChannel = index;
                }
            }

            return leastUsedChannel;
        }

        private int CountRecentSFXOccurrences(AudioClip clip)
        {
            if (clip == null) return 0;

            // 최근 재생된 SFX 중 clip에 해당하는 항목의 개수를 계산
            int count = 0;
            foreach (var recentSfx in recentSFXQueue)
            {
                if (sfxClips[(int)recentSfx] == clip)
                {
                    count++;
                }
            }

            return count;
        }

        // 마스터 볼륨 설정 함수
        public void SetMasterVolume(float volume)
        {
            SettingData.masterSound = volume;
            m_MusicMasterSlider.SetValue(SettingData.masterSound);

            if (SettingData.masterSound == 0.001f)
                M_AudioMixer.SetFloat("Master", -80);
            else
                M_AudioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);

            masterFillImage.SetFillAmount(SettingData.masterSound);
            masterSoundLabel.SetLabelText(volume);
        }

        // BGM 볼륨 설정 함수
        public void SetBGMVolume(float volume)
        {
            SettingData.bgmSound = volume;
            m_MusicBGMSlider.SetValue(SettingData.bgmSound);

            if (SettingData.bgmSound == 0.001f)
                M_AudioMixer.SetFloat("BGM", -80);
            else
                M_AudioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
            bgmFillImage.SetFillAmount(SettingData.bgmSound);
            bgmSoundLabel.SetLabelText(volume);
        }

        // SFX 볼륨 설정 함수
        public void SetSFXVolume(float volume)
        {
            SettingData.sfxSound = volume;
            m_MusicSFXSlider.SetValue(SettingData.sfxSound);

            if (SettingData.sfxSound == 0.001f)
                M_AudioMixer.SetFloat("SFX", -80);
            else
                M_AudioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
            sfxFillImage.SetFillAmount(SettingData.sfxSound);
            sfxSoundLabel.SetLabelText(volume);
        }

        // 음소거 설정 함수
        public void SetMuteSetting(bool isOn)
        {
            soundMuteToggle.SetToggleValue(isOn);
        }

        // 전체 화면 설정 함수
        public void FullScreenSetting(bool isOn)
        {
            fullScreenToggle.SetFullScreen(isOn);
        }

        // BGM 교체 함수
        public void SwitchBGM(int clipIndex)
        {
            bgmPlayer.Stop(); // 현재 재생 중인 BGM 정지

            bgmPlayer.clip = bgmClips[clipIndex]; // 새로운 BGM 클립 설정
            bgmPlayer.Play(); // 새로운 BGM 재생
        }
    }
}