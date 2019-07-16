﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockSquadButton : Button
{
    public static event ButtonAction OnPressed;

    public override void ButtonPressAction()
    {
        base.ButtonPressAction();

        OnPressed?.Invoke();
    }
}