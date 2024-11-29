using UnityEngine;

public class GameVariableInitilizer : MonoBehaviour
{
    [SerializeField] private RectTransform canvas;

    [SerializeField] private Vector2Variable canvasSize;

    private void Awake()
    {
        canvasSize.Value = canvas.sizeDelta;
    }
}
