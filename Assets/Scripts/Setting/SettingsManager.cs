using UnityEngine;
using UnityEngine.Audio;

public class SettingsManager : MonoBehaviour
{
    public static SettingsManager Instance;

    public AudioMixer audioMixer;

    // 目前音量數值 (0 ~ 1)
    private float bgmVolume = 0.5f;
    private float sfxVolume = 0.5f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 從 PlayerPrefs 載入
            bgmVolume = PlayerPrefs.GetFloat("BGMVolumeSlider", 0.5f);
            sfxVolume = PlayerPrefs.GetFloat("SFXVolumeSlider", 0.5f);

            ApplyVolumes();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // 將目前的數值設定到 AudioMixer
    private void ApplyVolumes()
    {
        SetBGMVolume(bgmVolume);
        SetSFXVolume(sfxVolume);
    }

    // 只更新數值並套用音量，不管UI
    public void SetBGMVolume(float value)
    {
        bgmVolume = value;
        float volume = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20;
        audioMixer.SetFloat("BGMVolume", volume);
        PlayerPrefs.SetFloat("BGMVolumeSlider", value);
    }

    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
        float volume = Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20;
        audioMixer.SetFloat("SFXVolume", volume);
        PlayerPrefs.SetFloat("SFXVolumeSlider", value);
    }

    // 外部取得目前值（用來同步UI）
    public float GetBGMVolume() => bgmVolume;
    public float GetSFXVolume() => sfxVolume;
}
