using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using GoogleMobileAds.Api;
using System;
using System.Collections;
using TMPro;

public class RewardedAdController : MonoBehaviour
{
    public static RewardedAdController Instance { get; private set; }

    private RewardedAd rewardedAd;
    private bool isLoading = false;
    private float previousTimeScale = 1f;

    [Header("Ad Unit IDs")]
    [SerializeField] private string androidAdUnitId = "ca-app-pub-3025488325095617/2892239297";
    [SerializeField] private string iosAdUnitId = "ca-app-pub-3025488325095617/2675674920";

    [Header("UI")]
    public Button watchAdButton;

    private void Awake()
    {
        // SINGLETON PATTERN + BUTTON TRANSFER
        if (Instance != null && Instance != this)
        {
            // If this new instance has a button directly wired in Inspector, pass it to the Singleton instance
            if (watchAdButton != null)
            {
                Instance.watchAdButton = watchAdButton;
                Instance.ReconnectUI();
            }
            else
            {
                // Otherwise tell existing instance to search again
                Instance.ReconnectUI();
            }

            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;

        LoadRewardedAd();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        rewardedAd?.Destroy();
    }

    private void Start()
    {
        ReconnectUI();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Small delay to allow UI to Initialize
        Invoke(nameof(ReconnectUI), 0.2f);
    }

    // ================= LOAD AD =================
    public void LoadRewardedAd()
    {
        if (isLoading || (rewardedAd != null && rewardedAd.CanShowAd()))
            return;

        isLoading = true;
        Debug.Log("[RewardedAd] Loading new ad...");

        rewardedAd?.Destroy();
        rewardedAd = null;

        var request = new AdRequest();
        RewardedAd.Load(GetAdUnitId(), request, AdLoadCallback);
    }

    private void AdLoadCallback(RewardedAd ad, LoadAdError error)
    {
        isLoading = false;

        if (error != null || ad == null)
        {
            Debug.LogError("[RewardedAd] Load failed: " + error?.GetMessage() + ". Retrying in 10s...");
            Invoke(nameof(LoadRewardedAd), 10f); // Retry after 10 seconds
            return;
        }

        rewardedAd = ad;
        RegisterAdEvents();
        Debug.Log("[RewardedAd] Loaded Successfully.");
    }

    private void RegisterAdEvents()
    {
        rewardedAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("[RewardedAd] Opened");
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            MusicManager.Instance?.PauseMusic();
        };

        rewardedAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("[RewardedAd] Closed");

            // Only restore timescale if we are NOT in the middle of reviving logic
            // (Reviving might keep timescale 0 or handle it distinctly)
            if (GameManager.Instance == null || !GameManager.Instance.isReviving)
            {
                Time.timeScale = previousTimeScale;
            }

            MusicManager.Instance?.ResumeMusicAfterAd();
            
            // Queue load for next time
            LoadRewardedAd();
        };

        rewardedAd.OnAdFullScreenContentFailed += (AdError err) =>
        {
            Debug.LogError("[RewardedAd] Failed to show: " + err.GetMessage());
            Time.timeScale = previousTimeScale; // Restore immediately
            MusicManager.Instance?.ResumeMusicAfterAd();
            
            LoadRewardedAd();
        };
    }

    // ================= SHOW AD LOGIC =================
    public void ShowRewardedAd(Action onRewardEarned, Action onAdClosed = null)
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            // Update pending close action if provided
            // Note: In this simple implementation we rely on the generic 'OnAdClosed' event listener
            // which just resumes everything. If we need specific callbacks per ad-show, we'd store it here.
            
            // For now, onAdClosed is invoked if ad is NOT ready, or if we want extra logic.
            
            rewardedAd.Show((Reward reward) =>
            {
                Debug.Log("[RewardedAd] User earned reward.");
                onRewardEarned?.Invoke();
            });
        }
        else
        {
            Debug.LogWarning("[RewardedAd] Ad is NOT ready.");
            onAdClosed?.Invoke(); 
            
            // Trigger load again just in case
            LoadRewardedAd();
        }
    }

    // ================= BUTTON PRESS HANDLER =================
    
    // This method is called by the UI Button (Revive / Watch Ad)
    public void OnWatchAdButtonPressed()
    {
        Debug.Log("[RewardedAdController] 'Watch Ad' Button Pressed.");

        ShowRewardedAd(
            () =>
            {
                // This runs ONLY if the user completes the ad
                Debug.Log("[RewardedAdController] Starting Revive Process...");
                StartCoroutine(DelayedRevive());
            },
            () => 
            {
               Debug.Log("[RewardedAdController] Ad not ready or closed without reward (via button).");
            }
        );
    }

    // Kept for compatibility if assigned in older scenes
    public void OnReviveByAdButtonPressed()
    {
        OnWatchAdButtonPressed();
    }

    // ================= REVIVE ACTIONS =================
    private IEnumerator DelayedRevive()
    {
        // Wait a tiny bit (realtime) so the Ad close animation finishes cleanly
        yield return new WaitForSecondsRealtime(0.2f);

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RevivePlayer();
            
            // Activate Shield if player exists
            var player = FindFirstObjectByType<PlayerController>(); 
            if (player == null) player = FindObjectOfType<PlayerController>();
            
            player?.ActivateReviveShield();
        }
        else
        {
            Debug.LogError("[RewardedAdController] Cannot revive: GameManager Instance is NULL.");
            Time.timeScale = 1f; // Force unpause if something is broken
        }
    }

    // ================= HELPER =================
    private string GetAdUnitId()
    {
#if UNITY_ANDROID
        return androidAdUnitId;
#elif UNITY_IOS
        return iosAdUnitId;
#else
        return "unexpected_platform";
#endif
    }

    public bool IsRewardedAdReady() => rewardedAd != null && rewardedAd.CanShowAd();

    // ================= UI CONNECTION =================
    public void ReconnectUI()
    {
        // If we don't have a button, try to find one by text content "Revive" or "Watch Ad"
        if (watchAdButton == null)
        {
            Debug.Log("[RewardedAdController] Searching for Revive/Watch Button in scene...");
            var buttons = GameObject.FindObjectsOfType<Button>(true);
            foreach (var b in buttons)
            {
                var tmp = b.GetComponentInChildren<TextMeshProUGUI>(true);
                if (tmp != null)
                {
                    string t = tmp.text.ToLower();
                    // Covers "Revive" or "Watch Ad"
                    if (t.Contains("revive") || (t.Contains("watch") && t.Contains("ad")))
                    {
                        watchAdButton = b;
                        Debug.Log($"[RewardedAdController] Found button: '{b.name}' with text '{t}'");
                        break; 
                    }
                }
            }
        }

        // Bind the listener
        if (watchAdButton != null)
        {
            watchAdButton.onClick.RemoveAllListeners();
            watchAdButton.onClick.AddListener(OnWatchAdButtonPressed);
            Debug.Log("[RewardedAdController] UI Connected Successfully.");
        }
        else
        {
            // Not necessarily an error, maybe this screen strictly doesn't have one
           // Debug.Log("[RewardedAdController] No Revive/Ad button found.");
        }
    }
}
