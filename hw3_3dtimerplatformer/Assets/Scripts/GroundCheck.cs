using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public Dictionary<Collider, List<ContactPoint>> contactPoints;

    private void Awake()
    {
        contactPoints = new Dictionary<Collider, List<ContactPoint>>();
    }

    public bool GetOnFloor()
    {
        foreach (List<ContactPoint> c in contactPoints.Values)
        {
            for (int i = 0; i < c.Count; i++)
            {
                Vector2 normal = c[i].normal;

                if (normal.y < 0.5f)
                    continue;

                return true;
            }
        }

        return false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] contacts = new ContactPoint[10];
        int count = collision.GetContacts(contacts);

        contactPoints[collision.collider] = new List<ContactPoint>(contacts);
    }

    private void OnCollisionStay(Collision collision)
    {
        if (!contactPoints.ContainsKey(collision.collider))
            return;

        ContactPoint[] contacts = new ContactPoint[10];
        int count = collision.GetContacts(contacts);

        contactPoints[collision.collider] = new List<ContactPoint>(contacts);
    }

    private void OnCollisionExit(Collision collision)
    {
        contactPoints.Remove(collision.collider);
    }
}
