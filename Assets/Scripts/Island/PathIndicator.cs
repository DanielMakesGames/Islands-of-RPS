using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathIndicator : MonoBehaviour
{
    Squad mySquad;
    LineRenderer myLineRenderer;
    const float lineOffset = 0.6f;

    private void Awake()
    {
        mySquad = GetComponentInParent<Squad>();
        myLineRenderer = GetComponentInChildren<LineRenderer>();
        myLineRenderer.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (mySquad.path.Count > 0)
        {
            Vector3[] pos = new Vector3[mySquad.path.Count];

            for (int i = 0; i < pos.Length; ++i)
            {
                pos[i] = mySquad.path[i].transform.position + new Vector3(0, lineOffset, 0);
            }

            myLineRenderer.positionCount = pos.Length;
            myLineRenderer.SetPositions(pos);

            myLineRenderer.gameObject.SetActive(true);

            //Setup a touch indicator at the final node
        }
    }
}
