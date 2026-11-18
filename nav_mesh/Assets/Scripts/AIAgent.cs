using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIAgent : MonoBehaviour
{
    public AIAgentTarget target;
    NavMeshAgent agent;

    public List<Vector3> queuedNodeTargets;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        queuedNodeTargets = PathfindingManager.instance.GetPathToPosition(transform.position, target.transform.position);
    }

    // Update is called once per frame
    void Update()
    {
        if (queuedNodeTargets == null || queuedNodeTargets.Count <= 0)
        {
            if (Mathf.Abs((Vector3.Scale(transform.position, new Vector3(1, 0, 1)) - Vector3.Scale(target.transform.position, new Vector3(1, 0, 1))).magnitude) > 0.0f)
                queuedNodeTargets = PathfindingManager.instance.GetPathToPosition(transform.position, target.transform.position);

            return; 
        }

        if (Mathf.Abs((target.transform.position - queuedNodeTargets[queuedNodeTargets.Count - 1]).magnitude) > 0.0f)
            queuedNodeTargets = PathfindingManager.instance.GetPathToPosition(transform.position, target.transform.position);   

        if (Mathf.Abs((Vector3.Scale(transform.position, new Vector3(1, 0, 1)) - Vector3.Scale(queuedNodeTargets[0], new Vector3(1, 0, 1))).magnitude) > 0.0f)
            agent.SetDestination(queuedNodeTargets[0]);
        else
            queuedNodeTargets.RemoveAt(0);
    }
}
