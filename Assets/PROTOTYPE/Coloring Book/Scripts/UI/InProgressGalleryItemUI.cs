using System.Drawing;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Image = UnityEngine.UI.Image;

namespace Saferio.Prototype.ColoringBook
{
    public class InProgressGalleryItemUI : GalleryItemUI
    {
        public override void SetupUI(RectTransform parent, int galleryItemIndex)
        {
            if (!galleryItemDataContainer.Items[galleryItemIndex].IsInProgress)
            {
                gameObject.SetActive(false);

                return;
            }
            else
            {
                gameObject.SetActive(true);
            }

            base.SetupUI(parent, galleryItemIndex);
        }
    }
}
