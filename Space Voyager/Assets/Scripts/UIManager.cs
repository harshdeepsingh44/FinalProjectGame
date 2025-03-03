using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI Elements")]
    public Canvas gameCanvas;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI highScoreText;
    private GameObject gameOverPanel;
    private TextMeshProUGUI finalScoreText;
    private Button restartButton;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SetupUI();
            // Initialize UI with starting values
            UpdateUI(0, PlayerPrefs.GetFloat("HighScore", 0));
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void UpdateUI(float score, float highScore)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {Mathf.Floor(score)}";
        }

        if (highScoreText != null)
        {
            highScoreText.text = $"High Score: {Mathf.Floor(highScore)}";
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = $"Final Score: {Mathf.Floor(score)}";
        }
    }

    public void ShowGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    public void HideGameOverPanel()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    private void SetupUI()
    {
        Debug.Log("Setting up UI...");
        
        // Create Canvas if it doesn't exist
        if (gameCanvas == null)
        {
            Debug.Log("Creating new Canvas");
            GameObject canvasObj = new GameObject("GameCanvas");
            gameCanvas = canvasObj.AddComponent<Canvas>();
            gameCanvas.renderMode = RenderMode.ScreenSpaceOverlay;
            gameCanvas.sortingOrder = 1; // Ensure it's on top

            // Create game over panel
            gameOverPanel = new GameObject("GameOverPanel");
            gameOverPanel.transform.SetParent(gameCanvas.transform, false);
            
            // Add panel image
            Image panelImage = gameOverPanel.AddComponent<Image>();
            panelImage.color = new Color(0, 0, 0, 0.8f);
            
            // Set panel size
            RectTransform panelRect = gameOverPanel.GetComponent<RectTransform>();
            panelRect.anchorMin = Vector2.zero;
            panelRect.anchorMax = Vector2.one;
            panelRect.sizeDelta = Vector2.zero;
            
            // Add Game Over text
            GameObject gameOverTextObj = new GameObject("GameOverText");
            gameOverTextObj.transform.SetParent(gameOverPanel.transform, false);
            TextMeshProUGUI gameOverText = gameOverTextObj.AddComponent<TextMeshProUGUI>();
            gameOverText.text = "Game Over";
            gameOverText.fontSize = 72;
            gameOverText.color = Color.white;
            gameOverText.alignment = TextAlignmentOptions.Center;
            gameOverText.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
            if (gameOverText.font == null)
            {
                Debug.LogWarning("Could not load default font, trying to use any available TMP font");
                gameOverText.font = Resources.FindObjectsOfTypeAll<TMP_FontAsset>()[0];
            }
            gameOverText.enableWordWrapping = false;
            gameOverText.overflowMode = TextOverflowModes.Overflow;
            
            // Position Game Over text
            RectTransform gameOverTextRect = gameOverTextObj.GetComponent<RectTransform>();
            gameOverTextRect.anchorMin = new Vector2(0.5f, 0.7f);
            gameOverTextRect.anchorMax = new Vector2(0.5f, 0.7f);
            gameOverTextRect.sizeDelta = new Vector2(600, 100);
            gameOverTextRect.anchoredPosition = Vector2.zero;
            
            // Add Final Score text
            GameObject finalScoreObj = new GameObject("FinalScoreText");
            finalScoreObj.transform.SetParent(gameOverPanel.transform, false);
            finalScoreText = finalScoreObj.AddComponent<TextMeshProUGUI>();
            finalScoreText.text = "Final Score: 0";
            finalScoreText.fontSize = 48;
            finalScoreText.color = Color.white;
            finalScoreText.alignment = TextAlignmentOptions.Center;
            finalScoreText.font = gameOverText.font; // Use the same font
            finalScoreText.enableWordWrapping = false;
            finalScoreText.overflowMode = TextOverflowModes.Overflow;
            
            // Position Final Score text
            RectTransform finalScoreRect = finalScoreObj.GetComponent<RectTransform>();
            finalScoreRect.anchorMin = new Vector2(0.5f, 0.5f);
            finalScoreRect.anchorMax = new Vector2(0.5f, 0.5f);
            finalScoreRect.sizeDelta = new Vector2(400, 80);
            finalScoreRect.anchoredPosition = Vector2.zero;
            
            // Add Restart button
            GameObject restartButtonObj = new GameObject("RestartButton");
            restartButtonObj.transform.SetParent(gameOverPanel.transform, false);
            
            // Add button image
            Image restartButtonImage = restartButtonObj.AddComponent<Image>();
            restartButtonImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            
            // Add button component
            restartButton = restartButtonObj.AddComponent<Button>();
            restartButton.targetGraphic = restartButtonImage;
            
            // Add button text
            GameObject restartButtonTextObj = new GameObject("RestartButtonText");
            restartButtonTextObj.transform.SetParent(restartButtonObj.transform, false);
            TextMeshProUGUI restartButtonText = restartButtonTextObj.AddComponent<TextMeshProUGUI>();
            restartButtonText.text = "Restart";
            restartButtonText.fontSize = 36;
            restartButtonText.color = Color.white;
            restartButtonText.alignment = TextAlignmentOptions.Center;
            restartButtonText.font = gameOverText.font; // Use the same font
            restartButtonText.enableWordWrapping = false;
            restartButtonText.overflowMode = TextOverflowModes.Overflow;
            
            // Position restart button
            RectTransform restartButtonRect = restartButtonObj.GetComponent<RectTransform>();
            restartButtonRect.anchorMin = new Vector2(0.5f, 0.3f);
            restartButtonRect.anchorMax = new Vector2(0.5f, 0.3f);
            restartButtonRect.sizeDelta = new Vector2(200, 60);
            restartButtonRect.anchoredPosition = Vector2.zero;
            
            // Add button click handler
            restartButton.onClick.AddListener(() => {
                HideGameOverPanel();
                GameManager.Instance.StartGame();
            });
            
            // Hide panel initially
            gameOverPanel.SetActive(false);
            
            // Add Canvas Scaler
            CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            
            // Add Graphics Raycaster
            canvasObj.AddComponent<GraphicRaycaster>();

            // Create Event System if it doesn't exist
            if (FindFirstObjectByType<EventSystem>() == null)
            {
                GameObject eventSystem = new GameObject("EventSystem");
                eventSystem.AddComponent<EventSystem>();
                eventSystem.AddComponent<StandaloneInputModule>();
            }

            // Add start button
            GameObject startButtonObj = new GameObject("StartButton");
            startButtonObj.transform.SetParent(gameCanvas.transform, false);
            
            // Add button image
            Image startButtonImage = startButtonObj.AddComponent<Image>();
            startButtonImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
            
            // Add button component
            Button startButton = startButtonObj.AddComponent<Button>();
            startButton.targetGraphic = startButtonImage;
            
            // Create button text
            GameObject startButtonTextObj = new GameObject("ButtonText");
            startButtonTextObj.transform.SetParent(startButtonObj.transform, false);
            TextMeshProUGUI startButtonText = startButtonTextObj.AddComponent<TextMeshProUGUI>();
            startButtonText.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
            startButtonText.text = "Start Game";
            startButtonText.fontSize = 36;
            startButtonText.color = Color.white;
            startButtonText.alignment = TextAlignmentOptions.Center;

            // Position button
            RectTransform startButtonRect = startButtonObj.GetComponent<RectTransform>();
            startButtonRect.anchorMin = new Vector2(0.5f, 0.5f);
            startButtonRect.anchorMax = new Vector2(0.5f, 0.5f);
            startButtonRect.sizeDelta = new Vector2(200, 60);
            startButtonRect.anchoredPosition = Vector2.zero;

            // Add click handler
            startButton.onClick.AddListener(() => {
                startButtonObj.SetActive(false);
                GameManager.Instance.StartGame();
            });
        }

        CreateGameOverPanel();

        // Create Score Text if it doesn't exist
        if (scoreText == null)
        {
            Debug.Log("Creating Score Text");
            GameObject scoreObj = new GameObject("ScoreText");
            scoreObj.transform.SetParent(gameCanvas.transform, false);
            scoreText = scoreObj.AddComponent<TextMeshProUGUI>();
            scoreText.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF"); // Set default font
            scoreText.fontSize = 36;
            scoreText.alignment = TextAlignmentOptions.TopLeft;
            scoreText.text = "Score: 0";
            scoreText.color = Color.white;
            
            // Position in top-left corner
            RectTransform rectTransform = scoreText.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(0, 1);
            rectTransform.pivot = new Vector2(0, 1);
            rectTransform.anchoredPosition = new Vector2(20, -20);
            rectTransform.sizeDelta = new Vector2(200, 50);
        }

        // Create High Score Text if it doesn't exist
        if (highScoreText == null)
        {
            Debug.Log("Creating High Score Text");
            GameObject highScoreObj = new GameObject("HighScoreText");
            highScoreObj.transform.SetParent(gameCanvas.transform, false);
            highScoreText = highScoreObj.AddComponent<TextMeshProUGUI>();
            highScoreText.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF"); // Set default font
            highScoreText.fontSize = 36;
            highScoreText.alignment = TextAlignmentOptions.TopRight;
            highScoreText.text = "High Score: 0";
            highScoreText.color = Color.white;
            
            // Position in top-right corner
            RectTransform rectTransform = highScoreText.GetComponent<RectTransform>();
            rectTransform.anchorMin = new Vector2(1, 1);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.pivot = new Vector2(1, 1);
            rectTransform.anchoredPosition = new Vector2(-20, -20);
            rectTransform.sizeDelta = new Vector2(200, 50);
        }
    }

    private void CreateGameOverPanel()
    {
        if (gameOverPanel != null) return;

        // Create Game Over Panel
        gameOverPanel = new GameObject("GameOverPanel");
        gameOverPanel.transform.SetParent(gameCanvas.transform, false);
        
        // Setup panel rect transform
        RectTransform panelRect = gameOverPanel.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.sizeDelta = Vector2.zero;

        // Add panel background
        Image panelBg = gameOverPanel.AddComponent<Image>();
        panelBg.color = new Color(0, 0, 0, 0.8f);

        // Create "GAME OVER" text
        GameObject gameOverTextObj = new GameObject("GameOverText");
        gameOverTextObj.transform.SetParent(gameOverPanel.transform, false);
        TextMeshProUGUI gameOverText = gameOverTextObj.AddComponent<TextMeshProUGUI>();
        gameOverText.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        gameOverText.text = "GAME OVER";
        gameOverText.fontSize = 72;
        gameOverText.color = Color.red;
        gameOverText.alignment = TextAlignmentOptions.Center;

        RectTransform gameOverTextRect = gameOverTextObj.GetComponent<RectTransform>();
        gameOverTextRect.anchorMin = new Vector2(0.5f, 0.7f);
        gameOverTextRect.anchorMax = new Vector2(0.5f, 0.8f);
        gameOverTextRect.sizeDelta = new Vector2(600, 100);
        gameOverTextRect.anchoredPosition = Vector2.zero;

        // Create final score text
        GameObject finalScoreObj = new GameObject("FinalScoreText");
        finalScoreObj.transform.SetParent(gameOverPanel.transform, false);
        finalScoreText = finalScoreObj.AddComponent<TextMeshProUGUI>();
        finalScoreText.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        finalScoreText.fontSize = 48;
        finalScoreText.color = Color.white;
        finalScoreText.alignment = TextAlignmentOptions.Center;

        RectTransform finalScoreRect = finalScoreObj.GetComponent<RectTransform>();
        finalScoreRect.anchorMin = new Vector2(0.5f, 0.5f);
        finalScoreRect.anchorMax = new Vector2(0.5f, 0.6f);
        finalScoreRect.sizeDelta = new Vector2(400, 80);
        finalScoreRect.anchoredPosition = Vector2.zero;

        // Create restart button
        GameObject buttonObj = new GameObject("RestartButton");
        buttonObj.transform.SetParent(gameOverPanel.transform, false);
        
        // Add button image
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.2f, 0.2f, 1f);
        
        // Add button component
        restartButton = buttonObj.AddComponent<Button>();
        restartButton.targetGraphic = buttonImage;
        
        // Create button text
        GameObject buttonTextObj = new GameObject("ButtonText");
        buttonTextObj.transform.SetParent(buttonObj.transform, false);
        TextMeshProUGUI buttonText = buttonTextObj.AddComponent<TextMeshProUGUI>();
        buttonText.font = Resources.Load<TMP_FontAsset>("Fonts & Materials/LiberationSans SDF");
        buttonText.text = "Restart";
        buttonText.fontSize = 36;
        buttonText.color = Color.white;
        buttonText.alignment = TextAlignmentOptions.Center;

        // Position button
        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.3f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.4f);
        buttonRect.sizeDelta = new Vector2(200, 60);
        buttonRect.anchoredPosition = Vector2.zero;

        // Add button click handler
        restartButton.onClick.AddListener(() => {
            HideGameOverPanel();
            GameManager.Instance.StartGame();
        });

        // Hide panel initially
        gameOverPanel.SetActive(false);
    }

    // This method will be removed as we have ShowGameOverPanel and HideGameOverPanel above
}
