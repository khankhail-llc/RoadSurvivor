using UnityEngine;
using GoogleMobileAds.Api;
using UnityEngine.SceneManagement;
using System.Linq;

public class BannerAdController : MonoBehaviour
{
    public static BannerAdController Instance { get; private set; }
    private BannerView bannerView;
    private bool isBannerLoaded = false;

    [Header("Ad Unit IDs")]
    [SerializeField] private string androidAdUnitId = "ca-app-pub-3025488325095617/8891220170";
    [SerializeField] private string iosAdUnitId = "ca-app-pub-3025488325095617/7928001602";

    [Header("Scenes To Show Banner")]
    [SerializeField] private string[] bannerScenes = { "Main Menu", "GameScene", "GamePlay" };

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.Log("[BannerAdController] Duplicate instance detected, destroying new instance.");
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("[BannerAdController] Instance initialized.");

        MobileAds.Initialize(initStatus =>
        {
            Debug.Log("[BannerAdController] MobileAds initialized.");
            CreateAndLoadBanner();
        });

        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log("[BannerAdController] SceneManager.sceneLoaded subscribed.");
    }

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameRestart += ReloadBanner;
            Debug.Log("[BannerAdController] Subscribed to GameManager.OnGameRestart.");
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnGameRestart -= ReloadBanner;
            Debug.Log("[BannerAdController] Unsubscribed from GameManager.OnGameRestart.");
        }
    }

    public void CreateAndLoadBanner()
    {
        Debug.Log("[BannerAdController] Creating and loading banner...");
        bannerView?.Destroy();
        bannerView = null;
        isBannerLoaded = false;

        string adUnitId = GetAdUnitId();
        bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Bottom);

        bannerView.OnBannerAdLoaded += () =>
        {
            isBannerLoaded = true;
            Debug.Log("[BannerAdController] Banner loaded successfully.");
            ShowBannerInCurrentScene();
        };

        bannerView.OnBannerAdLoadFailed += (error) =>
        {
            isBannerLoaded = false;
            Debug.LogError("[BannerAdController] Banner failed to load: " + error.GetMessage());
            Debug.Log("[BannerAdController] Retrying banner load in 10 seconds...");
            Invoke(nameof(CreateAndLoadBanner), 10f);
        };

        bannerView.LoadAd(new AdRequest());
        bannerView.Hide();
        Debug.Log("[BannerAdController] Banner request sent and initially hidden.");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("[BannerAdController] Scene loaded: " + scene.name);
        ShowBannerInCurrentScene();
    }

    private void ShowBannerInCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (bannerScenes.Contains(currentScene))
        {
            if (isBannerLoaded)
            {
                bannerView.Show();
                Debug.Log("[BannerAdController] Banner shown in scene: " + currentScene);
            }
            else
            {
                Debug.Log("[BannerAdController] Banner not yet loaded, cannot show in scene: " + currentScene);
            }
        }
        else
        {
            bannerView?.Hide();
            Debug.Log("[BannerAdController] Banner hidden (scene not in bannerScenes): " + currentScene);
        }
    }

    public void ShowBanner()
    {
        bannerView?.Show();
        Debug.Log("[BannerAdController] ShowBanner() called.");
    }

    public void HideBanner()
    {
        bannerView?.Hide();
        Debug.Log("[BannerAdController] HideBanner() called.");
    }

    public void ReloadBanner()
    {
        Debug.Log("[BannerAdController] ReloadBanner() called.");
        CreateAndLoadBanner();
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
            SceneManager.sceneLoaded -= OnSceneLoaded;
            bannerView?.Destroy();
            Instance = null;
            Debug.Log("[BannerAdController] Destroyed and unsubscribed from sceneLoaded.");
        }
    }
}
