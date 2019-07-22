﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayScreen : UIScreen
{
    private void OnEnable()
    {
        PlayButton.OnPressed += EnableScreen;
    }

    private void OnDisable()
    {
        PlayButton.OnPressed -= EnableScreen;
    }
}
