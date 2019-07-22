using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public delegate void GameplayManagerAction();
    public static event GameplayManagerAction OnGameplayStart;

    [SerializeField] GameObject[] Islands = null;

    GameObject currentIsland = null;

    private void OnEnable()
    {
        PlayButton.OnPressed += PlayButtonOnPressed;
        ReturnToTitleButton.OnPressed += ReturnToTitleButtonOnPressed;
    }

    private void OnDisable()
    {
        PlayButton.OnPressed -= PlayButtonOnPressed;
        ReturnToTitleButton.OnPressed -= ReturnToTitleButtonOnPressed;
    }

    void PlayButtonOnPressed()
    {
        currentIsland = Instantiate(Islands[0]);
        currentIsland.transform.parent = transform;
        currentIsland.transform.localPosition = Vector3.zero;
        currentIsland.transform.localRotation = Quaternion.identity;

        OnGameplayStart?.Invoke();
    }

    void ReturnToTitleButtonOnPressed()
    {
        Destroy(currentIsland);
    }

}
