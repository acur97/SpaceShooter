#if Platform_Mobile
using GoogleMobileAds.Api;
using System;
using UnityEngine;
#endif

public class AdsManager
{
#if Platform_Mobile
    public enum AdType
    {
        Banner,
        Banner2,
        Rewarded_Life,
        Rewarded2_Life,
        Rewarded_Coins,
        Rewarded2_Coins
    }

    private const string bannerUnitId1 = "ca-app-pub-8907326292508524/3533112980";
    private const string bannerUnitId2 = "ca-app-pub-8907326292508524/2375478360";
    private static int bannerUnitTrys = 0;

    public const string rewardedUnitId1_Life = "ca-app-pub-8907326292508524/9668982787";
    public const string rewardedUnitId2_Life = "ca-app-pub-8907326292508524/7605254741";
    private static int rewardedUnitTrys_Life = 0;

    public const string rewardedUnitId1_Coins = "ca-app-pub-8907326292508524/9624656261";
    public const string rewardedUnitId2_Coins = "ca-app-pub-8907326292508524/8176100389";
    private static int rewardedUnitTrys_Coins = 0;

    private static readonly RequestConfiguration requestConfiguration = new();

    private static bool isInitialized = false;

    private static BannerView _bannerView;
    public static bool BannerLoaded = false;
    private static RewardedAd _rewardedAd;
    public static bool RewardedLoaded = false;
    public static Action<bool, double> OnRewardedAdLoaded;
    public static Action<bool> OnRewardedAdCompleted;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        isInitialized = false;

        _bannerView = null;
        _rewardedAd = null;

        OnRewardedAdLoaded = null;
        OnRewardedAdCompleted = null;

        bannerUnitTrys = 0;
        rewardedUnitTrys_Life = 0;
        rewardedUnitTrys_Coins = 0;

        BannerLoaded = false;
        RewardedLoaded = false;
    }
#endif

    public static void Init()
    {
#if Platform_Mobile
        bannerUnitTrys = 0;
        rewardedUnitTrys_Life = 0;
        rewardedUnitTrys_Coins = 0;

        MobileAds.SetiOSAppPauseOnBackground(true);
        MobileAds.RaiseAdEventsOnUnityMainThread = true;

        MobileAds.SetRequestConfiguration(requestConfiguration);

        MobileAds.Initialize(InitializeStatus);
#endif
    }

