using UnityEngine;
using UnityEngine.UI;

public class SelectColorButton : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private Button selectButton;

    [SerializeField] private Image buttonImage;

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
}
