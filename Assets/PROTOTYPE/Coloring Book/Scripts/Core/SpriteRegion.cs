using System;
using PrimeTween;
using Saferio.Prototype.ColoringBook;
using Saferio.Util.SaferioTween;
using UnityEngine;

public class SpriteRegion : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoxCollider2D regionCollider;

    private int _segmentIndex;
    private Sprite _outlinedSprite;
    private Sprite _sprite;
    private Sprite _highlightSprite;

    [SerializeField] private Color _colorGroup;

    #region PRIVATE FIELD
    private bool _isFilled;
    #endregion  

    #region ACTION
    public static event Action<Color> fillSpriteRegionEvent;
    public static event Action<int> fillSpriteSegmentEvent;
    #endregion

    public int SegmentIndex
    {
        get => _segmentIndex;
        set => _segmentIndex = value;
    }

    public Sprite OutlinedSprite
    {
        get => _outlinedSprite;
        set => _outlinedSprite = value;
    }

    public Sprite Sprite
    {
        get => _sprite;
        set => _sprite = value;
    }

    public Sprite HighlightSprite
    {
        get => _highlightSprite;
        set => _highlightSprite = value;
    }

    public Color ColorGroup
    {
        get => _colorGroup;
        set => _colorGroup = value;
    }

    public bool IsFilled
    {
        get => _isFilled;
        set => _isFilled = value;
    }

    private void Awake()
    {
        SelectColorButton.selectColorGroupEvent += SelectColorButtonPressed;
        LevelDataManager.setSpriteSegmentFilledEvent += FillColor;
    }

    private void OnDestroy()
    {
        SelectColorButton.selectColorGroupEvent -= SelectColorButtonPressed;
        LevelDataManager.setSpriteSegmentFilledEvent -= FillColor;
    }

    public void Setup(Sprite outlinedSprite, Sprite filledSprite, Sprite highlightSprite)
    {
        _outlinedSprite = outlinedSprite;
        _sprite = filledSprite;
        _highlightSprite = highlightSprite;

        spriteRenderer.sprite = _outlinedSprite;

        UpdateCollider();

        regionCollider.enabled = false;
    }

    public void FillColor()
    {
        if (_isFilled)
        {
            return;
        }

        spriteRenderer.sprite = _sprite;

        spriteRenderer.color = ColorUtil.WithAlpha(spriteRenderer.color, 0);

        Tween.Alpha(spriteRenderer, 1, duration: 0.3f);

        fillSpriteRegionEvent?.Invoke(_colorGroup);
        fillSpriteSegmentEvent?.Invoke(_segmentIndex);

        regionCollider.enabled = false;

        _isFilled = true;
    }

    public void FillColor(int segmentIndex)
    {
        if (segmentIndex == _segmentIndex)
        {
            FillColor();
        }
    }

    private void SelectColorButtonPressed(Color color)
    {
        if (_isFilled)
        {
            return;
        }

        if (color == _colorGroup)
        {
            spriteRenderer.sprite = _highlightSprite;

            regionCollider.enabled = true;
        }
        else
        {
            spriteRenderer.sprite = _outlinedSprite;

            regionCollider.enabled = false;
        }
    }

    public void UpdateCollider()
    {
        if (spriteRenderer != null && regionCollider != null)
        {
            regionCollider.size = spriteRenderer.bounds.size;

            regionCollider.offset = spriteRenderer.bounds.center - transform.position;
        }
    }
}
