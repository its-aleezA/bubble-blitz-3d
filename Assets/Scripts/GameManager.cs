using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [Header("UI Elements")]
    public TMP_Text scoreText;
    public TMP_Text timerText;
    public TMP_Text levelText;
    public TMP_Text ballsText;
    public GameObject gameOverPanel;
    public GameObject levelCompletePanel;
    public Image nextBallImage;
    
    [Header("Game Settings")]
    public int score = 0;
    public int level = 1;
    public float timeRemaining = 60f; // Starts at 60
    public int ballsRemaining = 25;
    
    [Header("Bubble Colors")]
    public Color[] colors = new Color[5];
    private int nextColorIndex = 0;
    
    [Header("References")]
    public GameObject bubblePrefab;
    private List<GameObject> activeBubbles = new List<GameObject>();
    
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        // Initialize values
        score = 0;
        timeRemaining = 60f;
        ballsRemaining = 25;
        level = 1;
    }
    
    void Start()
    {
        Debug.Log("GameManager Start() called");
        
        // Initialize colors PROPERLY
        colors = new Color[5];
        colors[0] = new Color(1f, 0.2f, 0.2f, 0.85f);    // Red
        colors[1] = new Color(0.2f, 0.4f, 1f, 0.85f);    // Blue  
        colors[2] = new Color(0.2f, 0.8f, 0.2f, 0.85f);  // Green
        colors[3] = new Color(1f, 0.8f, 0.2f, 0.85f);    // Yellow
        colors[4] = new Color(0.8f, 0.2f, 0.8f, 0.85f);  // Purple
        
        // Debug colors
        for (int i = 0; i < colors.Length; i++)
        {
            Debug.Log($"Color {i}: R={colors[i].r}, G={colors[i].g}, B={colors[i].b}");
        }
        
        nextColorIndex = Random.Range(0, colors.Length);
        
        // Make sure panels are hidden
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        if (levelCompletePanel != null)
            levelCompletePanel.SetActive(false);
        
        UpdateNextBallDisplay();
        UpdateUI();
        
        // Spawn initial bubbles
        SpawnInitialBubbles();
        
        Debug.Log("Game initialized - Score: 0, Time: " + timeRemaining);
    }
    
    void Update()
    {
        // Timer countdown
        if (timeRemaining > 0)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                GameOver();
            }
            
            // Update timer display every frame
            if (timerText != null)
                timerText.text = $"Time: {Mathf.CeilToInt(timeRemaining)}";
        }
    }
    
    void SpawnInitialBubbles()
    {
        int rows = level == 1 ? 5 : 7;
        int cols = 8;
        float spacing = 1.2f;
        
        // Clear any existing bubbles
        Bubble[] existingBubbles = FindObjectsOfType<Bubble>();
        foreach (Bubble bubble in existingBubbles)
        {
            if (bubble.gameObject != bubblePrefab)
                Destroy(bubble.gameObject);
        }
        activeBubbles.Clear();
        
        // Create grid
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                // Calculate position
                float xOffset = (row % 2 == 0) ? 0 : spacing * 0.5f;
                Vector3 position = new Vector3(
                    (col * spacing) + xOffset - ((cols - 1) * spacing * 0.5f),
                    (row * spacing * 0.866f) + 4f,
                    0
                );
                
                // Create bubble
                GameObject bubble = Instantiate(bubblePrefab, position, Quaternion.identity);
                bubble.name = $"Bubble_R{row}_C{col}"; // For debugging
                
                Bubble bubbleScript = bubble.GetComponent<Bubble>();
                
                if (bubbleScript != null)
                {
                    // Random color (limited in level 1)
                    int maxColors = level == 1 ? 3 : 5;
                    int colorIndex = Random.Range(0, maxColors);
                    Color bubbleColor = colors[colorIndex];
                    
                    bubbleScript.SetColor(colorIndex, bubbleColor);
                    Debug.Log($"Created bubble at ({row},{col}) with color index {colorIndex}");
                }
                
                // Make it stick to grid
                Rigidbody rb = bubble.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                    rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
                }
                
                activeBubbles.Add(bubble);
            }
        }
        
        Debug.Log("Spawned " + activeBubbles.Count + " bubbles in grid");
    }
    
    public void AddScore(int points)
    {
        score += points;
        Debug.Log("Score added: " + points + " | Total: " + score);
        UpdateUI();
    }
    
    public void BallShot()
    {
        ballsRemaining--;
        Debug.Log("Ball shot. Remaining: " + ballsRemaining);
        
        if (ballsRemaining <= 0)
        {
            Debug.Log("No balls left - Game Over");
            GameOver();
            return;
        }
        
        nextColorIndex = Random.Range(0, colors.Length);
        UpdateNextBallDisplay();
        UpdateUI();
    }
    
    public int GetNextColorIndex()
    {
        return nextColorIndex;
    }
    
    public Color GetColor(int index)
    {
        if (index >= 0 && index < colors.Length)
            return colors[index];
        return Color.white;
    }
    
    void UpdateNextBallDisplay()
    {
        if (nextBallImage != null && colors.Length > nextColorIndex)
        {
            nextBallImage.color = colors[nextColorIndex];
        }
    }
    
    void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
            Debug.Log("UI Updated - Score: " + score);
        }
        
        if (timerText != null)
            timerText.text = $"Time: {Mathf.CeilToInt(timeRemaining)}";
        
        if (levelText != null)
            levelText.text = $"Level: {level}";
        
        if (ballsText != null)
            ballsText.text = $"Balls: {ballsRemaining}";
    }
    
    public void GameOver()
    {
        Debug.Log("GameOver() called");
        
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Debug.Log("Game Over panel activated");
        }
        else
        {
            Debug.LogError("Game Over panel is null!");
        }
        
        Time.timeScale = 0f;
    }
    
    void LevelComplete()
    {
        Debug.Log("LevelComplete() called - Level: " + level);
        
        if (level == 1)
        {
            level = 2;
            if (levelCompletePanel != null)
            {
                levelCompletePanel.SetActive(true);
                Invoke("LoadNextLevel", 2f);
            }
        }
        else
        {
            // Game complete
            if (levelCompletePanel != null)
            {
                TMP_Text text = levelCompletePanel.GetComponentInChildren<TMP_Text>();
                if (text != null) text.text = "YOU WIN!";
                levelCompletePanel.SetActive(true);
            }
        }
    }
    
    void LoadNextLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void BubbleDestroyed(GameObject bubble)
    {
        if (activeBubbles.Contains(bubble))
        {
            activeBubbles.Remove(bubble);
            Debug.Log("Bubble destroyed. Remaining: " + activeBubbles.Count);
        }
        
        // Check win condition
        if (activeBubbles.Count == 0)
        {
            LevelComplete();
        }
    }
    
    // BUTTON FUNCTIONS - MUST BE PUBLIC
    public void RestartGame()
    {
        Debug.Log("RestartGame() called");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void MainMenu()
    {
        Debug.Log("MainMenu() called");
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
    
    public void QuitGame()
    {
        Debug.Log("QuitGame() called");
        Application.Quit();
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}