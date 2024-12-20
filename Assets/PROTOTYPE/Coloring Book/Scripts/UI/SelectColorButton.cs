using System;
using Saferio.Prototype.ColoringBook;
using UnityEngine;
using UnityEngine.UI;

namespace Saferio.Prototype.ColoringBook
{
    public class SelectColorButton : MonoBehaviour
    {
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Button selectButton;

        [SerializeField] private Image buttonImage;
        [SerializeField] private Image progress;

        [SerializeField] private Color _colorGroup;
        [SerializeField] private int _numberOfRegions;
        private int _numberOfFilledRegions;

        public static event Action<Color> selectColorGroupEvent;
        public static event Action<ColorGroupData> getColorGroupDataEvent;

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

        public int NumberOfRegions
        {
            get => _numberOfRegions;
            set => _numberOfRegions = value;
        }

        private void Awake()
        {
            ImageSegmenter.addRegionColorButtonEvent += AddRegion;
            SpriteRegion.fillSpriteRegionEvent += OnSpriteRegionFilled;
            LevelDataManager.incrementNumberOfFilledSegmentOfColorGroupEvent += OnSpriteRegionFilled;

            selectButton.onClick.AddListener(SelectColor);

            progress.fillAmount = 0;

            getColorGroupDataEvent?.Invoke(new ColorGroupData()
            {
                ColorString = _colorGroup.ToString(),
                NumberOfRegions = _numberOfRegions,
                NumberOfFilledRegions = _numberOfFilledRegions
            });
        }

        private void OnDestroy()
        {
            ImageSegmenter.addRegionColorButtonEvent -= AddRegion;
            SpriteRegion.fillSpriteRegionEvent -= OnSpriteRegionFilled;
            LevelDataManager.incrementNumberOfFilledSegmentOfColorGroupEvent -= OnSpriteRegionFilled;
        }

        private void SelectColor()
        {
            selectColorGroupEvent?.Invoke(_colorGroup);
        }

        private void AddRegion(Color color)
        {
            if (color == _colorGroup)
            {
                _numberOfRegions++;
            }
        }

        private void OnSpriteRegionFilled(Color color)
        {
            if (color == _colorGroup)
            {
                _numberOfFilledRegions++;

                // Debug.Log(_numberOfFilledRegions + "/" + _numberOfRegions);

                progress.fillAmount = (float)_numberOfFilledRegions / _numberOfRegions;
            }
        }
    }
}
