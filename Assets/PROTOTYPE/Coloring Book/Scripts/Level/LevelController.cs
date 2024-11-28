using System;
using Saferio.Prototype.ColoringBook;
using Saferio.Util.SaferioTween;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    [Header("SCRIPTABLE OBJECT")]
    [SerializeField] private CurrentLevelData currentLevelData;

    #region ACTION
    public static event Action<CurrentLevelData> spawnSegmentationSpriteEvent;
    #endregion  

    private void Awake()
    {
        SaferioTween.DelayAsync(0.5f, onCompletedAction: () =>
        {
            spawnSegmentationSpriteEvent?.Invoke(currentLevelData);
        });
    }
}
