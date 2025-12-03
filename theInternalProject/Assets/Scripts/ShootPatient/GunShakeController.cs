using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GunShakeController : MonoBehaviour
{
    [Header("References")]
    public Image gunImage;                      // GunImage Image component
    public RectTransform gunTransform;          // GunImage RectTransform
    public Image backgroundImage;               // Background Image component
    public RectTransform backgroundTransform;   // Background RectTransform

    [Header("Sprites")]
    public Sprite idleGunSprite;                // normal hand gun sprite
    public Sprite shootGunSprite;               // flash sprite when shooting
    public float shootFlashDuration = 0.1f;

    [Header("Shake Settings")]
    public float totalDuration = 10f;                // how long the player has to shoot
    public float minShakeAmount = 2f;               // starting shake
    public float maxShakeAmount = 15f;              // shake near the end
    public float backgroundShakeMultiplier = 0.4f;  // background shakes less

    [Header("Rotation Shake")]  // in degrees
    public float minRotation = 1f;
    public float maxRotation = 6f;

    [Header("Background Tint")]
    public Color maxRedTint = new Color(1f, 0.2f, 0.2f, 1f); // target colour at end

    private Vector2 gunOriginalPos;
    private float gunOriginalRotZ;
    private Vector2 bgOriginalPos;
    private Color bgOriginalColor;

    private float elapsed = 0f;
    private bool isRunning = false; // hand is shaking
    private bool hasShot = false;


    void Start()
    {
       if (gunTransform == null)
            gunTransform = GetComponent<RectTransform>();
        if (gunImage == null)
            gunImage = GetComponent<Image>();

        // store original transforms
        gunOriginalPos = gunTransform.anchoredPosition;
        gunOriginalRotZ = gunTransform.localEulerAngles.z;

        if (backgroundTransform != null)
            bgOriginalPos = backgroundTransform.anchoredPosition;
        if (backgroundImage != null)
            bgOriginalColor = backgroundImage.color;

        // default idle sprite if not set
        if (idleGunSprite == null && gunImage != null)
            idleGunSprite = gunImage.sprite;

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

        // if shoot (press space) OR time up
        if (!hasShot && Input.GetKeyDown(KeyCode.Space) || elapsed >= totalDuration)
        {
            OnShoot();
        }
    }

    public void StartShake()
    {
        elapsed = 0f;
        isRunning = true;
        hasShot = false;

        // start heavy breathing audio
        if (AudioManager.instance != null) { AudioManager.instance.HeavyBreathing(); }
    }

    void OnShoot()
    {
        hasShot = true;
        Debug.Log("SHOT!");
    
        // sound effect
        if (AudioManager.instance != null) { AudioManager.instance.ShootPatient(); }

        // change to shooting sprite briefly
        if (gunImage != null && shootGunSprite != null)
        {
            gunImage.sprite = shootGunSprite;
            yield return new WaitForSeconds(shootFlashDuration);
            gunImage.sprite = idleGunSprite;
        }

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

        // reset positions/rotation/color of background
        if (backgroundTransform != null)
            backgroundTransform.anchoredPosition = bgOriginalPos;
        if (backgroundImage != null)
            backgroundImage.color = bgOriginalColor;

        // stop breathing sound
        if (AudioManager.instance != null) { AudioManager.instance.HeavyBreathing(); }
    }
}
