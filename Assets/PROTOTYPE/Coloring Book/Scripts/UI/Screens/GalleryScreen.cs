using System.Linq;
using Saferio.Prototype.ColoringBook;
using Saferio.Util;
using Saferio.Util.SaferioTween;
using UnityEngine;

public class GalleryScreen : MonoBehaviour
{
    [SerializeField] private ObjectPoolingScrollView galleryScrollView;

    [Header("SCRIPTABLE OBJECT")]
    [SerializeField] private GalleryItemDataContainer galleryItemDataContainer;

    private GalleryItemUI[] _galleryItems;

    private void Awake()
    {
        SaferioTween.DelayAsync(1f, onCompletedAction: () =>
        {
            _galleryItems = galleryScrollView.Items.Select(item => item.GetComponent<GalleryItemUI>()).ToArray();

            for (int i = 0; i < _galleryItems.Length; i++)
            {
                _galleryItems[i].Setup(galleryItemDataContainer.Items[i]);
            }
        });
    }
}
