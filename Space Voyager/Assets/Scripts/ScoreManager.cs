using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    private const string HIGH_SCORE_KEY = "HighScore";
    private float currentScore;
    private float highScore;
    
    private TextMeshProUGUI scoreText;
    private TextMeshProUGUI highScoreText;
    
    private void Start()
    {
        // Get references from UIManager
        if (UIManager.Instance != null)
        {
            scoreText = UIManager.Instance.scoreText;
            highScoreText = UIManager.Instance.highScoreText;
            Debug.Log("UI references obtained from UIManager");
        }
        else
        {
            Debug.LogError("UIManager instance not found!");
        }

        highScore = PlayerPrefs.GetFloat(HIGH_SCORE_KEY, 0);
        currentScore = 0;
        UpdateHighScoreText();
        UpdateScoreText();
        
        Debug.Log("ScoreManager initialized. High Score: " + highScore);
    }
    
    private void Update()
    {
        if (!GameManager.Instance.isGameActive) return;
        
        // Update current score based on distance traveled
        currentScore = GameManager.Instance.distanceTraveled;
        UpdateScoreText();
        
        // Check for new high score
        if (currentScore > highScore)
        {
            highScore = currentScore;
            PlayerPrefs.SetFloat(HIGH_SCORE_KEY, highScore);
            UpdateHighScoreText();
        }
    }
    
    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {Mathf.Floor(currentScore)}";
        }
    }
    
    private void UpdateHighScoreText()
    {
        if (highScoreText != null)
        {
            highScoreText.text = $"High Score: {Mathf.Floor(highScore)}";
        }
    }
}
