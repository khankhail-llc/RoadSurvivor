using UnityEngine;

public class FuelCan : MonoBehaviour
{
    public float moveSpeed = 8f;

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
            FuelManager.Instance?.CollectFuel();
            gameObject.SetActive(false);
        }
    }
}
