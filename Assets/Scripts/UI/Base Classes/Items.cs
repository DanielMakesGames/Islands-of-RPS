using UnityEngine;
using System.Collections;

public class Items : MonoBehaviour 
{
    public delegate void ItemsAction(float distanceBetweenItems);
    public event ItemsAction OnCalculateMinMaxDistance;

    protected bool isTouchActive;
    protected float momentum;
    protected Vector3 localPosition;
    float minX = 0f;
    float maxX = 0f;

    protected bool isTransitioning = false;
    protected float targetPosition = 0f;
    const float transitionSpeed = 6f;
    float distanceBetweenItems = 400f;

    protected const float categoriesPercentage = 0.7f;
    protected int numberOfOverlays = 0;

    const float movementSpeed = 8f;
    const float momentumDeceleration = 3.5f;
    const float momentumDeadZone = 25f;

    protected virtual void Awake()
    {
        localPosition = transform.localPosition;
        momentum = 0f;
    }

    void Start()
    {
        CalculateMinMaxX();
    }

    protected void CalculateMinMaxX()
    {
        int childCount = 0;
        for (int i = 0; i < transform.childCount; ++i)
        {
            Transform child = transform.GetChild(i);
            if (child.gameObject.activeSelf == true)
            {
                ++childCount;
                minX = Mathf.Max(minX, child.localPosition.x);
                maxX = Mathf.Min(maxX, child.localPosition.x);
            }
        }
        if (childCount > 1)
        {
            distanceBetweenItems = (minX - maxX) / (float)(childCount - 1f);
        }
        minX = -minX - distanceBetweenItems;
        maxX = -maxX + distanceBetweenItems;

        OnCalculateMinMaxDistance?.Invoke(distanceBetweenItems);
    }

    protected virtual void OnEnable()
    {
        numberOfOverlays = 0;
        EnableInput();

        Button.OnButtonPress += DisableInput;
        Button.OnButtonRelease += EnableInput;
    }

    protected virtual void OnDisable()
    {
        DisableInput();

        Button.OnButtonPress -= DisableInput;
        Button.OnButtonRelease -= EnableInput;

        SetToNearestCharacter();
    }

    protected void EnableInput()
    {
        ++numberOfOverlays;
        if (numberOfOverlays == 1)
        {
            InputManager.OnUITouchBegin += OnUITouchBegin;
            InputManager.OnUITouchMove += OnUITouchMove;
            InputManager.OnUITouchEnd += OnUITouchEnd;
            ItemSelectPivot.OnItemSelectTapped += OnItemSelectTapped;
        }

        momentum = 0f;
    }

    protected void DisableInput()
    {
        --numberOfOverlays;
        if (numberOfOverlays == 0)
        {
            InputManager.OnUITouchBegin -= OnUITouchBegin;
            InputManager.OnUITouchMove -= OnUITouchMove;
            InputManager.OnUITouchEnd -= OnUITouchEnd;
            ItemSelectPivot.OnItemSelectTapped -= OnItemSelectTapped;
        }
    }

    protected virtual void OnUITouchBegin(int fingerId, Vector3 tapPosition,
        RaycastHit hitInfo)
    {
        Button isButton = hitInfo.transform.GetComponent<Button>();
        if (isButton == null)
        {
            isTouchActive = true;
            isTransitioning = false;
        }
    }

    void OnUITouchMove(int fingerId, Vector3 tapPosition, Vector3 touchDelta,
        RaycastHit hitInfo)
    {
        if (isTouchActive)
        {
            localPosition.x += touchDelta.x;
            momentum = touchDelta.x;
            localPosition.x = Mathf.Clamp(localPosition.x, minX, maxX);
            transform.localPosition = localPosition;
        }
    }

    void OnUITouchEnd(int fingerId, Vector3 tapPosition,
        RaycastHit hitInfo)
    {
        isTouchActive = false;
    }

    void OnItemSelectTapped(ItemSelectPivot myItemSelectPivot)
    {
        if (myItemSelectPivot.ParentItems == this)
        {
            momentum = 0f;
            isTransitioning = true;
            targetPosition = -myItemSelectPivot.transform.localPosition.x;
        }
    }

    void Update()
    {
        if (!isTouchActive)
        {
            MomentumDeceleration();
        }
    }

    public void SetLocalPosition(Vector3 newLocalPosition)
    {
        localPosition = newLocalPosition;
        transform.localPosition = localPosition;
    }

    void MomentumDeceleration()
    {
        momentum += (0f - momentum) * momentumDeceleration * Time.deltaTime;
        if (localPosition.x < minX + momentumDeadZone || localPosition.x > maxX - momentumDeadZone)
        {
            momentum = 0f;   
        }

        if (Mathf.Abs(momentum) > momentumDeadZone)
        {
            localPosition.x += momentum;
        }
        else
        {
            momentum = 0f;
            RoundToNearestCharacter();
        }

        localPosition.x = Mathf.Clamp(localPosition.x, minX, maxX);
        transform.localPosition = localPosition;
    }

    void RoundToNearestCharacter()
    {
        if (!isTransitioning)
        {
            targetPosition = Mathf.Round(localPosition.x / distanceBetweenItems) * distanceBetweenItems;
            targetPosition = Mathf.Clamp(targetPosition, minX + distanceBetweenItems, maxX - distanceBetweenItems);
        }

        float distance = targetPosition - localPosition.x;
        if (Mathf.Abs(distance) > 0.01f)
        {
            localPosition.x += distance * Time.deltaTime * transitionSpeed;
        }
        else
        {
            localPosition.x = targetPosition;
        }
    }

    void SetToNearestCharacter()
    {
        isTouchActive = false;
        isTransitioning = false;
        localPosition.x = targetPosition;
        transform.localPosition = localPosition;
    }
}
