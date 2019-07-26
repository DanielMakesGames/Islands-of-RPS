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

    void Awake()
    {
        islandNodes = GetComponentsInChildren<Node>();
    }

    public void SetPlayerNodeDistances(Node startingNode)
    {
        ResetPlayerNodeValues();
        startingNode.PlayerVisited = 0;
        int[] testArray = new int[islandNodes.Length];

        for (int step = 1; step < testArray.Length; ++step)
        {
            for (int i = 0; i < islandNodes.Length; ++i)
            {
                if (islandNodes[i].PlayerVisited == step - 1)
                {
                    TestFourPlayerDirections(islandNodes[i], step);
                }
            }
        }
    }

    public void SetEnemyNodeDistances(Node startingNode)
    {
        ResetEnemyNodeValues();
        startingNode.EnemyVisited = 0;
        int[] testArray = new int[islandNodes.Length];

        for (int step = 1; step < testArray.Length; ++step)
        {
            for (int i = 0; i < islandNodes.Length; ++i)
            {
                if (islandNodes[i].EnemyVisited == step - 1)
                {
                    TestFourEnemyDirections(islandNodes[i], step);
                }
            }
        }
    }

    public List<Node> GetPlayerPath(Node targetNode)
    {
        int step;
        List<Node> path = new List<Node>();
        List<Node> tempList = new List<Node>();

        if (targetNode.PlayerVisited != -1)
        {
            path.Add(targetNode);
            step = targetNode.PlayerVisited - 1;
        }
        else
        {
            Debug.Log("Path not available.");
            return path;
        }

        for (int i = step; step > -1; --step)
        {
            if (TestPlayerDirection(targetNode, step, NodeDirection.North))
            {
                tempList.Add(targetNode.NorthNode);
            }
            if (TestPlayerDirection(targetNode, step, NodeDirection.East))
            {
                tempList.Add(targetNode.EastNode);
            }
            if (TestPlayerDirection(targetNode, step, NodeDirection.South))
            {
                tempList.Add(targetNode.SouthNode);
            }
            if (TestPlayerDirection(targetNode, step, NodeDirection.West))
            {
                tempList.Add(targetNode.WestNode);
            }

            Node closestNode = FindClosest(targetNode.transform, tempList);
            path.Add(closestNode);
            targetNode = closestNode;
            tempList.Clear();
        }

        path.Reverse();
        return path;
    }

    public List<Node> GetEnemyPath(Node targetNode)
    {
        int step;
        List<Node> path = new List<Node>();
        List<Node> tempList = new List<Node>();

        if (targetNode.EnemyVisited != -1)
        {
            path.Add(targetNode);
            step = targetNode.EnemyVisited - 1;
        }
        else
        {
            Debug.Log("Path not available: " + targetNode.transform.parent.name + 
                " " + targetNode.gameObject.name);
            return path;
        }

        for (int i = step; step > -1; --step)
        {
            if (TestEnemyDirection(targetNode, step, NodeDirection.North))
            {
                tempList.Add(targetNode.NorthNode);
            }
            if (TestEnemyDirection(targetNode, step, NodeDirection.East))
            {
                tempList.Add(targetNode.EastNode);
            }
            if (TestEnemyDirection(targetNode, step, NodeDirection.South))
            {
                tempList.Add(targetNode.SouthNode);
            }
            if (TestEnemyDirection(targetNode, step, NodeDirection.West))
            {
                tempList.Add(targetNode.WestNode);
            }

            Node closestNode = FindClosest(targetNode.transform, tempList);
            path.Add(closestNode);
            targetNode = closestNode;
            tempList.Clear();
        }

        path.Reverse();
        return path;
    }

    void TestFourPlayerDirections(Node tNode, int step)
    {
        if (TestPlayerDirection(tNode, -1, NodeDirection.North))
        {
            SetPlayerVisited(tNode.NorthNode, step);
        }
        if (TestPlayerDirection(tNode, -1, NodeDirection.East))
        {
            SetPlayerVisited(tNode.EastNode, step);
        }
        if (TestPlayerDirection(tNode, -1, NodeDirection.South))
        {
            SetPlayerVisited(tNode.SouthNode, step);
        }
        if (TestPlayerDirection(tNode, -1, NodeDirection.West))
        {
            SetPlayerVisited(tNode.WestNode, step);
        }
    }

    void TestFourEnemyDirections(Node tNode, int step)
    {
        if (TestEnemyDirection(tNode, -1, NodeDirection.North))
        {
            SetEnemyVisited(tNode.NorthNode, step);
        }
        if (TestEnemyDirection(tNode, -1, NodeDirection.East))
        {
            SetEnemyVisited(tNode.EastNode, step);
        }
        if (TestEnemyDirection(tNode, -1, NodeDirection.South))
        {
            SetEnemyVisited(tNode.SouthNode, step);
        }
        if (TestEnemyDirection(tNode, -1, NodeDirection.West))
        {
            SetEnemyVisited(tNode.WestNode, step);
        }
    }

    bool TestPlayerDirection(Node tNode, int step, NodeDirection direction)
    {
        switch (direction)
        {
            case NodeDirection.North:
                if (tNode.NorthNode != null && tNode.NorthNode.PlayerVisited == step)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case NodeDirection.East:
                if (tNode.EastNode != null && tNode.EastNode.PlayerVisited == step)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case NodeDirection.South:
                if (tNode.SouthNode != null && tNode.SouthNode.PlayerVisited == step)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case NodeDirection.West:
                if (tNode.WestNode != null && tNode.WestNode.PlayerVisited == step)
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

    bool TestEnemyDirection(Node tNode, int step, NodeDirection direction)
    {
        switch (direction)
        {
            case NodeDirection.North:
                if (tNode.NorthNode != null && tNode.NorthNode.EnemyVisited == step)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case NodeDirection.East:
                if (tNode.EastNode != null && tNode.EastNode.EnemyVisited == step)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case NodeDirection.South:
                if (tNode.SouthNode != null && tNode.SouthNode.EnemyVisited == step)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case NodeDirection.West:
                if (tNode.WestNode != null && tNode.WestNode.EnemyVisited == step)
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

    void SetPlayerVisited(Node tNode, int step)
    {
        tNode.PlayerVisited = step;
    }

    void SetEnemyVisited(Node tNode, int step)
    {
        tNode.EnemyVisited = step;
    }

    public void ResetPlayerNodeValues()
    {
        for (int i = 0; i < islandNodes.Length; ++i)
        {
            islandNodes[i].PlayerVisited = -1;
        }
    }

    public void ResetEnemyNodeValues()
    {
        for (int i = 0; i < islandNodes.Length; ++i)
        {
            islandNodes[i].EnemyVisited = -1;
        }
    }

    public Node FindClosest(Transform targetLocation, List<Node> tNodes)
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
}
