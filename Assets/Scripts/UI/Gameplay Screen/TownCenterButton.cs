using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownCenterButton : MoreButtonsButton
{
    public static event ButtonAction OnPressed;

    [SerializeField] GameObject RockSilhouette = null;
    [SerializeField] GameObject PaperSilhouette = null;
    [SerializeField] GameObject ScissorSilhouette = null;
    [SerializeField] GameObject EmptyIcon = null;

    [SerializeField] GameObject Armies = null;

    TownCenter myTownCenter;
    Coroutine shakeCoroutine = null;

    int currentArmyIndex = 0;
    List<GameObject> icons;
    const float spacing = 100f;

    protected override void Awake()
    {
        base.Awake();
        icons = new List<GameObject>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        RockSquadButton.OnPressed += DisableMoreButtons;
        PaperSquadButton.OnPressed += DisableMoreButtons;
        ScissorSquadButton.OnPressed += DisableMoreButtons;

        TownCenter.OnSpawnNewSquad += OnSpawnNewSquad;
        TownCenter.OnMaxSquadReached += OnMaxSquadReached;

        GameplayManager.OnIslandLoaded += OnIslandLoaded;

        DisableMoreButtons();
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        RockSquadButton.OnPressed -= DisableMoreButtons;
        PaperSquadButton.OnPressed -= DisableMoreButtons;
        ScissorSquadButton.OnPressed -= DisableMoreButtons;

        TownCenter.OnSpawnNewSquad -= OnSpawnNewSquad;
        TownCenter.OnMaxSquadReached -= OnMaxSquadReached;

        GameplayManager.OnIslandLoaded -= OnIslandLoaded;

        RemoveIcons();
    }

    void OnIslandLoaded()
    {
        myTownCenter = FindObjectOfType<TownCenter>();
        InitializeArmyIcons();
    }

    void OnSpawnNewSquad(Squad newSquad)
    {
        switch (newSquad.RPSType)
        {
            case Squad.SquadType.Rock:
                InstantiatePlayerSquadIcon(RockSilhouette);
                break;
            case Squad.SquadType.Paper:
                InstantiatePlayerSquadIcon(PaperSilhouette);
                break;
            case Squad.SquadType.Scissor:
                InstantiatePlayerSquadIcon(ScissorSilhouette);
                break;
        }

    }

    void InitializeArmyIcons()
    {
        if (myTownCenter == null)
        {
            myTownCenter = FindObjectOfType<TownCenter>();
        }

        icons = new List<GameObject>();
        currentArmyIndex = 0;
        if (icons.Count == 0)
        {
            float startPosition = (myTownCenter.MaximumSquadNumber - 1) * spacing / -2f;
            for (int i = 0; i < myTownCenter.MaximumSquadNumber; ++i)
            {
                GameObject clone = Instantiate(EmptyIcon);
                clone.transform.SetParent(Armies.transform, false);

                clone.transform.localPosition = new Vector3(
                    startPosition + i * spacing, 0f, 0f);

                icons.Add(clone);
            }
        }
    }

    void InstantiatePlayerSquadIcon(GameObject squadIcon)
    {
        GameObject clone = Instantiate(squadIcon);
        clone.transform.SetParent(Armies.transform, false);
        clone.transform.localPosition = icons[currentArmyIndex].transform.localPosition;

        Destroy(icons[currentArmyIndex]);
        icons[currentArmyIndex] = clone;

        ++currentArmyIndex;
        if (currentArmyIndex >= icons.Count)
        {
            currentArmyIndex = 0;
        }
    }

    void RemoveIcons()
    {
        for (int i = 0; i < icons.Count; ++i)
        {
            Destroy(icons[i]);
        }
        icons.Clear();
    }

    public override void ButtonPressAction()
    {
        base.ButtonPressAction();

        OnPressed?.Invoke();
    }

    void OnMaxSquadReached()
    {
        if (shakeCoroutine == null)
        {
            shakeCoroutine = StartCoroutine(ShakeButtonCoroutine());
        }
    }

    IEnumerator ShakeButtonCoroutine()
    {
        float timer = 0f;
        Vector3 startingLocalPosition = transform.localPosition;
        Vector3 tempLocalPosition = startingLocalPosition;

        while (timer < 1f)
        {
            timer += Time.deltaTime * 10f;
            tempLocalPosition.x = startingLocalPosition.x + Random.Range(-50f, 50f);
            tempLocalPosition.y = startingLocalPosition.y + Random.Range(-50f, 50f);

            transform.localPosition = tempLocalPosition;

            yield return null;
        }

        transform.localPosition = startingLocalPosition;
        shakeCoroutine = null;
    }
}
