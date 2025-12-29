using UnityEngine;
using UnityEngine.UI;

public class CoinAdButton : MonoBehaviour
{
    [Header("Settings")]
    public int rewardAmount = 50;
    
    private Button button;

    void Start()
    {
        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(OnWatchAdClicked);
        }
    }

    void OnWatchAdClicked()
    {
        if (RealAdManager.Instance != null && RealAdManager.Instance.IsAdReady())
        {
            RealAdManager.Instance.ShowRewardedAd(OnAdRewarded);
        }
        else
        {
            Debug.Log("Ad Not Ready or Manager Missing");
            // Optional: Show "Ad Loading..." or "No Ad Available" toast
        }
    }

    void OnAdRewarded()
    {
        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.AddCoins(rewardAmount);
            Debug.Log($"Granted {rewardAmount} Coins for watching Ad!");
        }
    }
}
