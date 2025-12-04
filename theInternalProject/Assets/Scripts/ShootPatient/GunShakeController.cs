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
    public Image fadeImage;                     // full-screen black Image

    [Header("Shooting Message Hint")]
    public Graphic[] shootingMsgGraphics;       // box + text
    public float shootingMsgBlinkSpeed = 3f;    // blink speed

    [Header("Sprites")]
    public Sprite idleGunSprite;                // normal hand gun sprite
    public Sprite shootGunSprite;               // flash sprite when shooting
    public float shootFlashDuration = 0.1f;

    [Header("Shake Settings")]
    public float totalDuration = 7f;                // how long the player has to shoot
    public float minShakeAmount = 2f;               // starting shake
    public float maxShakeAmount = 15f;              // shake near the end
    public float backgroundShakeMultiplier = 0.4f;  // background shakes less

    [Header("Rotation Shake")]  // in degrees
    public float minRotation = 1f;
    public float maxRotation = 6f;

    [Header("Background Tint")]
    public Color maxRedTint = new Color(1f, 0.2f, 0.2f, 1f); // target colour at end

    [Header("BlackScreen & Scene")]
    public float blackHoldDuration = 4f;
    public string nextSceneName = "PatientSelection";


    // original transforms/colors
    private Vector2 gunOriginalPos;
    private float gunOriginalRotZ;
    private Vector2 bgOriginalPos;
    private Color bgOriginalColor;
    private Color fadeOriginalColor;
    private Color[] shootingMsgOriginalColors;

    // runtime variables
    private float elapsed = 0f;
    private bool isRunning = false; // hand is shaking
    private bool hasShot = false;


    void Start()
    {
       if (gunTransform == null) { gunTransform = GetComponent<RectTransform>(); }
        if (gunImage == null) { gunImage = GetComponent<Image>(); }

        // store original transforms
        gunOriginalPos = gunTransform.anchoredPosition;
        gunOriginalRotZ = gunTransform.localEulerAngles.z;

        if (backgroundTransform != null) { bgOriginalPos = backgroundTransform.anchoredPosition; }
        if (backgroundImage != null) { bgOriginalColor = backgroundImage.color; }
            
        if (fadeImage != null) { fadeOriginalColor = fadeImage.color; }

        if (shootingMsgGraphics != null && shootingMsgGraphics.Length > 0)
        {
            shootingMsgOriginalColors = new Color[shootingMsgGraphics.Length];
            for (int i = 0; i < shootingMsgGraphics.Length; i++)
            {
                if (shootingMsgGraphics[i] != null)
                    shootingMsgOriginalColors[i] = shootingMsgGraphics[i].color;
            }
        }
        // default idle sprite if not set
        if (idleGunSprite == null && gunImage != null) { idleGunSprite = gunImage.sprite; }

        StartShake();
    }

    void Update()
    {
        if (!isRunning)
        {
            UpdateShootingMsgBlink(false); // hint off when not active
            return;
        }

        // timer
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / totalDuration);

        // increasing shaking intensity
        float posIntensity = Mathf.Lerp(minShakeAmount, maxShakeAmount, t);
        float rotIntensity = Mathf.Lerp(minRotation, maxRotation, t);

        // shake gun
        Vector2 gunOffset = Random.insideUnitCircle * posIntensity;
        float gunRotOffset = Random.Range(-rotIntensity, rotIntensity);

        gunTransform.anchoredPosition = gunOriginalPos + gunOffset;
        Vector3 euler = gunTransform.localEulerAngles;
        euler.z = gunOriginalRotZ + gunRotOffset;
        gunTransform.localEulerAngles = euler;

        // shake background
        if (backgroundTransform != null)
        {
            Vector2 bgOffset = Random.insideUnitCircle * (posIntensity * backgroundShakeMultiplier);
            backgroundTransform.anchoredPosition = bgOriginalPos + bgOffset;
        }

        // tint background red over time
        if (backgroundImage != null)
        {
            float tintAmount = Mathf.Clamp01(t * 2f);
            backgroundImage.color = Color.Lerp(bgOriginalColor, maxRedTint, tintAmount);
        }

        // blink the shooting message while shaking
        UpdateShootingMsgBlink(true);

        // if player shoots (space bar) OR time runs out =>>> shoot patient
        if (!hasShot && (Input.GetKeyDown(KeyCode.Space) || elapsed >= totalDuration))
        {
            StartCoroutine(HandleShotSequence());
        }
    }

    public void StartShake()
    {
        elapsed = 0f;
        isRunning = true;
        hasShot = false;

        // start heavy breathing audio
        AudioManager.instance?.HeavyBreathing();
    }

    IEnumerator HandleShotSequence()
    {
        hasShot = true;
    
        // stop heavy breathing audio
        AudioManager.instance?.StopSFX();
        // play shoot patient audio
        AudioManager.instance?.ShootPatient();

        // flash shooting sprite
        if (gunImage != null && shootGunSprite != null)
        {
            gunImage.sprite = shootGunSprite;
            yield return new WaitForSeconds(shootFlashDuration);
            gunImage.sprite = idleGunSprite;
        }

        EndShake();

        // instantly go black + exhale + change scene
        yield return StartCoroutine(BlackScreenAndGoToNextScene());
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
    }


    IEnumerator BlackScreenAndGoToNextScene()
    {
        // instantly turn screen black
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 1f; // fully opaque black
            fadeImage.color = c;
        }

        // play exhale sound after the shot
        AudioManager.instance?.DeepExhale();

        // Short pause to let the exhale play
        yield return new WaitForSeconds(blackHoldDuration);

        // load next scene
        SceneManager.LoadScene(nextSceneName);
    }

    // make the ShootingMsg box blink
    void UpdateShootingMsgBlink(bool active)
    {
        if (shootingMsgGraphics == null || shootingMsgGraphics.Length == 0)
            return;

        float a = 0f;

        if (active)
        {
            // alpha oscillates 0–1
            a = (Mathf.Sin(Time.time * shootingMsgBlinkSpeed) + 1f) / 2f;
        }

        for (int i = 0; i < shootingMsgGraphics.Length; i++)
        {
            var g = shootingMsgGraphics[i];
            if (g == null) continue;

            Color baseColor = (shootingMsgOriginalColors != null && i < shootingMsgOriginalColors.Length)
                ? shootingMsgOriginalColors[i]
                : g.color;

            baseColor.a = a;
            g.color = baseColor;
        }
    }

}
