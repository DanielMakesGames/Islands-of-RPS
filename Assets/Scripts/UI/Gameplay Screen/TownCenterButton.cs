using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownCenterButton : MoreButtonsButton
{
    public static event ButtonAction OnPressed;

    protected override void OnEnable()
    {
        base.OnEnable();

        RockSquadButton.OnPressed += DisableMoreButtons;
        PaperSquadButton.OnPressed += DisableMoreButtons;
        ScissorSquadButton.OnPressed += DisableMoreButtons;

        DisableMoreButtons();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        RockSquadButton.OnPressed -= DisableMoreButtons;
        PaperSquadButton.OnPressed -= DisableMoreButtons;
        ScissorSquadButton.OnPressed -= DisableMoreButtons;
    }

    public override void ButtonPressAction()
    {
        base.ButtonPressAction();

        OnPressed?.Invoke();
    }
}
