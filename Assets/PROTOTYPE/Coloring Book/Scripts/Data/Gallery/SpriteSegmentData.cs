using UnityEngine;

public class SpriteSegmentData
{
    private string _colorGroup;

    private bool _isFilled;

    public string ColorGroup
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
