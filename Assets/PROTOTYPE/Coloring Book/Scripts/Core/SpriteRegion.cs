using UnityEngine;

public class SpriteRegion : MonoBehaviour
{
    [SerializeField] private SpriteRenderer SpriteRenderer;
    private Sprite _highlightSprite;
    private Sprite _sprite;

    [SerializeField] private Color _colorGroup;

    public Sprite HighlightSprite
    {
        get => _highlightSprite;
        set => _highlightSprite = value;
    }

    public Sprite Sprite
    {
        get => _sprite;
        set => _sprite = value;
    }

    public Color ColorGroup
    {
        get => _colorGroup;
        set => _colorGroup = value;
    }

    private void Awake()
    {
        SelectColorButton.selectColorGroupEvent += SelectColorButtonPressed;
    }

    private void OnDestroy()
    {
        SelectColorButton.selectColorGroupEvent -= SelectColorButtonPressed;
    }

    public void FillColor()
    {
        SpriteRenderer.sprite = _sprite;
    }

    private void SelectColorButtonPressed(Color color)
    {
        if (color == _colorGroup)
        {
            FillColor();
        }
    }
}
