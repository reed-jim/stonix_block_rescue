using Saferio.Util.SaferioTween;
using UnityEngine;

public class BasicScrewPort : MonoBehaviour, IScrewPort
{
    [Header("JOINT")]
    [SerializeField] private Rigidbody2D attachedRigidbody;

    public void PutScrew(IScrew screw)
    {
        foreach (var joint in screw.Joints)
        {
            joint.connectedBody = attachedRigidbody;
            joint.enabled = true;
        }

        SaferioTween.PositionAsync(screw.Transform, transform.position, duration: 0.4f, onCompletedAction: (() =>
        {

        }));
    }
}
