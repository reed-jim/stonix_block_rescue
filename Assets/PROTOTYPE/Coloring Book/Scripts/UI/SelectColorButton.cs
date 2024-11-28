using System;
using UnityEngine;
using UnityEngine.UI;

public class SelectColorButton : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Button selectButton;

    [SerializeField] private Image buttonImage;

    private Color _colorGroup;

    public static event Action<Color> selectColorGroupEvent;

    private void Awake()
    {
        selectButton.onClick.AddListener(SelectColor);
    }

    public RectTransform RectTransform
    {
        get => rectTransform;
        set => rectTransform = value;
    }

    public Image ButtonImage
    {
        get => buttonImage;
        set => buttonImage = value;
    }

    public Color ColorGroup
    {
        get => _colorGroup;
        set => _colorGroup = value;
    }

    private void SelectColor()
    {
        selectColorGroupEvent?.Invoke(_colorGroup);
    }
}
