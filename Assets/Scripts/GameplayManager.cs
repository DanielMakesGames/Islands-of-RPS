using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public delegate void GameplayManagerAction();
    public static event GameplayManagerAction OnGameplayStart;

    [SerializeField] GameObject[] Islands = null;

    private void OnEnable()
    {
        PlayButton.OnPressed += PlayButtonOnPressed;
    }

    private void OnDisable()
    {
        PlayButton.OnPressed -= PlayButtonOnPressed;
    }

    void PlayButtonOnPressed()
    {
        GameObject island = Instantiate(Islands[0]);
        island.transform.parent = transform;
        island.transform.localPosition = Vector3.zero;
        island.transform.localRotation = Quaternion.identity;

        OnGameplayStart?.Invoke();
    }

}
