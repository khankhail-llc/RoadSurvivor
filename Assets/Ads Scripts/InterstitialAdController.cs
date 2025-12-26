using System;
using System.Collections;
using UnityEngine;
using GoogleMobileAds.Api;

public class InterstitialAdController : MonoBehaviour
{
    public static InterstitialAdController Instance { get; private set; }

    private InterstitialAd interstitialAd;
    private int gameOverCount = 0;
    private const int GAMES_PER_AD = 5;

    [Header("Ad Unit IDs")]
    [SerializeField] private string androidAdUnitId = "ca-app-pub-3025488325095617/7524676922";
    [SerializeField] private string iosAdUnitId = "ca-app-pub-3025488325095617/5821654963";

    private bool isLoading = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadInterstitialAd();
    }

    public void LoadInterstitialAd()
    {
        if (interstitialAd != null || isLoading) return; // prevent multiple load calls

        string adUnitId = GetAdUnitId();
        isLoading = true;

        AdRequest request = new AdRequest();

        InterstitialAd.Load(adUnitId, request, (InterstitialAd ad, LoadAdError error) =>
        {
            isLoading = false;

            if (error != null || ad == null)
            {
                Debug.LogError("Interstitial ad failed to load: " + error?.GetMessage());
                return;
            }

            interstitialAd = ad;
            Debug.Log("Interstitial Loaded Successfully!");

            // Subscribe to events
            interstitialAd.OnAdFullScreenContentOpened += () => 
            { 
                Debug.Log("Interstitial opened");
                // Ad open hone par game pause karo (agar already paused nahi hai)
                if (Time.timeScale > 0)
                {
                    Time.timeScale = 0f;
                }
            };
            interstitialAd.OnAdFullScreenContentClosed += () =>
            {
                Debug.Log("Interstitial closed");
                
                // IMPORTANT: Ad close hone ke baad game continue nahi honi chahiye
                // Game over state maintain karo - Time.timeScale = 0f rakhna chahiye
                if (GameManager.Instance != null && GameManager.Instance.isGameOver)
                {
                    // Coroutine start karo to ensure game paused rahe
                    Instance.StartCoroutine(EnsureGamePausedAfterAd());
                }
                
                interstitialAd.Destroy();
                interstitialAd = null;
                LoadInterstitialAd(); // reload for next time
            };
            interstitialAd.OnAdFullScreenContentFailed += (AdError adError) =>
            {
                Debug.LogError("Interstitial failed to show: " + adError.GetMessage());
                
                // Ad fail hone par bhi game over state maintain karo
                if (GameManager.Instance != null && GameManager.Instance.isGameOver)
                {
                    Time.timeScale = 0f;
                }
                
                interstitialAd.Destroy();
                interstitialAd = null;
                LoadInterstitialAd(); // retry load
            };
        });
    }

    public void TryShowAd(Action onAdFinished = null)
    {
        gameOverCount++;
        if (gameOverCount < GAMES_PER_AD)
        {
            onAdFinished?.Invoke();
            return; // only show every 3rd game over
        }

        gameOverCount = 0;

        if (interstitialAd != null && interstitialAd.CanShowAd())
        {
            interstitialAd.Show();
            onAdFinished?.Invoke();
        }
        else
        {
            Debug.Log("Interstitial Not Ready → Will retry next time.");
            LoadInterstitialAd();
            onAdFinished?.Invoke();
        }
    }

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

    private void OnDestroy()
    {
        if (Instance == this)
        {
            interstitialAd?.Destroy();
            Instance = null;
        }
    }

    // Legacy support for old scripts
    public void LoadInterstitial() => LoadInterstitialAd();
    public void TryShowInterstitial() => TryShowAd();
    
    // Coroutine to ensure game remains paused after interstitial ad closes
    private IEnumerator EnsureGamePausedAfterAd()
    {
        // Wait a frame to ensure ad SDK has finished processing
        yield return null;
        
        // Multiple checks to ensure game stays paused
        for (int i = 0; i < 5; i++)
        {
            if (GameManager.Instance != null && GameManager.Instance.isGameOver)
            {
                Time.timeScale = 0f; // Force game to stay paused
            }
            yield return new WaitForSecondsRealtime(0.1f);
        }
        
        Debug.Log("[InterstitialAdController] Game remains paused after ad closed (Game Over state)");
    }
}
