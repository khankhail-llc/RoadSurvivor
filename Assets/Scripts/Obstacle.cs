using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public float speed = 8f;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
        // Move downwards
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        // Deactivate if below screen
        if (transform.position.y < mainCamera.transform.position.y - mainCamera.orthographicSize - 1f)
        {
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.GameOver();
        }
    }
}
