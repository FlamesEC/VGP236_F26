using UnityEngine;

public class ObstacleMover : InheritableVelocityObject
{
    protected Vector3 StartSpeed;
    protected Vector3 StartPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        StartPos = transform.position;
        SetSelfRotation(transform.rotation.eulerAngles);
        SetSelfRotation(transform.rotation.eulerAngles);
    }

    public virtual Vector3 GetNewSpeed()
    {
        return StartSpeed;
    }

    // Update is called once per frame
    protected override void ObjectUpdate()
    {
        selfVelocity = GetNewSpeed();
    }
}
