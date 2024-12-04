using UnityEngine;

public class GalleryScrollViewModifier : MonoBehaviour, ISaferioLayoutModifier
{
    [SerializeField] private RectTransform canvas;
    [SerializeField] private RectTransform viewArea;

    private Vector2 _canvasSize;

    public void Modify()
    {
        _canvasSize = canvas.sizeDelta;

        UIUtil.SetSize(viewArea, 0.9f * _canvasSize.x, 0.7f * _canvasSize.y);

        UIUtil.SetLocalPositionOfRectToAnotherRectVertically(viewArea, canvas, 0.5f, -0.5f);
    }
}
