using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    IslandGrid islandGrid;
    public Node StartNode;
    public Node TargetNode;

    int myFingerId = InputManager.InactiveTouch;
    List<Node> path = new List<Node>();

    private void Awake()
    {
        islandGrid = GetComponent<IslandGrid>();
    }

    void OnEnable()
    {
        InputManager.OnTouchBegin += OnTouchBegin;
    }

    void OnDisable()
    {
        InputManager.OnTouchBegin -= OnTouchBegin;
    }

    void Start()
    {
        islandGrid.ResetNodeValues();
    }

    void OnTouchBegin(int fingerId, Vector3 tapPosition, RaycastHit hitInfo)
    {
        if (myFingerId == InputManager.InactiveTouch)
        {
            if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Tile"))
            {
                myFingerId = fingerId;

                islandGrid.ResetNodeValues();
                StartNode = hitInfo.transform.GetComponent<Node>();
                StartNode.visited = 0;
                islandGrid.SetNodeDistances(StartNode);
            }
        }
    }
}
