using UnityEngine;

public class ObstacleMover : MonoBehaviour
{
    protected Vector3 StartPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
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
