using UnityEngine;

namespace Saferio.Prototype.ColoringBook
{
    [CreateAssetMenu(menuName = "ScriptableObject/ColoringBook/GalleryItemData")]
    public class GalleryItemData : ScriptableObject
    {
        [SerializeField] private Sprite sprite;
        [SerializeField] private Sprite outlinedSprite;

        public Sprite Sprite
        {
            get => sprite;
        }

        public Sprite OutlinedSprite
        {
            get => outlinedSprite;
        }
    }
}
