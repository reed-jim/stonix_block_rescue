using UnityEngine;

namespace Saferio.Prototype.ColoringBook
{
    [CreateAssetMenu(menuName = "ScriptableObject/ColoringBook/GalleryItemData")]
    public class GalleryItemData : ScriptableObject
    {
        [SerializeField] private Sprite sprite;
        

        public Sprite Sprite
        {
            get => sprite;
        }
    }
}
