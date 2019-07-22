using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandRotation : MonoBehaviour
{
    void Update()
    {
        transform.Rotate(0f, Time.deltaTime * 90f, 0f);
    }
}
