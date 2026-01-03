using UnityEngine;

public class CoinCollectible : MonoBehaviour
{
    public int coinValue = 1; // How many coins this gives
    public AudioClip collectSound; // optional sound

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the player touched the coin
        if (other.CompareTag("Player"))
        {
            // Add to player's coin count
            PlayerStats.Instance.AddCoins(coinValue);

            // Play sound if assigned
            if (collectSound != null)
                AudioSource.PlayClipAtPoint(collectSound, transform.position);

            // Destroy the coin
            Destroy(gameObject);
        }
    }

void Update()
{
    // transform.Rotate(0, 0, 180 * Time.deltaTime); // spins 180° per second
    transform.position = new Vector3(
        transform.position.x,
        transform.position.y + Mathf.Sin(Time.time * 2f) * 0.002f,
        transform.position.z
    );
}
}
