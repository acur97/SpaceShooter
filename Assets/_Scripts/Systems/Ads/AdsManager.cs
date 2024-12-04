using GoogleMobileAds.Api;
using System;
using UnityEngine;

public class AdsManager
{
#if Platform_Mobile || UNITY_EDITOR

    // common events
    //public Action<AdValue> OnAdPaid;
    //public Action OnAdClicked;
    //public Action OnAdImpressionRecorded;
    //public Action OnAdFullScreenContentOpened;
    //public Action OnAdFullScreenContentClosed;

    //// rewarded specific events
    //public Action<AdError> OnAdFullScreenContentFailed;

    //// banner specific events
    //public Action OnBannerAdLoaded;
    //public Action<LoadAdError> OnBannerAdLoadFailed;

    private static readonly RequestConfiguration requestConfiguration = new()
    {
        TestDeviceIds = new()
            {
                AdRequest.TestDeviceSimulator,
                #if UNITY_IPHONE
                ""
                #elif UNITY_ANDROID
                "bcbb08c53d80435abd0fbe4d9f37daed" // my "moto e(7) plus" ads id
                #endif
            }
    };

    private static bool isInitialized = false;

    #region BottomBannerAd
    //private const string bannerUnitId = "ca-app-pub-3531643773215390/6859179168"; // my banner id

    // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
    private const string bannerUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
    private const string bannerUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
    private const string bannerUnitId = "unused";
#endif

    private static BannerView _bannerView;
    #endregion

    #region RewardedAd
    //private const string rewardedAdUnitId = "ca-app-pub-3531643773215390/3453049523"; // my rewarded ad id

    // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
    private const string rewardedUnitId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
    private const string rewardedUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
    private const string rewardedUnitId = "unused";
#endif

    private static RewardedAd _rewardedAd;
    public static Action<bool> OnRewardedAdCompleted;
    #endregion

    public static void Init()
    {
        MobileAds.SetiOSAppPauseOnBackground(true);
        MobileAds.RaiseAdEventsOnUnityMainThread = true;

        MobileAds.SetRequestConfiguration(requestConfiguration);

        MobileAds.Initialize(InitializeStatus);
    }

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

            InitBottomBannerAd();
        }
    }

    #region BottomBannerAd
    public static void InitBottomBannerAd()
    {
        if (!isInitialized)
        {
            Debug.LogError("Google Mobile Ads SDK is not correctly initialized.");
            return;
        }

        RectSafeArea.RefreshAdSafeArea(false, 0);
        _bannerView?.Destroy();

        AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
        _bannerView = new BannerView(bannerUnitId, adaptiveSize, AdPosition.Bottom);

        ListenBannerAdEvents();

        AdRequest adRequest = new();
        _bannerView.LoadAd(adRequest);
    }

    private static void ListenBannerAdEvents()
    {
        _bannerView.OnBannerAdLoaded += () =>
        {
            Debug.LogWarning($"Banner view loaded an ad with response : {_bannerView.GetResponseInfo()}");

            RectSafeArea.RefreshAdSafeArea(true, _bannerView.GetHeightInPixels());
        };

        _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError($"Banner view failed to load an ad with error : {error}");
        };

        // Raised when the ad is estimated to have earned money.
        _bannerView.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log($"Banner view paid {adValue.Value} {adValue.CurrencyCode}.");
        };

        _bannerView.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Banner view recorded an impression.");
        };

        _bannerView.OnAdClicked += () =>
        {
            Debug.Log("Banner view was clicked.");
        };

        _bannerView.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Banner view full screen content opened.");
        };

        _bannerView.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Banner view full screen content closed.");
        };
    }

    public static void DestroyBottomBannerAd()
    {
        _bannerView?.Destroy();
        _bannerView = null;
        RectSafeArea.RefreshAdSafeArea(false, 0);
    }
    #endregion

    #region RewardedAd
    public static void InitRewardedAd()
    {
        _rewardedAd?.Destroy();

        AdRequest adRequest = new();

        RewardedAd.Load(rewardedUnitId, adRequest, RewardedAdLoad);
    }

    private static void RewardedAdLoad(RewardedAd ad, LoadAdError error)
    {
        if (error != null)
        {
            Debug.LogError($"Rewarded ad failed to load an ad with error : {error}");
            return;
        }
        if (ad == null)
        {
            Debug.LogError("Unexpected error: Rewarded load event fired with null ad and null error.");
            return;
        }

        Debug.Log($"Rewarded ad loaded with response : {ad.GetResponseInfo()}");
        _rewardedAd = ad;

        ListenRewardedAdEvents();

        ShowRewardedAd();
    }

    private static void ListenRewardedAdEvents()
    {
        _rewardedAd.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log($"Rewarded ad paid {adValue.Value} {adValue.CurrencyCode}.");
        };

        _rewardedAd.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };

        _rewardedAd.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };

        _rewardedAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };

        _rewardedAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad full screen content closed.");
        };

        _rewardedAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError($"Rewarded ad failed to open full screen content with error : {error}");

            OnRewardedAdCompleted?.Invoke(false);
        };
    }

    public static void ShowRewardedAd()
    {
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _rewardedAd.Show(UserRewardEarned);
        }
        else
        {
            Debug.LogError("Rewarded ad is not ready yet.");
        }
    }

    private static void UserRewardEarned(Reward reward)
    {
        Debug.Log($"Rewarded ad granted a reward: {reward.Amount} {reward.Type}");

        OnRewardedAdCompleted?.Invoke(true);
    }

    public static void DestroyRewardedAd()
    {
        _rewardedAd?.Destroy();
        _rewardedAd = null;
    }
    #endregion

#endif
}