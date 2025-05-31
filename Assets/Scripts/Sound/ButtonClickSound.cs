using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Audio;

public class ButtonClickSound : MonoBehaviour, IPointerClickHandler
{
    public AudioClip clickSound;
    public AudioMixerGroup sfxGroup; // 在 Inspector 拖 SFX group 進來

    private AudioSource audioSource;

    void Start()
    {
        // 嘗試找到現有 AudioSource，沒有就自己建立一個
        audioSource = GameObject.Find("UIClickAudioSource")?.GetComponent<AudioSource>();
        if (audioSource == null)
        {
            GameObject obj = new GameObject("UIClickAudioSource");
            audioSource = obj.AddComponent<AudioSource>();
            DontDestroyOnLoad(obj);
        }

        // 將這個 AudioSource 的輸出指定到 SFX Mixer Group
        if (sfxGroup != null)
        {
            audioSource.outputAudioMixerGroup = sfxGroup;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickSound != null)
        {
            audioSource.Stop(); // 確保不會疊音
            audioSource.clip = clickSound;
            audioSource.Play();
        }
    }
}
