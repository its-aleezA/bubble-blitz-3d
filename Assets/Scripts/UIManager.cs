using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_Text scoreText;
    public TMP_Text highScoreText;
    public TMP_Text levelText;
    public TMP_Text timerText;
    public Image[] lifeIcons;
    public GameObject pausePanel;
    
    [Header("Animation")]
    public Animator scoreAnimator;
    public Animator levelAnimator;
    
    private int currentScore = 0;
    private int highScore = 0;
    
    void Start()
    {
        // Load high score
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        UpdateHighScore();
        
        // Hide pause panel
        if (pausePanel != null)
            pausePanel.SetActive(false);
    }
    
    void Update()
    {
        // Pause with ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }
    
    public void UpdateScore(int newScore)
    {
        if (newScore > currentScore)
        {
            // Animate score increase
            if (scoreAnimator != null)
                scoreAnimator.Play("ScorePop");
        }
        
        currentScore = newScore;
        scoreText.text = currentScore.ToString("D6");
        
        // Update high score if needed
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetInt("HighScore", highScore);
            UpdateHighScore();
        }
    }
    
    void UpdateHighScore()
    {
        if (highScoreText != null)
            highScoreText.text = $"BEST: {highScore:D6}";
    }
    
    public void UpdateLevel(int level)
    {
        levelText.text = $"LEVEL {level}";
        
        // Animate level text
        if (levelAnimator != null)
            levelAnimator.Play("LevelShow");
    }
    
    public void UpdateTimer(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        timerText.text = $"{minutes:00}:{seconds:00}";
        
        // Flash when time is low
        if (time < 10f)
        {
            timerText.color = Color.Lerp(Color.red, Color.white, Mathf.PingPong(Time.time * 2f, 1f));
        }
    }
    
    public void UpdateLives(int lives)
    {
        for (int i = 0; i < lifeIcons.Length; i++)
        {
            if (lifeIcons[i] != null)
                lifeIcons[i].gameObject.SetActive(i < lives);
        }
    }
    
    public void TogglePause()
    {
        if (pausePanel != null)
        {
            bool isPaused = !pausePanel.activeSelf;
            pausePanel.SetActive(isPaused);
            Time.timeScale = isPaused ? 0f : 1f;
        }
    }
    
    public void ResumeGame()
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1f;
        }
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex);
    }
    
    public void QuitToMenu()
    {
        Time.timeScale = 1f;
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }
}