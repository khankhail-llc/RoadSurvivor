// using UnityEngine;
// using UnityEngine.UI;
// using UnityEngine.SceneManagement;
// using TMPro;

// public class CarSelection : MonoBehaviour
// {
//     [Header("Car Display")] 
//     [SerializeField] private SpriteRenderer previewRenderer;  // Car preview
//     [SerializeField] private Sprite[] carPreviews;            // Har car ka preview sprite (7 cars)

//     [Header("Stats Display")]
//     [SerializeField] private Image statsBarsImage;            // UI Image jo stats bars dikhaayega
//     [SerializeField] private Sprite[] statsBarsSprites;       // Har car ke stats bars (7 cars)

//     [Header("UI Elements - Lock/Unlock System")]
//     [SerializeField] private GameObject lockIcon;             // Lock icon (jab car locked ho)
//     [SerializeField] private GameObject watchAdButton;        // "Watch Ad to Unlock" button
//     [SerializeField] private GameObject selectButton;         // "Select" button (jab car unlocked ho)
    
//     [Header("Buying System")]
//     [SerializeField] private Button buyButton;                // "Buy" button (coins se khareedne ke liye)
//     [SerializeField] private TextMeshProUGUI priceText;       // Optional: Single price text on button
//     [SerializeField] private GameObject[] carPriceLabels;     // ✅ USER REQUEST: Separate price objects for each car
//     [SerializeField] private int[] carPrices;                 // Har car ki price (Inspector mai set karo)

//     private int currentIndex = 0;
//     private const int TOTAL_CARS = 7;

//     private void Start()
//     {
//         // Default prices if not set individually
//         if (carPrices == null || carPrices.Length != TOTAL_CARS)
//         {
//             carPrices = new int[] { 0, 30 , 500 , 1000, 1500, 2000, 2500, 3000 };
//         }

//         if (buyButton != null) 
//             buyButton.onClick.AddListener(BuyCurrentCar);

//         // Previously selected car load karo
//         currentIndex = PlayerPrefs.GetInt("SelectedCar", 0);
        
//         // Make sure currentIndex valid hai
//         if (currentIndex < 0 || currentIndex >= TOTAL_CARS)
//             currentIndex = 0;

//         UpdateCarDisplay();
//     }

//     private void UpdateCarDisplay()
//     {
//         // Car preview change
//         if (previewRenderer != null && currentIndex < carPreviews.Length)
//             previewRenderer.sprite = carPreviews[currentIndex];

//         // Stats bars image change
//         if (statsBarsImage != null && currentIndex < statsBarsSprites.Length)
//             statsBarsImage.sprite = statsBarsSprites[currentIndex];

//         // Price Labels update (Toggle correct price label)
//         if (carPriceLabels != null)
//         {
//             for (int i = 0; i < carPriceLabels.Length; i++)
//             {
//                 if (carPriceLabels[i] != null)
//                     carPriceLabels[i].SetActive(i == currentIndex);
//             }
//         }

//         // Lock/Unlock UI update karo
//         UpdateUIBasedOnLockState();
//     }

//     private void UpdateUIBasedOnLockState()
//     {
//         bool isUnlocked = CarUnlockManager.Instance.IsCarUnlocked(currentIndex);
//         int price = (currentIndex < carPrices.Length) ? carPrices[currentIndex] : 999999;
//         int currentCoins = (CoinManager.Instance != null) ? CoinManager.Instance.GetTotalCoins() : 0;

//         // Hide all price labels if unlocked (Optional: depends if user wants to see price of owned cars)
//         // User didn't specify, but usually we hide price if owned.
//         // Let's keep them active in UpdateCarDisplay so user knows value, OR hide them here if unlocked.
//         // User said "jis sy user ko pta chaly a y car kitny ki hai" (so they know how much it IS).
//         // I will hide price labels if unlocked to be clean, or keep them if they are just info.
//         // Assuming Hide for now to avoid clutter, can change if requested.
//         if (isUnlocked && carPriceLabels != null && currentIndex < carPriceLabels.Length && carPriceLabels[currentIndex] != null)
//         {
//             carPriceLabels[currentIndex].SetActive(false);
//         }

//         if (isUnlocked)
//         {
//             // Car unlocked hai
//             if (lockIcon != null) lockIcon.SetActive(false);
//             if (watchAdButton != null) watchAdButton.SetActive(false);
//             if (buyButton != null) buyButton.gameObject.SetActive(false); 
//             if (selectButton != null) selectButton.SetActive(true);
//         }
//         else
//         {
//             // Car locked hai
//             if (lockIcon != null) lockIcon.SetActive(true);
//             if (selectButton != null) selectButton.SetActive(false);

//             // ✅ USER REQUEST: Watch Ad button ALWAYS visible if locked
//             if (watchAdButton != null) watchAdButton.SetActive(true);

