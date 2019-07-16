using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoreButtonsButton : Button
{
    bool isShowingMoreButtons = false;

    [SerializeField] GameObject[] MoreButtons;

    void OnEnable()
    {
        EnableButton();
        ResetButton();
    }

    void OnDisable()
    {
    }

    public override void ButtonPressAction()
    {
        if (!isShowingMoreButtons)
        {
            base.ButtonPress();
            EnableMoreButtons();
        }
        else
        {

            base.ButtonRelease();
            DisableMoreButtons();
        }
    }

    void ResetButton()
    {
        DisableMoreButtons();
    }

    void EnableMoreButtons()
    {
        isShowingMoreButtons = true;
        for (int i = 0; i < MoreButtons.Length; ++i)
        {
            MoreButtons[i].SetActive(true);
        }
    }

    void DisableMoreButtons()
    {
        isShowingMoreButtons = false;
        for (int i = 0; i < MoreButtons.Length; ++i)
        {
            MoreButtons[i].SetActive(false);
        }
    }
}
