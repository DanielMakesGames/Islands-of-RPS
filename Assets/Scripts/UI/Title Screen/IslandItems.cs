using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandItems : Items
{
    protected override void Awake()
    {
        string currentIsland = SaveData.GetString(SaveData.IslandKey);
        IslandItemSelect[] islandItemSelects = GetComponentsInChildren<IslandItemSelect>();

        Vector3 islandLocalPosition = transform.localPosition;
        for (int i = 0; i < islandItemSelects.Length; ++i)
        {
            if (currentIsland == islandItemSelects[i].IslandName)
            {
                islandLocalPosition.x = -islandItemSelects[i].transform.parent.localPosition.x;
                break;
            }
        }
        transform.localPosition = islandLocalPosition;
        base.Awake();
    }
}
