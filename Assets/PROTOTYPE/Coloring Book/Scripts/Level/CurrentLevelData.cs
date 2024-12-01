using UnityEngine;

namespace Saferio.Prototype.ColoringBook
{
    [CreateAssetMenu(menuName = "ScriptableObject/ColoringBook/CurrentLevelData")]
    public class CurrentLevelData : ScriptableObject
    {
        [SerializeField] private int level;
        private Sprite _sprite;

        public int Level
        {
            get => level;
            set => level = value;
        }

        public Sprite Sprite
        {
            get => _sprite;
            set => _sprite = value;
        }
    }
}
