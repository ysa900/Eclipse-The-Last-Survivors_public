using UnityEngine;

namespace Eclipse.Lobby
{
    public partial class InputManager
    {
        private void BindSettingPageInputEvents()
        {
            var gui = client.GetManager<GUIManager>();

            //==================================================================

            void OnEnter()
            {
                Debug.Log("Enter Setting Page");
                SettingData settingData = Eclipse.Lobby.AudioManager.instance.SettingData;

                var settingPageViewer = gui.settingPageViewer;
                var characterSelectPageViewer = gui.characterSelectPageViewer;

                var characterSelectPageBackButton = gui.characterSelectPageBackButton;
                var mageSelectButton = gui.mageSelectButton;
                var warriorSelectButton = gui.warriorSelectButton;
                var assassinSelectButton = gui.assassinSelectButton;
                var gameStartButton = gui.gameStartButton;

                // SettingPage Back 버튼
                var settingPageBackButton = gui.settingPageBackButton;
                settingPageBackButton.onClick = () =>
                {
                    stateMachine.Pop(); // 최상위 스택(Setting Page) Pop
                };
                settingPageBackButton.Show();

                // 슬라이더 초기화
                gui.masterSlider.SetValue(settingData.masterSound);
                gui.bgmSlider.SetValue(settingData.bgmSound);
                gui.sfxSlider.SetValue(settingData.sfxSound);

                // FillAmount 초기화
                gui.masterFillImage.SetFillAmount(gui.masterSlider.GetValue());
                gui.bgmFillImage.SetFillAmount(gui.bgmSlider.GetValue());
                gui.sfxFillImage.SetFillAmount(gui.sfxSlider.GetValue());

                // Label 초기화
                gui.masterSoundLabel.SetLabelText(gui.masterSlider.GetValue());
                gui.bgmSoundLabel.SetLabelText(gui.bgmSlider.GetValue());
                gui.sfxSoundLabel.SetLabelText(gui.sfxSlider.GetValue());

                // 음소거 토글 초기화
                gui.muteToggle.SetToggleValue(settingData.isMute);

                // 전체화면 토글 초기화
                gui.fullScreenToggle.SetFullScreen(settingData.isFullScreen);

                // 슬라이더 값 변경 이벤트 처리
                gui.masterSlider.onValueChanged = (value) =>
                {
                    Eclipse.Lobby.AudioManager.instance.PlaySfxWithCooldown((int)Eclipse.Lobby.AudioManager.Sfx.SFX_Check);

                    Eclipse.Lobby.AudioManager.instance.SettingData.masterSound = value;
                    if (value == 0.001f)
                    {
                        Eclipse.Lobby.AudioManager.instance.M_AudioMixer.SetFloat("Master", -80);
                    }
                    else
                    {
                        Eclipse.Lobby.AudioManager.instance.M_AudioMixer.SetFloat("Master", Mathf.Log10(value) * 20);
                    }
                    gui.masterFillImage.SetFillAmount(value); // FillAmount 변경
                    gui.masterSoundLabel.SetLabelText(value); // Label 변경
                };
                gui.masterSlider.Show();

                gui.bgmSlider.onValueChanged = (value) =>
                {
                    Eclipse.Lobby.AudioManager.instance.PlaySfxWithCooldown((int)Eclipse.Lobby.AudioManager.Sfx.SFX_Check);

                    Eclipse.Lobby.AudioManager.instance.SettingData.bgmSound = value;
                    if (value == 0.001f)
                    {
                        Eclipse.Lobby.AudioManager.instance.M_AudioMixer.SetFloat("BGM", -80);
                    }
                    else
                    {
                        Eclipse.Lobby.AudioManager.instance.M_AudioMixer.SetFloat("BGM", Mathf.Log10(value) * 20);
                    }
                    gui.bgmFillImage.SetFillAmount(value); // FillAmount 변경
                    gui.bgmSoundLabel.SetLabelText(value); // Label 변경
                };
                 gui.bgmSlider.Show();

                gui.sfxSlider.onValueChanged = (value) =>
                {
                    Eclipse.Lobby.AudioManager.instance.PlaySfxWithCooldown((int)Eclipse.Lobby.AudioManager.Sfx.SFX_Check);
                    
                    Eclipse.Lobby.AudioManager.instance.SettingData.sfxSound = value;
                    if (value == 0.001f)
                    {
                        Eclipse.Lobby.AudioManager.instance.M_AudioMixer.SetFloat("SFX", -80);
                    }
                    else
                    {
                        Eclipse.Lobby.AudioManager.instance.M_AudioMixer.SetFloat("SFX", Mathf.Log10(value) * 20);
                    }
                    gui.sfxFillImage.SetFillAmount(value); // FillAmount 변경
                    gui.sfxSoundLabel.SetLabelText(value); // Label 변경
                };
                gui.sfxSlider.Show();

                // 음소거 토글 값 변경 시 이벤트 처리
                gui.muteToggle.onToggleChanged = (isOn) =>
                {
                    Eclipse.Lobby.AudioManager.instance.SettingData.isMute = isOn;
                    AudioListener.volume = isOn ? 0 : 1;

                    gui.muteToggle.GetComponent<UnityEngine.UI.Toggle>().isOn = isOn;
                };
                gui.muteToggle.Show();

                // 전체화면 토글 값 변경 시 이벤트 처리
                gui.fullScreenToggle.onToggleChanged = (isOn) =>
                {
                    Eclipse.Lobby.AudioManager.instance.SettingData.isFullScreen = isOn;
                    gui.fullScreenToggle.SetFullScreen(isOn);
                };
                gui.fullScreenToggle.Show();

                if (stateMachine.HasContainedState(States.CharacterSelectPage))
                {
                    gui.characterSelectPageViewer.Show();
                }
                if (stateMachine.HasContainedState(States.CharacterDescriptionPage))
                {
                    gui.characterSelectPageViewer.Show();
                    gui.characterDescrptionViewer.Show();
                }

                settingPageViewer.Show();
            };

            //==================================================================

            void OnExit()
            {
                Debug.Log("Exit Setting Page");

                var settingPageViewer = gui.settingPageViewer;
                settingPageViewer.Hide();

                // 메인 로비 화면인 경우
                if (stateMachine.CurrentState.ID == States.MainPage)
                {
                    // 다른 버튼들 정상화
                    gui.exitButton.MakeInteractable();
                    gui.characterButton.MakeInteractable();
                    gui.gameDescriptionButton.MakeInteractable();
                }
                // 캐릭터 선택창인 경우
                if (stateMachine.CurrentState.ID == States.CharacterSelectPage)
                {
                    gui.characterSelectPageViewer.Show();

                    // 뒤로가기 버튼 및 캐릭터들 선택 버튼들 공통적으로 정상화
                    gui.characterSelectPageBackButton.MakeInteractable();
                    gui.mageSelectButton.MakeInteractable();
                    gui.warriorSelectButton.MakeInteractable();
                    gui.assassinSelectButton.MakeInteractable();
                }
                // 캐릭터 설명창인 경우
                if (stateMachine.CurrentState.ID == States.CharacterDescriptionPage)
                {
                    gui.gameStartButton.MakeInteractable();
                }
                

                // 슬라이더와 토글 이벤트 리스너 해제
                gui.masterSlider.RemoveListeners();
                gui.bgmSlider.RemoveListeners();
                gui.sfxSlider.RemoveListeners();
                gui.muteToggle.onToggleChanged = null;
            }

            //==================================================================

            var state = new State<States>(States.SettingPage);
            state.onEnter = OnEnter;
            state.onExit = OnExit;

            stateMachine.Add(state);
        }

        private void ActLikeSettingPageBackButton()
        {
            Eclipse.Lobby.AudioManager.instance.PlaySfx((int)Eclipse.Lobby.AudioManager.Sfx.Select);
            
            stateMachine.Pop();
        }
    }
}
