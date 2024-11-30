using System.Collections.Generic;
using Saferio.Prototype.ColoringBook;
using UnityEngine;

namespace Saferio.Prototype.ColoringBook
{
    public class LevelDataManager : MonoBehaviour
    {
        [SerializeField] private CurrentLevelData currentLevelData;

        private void Awake()
        {
            ImageSegmenter.saveSpriteRegionsEvent += SaveSpriteSegmentsData;
        }

        private void OnDestroy()
        {
            ImageSegmenter.saveSpriteRegionsEvent -= SaveSpriteSegmentsData;
        }

        private void SaveSpriteSegmentsData(List<SpriteRegion> spriteRegions, ColorGroupData[] colorGroupsData)
        {
            LevelData levelData = DataUtility.Load<LevelData>(GameConstant.SAVE_FILE_NAME, $"Level_{currentLevelData.Level}_Data", null);

            if (levelData == null)
            {
                levelData.Level = currentLevelData.Level;

                levelData.SpriteSegmentsData = new SpriteSegmentData[spriteRegions.Count];

                for (int i = 0; i < spriteRegions.Count; i++)
                {
                    levelData.SpriteSegmentsData[i].ColorGroup = spriteRegions[i].ColorGroup;
                    levelData.SpriteSegmentsData[i].IsFilled = spriteRegions[i].IsFilled;
                }

                levelData.ColorGroupsData = colorGroupsData;

                DataUtility.Save<LevelData>(GameConstant.SAVE_FILE_NAME, $"Level_{currentLevelData.Level}_Data", levelData);
            }
        }
    }
}
