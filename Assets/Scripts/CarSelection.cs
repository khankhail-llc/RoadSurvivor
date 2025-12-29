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
    private const int TOTAL_CARS = 10;

    private void Start()
    {
        if (carPrices == null || carPrices.Length != TOTAL_CARS)
        {
            carPrices = new int[] { 0, 30, 500, 1000, 1500, 2000, 2500,3000,3500 };
        }

        if (buyButton != null)
            buyButton.onClick.AddListener(BuyCurrentCar);

        currentIndex = PlayerPrefs.GetInt("SelectedCar", 0);
        currentIndex = Mathf.Clamp(currentIndex, 0, TOTAL_CARS - 1);

        UpdateCarDisplay(); // 🔥 FULL UI INIT
    }

    // ================= DISPLAY =================
    private void UpdateCarDisplay()
    {
        if (previewRenderer != null)
            previewRenderer.sprite = carPreviews[currentIndex];

        if (statsBarsImage != null)
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
        int price = carPrices[currentIndex];
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
        currentIndex = (currentIndex + 1) % TOTAL_CARS;
        UpdateCarDisplay();
    }

    public void PreviousCar()
    {
        currentIndex--;
        if (currentIndex < 0) currentIndex = TOTAL_CARS - 1;
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
        CarUnlockManager.Instance.UnlockAllCars(TOTAL_CARS);
        UpdateCarDisplay();
    }

    public void DevResetAllCars()
    {
        CarUnlockManager.Instance.ResetAllCars(TOTAL_CARS);
        UpdateCarDisplay();
    }
}
