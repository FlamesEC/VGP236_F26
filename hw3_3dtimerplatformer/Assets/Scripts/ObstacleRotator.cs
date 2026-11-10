using UnityEngine;

public class ObstacleRotator : InheritableVelocityObject
{
    protected Vector3 StartRot;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        StartRot = transform.localRotation.eulerAngles;
    }

    public virtual Vector3 GetNewRot()
    {
        return StartRot;
    }

    protected override void ObjectFixedUpdate()
    {
        SetSelfRotation(GetNewRot());
    }
}
