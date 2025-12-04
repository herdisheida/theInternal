using System.Collections;
using UnityEngine;

public class HintFader : MonoBehaviour
{
    public CanvasGroup hintGroup;
    public float delayBeforeFade = 5f;
    public float fadeDuration = 1f;

    void Start()
    {
        StartCoroutine(FadeInDelayed());
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
