using UnityEngine;

public class ColorGroupData
{
    private Color _color;
    private int _numberOfRegions;
    private int _numberOfFilledRegions;

    public Color Color
    {
        get => _color;
        set => _color = value;
    }

    public int NumberOfRegions
    {
        get => _numberOfRegions;
        set => _numberOfRegions = value;
    }

    public int NumberOfFilledRegions
    {
        get => _numberOfFilledRegions;
        set => _numberOfFilledRegions = value;
    }
}
