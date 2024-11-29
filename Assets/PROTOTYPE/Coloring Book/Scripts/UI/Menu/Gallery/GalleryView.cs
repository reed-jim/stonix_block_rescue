using Saferio.Prototype.ColoringBook;
using Saferio.Util;
using Saferio.Util.SaferioTween;
using UnityEngine;
using static Saferio.Prototype.ColoringBook.GameEnum;

public class GalleryView : MonoBehaviour
{
    [SerializeField] private RectTransform container;

    [SerializeField] private ObjectPoolingScrollView galleryByTag;

    [Header("SCRIPTABLE OBJECT")]
    [SerializeField] private Vector2Variable canvasSize;

    [Header("CUSTOMIZE")]
    [SerializeField] private GalleryImageTag galleryImageTag;
    [SerializeField] private bool isSelectedAtBegin;

    private void Awake()
    {
        GalleryImageTagUI.selectTagEvent += OnGalleryImageTagSelected;

        if (isSelectedAtBegin)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        GalleryImageTagUI.selectTagEvent -= OnGalleryImageTagSelected;
    }

    private void Open()
    {
        container.gameObject.SetActive(true);

        UIUtil.SetLocalPositionX(container, canvasSize.Value.x);

        SaferioTween.LocalPositionAsync(container, Vector2.zero, duration: 0.5f);
    }

    private void Close()
    {
        SaferioTween.LocalPositionAsync(container, new Vector2(-canvasSize.Value.x, container.localPosition.y), duration: 0.5f, onCompletedAction: () =>
        {
            container.gameObject.SetActive(false);
        });
    }

    private void OnGalleryImageTagSelected(GalleryImageTag tag)
    {
        if (tag == galleryImageTag)
        {
            Open();
        }
        else
        {
            Close();
        }
    }
}
