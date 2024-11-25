using Saferio.Util.SaferioTween;
using UnityEngine;

public class FixedScrewPort : MonoBehaviour, IScrewPort
{
    public void PutScrew(IScrew screw)
    {
        SaferioTween.PositionAsync(screw.Transform, transform.position, duration: 0.4f);
    }
}
