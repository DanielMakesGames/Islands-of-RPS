using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FreeLookCamera : MonoBehaviour
{
    CinemachineFreeLook myCinemachineFreeLook;
    int myFingerId = InputManager.InactiveTouch;

    const float yAxisSpeed = 0.002f;
    const float xAxisSpeed = 0.8f;

    private void Awake()
    {
        myCinemachineFreeLook = GetComponent<CinemachineFreeLook>();
    }

    private void OnEnable()
    {
        InputManager.OnTouchBegin += OnTouchBegin;
        InputManager.OnTouchMove += OnTouchMove;
        InputManager.OnTouchEnd += OnTouchEnd;
    }

    private void OnDisable()
    {
        InputManager.OnTouchBegin -= OnTouchBegin;
        InputManager.OnTouchMove -= OnTouchMove;
        InputManager.OnTouchEnd -= OnTouchEnd;
    }

    void OnTouchBegin(int fingerId, Vector3 tapPosition, RaycastHit hitInfo)
    {
        if (hitInfo.transform == null && myFingerId == InputManager.InactiveTouch)
        {
            myFingerId = fingerId;
        }
    }

    void OnTouchMove(int fingerId, Vector3 tapPosition, Vector3 touchDelta)
    {
        if (myFingerId == fingerId)
        {
            myCinemachineFreeLook.m_XAxis.Value += touchDelta.x * xAxisSpeed;
            myCinemachineFreeLook.m_YAxis.Value += touchDelta.y * yAxisSpeed;
        }
    }

    void OnTouchEnd(int fingerId, Vector3 tapPosition)
    {
        if (myFingerId == fingerId)
        {
            myFingerId = InputManager.InactiveTouch;
        }
    }

}
