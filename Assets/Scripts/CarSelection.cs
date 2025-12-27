using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CarSelection : MonoBehaviour
{
    [Header("Car Display")]
    [SerializeField] private SpriteRenderer previewRenderer;  // Car preview
    [SerializeField] private Sprite[] carPreviews;            // Har car ka preview sprite (7 cars)

    [Header("Stats Display")]
    [SerializeField] private Image statsBarsImage;            // UI Image jo stats bars dikhaayega
    [SerializeField] private Sprite[] statsBarsSprites;       // Har car ke stats bars (7 cars)

    [Header("UI Elements - Lock/Unlock System")]
    [SerializeField] private GameObject lockIcon;             // Lock icon (jab car locked ho)
    [SerializeField] private GameObject watchAdButton;        // "Watch Ad to Unlock" button
    [SerializeField] private GameObject selectButton;         // "Select" button (jab car unlocked ho)

    private int currentIndex = 0;
    private const int TOTAL_CARS = 7;

    private void Start()
    {
        // Previously selected car load karo
        currentIndex = PlayerPrefs.GetInt("SelectedCar", 0);
        
        // Make sure currentIndex valid hai
        if (currentIndex < 0 || currentIndex >= TOTAL_CARS)
            currentIndex = 0;

        UpdateCarDisplay();
    }

    private void UpdateCarDisplay()
    {
        // Car preview change
        if (previewRenderer != null && currentIndex < carPreviews.Length)
            previewRenderer.sprite = carPreviews[currentIndex];

        // Stats bars image change
        if (statsBarsImage != null && currentIndex < statsBarsSprites.Length)
            statsBarsImage.sprite = statsBarsSprites[currentIndex];

        // Lock/Unlock UI update karo
        UpdateUIBasedOnLockState();
    }

    private void UpdateUIBasedOnLockState()
    {
        bool isUnlocked = CarUnlockManager.Instance.IsCarUnlocked(currentIndex);

        if (isUnlocked)
        {
            // Car unlocked hai
            if (lockIcon != null) lockIcon.SetActive(false);
            if (watchAdButton != null) watchAdButton.SetActive(false);
            if (selectButton != null) selectButton.SetActive(true);
        }
        else
        {
            // Car locked hai
            if (lockIcon != null) lockIcon.SetActive(true);
            if (watchAdButton != null) watchAdButton.SetActive(true);
            if (selectButton != null) selectButton.SetActive(false);
        }
    }

    public void NextCar()
    {
        currentIndex++;
        if (currentIndex >= TOTAL_CARS)
            currentIndex = 0;

        UpdateCarDisplay();
    }

    public void PreviousCar()
    {
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = TOTAL_CARS - 1;

        UpdateCarDisplay();
    }

    /// <summary>
    /// Jab user "Watch Ad to Unlock" button pe click kare
    /// </summary>
    public void OnWatchAdButtonClicked()
    {
        Debug.Log("Watch Ad button clicked for car: " + currentIndex);

        // Check if RealAdManager available hai
        if (RealAdManager.Instance == null)
        {
            Debug.LogError("RealAdManager not found! Make sure RealAdManager is in the scene.");
            return;
        }

        // Check if ad ready hai
        if (!RealAdManager.Instance.IsAdReady())
        {
            Debug.LogWarning("Ad is not ready yet! Please wait...");
            // Optional: User ko message dikha sakte ho "Please wait, loading ad..."
            return;
        }

        // Rewarded ad dikhao
        RealAdManager.Instance.ShowRewardedAd(() => 
        {
            // Ad complete ho gayi, ab car unlock karo
            OnAdCompleted();
        });
    }

    /// <summary>
    /// Jab ad successfully complete ho jaye
    /// </summary>
    private void OnAdCompleted()
    {
        Debug.Log("Ad completed! Unlocking car: " + currentIndex);
        
        // Car unlock karo
        CarUnlockManager.Instance.UnlockCar(currentIndex);
        
        // UI update karo
        UpdateUIBasedOnLockState();
    }

    /// <summary>
    /// Jab user "Select" button pe click kare (unlocked car select karne ke liye)
    /// </summary>
    public void OnSelectButtonClicked()
    {
        // Check karo car unlocked hai ya nahi
        if (!CarUnlockManager.Instance.IsCarUnlocked(currentIndex))
        {
            Debug.LogWarning("Cannot select locked car!");
            return;
        }

        // Selected car save karo
        PlayerPrefs.SetInt("SelectedCar", currentIndex);
        PlayerPrefs.Save();
        
        Debug.Log("Car selected: " + currentIndex);
        
        // GamePlay scene load karo
        SceneManager.LoadScene("GamePlay");
    }

    // ===== DEVELOPMENT HELPER METHODS =====
    // Ye methods development/testing ke liye hain

    /// <summary>
    /// Development: Sab cars unlock kar do
    /// </summary>
    public void DevUnlockAllCars()
    {
        CarUnlockManager.Instance.UnlockAllCars(TOTAL_CARS);
        UpdateUIBasedOnLockState();
    }

    /// <summary>
    /// Development: Sab cars reset kar do (first car ke alawa)
    /// </summary>
    public void DevResetAllCars()
    {
        CarUnlockManager.Instance.ResetAllCars(TOTAL_CARS);
        UpdateUIBasedOnLockState();
    }
}
