using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshController : MonoBehaviour
{
    NavMeshAgent myNavMeshAgent;

    private void Awake()
    {
        myNavMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void OnEnable()
    {
        InputManager.OnTouchBegin += OnTouchBegin;
    }

    private void OnDisable()
    {
        InputManager.OnTouchBegin -= OnTouchBegin;
    }

    void OnTouchBegin(int fingerId, Vector3 tapPosition, RaycastHit hitInfo)
    {
        if (hitInfo.transform != null)
        {
            myNavMeshAgent.SetDestination(hitInfo.point);
        }
    }

}
