using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TownCenterButton : MoreButtonsButton
{
    public static event ButtonAction OnPressed;

    [SerializeField] GameObject RockSilhouette = null;
    [SerializeField] GameObject PaperSilhouette = null;
    [SerializeField] GameObject ScissorSilhouette = null;

    [SerializeField] TextMeshProUGUI textMesh = null;

    TownCenter myTownCenter;
    Coroutine shakeCoroutine = null;

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

        DisableMoreButtons();

        myTownCenter = FindObjectOfType<TownCenter>();
        StartCoroutine(UpdateTextDelayed());
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        RockSquadButton.OnPressed -= DisableMoreButtons;
        PaperSquadButton.OnPressed -= DisableMoreButtons;
        ScissorSquadButton.OnPressed -= DisableMoreButtons;

        TownCenter.OnSpawnNewSquad -= OnSpawnNewSquad;
        TownCenter.OnMaxSquadReached -= OnMaxSquadReached;

        RemoveIcons();
    }

    void OnSpawnNewSquad(Squad newSquad, TownCenter.SquadType squadType)
    {
        switch (squadType)
        {
            case TownCenter.SquadType.Rock:
                InstantiatePlayerSquadIcon(RockSilhouette);
                break;
            case TownCenter.SquadType.Paper:
                InstantiatePlayerSquadIcon(PaperSilhouette);
                break;
            case TownCenter.SquadType.Scissor:
                InstantiatePlayerSquadIcon(ScissorSilhouette);
                break;
        }

        UpdateText();
    }

    void UpdateText()
    {
        if (myTownCenter == null)
        {
            myTownCenter = FindObjectOfType<TownCenter>();
        }

        textMesh.text = "Armies\n" + myTownCenter.CurrentSquadNumber +
            " / " + myTownCenter.MaximumSquadNumber;
    }

    void InstantiatePlayerSquadIcon(GameObject squadIcon)
    {
        GameObject clone = Instantiate(squadIcon);
        clone.transform.SetParent(transform, false);

        icons.Add(clone);

        float startPosition = (icons.Count - 1) * spacing / -2f;
        for (int i = 0; i < icons.Count; ++i)
        {
            icons[i].transform.localPosition = new Vector3(
                startPosition + i * spacing, -150f, 0f);
            clone.transform.localRotation = Quaternion.identity;
            clone.transform.localScale = Vector3.one * 15f;
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

    IEnumerator UpdateTextDelayed()
    {
        yield return null;
        UpdateText();
    }
}
