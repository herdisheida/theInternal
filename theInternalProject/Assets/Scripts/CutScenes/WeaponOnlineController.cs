using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class WeaponOnlineController : MonoBehaviour
{
    [Header("UI References")]
    public Image fadeImage;            // full-screen black image
    public CanvasGroup hudGroup;       // panel that flickers on
    public TextMeshProUGUI statusText; // main HUD text
    public GameObject spaceKeyHint;    // the space-bar image + text container

    [Header("Gun Glow")]
    public DamageFlash gunFlash;       // DamageFlash on your GunIdle object
    public float glowDelay = 0.3f;

    [Header("Timings")]
    public float darkenDuration = 0.8f;
    public float flickerDuration = 0.6f;
    public float textHoldTime = 3f;

    [Header("Scene Flow")]
    public string nextSceneName = "BossBattle";
    public int requiredShots = 5;  // how many space presses before going to Boss Scene


    [Header("Player Character Reference")]
    public Transform player;


    void Start()
    {
        // start fully black, then fade IN so player + background are revealed
        if (fadeImage != null)
        {
            var c = fadeImage.color;
            c.a = 1f; // fully opaque to start
            fadeImage.color = c;
        }

        if (hudGroup != null)
            hudGroup.alpha = 0f;

        if (statusText != null)
            statusText.text = "";

        if (spaceKeyHint != null)
            spaceKeyHint.SetActive(false);

        // start the cutscene
        StartCoroutine(CutsceneRoutine());
    }


    void Update()
    {
        
    }


    IEnumerator CutsceneRoutine()
    {
        // Fade IN from black
        yield return StartCoroutine(FadeFromBlack());

        // HUD flicker on
        yield return StartCoroutine(HUDFlicker());

        // Message 1
        yield return StartCoroutine(ShowLine("Infection Concentration CRITICAL."));

        // Message 2
        yield return StartCoroutine(ShowLine("Activate the Power of Friendship."));

        // Gun glow
        yield return new WaitForSeconds(glowDelay);
        AudioManager.instance?.WeaponOnline();
        if (gunFlash != null)
            gunFlash.Flash();

        // Final prompt + space-key hint
        if (statusText != null)
            statusText.text = "PRESS SPACE TO SHOOT.";

        if (spaceKeyHint != null)
            spaceKeyHint.SetActive(true);


        // Wait for N shots (space presses), then go to the boss scene
        yield return StartCoroutine(WaitForShots(requiredShots));
        SceneManager.LoadScene(nextSceneName);
    }

    IEnumerator FadeFromBlack()
    {
        if (fadeImage == null)
            yield break;

        float t = 0f;
        Color c = fadeImage.color;

        while (t < darkenDuration)
        {
            t += Time.deltaTime;
            float a = Mathf.Lerp(1f, 0f, t / darkenDuration); // 1 -> 0
            c.a = a;
            fadeImage.color = c;
            yield return null;
        }

        // ensure fully transparent at the end
        c.a = 0f;
        fadeImage.color = c;
    }

    IEnumerator HUDFlicker()
    {
        if (hudGroup == null)
            yield break;

        float elapsed = 0f;
        bool on = false;

        while (elapsed < flickerDuration)
        {
            on = !on;
            hudGroup.alpha = on ? 1f : 0f;
            PlayBeep();

            float step = Random.Range(0.05f, 0.15f);
            elapsed += step;
            yield return new WaitForSeconds(step);
        }

        hudGroup.alpha = 1f; // stay on
    }


    // wait until the player has pressed Space requiredShots times
    IEnumerator WaitForShots(int requiredShots)
    {
        int shots = 0;

        while (shots < requiredShots)
        {
            if (Input.GetKeyDown(KeyCode.Space)) { shots++; }
            yield return null;
        }
    }


    IEnumerator ShowLine(string line)
    {
        if (statusText != null)
            statusText.text = line;

        PlayBeep();
        yield return new WaitForSeconds(textHoldTime);
    }

    void PlayBeep()
    {
        // beep for HUD flicker and text lines
        AudioManager.instance?.ButtonClick();
    }

}
