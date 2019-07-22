﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverScreen : UIScreen
{
    private void OnEnable()
    {
        TownCenter.OnTownCenterDestroyed += EnableScreen;
    }

    private void OnDisable()
    {
        TownCenter.OnTownCenterDestroyed -= EnableScreen;
    }
}
