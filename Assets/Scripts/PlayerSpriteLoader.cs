using UnityEngine;

public class PlayerSpriteLoader : MonoBehaviour
{
    [SerializeField] SpriteRenderer playerRenderer;
    [SerializeField] Sprite[] carSprites;

    void Start()
    {
        int index = PlayerPrefs.GetInt("SelectedCar", 0);

        if (index < 0 || index >= carSprites.Length)
            index = 0;

        playerRenderer.sprite = carSprites[index];
    }
}