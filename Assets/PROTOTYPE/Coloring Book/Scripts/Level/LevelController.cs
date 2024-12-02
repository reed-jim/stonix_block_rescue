using System;
using Saferio.Prototype.ColoringBook;
using Saferio.Util.SaferioTween;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [Header("SCRIPTABLE OBJECT")]
    [SerializeField] private CurrentLevelData currentLevelData;

    #region PRIVATE FIELD
    private int _totalNumberOfSegment;
    private int _remainingNumberOfSegment;
    #endregion

    #region ACTION
    public static event Action<CurrentLevelData> spawnSegmentationSpriteEvent;
    public static event Action winLevelEvent;
    #endregion  

    private void Awake()
    {
        SelectColorButton.getColorGroupDataEvent += GetColorGroupData;
        SpriteRegion.fillSpriteRegionEvent += OnSpriteSegmentFilled;

        SaferioTween.DelayAsync(0.5f, onCompletedAction: () =>
        {
            spawnSegmentationSpriteEvent?.Invoke(currentLevelData);
        });
    }

    private void OnDestroy()
    {
        SelectColorButton.getColorGroupDataEvent -= GetColorGroupData;
        SpriteRegion.fillSpriteRegionEvent -= OnSpriteSegmentFilled;
    }

    private void GetColorGroupData(ColorGroupData colorGroupData)
    {
        _totalNumberOfSegment += colorGroupData.NumberOfRegions;
        _remainingNumberOfSegment += colorGroupData.NumberOfRegions - colorGroupData.NumberOfFilledRegions;
    }

    private void OnSpriteSegmentFilled(Color color)
    {
        _remainingNumberOfSegment--;

        Debug.Log(_remainingNumberOfSegment);

        if (_remainingNumberOfSegment <= 65)
        {
            winLevelEvent?.Invoke();
        }
    }
}
