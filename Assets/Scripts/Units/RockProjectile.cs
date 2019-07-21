using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockProjectile : MonoBehaviour
{
    Transform destinationTransform;
    int opponentLayer;
    float damage;
    SquadUnit.DamageType damageType;

    public void SetDestination(Vector3 start, Transform end,
        float damage, SquadUnit.DamageType damageType)
    {
        transform.position = start;
        destinationTransform = end;
        opponentLayer = destinationTransform.gameObject.layer;
        this.damage = damage;
        this.damageType = damageType;

        StartCoroutine(Parabola(start, destinationTransform.position, 20f, 1f));
    }

    IEnumerator Parabola(Vector3 start, Vector3 end, float height, float duration)
    {
        float normalizedTime = 0.0f;
        while (normalizedTime < 1.0f)
        {
            float yOffset = height * 4.0f * (normalizedTime - normalizedTime * normalizedTime);
            transform.position = Vector3.Lerp(start, end, normalizedTime) + yOffset * Vector3.up;
            normalizedTime += Time.deltaTime / duration;
            yield return null;
        }
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == opponentLayer)
        {
            other.GetComponent<SquadUnit>().ReceiveDamage(damage, damageType);
        }
    }

}
