using System;
using UnityEngine;

public class GameplayAdsController : MonoBehaviour
{
    public static event Action showBannerAdEvent;

    private void Awake() {
        showBannerAdEvent?.Invoke();
    }
}
