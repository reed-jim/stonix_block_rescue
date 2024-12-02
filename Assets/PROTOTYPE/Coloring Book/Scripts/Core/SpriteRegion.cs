using System;
using System.Threading.Tasks;
using PrimeTween;
using Saferio.Prototype.ColoringBook;
using Saferio.Util.SaferioTween;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SpriteRegion : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private BoxCollider2D regionCollider;

    [Header("SCRIPTABLE OBJECT")]
    [SerializeField] private CurrentLevelData currentLevelData;

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
    public static event Action<SpriteRegion> getSpriteRegionEvent;
    public static event Action playSoundFillColorEvent;
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

        LoadSprite();
    }

    private void OnDestroy()
    {
        SelectColorButton.selectColorGroupEvent -= SelectColorButtonPressed;
        LevelDataManager.setSpriteSegmentFilledEvent -= FillColor;
    }

    private async void LoadSprite()
    {
        Sprite originalSprite = await LoadSpriteFromAddressableAsync(currentLevelData.SpriteAdress);

        _segmentIndex = transform.GetSiblingIndex();

        _outlinedSprite = await LoadSpriteFromAddressableAsync($"Outline - {originalSprite.texture.name} {_segmentIndex}");
        _sprite = await LoadSpriteFromAddressableAsync($"Filled - {originalSprite.texture.name} {_segmentIndex}");
        _highlightSprite = await LoadSpriteFromAddressableAsync($"Highlight - {originalSprite.texture.name} {_segmentIndex}");

        spriteRenderer.sprite = _outlinedSprite;

        UpdateCollider();

        regionCollider.enabled = false;

        getSpriteRegionEvent?.Invoke(this);
    }

    private async Task<Sprite> LoadSpriteFromAddressableAsync(string address)
    {
        AsyncOperationHandle<Texture2D> handle = Addressables.LoadAssetAsync<Texture2D>(address);

        await handle.Task;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Texture2D texture = handle.Result;

            return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
        }
        else
        {
            return null;
        }
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

        playSoundFillColorEvent?.Invoke();

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
