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

    [Header("Space Key Hint")]
    public Image spaceKeyImage;             // UI Image that shows the space bar
    public Sprite spaceBarUnpressedSprite;  // normal key
    public Sprite spaceBarPressedSprite;    // pressed key
    public float spaceKeyBlinkSpeed = 8f;   // how fast it switches


    [Header("Sprites")]
    public Sprite idleGunSprite;                // normal hand gun sprite
    public Sprite shootGunSprite;               // flash sprite when shooting
    public float shootFlashDuration = 0.1f;

    [Header("Shake Settings")]
    public float totalDuration = 8f;                // how long the player has to shoot
    public float minShakeAmount = 2f;               // starting shake
    public float maxShakeAmount = 15f;              // shake near the end
    public float backgroundShakeMultiplier = 0.4f;  // background shakes less

    [Header("Rotation Shake")]  // in degrees
    public float minRotation = 1f;
    public float maxRotation = 6f;

    [Header("Background Red Tint")]
    public Color maxRedTint = new Color(1f, 0.2f, 0.2f, 1f); // target colour at end

    [Header("BlackScreen & Scene")]

    // TODO HERDIS CHANGE AFTER ALPHA
    // public string nextSceneName = "PatientSelection";
    private string nextSceneName = "Credits";


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


    IEnumerator Start()
    {

       if (gunTransform == null) { gunTransform = GetComponent<RectTransform>(); }
        if (gunImage == null) { gunImage = GetComponent<Image>(); }

        // store original transforms
        gunOriginalPos = gunTransform.anchoredPosition;
        gunOriginalRotZ = gunTransform.localEulerAngles.z;

        if (backgroundTransform != null) { bgOriginalPos = backgroundTransform.anchoredPosition; }
        if (backgroundImage != null) { bgOriginalColor = backgroundImage.color; }
            
        if (fadeImage != null) { fadeOriginalColor = fadeImage.color; }

        // default idle sprite if not set
        if (idleGunSprite == null && gunImage != null) { idleGunSprite = gunImage.sprite; }

        // start with black screen for 2 seconds
        yield return StartCoroutine(BlackScreen(2f));

        // after black screen, start shaking
        StartShake();
    }

    void Update()
    {
        if (!isRunning)
        {
            // keep key in unpressed state when not running
            UpdateSpaceKeyBlink(false);
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

        // toggle the space key sprite
        UpdateSpaceKeyBlink(true);

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
        yield return StartCoroutine(BlackScreen(4f));
        // load next scene
        SceneManager.LoadScene(nextSceneName);
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


    IEnumerator BlackScreen(float blackHoldDuration)
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

        // fade back to transparent
        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }
    }

    // make the SpaceKey box blink
    void UpdateSpaceKeyBlink(bool active)
    {
        if (spaceKeyImage == null || spaceBarUnpressedSprite == null || spaceBarPressedSprite == null)
            return;

        if (!active)
        {
            // when not shaking -> display nothing
            spaceKeyImage.enabled = false;
            return;
        }

        spaceKeyImage.enabled = true;

        // use a sine wave to flip between sprites
        float v = Mathf.Sin(Time.time * spaceKeyBlinkSpeed);

        if (v > 0f)
            { spaceKeyImage.sprite = spaceBarPressedSprite; }
        else
            { spaceKeyImage.sprite = spaceBarUnpressedSprite; }
    }
}
