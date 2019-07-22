using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreen : UIScreen
{
    [SerializeField] Material grayscaleMaterial = null;
    public Material GrayscaleMaterial
    {
        get { return grayscaleMaterial; }
    }

    private void OnEnable()
    {
        isScreenEnabled = true;
        PlayButton.OnPressed += DisableScreen;
    }

    private void OnDisable()
    {
        PlayButton.OnPressed -= DisableScreen;
    }
}
