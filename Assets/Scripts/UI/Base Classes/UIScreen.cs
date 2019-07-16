using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIScreen : MonoBehaviour 
{
    public delegate void ScreenEnabledAction(Color backgroundColor);
    public delegate void ScreenDisabledAction();

    [SerializeField] protected Color backgroundColor;
    protected bool isScreenEnabled = false;

    protected void EnableScreen()
    {
        if (!isScreenEnabled)
        {
            isScreenEnabled = true;
            for (int i = 0; i < transform.childCount; ++i)
            {
                transform.GetChild(i).gameObject.SetActive(true);
            }
            OnUIScreenEnabled();
        }
    }

    protected void DisableScreen()
    {
        if (isScreenEnabled)
        {
            isScreenEnabled = false;
            for (int i = 0; i < transform.childCount; ++i)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
            OnUIScreenDisabled();
        }
    }

    protected virtual void OnUIScreenEnabled()
    {
    }

    protected virtual void OnUIScreenDisabled()
    {
    }
}
