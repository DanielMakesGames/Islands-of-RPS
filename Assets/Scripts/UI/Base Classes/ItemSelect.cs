using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSelect : MonoBehaviour 
{
    protected UIScreen myUIScreen;
    protected ItemSelectPivot myItemSelectPivot;

    protected MeshRenderer[] myMeshRenderers;

    protected bool isActive = false;
    bool isPressed = false;
    float amountMoved = 0f;
    const float scaleSpeed = 4f;
    const float scaleAmount = 1.2f;
    Color[] originalColors;

	protected virtual void Awake()
    {
        myMeshRenderers = GetComponentsInChildren<MeshRenderer>();
        myUIScreen = GetComponentInParent<UIScreen>();
        myItemSelectPivot = GetComponentInParent<ItemSelectPivot>();

        ChangeColorGray();
	}

    protected virtual void OnEnable()
    {
        if (myItemSelectPivot != null)
        {
            myItemSelectPivot.OnItemSelect += MyItemSelectPivotOnItemSelect;
            myItemSelectPivot.OnItemDeselect += MyItemSelectPivotOnItemDeselect;
            if (myItemSelectPivot.CurrentState == ItemSelectPivot.ItemState.Selected)
            {
                MyItemSelectPivotOnItemSelect();
            }

            InputManager.OnUITouchBegin += OnUITouchBegin;
            InputManager.OnUITouchMove += OnUITouchMove;
            InputManager.OnUITouchEnd += OnUITouchEnd;
        }
    }

    protected virtual void OnDisable()
    {
        if (myItemSelectPivot != null)
        {
            myItemSelectPivot.OnItemSelect -= MyItemSelectPivotOnItemSelect;
            myItemSelectPivot.OnItemDeselect -= MyItemSelectPivotOnItemDeselect;

            InputManager.OnUITouchBegin -= OnUITouchBegin;
            InputManager.OnUITouchMove -= OnUITouchMove;
            InputManager.OnUITouchEnd -= OnUITouchEnd;
        }
    }

    void OnUITouchBegin(int fingerId, Vector3 tapPosition,
        RaycastHit hitInfo)
    {
        if (hitInfo.transform == transform)
        {
            amountMoved = 0f;
            isPressed = true;
            ItemSelectPress();
        }
    }

    void OnUITouchMove(int fingerId, Vector3 tapPosition, Vector3 touchDelta,
        RaycastHit hitInfo)
    {
        if (isPressed)
        {
            amountMoved += touchDelta.sqrMagnitude;
            if (amountMoved > 5f)
            {
                isPressed = false;
                ItemSelectDelease();
            }
        }
    }

    void OnUITouchEnd(int fingerId, Vector3 tapPosition,
        RaycastHit hitInfo)
    {
        if (isPressed)
        {
            isPressed = false;
            ItemSelectRelease();
        }
    }

    void LateUpdate()
    {
        if (isActive)
        {
            transform.localPosition = GetLocalDestinationPosition();
        }
    }

    Vector3 GetLocalDestinationPosition()
    {
        return transform.parent.InverseTransformPoint( myUIScreen.transform.TransformPoint( new Vector3(0f, transform.parent.parent.localPosition.y, 0f) ) );
    }

    protected virtual void MyItemSelectPivotOnItemSelect()
    {
        ChangeColorWhite();
        ScaleUp();
    }

    protected virtual void MyItemSelectPivotOnItemDeselect()
    {
        ChangeColorGray();
        ScaleDown();
    }

    public void ItemSelectPress()
    {
        myItemSelectPivot.ItemSelectPivotPress();
    }

    public void ItemSelectRelease()
    {
        myItemSelectPivot.ItemSelectPivotRelease();
    }

    public void ItemSelectDelease()
    {
        myItemSelectPivot.ItemSelectPivotIgnoreRelease();
        myItemSelectPivot.ItemSelectPivotRelease();
    }

    protected virtual void ScaleUp()
    {
        StopAllCoroutines();
        StartCoroutine( ScaleUpCoroutine() );
    }

    protected virtual void ScaleDown()
    {
        StopAllCoroutines();
        StartCoroutine( ScaleDownCoroutine() );
    }

    protected void ChangeColorGray()
    {
        originalColors = new Color[myMeshRenderers.Length];
        for (int i = 0; i < myMeshRenderers.Length; ++i)
        {
            if (myMeshRenderers[i].material.HasProperty("_Color"))
            {
                originalColors[i] = myMeshRenderers[i].material.color;
                myMeshRenderers[i].material.color = originalColors[i] * 0.25f;
            }
        }
    }

    protected void ChangeColorWhite()
    {
        for (int i = 0; i < myMeshRenderers.Length; ++i)
        {
            if (myMeshRenderers[i].material.HasProperty("_Color"))
            {
                myMeshRenderers[i].material.color = originalColors[i];
            }
        }
    }

    protected IEnumerator ScaleUpCoroutine()
    {
        float timer = 0f;
        Vector3 destinationScale = Vector3.one * scaleAmount;

        while (timer < 1f)
        {
            timer += Time.deltaTime * scaleSpeed;
            transform.localScale = Vector3.Lerp(transform.localScale, destinationScale, timer);
            transform.localPosition = Vector3.Lerp(transform.localPosition, GetLocalDestinationPosition(), timer);
            yield return null;
        }

        transform.localScale = destinationScale;
        transform.localPosition = GetLocalDestinationPosition();
        isActive = true;
    }

    protected IEnumerator ScaleDownCoroutine()
    {
        isActive = false;
        float timer = 0f;
        Vector3 destinationScale = Vector3.one;

        while (timer < 1f)
        {
            timer += Time.deltaTime * scaleSpeed;
            transform.localScale = Vector3.Lerp(transform.localScale, destinationScale, timer);
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, timer);
            yield return null;
        }

        transform.localScale = destinationScale;
        transform.localPosition = Vector3.zero;
    }

}
