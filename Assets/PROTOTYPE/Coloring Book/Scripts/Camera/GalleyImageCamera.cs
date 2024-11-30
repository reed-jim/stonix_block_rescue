using UnityEngine;

namespace Saferio.Prototype.ColoringBook
{
    public class GalleyImageCamera : MonoBehaviour
    {
        [SerializeField] private Camera imageCamera;
        [SerializeField] private Transform imageCameraTransform;

        [Header("CUSTOMIZE")]
        [SerializeField] private float cameraMoveSpeedMultiplier;

        [SerializeField] private CurrentLevelData currentLevelData;

        private void Awake()
        {
            SwipeGesture.swipeGestureEvent += MoveCamera;

            FitTheGalleryImage();
        }

        private void OnDestroy()
        {
            SwipeGesture.swipeGestureEvent -= MoveCamera;
        }

        private void FitTheGalleryImage()
        {
            imageCamera.orthographicSize = 1.2f * currentLevelData.Sprite.bounds.size.x;
        }

        private void MoveCamera(Vector2 moveDirection)
        {
            imageCameraTransform.position -= cameraMoveSpeedMultiplier * (Vector3)moveDirection;
        }
    }
}
