using UnityEngine;

namespace Saferio.Prototype.ColoringBook
{
    [CreateAssetMenu(menuName = "ScriptableObject/ColoringBook/CurrentLevelData")]
    public class CurrentLevelData : ScriptableObject
    {
        private int _level;
        private Sprite _sprite;

        public int Level
        {
            get => _level;
            set => _level = value;
        }

        public Sprite Sprite
        {
            get => _sprite;
            set => _sprite = value;
        }
    }
}
