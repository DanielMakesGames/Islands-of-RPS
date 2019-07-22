using UnityEngine;
using System.Collections;

public class ItemSelectPivot : MonoBehaviour 
{
    public delegate void ItemSelectAction();
    public delegate void ItemTappedAction(ItemSelectPivot myItemSelectPivot);
    public event ItemSelectAction OnItemSelect;
    public event ItemSelectAction OnItemDeselect;
    public static event ItemTappedAction OnItemSelectTapped;
    public static event ItemSelectAction OnItemPressed;
    public static event ItemSelectAction OnItemReleased;

    public enum ItemState
    {
        Selected,
        NotSelected
    }
    ItemState currentState = ItemState.NotSelected;
    public ItemState CurrentState
    {
        get { return currentState; }
    }

    float distanceFromCenter;
    float distanceBetweenItems;
    Items myItems;
    public Items ParentItems
    {
        get { return myItems; }
    }

    protected Vector3 localScale;
    protected Vector3 originalLocalScale;
    const float itemSelectPivotPressSpeed = 12f;

    Coroutine itemSelectPivotPressCoroutine;
    Coroutine itemSelectPivotReleaseCoroutine;

    protected ItemState stateWhenPressed = ItemState.NotSelected;
    bool isPressed = false;
    protected float pressedTimer = 0f;
    protected const float pressedTime = 0.75f;

    UIScreen myUIScreen;

    void Awake()
    {
        myUIScreen = GetComponentInParent<UIScreen>();
        currentState = ItemState.NotSelected;
        myItems = GetComponentInParent<Items>();
        myItems.OnCalculateMinMaxDistance += MyItems_OnCalculateMinMaxDistance;

        originalLocalScale = transform.localScale;
        localScale = originalLocalScale;
    }

    protected virtual void OnEnable()
    {
        localScale = originalLocalScale;
        transform.localScale = localScale;
    }

    protected virtual void OnDisable()
    {
        StopAllCoroutines();
        itemSelectPivotPressCoroutine = null;
        itemSelectPivotReleaseCoroutine = null;
    }

    void MyItems_OnCalculateMinMaxDistance(float distanceBetweenItems)
    {
        this.distanceBetweenItems = distanceBetweenItems / 2f;
    }

    void Update() 
    {
        distanceFromCenter = myUIScreen.transform.position.x - transform.position.x;
        if (Mathf.Abs(distanceFromCenter) < distanceBetweenItems)
        {
            if (currentState == ItemState.NotSelected)
            {
                currentState = ItemState.Selected;
                if (OnItemSelect != null)
                {
                    OnItemSelect();
                }
            }
        }
        else
        {
            if (currentState == ItemState.Selected)
            {
                currentState = ItemState.NotSelected;
                stateWhenPressed = currentState;
                if (OnItemDeselect != null)
                {
                    OnItemDeselect();
                }
            }
        }

        if (isPressed)
        {
            pressedTimer += Time.deltaTime;
            if (pressedTimer >= pressedTime)
            {
                ItemSelectPivotRelease();
            }
        }
    }

    public void ItemSelectPivotPress()
    {
        stateWhenPressed = currentState;
        isPressed = true;
        pressedTimer = 0f;
        if (itemSelectPivotReleaseCoroutine != null)
        {
            StopCoroutine( itemSelectPivotReleaseCoroutine );
            itemSelectPivotReleaseCoroutine = null;
            ItemSelectPivotPressAction();
        }

        if (gameObject.activeInHierarchy)
        {
            itemSelectPivotPressCoroutine = StartCoroutine( ItemSelectPivotPressCoroutine() );
        }

        if (OnItemPressed != null)
        {
            OnItemPressed();
        }
    }

    public void ItemSelectPivotRelease()
    {
        if (isPressed)
        {
            if (itemSelectPivotPressCoroutine != null)
            {
                StopCoroutine( itemSelectPivotPressCoroutine );
                itemSelectPivotPressCoroutine = null;
            }

            if (gameObject.activeInHierarchy)
            {
                itemSelectPivotReleaseCoroutine = StartCoroutine( ItemSelectPivotReleaseCoroutine() );
            }
            isPressed = false;

            if (OnItemReleased != null)
            {
                OnItemReleased();
            }
        }
    }

    public void ItemSelectPivotIgnoreRelease()
    {
        pressedTimer = pressedTime;
    }

    protected virtual void ItemSelectPivotPressAction()
    {
        if (pressedTimer >= pressedTime)
        {
            return;
        }
        if (stateWhenPressed == ItemState.NotSelected && currentState == ItemState.NotSelected)
        {
            if (OnItemSelectTapped != null)
            {
                OnItemSelectTapped(this);
            }
        }
    }

    IEnumerator ItemSelectPivotPressCoroutine()
    {
        float timer = 0f;

        while (timer < 1f)
        {
            timer += Time.deltaTime * itemSelectPivotPressSpeed;
            localScale = Vector3.Lerp(localScale, originalLocalScale * 0.9f, timer);
            transform.localScale = localScale;

            yield return null;
        }

        localScale = originalLocalScale * 0.9f;
        transform.localScale = localScale;

        itemSelectPivotPressCoroutine = null;
    }

    IEnumerator ItemSelectPivotReleaseCoroutine()
    {
        float timer = 0f;

        while (timer < 1f)
        {
            timer += Time.deltaTime * itemSelectPivotPressSpeed;
            localScale = Vector3.Lerp(localScale, originalLocalScale, timer);
            transform.localScale = localScale;

            yield return null;
        }

        localScale = originalLocalScale;
        transform.localScale = localScale;

        ItemSelectPivotPressAction();
        itemSelectPivotReleaseCoroutine = null;
    }
}