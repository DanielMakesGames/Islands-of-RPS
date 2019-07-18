using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSquad : Squad
{
    public delegate void SquadSelectedAction(Squad squad);
    public static event SquadSelectedAction OnSquadSelected;
    public static event SquadSelectedAction OnSquadDeselected;

    int myFingerId = InputManager.InactiveTouch;

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
                            path = islandGrid.GetPath(PathfindingNode);
                        }
                        if (hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Squad"))
                        {
                            PathfindingNode = hitInfo.transform.GetComponent<Squad>().CurrentNode;
                            path = islandGrid.GetPath(PathfindingNode);
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
                            islandGrid.ResetNodeValues();
                            currentNode.visited = 0;
                            islandGrid.SetNodeDistances(currentNode);

                            OnSquadSelected?.Invoke(this);
                            AnimateSquadSelected();
                        }
                    }
                    break;
                case Squad.SquadState.Selected:
                    if (path.Count > 0)
                    {
                        mySquadState = Squad.SquadState.Moving;
                        targetNode = PathfindingNode;

                        StartCoroutine(MoveToTargetCoroutine());
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
                    mySquadState = Squad.SquadState.Ready;
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
}
