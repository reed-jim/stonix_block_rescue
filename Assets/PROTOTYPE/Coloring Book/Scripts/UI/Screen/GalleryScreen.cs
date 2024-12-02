using System;
using System.Linq;
using Saferio.Prototype.ColoringBook;
using Saferio.Util;
using Saferio.Util.SaferioTween;
using UnityEngine;
using UnityEngine.UI;
using static Saferio.Prototype.ColoringBook.GameEnum;

namespace Saferio.Prototype.ColoringBook
{
    public class GalleryScreen : UIScreen
    {
        [SerializeField] private ObjectPoolingScrollView galleryScrollView;

        [Header("SCRIPTABLE OBJECT")]
        [SerializeField] private GalleryItemDataContainer galleryItemDataContainer;

        private GalleryItemUI[] _galleryItems;
    }
}
