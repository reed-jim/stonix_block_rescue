using System.Collections.Generic;
using UnityEngine;

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
            UIUtil.SetLocalPositionX(_colorButtons[i].RectTransform, -canvas.sizeDelta.x + i * 1.1f * buttonSize.x);
        }
    }
}
