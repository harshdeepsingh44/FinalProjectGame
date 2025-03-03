using UnityEngine;
using System.Collections.Generic;

public class ObstacleManager : MonoBehaviour
{
    public static ObstacleManager Instance { get; private set; }

    [Header("Enemy Ship Settings")]
    public GameObject[] enemyShipPrefabs;  // Array of different enemy ship prefabs
    public float spawnInterval = 3f;       // Time between spawns
    public float verticalGap = 3f;         // Gap between ships in a formation
    public float moveSpeed = 3f;           // Speed of enemy ships
    
    private float nextSpawnTime;
    private List<GameObject> activeObstacles = new List<GameObject>();
    private float screenWidth;
    private float screenHeight;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    private void Start()
    {
        // Calculate screen boundaries in world coordinates
        Camera mainCamera = Camera.main;
        screenHeight = 2f * mainCamera.orthographicSize;
        screenWidth = screenHeight * mainCamera.aspect;
        
        nextSpawnTime = Time.time + spawnInterval;
    }
    
    private void Update()
    {
        if (!GameManager.Instance.isGameActive) return;
        
        // Spawn new enemies
        if (Time.time >= nextSpawnTime)
        {
            SpawnEnemyFormation();
            nextSpawnTime = Time.time + spawnInterval;
        }
        
        // Move and cleanup enemies
        MoveEnemies();
        CleanupEnemies();
    }
    
    private void SpawnEnemyFormation()
    {
        if (enemyShipPrefabs == null || enemyShipPrefabs.Length == 0)
        {
            Debug.LogError("No enemy ship prefabs assigned!");
            return;
        }

        float centerY = Random.Range(-screenHeight/4, screenHeight/4);
        int formationSize = Random.Range(2, 5); // Random formation size
        
        for (int i = 0; i < formationSize; i++)
        {
            // Pick a random enemy ship prefab
            GameObject prefab = enemyShipPrefabs[Random.Range(0, enemyShipPrefabs.Length)];
            
            // Calculate position
            float yOffset = (i - (formationSize-1)/2f) * verticalGap;
            Vector3 spawnPos = new Vector3(screenWidth/2 + 2f, centerY + yOffset, 0);
            
            // Spawn enemy
            GameObject enemy = Instantiate(prefab, spawnPos, Quaternion.identity);
            enemy.tag = "Enemy"; // Ensure enemy tag is set
            
            // Ensure proper collider setup
            BoxCollider2D collider = enemy.GetComponent<BoxCollider2D>();
            if (collider == null)
            {
                collider = enemy.AddComponent<BoxCollider2D>();
            }
            collider.isTrigger = true;
            
            // Adjust collider size to match sprite
            SpriteRenderer spriteRenderer = enemy.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                collider.size = spriteRenderer.bounds.size;
            }
            
            // Add Rigidbody2D for better collision detection
            Rigidbody2D rb = enemy.GetComponent<Rigidbody2D>();
            if (rb == null)
            {
                rb = enemy.AddComponent<Rigidbody2D>();
            }
            rb.isKinematic = true; // We'll move it manually
            
            activeObstacles.Add(enemy);
        }
        
        // Formation is complete
    }
    
    private void MoveEnemies()
    {
        foreach (GameObject enemy in activeObstacles.ToArray())
        {
            if (enemy != null)
            {
                enemy.transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
            }
        }
    }
    
    private void CleanupEnemies()
    {
        activeObstacles.RemoveAll(enemy => 
        {
            if (enemy == null) return true;
            
            if (enemy.transform.position.x < -screenWidth/2 - 2f)
            {
                Destroy(enemy);
                return true;
            }
            return false;
        });
    }

    public void ClearAllEnemies()
    {
        foreach (GameObject enemy in activeObstacles.ToArray())
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        activeObstacles.Clear();
        nextSpawnTime = Time.time + spawnInterval;
    }
}
