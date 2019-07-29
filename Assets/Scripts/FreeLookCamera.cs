using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FreeLookCamera : MonoBehaviour
{
    enum FreeLookCameraState
    {
        TransitionIn,
        FreeLook,
        TransitionOut
    }
    FreeLookCameraState myFreeLookCameraState;

    CinemachineFreeLook myCinemachineFreeLook;
    int myFingerId0 = InputManager.InactiveTouch;
    int myFingerId1 = InputManager.InactiveTouch;

    const float yAxisSpeed = 0.004f;
    const float xAxisSpeed = 0.2f;
    const float pinchSpeed = 0.06f;

    Vector3 tapPosition0;
    Vector3 tapPosition1;
    Vector3 touchDelta0;
    Vector3 touchDelta1;

    [SerializeField] readonly float MinOrthoSize = 40f;
    [SerializeField] readonly float MaxOrthoSize = 140f;

    const float ZoomAnimationSpeed = 2f;
    float currentOrthoZoom = 0f;

    float prevTouchDeltaX = 0f;
    float xAxisMomentum = 0f;

    bool isInStrategyMode = false;

    private void Awake()
    {
        myCinemachineFreeLook = GetComponent<CinemachineFreeLook>();
        myFreeLookCameraState = FreeLookCameraState.FreeLook;
    }

    private void OnEnable()
    {
        InputManager.OnTouchBegin += OnTouchBegin;
        InputManager.OnTouchMove += OnTouchMove;
        InputManager.OnTouchEnd += OnTouchEnd;

        PlayButton.OnPressed += PlayButtonOnPressed;
        PlayerSquadManager.OnEnterStrategyMode += OnEnterStrategyMode;
        PlayerSquadManager.OnExitStrategyMode += OnExitStrategyMode;
    }

    private void OnDisable()
    {
        InputManager.OnTouchBegin -= OnTouchBegin;
        InputManager.OnTouchMove -= OnTouchMove;
        InputManager.OnTouchEnd -= OnTouchEnd;

        PlayButton.OnPressed -= PlayButtonOnPressed;
        PlayerSquadManager.OnEnterStrategyMode -= OnEnterStrategyMode;
        PlayerSquadManager.OnExitStrategyMode -= OnExitStrategyMode;
    }

    private void LateUpdate()
    {
        if (myFreeLookCameraState != FreeLookCameraState.TransitionIn)
        {
            xAxisMomentum = Mathf.Lerp(xAxisMomentum, 0f, Time.deltaTime);
            myCinemachineFreeLook.m_XAxis.Value += xAxisMomentum * Time.deltaTime;
        }
    }

    void OnTouchBegin(int fingerId, Vector3 tapPosition, RaycastHit hitInfo)
    {
        if (myFreeLookCameraState == FreeLookCameraState.TransitionIn)
        {
            return;
        }
        if (hitInfo.transform != null &&
            (isInStrategyMode || hitInfo.transform.gameObject.layer == LayerMask.NameToLayer("Player Squad")))
        {
            return;
        }

        if (myFingerId0 == InputManager.InactiveTouch)
        {
            myFingerId0 = fingerId;
            xAxisMomentum = 0f;
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

            prevTouchDeltaX = touchDelta.x * xAxisSpeed;
        }

        if (myFingerId0 == fingerId)
        {
            tapPosition0 = tapPosition;
            touchDelta0 = touchDelta;
            return;
        }

        prevTouchDeltaX = 0f;

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
            float pinchDistanceDelta = prevDistance - pinchDistance;

            myCinemachineFreeLook.m_Lens.OrthographicSize += pinchDistanceDelta * pinchSpeed;
            myCinemachineFreeLook.m_Lens.OrthographicSize = Mathf.Clamp(
                myCinemachineFreeLook.m_Lens.OrthographicSize, MinOrthoSize, MaxOrthoSize);
        }
    }

    void OnTouchEnd(int fingerId, Vector3 tapPosition, RaycastHit hitInfo)
    {
        if (myFingerId0 == fingerId)
        {
            myFingerId0 = InputManager.InactiveTouch;
            xAxisMomentum = prevTouchDeltaX * 10f;
        }
        if (myFingerId1 == fingerId)
        {
            myFingerId1 = InputManager.InactiveTouch;
        }
    }

    void OnEnterStrategyMode()
    {
        StopAllCoroutines();
        isInStrategyMode = true;
        if (Mathf.Abs(currentOrthoZoom) < Mathf.Epsilon)
        {
            currentOrthoZoom = myCinemachineFreeLook.m_Lens.OrthographicSize;
            StartCoroutine(ZoomLensAnimation(currentOrthoZoom - 10f));
        }
        else
        {
            StartCoroutine(ZoomLensAnimation(currentOrthoZoom));
        }
    }

    void OnExitStrategyMode()
    {
        StopAllCoroutines();
        isInStrategyMode = false;

        if (Mathf.Abs(currentOrthoZoom) < Mathf.Epsilon)
        {
            currentOrthoZoom = myCinemachineFreeLook.m_Lens.OrthographicSize;
            StartCoroutine(ZoomLensAnimation(currentOrthoZoom + 10f));
        }
        else
        {
            StartCoroutine(ZoomLensAnimation(currentOrthoZoom));
        }
    }

    void PlayButtonOnPressed()
    {
        StartCoroutine(TransitionInCamera());
    }

    IEnumerator ZoomLensAnimation(float destinationOrthoSize)
    {
        float timer = 0f;
        destinationOrthoSize = Mathf.Clamp(destinationOrthoSize,
            MinOrthoSize, MaxOrthoSize);

        while (timer < 1f)
        {
            timer += Time.unscaledDeltaTime * ZoomAnimationSpeed;
            myCinemachineFreeLook.m_Lens.OrthographicSize =
                Mathf.Lerp(myCinemachineFreeLook.m_Lens.OrthographicSize, destinationOrthoSize, timer);

            yield return null;
        }
        currentOrthoZoom = 0f;
    }

    IEnumerator TransitionInCamera()
    {
        myFreeLookCameraState = FreeLookCameraState.TransitionIn;

        float timer = 0f;
        xAxisMomentum = -360f;

        float startingOrtho = 160f;
        float destinationOrtho = 60f;
        Camera.main.orthographicSize = startingOrtho;
        myCinemachineFreeLook.m_Lens.OrthographicSize = startingOrtho;

        while (timer < 1f)
        {
            timer += Time.deltaTime;
            myCinemachineFreeLook.m_XAxis.Value += xAxisMomentum * Time.deltaTime;
            myCinemachineFreeLook.m_Lens.OrthographicSize = Mathf.Lerp(
                startingOrtho, destinationOrtho, timer);

            yield return new WaitForEndOfFrame();
        }

        myCinemachineFreeLook.m_Lens.OrthographicSize = destinationOrtho;

        myFreeLookCameraState = FreeLookCameraState.FreeLook;
    }
}