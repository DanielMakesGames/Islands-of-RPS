using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ocean : MonoBehaviour
{
    MeshFilter myMeshFilter;
    Vector3[] vertices;
    Color[] colors;
    bool[] ignore;

    List<Collider> myBoatColliders;
    const float FoamReductionRate = 0.05f;

    private void Awake()
    {
        myMeshFilter = GetComponent<MeshFilter>();
        vertices = myMeshFilter.mesh.vertices;
        colors = myMeshFilter.mesh.colors;
        ignore = new bool[vertices.Length];

        for (int i = 0; i < vertices.Length; ++i)
        {
            vertices[i] = transform.localToWorldMatrix.MultiplyPoint3x4(vertices[i]);
            if (colors[i].g > 0f)
            {
                ignore[i] = true;
            }
            else
            {
                ignore[i] = false;
            }

            myBoatColliders = new List<Collider>();
        }
    }

    private void OnEnable()
    {

    }

    private void OnDisable()
    {

    }

    private void Update()
    {
        for (int i = 0; i < vertices.Length; ++i)
        {
            for (int j = 0; j < myBoatColliders.Count; ++j)
            {
                if (myBoatColliders[j].bounds.Contains(vertices[i]))
                {
                    if (!ignore[i])
                    {
                        colors[i].g = 1f;
                    }
                }
                else
                {
                    if (!ignore[i])
                    {
                        colors[i].g = Mathf.Clamp01(colors[i].g - Time.deltaTime * FoamReductionRate);
                    }
                }
            }
        }
        myMeshFilter.mesh.colors = colors;
    }
}
