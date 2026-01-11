using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

namespace Eclipse.Lobby
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
        private float sfxCheckCooldown = 0.05f;
        private float lastSfxCheckTime = -1f;

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
        FullScreenToggle fullScreenToggle; // 전체화면 토글

        public enum Sfx { Select, SFX_Check }
        public enum Bgm { MainPage, CharacterSelectPage }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }

            string bgmPath = "Audio/Background Effect/";
            bgmClips.Add(Resources.Load<AudioClip>(bgmPath + "Lobby"));
            bgmClips.Add(Resources.Load<AudioClip>(bgmPath + "Character Select"));

            string audioPath = "Audio/Sound Effect/";
            sfxClips.Add(Resources.Load<AudioClip>(audioPath + "Select_Denoise"));
            sfxClips.Add(Resources.Load<AudioClip>(audioPath + "SFX_Check"));
        }

        private void OnEnable()
        {
            Init();
        }

        private void Start()
        {
            // 오디오 클립을 미리 로드해서 렉 방지
            LoadBgm(AudioManager.Bgm.CharacterSelectPage);

            // 사운드 데이터에 저장된 값 불러와서 적용
            SetMasterVolume(SettingData.masterSound);
            SetBGMVolume(SettingData.bgmSound);
            SetSFXVolume(SettingData.sfxSound);
            SetMuteSetting(SettingData.isMute);
            SetFullScreenSetting(SettingData.isFullScreen);
        }

        private void InitializeAudioPlayers()
        {
            string outputMixer = "Master";

            // BGM 오디오 플레이어 초기화
            GameObject bgmObject = new GameObject("BgmPlayer");
            bgmObject.transform.parent = transform;
            bgmPlayer = bgmObject.AddComponent<AudioSource>();
            bgmPlayer.outputAudioMixerGroup = M_AudioMixer.FindMatchingGroups(outputMixer)[1];

            // SFX 오디오 플레이어 초기화
            GameObject sfxObject = new GameObject("SfxPlayer");
            sfxObject.transform.parent = transform;
            sfxPlayer = sfxObject.AddComponent<AudioSource>();
            sfxPlayer.outputAudioMixerGroup = M_AudioMixer.FindMatchingGroups(outputMixer)[2];

            GUIManager gui = client.GetManager<GUIManager>();
            soundMuteToggle = gui.muteToggle;
            fullScreenToggle = gui.fullScreenToggle;
            m_MusicMasterSlider = gui.masterSlider;
            m_MusicBGMSlider = gui.bgmSlider;
            m_MusicSFXSlider = gui.sfxSlider;

            masterSoundLabel = gui.masterSoundLabel;
            bgmSoundLabel = gui.bgmSoundLabel;
            sfxSoundLabel = gui.sfxSoundLabel;

            masterFillImage = gui.masterFillImage;
            bgmFillImage = gui.bgmFillImage;
            sfxFillImage = gui.sfxFillImage;
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

        public void SetFullScreenSetting(bool isOn)
        {
            fullScreenToggle.SetFullScreen(isOn);
        }

        // 배경음 플레이
        public void playBgm(int clipIndex)
        {
            bgmPlayer.clip = bgmClips[clipIndex];
            bgmPlayer.Play();
            bgmPlayer.playOnAwake = false;
            bgmPlayer.loop = true; // 무한 반복
        }

        public void LoadBgm(Bgm bgm)
        {
            bgmPlayer.clip = bgmClips[(int)bgm];
            bgmPlayer.clip.LoadAudioData();
        }

        public void PlaySfxWithCooldown(int clipIndex)
        {
            // 슬라이더 SFX만 쿨타임 적용
            if (clipIndex == (int)Sfx.SFX_Check)
            {
                float now = Time.realtimeSinceStartup;
                if (now - lastSfxCheckTime < sfxCheckCooldown)
                {
                    return;
                }

                lastSfxCheckTime = now;
            }

            PlaySfx(clipIndex); // 원래 함수 호출
        }

        // 효과음 플레이
        public void PlaySfx(int clipIndex)
        {
            sfxPlayer.PlayOneShot(sfxClips[clipIndex]);
            sfxPlayer.playOnAwake = false;
            sfxPlayer.loop = false;
        }

        // BGM 변경
        public void SwitchBGM(int clipIndex)
        {
            bgmPlayer.Stop();
            bgmPlayer.clip = bgmClips[clipIndex];
            bgmPlayer.Play();
        }

        public bool IsBgmPlaying(int clipIndex)
        {
            // bgmPlayer가 존재하고, 현재 재생 중인 BGM이 clipIndex와 동일한지 확인
            if (bgmPlayer != null && bgmPlayer.clip == bgmClips[clipIndex])
            {
                return bgmPlayer.isPlaying;
            }

            return false;  // BGM이 재생 중이 아니거나 클립이 일치하지 않으면 false 반환
        }


        private void Init()
        {
            InitializeAudioPlayers();
        }
    }
}