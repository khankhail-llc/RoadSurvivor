// using UnityEngine;

// public class Coin : MonoBehaviour
// {
//     public float moveSpeed = 8f;

//     void Update()
//     {
//         // Move the coin downwards
//         transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);

//         // Disable coin if it goes off-screen
//         if (transform.position.y < -15f)
//             gameObject.SetActive(false);
//     }

//     void OnTriggerEnter2D(Collider2D other)
//     {
//         if (other.CompareTag("Player"))
//         {
//             // Increment coin count
//             CoinManager.Instance?.CollectCoin();

//             // Disable the coin after collection
//             gameObject.SetActive(false);
//         }
//     }
// }





using UnityEngine;

public class Coin : MonoBehaviour
{
    public float moveSpeed = 8f;
    public AudioClip collectSound;   // Drag your coin collect sound here

    void Update()
    {
        transform.Translate(Vector3.down * moveSpeed * Time.deltaTime);

        if (transform.position.y < -15f)
            gameObject.SetActive(false);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Increment coin count
            CoinManager.Instance?.CollectCoin();

            // Play sound at coin's position (independent of object)
            if (collectSound != null)
                AudioSource.PlayClipAtPoint(collectSound, transform.position);

            // Disable the coin immediately
            gameObject.SetActive(false);
        }
    }
}
