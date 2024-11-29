using System.Drawing;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

namespace Saferio.Prototype.ColoringBook
{
    public class GalleryItemUI : MonoBehaviour
    {
        [SerializeField] private RectTransform container;
        [SerializeField] private RectTransform imageRT;
        [SerializeField] private Image image;
        [SerializeField] private Button playButton;

        [SerializeField] private CurrentLevelData currentLevelData;

        private GalleryItemData _galleryItemData;

        public RectTransform Container
        {
            get => container;
            set => container = value;
        }

        private void Awake()
        {
            playButton.onClick.AddListener(Play);
        }

        public void Setup(GalleryItemData galleryItemData)
        {
            image.sprite = galleryItemData.OutlinedSprite;

            _galleryItemData = galleryItemData;
        }

        public void SetupUI(RectTransform parent, int indexInParent)
        {
            UIUtil.SetSize(container, 0.45f * parent.sizeDelta.x, parent.sizeDelta.y);
            UIUtil.SetSizeKeepRatioX(imageRT, 1.2f * container.sizeDelta.y);

            float distanceBetweenItem = 0.05f * container.sizeDelta.x;

            UIUtil.SetLocalPositionX(container, (-(2 - 1) / 2f + indexInParent) * (container.sizeDelta.x + distanceBetweenItem));
        }

        private void GenerateUI()
        {

        }

        private void Play()
        {
            currentLevelData.Sprite = _galleryItemData.Sprite;

            SceneManager.LoadSceneAsync("Coloring Book - Gameplay");
        }
    }
}
