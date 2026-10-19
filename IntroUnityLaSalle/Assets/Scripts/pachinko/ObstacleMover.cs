using UnityEngine;

public class ObstacleMover : MonoBehaviour
{
    public Vector3 StartPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartPos = transform.position;    
    }

    public virtual Vector3 GetNewPos()
    {
        return StartPos;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = GetNewPos();
    }
}
