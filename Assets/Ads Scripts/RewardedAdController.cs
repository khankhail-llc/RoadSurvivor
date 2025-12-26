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
    private bool adIsShowing = false;
    private float previousTimeScale = 1f;

    [Header("Ad Unit IDs")]
    [SerializeField] private string androidAdUnitId = "ca-app-pub-3025488325095617/2892239297";
    [SerializeField] private string iosAdUnitId = "ca-app-pub-3025488325095617/2675674920";

    [Header("UI")]
    public Button watchAdButton;

    private Action pendingOnReward;
    private Action pendingOnAdClosed;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
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
        if (Instance == this) Instance = null;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        rewardedAd?.Destroy();
    }

    private void Start()
    {
        ReconnectUI();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ReconnectUI();
    }

    // ================= LOAD AD =================
    public void LoadRewardedAd()
    {
        if (isLoading || (rewardedAd != null && rewardedAd.CanShowAd()))
            return;

        isLoading = true;
        rewardedAd?.Destroy();
        rewardedAd = null;

        var request = new AdRequest();
        RewardedAd.Load(GetAdUnitId(), request, AdLoadCallback);
    }

    private void AdLoadCallback(RewardedAd ad, LoadAdError error)
    {
        if (error != null || ad == null)
        {
            Debug.LogError("[RewardedAd] Load failed: " + error?.GetMessage());
            isLoading = false;
            Invoke(nameof(LoadRewardedAd), 10f);
            return;
        }

        rewardedAd = ad;
        isLoading = false;
        RegisterAdEvents();
        Debug.Log("[RewardedAd] Loaded");
    }

    private void RegisterAdEvents()
    {
        rewardedAd.OnAdFullScreenContentOpened += () =>
        {
            adIsShowing = true;
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;
            MusicManager.Instance?.PauseMusic();
        };

        rewardedAd.OnAdFullScreenContentClosed += () =>
        {
            adIsShowing = false;

            if (!GameManager.Instance?.isReviving ?? false)
                Time.timeScale = previousTimeScale;

            MusicManager.Instance?.ResumeMusic();

            pendingOnAdClosed?.Invoke();
            pendingOnAdClosed = null;

            LoadRewardedAd();
        };

        rewardedAd.OnAdFullScreenContentFailed += (AdError err) =>
        {
            adIsShowing = false;
            Time.timeScale = previousTimeScale;
            MusicManager.Instance?.ResumeMusic();

            Debug.LogError("[RewardedAd] Show failed: " + err.GetMessage());

            pendingOnAdClosed?.Invoke();
            pendingOnAdClosed = null;

            LoadRewardedAd();
        };
    }

    // ================= SHOW AD =================
    public void ShowRewardedAd(Action onRewardEarned, Action onAdClosed)
    {
        if (rewardedAd != null && rewardedAd.CanShowAd() && !adIsShowing)
        {
            pendingOnReward = onRewardEarned;
            pendingOnAdClosed = onAdClosed;

            previousTimeScale = Time.timeScale;

            rewardedAd.Show((Reward reward) =>
            {
                pendingOnReward?.Invoke();
                pendingOnReward = null;
            });
        }
        else
        {
            Debug.Log("[RewardedAd] Not ready");
            onAdClosed?.Invoke();
        }
    }

    // ================= BUTTON =================
    public void OnWatchAdButtonPressed()
    {
        ShowRewardedAd(
            () =>
            {
                StartCoroutine(DelayedRevive());
            },
            () =>
            {
                Debug.Log("Rewarded Ad closed");
            }
        );
    }

    // ================= REVIVE + SHIELD =================
    private IEnumerator DelayedRevive()
    {
        yield return new WaitForSecondsRealtime(0.3f);

        Time.timeScale = 1f;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.RevivePlayer();

            // ✅ NEW (SAFE): Revive Shield for 5 sec
            FindObjectOfType<PlayerController>()?.ActivateReviveShield();
        }
    }

    // ================= HELPERS =================
    public bool IsRewardedAdReady() =>
        rewardedAd != null && rewardedAd.CanShowAd();

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

    // ================= UI RECONNECT =================
    public void ReconnectUI()
    {
        if (watchAdButton != null)
        {
            watchAdButton.onClick.RemoveAllListeners();
            watchAdButton.onClick.AddListener(OnWatchAdButtonPressed);
            return;
        }

        var buttons = GameObject.FindObjectsOfType<Button>(true);
        foreach (var b in buttons)
        {
            var tmp = b.GetComponentInChildren<TextMeshProUGUI>(true);
            if (tmp != null)
            {
                string t = tmp.text.ToLower();
                if (t.Contains("watch") && t.Contains("ad"))
                {
                    watchAdButton = b;
                    watchAdButton.onClick.RemoveAllListeners();
                    watchAdButton.onClick.AddListener(OnWatchAdButtonPressed);
                    return;
                }
            }
        }
    }
}
