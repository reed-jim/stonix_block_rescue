using UnityEngine;

namespace Saferio.Prototype.ColoringBook
{
    [CreateAssetMenu(menuName = "ScriptableObject/ColoringBook/CurrentLevelData")]
    public class CurrentLevelData : ScriptableObject
    {
        [SerializeField] private int level;
        private string _spriteAdress;

        public int Level
        {
            get => level;
            set => level = value;
        }

        public string SpriteAdress
        {
            get => _spriteAdress;
            set => _spriteAdress = value;
        }
    }
}
