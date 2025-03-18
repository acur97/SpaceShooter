#if Platform_Mobile
using GoogleMobileAds.Api;
using System;
using UnityEngine;
#endif

public class AdsManager
{
#if Platform_Mobile
    private static readonly RequestConfiguration requestConfiguration = new()
    {
        //        TestDeviceIds = new()
        //            {
        //                AdRequest.TestDeviceSimulator,
        //#if UNITY_IPHONE
        //                ""
        //#elif UNITY_ANDROID
        //                "bcbb08c53d80435abd0fbe4d9f37daed" // my "moto e(7) plus" ads id
        //#endif
        //            }
    };

    private static bool isInitialized = false;

    #region BottomBannerAd
    private const string bannerUnitId = "ca-app-pub-8907326292508524/3533112980";

    // test ads.
    //#if UNITY_ANDROID
    //    private const string bannerUnitId = "ca-app-pub-3940256099942544/6300978111";
    //#elif UNITY_IPHONE
    //    private const string bannerUnitId = "ca-app-pub-3940256099942544/2934735716";
    //#else
    //    private const string bannerUnitId = "unused";
    //#endif

    private static BannerView _bannerView;
    #endregion

    #region RewardedAd
    private const string rewardedUnitId = "ca-app-pub-8907326292508524/9668982787";

    // test ads.
    //#if UNITY_ANDROID
    //    private const string rewardedUnitId = "ca-app-pub-3940256099942544/5224354917";
    //#elif UNITY_IPHONE
    //    private const string rewardedUnitId = "ca-app-pub-3940256099942544/1712485313";
    //#else
    //    private const string rewardedUnitId = "unused";
    //#endif

    private static RewardedAd _rewardedAd;
    public static Action<bool> OnRewardedAdCompleted;
    #endregion

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Initialize()
    {
        isInitialized = false;
        _bannerView = null;
        _rewardedAd = null;
        OnRewardedAdCompleted = null;
    }
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

            InitBottomBannerAd();
        }
    }
#endif

    #region BottomBannerAd
#if Platform_Mobile
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
    }
#endif

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
#if Platform_Mobile
    public static void PrepareRewardedAd()
    {
        _rewardedAd?.Destroy();

        RewardedAd.Load(rewardedUnitId, new AdRequest(), RewardedAdLoad);
    }

    private static void PrepareInitRewardedAd()
    {
        _rewardedAd?.Destroy();

        RewardedAd.Load(rewardedUnitId, new AdRequest(), RewardedAdLoadInit);
    }

    public static void InitRewardedAd()
    {
        UiManager.Instance.SetUi(UiType.Loading, true, 0.25f);

        if (_rewardedAd != null)
        {
            ListenRewardedAdEvents();
            ShowRewardedAd();
        }
        else
        {
            PrepareInitRewardedAd();
        }
    }

    private static void RewardedAdLoad(RewardedAd ad, LoadAdError error)
    {
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
    }

    private static void RewardedAdLoadInit(RewardedAd ad, LoadAdError error)
    {
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

            UiManager.Instance.SetUi(UiType.Loading, false);
        };

        _rewardedAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            if (error != null)
            {
                Debug.LogError($"Rewarded ad failed to open full screen content with error : {error}");
            }

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

            UiManager.Instance.SetUi(UiType.Loading, false);
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
#endif
    #endregion
}