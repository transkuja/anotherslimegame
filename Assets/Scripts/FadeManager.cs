using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeManager : MonoBehaviour {
    RawImage fadeImage;

    private void OnLevelWasLoaded(int level)
    {
        FadeIn(0.2f);
    }

    public void Init()
    {
        fadeImage = transform.GetChild(0).GetComponent<RawImage>();
    }

    public void FadeOut(float fadeTime, Action toCallOnEnd = null, bool stayVisibleAtTheEnd = true)
    {
        StartCoroutine(FadeOutCoroutine(fadeTime, toCallOnEnd, stayVisibleAtTheEnd));
    }

    public void FadeIn(float fadeTime, Action toCallOnEnd = null)
    {
        StartCoroutine(FadeInCoroutine(fadeTime, toCallOnEnd));
    }


    public IEnumerator FadeInCoroutine(float fadeTime, Action toCallOnEnd = null)
    {
        if(fadeImage == null)
            Init();

        StopCoroutine("FadeOut");
        float timer = 0.0f;
        
        Color color = fadeImage.color;

        while (timer < fadeTime)
        {
            timer += Time.unscaledDeltaTime;
            color.a = Mathf.Lerp(1f, 0f, timer / fadeTime);
            fadeImage.color = color;
            yield return null;
        }
        color.a = 0f;
        fadeImage.color = color;

        if (toCallOnEnd != null)
            toCallOnEnd();
    }

    public IEnumerator FadeOutCoroutine(float fadeTime, Action toCallOnEnd = null, bool stayVisibleAtTheEnd = false)
    {
        if (fadeImage == null)
            Init();

        StopCoroutine("FadeIn");

        float timer = 0.0f;

        Color color = fadeImage.color;

        while (timer < fadeTime)
        {
            timer += Time.unscaledDeltaTime;
            color.a = Mathf.Lerp(0f, 1f, timer / fadeTime);
            fadeImage.color = color;
            yield return null;
        }
        color.a = stayVisibleAtTheEnd ? 1f : 0f;
        fadeImage.color = color;

        if (toCallOnEnd != null)
            toCallOnEnd();
    }
}
