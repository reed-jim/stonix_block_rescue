using UnityEngine;

public interface IScrew
{
    public void Select();
    public void Move(Vector3 position);
    public void BreakJoint();
    public Transform Transform
    {
        get;
    }
}
