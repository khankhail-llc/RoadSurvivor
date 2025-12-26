using UnityEngine;

public class DestroyBehind : MonoBehaviour
{
    public float destroyY = -20f;

    void Update()
    {
        if (transform.position.y < destroyY)
            gameObject.SetActive(false);
    }
}
