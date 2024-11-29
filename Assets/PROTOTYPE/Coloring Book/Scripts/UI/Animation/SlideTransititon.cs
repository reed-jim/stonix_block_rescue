using System;
using PrimeTween;
using Saferio.Prototype.ColoringBook;
using Saferio.Util.SaferioTween;
using UnityEngine;

namespace Saferio.Prototype.ColoringBook
{
    public class SlideTransition : MonoBehaviour, ISaferioUIAnimation
    {
        [SerializeField] private RectTransform target;

        [Header("SCRIPTABLE OBJECT")]
        [SerializeField] private Vector2Variable canvasSize;

        [Header("CUSTOMIZE")]
        [SerializeField] private float duration;

        public void Show()
        {
            target.gameObject.SetActive(true);

            UIUtil.SetLocalPositionX(target, canvasSize.Value.x);

            Tween.LocalPositionX(target, 0, duration: duration);

            // SaferioTween.LocalPositionAsync(target, Vector2.zero, duration: duration);
        }

        public void Hide()
        {
            Tween.LocalPositionX(target, -canvasSize.Value.x, duration: duration).OnComplete(() =>
            {
                target.gameObject.SetActive(true);
            });

            // SaferioTween.LocalPositionAsync(target, new Vector2(-canvasSize.Value.x, 0), duration: duration, onCompletedAction: () =>
            // {
            //     target.gameObject.SetActive(true);
            // });
        }
    }
}
