using System.Collections.Generic;
using UnityEngine;

namespace Saferio.Prototype.ColoringBook
{
    public class ColorButtonSpawner : MonoBehaviour
    {
        [SerializeField] private RectTransform canvas;
        [SerializeField] private SelectColorButton colorButtonPrefab;
        [SerializeField] private RectTransform container;

        private List<SelectColorButton> _colorButtons;

        private void Awake()
        {
            ImageSegmenter.spawnColorButtonEvent += SpawnColorButton;

            _colorButtons = new List<SelectColorButton>();
        }

        private void OnDestroy()
        {
            ImageSegmenter.spawnColorButtonEvent -= SpawnColorButton;
        }

        public void SpawnColorButtons(SpriteRegion[] spriteRegions)
        {
            _colorButtons = new List<SelectColorButton>();

            for (int i = 0; i < spriteRegions.Length; i++)
            {
                bool isColorGroupExist = false;

                for (int j = 0; j < _colorButtons.Count; j++)
                {
                    if (_colorButtons[j].ColorGroup == spriteRegions[i].ColorGroup)
                    {
                        _colorButtons[j].NumberOfRegions++;

                        isColorGroupExist = true;

                        break;
                    }
                }

                if (!isColorGroupExist)
                {
                    SpawnColorButton(spriteRegions[i].ColorGroup);
                }
            }
        }

        private void SpawnColorButton(Color color)
        {
            SelectColorButton selectColorButton = Instantiate(colorButtonPrefab, container);

            selectColorButton.ButtonImage.color = color;
            selectColorButton.ColorGroup = color;
            selectColorButton.NumberOfRegions = 1;

            _colorButtons.Add(selectColorButton);

            GenerateUI();
        }

        private void GenerateUI()
        {
            Vector2 buttonSize = _colorButtons[0].RectTransform.sizeDelta;

            for (int i = 0; i < _colorButtons.Count; i++)
            {
                UIUtil.SetLocalPositionX(_colorButtons[i].RectTransform, -0.45f * canvas.sizeDelta.x + i * 1.2f * buttonSize.x);
            }
        }
    }
}
