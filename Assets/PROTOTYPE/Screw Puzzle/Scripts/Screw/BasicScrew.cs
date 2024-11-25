using UnityEngine;

public class BasicScrew : MonoBehaviour, IScrew
{
    [SerializeField] private Rigidbody2D screwRigidBody;

    [Header("JOINT")]
    [SerializeField] private HingeJoint2D joint;

    [Header("CUSTOMIZE")]
    [SerializeField] private float moveForceMultiplier;

    public void BreakJoint()
    {
        joint.enabled = false;
    }

    public void Move(Vector3 position)
    {
        // Vector2 direction = position - transform.position;
        // Vector2 force = moveForceMultiplier * direction;
        // screwRigidBody.bodyType = RigidbodyType2D.Dynamic;
        // Debug.Log(force);

        // screwRigidBody.AddForce(force);

        transform.position = Vector2.Lerp(transform.position, position, moveForceMultiplier);
    }
}
