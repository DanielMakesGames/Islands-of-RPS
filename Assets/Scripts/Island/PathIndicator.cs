using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathIndicator : MonoBehaviour
{
    Squad mySquad;
    LineRenderer myLineRenderer;
    ParticleSystem myTargetNodeIndicator;
    Vector3 lineOffset;

    private void Awake()
    {
        mySquad = GetComponentInParent<Squad>();
        myLineRenderer = GetComponentInChildren<LineRenderer>();
        myTargetNodeIndicator = GetComponentInChildren<ParticleSystem>();
        myLineRenderer.gameObject.SetActive(false);
        myTargetNodeIndicator.gameObject.SetActive(true);

        lineOffset = new Vector3(0, 6f, 0);
    }

    private void OnEnable()
    {
        mySquad.OnAnimateSquadPath += OnAnimateSquadPath;
    }

    private void OnDisable()
    {
        mySquad.OnAnimateSquadPath -= OnAnimateSquadPath;
    }

    private void Update()
    {
        switch (mySquad.CurrentSquadState)
        {
            case Squad.SquadState.Selected:
            case Squad.SquadState.Moving:
            case Squad.SquadState.PendingUnitsReady:
                if (myTargetNodeIndicator.isStopped)
                {
                    myTargetNodeIndicator.Play();
                }
                break;
            default:
                if (myTargetNodeIndicator.isPlaying)
                {
                    myTargetNodeIndicator.Stop();
                }
                break;
        }
    }

    void OnAnimateSquadPath()
    {
        if (mySquad.path.Count > 0)
        {
            Vector3[] pos = new Vector3[mySquad.path.Count];
            pos[0] = mySquad.transform.position;

            for (int i = 1; i < pos.Length; ++i)
            {
                pos[i] = mySquad.path[i].transform.position + lineOffset;
            }

            myLineRenderer.positionCount = pos.Length;
            myLineRenderer.SetPositions(pos);

            myLineRenderer.gameObject.SetActive(true);

            myTargetNodeIndicator.transform.position = pos[pos.Length - 1];
        }
        else
        {
            myLineRenderer.gameObject.SetActive(false);
        }
    }
}
