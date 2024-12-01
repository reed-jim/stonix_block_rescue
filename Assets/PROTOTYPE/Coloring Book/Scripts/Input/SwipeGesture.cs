using System;
using System.Collections.Generic;
using Saferio.Util.SaferioTween;
using UnityEngine;

public class SwipeGesture : MonoBehaviour
{
    // private List<Vector2> _mousePositions;

    private Vector2 _moveDirection;

    private Vector2 _lastPosition;
    private bool _isLastPositionExist;
    private bool _isDisabled;

    #region ACTION
    public static event Action<Vector2> swipeGestureEvent;
    #endregion

    private void Awake()
    {
        PinchGesture.pinchGestureEvent += TemporaryDisableSwipe;
    }

    private void OnDestroy()
    {
        PinchGesture.pinchGestureEvent -= TemporaryDisableSwipe;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && Input.touchCount == 1 && !_isDisabled)
        {
            Vector2 mousePosition = Input.mousePosition;

            if (!_isLastPositionExist)
            {
                _lastPosition = Input.mousePosition;

                _isLastPositionExist = true;
            }
            else
            {
                _moveDirection = mousePosition - _lastPosition;

                swipeGestureEvent?.Invoke(_moveDirection);

                _lastPosition = mousePosition;
            }
        }
        else
        {
            _isLastPositionExist = false;
        }
    }

    private void TemporaryDisableSwipe(float value)
    {
        _isDisabled = true;

        SaferioTween.DelayAsync(1f, onCompletedAction: () =>
        {
            _isDisabled = false;
        });
    }
}
