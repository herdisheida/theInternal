using UnityEngine;
using UnityEngine.UI;

public class GunShakeController : MonoBehaviour
{
    [Header("References")]
    public RectTransform gunTransform;        // assign GunImage RectTransform
    public RectTransform backgroundTransform; // assign Background RectTransform (optional)

    [Header("Shake Settings")]
    public float totalDuration = 10f;                // how long the player has to shoot
    public float minShakeAmount = 2f;               // starting shake
    public float maxShakeAmount = 15f;              // shake near the end
    public float backgroundShakeMultiplier = 0.4f;  // background shakes less

    [Header("Rotation Shake")]  // in degrees
    public float minRotation = 1f;
    public float maxRotation = 6f;

    private Vector2 gunOriginalPos;
    private float gunOriginalRotZ;
    private Vector2 bgOriginalPos;

    private float elapsed = 0f;
    private bool isRunning = false; // hand is shaking
    private bool hasShot = false;


    void Start()
    {
        if (gunTransform == null)
            gunTransform = GetComponent<RectTransform>();

        // store original transforms
        gunOriginalPos = gunTransform.anchoredPosition;
        gunOriginalRotZ = gunTransform.localEulerAngles.z;

        if (backgroundTransform != null)
            bgOriginalPos = backgroundTransform.anchoredPosition;

        StartShake();
    }

    void Update()
    {
        if (!isRunning)
            return;

        // timer
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / totalDuration);

        // increasing intensity
        float posIntensity = Mathf.Lerp(minShakeAmount, maxShakeAmount, t);
        float rotIntensity = Mathf.Lerp(minRotation, maxRotation, t);

        // random offset for gun
        Vector2 gunOffset = Random.insideUnitCircle * posIntensity;
        float gunRotOffset = Random.Range(-rotIntensity, rotIntensity);

        gunTransform.anchoredPosition = gunOriginalPos + gunOffset;
        Vector3 euler = gunTransform.localEulerAngles;
        euler.z = gunOriginalRotZ + gunRotOffset;
        gunTransform.localEulerAngles = euler;

        // background shake
        if (backgroundTransform != null)
        {
            Vector2 bgOffset = Random.insideUnitCircle * (posIntensity * backgroundShakeMultiplier);
            backgroundTransform.anchoredPosition = bgOriginalPos + bgOffset;
        }

        // listen for shoot input
        if (!hasShot && Input.GetKeyDown(KeyCode.Space))
        {
            OnShoot();
        }

        // time up
        if (elapsed >= totalDuration)
        {
            EndShake();
        }
    }

    public void StartShake()
    {
        elapsed = 0f;
        isRunning = true;
        hasShot = false;
    }

    void OnShoot()
    {
        hasShot = true;
        Debug.Log("SHOT!");

        // TODO: play SFX, spawn bullet, go to next scene (with patient dead)
        // AudioManager.instance.ShootPatient();
        EndShake();
    }

    void EndShake()
    {
        isRunning = false;

        // reset positions/rotation of gun
        gunTransform.anchoredPosition = gunOriginalPos;
        gunTransform.localEulerAngles = new Vector3(
            gunTransform.localEulerAngles.x,
            gunTransform.localEulerAngles.y,
            gunOriginalRotZ
        );

        // reset positions/rotation of background
        if (backgroundTransform != null)
            backgroundTransform.anchoredPosition = bgOriginalPos;

        if (!hasShot)
        {
            Debug.Log("Player did NOT shoot in time!");
            OnShoot(); // auto-shoot
        }
    }
}
