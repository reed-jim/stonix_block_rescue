using System.Collections;
using System.Collections.Generic;
using Saferio.Util.SaferioTween;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private RectTransform canvas;
    [SerializeField] private RectTransform container;
    [SerializeField] private RectTransform loadingTextRT;
    [SerializeField] private Image fadeBackground;

    [Header("CUSTOMIZE")]
    [SerializeField] private float startDelay;
    [SerializeField] private float fadeOutDuration;

    private Vector2 _canvasSize;

    private void Awake()
    {
        _canvasSize = canvas.sizeDelta;
        
        Transition();
    }

    private void Transition()
    {
        SaferioTween.DelayAsync(startDelay, onCompletedAction: (() =>
        {
            SaferioTween.LocalPositionAsync(loadingTextRT, new Vector3(0, -_canvasSize.y), duration: fadeOutDuration);
            SaferioTween.AlphaAsync(fadeBackground, 0, duration: fadeOutDuration, onCompletedAction: () =>
            {
                container.gameObject.SetActive(false);
            });
        }));
    }
}
