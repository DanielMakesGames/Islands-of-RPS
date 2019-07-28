using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public delegate void GameplayManagerAction();
    public static event GameplayManagerAction OnIslandLoaded;
    public static event GameplayManagerAction OnStartGameplay;

    [SerializeField] GameObject[] Islands = null;
    [SerializeField] SkyboxColor[] SkyboxColors = null;

    IslandItemSelect currentIslandItemSelect = null;
    GameObject currentIsland = null;

    int myFingerId = InputManager.InactiveTouch;

    bool hasGameStarted = false;

    private void OnEnable()
    {
        PlayButton.OnPressed += PlayButtonOnPressed;
        ReturnToTitleButton.OnPressed += ReturnToTitleButtonOnPressed;
        IslandItemSelect.OnIslandSelected += OnIslandSelected;

        InputManager.OnTouchBegin += InputManager_OnTouchBegin;
        InputManager.OnTouchEnd += InputManager_OnTouchEnd;

        StartCoroutine(TransitionSkyboxColor(SkyboxColors[0]));
    }

    private void OnDisable()
    {
        PlayButton.OnPressed -= PlayButtonOnPressed;
        ReturnToTitleButton.OnPressed -= ReturnToTitleButtonOnPressed;
        IslandItemSelect.OnIslandSelected -= OnIslandSelected;

        InputManager.OnTouchBegin -= InputManager_OnTouchBegin;
        InputManager.OnTouchEnd -= InputManager_OnTouchEnd;
    }

    void InputManager_OnTouchBegin(int fingerId, Vector3 tapPosition, RaycastHit hitInfo)
    {
        myFingerId = fingerId;
    }

    void InputManager_OnTouchEnd(int fingerId, Vector3 tapPosition, RaycastHit hitInfo)
    {
        if (myFingerId == fingerId)
        {
            if (!hasGameStarted)
            {
                hasGameStarted = true;
                OnStartGameplay?.Invoke();
            }
        }
    }

    void OnIslandSelected(IslandItemSelect island)
    {
        currentIslandItemSelect = island;
    }

    void PlayButtonOnPressed()
    {
        StartCoroutine(IslandLoadedCoroutine());
    }

    IEnumerator IslandLoadedCoroutine()
    {
        yield return null;
        hasGameStarted = false;

        for (int i = 0; i < Islands.Length; ++i)
        {
            if (Islands[i].name == currentIslandItemSelect.IslandName)
            {
                currentIsland = Instantiate(Islands[i]);
                StartCoroutine(TransitionSkyboxColor(SkyboxColors[i]));
                break;
            }
        }

        currentIsland.transform.parent = transform;
        currentIsland.transform.localPosition = Vector3.zero;
        currentIsland.transform.localRotation = Quaternion.identity;

        OnIslandLoaded?.Invoke();
    }

    IEnumerator TransitionSkyboxColor(SkyboxColor newSkyboxColor)
    {
        float timer = 0f;
        Color startingTopColor = RenderSettings.skybox.GetColor("_Color2");
        Color startingBottomColor = RenderSettings.skybox.GetColor("_Color1");
        float startingExponent = RenderSettings.skybox.GetFloat("_Exponent");

        while (timer < 1f)
        {
            timer += Time.deltaTime;
            RenderSettings.skybox.SetColor("_Color2",
                Color.Lerp(startingTopColor, newSkyboxColor.TopColor, timer));
            RenderSettings.skybox.SetColor("_Color1",
                Color.Lerp(startingBottomColor, newSkyboxColor.BottomColor, timer));
            RenderSettings.skybox.SetFloat("_Exponent",
                Mathf.Lerp(startingExponent, newSkyboxColor.Exponent, timer));

            yield return null;
        }
    }

    void ReturnToTitleButtonOnPressed()
    {
        StartCoroutine(TransitionSkyboxColor(SkyboxColors[0]));
        Destroy(currentIsland);
    }
}
