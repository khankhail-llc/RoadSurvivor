using UnityEngine;

public class BoostPickup : MonoBehaviour
{
    public AudioClip shieldCollectSound;  // Assign your shield collect sound in Inspector

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController player = other.GetComponent<PlayerController>();
        if (player == null) return;

        // Activate shield on player
        player.ActivateShield();

        // Play shield collect sound at pickup position
        if (shieldCollectSound != null)
            AudioSource.PlayClipAtPoint(shieldCollectSound, transform.position);

        // Disable pickup safely
        gameObject.SetActive(false);
    }
}
