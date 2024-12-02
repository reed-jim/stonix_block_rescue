using UnityEngine;

namespace Saferio.Prototype.ColoringBook
{
    [CreateAssetMenu(menuName = "ScriptableObject/ColoringBook/GalleryItemData")]
    public class GalleryItemData : ScriptableObject
    {
        [SerializeField] private int level;
        [SerializeField] private string spriteAdress;
        [SerializeField] private Sprite outlinedSprite;
        [SerializeField] private GameEnum.GalleryImageTag galleryImageTag;

        public int Level
        {
            get => level;
        }

        public string SpriteAdress
        {
            get => spriteAdress;
        }

        public Sprite OutlinedSprite
        {
            get => outlinedSprite;
        }
    }
}
