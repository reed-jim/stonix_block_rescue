using UnityEngine;

public class LevelData
{
    private int _level;
    private SpriteSegmentData[] spriteSegmentsData;
    private ColorGroupData[] colorGroupsData;

    public int Level
    {
        get => _level;
        set => _level = value;
    }

    public SpriteSegmentData[] SpriteSegmentsData
    {
        get => spriteSegmentsData;
        set => spriteSegmentsData = value;
    }

    public ColorGroupData[] ColorGroupsData
    {
        get => colorGroupsData;
        set => colorGroupsData = value;
    }
}
