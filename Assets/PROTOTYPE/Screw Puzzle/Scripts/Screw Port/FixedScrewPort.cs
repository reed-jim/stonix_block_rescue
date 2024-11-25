using Saferio.Util.SaferioTween;
using UnityEngine;

public class FixedScrewPort : MonoBehaviour, IScrewPort
{
    public void PutScrew(IScrew screw)
    {
        Vector3 destination = transform.position;
        destination.z = screw.Transform.position.z;

        SaferioTween.PositionAsync(screw.Transform, destination, duration: 0.4f);
    }
}
