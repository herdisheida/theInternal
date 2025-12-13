using System.Collections;
using UnityEngine;

public class HintFader : MonoBehaviour
{
    public CanvasGroup hintGroup;
    public float delayBeforeFade = 0f;
    public float idleTime = 1f;
    public float fadeDuration = 0f;

    void Start()
    {
        StartCoroutine(FadeInDelayed());
    }

    void Update()
    {
        if (idleTime == 0 || hintGroup.alpha ==  0){
            StopCoroutine(FadeInDelayed());
        }
        else
        {
            idleTime += Time.deltaTime;
        }
    }

    IEnumerator FadeInDelayed()
    {
        yield return new WaitForSeconds(delayBeforeFade);
        
        float tim = 0f;
        while (tim < fadeDuration)
        {
            tim += Time.deltaTime;
            float a = tim / fadeDuration;
            hintGroup.alpha = a;
            yield return null;
        }
        hintGroup.alpha = 1f;
    }
}
