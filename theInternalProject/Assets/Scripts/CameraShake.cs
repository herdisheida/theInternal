using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake instance;

    private Vector3 originalPos;
    private float shakeTimer = 0f;
    private float shakeStrength = 0.1f;

    void Awake()
    {
        instance = this;
        originalPos = transform.localPosition;
    }

    void Update()
    {
        if (shakeTimer > 0)
        {
            transform.localPosition = originalPos + (Vector3)Random.insideUnitCircle * shakeStrength;
            shakeTimer -= Time.deltaTime;
        }
        else
        {
            transform.localPosition = originalPos;
        }
    }

    public void Shake(float duration, float strength)
    {
        shakeTimer = duration;
        shakeStrength = strength;
    }
}
