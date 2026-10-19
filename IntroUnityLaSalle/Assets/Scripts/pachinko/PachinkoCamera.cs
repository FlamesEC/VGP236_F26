using System.Collections;
using UnityEngine;

public class PachinkoCamera : MonoBehaviour
{
    public Transform Target;
    Vector3 startPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position;
        StartCoroutine(camAddition());
    }

    IEnumerator camAddition()
    {
        while (Pachinko.instance == null)
        {
            yield return new WaitForEndOfFrame();
        }

        Pachinko.instance.cam = this;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (Target == null)
            return;

        transform.position = new Vector3(transform.position.x, Mathf.Clamp(Target.transform.position.y, Pachinko.instance.BottomY, Pachinko.instance.TopY), transform.position.z);
    }

    public void RemoveTarget(Transform target)
    {
        if (target != Target)
            return;

        Target = null;
        transform.position = startPos;
    }
}
