using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSquad : Squad
{
    public delegate void SquadSelectedAction(Squad squad);
    public static event SquadSelectedAction OnSquadSelected;
    public static event SquadSelectedAction OnSquadDeselected;

    int myFingerId = InputManager.InactiveTouch;
    Vector3 startingTap;

    void OnEnable()
    {
        InputManager.OnTouchBegin += OnTouchBegin;
        InputManager.OnTouchMove += OnTouchMove;
        InputManager.OnTouchEnd += OnTouchEnd;

        PlayerSquad.OnSquadSelected += Squad_OnSquadSelected;
        PlayerSquad.OnSquadDeselected += Squad_OnSquadDeselected;
    }

    void OnDisable()
    {
        InputManager.OnTouchBegin -= OnTouchBegin;
        InputManager.OnTouchMove -= OnTouchMove;
        InputManager.OnTouchEnd -= OnTouchEnd;

        PlayerSquad.OnSquadSelected -= Squad_OnSquadSelected;
        PlayerSquad.OnSquadDeselected -= Squad_OnSquadDeselected;
    }

    void OnTouchBegin(int fingerId, Vector3 tapPosition, RaycastHit hitInfo)
    {
        if (myFingerId == InputManager.InactiveTouch)
        {
            myFingerId = fingerId;
            startingTap = tapPosition;

            switch (mySquadState)
            {
                case Squad.SquadState.Ready:
                    if (hitInfo.transform != null)
                    {
                        if (hitInfo.transform == transform)
                        {
                            mySquadState = SquadState.OnTapped;
                        }
                    }
                    break;
                case Squad.SquadState.OnTapped:
                    break;
                case Squad.SquadState.Selected:
                    if (hitInfo.transform != null)
                    {
                        if (hitInfo.transform == transform)
                        {
                            mySquadState = SquadState.OnTappedSelected;
                        }
                    }
                    break;
                case Squad.SquadState.OnTappedSelected:
                    break;
                case Squad.SquadState.Moving:
                    break;
            }
        }
    }

    void OnTouchMove(int fingerId, Vector3 tapPosition, Vector3 touchDelta, RaycastHit hitInfo)
    {
        if (myFingerId == fingerId)
        {
            switch (mySquadState)
            {
                case Squad.SquadState.Ready:
                    break;
                case Squad.SquadState.OnTapped:
                    break;
                case Squad.SquadState.Selected:
                case Squad.SquadState.OnTappedSelected:
                    if (hitInfo.transform != null)
                    {
                        if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Node"))
                        {
                            PathfindingNode = hitInfo.transform.GetComponent<Node>();
                            if (PathfindingNode.IsWalkable)
                            {
                                path = islandGrid.GetPlayerPath(PathfindingNode);
                            }
                            else
                            {
                                path.Clear();
                            }
                        }
                        else if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Player Squad"))
                        {
                            if (Vector3.SqrMagnitude(tapPosition - startingTap) > 100f)
                            {
                                PathfindingNode = hitInfo.transform.GetComponent<PlayerSquad>().CurrentNode;
                                path = islandGrid.GetPlayerPath(PathfindingNode);
                            }
                        }
                        else if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Enemy Squad"))
                        {
                            PathfindingNode = hitInfo.transform.GetComponent<EnemySquad>().CurrentNode;
                            path = islandGrid.GetPlayerPath(PathfindingNode);
                        }
                    }
                    else
                    {
                        path.Clear();
                    }
                    AnimateSquadPath();
                    break;
                case Squad.SquadState.Moving:
                    break;
            }
        }
    }

    void OnTouchEnd(int fingerId, Vector3 tapPosition, RaycastHit hitInfo)
    {
        if (myFingerId == fingerId)
        {
            myFingerId = InputManager.InactiveTouch;

            switch (mySquadState)
            {
                case Squad.SquadState.Ready:
                    break;
                case Squad.SquadState.OnTapped:
                    if (hitInfo.transform != null)
                    {
                        if (hitInfo.transform == transform)
                        {
                            mySquadState = Squad.SquadState.Selected;
                            islandGrid.ResetPlayerNodeValues();
                            currentNode.PlayerVisited = 0;
                            islandGrid.SetPlayerNodeDistances(currentNode);

                            OnSquadSelected?.Invoke(this);
                            AnimateSquadSelected();
                        }
                    }
                    break;
                case Squad.SquadState.Selected:
                    if (path.Count > 0)
                    {
                        targetNode = PathfindingNode;
                        MoveToTarget(targetNode);

                        OnSquadDeselected?.Invoke(this);
                        AnimateSquadDeselected();
                    }
                    break;
                case Squad.SquadState.OnTappedSelected:
                    if (hitInfo.transform != null)
                    {
                        if (hitInfo.transform == transform)
                        {
                            mySquadState = Squad.SquadState.Ready;
                            path.Clear();
                            targetNode = null;

                            OnSquadDeselected?.Invoke(this);
                            AnimateSquadDeselected();
                        }
                    }
                    break;
                case Squad.SquadState.Moving:
                    break;
            }
            AnimateSquadPath();
        }
    }

    void Squad_OnSquadSelected(Squad squad)
    {
        if (squad != this)
        {
            switch (mySquadState)
            {
                case Squad.SquadState.Ready:
                    break;
                case Squad.SquadState.OnTapped:
                    mySquadState = SquadState.Ready;
                    break;
                case Squad.SquadState.Selected:
                case Squad.SquadState.OnTappedSelected:
                    if (path.Count > 0)
                    {
                        targetNode = PathfindingNode;
                        MoveToTarget(targetNode);
                    }
                    else
                    {
                        mySquadState = Squad.SquadState.Ready;
                    }

                    myFingerId = InputManager.InactiveTouch;
                    OnSquadDeselected?.Invoke(this);
                    AnimateSquadDeselected();
                    break;
                case Squad.SquadState.Moving:
                    break;
            }
        }
    }

    void Squad_OnSquadDeselected(Squad squad)
    {
    }

    protected override void SetCurrentNode()
    {
        if (Physics.Raycast(transform.position + transform.up * rayOffset, -transform.up, out RaycastHit raycastHit,
            rayDistance, nodeLayerMask, QueryTriggerInteraction.Ignore))
        {
            Node hitNode = raycastHit.transform.GetComponent<Node>();
            currentNode = hitNode;

            if (currentNode.CurrentPlayerSquad != null)
            {
                if (currentNode.CurrentPlayerSquad.PreviousNode != null && currentNode.CurrentPlayerSquad.PreviousNode.CurrentPlayerSquad == null)
                {
                    currentNode.CurrentPlayerSquad.MoveToTarget(currentNode.CurrentPlayerSquad.PreviousNode);
                }
                else if (PreviousNode != null && PreviousNode.CurrentPlayerSquad == null)
                {
                    currentNode.CurrentPlayerSquad.MoveToTarget(PreviousNode);
                }
                else
                {
                    bool didMove = false;
                    for (int i = 0; i < currentNode.Neighbours.Count; ++i)
                    {
                        if (currentNode.Neighbours[i].CurrentPlayerSquad == null)
                        {
                            currentNode.CurrentPlayerSquad.MoveToTarget(currentNode.Neighbours[i]);
                            didMove = true;
                            break;
                        }
                    }
                    if (!didMove)
                    {

                        currentNode.CurrentPlayerSquad.MoveToTarget(currentNode.Neighbours[
                            Random.Range(0, currentNode.Neighbours.Count)]);
                    }
                }
            }
            currentNode.CurrentPlayerSquad = this;
        }
    }

    public override void MoveToTarget(Node destinationNode)
    {
        mySquadState = Squad.SquadState.Moving;

        islandGrid.ResetPlayerNodeValues();
        currentNode.PlayerVisited = 0;
        currentNode.CurrentPlayerSquad = null;
        islandGrid.SetPlayerNodeDistances(currentNode);
        targetNode = destinationNode;
        path = islandGrid.GetPlayerPath(destinationNode);

        UpdateNavMeshAgents(destinationNode.transform.position + nodePositionOffset);
        StopAllCoroutines();
        StartCoroutine(MoveToTargetCoroutine());
    }

    public override void MoveFromTownCenter(Node destinationNode)
    {
        currentNode.PlayerVisited = 0;
        currentNode.CurrentPlayerSquad = null;

        transform.position = destinationNode.transform.position + nodePositionOffset;
        mySquadState = SquadState.Ready;
        path.Clear();

        SetCurrentNode();
        targetNode = null;
    }

}
