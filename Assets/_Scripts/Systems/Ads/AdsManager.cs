using Cysharp.Threading.Tasks;
using GoogleMobileAds.Api;
using System.Collections.Generic;
using UnityEngine;

public class AdsManager : MonoBehaviour
{
#if UNITY_ANDROID || UNITY_IOS || UNITY_EDITOR

#if UNITY_ANDROID
    private const string _adUnitId = "ca-app-pub-3940256099942544/6300978111";
#elif UNITY_IPHONE
        private const string _adUnitId = "ca-app-pub-3940256099942544/2934735716";
#else
        private const string _adUnitId = "unused";
#endif

    private BannerView _bannerView;
    private RectTransform ad;

    private RequestConfiguration requestConfiguration = new()
    {
        TestDeviceIds = testDeviceIds
    };

    private static List<string> testDeviceIds = new()
    {
        AdRequest.TestDeviceSimulator,
        #if UNITY_IPHONE
        ""
        #elif UNITY_ANDROID
        "bcbb08c53d80435abd0fbe4d9f37daed"
        #endif
    };

    private async UniTaskVoid Start()
    {
        await UniTask.WaitForSeconds(2);

        MobileAds.SetRequestConfiguration(requestConfiguration);

        MobileAds.Initialize((InitializationStatus status) => RequestBanner());
    }

    private void RequestBanner()
    {
        // These ad units are configured to always serve test ads.
#if UNITY_EDITOR
        string adUnitId = "unused";
#elif UNITY_ANDROID
            string adUnitId = "ca-app-pub-3212738706492790/6113697308";
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3212738706492790/5381898163";
#else
            string adUnitId = "unexpected_platform";
#endif

        _bannerView?.Destroy();

        AdSize adaptiveSize = AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);
        _bannerView = new BannerView("ca-app-pub-3531643773215390/4052779353", adaptiveSize, AdPosition.Bottom);

        ListenToAdEvents();

        AdRequest adRequest = new();
        _bannerView.LoadAd(adRequest);
    }

    private void ListenToAdEvents()
    {
        _bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log($"Banner view loaded an ad with response : {_bannerView.GetResponseInfo()}");

            ad = GameObject.FindGameObjectWithTag(Tags.Ad).transform.GetChild(0).GetComponent<RectTransform>();
            RectSafeArea.onApplySafeArea += UpdateAdOffset;
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

    private void UpdateAdOffset()
    {
        ad.anchoredPosition = new Vector2(0, RectSafeArea.bottom - (_bannerView.GetHeightInPixels() * 0.5f));
    }

#endif
}