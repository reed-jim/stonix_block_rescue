using Saferio.Prototype.ColoringBook;
using UnityEngine;

public class PairGalleryItemUI : MonoBehaviour, ISaferioScrollViewItem
{
    [SerializeField] private RectTransform container;

    [Header("SCRIPTABLE OBJECT")]
    [SerializeField] private GalleryItemDataContainer galleryItemDataContainer;

    [SerializeField] private GalleryItemUI[] _pairGalleryItem;

    private GalleryItemData[] _pairGalleryItemData;

    public void Setup(int index, RectTransform parent)
    {
        _pairGalleryItemData = new GalleryItemData[2];

        _pairGalleryItemData[0] = galleryItemDataContainer.Items[index * 2];
        _pairGalleryItemData[1] = galleryItemDataContainer.Items[index * 2 + 1];

        for (int i = 0; i < _pairGalleryItem.Length; i++)
        {
            _pairGalleryItem[i].Setup(_pairGalleryItemData[i]);
        }

        GenerateUI(parent);
    }

    public void Refresh(int index)
    {
        _pairGalleryItemData = new GalleryItemData[2];

        _pairGalleryItemData[0] = galleryItemDataContainer.Items[index * 2];
        _pairGalleryItemData[1] = galleryItemDataContainer.Items[index * 2 + 1];

        for (int i = 0; i < _pairGalleryItem.Length; i++)
        {
            _pairGalleryItem[i].Setup(_pairGalleryItemData[i]);
        }
    }

    private void GenerateUI(RectTransform parent)
    {
        UIUtil.SetSize(container, parent.sizeDelta.x, 0.2f * parent.sizeDelta.y);

        for (int i = 0; i < _pairGalleryItem.Length; i++)
        {
            int indexInParent = i;

            _pairGalleryItem[i].SetupUI(container, indexInParent);
        }
    }
}
