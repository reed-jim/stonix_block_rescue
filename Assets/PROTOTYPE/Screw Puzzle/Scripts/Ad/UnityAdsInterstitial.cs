using System;
using UnityEngine;
using UnityEngine.Advertisements;

public class UnityAdsInterstitial : MonoBehaviour, IUnityAdsLoadListener, IUnityAdsShowListener
{
    [SerializeField] string _androidAdUnitId = "Interstitial_Android";
    [SerializeField] string _iOsAdUnitId = "Interstitial_iOS";
    string _adUnitId;

    #region ACTION
    public static event Action interstitialAdCompletedEvent;
    #endregion

    void Awake()
    {
        UnityAdsInitializer.onUnityAdsSDKLoadedEvent += LoadAd;
        GameplayUIScreen.showInterstitialAdEvent += ShowAd;

        _adUnitId = (Application.platform == RuntimePlatform.IPhonePlayer)
            ? _iOsAdUnitId
            : _androidAdUnitId;

        DontDestroyOnLoad(gameObject);
    }

    private void OnDestroy()
    {
        UnityAdsInitializer.onUnityAdsSDKLoadedEvent -= LoadAd;
        GameplayUIScreen.showInterstitialAdEvent -= ShowAd;
    }

    public void LoadAd()
    {
        Advertisement.Load(_adUnitId, this);
    }

    public void ShowAd()
    {
        Advertisement.Show(_adUnitId, this);
    }

    public void OnUnityAdsAdLoaded(string adUnitId)
    {

    }

    public void OnUnityAdsFailedToLoad(string _adUnitId, UnityAdsLoadError error, string message)
    {
        Debug.Log($"Error loading Ad Unit: {_adUnitId} - {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowFailure(string _adUnitId, UnityAdsShowError error, string message)
    {
        Debug.Log($"Error showing Ad Unit {_adUnitId}: {error.ToString()} - {message}");
    }

    public void OnUnityAdsShowStart(string _adUnitId)
    {

    }
    public void OnUnityAdsShowClick(string _adUnitId)
    {

    }
    public void OnUnityAdsShowComplete(string _adUnitId, UnityAdsShowCompletionState showCompletionState)
    {
        interstitialAdCompletedEvent?.Invoke();
    }
}
