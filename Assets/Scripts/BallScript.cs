using UnityEngine;

public class BallScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            // Call the player's death method
            HeroKnight playerHealth = collision.collider.GetComponent<HeroKnight>();
            if (playerHealth != null)
                playerHealth.Die();
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
