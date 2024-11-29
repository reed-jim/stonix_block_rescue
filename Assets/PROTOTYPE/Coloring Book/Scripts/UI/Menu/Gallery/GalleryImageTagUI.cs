using System;
using PrimeTween;
using UnityEngine;
using UnityEngine.UI;
using static Saferio.Prototype.ColoringBook.GameEnum;

public class GalleryImageTagUI : MonoBehaviour
{
    [SerializeField] private Button selectButton;
    [SerializeField] private Image selectBackground;

    [SerializeField] private bool isSelectedFromStart;

    [SerializeField] private GalleryImageTag galleryImageTag;

    public static event Action<GalleryImageTag> selectTagEvent;

    private void Awake()
    {
        selectTagEvent += OnGalleryImageTagSelected;

        selectButton.onClick.AddListener(Select);

        if (isSelectedFromStart)
        {
            selectBackground.color = ColorUtil.WithAlpha(selectBackground.color, 1);
        }
        else
        {
            selectBackground.color = ColorUtil.WithAlpha(selectBackground.color, 0f);
        }
    }

    private void OnDestroy()
    {
        selectTagEvent -= OnGalleryImageTagSelected;
    }

    private void Select()
    {
        selectTagEvent?.Invoke(galleryImageTag);
    }

    private void OnGalleryImageTagSelected(GalleryImageTag tag)
    {
        if (tag == galleryImageTag)
        {
            Tween.Alpha(selectBackground, 1, duration: 0.3f);
        }
        else
        {
            Tween.Alpha(selectBackground, 0, duration: 0.3f);
        }
    }
}
