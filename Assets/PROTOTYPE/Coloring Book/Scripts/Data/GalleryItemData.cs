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
        [SerializeField] private bool isInProgress;

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

        public bool IsInProgress
        {
            get => isInProgress;
            set => isInProgress = value;
        }
    }
}
