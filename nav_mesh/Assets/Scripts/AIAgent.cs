using UnityEngine;
using UnityEngine.AI;

public class AIAgent : MonoBehaviour
{
    public AIAgentTarget target;
    NavMeshAgent agent;
    private Vector3 lastTarget;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Mathf.Abs((lastTarget - target.transform.position).magnitude) > 0.0f)
        {
            agent.SetDestination(target.transform.position);
            lastTarget = target.transform.position;
        }
    }
}
