using System;
using Unity.VisualScripting;
using UnityEngine;

public class NodeObstacle : ObstacleMover
{
    [Serializable]
    public class NodeObject
    {
        public Transform node;
        [Range(0, 10)]
        public float secondsToWait;
    }

    public NodeObject[] nodes;

    public float timeOffset;
    public float speed = 10;

    public float timeProgress = 0;
    public float timeConstantLength = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartPos = GetStartPos();
        transform.position = StartPos;

        timeConstantLength = GetTimeConstant();
        timeProgress = timeOffset;
    }

    public float GetTimeConstant()
    {
        float t = 0;

        foreach (NodeObject n in nodes)
            t += n.secondsToWait;
        for (int i = 0; i < nodes.Length; i++)
        {
            int nextNodeIndex = (i + 1) % nodes.Length;

            t += Vector3.Distance(nodes[i].node.position, nodes[nextNodeIndex].node.position) / speed;
        }

        return t;
    }

    public Vector3 GetStartPos()
    {
        float t = timeOffset;

        int currentIndex = 0;

        while (true)
        {
            NodeObject currentNode = nodes[currentIndex];
            NodeObject nextNode = nodes[(currentIndex + 1) %  nodes.Length];

            float timeToNext = Vector3.Distance(currentNode.node.position, nextNode.node.position) / speed;
            
            if (t - timeToNext <= 0)
                return Vector3.Lerp(currentNode.node.position, nextNode.node.position, t / timeToNext);

            t -= timeToNext;

            if (t - nextNode.secondsToWait <= 0)
                return nextNode.node.position;

            t -= nextNode.secondsToWait;
            currentIndex++;
            currentIndex = currentIndex % nodes.Length;
        }
    }

    public Vector3 GetPositionByTime(float time)
    {
        int currentIndex = 0;

        while (true)
        {
            NodeObject currentNode = nodes[currentIndex];
            NodeObject nextNode = nodes[(currentIndex + 1) % nodes.Length];

            float timeToNext = Vector3.Distance(currentNode.node.position, nextNode.node.position) / speed;

            if (time - timeToNext <= 0)
                return Vector3.Lerp(currentNode.node.position, nextNode.node.position, time / timeToNext);

            time -= timeToNext;

            if (time - nextNode.secondsToWait <= 0)
                return nextNode.node.position;

            time -= nextNode.secondsToWait;
            currentIndex++;
            currentIndex = currentIndex % nodes.Length;
        }
    }

    public override Vector3 GetNewSpeed()
    {
        Vector3 destinyPos = GetPositionByTime(timeProgress);

        Vector3 difference = destinyPos - transform.position;

        if (difference.sqrMagnitude < 0.001f)
            return Vector3.zero;

        return difference.normalized * Mathf.Min(speed, difference.magnitude / Time.deltaTime);
    }

    private void OnDrawGizmos()
    {
        if (nodes == null || nodes.Length <= 1)
            return;

        Gizmos.color = Color.greenYellow;

        foreach (NodeObject n in nodes)
            Gizmos.DrawSphere(n.node.position, 1);

        Gizmos.color = Color.yellow;

        for (int i = 0; i < nodes.Length - 1; i++)
            Gizmos.DrawLine(nodes[i].node.position, nodes[i + 1].node.position);
        Gizmos.DrawLine(nodes[nodes.Length - 1].node.position, nodes[0].node.position);

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(GetStartPos(), 1.5f);
        Gizmos.DrawLine(transform.position, GetStartPos());
    }

    protected override void ObjectUpdate()
    {
        timeProgress += Time.deltaTime;
        timeProgress = Mathf.Repeat(timeProgress, timeConstantLength);

        base.ObjectUpdate();
    }
}
