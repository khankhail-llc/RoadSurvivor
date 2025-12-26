using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    [Header("UI Elements")]
    public TextMeshProUGUI coinText;

    private int coinCount = 0;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        UpdateCoinUI();
    }

    // Call this function whenever player collects a coin
    public void CollectCoin()
    {
        coinCount++;
        UpdateCoinUI();
    }

    private void UpdateCoinUI()
    {
        if (coinText != null)
            coinText.text = coinCount.ToString(); // Show 1, 2, 3… as coins are collected
    }

    // Reset coin count, e.g., on game restart
    public void ResetCoins()
    {
        coinCount = 0;
        UpdateCoinUI();
    }

    // Optional: hide/show UI (like when in pause menu)
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
