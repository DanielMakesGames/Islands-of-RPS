using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadManager : MonoBehaviour
{
    protected List<Squad> mySquads;

    protected virtual void Awake()
    {
        mySquads = new List<Squad>();
    }

    public void RemoveSquad(Squad squad)
    {
        mySquads.Remove(squad);
    }
}
