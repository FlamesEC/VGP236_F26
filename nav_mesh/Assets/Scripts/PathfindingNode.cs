using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingNode : MonoBehaviour
{
    public PathfindingNode[] neighbors;

    public PathfindingNode GetNearestNeighborNode(Vector3 target)
    {
        if (neighbors == null) return null;

        PathfindingNode min = neighbors[0];

        for (int i = 1; i < neighbors.Length; i++)
        {
            if (Vector3.Distance(neighbors[i].transform.position, target) < Vector3.Distance(min.transform.position, target))
                min = neighbors[i];
        }

        if (Vector3.Distance(this.transform.position, target) < Vector3.Distance(min.transform.position, target))
            return null;

        return min;
    }

    private void Start()
    {
        if (PathfindingManager.instance.ContainsNode(this))
            return;

        PathfindingManager.instance.AddNode(this);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        if (neighbors == null || neighbors.Length <= 0)
            Gizmos.color = Color.softYellow;

        Gizmos.DrawSphere(transform.position, 0.5f);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        foreach (PathfindingNode node in neighbors)
        {
            Gizmos.DrawLine(transform.position, node.transform.position);
        }
    }

    private void OnEnable()
    {
        if (PathfindingManager.instance == null)
            return;
        if (PathfindingManager.instance.ContainsNode(this))
            return;

        PathfindingManager.instance.AddNode(this);
    }

    private void OnDisable()
    {
        if (PathfindingManager.instance == null)
            return;

        PathfindingManager.instance.RemoveNode(this);
    }
}
