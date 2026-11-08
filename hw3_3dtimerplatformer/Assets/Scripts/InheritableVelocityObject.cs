using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class InheritableVelocityObject : MonoBehaviour
{
    public Vector3 selfVelocity;

    [Space(15)]
    [Tooltip("If this momentumLayer is higher than the layer on the colliding momentumLayer, it ignores it")]
    public int layer;
    protected List<InheritableVelocityObject> velocityAdditions;
    protected List<InheritableVelocityObject> velocityQueued;

    [Space(15)]
    public Rigidbody rb;
    public Collider col;

    // Update is called once per frame
    void Update()
    {
        ObjectUpdate();
    }

    protected virtual void ObjectUpdate() {}

    void FixedUpdate()
    {
        ObjectFixedUpdate();
        ApplyVelocity();
    }

    protected virtual void ObjectFixedUpdate() { }

    public Vector3 GetVelocity()
    {
        Vector3 vel = selfVelocity;

        if (velocityAdditions == null)
            velocityAdditions = new List<InheritableVelocityObject>();

        foreach (InheritableVelocityObject v in velocityAdditions)
        {
            vel += v.GetVelocity();
        }

        return vel;
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

    protected void OnCollisionEnter(Collision collision)
    {
        if (velocityAdditions == null)
            velocityAdditions = new List<InheritableVelocityObject>();

        if (velocityQueued == null)
            velocityQueued = new List<InheritableVelocityObject>();

        if (collision.gameObject.GetComponent<InheritableVelocityObject>() != null)
        {
            InheritableVelocityObject v = collision.gameObject.GetComponent<InheritableVelocityObject>();

            if (layer >= v.layer)
                return;
            if (velocityQueued.Contains(v))
                return;

            velocityQueued.Add(v);
        }
    }

    protected void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.GetComponent<InheritableVelocityObject>() == null)
            return;

        InheritableVelocityObject v = collision.gameObject.GetComponent<InheritableVelocityObject>();

        if (!velocityQueued.Contains(v))
            return;

        velocityAdditions.Remove(v);

        ContactPoint[] contacts = new ContactPoint[10];
        int count = collision.GetContacts(contacts);

        for (int i = 0; i < count; i++)
        {
            Vector2 normal = contacts[i].normal;

            if (normal.y < 0.5f)
                continue;

            velocityAdditions.Add(v);
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

    protected void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<InheritableVelocityObject>() != null)
        {
            InheritableVelocityObject v = collision.gameObject.GetComponent<InheritableVelocityObject>();
            velocityQueued.Remove(v);
            velocityAdditions.Remove(v);
        }
    }
}
