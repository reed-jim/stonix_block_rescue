using UnityEngine;

public class SpriteRegion : MonoBehaviour
{
    [SerializeField] private SpriteRenderer SpriteRenderer;
    private Sprite _highlightSprite;
    private Sprite _sprite;

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

    public void FillColor()
    {
        SpriteRenderer.sprite = _sprite;
    }
}
