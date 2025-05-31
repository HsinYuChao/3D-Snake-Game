using UnityEngine;
using UnityEngine.UI;

public class SettingsUIController : MonoBehaviour
{
    public GameObject settingsPanel;
    public Slider bgmSlider;
    public Slider sfxSlider;

    void Start()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);

        if (SettingsManager.Instance == null)
        {
            Debug.LogWarning("找不到 SettingsManager 單例");
            return;
        }

        // 初始化滑桿值，跟 SettingsManager 保持一致
        bgmSlider.value = SettingsManager.Instance.GetBGMVolume();
        sfxSlider.value = SettingsManager.Instance.GetSFXVolume();

        // 清除舊監聽器，防止重複綁定
        bgmSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.onValueChanged.RemoveAllListeners();

        // 新監聽器，滑桿改變時呼叫 SettingsManager 的設定方法
        bgmSlider.onValueChanged.AddListener(SettingsManager.Instance.SetBGMVolume);
        sfxSlider.onValueChanged.AddListener(SettingsManager.Instance.SetSFXVolume);
    }

    // UI按鈕呼叫，開啟設定面板
    public void OpenSettings()
    {
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);

            // 每次打開時再同步滑桿（以防場景切換）
            bgmSlider.value = SettingsManager.Instance.GetBGMVolume();
            sfxSlider.value = SettingsManager.Instance.GetSFXVolume();
        }
    }

    // UI按鈕呼叫，關閉設定面板
    public void CloseSettings()
    {
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }
}

