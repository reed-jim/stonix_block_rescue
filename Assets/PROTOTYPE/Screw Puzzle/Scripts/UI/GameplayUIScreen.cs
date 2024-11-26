using System;
using Saferio.Util.SaferioTween;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameplayUIScreen : MonoBehaviour
{
    [SerializeField] private RectTransform canvas;
    [SerializeField] private RectTransform continueButtonRT;
    [SerializeField] private Button homeButton;
    [SerializeField] private Button replayButton;
    [SerializeField] private Button continueButton;

    [SerializeField] private Image fadeBackground;
    [SerializeField] private RectTransform winTextRT;

    private Vector2 _canvasSize;

    #region ACTION
    public static event Action replayEvent;
    public static event Action showInterstitialAdEvent;
    #endregion

    private void Awake()
    {
        WinChecker.winLevelEvent += OnWin;
        UnityAdsInterstitial.interstitialAdCompletedEvent += ActualReplayLevel;

        homeButton.onClick.AddListener(BackHome);
        replayButton.onClick.AddListener(Replay);
        continueButton.onClick.AddListener(Replay);

        _canvasSize = canvas.sizeDelta;

        winTextRT.gameObject.SetActive(false);
        continueButtonRT.gameObject.SetActive(false);
        fadeBackground.gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        WinChecker.winLevelEvent -= OnWin;
        UnityAdsInterstitial.interstitialAdCompletedEvent -= ActualReplayLevel;
    }

    private void BackHome()
    {
        PlaySoundClick();

        SceneManager.LoadSceneAsync("Menu");
    }

    private void Replay()
    {
        PlaySoundClick();

        winTextRT.gameObject.SetActive(false);
        continueButtonRT.gameObject.SetActive(false);
        fadeBackground.gameObject.SetActive(false);

        showInterstitialAdEvent?.Invoke();
    }

    private void ActualReplayLevel()
    {
        replayEvent?.Invoke();
    }

    private void OnWin()
    {
        PlaySoundWin();

        winTextRT.localPosition = new Vector2(0, _canvasSize.y);
        continueButtonRT.localPosition = new Vector2(0, -_canvasSize.y);

        winTextRT.gameObject.SetActive(true);
        continueButtonRT.gameObject.SetActive(true);
        fadeBackground.gameObject.SetActive(true);
        fadeBackground.color = new Color(fadeBackground.color.r, fadeBackground.color.g, fadeBackground.color.b, 0);

        SaferioTween.AlphaAsync(fadeBackground, 0.9f, duration: 0.5f);
        SaferioTween.LocalPositionAsync(winTextRT, new Vector2(0, 0.2f * _canvasSize.y), duration: 0.5f);
        SaferioTween.LocalPositionAsync(continueButtonRT, new Vector2(0, -0.2f * _canvasSize.y), duration: 0.5f);
    }

    private void PlaySoundClick()
    {
        AudioSource selectSound = ObjectPoolingEverything.GetFromPool("ClickSound").GetComponent<AudioSource>();

        selectSound.gameObject.SetActive(true);

        selectSound.Play();
    }

    private void PlaySoundWin()
    {
        AudioSource selectSound = ObjectPoolingEverything.GetFromPool("WinSound").GetComponent<AudioSource>();

        selectSound.gameObject.SetActive(true);

        selectSound.Play();
    }
}
