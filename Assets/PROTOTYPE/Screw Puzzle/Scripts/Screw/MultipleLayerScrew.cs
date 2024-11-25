using System.Collections.Generic;
using Saferio.Util.SaferioTween;
using UnityEngine;

public class MultipleLayerScrew : MonoBehaviour, IScrew
{
    [SerializeField] private Rigidbody2D screwRigidBody;

    [Header("JOINT")]
    [SerializeField] private HingeJoint2D[] joints;

    [Header("CUSTOMIZE")]
    [SerializeField] private Vector2 offsetOnSelected;
    [SerializeField] private float durationAnimationOnSelected;

    private Vector3 _initialScale;

    public Transform Transform { get => transform; }
    public HingeJoint2D[] Joints { get => joints; set => joints = value; }

    private void Awake()
    {
        joints = GetComponents<HingeJoint2D>();

        _initialScale = transform.localScale;
    }

    public void Select()
    {
        SaferioTween.PositionAsync(transform, transform.position + (Vector3)offsetOnSelected, duration: durationAnimationOnSelected);
        // SaferioTween.ScaleAsync(transform, 1.2f * _initialScale, duration: durationAnimationOnSelected);
    }

    public void BreakJoint()
    {
        foreach (var joint in joints)
        {
            joint.enabled = false;
        }
    }

    public void Move(Vector3 position)
    {

    }
}
