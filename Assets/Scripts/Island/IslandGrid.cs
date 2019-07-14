using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandGrid : MonoBehaviour
{
    enum NodeDirection
    {
        North,
        East,
        South,
        West
    }

    Node[] islandNodes;
    List<Node> path = new List<Node>();

    void Awake()
    {
        islandNodes = GetComponentsInChildren<Node>();
    }

    public void SetNodeDistances(Node startingNode)
    {
        ResetNodeValues();
        startingNode.visited = 0;
        int[] testArray = new int[islandNodes.Length];

        for (int step = 1; step < testArray.Length; ++step)
        {
            for (int i = 0; i < islandNodes.Length; ++i)
            {
                if (islandNodes[i].visited == step - 1)
                {
                    TestFourDirections(islandNodes[i], step);
                }
            }
        }
    }

    void GetPath(Node targetNode)
    {
        int step;
        List<Node> tempList = new List<Node>();
        path.Clear();

        if (targetNode.visited != -1)
        {
            path.Add(targetNode);
            step = targetNode.visited - 1;
        }
        else
        {
            Debug.Log("Path not available.");
            return;
        }

        for (int i = step; step > -1; --step)
        {
            if (TestDirection(targetNode, step, NodeDirection.North))
            {
                tempList.Add(targetNode.NorthNode);
            }
            if (TestDirection(targetNode, step, NodeDirection.East))
            {
                tempList.Add(targetNode.EastNode);
            }
            if (TestDirection(targetNode, step, NodeDirection.South))
            {
                tempList.Add(targetNode.SouthNode);
            }
            if (TestDirection(targetNode, step, NodeDirection.West))
            {
                tempList.Add(targetNode.WestNode);
            }

            Node closestNode = FindClosest(targetNode.transform, tempList);
            path.Add(closestNode);
            targetNode = closestNode;
            tempList.Clear();
        }
    }

    void TestFourDirections(Node tNode, int step)
    {
        if (TestDirection(tNode, -1, NodeDirection.North))
        {
            SetVisited(tNode.NorthNode, step);
        }
        if (TestDirection(tNode, -1, NodeDirection.East))
        {
            SetVisited(tNode.EastNode, step);
        }
        if (TestDirection(tNode, -1, NodeDirection.South))
        {
            SetVisited(tNode.SouthNode, step);
        }
        if (TestDirection(tNode, -1, NodeDirection.West))
        {
            SetVisited(tNode.WestNode, step);
        }
    }

    bool TestDirection(Node tNode, int step, NodeDirection direction)
    {
        switch (direction)
        {
            case NodeDirection.North:
                if (tNode.NorthNode != null && tNode.NorthNode.visited == step)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case NodeDirection.East:
                if (tNode.EastNode != null && tNode.EastNode.visited == step)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case NodeDirection.South:
                if (tNode.SouthNode != null && tNode.SouthNode.visited == step)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case NodeDirection.West:
                if (tNode.WestNode != null && tNode.WestNode.visited == step)
                {
                    return true;
                }
                else
                {
                    return false;
                }
        }
        return false;
    }

    void SetVisited(Node tNode, int step)
    {
        tNode.visited = step;
    }

    Node FindClosest(Transform targetLocation, List<Node> tNodes)
    {
        float currentDistance = islandNodes.Length * 100f;
        int indexNumber = 0;
        for (int i = 0; i < tNodes.Count; ++i)
        {
            float distance = Vector3.Distance(targetLocation.position, tNodes[i].transform.position);
            if (distance < currentDistance)
            {
                currentDistance = distance;
                indexNumber = i;
            }
        }
        return tNodes[indexNumber];
    }

    public void ResetNodeValues()
    {
        for (int i = 0; i < islandNodes.Length; ++i)
        {
            islandNodes[i].visited = -1;
        }
    }
}
