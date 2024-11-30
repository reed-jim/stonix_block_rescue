using UnityEngine;

public class SpriteSegmentData
{
    private Color _colorGroup;

    private bool _isFilled;

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
}
