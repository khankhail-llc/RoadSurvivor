
using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    [Header("UI Elements")]
    public TextMeshProUGUI coinText;       // Current run coins
    public TextMeshProUGUI totalCoinText;  // Total saved coins

    private int coinCount = 0;   // 👈 RESET on restart
    private int totalCoins = 0;  // 👈 NEVER reset

    private const string TOTAL_COIN_KEY = "TOTAL_COINS";

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        // 👉 Load total coins only
        totalCoins = PlayerPrefs.GetInt(TOTAL_COIN_KEY, 0);

        // 👉 Reset current coins every time game starts
        coinCount = 0;

        UpdateCoinUI();
    }

    public void CollectCoin()
    {
        coinCount++;      // current run
        totalCoins++;     // lifetime

        // 👉 Save ONLY total coins
        PlayerPrefs.SetInt(TOTAL_COIN_KEY, totalCoins);
        PlayerPrefs.Save();

        UpdateCoinUI();
    }

    private void UpdateCoinUI()
    {
        if (coinText != null)
            coinText.text = coinCount.ToString(); // reset on restart

        if (totalCoinText != null)
            totalCoinText.text = totalCoins.ToString(); // never reset
    }

    // 👉 Get current run coins (for game over panel)
    public int GetCurrentCoins()
    {
        return coinCount;
    }

    // 👉 Get Total Saved Coins
    public int GetTotalCoins()
    {
        return totalCoins;
    }

    // 👉 Spend Coins (Returns true if successful)
    public bool SpendCoins(int amount)
    {
        if (totalCoins >= amount)
        {
            totalCoins -= amount;
            PlayerPrefs.SetInt(TOTAL_COIN_KEY, totalCoins);
            PlayerPrefs.Save();
            UpdateCoinUI();
            return true;
        }
        return false;
    }

    // 👉 Call this on Game Over Restart Button
    public void ResetCoins()
    {
        coinCount = 0;
        UpdateCoinUI();
    }

    public void HideCoinUI()
    {
        if (coinText != null)
            coinText.gameObject.SetActive(false);
    }

    public void ShowCoinUI()
    {
        if (coinText != null)
            coinText.gameObject.SetActive(true);
    }
}
