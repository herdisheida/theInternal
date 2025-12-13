using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class HintFader : MonoBehaviour
{
    public CanvasGroup hintGroup;
    public float showAfterIdleSeconds = 0.8f;
    public float fadeDuration = 0.25f;

    float idleTimer;
    Coroutine fadeCoroutine;


    void Awake()
    {
        if (hintGroup != null)
        {
            hintGroup.alpha = 0f;
        }
    }

    void Update()
    {
        
        bool relevantInput = 
        Input.GetKey(KeyCode.Space) ||
        Input.GetKey(KeyCode.LeftArrow) ||
        Input.GetKey(KeyCode.RightArrow);

        if (relevantInput){

            idleTimer = 0f;

            if (hintGroup != null) hintGroup.alpha = 0f;

            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
                fadeCoroutine = null;
            }
            return;
        }
        
        idleTimer += Time.deltaTime;

        if (idleTimer >= showAfterIdleSeconds && hintGroup != null && hintGroup.alpha < 1f && fadeCoroutine == null)
        {
            fadeCoroutine = StartCoroutine(FadeIn());
        }
    }

    IEnumerator FadeIn()
    {
        float t = 0f;
        float startA = hintGroup.alpha;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float a = (fadeDuration <= 0) ? 1f : Mathf.Lerp(startA, 1f, t / fadeDuration);
            hintGroup.alpha = a;
            yield return null;
        }

        hintGroup.alpha = 1f;
        fadeCoroutine = null;
    }
}
