using Saferio.Prototype.ColoringBook;
using UnityEngine;

public class BottomBar : MonoBehaviour
{
    [SerializeField] private RectTransform[] switchRouteButtonRTs;

    [SerializeField] private Vector2Variable canvasSize;

    private void Awake()
    {
        GenerateUI();
    }

    private void GenerateUI()
    {
        for (int i = 0; i < switchRouteButtonRTs.Length; i++)
        {
            UIUtil.SetLocalPositionX(switchRouteButtonRTs[i], (-(switchRouteButtonRTs.Length - 1) / 2f + i) * (1.7f * switchRouteButtonRTs[i].sizeDelta.x));
        }
    }
}
