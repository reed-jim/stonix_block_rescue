using UnityEngine;

namespace Saferio.Prototype.ColoringBook
{
    [CreateAssetMenu(menuName = "ScriptableObject/ColoringBook/GalleryItemDataContainer")]
    public class GalleryItemDataContainer : ScriptableObject
    {
        [SerializeField] private GalleryItemData[] items;

        public GalleryItemData[] Items
        {
            get => items;
        }

        public void SetGalleryItemInProgress(int index, bool isInProgress)
        {
            items[index].IsInProgress = isInProgress;
        }
    }
}
