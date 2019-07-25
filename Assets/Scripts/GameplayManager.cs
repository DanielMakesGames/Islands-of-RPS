using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public delegate void GameplayManagerAction();
    public static event GameplayManagerAction OnGameplayStart;

    [SerializeField] GameObject[] Islands = null;

    IslandItemSelect currentIslandItemSelect = null;
    GameObject currentIsland = null;

    private void OnEnable()
    {
        PlayButton.OnPressed += PlayButtonOnPressed;
        ReturnToTitleButton.OnPressed += ReturnToTitleButtonOnPressed;
        IslandItemSelect.OnIslandSelected += OnIslandSelected;
    }

    private void OnDisable()
    {
        PlayButton.OnPressed -= PlayButtonOnPressed;
        ReturnToTitleButton.OnPressed -= ReturnToTitleButtonOnPressed;
        IslandItemSelect.OnIslandSelected -= OnIslandSelected;
    }

    void OnIslandSelected(IslandItemSelect island)
    {
        currentIslandItemSelect = island;
    }

    void PlayButtonOnPressed()
    {
        for (int i = 0; i < Islands.Length; ++i)
        {
            if (Islands[i].name == currentIslandItemSelect.IslandName)
            {
                currentIsland = Instantiate(Islands[i]);
                break;
            }
        }

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
