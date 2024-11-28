using System.Drawing;
using UnityEngine;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

namespace Saferio.Prototype.ColoringBook
{
    public class GalleryItemUI : MonoBehaviour
    {
        [SerializeField] private Image image;
        [SerializeField] private Button playButton;

        [SerializeField] private CurrentLevelData currentLevelData;

        private GalleryItemData _galleryItemData;

        public void Setup(GalleryItemData galleryItemData)
        {
            image.sprite = galleryItemData.Sprite;

            _galleryItemData = galleryItemData;
        }

        private void Play()
        {
            currentLevelData.Sprite = _galleryItemData.Sprite;
        }
    }
}
