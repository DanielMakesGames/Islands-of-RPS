using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Button : MonoBehaviour 
{
    public delegate void ButtonAction();
    public static event ButtonAction OnButtonPress;
    public static event ButtonAction OnButtonRelease;

    Image myImage;
    UnityEngine.UI.Button myButton;
    BoxCollider myBoxCollider;

    protected virtual void Awake()
    {
        myImage = GetComponent<Image>();
        myButton = GetComponent<UnityEngine.UI.Button>();
        myBoxCollider = GetComponent<BoxCollider>();
    }

    public virtual void ButtonPressAction()
    {
    }

    public virtual void ButtonPress()
    {
        OnButtonPress?.Invoke();
    }

    public virtual void ButtonRelease()
    {
        OnButtonRelease?.Invoke();
    }

    public virtual void DisableButton()
    {
        myButton.interactable = false;
    }

    public virtual void EnableButton()
    {
        myButton.interactable = true;
    }

}
