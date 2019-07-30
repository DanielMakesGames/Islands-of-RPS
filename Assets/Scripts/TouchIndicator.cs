using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TouchIndicator : MonoBehaviour
{
    int myFingerId0 = InputManager.InactiveTouch;
    [SerializeField] GameObject touchCircle0 = null;

    int myFingerId1 = InputManager.InactiveTouch;
    [SerializeField] GameObject touchCircle1 = null;

    Camera mainCamera;
    RectTransform uiCanvasRectTransform;
    Vector2 localPoint;

    private void Awake()
    {
        mainCamera = Camera.main;
        uiCanvasRectTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        InputManager.OnTouchBegin += OnTouchBegin;
        InputManager.OnTouchMove += OnTouchMove;
        InputManager.OnTouchEnd += OnTouchEnd;

        InputManager.OnUITouchBegin += OnTouchBegin;
        InputManager.OnUITouchMove += OnTouchMove;
        InputManager.OnUITouchEnd += OnTouchEnd;
    }

    private void OnDisable()
    {
        InputManager.OnTouchBegin -= OnTouchBegin;
        InputManager.OnTouchMove -= OnTouchMove;
        InputManager.OnTouchEnd -= OnTouchEnd;

        InputManager.OnUITouchBegin -= OnTouchBegin;
        InputManager.OnUITouchMove -= OnTouchMove;
        InputManager.OnUITouchEnd -= OnTouchEnd;
    }

    void OnTouchBegin(int fingerId, Vector3 tapPosition, RaycastHit hitInfo)
    {
        if (myFingerId0 == InputManager.InactiveTouch)
        {
            myFingerId0 = fingerId;

            touchCircle0.SetActive(true);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                uiCanvasRectTransform, tapPosition, mainCamera, out localPoint);
            touchCircle0.transform.localPosition = localPoint;
        }
        else if (myFingerId1 == InputManager.InactiveTouch)
        {
            myFingerId1 = fingerId;

            touchCircle1.SetActive(true);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                uiCanvasRectTransform, tapPosition, mainCamera, out localPoint);
            touchCircle1.transform.localPosition = localPoint;
        }
    }

    void OnTouchMove(int fingerId, Vector3 tapPosition, Vector3 touchDelta, RaycastHit hitInfo)
    {
        if (myFingerId0 == fingerId)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                 uiCanvasRectTransform, tapPosition, mainCamera, out localPoint);
            touchCircle0.transform.localPosition = localPoint;
        }

        if (myFingerId1 == fingerId)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                 uiCanvasRectTransform, tapPosition, mainCamera, out localPoint);
            touchCircle1.transform.localPosition = localPoint;
        }
    }

    void OnTouchEnd(int fingerId, Vector3 tapPosition, RaycastHit hitInfo)
    {
        if (myFingerId0 == fingerId)
        {
            myFingerId0 = InputManager.InactiveTouch;
            touchCircle0.SetActive(false);
        }
        if (myFingerId1 == fingerId)
        {
            myFingerId1 = InputManager.InactiveTouch;
            touchCircle1.SetActive(false);
        }
    }

}
