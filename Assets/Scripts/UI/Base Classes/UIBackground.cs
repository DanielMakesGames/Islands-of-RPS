using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIBackground : MonoBehaviour
{
    Image myImage;
    Collider myCollider;
    Color myColor;
    Color fadeInColor;

    int fadeIns = 0;
    const float fadeInSpeed = 3f;
    const float fadeOutSpeed = 10f;

    void Awake()
    {
        myImage = GetComponent<Image>();
        myCollider = GetComponent<Collider>();
        myColor = myImage.color;
        fadeInColor = myColor;

        myImage.enabled = true;
        myCollider.enabled = true;
    }

    private void OnEnable()
    {
        PlayButton.OnPressed += FadeOut;
    }

    private void OnDisable()
    {
        PlayButton.OnPressed -= FadeOut;
    }

    protected void FadeIn()
    {
        ++fadeIns;
        if (fadeIns > 0)
        {
            myImage.enabled = true;
            myCollider.enabled = true;
            StopAllCoroutines();
            StartCoroutine(FadeInCoroutine());
        }
    }

    protected void FadeOut()
    {
        --fadeIns;
        if (fadeIns <= 0)
        {
            fadeIns = 0;
            myCollider.enabled = false;
            StopAllCoroutines();
            StartCoroutine(FadeOutCoroutine());
        }
    }

    protected void FadeOutInstant()
    {
        fadeIns = 0;
        myCollider.enabled = false;
        StopAllCoroutines();
        myColor.a = 0f;
        myImage.color = myColor;
        myImage.enabled = false;
    }

    IEnumerator FadeInCoroutine()
    {
        float timer = 0f;
        Color startingColor = myColor;

        while (timer < 1f)
        {
            timer += Time.deltaTime * fadeInSpeed;
            myColor = Color.Lerp(startingColor, fadeInColor, timer);
            myImage.color = myColor;

            yield return null;
        }

        myColor = fadeInColor;
        myImage.color = myColor;
    }

    IEnumerator FadeOutCoroutine()
    {
        float timer = 0f;
        float startingAlpha = myColor.a;

        while (timer < 1f)
        {
            timer += Time.deltaTime * fadeOutSpeed;
            myColor.a = Mathf.Lerp(startingAlpha, 0f, timer);
            myImage.color = myColor;

            yield return null;
        }

        myColor.a = 0f;
        myImage.color = myColor;
        myImage.enabled = false;
    }
}