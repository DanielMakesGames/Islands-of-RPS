using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public delegate void TouchBeginAction(int fingerId, Vector3 tapPosition, RaycastHit hitInfo);
    public delegate void TouchMoveAction(int fingerId, Vector3 tapPosition, Vector3 touchDelta);
    public delegate void TouchEndAction(int fingerId, Vector3 tapPosition);
    public delegate void TouchPinchAction(float pinchDistanceDelta, float turnAngleDelta);

    public static event TouchBeginAction OnTouchBegin;
    public static event TouchMoveAction OnTouchMove;
    public static event TouchEndAction OnTouchEnd;

    public static event TouchBeginAction OnUITouchBegin;
    public static event TouchMoveAction OnUITouchMove;
    public static event TouchEndAction OnUITouchEnd;

    public const int InactiveTouch = -1;
    const float raycastLength = 100f;

    Camera mainCamera;
    int uiLayer;

    Ray myRay;
    RaycastHit hitInfo;

    bool uiScreenPressed = false;
    Vector3 previousMousePosition;

    void Awake()
    {
        mainCamera = Camera.main.GetComponent<Camera>();
        uiLayer = LayerMask.GetMask("UI");
    }

    void Update()
    {
    #if UNITY_EDITOR
        //UpdateKeyboardInput();
        UpdateTouchInput();
    #elif UNITY_IPHONE || UNITY_ANDROID
        UpdateTouchInput();
    #else
        UpdateKeyboardInput();
    #endif
    }

    void UpdateKeyboardInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnTapDown(0, Input.mousePosition);
        }
        if (Input.GetMouseButton(0))
        {
            OnTapMove(0, Input.mousePosition, Input.mousePosition - previousMousePosition);
        }
        if (Input.GetMouseButtonUp(0))
        {
            OnTapUp(0, Input.mousePosition);
        }

        previousMousePosition = Input.mousePosition;
    }

    void UpdateTouchInput()
    {
        for (int i = 0; i < Input.touchCount; ++i)
        {
            switch (Input.touches[i].phase)
            {
                case TouchPhase.Began:
                    OnTapDown(Input.touches[i].fingerId,
                        Input.touches[i].position);
                    break;
                case TouchPhase.Moved:
                    OnTapMove(Input.touches[i].fingerId,
                        Input.touches[i].position,
                        FixTouchDelta(Input.touches[i]));
                    break;
                case TouchPhase.Ended:
                    OnTapUp(Input.touches[i].fingerId,
                        Input.touches[i].position);
                    break;
                case TouchPhase.Canceled:
                    OnTapUp(Input.touches[i].fingerId,
                        Input.touches[i].position);
                    break;
            }
        }
    }

    void OnTapDown(int fingerId, Vector3 tapPosition)
    {
        myRay = mainCamera.ScreenPointToRay(tapPosition);

        if (Physics.Raycast(myRay, out hitInfo, raycastLength, uiLayer))
        {
            uiScreenPressed = true;
            OnUITouchBegin?.Invoke(fingerId, tapPosition, hitInfo);
        }
        else
        {
            myRay = mainCamera.ScreenPointToRay(tapPosition);
            if (Physics.Raycast(myRay, out hitInfo, raycastLength))
            {
                OnTouchBegin?.Invoke(fingerId, tapPosition, hitInfo);
            }
            else
            {
                OnTouchBegin?.Invoke(fingerId, tapPosition, new RaycastHit());
            }
        }
    }

    void OnTapUp(int fingerId, Vector3 tapPosition)
    {
        if (uiScreenPressed)
        {
            OnUITouchEnd?.Invoke(fingerId, tapPosition);
            uiScreenPressed = false;
        }
        else
        {
            OnTouchEnd?.Invoke(fingerId, tapPosition);
        }
    }

    void OnTapMove(int fingerId, Vector3 tapPosition, Vector3 touchDelta)
    {
        if (uiScreenPressed)
        {
            OnUITouchMove?.Invoke(fingerId, tapPosition, touchDelta);
        }
        else
        {
            OnTouchMove?.Invoke(fingerId, tapPosition, touchDelta);
        }
    }

    public static Vector2 FixTouchDelta(Touch aT)
    {
    #if UNITY_ANDROID
        float dt = Time.deltaTime / aT.deltaTime;
        if (float.IsNaN(dt) || float.IsInfinity(dt))
        {
        dt = 1.0f;
        }
        return aT.deltaPosition * dt;
    #else
        return aT.deltaPosition;
    #endif
    }

    static private float Angle(Vector2 pos1, Vector2 pos2)
    {
        Vector2 from = pos2 - pos1;
        Vector2 to = new Vector2(1, 0);

        float result = Vector2.Angle(from, to);
        Vector3 cross = Vector3.Cross(from, to);

        if (cross.z > 0)
        {
            result = 360f - result;
        }

        return result;
    }
}
