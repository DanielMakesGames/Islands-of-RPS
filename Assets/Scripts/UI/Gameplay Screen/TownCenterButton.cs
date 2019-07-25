using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TownCenterButton : MoreButtonsButton
{
    public static event ButtonAction OnPressed;
    [SerializeField] TextMeshProUGUI textMesh = null;
    TownCenter myTownCenter;
    Coroutine shakeCoroutine = null;

    protected override void OnEnable()
    {
        base.OnEnable();

        myTownCenter = FindObjectOfType<TownCenter>();

        RockSquadButton.OnPressed += DisableMoreButtons;
        PaperSquadButton.OnPressed += DisableMoreButtons;
        ScissorSquadButton.OnPressed += DisableMoreButtons;

        TownCenter.OnSpawnNewSquad += OnSpawnNewSquad;
        TownCenter.OnMaxSquadReached += OnMaxSquadReached;

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
    }

    void OnSpawnNewSquad(Squad newSquad)
    {
        if (myTownCenter == null)
        {
            myTownCenter = FindObjectOfType<TownCenter>();
        }
        textMesh.text = "Armies\n" + myTownCenter.CurrentSquadNumber +
            " / " + myTownCenter.MaximumSquadNumber;
    }

    void OnMaxSquadReached()
    {
        if (shakeCoroutine == null)
        {
            shakeCoroutine = StartCoroutine(ShakeButtonCoroutine());
        }
    }

    public override void ButtonPressAction()
    {
        base.ButtonPressAction();

        OnPressed?.Invoke();
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
