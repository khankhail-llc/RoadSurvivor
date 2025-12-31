using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class CarSelection : MonoBehaviour
{
    [Header("Car Display")]
    [SerializeField] private SpriteRenderer previewRenderer;
    [SerializeField] private Sprite[] carPreviews;

    [Header("Stats Display")]
    [SerializeField] private Image statsBarsImage;
    [SerializeField] private Sprite[] statsBarsSprites;

    [Header("UI Elements - Lock/Unlock System")]
    [SerializeField] private GameObject lockIcon;
    [SerializeField] private GameObject watchAdButton;
    [SerializeField] private GameObject selectButton;

    [Header("Buying System")]
    [SerializeField] private Button buyButton;
    [SerializeField] private TextMeshProUGUI priceText;
    [SerializeField] private GameObject[] carPriceLabels;
    [SerializeField] private int[] carPrices;

    [Header("Ad Logic")]
    [SerializeField] private TextMeshProUGUI adProgressText; // 👈 Assign this in Inspector
    private const int ADS_TO_UNLOCK = 4;

    private int currentIndex = 0;
    private int totalCars = 20; // Default fallback

    private void Start()
    {
        // 🔥 Dynamic Total Cars Calculation
        if (carPreviews != null && carPreviews.Length > 0)
        {
            totalCars = carPreviews.Length;
        }

        // Initialize Prices if missing
        if (carPrices == null || carPrices.Length < totalCars)
        {
            // Providing default prices for up to 18 cars as per user requirement
            int[] defaultPrices = new int[] { 0, 30, 500, 1000, 1500, 2000, 2500, 3000, 3500, 4000, 4500, 5000, 5500, 6000, 6500, 7000, 7500, 8000,8500,9000 };
            
            // If carPrices is null, just use default
            if (carPrices == null)
            {
                carPrices = defaultPrices;
            }
            // If carPrices is too small, resize it? Or just warn?
            // For now, let's keep the user's manual assignment priority, but ensure we don't crash.
            // If the user assigned fewer prices than cars, we might have issues. 
            // Let's rely on bounds checking in accessors.
        }

        if (buyButton != null)
            buyButton.onClick.AddListener(BuyCurrentCar);

        currentIndex = PlayerPrefs.GetInt("SelectedCar", 0);
        currentIndex = Mathf.Clamp(currentIndex, 0, totalCars - 1);

        UpdateCarDisplay(); // 🔥 FULL UI INIT
    }

    // ================= DISPLAY =================
    private void UpdateCarDisplay()
    {
        // Safety check for indices
        if (currentIndex < 0 || currentIndex >= totalCars) 
        {
            currentIndex = 0; // Reset if out of bounds
        }

        if (previewRenderer != null && carPreviews != null && currentIndex < carPreviews.Length)
            previewRenderer.sprite = carPreviews[currentIndex];

        if (statsBarsImage != null && statsBarsSprites != null && currentIndex < statsBarsSprites.Length)
            statsBarsImage.sprite = statsBarsSprites[currentIndex];

        // Price labels
        if (carPriceLabels != null)
        {
            for (int i = 0; i < carPriceLabels.Length; i++)
            {
                if (carPriceLabels[i] != null)
                    carPriceLabels[i].SetActive(i == currentIndex && 
                        !CarUnlockManager.Instance.IsCarUnlocked(currentIndex));
            }
        }

        UpdateUIBasedOnLockState();
    }

    // ================= UI STATE =================
    private void UpdateUIBasedOnLockState()
    {
        bool isUnlocked = CarUnlockManager.Instance.IsCarUnlocked(currentIndex);
        
        // Bounds check for prices
        int price = 0;
        if (carPrices != null && currentIndex < carPrices.Length)
        {
            price = carPrices[currentIndex];
        }

        int coins = CoinManager.Instance.GetTotalCoins();

        if (isUnlocked)
        {
            lockIcon.SetActive(false);
            watchAdButton.SetActive(false);
            buyButton.gameObject.SetActive(false);
            selectButton.SetActive(true);
            
            if(adProgressText != null) adProgressText.gameObject.SetActive(false); // Hide text if unlocked
            return;
        }

        // Locked state
        lockIcon.SetActive(true);
        selectButton.SetActive(false);
        watchAdButton.SetActive(true);

        // Show Ad Progress
        UpdateAdProgressText(); // Update the text content

        if (coins >= price)
        {
            buyButton.gameObject.SetActive(true);
            if (priceText != null)
                priceText.text = price.ToString();
        }
        else
        {
            buyButton.gameObject.SetActive(false);
        }
    }

    private void UpdateAdProgressText()
    {
        if (adProgressText != null)
        {
            adProgressText.gameObject.SetActive(true);
            int watched = GetAdsWatched(currentIndex);
            // Display: 3/4 (User wants to see how many left or watched)
            // Example: "Ads: 1/4"
            adProgressText.text = $"{watched}/{ADS_TO_UNLOCK}";
        }
    }

    // ================= BUY =================
    public void BuyCurrentCar()
    {
        // Safety check
        if (carPrices == null || currentIndex >= carPrices.Length) return;

        int price = carPrices[currentIndex];

        if (CoinManager.Instance.SpendCoins(price))
        {
            CarUnlockManager.Instance.UnlockCar(currentIndex);
            ClickSound.Instance?.PlayClick();

            // 🔥 MOST IMPORTANT LINE
            UpdateCarDisplay();   // FULL refresh SAME FRAME

            Debug.Log("Car Purchased: " + currentIndex);
        }
        else
        {
            Debug.Log("Not enough coins");
        }
    }

    // ================= NAVIGATION =================
    public void NextCar()
    {
        currentIndex = (currentIndex + 1) % totalCars; // Use totalCars
        UpdateCarDisplay();
    }

    public void PreviousCar()
    {
        currentIndex--;
        if (currentIndex < 0) currentIndex = totalCars - 1; // Use totalCars
        UpdateCarDisplay();
    }

    // ================= ADS =================
    public void OnWatchAdButtonClicked()
    {
        if (!RealAdManager.Instance.IsAdReady())
            return;

        RealAdManager.Instance.ShowRewardedAd(OnAdCompleted);
    }

    private void OnAdCompleted()
    {
        // Increment ad count
        int watched = GetAdsWatched(currentIndex);
        watched++;
        SetAdsWatched(currentIndex, watched);

        if (watched >= ADS_TO_UNLOCK)
        {
            // Unlock only if target reached
            CarUnlockManager.Instance.UnlockCar(currentIndex);
        }
        
        UpdateCarDisplay(); // Update UI (Text will update or Disappear if unlocked)
    }

    // ================= DATA HELPERS =================
    private int GetAdsWatched(int carIndex)
    {
        return PlayerPrefs.GetInt("AdsWatched_" + carIndex, 0);
    }

    private void SetAdsWatched(int carIndex, int count)
    {
        PlayerPrefs.SetInt("AdsWatched_" + carIndex, count);
        PlayerPrefs.Save();
    }

    // ================= SELECT =================
    public void OnSelectButtonClicked()
    {
        if (!CarUnlockManager.Instance.IsCarUnlocked(currentIndex))
            return;

        PlayerPrefs.SetInt("SelectedCar", currentIndex);
        PlayerPrefs.Save();

        SceneManager.LoadScene("GamePlay");
    }

    // ================= DEV =================
    public void DevUnlockAllCars()
    {
        CarUnlockManager.Instance.UnlockAllCars(totalCars);
        UpdateCarDisplay();
    }

    public void DevResetAllCars()
    {
        CarUnlockManager.Instance.ResetAllCars(totalCars);
        UpdateCarDisplay();
    }
}
