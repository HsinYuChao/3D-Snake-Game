using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance;

    public Animator animator;  // ����ʵe���
    private string targetScene;  // �ؼг����W��

    private bool isTransitioning = false;  // �аO�O�_���b�i��L��ʵe

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // �T�O�������u���@�ӹ��
        }
    }

    // ���J���w������
    public void LoadScene(string sceneName)
    {
        Debug.Log("���ո��J����: " + sceneName);
        targetScene = sceneName;
        if (!isTransitioning)
        {
            StartCoroutine(PlaySceneTransition());
        }
    }

    // ���չC���]���������^
    public void RetryGame()
    {
        if (!isTransitioning)
        {
            Debug.Log("RetryGame�G���s�}�l�C��");
            StartCoroutine(ReloadCurrentSceneCoroutine());  // �}�l��������
        }
    }

    // �o�O�B�z�L��ʵe�M�����[������{
    private IEnumerator PlaySceneTransition()
    {
        isTransitioning = true;  // �аO�i��L��ʵe

        // ��������ʵe�]fade out�^
        animator.SetTrigger("change scene");

        // ���ݰʵe���񧹲�
        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength);

        // �[���ؼг���
        SceneManager.LoadScene(targetScene);

        // ���ݳ����[������
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetScene);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // �[�������Ἵ�� fade in �ʵe
        animator.SetTrigger("change scene");

        isTransitioning = false;  // ���L��ʵe�����A���\��L�ާ@
    }

    // ���s�[����e����
    private IEnumerator ReloadCurrentSceneCoroutine()
    {
        isTransitioning = true;  // �аO�L��ʵe���b�i�椤

        // ����L��ʵe�]fade out�^
        animator.SetTrigger("change scene");

        // ���ݰʵe���񧹲�
        float animationLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength);

        // �[����e����
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);

        // ���ݳ����[������
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(currentScene);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        // �[�������Ἵ�� fade in �ʵe
        animator.SetTrigger("change scene");

        isTransitioning = false;  // ���L��ʵe�����A���\��L�ާ@
    }

    // ���}�C��
    public void QuitGame()
    {
        Debug.Log("QuitGame�G�h�X�C��");
        Application.Quit();
    }
}


