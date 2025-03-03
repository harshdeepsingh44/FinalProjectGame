using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    
    [Header("Game Settings")]
    public float gameSpeed = 5f;
    public float speedIncreaseRate = 0.1f;
    public float distanceTraveled = 0f;
    public bool isGameActive = false;

    [Header("Prefabs")]
    public GameObject playerPrefab; // Assign the super spaceship prefab here

    private PlayerController playerController; // Assign the player from the scene
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Initialize high score to 0
            PlayerPrefs.SetFloat("HighScore", 0f);
            PlayerPrefs.Save();
            
            InitializeGame();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeGame()
    {
        Debug.Log("Initializing game...");
        
        // Initialize game state
        isGameActive = false;
        gameSpeed = 5f;
        distanceTraveled = 0f;
        Time.timeScale = 1;

        // Find existing player first
        if (playerController == null)
        {
            playerController = FindFirstObjectByType<PlayerController>();
        }

        // Create new player if none exists and we have a prefab
        if (playerController == null && playerPrefab != null)
        {
            Debug.Log("Creating new player from prefab...");
            GameObject playerObj = Instantiate(playerPrefab, new Vector3(-7, 0, 0), Quaternion.identity);
            playerObj.name = "Player";
            playerController = playerObj.GetComponent<PlayerController>();
        }

        // Verify we have a player one way or another
        if (playerController == null)
        {
            Debug.LogError("No player found and couldn't create one! Make sure there's a player in the scene or the prefab is assigned.");
            return;
        }

        // Set initial state but keep visible
        playerController.transform.position = new Vector3(-7, 0, 0);
        playerController.gameObject.SetActive(true); // Keep visible
        if (playerController.GetComponent<Rigidbody2D>() != null)
        {
            playerController.GetComponent<Rigidbody2D>().simulated = false;
        }

        Debug.Log("Player initialized and positioned.");
        
        // Clear any existing enemies
        if (ObstacleManager.Instance != null)
        {
            ObstacleManager.Instance.ClearAllEnemies();
        }

        Debug.Log("Game initialization complete.");
    }

    private void Update()
    {
        if (isGameActive)
        {
            // Increase distance traveled
            distanceTraveled += gameSpeed * Time.deltaTime;
            
            // Gradually increase game speed
            gameSpeed += speedIncreaseRate * Time.deltaTime;

            // Update UI with current score
            if (UIManager.Instance != null)
            {
                UIManager.Instance.UpdateUI(distanceTraveled, PlayerPrefs.GetFloat("HighScore", 0f));
            }
        }
    }

    public void StartGame()
    {
        Debug.Log("Starting game...");
        
        // Find the player first
        PlayerController player = FindFirstObjectByType<PlayerController>();
        if (player == null)
        {
            Debug.LogError("Player not found in scene!");
            return; // Don't start the game if player is missing
        }

        // Activate player first
        Debug.Log("Setting up player...");
        player.gameObject.SetActive(true);
        player.transform.position = new Vector3(-7, 0, 0);
        player.ResetPlayer();
        Debug.Log("Player is active: " + player.gameObject.activeSelf);

        // Reset game state
        isGameActive = true;
        gameSpeed = 5f;
        distanceTraveled = 0f;
        Time.timeScale = 1;

        // Clear all existing enemies
        if (ObstacleManager.Instance != null)
        {
            ObstacleManager.Instance.ClearAllEnemies();
        }

        // Make sure we have a valid player reference
        if (playerController == null)
        {
            playerController = FindFirstObjectByType<PlayerController>();
            if (playerController == null)
            {
                Debug.LogError("No player found in scene!");
                return;
            }
        }

        // Reset game state
        isGameActive = true;
        gameSpeed = 5f;
        distanceTraveled = 0f;
        Time.timeScale = 1;

        // Clear all existing enemies
        if (ObstacleManager.Instance != null)
        {
            ObstacleManager.Instance.ClearAllEnemies();
        }

        // Reset and show player
        Debug.Log("Setting up player...");
        playerController.transform.position = new Vector3(-7, 0, 0);
        playerController.gameObject.SetActive(true);
        playerController.ResetPlayer();
        Debug.Log("Player is active: " + playerController.gameObject.activeSelf);

        // Update UI
        if (UIManager.Instance != null)
        {
            UIManager.Instance.HideGameOverPanel();
            UIManager.Instance.UpdateUI(distanceTraveled, PlayerPrefs.GetFloat("HighScore", 0f));
        }

        Debug.Log("Game started successfully.");
    }

    public void GameOver()
    {
        if (!isGameActive) return; // Prevent multiple game over calls

        Debug.Log("Game Over called");
        isGameActive = false;
        Time.timeScale = 0;
        
        // Save high score
        float currentScore = distanceTraveled;
        float highScore = PlayerPrefs.GetFloat("HighScore", 0f);
        if (currentScore > highScore)
        {
            PlayerPrefs.SetFloat("HighScore", currentScore);
            PlayerPrefs.Save();
        }

        // Update UI first before modifying player
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameOverPanel();
            UIManager.Instance.UpdateUI(currentScore, highScore);
        }

        // Disable player physics but keep visible
        if (playerController != null)
        {
            Rigidbody2D rb = playerController.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.simulated = false;
                rb.linearVelocity = Vector2.zero;
            }
        }

        // Show game over UI
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameOverPanel();
        }
    }
}
