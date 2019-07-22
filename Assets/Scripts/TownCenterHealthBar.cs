using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownCenterHealthBar : MonoBehaviour
{
    [SerializeField] Transform HealthBarFill = null;
    Vector3 healthBarLocalScale;

    TownCenter myTownCenter;

    void Awake()
    {
        myTownCenter = GetComponentInParent<TownCenter>();
        healthBarLocalScale = Vector3.one;
    }

    void Update()
    {
        healthBarLocalScale.x = Mathf.Lerp(healthBarLocalScale.x,
            myTownCenter.Heatlh / 100f, Time.deltaTime * 10f);
        HealthBarFill.localScale = healthBarLocalScale;
    }

    private void LateUpdate()
    {
        transform.LookAt(Camera.main.transform);
    }
}
