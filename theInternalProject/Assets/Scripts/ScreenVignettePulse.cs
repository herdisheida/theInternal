using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ScreenVignettePulse : MonoBehaviour
{
    public static ScreenVignettePulse instance;

    private Vignette vignette;

    public float pulseSpeed = 4f;
    public float maxPulseIntensity = 0.45f;

    private bool pulsating = false;

    void Awake()
    {
        instance = this;

        Volume volume = GetComponent<Volume>();

        if (volume.profile.TryGet(out Vignette v))
        {
            vignette = v;
        }
        else
        {
            Debug.LogError("Vignette not found in Volume!");
        }
    }

    void Update()
    {
        if (vignette == null) return;

        if (pulsating)
        {
            // Pulse between 0 → maxPulseIntensity
            float pulse = Mathf.Abs(Mathf.Sin(Time.time * pulseSpeed));
            vignette.intensity.value = Mathf.Lerp(0f, maxPulseIntensity, pulse);
        }
        else
        {
            // Smooth fade out
            vignette.intensity.value = Mathf.Lerp(vignette.intensity.value, 0f, Time.deltaTime * 6f);
        }
    }

    public void StartPulse()
    {
        pulsating = true;
    }

    public void StopPulse()
    {
        pulsating = false;
    }
}
