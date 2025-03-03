using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private void Start()
    {
        // Make sure the obstacle has the correct tag
        gameObject.tag = "Obstacle";
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HandleCollision(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        HandleCollision(other.gameObject);
    }

    private void HandleCollision(GameObject other)
    {
        Debug.Log($"Obstacle collision with: {other.name}, Tag: {other.tag}");
        
        // If the player hits the obstacle, they die
        if (other.CompareTag("Player"))
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null)
            {
                Debug.Log("Player hit obstacle - calling Die()");
                player.Die();
            }
            else
            {
                Debug.LogWarning("Player object found but no PlayerController component!");
            }
        }
    }
}
