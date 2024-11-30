using System;
using System.Collections.Generic;
using Saferio.Prototype.ColoringBook;
using UnityEngine;

namespace Saferio.Prototype.ColoringBook
{
    public class LevelDataManager : MonoBehaviour
    {
        [SerializeField] private CurrentLevelData currentLevelData;

        #region PRIVATE FIELD
        private LevelData _levelData;
        #endregion

        #region ACTION
        public static event Action<int> setSpriteSegmentFilledEvent;
        public static event Action<Color> incrementNumberOfFilledSegmentOfColorGroupEvent;
        #endregion

        private void Awake()
        {
            ImageSegmenter.saveSpriteRegionsEvent += SaveSpriteSegmentsData;
            SpriteRegion.fillSpriteSegmentEvent += OnSpriteSegmentFilled;
        }

        private void OnDestroy()
        {
            ImageSegmenter.saveSpriteRegionsEvent -= SaveSpriteSegmentsData;
            SpriteRegion.fillSpriteSegmentEvent -= OnSpriteSegmentFilled;
        }

        private void OnApplicationQuit()
        {
            SaveLevelData();
        }

        private void SaveSpriteSegmentsData(List<SpriteRegion> spriteRegions, ColorGroupData[] colorGroupsData)
        {
            LevelData levelData = DataUtility.Load<LevelData>(GameConstant.SAVE_FILE_NAME, $"Level_{currentLevelData.Level}_Data", null);

            if (levelData == null)
            {
                levelData = new LevelData();

                levelData.Level = currentLevelData.Level;

                levelData.SpriteSegmentsData = new SpriteSegmentData[spriteRegions.Count];

                for (int i = 0; i < spriteRegions.Count; i++)
                {
                    levelData.SpriteSegmentsData[i] = new SpriteSegmentData();

                    levelData.SpriteSegmentsData[i].ColorGroup = spriteRegions[i].ColorGroup.ToString();
                    levelData.SpriteSegmentsData[i].IsFilled = spriteRegions[i].IsFilled;
                }

                levelData.ColorGroupsData = colorGroupsData;

                DataUtility.Save(GameConstant.SAVE_FILE_NAME, $"Level_{currentLevelData.Level}_Data", levelData);
            }
            else
            {
                for (int i = 0; i < levelData.SpriteSegmentsData.Length; i++)
                {
                    if (levelData.SpriteSegmentsData[i].IsFilled)
                    {
                        int segmentIndex = i;

                        setSpriteSegmentFilledEvent?.Invoke(segmentIndex);

                        incrementNumberOfFilledSegmentOfColorGroupEvent?.Invoke(ColorStringToColor(levelData.SpriteSegmentsData[i].ColorGroup));
                    }
                }
            }

            _levelData = levelData;
        }

        private void OnSpriteSegmentFilled(int segmentIndex)
        {
            if (_levelData == null)
            {
                return;
            }

            _levelData.SpriteSegmentsData[segmentIndex].IsFilled = true;
        }

        private void SaveLevelData()
        {
            DataUtility.Save(GameConstant.SAVE_FILE_NAME, $"Level_{currentLevelData.Level}_Data", _levelData);
        }

        private Color ColorStringToColor(string colorString)
        {
            string cleanedString = colorString.Replace("RGBA(", "").Replace(")", "");

            string[] rgbaValues = cleanedString.Split(',');

            float r = float.Parse(rgbaValues[0]);
            float g = float.Parse(rgbaValues[1]);
            float b = float.Parse(rgbaValues[2]);
            float a = float.Parse(rgbaValues[3]);

            return new Color(r, g, b, a);
        }
    }
}