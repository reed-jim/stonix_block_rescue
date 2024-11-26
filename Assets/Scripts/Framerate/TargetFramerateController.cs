using UnityEngine;

public class TargetFramerateController : MonoBehaviour
{
    private void Awake()
    {
        Application.targetFrameRate = 60;
    }
}
