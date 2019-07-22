using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayScreen : UIScreen
{
    private void OnEnable()
    {
        PlayButton.OnPressed += EnableScreen;
        TownCenter.OnTownCenterDestroyed += DisableScreen;
    }

    private void OnDisable()
    {
        PlayButton.OnPressed -= EnableScreen;
        TownCenter.OnTownCenterDestroyed -= DisableScreen;
    }
}
