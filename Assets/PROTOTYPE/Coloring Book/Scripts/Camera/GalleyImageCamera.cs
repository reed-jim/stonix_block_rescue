using PrimeTween;
using Saferio.Util.SaferioTween;
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

        #region PRIVATE FIELD
        private Sprite _originalSprite;
        #endregion

        private void Awake()
        {
            SwipeGesture.swipeGestureEvent += MoveCamera;
            PinchGesture.pinchGestureEvent += ZoomCamera;
            LevelController.winLevelEvent += OnWinLevel;

            FitTheGalleryImage();
        }

        private void OnDestroy()
        {
            SwipeGesture.swipeGestureEvent -= MoveCamera;
            PinchGesture.pinchGestureEvent -= ZoomCamera;
            LevelController.winLevelEvent -= OnWinLevel;
        }

        private async void FitTheGalleryImage()
        {
            _originalSprite = await AssetUtil.LoadSpriteFromAddressableAsync(currentLevelData.SpriteAdress);

            imageCamera.orthographicSize = 1.2f * _originalSprite.bounds.size.x;
        }

        private void MoveCamera(Vector2 moveDirection)
        {
            imageCameraTransform.position -= cameraMoveSpeedMultiplier * (Vector3)moveDirection;
        }

        private void ZoomCamera(float zoomValue)
        {
            imageCamera.orthographicSize = zoomValue;
        }

        private void OnWinLevel()
        {
            SaferioTween.PositionAsync(imageCameraTransform, new Vector3(0, 0, imageCameraTransform.position.z), duration: 0.5f);
            Tween.CameraOrthographicSize(imageCamera, 1.1f * _originalSprite.bounds.size.x, duration: 0.5f);
        }
    }
}
