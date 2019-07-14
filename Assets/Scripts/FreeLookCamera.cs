﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FreeLookCamera : MonoBehaviour
{
    CinemachineFreeLook myCinemachineFreeLook;
    int myFingerId0 = InputManager.InactiveTouch;
    int myFingerId1 = InputManager.InactiveTouch;

    const float yAxisSpeed = 0.002f;
    const float xAxisSpeed = 0.8f;

    Vector3 tapPosition0;
    Vector3 tapPosition1;
    Vector3 touchDelta0;
    Vector3 touchDelta1;

    [SerializeField] readonly float MinOrthoSize = 10f;
    [SerializeField] readonly float MaxOrthoSize = 30f;

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
        if (hitInfo.transform != null)
        {
            return;
        }
        if (myFingerId0 == InputManager.InactiveTouch)
        {
            myFingerId0 = fingerId;
        }
        else if (myFingerId1 == InputManager.InactiveTouch)
        {
            myFingerId1 = fingerId;
        }
    }

    void OnTouchMove(int fingerId, Vector3 tapPosition, Vector3 touchDelta, RaycastHit hitInfo)
    {
        if (myFingerId0 == fingerId && myFingerId1 == InputManager.InactiveTouch)
        {
            myCinemachineFreeLook.m_XAxis.Value += touchDelta.x * xAxisSpeed;
            myCinemachineFreeLook.m_YAxis.Value += touchDelta.y * yAxisSpeed;
        }

        if (myFingerId0 == fingerId)
        {
            tapPosition0 = tapPosition;
            touchDelta0 = touchDelta;
            return;
        }
        if (myFingerId1 == fingerId)
        {
            tapPosition1 = tapPosition;
            touchDelta1 = touchDelta;
        }

        if (myFingerId0 != InputManager.InactiveTouch && 
            myFingerId1 != InputManager.InactiveTouch)
        {
            float pinchDistance = Vector3.Distance(tapPosition0, tapPosition1);
            float prevDistance = Vector3.Distance(tapPosition0 - touchDelta0,
                tapPosition1 - touchDelta1);
            float pinchDistanceDelta = pinchDistance - prevDistance;

            myCinemachineFreeLook.m_Lens.OrthographicSize += pinchDistanceDelta;
            myCinemachineFreeLook.m_Lens.OrthographicSize = Mathf.Clamp(
                myCinemachineFreeLook.m_Lens.OrthographicSize, MinOrthoSize, MaxOrthoSize);
        }
    }

    void OnTouchEnd(int fingerId, Vector3 tapPosition, RaycastHit hitInfo)
    {
        if (myFingerId0 == fingerId)
        {
            myFingerId0 = InputManager.InactiveTouch;
        }
        if (myFingerId1 == fingerId)
        {
            myFingerId1 = InputManager.InactiveTouch;
        }
    }

}
