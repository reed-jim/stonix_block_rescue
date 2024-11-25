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

    public static event Action replayEvent;

    private void Awake()
    {
        WinChecker.winLevelEvent += OnWin;

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
    }

    private void BackHome()
    {
        SceneManager.LoadSceneAsync("Home");
    }

    private void Replay()
    {
        replayEvent?.Invoke();

        winTextRT.gameObject.SetActive(false);
        continueButtonRT.gameObject.SetActive(false);
        fadeBackground.gameObject.SetActive(false);
    }

    private void OnWin()
    {
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
}
