using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Skybox/Background Color")]
public class SkyboxColor : ScriptableObject
{
    [SerializeField] Color topColor = Color.white;
    public Color TopColor
    {
        get { return topColor; }
    }

    [SerializeField] Color bottomColor = Color.white;
    public Color BottomColor
    {
        get { return bottomColor; }
    }

    [SerializeField] float exponent = 1f;
    public float Exponent
    {
        get { return exponent; }
    }
}
