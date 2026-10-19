using UnityEngine;

public class PachinkoCamera : MonoBehaviour
{
    public Transform Target;
    public float TopY;
    public float BottomY;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Target == null)
            return;

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(Target.transform.position.y, BottomY, TopY), transform.position.z);
    }
}
