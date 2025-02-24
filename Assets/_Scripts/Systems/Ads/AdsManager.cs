using GoogleMobileAds.Api;
using System;
using UnityEngine;

public class AdsManager
{
#if Platform_Mobile
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
    //private const string bannerUnitId = "ca-app-pub-8907326292508524/3533112980"; // my banner id

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
    //private const string rewardedAdUnitId = "ca-app-pub-8907326292508524/9668982787"; // my rewarded ad id

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
#endif

    public static void Init()
    {
#if Platform_Mobile
        MobileAds.SetiOSAppPauseOnBackground(true);
        MobileAds.RaiseAdEventsOnUnityMainThread = true;

        MobileAds.SetRequestConfiguration(requestConfiguration);

        MobileAds.Initialize(InitializeStatus);
#endif
    }

    private static void InitializeStatus(InitializationStatus status)
    {
#if Platform_Mobile
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
#endif
    }

    #region BottomBannerAd
    public static void InitBottomBannerAd()
    {
#if Platform_Mobile
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
#endif
    }

    private static void ListenBannerAdEvents()
    {
#if Platform_Mobile
        _bannerView.OnBannerAdLoaded += () =>
        {
            //Debug.LogWarning($"Banner view loaded an ad with response : {_bannerView.GetResponseInfo()}");

            RectSafeArea.RefreshAdSafeArea(true, _bannerView.GetHeightInPixels());

            GC.Collect();

            if (GameManager.Instance.hasStarted)
            {
                DestroyBottomBannerAd();
            }
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
#endif
    }

    public static void DestroyBottomBannerAd()
    {
#if Platform_Mobile
        _bannerView?.Destroy();
        _bannerView = null;
        RectSafeArea.RefreshAdSafeArea(false, 0);
#endif
    }
    #endregion

    #region RewardedAd
    public static void InitRewardedAd()
    {
#if Platform_Mobile
        _rewardedAd?.Destroy();

        AdRequest adRequest = new();

        UiManager.Instance.SetUi(UiType.Loading, true, 0.25f);
        RewardedAd.Load(rewardedUnitId, adRequest, RewardedAdLoad);
#endif
    }

    private static void RewardedAdLoad(RewardedAd ad, LoadAdError error)
    {
#if Platform_Mobile
        if (error != null)
        {
            Debug.LogError($"Rewarded ad failed to load an ad with error : {error}");
            UiManager.Instance.SetUi(UiType.Loading, false);
            return;
        }
        if (ad == null)
        {
            Debug.LogError("Unexpected error: Rewarded load event fired with null ad and null error.");
            UiManager.Instance.SetUi(UiType.Loading, false);
            return;
        }

        Debug.Log($"Rewarded ad loaded with response : {ad.GetResponseInfo()}");
        _rewardedAd = ad;

        ListenRewardedAdEvents();

        ShowRewardedAd();
#endif
    }

    private static void ListenRewardedAdEvents()
    {
#if Platform_Mobile
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

            UiManager.Instance.SetUi(UiType.Loading, false);
        };

        _rewardedAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError($"Rewarded ad failed to open full screen content with error : {error}");

            OnRewardedAdCompleted?.Invoke(false);
        };
#endif
    }

    public static void ShowRewardedAd()
    {
#if Platform_Mobile
        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _rewardedAd.Show(UserRewardEarned);
        }
        else
        {
            Debug.LogError("Rewarded ad is not ready yet.");

            UiManager.Instance.SetUi(UiType.Loading, false);
        }
#endif
    }

    private static void UserRewardEarned(Reward reward)
    {
#if Platform_Mobile
        Debug.Log($"Rewarded ad granted a reward: {reward.Amount} {reward.Type}");

        OnRewardedAdCompleted?.Invoke(true);
#endif
    }

    public static void DestroyRewardedAd()
    {
#if Platform_Mobile
        _rewardedAd?.Destroy();
        _rewardedAd = null;
#endif
    }
    #endregion
}