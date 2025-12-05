using System.Collections;
using UnityEngine;

public class GunPowerupFlash : MonoBehaviour
{
    [Header("Scale Settings")]
    public float appearScale = 1f;      // final normal size (usually 1)
    public float bigScale = 1.3f;       // peak size during pulse
    public int pulseCount = 3;          // how many big/small pulses
    public float pulseDuration = 0.12f; // time for half a pulse (up or down)

    private Vector3 originalScale;

    void Awake()
    {
        // remember original scale and start “hidden”
        originalScale = transform.localScale;
        transform.localScale = Vector3.zero; // invisible until powerup
    }

    public IEnumerator PlayPowerup()
    {
        // pop in from 0 -> appearScale
        float t = 0f;
        while (t < pulseDuration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / pulseDuration);
            float s = Mathf.Lerp(0f, appearScale, k);
            transform.localScale = new Vector3(s, s, s);
            yield return null;
        }

        // Pulsing big/small Mario-style
        for (int i = 0; i < pulseCount; i++)
        {
            // appearScale -> bigScale
            t = 0f;
            while (t < pulseDuration)
            {
                t += Time.deltaTime;
                float k = Mathf.Clamp01(t / pulseDuration);
                float s = Mathf.Lerp(appearScale, bigScale, k);
                transform.localScale = new Vector3(s, s, s);
                yield return null;
            }

            // bigScale -> appearScale
            t = 0f;
            while (t < pulseDuration)
            {
                t += Time.deltaTime;
                float k = Mathf.Clamp01(t / pulseDuration);
                float s = Mathf.Lerp(bigScale, appearScale, k);
                transform.localScale = new Vector3(s, s, s);
                yield return null;
            }
        }

        // make sure we end on a nice clean scale
        transform.localScale = new Vector3(appearScale, appearScale, appearScale);
    }
}
