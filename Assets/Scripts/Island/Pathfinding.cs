using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    IslandGrid islandGrid;
    public Transform StartPosition;
    public Transform TargetPosition;

    private void Awake()
    {
        islandGrid = GetComponent<IslandGrid>();
    }

    void Update()
    {
        FindPath(StartPosition.position, TargetPosition.position);
    }

    void FindPath(Vector3 startPosition, Vector3 targetPosition)
    {
        Node StartNode = islandGrid.NodeFromWorldPosition(startPosition);
        Node TargetNode = islandGrid.NodeFromWorldPosition(targetPosition);

        List<Node> OpenList = new List<Node>();
        HashSet<Node> ClosedList = new HashSet<Node>();

        OpenList.Add(StartNode);

        while (OpenList.Count > 0)
        {
            Node CurrentNode = OpenList[0];
            for (int i = 1; i < OpenList.Count; ++i)
            {
                if (OpenList[i].FCost < CurrentNode.FCost ||
                    OpenList[i].FCost == CurrentNode.FCost &&
                    OpenList[i].HCost < CurrentNode.HCost)
                {
                    CurrentNode = OpenList[i];
                }
            }
            OpenList.Remove(CurrentNode);
            ClosedList.Add(CurrentNode);

            if (CurrentNode == TargetNode)
            {
                GetFinalPath(StartNode, TargetNode);
            }

            foreach (Node NeighborNode in islandGrid.GetNeighborNodes(CurrentNode))
            {
                if (!NeighborNode.IsWalkable || ClosedList.Contains(NeighborNode))
                {
                    continue;
                }
                int MoveCost = CurrentNode.GCost + GetManhattenDistance(CurrentNode, NeighborNode);

                if (MoveCost < NeighborNode.GCost || !OpenList.Contains(NeighborNode))
                {
                    NeighborNode.GCost = MoveCost;
                    NeighborNode.HCost = GetManhattenDistance(NeighborNode, TargetNode);
                    NeighborNode.Parent = CurrentNode;

                    if (!OpenList.Contains(NeighborNode))
                    {
                        OpenList.Add(NeighborNode);
                    }
                }
            }
        }
    }

    int GetManhattenDistance(Node nodeA, Node nodeB)
    {
        int ix = Mathf.Abs(nodeA.GridX - nodeB.GridX);
        int iy = Mathf.Abs(nodeA.GridY - nodeB.GridY);

        return ix + iy;
    }

    void GetFinalPath(Node startingNode, Node endNode)
    {
        List<Node> FinalPath = new List<Node>();
        Node CurrentNode = endNode;

        while (CurrentNode != startingNode)
        {
            FinalPath.Add(CurrentNode);
            CurrentNode = CurrentNode.Parent;
        }

        FinalPath.Reverse();

        islandGrid.FinalPath = FinalPath;
    }
}
