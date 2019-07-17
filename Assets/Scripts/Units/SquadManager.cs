using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadManager : MonoBehaviour
{
    protected List<Squad> mySquads;

    private void Awake()
    {
        mySquads = new List<Squad>();
    }
}
