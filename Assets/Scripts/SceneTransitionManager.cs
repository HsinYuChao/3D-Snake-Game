using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    public Animator animator;  // 轉場動畫控制器
    private string targetScene;  // 目標場景名稱

    private bool isTransitioning = false;  // 標記是否正在進行過渡動畫

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // 確保場景中只有一個實例
        }
    }

    // 載入指定的場景
    public void LoadScene(string sceneName)
    {
        Debug.Log("嘗試載入場景: " + sceneName);
        targetScene = sceneName;
        if (!isTransitioning)
        {
            StartCoroutine(PlaySceneTransition());
        }
    }

    // 重試遊戲（場景重載）
    public void RetryGame()
    {
        if (!isTransitioning)
        {
            Debug.Log("RetryGame：重新開始遊戲");
            StartCoroutine(ReloadCurrentSceneCoroutine());  // 開始重載場景
        }
    }

    // 這是處理過渡動畫和場景加載的協程
    private IEnumerator PlaySceneTransition()
    {
        isTransitioning = true;  // 標記進行過渡動畫

        // 播放轉場動畫（fade out）
        animator.SetTrigger("change scene");

        // 等待動畫播放完畢
        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength);

        // 加載目標場景
        SceneManager.LoadScene(targetScene);

        // 等待場景加載完成
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetScene);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // 加載完成後播放 fade in 動畫
        animator.SetTrigger("change scene");

        isTransitioning = false;  // 讓過渡動畫結束，允許其他操作
    }

    // 重新加載當前場景
    private IEnumerator ReloadCurrentSceneCoroutine()
    {
        isTransitioning = true;  // 標記過渡動畫正在進行中

        // 播放過渡動畫（fade out）
        animator.SetTrigger("change scene");

        // 等待動畫播放完畢
        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength);

        // 加載當前場景
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);

        // 等待場景加載完成
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(currentScene);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // 加載完成後播放 fade in 動畫
        animator.SetTrigger("change scene");

        isTransitioning = false;  // 讓過渡動畫結束，允許其他操作
    }

    // 離開遊戲
    public void QuitGame()
    {
        Debug.Log("QuitGame：退出遊戲");
        Application.Quit();
    }
}


