using UnityEngine;
using UnityEngine.UI;  // Image ke liye
using UnityEngine.SceneManagement;

public class CarSelection : MonoBehaviour
{
    [SerializeField] private SpriteRenderer previewRenderer;  // Car preview (jaise pehle)
    [SerializeField] private Sprite[] carPreviews;            // Har car ka preview sprite

    [SerializeField] private Image statsBarsImage;            // Yeh UI Image jo stats bars dikhaayega
    [SerializeField] private Sprite[] statsBarsSprites;       // Har car ke stats bars wali image yahan daalo

    private int currentIndex = 0;

    private void Start()
    {
        currentIndex = PlayerPrefs.GetInt("SelectedCar", 0);
        UpdateCarDisplay();
    }

    private void UpdateCarDisplay()
    {
        // Car preview change
        if (previewRenderer != null && currentIndex < carPreviews.Length)
            previewRenderer.sprite = carPreviews[currentIndex];

        // Stats bars image change (yeh har car ke unique stats dikhaayega)
        if (statsBarsImage != null && currentIndex < statsBarsSprites.Length)
            statsBarsImage.sprite = statsBarsSprites[currentIndex];
    }

    public void NextCar()
    {
        currentIndex++;
        if (currentIndex >= carPreviews.Length)
            currentIndex = 0;

        UpdateCarDisplay();
    }

    public void PreviousCar()
    {
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = carPreviews.Length - 1;

        UpdateCarDisplay();
    }

    public void PlayButton()
    {
        PlayerPrefs.SetInt("SelectedCar", currentIndex);
        PlayerPrefs.Save();
        SceneManager.LoadScene("GamePlay");
    }
}

