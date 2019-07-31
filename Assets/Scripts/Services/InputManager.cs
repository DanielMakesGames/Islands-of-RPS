using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public delegate void TouchRaycastAction(int fingerId, Vector3 tapPosition, RaycastHit hitInfo);
    public delegate void TouchMoveAction(int fingerId, Vector3 tapPosition, Vector3 touchDelta, RaycastHit hitInfo);

    public static event TouchRaycastAction OnTouchBegin;
    public static event TouchMoveAction OnTouchMove;
    public static event TouchRaycastAction OnTouchEnd;

    public static event TouchRaycastAction OnUITouchBegin;
    public static event TouchMoveAction OnUITouchMove;
    public static event TouchRaycastAction OnUITouchEnd;

    public const int InactiveTouch = -1;
    const float raycastLength = 300f;

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
        UpdateKeyboardInput();
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

        if (Physics.Raycast(myRay, out hitInfo, raycastLength, uiLayer, QueryTriggerInteraction.Collide))
        {
            uiScreenPressed = true;
            OnUITouchBegin?.Invoke(fingerId, tapPosition, hitInfo);
        }
        else
        {
            myRay = mainCamera.ScreenPointToRay(tapPosition);
            if (Physics.Raycast(myRay, out hitInfo, raycastLength,
                Camera.main.cullingMask, QueryTriggerInteraction.Collide))
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
        myRay = mainCamera.ScreenPointToRay(tapPosition);
        Physics.Raycast(myRay, out hitInfo, raycastLength,
            Camera.main.cullingMask, QueryTriggerInteraction.Collide);

        if (uiScreenPressed)
        {
            OnUITouchEnd?.Invoke(fingerId, tapPosition, hitInfo);
            uiScreenPressed = false;
        }
        else
        {
            OnTouchEnd?.Invoke(fingerId, tapPosition, hitInfo);
        }
    }

    void OnTapMove(int fingerId, Vector3 tapPosition, Vector3 touchDelta)
    {
        myRay = mainCamera.ScreenPointToRay(tapPosition);
        Physics.Raycast(myRay, out hitInfo, raycastLength,
            Camera.main.cullingMask, QueryTriggerInteraction.Collide);

        if (uiScreenPressed)
        {
            OnUITouchMove?.Invoke(fingerId, tapPosition, touchDelta, hitInfo);
        }
        else
        {
            OnTouchMove?.Invoke(fingerId, tapPosition, touchDelta, hitInfo);
        }
    }

    public static Vector2 FixTouchDelta(Touch aT)
    {
        return aT.deltaPosition;
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
