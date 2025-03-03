using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    public float jumpVelocity = 2.5f;   // Further reduced jump velocity
    public float forwardSpeed = 1f;     // Even slower forward speed
    public float gravityScale = 0.7f;   // Even lighter gravity
    public float fallMultiplier = 1.1f; // Very gentle falling

    private Rigidbody2D rb;
    private bool isDead = false;

    private void Start()
    {
        SetupComponents();
        
        // Keep visible but disable physics
        if (rb != null)
        {
            rb.simulated = false;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
        isDead = false;
        transform.position = new Vector3(-7, 0, 0); // Set initial position
    }

    private void SetupComponents()
    {
        // Setup Rigidbody2D
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
        
        // Configure Rigidbody2D
        rb.gravityScale = gravityScale;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.mass = 1f; // Ensure consistent mass
        rb.linearDamping = 0.5f; // Add some air resistance

        // Setup Collider2D
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        if (collider == null)
        {
            collider = gameObject.AddComponent<BoxCollider2D>();
        }
        
        // Adjust collider size to match sprite
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            collider.size = spriteRenderer.bounds.size;
        }
    }

    public void ResetPlayer()
    {
        Debug.Log("Resetting player state...");
        isDead = false;

        // Ensure components are set up
        SetupComponents();

        if (rb != null)
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.gravityScale = gravityScale;
            transform.rotation = Quaternion.identity;
            Debug.Log("Rigidbody2D reset complete.");
        }
        else
        {
            Debug.LogError("Rigidbody2D is still missing after setup!");
            return;
        }

        Debug.Log("Player reset complete. Active: " + gameObject.activeSelf);
    }

    private void Update()
    {
        // Don't process input if game is not active or player is dead
        if (isDead || !GameManager.Instance.isGameActive) 
        {
            // Make sure physics is disabled when not active
            if (rb != null && rb.simulated)
            {
                rb.simulated = false;
            }
            return;
        }

        // Handle input (both touch and mouse click for testing)
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            Jump();
        }

        // Smoother gravity adjustment
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = gravityScale * fallMultiplier;
        }
        else
        {
            rb.gravityScale = gravityScale;
        }

        // Even smoother forward speed control
        Vector2 velocity = rb.linearVelocity;
        velocity.x = Mathf.Lerp(velocity.x, forwardSpeed, Time.deltaTime * 3f);
        
        // Tighter control on vertical movement
        float maxFallSpeed = jumpVelocity * 0.8f; // Slower falling than jumping
        velocity.y = Mathf.Clamp(velocity.y, -maxFallSpeed, jumpVelocity);
        
        rb.linearVelocity = velocity;

        // Smoother rotation
        float normalizedVelocity = Mathf.Clamp(rb.linearVelocity.y / jumpVelocity, -1f, 1f);
        float targetAngle = normalizedVelocity * 15f; // Even smaller rotation angle
        transform.rotation = Quaternion.Lerp(
            transform.rotation, 
            Quaternion.Euler(0, 0, targetAngle), 
            Time.deltaTime * 10f
        );
    }

    private void Jump()
    {
        // More controlled jump with velocity adjustment
        Vector2 velocity = rb.linearVelocity;
        velocity.y = Mathf.Max(jumpVelocity, velocity.y); // Don't override if already moving up faster
        rb.linearVelocity = velocity;
        
        // Reset gravity scale when jumping
        rb.gravityScale = gravityScale;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Only check collisions if game is active
        if (!GameManager.Instance.isGameActive) return;

        Debug.Log($"Collision detected with: {collision.gameObject.name} with tag: {collision.gameObject.tag}");
        
        // Check for enemy collision
        if (collision.gameObject.CompareTag("Enemy") || collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Debug.Log("Hit enemy - calling Die()");
            Die();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Only check triggers if game is active
        if (!GameManager.Instance.isGameActive) return;

        Debug.Log($"Trigger detected with: {other.gameObject.name} with tag: {other.gameObject.tag}");
        
        // Check for enemy trigger
        if (other.CompareTag("Enemy") || other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Debug.Log("Hit enemy trigger - calling Die()");
            Die();
        }
    }

    public void Die()
    {
        if (isDead) return; // Only prevent multiple deaths
        
        Debug.Log("Player died!");
        isDead = true;
        
        if (rb != null)
        {
            rb.simulated = false; // Disable physics first
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameOver();
        }
    }
}
