using System;
using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using UnityEngine;

public static class TransformUtil
{
    public static void RotateRight(Transform target, List<Tween> tweens, float duration = 0.2f, Action onCompletedAction = null)
    {
        Vector3 currentEulerAngles = target.eulerAngles;

        tweens.Add(
            Tween.Rotation(target, currentEulerAngles + new Vector3(0, 90, 0), duration: duration).OnComplete(() =>
            {
                onCompletedAction?.Invoke();
            })
        );
    }

    public static void RotateLeft(Transform target, List<Tween> tweens, float duration = 0.2f, Action onCompletedAction = null)
    {
        Vector3 currentEulerAngles = target.eulerAngles;

        tweens.Add(
            Tween.Rotation(target, currentEulerAngles - new Vector3(0, 90, 0), duration: duration).OnComplete(() =>
            {
                onCompletedAction?.Invoke();
            })
        );
    }

    public static void RotateBack(Transform target, List<Tween> tweens, float duration = 0.2f, Action onCompletedAction = null)
    {
        Vector3 currentEulerAngles = target.eulerAngles;

        tweens.Add(
            Tween.Rotation(target, currentEulerAngles + new Vector3(0, 180, 0), duration: duration).OnComplete(() =>
            {
                onCompletedAction?.Invoke();
            })
        );
    }

    public static Vector3 GetMaintainedXEulerAngle(Transform target, float initialValue = 0)
    {
        return new Vector3(initialValue, target.eulerAngles.y, target.eulerAngles.z);
    }

    public static T? GetComponentFromParents<T>(Transform child) where T : class
    {
        Transform currentTransform = child.parent;

        while (currentTransform != null)
        {
            T? component = currentTransform.GetComponent<T>();

            if (component != null)
            {
                return component;
            }

            currentTransform = currentTransform.parent;
        }

        return null;
    }

    public static Canvas GetCanvasFromParents(RectTransform rectTransform)
    {
        Transform currentTransform = rectTransform;

        while (currentTransform != null)
        {
            Canvas canvas = currentTransform.GetComponent<Canvas>();

            if (canvas != null)
            {
                return canvas;
            }

            currentTransform = currentTransform.parent;
        }

        return null;
    }
}
