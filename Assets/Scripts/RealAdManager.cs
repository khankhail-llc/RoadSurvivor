using UnityEngine;
using GoogleMobileAds.Api;
using System;

/// <summary>
/// Real Google AdMob Ads Manager - Production ads ke liye
/// </summary>
public class RealAdManager : MonoBehaviour
{
    [Header("AdMob Settings")]
    [SerializeField] private string androidAdUnitId = "YOUR_ANDROID_AD_UNIT_ID";
    [SerializeField] private string iosAdUnitId = "YOUR_IOS_AD_UNIT_ID";
    [SerializeField] private bool useTestAds = true; // Testing ke liye true rakho
    
    private RewardedAd rewardedAd;
    private Action onAdCompletedCallback;
    
    private static RealAdManager instance;
    public static RealAdManager Instance => instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAds();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAds()
    {
        // Google Mobile Ads SDK initialize karo
        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("AdMob initialized successfully!");
            LoadRewardedAd();
        });
    }

    /// <summary>
    /// Rewarded ad load karo
    /// </summary>
    private void LoadRewardedAd()
    {
        // Cleanup old ad
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        Debug.Log("Loading rewarded ad...");

        // Ad Unit ID set karo (platform ke hisaab se)
        string adUnitId = GetAdUnitId();

        // Request configuration
        AdRequest adRequest = new AdRequest();

        // Load the rewarded ad
        RewardedAd.Load(adUnitId, adRequest, (RewardedAd ad, LoadAdError error) =>
        {
            if (error != null || ad == null)
            {
                Debug.LogError("Rewarded ad failed to load: " + error);
                return;
            }

            Debug.Log("Rewarded ad loaded successfully!");
            rewardedAd = ad;

            // Register event handlers
            RegisterEventHandlers(ad);
        });
    }

    /// <summary>
    /// Platform ke hisaab se Ad Unit ID return karo
    /// </summary>
    private string GetAdUnitId()
    {
        if (useTestAds)
        {
            // Test Ad Unit IDs
#if UNITY_ANDROID
            return "ca-app-pub-3940256099942544/5224354917"; // Google test rewarded ad
#elif UNITY_IOS
            return "ca-app-pub-3940256099942544/1712485313"; // Google test rewarded ad
#else
            return "ca-app-pub-3940256099942544/5224354917";
#endif
        }
        else
        {
            // Real Ad Unit IDs
#if UNITY_ANDROID
            return androidAdUnitId;
#elif UNITY_IOS
            return iosAdUnitId;
#else
            return androidAdUnitId;
#endif
        }
    }

    /// <summary>
    /// Ad event handlers register karo
    /// </summary>
    private void RegisterEventHandlers(RewardedAd ad)
    {
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log("Rewarded ad paid: " + adValue.Value);
        };

        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad impression recorded");
        };

        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad clicked");
        };

        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad opened");
        };

        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad closed");
            // Ad close hone ke baad dobara load karo
            LoadRewardedAd();
        };

        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to show: " + error);
            // Fail hone par bhi dobara load karo
            LoadRewardedAd();
        };
    }

    /// <summary>
    /// Rewarded ad dikhao
    /// </summary>
    public void ShowRewardedAd(Action onComplete)
    {
        if (rewardedAd != null && rewardedAd.CanShowAd())
        {
            Debug.Log("Showing rewarded ad...");
            onAdCompletedCallback = onComplete;

            rewardedAd.Show((Reward reward) =>
            {
                // User ne puri ad dekhi aur reward mila!
                Debug.Log("User earned reward: " + reward.Type + " - " + reward.Amount);
                onAdCompletedCallback?.Invoke();
                onAdCompletedCallback = null;
            });
        }
        else
        {
            Debug.LogWarning("Rewarded ad is not ready yet! Loading new ad...");
            LoadRewardedAd();
            
            // Backup: Agar ad ready nahi hai, toh user ko message dikha sakte ho
            // Ya phir wait karke dobara try kar sakte ho
        }
    }

    /// <summary>
    /// Check karo ad ready hai ya nahi
    /// </summary>
    public bool IsAdReady()
    {
        return rewardedAd != null && rewardedAd.CanShowAd();
    }

    private void OnDestroy()
    {
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
        }
    }
}
