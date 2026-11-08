using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class MomentumableObject : MonoBehaviour
{
    public Vector3 selfVelocity;

    [Space(15)]
    [Tooltip("If this momentumLayer is higher than the layer on the colliding momentumLayer, it ignores it")]
    public int momentumLayer;
    public List<MomentumableObject> momentumAdditions;

    [Space(15)]
    public Rigidbody rb;
    public Collider col;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ApplyMomentum();
    }

    public Vector3 GetVelocity()
    {
        Vector3 vel = selfVelocity;

        foreach (MomentumableObject momentum in momentumAdditions)
            vel += momentum.GetVelocity();

        return vel;
    }

    public void ApplyMomentum()
    {
        rb.linearVelocity = GetVelocity();
    }

    protected void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.GetComponent<MomentumableObject>() != null)
        {
            MomentumableObject m = collision.gameObject.GetComponent<MomentumableObject>();

            if (momentumLayer < m.momentumLayer)
                momentumAdditions.Add(collision.gameObject.GetComponent<MomentumableObject>());
        }
    }

    protected void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.GetComponent<MomentumableObject>() != null)
        {
            momentumAdditions.Remove(collision.gameObject.GetComponent<MomentumableObject>());
        }
    }
}
