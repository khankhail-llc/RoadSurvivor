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

        if (watchAdButton != null)
        {
            // 🛠️ AUTO-FIX: Disable Raycast Target on Text inside this button
            var texts = watchAdButton.GetComponentsInChildren<Text>(true);
            foreach (var t in texts) t.raycastTarget = false;
            
            var tmps = watchAdButton.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true);
            foreach (var tmp in tmps) tmp.raycastTarget = false;
            
            Debug.Log("[CoinAdButton] Auto-fixed Watch Ad Button raycast targets!");

            watchAdButton.onClick.AddListener(() => {
                Debug.Log("[CoinAdButton] Watch Ad Button Clicked via Listener");
                if (ClickSound.Instance) ClickSound.Instance.PlayClick();
                OnWatchAdClicked();
            });
        }
        else
        {
            Debug.LogError("[CoinAdButton] 'Watch Ad Button' is NOT assigned in the Inspector!");
        }

        if (closePanelButton != null)
        {
            // 🛠️ AUTO-FIX: Disable Raycast Target on Text inside local button
            var texts = closePanelButton.GetComponentsInChildren<Text>(true);
            foreach (var t in texts) t.raycastTarget = false;

            var tmps = closePanelButton.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true);
            foreach (var tmp in tmps) tmp.raycastTarget = false;

            closePanelButton.onClick.AddListener(() => {
                if (ClickSound.Instance) ClickSound.Instance.PlayClick();
                ClosePanel();
            });
        }
        else
        {
            Debug.LogWarning("[CoinAdButton] 'Close Panel Button' is NOT assigned in the Inspector.");
        }

        // Ensure panel is closed at start
        if (coinAdPanel != null) 
        {
            coinAdPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("[CoinAdButton] 'Coin Ad Panel' is NOT assigned in the Inspector!");
        }
    }

    public void OpenPanel()
    {
        Debug.Log("[CoinAdButton] Opening Ad Panel");
        if (coinAdPanel != null) coinAdPanel.SetActive(true);
    }

    public void ClosePanel()
    {
        Debug.Log("[CoinAdButton] Closing Ad Panel");
        if (coinAdPanel != null) coinAdPanel.SetActive(false);
    }

    void OnWatchAdClicked()
    {
        Debug.Log("[CoinAdButton] Attempting to show ad...");

        if (RealAdManager.Instance == null)
        {
            Debug.LogError("[CoinAdButton] RealAdManager Instance is NULL! Make sure 'RealAdManager' prefab is in the scene.");
            return;
        }

        if (RealAdManager.Instance.IsAdReady())
        {
            Debug.Log("[CoinAdButton] Ad is ready. Showing now.");
            RealAdManager.Instance.ShowRewardedAd(OnAdRewarded);
        }
        else
        {
            Debug.LogWarning("[CoinAdButton] Ad is NOT ready yet.");
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
