using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class PathfindingManager : MonoBehaviour
{
    public static PathfindingManager instance = null;
    List<PathfindingNode> nodes;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
            return;
        }    

        instance = this;
        nodes = new List<PathfindingNode>();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDestroy()
    {
        if (instance == this)
            instance = null;
    }

    public Vector3 GetRandomNodePosition()
    {
        if (nodes.Count <= 0)
            return Vector3.zero;

        return nodes[Random.Range(0, nodes.Count)].transform.position;
    }

    public List<Vector3> GetPathToPosition(Vector3 start, Vector3 target)
    {
        if (nodes == null)
            return new Vector3[1] { target }.ToList();

        PathfindingNode min = nodes[0];

        for (int i = 1; i < nodes.Count; i++)
        {
            if (Vector3.Distance(nodes[i].transform.position, start) < Vector3.Distance(min.transform.position, start))
                min = nodes[i];
        }

        PathfindingNode currentNode = min;
        List<Vector3> positions = new List<Vector3>();

        while (currentNode != null)
        {
            positions.Add(currentNode.transform.position);
            currentNode = currentNode.GetNearestNeighborNode(target);
        }

        positions.Add(target);

        return positions;
    }

    public void AddNode(PathfindingNode n)
    {
        nodes.Add(n);
    }

    public void RemoveNode(PathfindingNode n)
    {
        nodes.Remove(n);
    }

    public bool ContainsNode(PathfindingNode n)
    {
        return nodes.Contains(n);
    }
}
