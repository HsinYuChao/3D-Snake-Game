using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public FoodManager foodManager;
    public BombManager bombManager;
    public LifeManager lifeManager;

    public GameObject gamePanel;
    public TMP_Text scoreText;
    public TMP_Text levelText;
    public TMP_Text timerText;
    private float gameTime = 0f;

    public GameObject gameOverPanel;
    public TMP_Text endingScoreText;
    public TMP_Text endingTimerText;

    public Image heart1;
    public Image heart2;
    public Image heart3;

    public GameObject pausePanel;
    public Button pauseButton;
    public Button resumeButton;

    public AudioClip gameOverSound;

    private bool isPaused = false;

    private int life = 3;
    private int score = 0;
    public bool isGameOver { get; private set; } = false;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        Time.timeScale = 1f;
        UpdateScoreText();
        UpdateLifeDisplay();
        UpdateLevelText();

        gamePanel?.SetActive(true);
        gameOverPanel?.SetActive(false);
        pausePanel?.SetActive(false);
    }

    private void Update()
    {
        if (!isGameOver && !isPaused)
        {
            gameTime += Time.deltaTime;
            UpdateTimerText();
        }
    }

    private void UpdateTimerText()
    {
        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(gameTime / 60f);
            int seconds = Mathf.FloorToInt(gameTime % 60f);
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    public void PlayerHitBody(int bodyIndex)
    {
        if (isGameOver) return;

        life--;
        UpdateLifeDisplay();
        Debug.Log($"撞到 body {bodyIndex}，剩餘生命：{life}");

        if (life <= 0)
        {
            GameOver(bodyIndex);
        }
    }

    public void PlayerHitBomb()
    {
        if (isGameOver) return;

        life--;
        UpdateLifeDisplay();
        Debug.Log("被炸彈炸到！剩餘生命：" + life);

        if (life <= 0)
        {
            GameOver(-1);
        }
    }


    public void GameOver(int reasonCode)
    {
        Debug.Log("Game Over - 原因代碼：" + reasonCode);
        Debug.Log("最終得分: " + score);

        // 播放 Game Over 音效
        if (gameOverSound != null && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlaySound(gameOverSound, 1f);
        }

        isGameOver = true;

        gamePanel?.SetActive(false);
        gameOverPanel?.SetActive(true);
        endingScoreText.text = score.ToString();
        endingTimerText.text = timerText.text;

        Time.timeScale = 1f;
        isPaused = false;
    }


    public void FoodEaten(GameObject food)
    {

        Destroy(food);
        SnakeManager.Instance.AddBodySegment();
        foodManager.SpawnFood();

        score++;
        UpdateScoreText();

        if (Random.value < 0.3f)
            bombManager.SpawnBomb();

        if (Random.value < 0.15f)
            lifeManager.SpawnLife();

        Debug.Log("吃到食物，得分：" + score);
    }


    private void UpdateScoreText()
    {
        if (scoreText != null)
            scoreText.text = score.ToString();
    }

    private void UpdateLifeDisplay()
    {
        heart1.enabled = life >= 1;
        heart2.enabled = life >= 2;
        heart3.enabled = life >= 3;
    }

    public void RetryGame()
    {
        pausePanel?.SetActive(false);

        Time.timeScale = 1f;
        isPaused = false;
        isGameOver = false;
        score = 0;
        life = 3;
        UpdateScoreText();
        UpdateLifeDisplay();

        gamePanel?.SetActive(true);
        gameOverPanel?.SetActive(false);
        pauseButton?.gameObject.SetActive(true);

        SnakeManager.Instance.ResetSnake();
        foodManager.ClearFood();
        foodManager.SpawnFood();

        bombManager.ClearBomb(); // 清除炸彈

        gameTime = 0f;
        UpdateTimerText();
    }

    private void UpdateLevelText()
    {
        if (levelText != null)
        {
            string sceneName = SceneManager.GetActiveScene().name;
            levelText.text = sceneName;
        }
    }

    public void PauseGame()
    {
        if (isGameOver || isPaused) return;

        isPaused = true;
        Time.timeScale = 0f;

        pausePanel?.SetActive(true);
        pauseButton?.gameObject.SetActive(false);
        resumeButton?.gameObject.SetActive(true);
    }

    public void ResumeGame()
    {
        if (!isPaused) return;

        isPaused = false;
        Time.timeScale = 1f;

        pausePanel?.SetActive(false);
        pauseButton?.gameObject.SetActive(true);
        resumeButton?.gameObject.SetActive(false);
    }

    public void AddLife(int amount)
    {
        if (isGameOver) return;

        life += amount;
        if (life > 3) life = 3;
        UpdateLifeDisplay();

        Debug.Log($"獲得生命，當前生命值：{life}");
    }


}
