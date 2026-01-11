using UnityEngine;

[CreateAssetMenu(fileName = "SettingData", menuName = "Scriptable Object/SettingData")]

public class SettingData : ScriptableObject
{
    // 사운드 시스템
    [Header("# Sound 정보")]
    public float masterSound;
    public float bgmSound;
    public float sfxSound;
    public bool isMute;

    // 그래픽 시스템
    [Header("# Screen 정보")]
    public bool isFullScreen;
}