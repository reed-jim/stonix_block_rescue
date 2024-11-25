using Saferio.Util.SaferioTween;
using UnityEngine;

public class BasicScrew : MonoBehaviour, IScrew
{
    [SerializeField] private Transform screwSprite;

    [Header("JOINT")]
    [SerializeField] private HingeJoint2D joint;

    [Header("CUSTOMIZE")]
    [SerializeField] private Vector2 offsetOnSelected;
    [SerializeField] private float durationAnimationOnSelected;

    private Vector3 _initialScale;

    public Transform Transform { get => transform; }
    public HingeJoint2D[] Joints { get => new HingeJoint2D[] { joint }; set => joint = value[0]; }

    private void Awake()
    {
        _initialScale = transform.localScale;
    }

    public void Select()
    {
        SaferioTween.LocalPositionAsync(screwSprite, offsetOnSelected, duration: durationAnimationOnSelected);
        // SaferioTween.ScaleAsync(transform, 1.2f * _initialScale, duration: durationAnimationOnSelected);
    }

    public void BreakJoint()
    {
        joint.enabled = false;

        SaferioTween.LocalPositionAsync(screwSprite, Vector3.zero, duration: durationAnimationOnSelected);
    }

    public void Move(Vector3 position)
    {
        // Vector2 direction = position - transform.position;
        // Vector2 force = moveForceMultiplier * direction;
        // screwRigidBody.bodyType = RigidbodyType2D.Dynamic;
        // Debug.Log(force);

        // screwRigidBody.AddForce(force);

        // transform.position = Vector2.Lerp(transform.position, position, moveForceMultiplier);
    }
}
