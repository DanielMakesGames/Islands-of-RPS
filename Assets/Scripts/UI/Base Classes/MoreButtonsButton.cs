using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoreButtonsButton : Button
{
    bool isShowingMoreButtons = false;

    [SerializeField] GameObject[] MoreButtons = null;

    protected virtual void OnEnable()
    {
        EnableButton();
        ResetButton();
    }

    protected virtual void OnDisable()
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

    protected void EnableMoreButtons()
    {
        isShowingMoreButtons = true;
        for (int i = 0; i < MoreButtons.Length; ++i)
        {
            MoreButtons[i].SetActive(true);
        }
    }

    protected void DisableMoreButtons()
    {
        isShowingMoreButtons = false;
        for (int i = 0; i < MoreButtons.Length; ++i)
        {
            MoreButtons[i].SetActive(false);
        }
    }
}