#if Platform_Mobile
    private static void InitializeStatus(InitializationStatus status)
    {
        if (status == null)
        {
            Debug.LogError("Google Mobile Ads initialization failed.");
            isInitialized = false;
        }
        else
        {
            Debug.Log("Google Mobile Ads initialization succeeded.");
            isInitialized = true;

            PrepareAd(AdType.Banner, true);
        }
    }

    public static void PrepareAd(AdType type, bool showWhenReady = false)
    {
        if (!isInitialized)
        {
            Debug.LogError("Google Mobile Ads SDK is not correctly initialized.");
            return;
        }

        switch (type)
        {
            case AdType.Banner:
                bannerUnitTrys++;
                PrepareBanner(bannerUnitId1);
                break;

            case AdType.Banner2:
                bannerUnitTrys++;
                PrepareBanner(bannerUnitId2);
                break;

            case AdType.Rewarded_Life:
                rewardedUnitTrys_Life++;
                PrepareRewarded(rewardedUnitId1_Life, showWhenReady);
                break;

            case AdType.Rewarded2_Life:
                rewardedUnitTrys_Life++;
                PrepareRewarded(rewardedUnitId2_Life, showWhenReady);
                break;

            case AdType.Rewarded_Coins:
                rewardedUnitTrys_Coins++;
                PrepareRewarded(rewardedUnitId1_Coins, showWhenReady);
                break;

            case AdType.Rewarded2_Coins:
                rewardedUnitTrys_Coins++;
                PrepareRewarded(rewardedUnitId2_Coins, showWhenReady);
                break;
        }
    }

    private static void PrepareBanner(string id)
    {
        RectSafeArea.RefreshAdSafeArea(false, 0);

        _bannerView?.Destroy();
        _bannerView = new BannerView(id, AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth), AdPosition.Bottom);

        ListenBannerEvents();

        _bannerView.LoadAd(new AdRequest());
    }

    private static void ListenBannerEvents()
    {
        if (_bannerView == null)
        {
            return;
        }

        _bannerView.OnBannerAdLoaded += OnBanner_Loaded;
        _bannerView.OnBannerAdLoadFailed += OnBanner_LoadFailed;

        _bannerView.OnAdPaid += OnBanner_Paid;
        _bannerView.OnAdImpressionRecorded += OnBanner_ImpressionRecorded;
        _bannerView.OnAdClicked += OnBanner_Clicked;
        _bannerView.OnAdFullScreenContentOpened += OnBanner_FullScreenContentOpened;
        _bannerView.OnAdFullScreenContentClosed += OnBanner_FullScreenContentClosed;
    }

    #region Banner Events
    private static void OnBanner_Loaded()
    {
        Debug.LogWarning($"Banner view loaded an ad with response : {_bannerView.GetResponseInfo()}");

        RectSafeArea.RefreshAdSafeArea(true, _bannerView.GetHeightInPixels());
        BannerLoaded = true;

        if (GameManager.Instance.hasStarted)
        {
            DestroyBottomBannerAd();

            GC.Collect();
        }
    }

    private static void OnBanner_LoadFailed(LoadAdError error)
    {
        Debug.LogError($"Banner view failed to load an ad with error : {error}");

        DestroyBottomBannerAd();

        if (bannerUnitTrys == 1)
        {
            Debug.LogWarning("Trying to load ad #2 ...");
            PrepareAd(AdType.Banner2);
        }
    }

    private static void OnBanner_Paid(AdValue adValue)
    {
        Debug.Log($"Banner view paid {adValue.Value} {adValue.CurrencyCode}.");
    }

    private static void OnBanner_ImpressionRecorded()
    {
        Debug.Log("Banner view recorded an impression.");
    }

    private static void OnBanner_Clicked()
    {
        Debug.Log("Banner view was clicked.");
    }

    private static void OnBanner_FullScreenContentOpened()
    {
        Debug.Log("Banner view full screen content opened.");
    }

    private static void OnBanner_FullScreenContentClosed()
    {
        Debug.Log("Banner view full screen content closed.");
    }
    #endregion

    public static void DestroyBottomBannerAd(bool resetRetrys = false)
    {
        if (resetRetrys)
        {
            ResetBannerRetrys();
        }

        if (_bannerView != null)
        {
            _bannerView.OnBannerAdLoaded -= OnBanner_Loaded;
            _bannerView.OnBannerAdLoadFailed -= OnBanner_LoadFailed;

            _bannerView.OnAdPaid -= OnBanner_Paid;
            _bannerView.OnAdImpressionRecorded -= OnBanner_ImpressionRecorded;
            _bannerView.OnAdClicked -= OnBanner_Clicked;
            _bannerView.OnAdFullScreenContentOpened -= OnBanner_FullScreenContentOpened;
            _bannerView.OnAdFullScreenContentClosed -= OnBanner_FullScreenContentClosed;

            _bannerView.Destroy();
        }

        _bannerView = null;
        BannerLoaded = false;

        RectSafeArea.RefreshAdSafeArea(false, 0);
    }

    public static void ResetBannerRetrys()
    {
        bannerUnitTrys = 0;
    }

    private static void PrepareRewarded(string id, bool showWhenReady)
    {
        _rewardedAd?.Destroy();

        RewardedAd.Load(id, new AdRequest(), (ad, error) => RewardedAdLoad(ad, error, id, showWhenReady));
    }

    private static void RewardedAdLoad(RewardedAd ad, LoadAdError error, string id, bool showWhenReady)
    {
        if (error != null)
        {
            Debug.LogError($"Rewarded ad failed to load an ad with error : {error}");

            switch (id)
            {
                case rewardedUnitId1_Life:
                    if (rewardedUnitTrys_Life == 1)
                    {
                        Debug.LogWarning("Trying to load ad #2 ...");
                        PrepareAd(AdType.Rewarded2_Life, showWhenReady);
                        return;
                    }
                    else
                    {
                        UiManager.Instance.SetUi(UiType.Loading, false);
                    }
                    break;

                case rewardedUnitId1_Coins:
                    if (rewardedUnitTrys_Coins == 1)
                    {
                        Debug.LogWarning("Trying to load ad #2 ...");
                        PrepareAd(AdType.Rewarded2_Coins, showWhenReady);
                        return;
                    }
                    else
                    {
                        UiManager.Instance.SetUi(UiType.Loading, false);
                    }
                    break;
            }

            OnRewardedAdLoaded?.Invoke(false, 0);

            return;
        }

        if (ad == null)
        {
            Debug.LogError("Unexpected error: Rewarded load event fired with null ad and null error.");
            return;
        }

        Debug.Log($"Rewarded ad loaded with response : {ad.GetResponseInfo()}");
        OnRewardedAdLoaded?.Invoke(true, ad.GetRewardItem().Amount);
        _rewardedAd = ad;
        RewardedLoaded = true;

        if (showWhenReady)
        {
            ShowRewarded();
        }
    }
