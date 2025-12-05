using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class WeaponOnlineController : MonoBehaviour
{
    [Header("UI References")]
    public CanvasGroup hudGroup;       // panel that flickers on
    public TextMeshProUGUI statusText; // main HUD text
    public GameObject spaceKeyHint;    // the space-bar image + text container

    [Header("Gun Powerup")]
    public GunPowerupFlash gunPowerup; // reference gun-powerup script

    [Header("Timings")]
    public float textFadeTime = 0.8f;  // fade in/out time for each line
    public float textHoldTime = 4f;    // how long each message stays fully visible

    [Header("Scene Flow")]
    public string nextSceneName = "BossBattle";
    public int requiredShots = 5;  // how many space presses before going to Boss Scene


    [Header("Player Character Reference")]
    public Transform player;


    void Start()
    {
        if (hudGroup != null)
            hudGroup.alpha = 1f;

        if (statusText != null)
        {
            Color c = statusText.color;
            c.a = 0f;                   // start invisible, we’ll fade it in
            statusText.color = c;
            statusText.text = "";
        }

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
        // wait 2 seconds before starting
        yield return new WaitForSeconds(2f);

        // Message 1
        yield return StartCoroutine(ShowLine("Infection Concentration CRITICAL."));

        // Message 2
        yield return StartCoroutine(ShowLine("Activate the Power of Friendship."));

        // Gun power-up: big + flash + sound
        AudioManager.instance?.WeaponOnline();
        if (gunPowerup != null)
            yield return StartCoroutine(gunPowerup.PlayPowerup());


        // final msg no fade out
        if (statusText != null) statusText.text = "PRESS SPACE TO SHOOT.";

        if (spaceKeyHint != null) spaceKeyHint.SetActive(true);


        // Wait until the player has pressed Space requiredShots times
        yield return StartCoroutine(WaitForShots(requiredShots));


        SceneManager.LoadScene(nextSceneName);
    }


    // Fade a line in, hold, then fade out
    IEnumerator ShowLine(string line)
    {
        if (statusText == null)
            yield break;

        statusText.text = line;
        PlayBeep();

        // Fade IN
        float t = 0f;
        while (t < textFadeTime)
        {
            t += Time.deltaTime;
            float a = Mathf.Clamp01(t / textFadeTime);
            SetTextAlpha(a);
            yield return null;
        }

        // Hold
        yield return new WaitForSeconds(textHoldTime);

        // Fade OUT
        t = 0f;
        while (t < textFadeTime)
        {
            t += Time.deltaTime;
            float a = Mathf.Clamp01(1f - (t / textFadeTime));
            SetTextAlpha(a);
            yield return null;
        }

        SetTextAlpha(0f);
    }

    void SetTextAlpha(float alpha)
    {
        if (statusText == null) return;
        Color c = statusText.color;
        c.a = alpha;
        statusText.color = c;
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

    void PlayBeep()
    {
        // beep for HUD flicker and text lines
        AudioManager.instance?.ButtonClick();
    }

}
