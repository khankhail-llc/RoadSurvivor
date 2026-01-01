using UnityEngine;
using UnityEngine.UI;

public class CoinAdButton : MonoBehaviour
{
    [Header("Settings")]
    public int rewardAmount = 50;
    
    [Header("UI Panels")]
    public GameObject coinAdPanel;
    public Button watchAdButton;
    public Button closePanelButton;

    private Button mainButton;

    void Start()
    {
        // Setup Main Button (Trigger)
        mainButton = GetComponent<Button>();
        if (mainButton != null)
        {
            mainButton.onClick.AddListener(() => {
                if (ClickSound.Instance) ClickSound.Instance.PlayClick();
                OpenPanel();
            });
        }

        // Setup Panel Buttons
        if (watchAdButton != null)
        {
            watchAdButton.onClick.AddListener(() => {
                if (ClickSound.Instance) ClickSound.Instance.PlayClick();
                OnWatchAdClicked();
            });
        }

        if (closePanelButton != null)
        {
            closePanelButton.onClick.AddListener(() => {
                if (ClickSound.Instance) ClickSound.Instance.PlayClick();
                ClosePanel();
            });
        }

        // Ensure panel is closed at start
        if (coinAdPanel != null) coinAdPanel.SetActive(false);
    }

    public void OpenPanel()
    {
        if (coinAdPanel != null) coinAdPanel.SetActive(true);
    }

    public void ClosePanel()
    {
        if (coinAdPanel != null) coinAdPanel.SetActive(false);
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
        
        ClosePanel();
    }
}