#endif

    public static void ShowRewarded()
    {
#if Platform_Mobile
        ListenRewardedAdEvents();
        ShowRewardedAd();
#endif
    }

#if Platform_Mobile
    private static void ListenRewardedAdEvents()
    {
        if (_bannerView == null)
        {
            return;
        }

        _rewardedAd.OnAdPaid += OnRewarded_Paid;
        _rewardedAd.OnAdImpressionRecorded += OnRewarded_ImpressionRecorded;
        _rewardedAd.OnAdClicked += OnRewarded_Clicked;
        _rewardedAd.OnAdFullScreenContentOpened += OnRewarded_FullScreenContentOpened;
        _rewardedAd.OnAdFullScreenContentClosed += OnRewarded_FullScreenContentClosed;
        _rewardedAd.OnAdFullScreenContentFailed += OnRewarded_FullScreenContentFailed;
    }

    #region Rewarded Events
    private static void OnRewarded_Paid(AdValue adValue)
    {
        Debug.Log($"Rewarded ad paid {adValue.Value} {adValue.CurrencyCode}.");
    }

    private static void OnRewarded_ImpressionRecorded()
    {
        Debug.Log("Rewarded ad recorded an impression.");
    }

    private static void OnRewarded_Clicked()
    {
        Debug.Log("Rewarded ad was clicked.");
    }

    private static void OnRewarded_FullScreenContentOpened()
    {
        Debug.Log("Rewarded ad full screen content opened.");
    }

    private static void OnRewarded_FullScreenContentClosed()
    {
        Debug.Log("Rewarded ad full screen content closed.");

        UiManager.Instance.SetUi(UiType.Loading, false);
    }

    private static void OnRewarded_FullScreenContentFailed(AdError error)
    {
        if (error != null)
        {
            Debug.LogError($"Rewarded ad failed to open full screen content with error : {error}");
        }

        OnRewardedAdCompleted?.Invoke(false);
    }
    #endregion

    private static void ShowRewardedAd()
    {
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _rewardedAd.Show(UserRewardEarned);
        }
        else
        {
            Debug.LogError("Rewarded ad is not ready yet.");

            UiManager.Instance.SetUi(UiType.Loading, false);
        }
    }

    private static void UserRewardEarned(Reward reward)
    {
        Debug.Log($"Rewarded ad granted a reward: {reward.Amount} {reward.Type}");

        OnRewardedAdCompleted?.Invoke(true);
    }

    public static void DestroyRewarded(bool resetRetrys = false)
    {
        if (resetRetrys)
        {
            ResetRewardedRetrys();
        }

        if (_rewardedAd != null)
        {
            _rewardedAd.OnAdPaid -= OnRewarded_Paid;
            _rewardedAd.OnAdImpressionRecorded -= OnRewarded_ImpressionRecorded;
            _rewardedAd.OnAdClicked -= OnRewarded_Clicked;
            _rewardedAd.OnAdFullScreenContentOpened -= OnRewarded_FullScreenContentOpened;
            _rewardedAd.OnAdFullScreenContentClosed -= OnRewarded_FullScreenContentClosed;
            _rewardedAd.OnAdFullScreenContentFailed -= OnRewarded_FullScreenContentFailed;

            _rewardedAd.Destroy();
        }

        _rewardedAd = null;
        RewardedLoaded = false;
    }

    public static void ResetRewardedRetrys()
    {
        rewardedUnitTrys_Life = 0;
        rewardedUnitTrys_Coins = 0;
    }
#endif
}