using UnityEngine;

public class ColorGroupData
{
    private string _colorString;
    private int _numberOfRegions;
    private int _numberOfFilledRegions;

    public string ColorString
    {
        get => _colorString;
        set => _colorString = value;
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
