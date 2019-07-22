using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreen : UIScreen
{
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
