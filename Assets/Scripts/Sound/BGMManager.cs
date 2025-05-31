using UnityEngine;

public class BGMManager : MonoBehaviour
{
    public static BGMManager Instance;

    private AudioSource audioSource;

    void Awake()
    {
        // 如果已經有一個 BGMManager，就摧毀重複的
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        // 設定為唯一實例並且不被銷毀
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // 取得 AudioSource
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 0.5f;

    }

    public void PlayMusic(AudioClip clip)
    {
        if (audioSource.clip == clip) return; // 避免重複播放同一首
        audioSource.clip = clip;
        audioSource.Play();
    }
}
