using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IslandItemSelect : ItemSelect
{
    public delegate void IslandSelectedAction(IslandItemSelect island);
    public static event IslandSelectedAction OnIslandSelected;

    [SerializeField] string islandName = "";
    public string IslandName
    {
        get { return islandName; }
    }

    public bool IsUnlocked
    {
        get { return SaveData.GetInt(islandName + SaveData.IsIslandUnlockedKey, 0) == 1; }
    }

    Material[] myDefaultMaterials;
    TitleScreen myTitleScreen;

    protected override void Awake()
    {
        base.Awake();
        myTitleScreen = GetComponentInParent<TitleScreen>();

        myDefaultMaterials = new Material[myMeshRenderers.Length];
        for (int i = 0; i < myMeshRenderers.Length; ++i)
        {
            myDefaultMaterials[i] = myMeshRenderers[i].material;
        }
    }

    private void Start()
    {
        if (!IsUnlocked)
        {
         //   LockIsland();
        }
    }

    void OnPurchaseCharacter()
    {
        if (IsUnlocked)
        {
            UnlockIsland();
        }
    }

    protected override void ScaleUp()
    {
        base.ScaleUp();
        OnIslandSelected?.Invoke(this);
    }

    void LockIsland()
    {
        for (int i = 0; i < myMeshRenderers.Length; ++i)
        {
            myMeshRenderers[i].material = myTitleScreen.GrayscaleMaterial;
        }
    }

    void UnlockIsland()
    {
        for (int i = 0; i < myMeshRenderers.Length; ++i)
        {
            myMeshRenderers[i].material = myDefaultMaterials[i];
        }
        ChangeColorWhite();
    }
}
