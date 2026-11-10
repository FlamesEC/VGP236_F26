using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform rootTarget;
    public Transform finalTarget;

    public LayerMask blockCameraLayers;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 pos = finalTarget.position;
        
        Vector3 dir = finalTarget.position - rootTarget.position;
        float dis = dir.magnitude;
        dir = dir.normalized;

        RaycastHit hit;

        if (Physics.Raycast(rootTarget.position, dir, out hit, dis, blockCameraLayers, QueryTriggerInteraction.Ignore))
            pos = hit.point;

        transform.position = pos;
        transform.rotation = finalTarget.rotation;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(rootTarget.position, 0.5f);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(finalTarget.position, 0.5f);
        Gizmos.color = Color.aquamarine;
        Gizmos.DrawLine(rootTarget.position, finalTarget.position);
        
        if (transform.position != finalTarget.position)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, 0.25f);
        }
    }
}
