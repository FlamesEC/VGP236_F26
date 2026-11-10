using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InheritableVelocityObject : MonoBehaviour
{
    public Vector3 selfVelocity;
    public Vector3 selfRotation { get; private set; }
    Vector3 lastSelfRotation;

    public float groundFriction;
    public float airFriction;

    public bool applyParentRot = true;
    public bool applyNegativeLeaveForces = false;
    public bool considerRot = true;

    [Space(15)]
    [Tooltip("If this momentumLayer is higher than the layer on the colliding momentumLayer, it ignores it")]
    public int layer = 1;

    protected List<InheritableVelocityObject> objectQueue;
    protected List<InheritableVelocityObject> velocityAdditions;
    protected List<InheritableVelocityObject> rotationAdditions;

    public List<Vector3> momentumValues;

    [Space(15)]
    public Rigidbody rb;
    public Collider col;
    public GroundCheck groundCheck;

    // Update is called once per frame
    void Update()
    {
        ObjectUpdate();
    }

    public void SetSelfRotation(Vector3 rot)
    {
        lastSelfRotation = selfRotation;
        selfRotation = rot;
    }

    public void AddSelfRotation(Vector3 rot)
    {
        SetSelfRotation(selfRotation + rot);
    }

    protected virtual void ObjectUpdate() {}

    float ApplyFriction(float value, float friction, float delta)
    {
        if (friction <= 0)
            return 0;

        float sign = Mathf.Sign(value);
        value = Mathf.Abs(value);
        value -= friction * delta;
        
        if (value < 0)
            value = 0;

        return value * sign;
    }

    public Vector3 GetDeltaRotation()
    {
        return new Vector3(
            Mathf.DeltaAngle(lastSelfRotation.x, selfRotation.x),
            Mathf.DeltaAngle(lastSelfRotation.y, selfRotation.y),
            Mathf.DeltaAngle(lastSelfRotation.z, selfRotation.z)
        );
    }

    void FixedUpdate()
    {
        ObjectFixedUpdate();

        for (int i = 0; i < momentumValues.Count; i++)
        {
            float friction = airFriction;
            if (groundCheck != null && groundCheck.GetOnFloor())
                friction = groundFriction;

            momentumValues[i] = new Vector3(
                ApplyFriction(momentumValues[i].x, friction, Time.fixedDeltaTime),
                ApplyFriction(momentumValues[i].y, friction, Time.fixedDeltaTime),
                ApplyFriction(momentumValues[i].z, friction, Time.fixedDeltaTime));
        }

        momentumValues.RemoveAll(x => x == Vector3.zero);

        ApplyVelocity();
        ApplyRotation();
    }

    protected virtual void ObjectFixedUpdate() { }

    public Vector3 GetVelocity()
    {
        Vector3 vel = selfVelocity;

        if (velocityAdditions == null)
            velocityAdditions = new List<InheritableVelocityObject>();
        if (rotationAdditions == null)
            rotationAdditions = new List<InheritableVelocityObject>();

        foreach (InheritableVelocityObject v in velocityAdditions)
            vel += v.GetVelocity();

        foreach (Vector3 v in momentumValues)
            vel += v;

        foreach (InheritableVelocityObject v in rotationAdditions)
        {
            print(v.GetDeltaRotation());

            Vector3 change = v.GetDeltaRotation();
            if (change == Vector3.zero)
                continue;

            Vector3 Center = v.transform.position;
            Vector3 Self = transform.position;

            float yChange = change.y;

            Vector3 offset = transform.position - v.transform.position;
            Vector3 rotatedOffset = Quaternion.Euler(0, yChange, 0) * offset;
            Vector3 displacement = rotatedOffset - offset;
            Vector3 Addition = displacement / Time.fixedDeltaTime;

            vel += Addition;
        }

        return vel;
    }

    public Vector3 GetRotation()
    {
        Vector3 rot = selfRotation;

        if (rotationAdditions == null)
            rotationAdditions = new List<InheritableVelocityObject>();

        foreach (InheritableVelocityObject r in rotationAdditions)
            if (r.considerRot)
                rot += r.GetRotation();

        return rot;
    }

    public void ApplyVelocity()
    {
        if (rb == null)
            return;

        if (rb.isKinematic)
            rb.MovePosition(rb.position + GetVelocity() * Time.fixedDeltaTime);
        else
            rb.linearVelocity = GetVelocity();
    }

    public void ApplyRotation()
    {
        if (applyParentRot)
            transform.localRotation = Quaternion.Euler(GetRotation());
        else
            transform.localRotation = Quaternion.Euler(selfRotation);
    }

    public void CheckCollisionStatus(Collision collision)
    {
        InheritableVelocityObject v = collision.gameObject.GetComponent<InheritableVelocityObject>();

        ContactPoint[] contacts = new ContactPoint[20];
        int count = collision.GetContacts(contacts);

        for (int i = 0; i < count; i++)
        {
            Vector2 normal = contacts[i].normal;

            if (normal.y < 0.5f)
                continue;

            velocityAdditions.Add(v);
            rotationAdditions.Add(v);
            return;
        }

        Rigidbody otherRb = v.rb;

        if (otherRb == null) return;

        Vector3 directionToMe =
            (transform.position - otherRb.position).normalized;

        float dot = Vector3.Dot(otherRb.linearVelocity.normalized, directionToMe);

        if (dot > 0.5f)
            velocityAdditions.Add(v);
    }

    protected void OnCollisionEnter(Collision collision)
    {
        if (velocityAdditions == null)
            velocityAdditions = new List<InheritableVelocityObject>();

        if (objectQueue == null)
            objectQueue = new List<InheritableVelocityObject>();

        if (collision.gameObject.GetComponent<InheritableVelocityObject>() != null)
        {
            InheritableVelocityObject v = collision.gameObject.GetComponent<InheritableVelocityObject>();

            if (layer >= v.layer)
                return;
            if (objectQueue.Contains(v))
                return;

            objectQueue.Add(v);

            CheckCollisionStatus(collision);
        }
    }

    protected void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.GetComponent<InheritableVelocityObject>() == null)
            return;

        InheritableVelocityObject v = collision.gameObject.GetComponent<InheritableVelocityObject>();

        if (!objectQueue.Contains(v))
            return;

        velocityAdditions.Remove(v);
        rotationAdditions.Remove(v);

        CheckCollisionStatus(collision);
    }

    protected void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<InheritableVelocityObject>() != null)
        {
            InheritableVelocityObject v = collision.gameObject.GetComponent<InheritableVelocityObject>();

            if ((groundFriction > 0 || airFriction > 0) && objectQueue.Contains(v) && (v.GetVelocity().y > 0 || applyNegativeLeaveForces))
                momentumValues.Add(Vector3.Scale(v.GetVelocity(), Vector3.up));

            objectQueue.Remove(v);
            velocityAdditions.Remove(v);
            rotationAdditions.Remove(v);
        }
    }
}
