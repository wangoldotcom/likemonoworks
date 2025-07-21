using UnityEngine;
using UnityEngine.Advertisements;
using System.Collections;

public class AdsManager : MonoBehaviour, IUnityAdsInitializationListener, IUnityAdsLoadListener, IUnityAdsShowListener
{
    private static AdsManager _instance;

    public static AdsManager Instance
    {
        get { return _instance; }
    }

    private string gameId;
    private bool testMode = false;

    private string interstitialPlacementId;
    private string bannerPlacementId;

    private bool isInterstitialAdLoaded = false;
    private bool isBannerAdLoaded = false;
    private bool isInterstitialAdShowing = false;

    public bool IsInterstitialAdShowing
    {
        get { return isInterstitialAdShowing; }
    }

    void Awake()
    {
        if (_instance != null)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeAds();
    }

    public void InitializeAds()
    {
#if UNITY_IOS
        gameId = "5664137"; // 실제 iOS용 gameId를 입력하세요
        interstitialPlacementId = "Interstitial_iOS"; // 실제 iOS용 Placement ID
        bannerPlacementId = "Banner_iOS"; // 실제 iOS용 Banner Placement ID
#elif UNITY_ANDROID
        gameId = "5664136"; // 실제 Android용 gameId를 입력하세요
        interstitialPlacementId = "Interstitial_Android"; // 실제 Android용 Placement ID
        bannerPlacementId = "Banner_Android"; // 실제 Android용 Banner Placement ID
#endif

        Advertisement.Initialize(gameId, testMode, this);
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads Initialization Complete.");
        LoadInterstitialAd();
        LoadBannerAd();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }

    // 전면 광고 로드 및 표시
    public void LoadInterstitialAd()
    {
        Advertisement.Load(interstitialPlacementId, this);
    }

    public void ShowInterstitialAd()
    {
        if (isInterstitialAdLoaded)
        {
            isInterstitialAdShowing = true; // 광고 표시 시작
            Advertisement.Show(interstitialPlacementId, this);
        }
        else
        {
            Debug.Log("Interstitial ad not loaded yet.");
            LoadInterstitialAd();
        }
    }

    // 배너 광고 로드 및 표시
    public void LoadBannerAd()
    {
        BannerLoadOptions options = new BannerLoadOptions
        {
            loadCallback = OnBannerLoaded,
            errorCallback = OnBannerFailedToLoad
        };
        Advertisement.Banner.SetPosition(BannerPosition.TOP_CENTER);
        Advertisement.Banner.Load(bannerPlacementId, options);
    }

    public void ShowBannerAd()
    {
        if (isBannerAdLoaded)
        {
            Advertisement.Banner.Show(bannerPlacementId);
        }
        else
        {
            Debug.Log("Banner ad not loaded yet.");
            LoadBannerAd();
        }
    }

    public void HideBannerAd()
    {
        Advertisement.Banner.Hide(true);
        Debug.Log("Banner ad hidden.");
    }

    // IUnityAdsLoadListener 구현
    public void OnUnityAdsAdLoaded(string placementId)
    {
        Debug.Log($"Ad loaded: {placementId}");
        if (placementId == interstitialPlacementId)
        {
            isInterstitialAdLoaded = true;
        }
        else if (placementId == bannerPlacementId)
        {
            isBannerAdLoaded = true;
            ShowBannerAd();
        }
    }

    public void OnUnityAdsFailedToLoad(string placementId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Failed to load Ad Unit {placementId}: {error.ToString()} - {message}");
        // 필요하면 재시도 로직 추가
    }

    // IUnityAdsShowListener 구현
    public void OnUnityAdsShowFailure(string placementId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Failed to show Ad Unit {placementId}: {error.ToString()} - {message}");
        if (placementId == interstitialPlacementId)
        {
            isInterstitialAdShowing = false;
            isInterstitialAdLoaded = false;
            LoadInterstitialAd();
        }
    }

    public void OnUnityAdsShowStart(string placementId)
    {
        if (placementId == interstitialPlacementId)
        {
            isInterstitialAdShowing = true;
        }
    }

    public void OnUnityAdsShowClick(string placementId) { }

    public void OnUnityAdsShowComplete(string placementId, UnityAdsShowCompletionState showCompletionState)
    {
        if (placementId == interstitialPlacementId)
        {
            isInterstitialAdShowing = false;
            isInterstitialAdLoaded = false;
            LoadInterstitialAd();
        }
    }

    // 배너 광고 콜백
    void OnBannerLoaded()
    {
        Debug.Log("Banner loaded.");
        isBannerAdLoaded = true;
        ShowBannerAd();
    }

    void OnBannerFailedToLoad(string message)
    {
        Debug.Log($"Banner failed to load: {message}");
        isBannerAdLoaded = false;
        // 필요하면 재시도 로직 추가
    }
}
