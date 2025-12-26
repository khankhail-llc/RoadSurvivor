using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Ump.Api;
using System;

public class AdMobInitializer : MonoBehaviour
{
    public static AdMobInitializer Instance { get; private set; }

    [Header("AdMob App IDs")]
    [SerializeField] private string androidAppId = "ca-app-pub-3025488325095617~6159624366";
    [SerializeField] private string iosAppId = "ca-app-pub-3025488325095617~8043551677";

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        ConsentRequestParameters request = new ConsentRequestParameters { TagForUnderAgeOfConsent = false };
        ConsentInformation.Update(request, OnConsentInfoUpdated);
    }

    private void OnConsentInfoUpdated(FormError error)
    {
        if (error != null) { Debug.LogError("Consent info update failed: " + error.Message); InitializeAdMob(); return; }
        if (ConsentInformation.IsConsentFormAvailable()) LoadConsentForm();
        else InitializeAdMob();
    }

    private void LoadConsentForm()
    {
        ConsentForm.Load((ConsentForm form, FormError error) =>
        {
            if (error != null) { Debug.LogError("Consent form load failed: " + error.Message); InitializeAdMob(); return; }
            if (ConsentInformation.ConsentStatus == ConsentStatus.Required)
            {
                form.Show((FormError showError) =>
                {
                    if (showError != null) Debug.LogError("Consent form show failed: " + showError.Message);
                    InitializeAdMob();
                });
            }
            else InitializeAdMob();
        });
    }

    public void InitializeAds() => InitializeAdMob();

    private void InitializeAdMob()
    {
        string appId = GetAppId();
        Debug.Log("Initializing AdMob with App ID: " + appId);
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            Debug.Log("AdMob initialization complete.");
            foreach (var adapter in initStatus.getAdapterStatusMap())
                Debug.Log($"Adapter: {adapter.Key}, State: {adapter.Value.InitializationState}, Description: {adapter.Value.Description}");
        });
    }

    private string GetAppId()
    {
#if UNITY_ANDROID
        return androidAppId;
#elif UNITY_IOS
        return iosAppId;
#else
        return "unexpected_platform";
#endif
    }

    public void ShowRewarded(Action onRewardEarned)
    {
        if (RewardedAdController.Instance != null)
            RewardedAdController.Instance.ShowRewardedAd(onRewardEarned, () => Debug.Log("Rewarded Ad Closed"));
        else Debug.LogWarning("RewardedAdController.Instance is null.");
    }

    public void ShowInterstitial()
    {
        if (InterstitialAdController.Instance != null) InterstitialAdController.Instance.TryShowAd();
        else Debug.LogWarning("InterstitialAdController.Instance is null.");
    }
}