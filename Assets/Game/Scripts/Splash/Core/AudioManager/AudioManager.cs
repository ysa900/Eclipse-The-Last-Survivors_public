using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Eclipse.Splash
{
    public class AudioManager : Eclipse.Manager
    {
        public static AudioManager instance;

        [SerializeField] private SettingData _settingData;
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
        [ReadOnly] public List<AudioClip> bgmClips; // BGM 관련 클립(파일) 배열
        public AudioSource bgmPlayer; // BGM 관련 오디오소스

        // SFX(효과음)
        [Header("#SFX")]
        [ReadOnly] public List<AudioClip> sfxClips; // SFX 관련 클립 배열
        public AudioSource sfxPlayer; // SFX 관련 오디오소스

        public enum Sfx { Select }
        public enum Bgm { Splash0, Splash1, Splash2, Splash3 }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            string bgmPath = "Audio/Background Effect/";
            bgmClips.Add(Resources.Load<AudioClip>(bgmPath + "Splash0"));
            bgmClips.Add(Resources.Load<AudioClip>(bgmPath + "Splash1"));
            bgmClips.Add(Resources.Load<AudioClip>(bgmPath + "Splash2"));
            bgmClips.Add(Resources.Load<AudioClip>(bgmPath + "Splash3"));

            string audioPath = "Audio/Sound Effect/";
            sfxClips.Add(Resources.Load<AudioClip>(audioPath + "Select"));
        }

        // Player들 초기화 먼저하기
        private void OnEnable()
        {
            Init();
        }

        private void Start()
        {
            // 세팅 데이터에 저장된 값 불러와서 적용
            SetMasterVolume(SettingData.masterSound);
            SetBGMVolume(SettingData.bgmSound);
            SetSFXVolume(SettingData.sfxSound);
            SetMuteSetting(SettingData.isMute);

            SetFullScreenSetting(SettingData.isFullScreen);
        }

        // 마스터 볼륨 설정 함수
        public void SetMasterVolume(float volume)
        {
            SettingData.masterSound = volume;

            if (SettingData.masterSound == 0.001f)
                M_AudioMixer.SetFloat("Master", -80);
            else
                M_AudioMixer.SetFloat("Master", Mathf.Log10(volume) * 20);
        }

        // BGM 볼륨 설정 함수
        public void SetBGMVolume(float volume)
        {
            SettingData.bgmSound = volume;

            if (SettingData.bgmSound == 0.001f)
                M_AudioMixer.SetFloat("BGM", -80);
            else
                M_AudioMixer.SetFloat("BGM", Mathf.Log10(volume) * 20);
        }

        // SFX 볼륨 설정 함수
        public void SetSFXVolume(float volume)
        {
            SettingData.sfxSound = volume;

            if (SettingData.sfxSound == 0.001f)
                M_AudioMixer.SetFloat("SFX", -80);
            else
                M_AudioMixer.SetFloat("SFX", Mathf.Log10(volume) * 20);
        }

        // 음소거 설정 함수
        public void SetMuteSetting(bool isOn)
        {
            AudioListener.volume = isOn ? 0 : 1;
        }

        public void SetFullScreenSetting(bool isOn)
        {
            if (isOn)
            {
                UnityEngine.Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                UnityEngine.Screen.SetResolution(1920, 1080, true);
            }
            else
            {
                UnityEngine.Screen.fullScreenMode = FullScreenMode.Windowed;
                UnityEngine.Screen.SetResolution(1280, 720, false);
            }

            UnityEngine.Screen.fullScreen = isOn;
        }

        private void InitializeAudioPlayers()
        {
            string outputMixer = "Master";

            // BGM ����� �÷��̾� �ʱ�ȭ
            GameObject bgmObject = new GameObject("BgmPlayer");
            bgmObject.transform.parent = transform;
            bgmPlayer = bgmObject.AddComponent<AudioSource>();
            bgmPlayer.outputAudioMixerGroup = M_AudioMixer.FindMatchingGroups(outputMixer)[1];

            // SFX ����� �÷��̾� �ʱ�ȭ
            GameObject sfxObject = new GameObject("SfxPlayer");
            sfxObject.transform.parent = transform;
            sfxPlayer = sfxObject.AddComponent<AudioSource>();
            sfxPlayer.outputAudioMixerGroup = M_AudioMixer.FindMatchingGroups(outputMixer)[2];
        }

        // ����� �÷���
        public void playBgm(int clipIndex)
        {
            bgmPlayer.clip = bgmClips[clipIndex];
            bgmPlayer.Play();
            bgmPlayer.playOnAwake = false;
            bgmPlayer.loop = true; // ���� �ݺ�
        }

        // ȿ���� �÷���
        public void playSfx(int clipIndex)
        {
            sfxPlayer.clip = sfxClips[clipIndex];
            sfxPlayer.Play();
            sfxPlayer.playOnAwake = false;
            sfxPlayer.loop = false;
        }

        private void Init()
        {
            InitializeAudioPlayers();
        }
    }
}