//             // Buy Button Logic
//             if (buyButton != null)
//             {
//                 // Agar coins enough hain, toh Buy button dikhao
//                 if (currentCoins >= price)
//                 {
//                     buyButton.gameObject.SetActive(true);
//                     if (priceText != null) priceText.text = price.ToString();
                    
//                     // Note: Ad button is ALREADY active above
//                 }
//                 else
//                 {
//                     // Coins kam hain -> Buy button HIDE
//                     buyButton.gameObject.SetActive(false);
//                 }
//             }
//         }
//     }

//     public void BuyCurrentCar()
//     {
//          int price = (currentIndex < carPrices.Length) ? carPrices[currentIndex] : 999999;

//          if (CoinManager.Instance.SpendCoins(price))
//          {
//              CarUnlockManager.Instance.UnlockCar(currentIndex);
//              ClickSound.Instance?.PlayClick(); // Optional click sound
//              UpdateUIBasedOnLockState();
//              Debug.Log("Car Purchased: " + currentIndex);
//          }
//          else
//          {
//              Debug.Log("Not enough coins!");
//          }
//     }

//     public void NextCar()
//     {
//         currentIndex++;
//         if (currentIndex >= TOTAL_CARS)
//             currentIndex = 0;

//         UpdateCarDisplay();
//     }

//     public void PreviousCar()
//     {
//         currentIndex--;
//         if (currentIndex < 0)
//             currentIndex = TOTAL_CARS - 1;

//         UpdateCarDisplay();
//     }

//     /// <summary>
//     /// Jab user "Watch Ad to Unlock" button pe click kare
//     /// </summary>
//     public void OnWatchAdButtonClicked()
//     {
//         Debug.Log("Watch Ad button clicked for car: " + currentIndex);

//         // Check if RealAdManager available hai
//         if (RealAdManager.Instance == null)
//         {
//             Debug.LogError("RealAdManager not found! Make sure RealAdManager is in the scene.");
//             return;
//         }

//         // Check if ad ready hai
//         if (!RealAdManager.Instance.IsAdReady())
//         {
//             Debug.LogWarning("Ad is not ready yet! Please wait...");
//             // Optional: User ko message dikha sakte ho "Please wait, loading ad..."
//             return;
//         }

//         // Rewarded ad dikhao
//         RealAdManager.Instance.ShowRewardedAd(() => 
//         {
//             // Ad complete ho gayi, ab car unlock karo
//             OnAdCompleted();
//         });
//     }

//     /// <summary>
//     /// Jab ad successfully complete ho jaye
//     /// </summary>
//     private void OnAdCompleted()
//     {
//         Debug.Log("Ad completed! Unlocking car: " + currentIndex);
        
//         // Car unlock karo
//         CarUnlockManager.Instance.UnlockCar(currentIndex);
        
//         // UI update karo
//         UpdateUIBasedOnLockState();
//     }

//     /// <summary>
//     /// Jab user "Select" button pe click kare (unlocked car select karne ke liye)
//     /// </summary>
//     public void OnSelectButtonClicked()
//     {
//         // Check karo car unlocked hai ya nahi
//         if (!CarUnlockManager.Instance.IsCarUnlocked(currentIndex))
//         {
//             Debug.LogWarning("Cannot select locked car!");
//             return;
//         }

//         // Selected car save karo
//         PlayerPrefs.SetInt("SelectedCar", currentIndex);
//         PlayerPrefs.Save();
        
//         Debug.Log("Car selected: " + currentIndex);
        
//         // GamePlay scene load karo
//         SceneManager.LoadScene("GamePlay");
//     }

//     // ===== DEVELOPMENT HELPER METHODS =====
//     // Ye methods development/testing ke liye hain

//     /// <summary>
//     /// Development: Sab cars unlock kar do
//     /// </summary>
//     public void DevUnlockAllCars()
//     {
//         CarUnlockManager.Instance.UnlockAllCars(TOTAL_CARS);
//         UpdateUIBasedOnLockState();
//     }

//     /// <summary>
//     /// Development: Sab cars reset kar do (first car ke alawa)
//     /// </summary>
//     public void DevResetAllCars()
//     {
//         CarUnlockManager.Instance.ResetAllCars(TOTAL_CARS);
//         UpdateUIBasedOnLockState();
//     }
// }




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

    private int currentIndex = 0;
    private const int TOTAL_CARS = 7;

    private void Start()
    {
        if (carPrices == null || carPrices.Length != TOTAL_CARS)
        {
            carPrices = new int[] { 0, 30, 500, 1000, 1500, 2000, 2500 };
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
            return;
        }

        // Locked state
        lockIcon.SetActive(true);
        selectButton.SetActive(false);
        watchAdButton.SetActive(true);

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
        CarUnlockManager.Instance.UnlockCar(currentIndex);
        UpdateCarDisplay(); // 🔥 same fix for ads
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
