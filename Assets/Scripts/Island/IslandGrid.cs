using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandGrid : MonoBehaviour
{
    public Transform StartPosition;
    public LayerMask WallMask;
    public Vector2 GridWorldSize;
    public float NodeRadius;
    public float Distance;

    Node[,] grid;
    public List<Node> FinalPath;

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    void Start()
    {
        nodeDiameter = NodeRadius * 2f;
        gridSizeX = Mathf.RoundToInt(GridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(GridWorldSize.y / nodeDiameter);
        CreateGrid();
    }

    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 bottomLeft = transform.position - Vector3.right * GridWorldSize.x / 2 -
            Vector3.forward * GridWorldSize.y / 2;

        for  (int y = 0; y < gridSizeY; ++y)
        {
            for (int x = 0; x < gridSizeX; ++x)
            {
                Vector3 worldPoint = bottomLeft + Vector3.right *
                    (x * nodeDiameter + NodeRadius) +
                    Vector3.forward * (y * nodeDiameter + NodeRadius);

                grid[x, y] = new Node(true, worldPoint, x, y);
            }
        }
    }

    public Node NodeFromWorldPosition(Vector3 worldPosition)
    {
        float xPoint = (worldPosition.x + GridWorldSize.x / 2) / GridWorldSize.x;
        float yPoint = (worldPosition.z + GridWorldSize.y / 2) / GridWorldSize.y;

        xPoint = Mathf.Clamp01(xPoint);
        yPoint = Mathf.Clamp01(yPoint);

        int x = Mathf.RoundToInt((gridSizeX - 1) * xPoint);
        int y = Mathf.RoundToInt((gridSizeY - 1) * yPoint);

        return grid[x, y];
    }

    public List<Node> GetNeighborNodes(Node node)
    {
        List<Node> neighboringNodes = new List<Node>();
        int xCheck;
        int yCheck;

        //Right
        xCheck = node.GridX + 1;
        yCheck = node.GridY;
        if (xCheck >= 0 && xCheck < gridSizeX)
        {
            if (yCheck >= 0 && yCheck < gridSizeY)
            {
                neighboringNodes.Add(grid[xCheck, yCheck]);
            }
        }

        //Left
        xCheck = node.GridX - 1;
        yCheck = node.GridY;
        if (xCheck >= 0 && xCheck < gridSizeX)
        {
            if (yCheck >= 0 && yCheck < gridSizeY)
            {
                neighboringNodes.Add(grid[xCheck, yCheck]);
            }
        }

        //Up
        xCheck = node.GridX;
        yCheck = node.GridY + 1;
        if (xCheck >= 0 && xCheck < gridSizeX)
        {
            if (yCheck >= 0 && yCheck < gridSizeY)
            {
                neighboringNodes.Add(grid[xCheck, yCheck]);
            }
        }

        //Down
        xCheck = node.GridX;
        yCheck = node.GridY - 1;
        if (xCheck >= 0 && xCheck < gridSizeX)
        {
            if (yCheck >= 0 && yCheck < gridSizeY)
            {
                neighboringNodes.Add(grid[xCheck, yCheck]);
            }
        }
        return neighboringNodes;
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(GridWorldSize.x, 1, GridWorldSize.y));

        if (grid != null)
        {
            foreach(Node node in grid)
            {
                if (node.IsWalkable)
                {
                    Gizmos.color = Color.white;
                }
                else
                {
                    Gizmos.color = Color.yellow;
                }

                if (FinalPath != null)
                {
                    if (FinalPath.Contains(node))
                    {
                        Gizmos.color = Color.red;
                    }
                }

                Gizmos.DrawCube(node.Position, Vector3.one * (nodeDiameter - Distance));
            }
        }
    }
}
