using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class IslandNavMeshSurface : MonoBehaviour
{
    NavMeshSurface myNavMeshSurface;

    void Awake()
    {
        myNavMeshSurface = GetComponent<NavMeshSurface>();
       //myNavMeshSurface.BuildNavMesh();
    }
}
