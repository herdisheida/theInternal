using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WeaponOnlineController : MonoBehaviour
{
    [Header("UI References")]
    public CanvasGroup hudGroup;       // panel that flickers on
    public TextMeshProUGUI statusText; // main HUD text
    public GameObject spaceKeyHint;    // the space-bar image + text container

    [Header("Gun & Shooting")]
    public GameObject gunRoot;               // parent object for the gun
    public GunController gunController;      // disabled at start, enabled after powerup
    public GunSpriteSwitcher gunSpriteSwitcher;

    [Header("Gun Powerup Pulse")]
    public float gunPulseScale = 1.3f;
    public float gunPulseDuration = 0.5f;

    [Header("Timings")]
    public float hudFlickerDuration = 0.6f;
    public float textHoldTime = 4f;

    [Header("Scene Flow")]
    public int requiredShots = 3; // how many shots before continuing
    public string nextSceneName = "BossBattle";

    [Header("Player Entrance")]
    public Transform player;                // your PlayerCharacter transform
    public Vector3 playerStartOffset = new Vector3(-6f, 0f, 0f); // how far left to start
    public float flyInDuration = 1.2f;
    public float flyInBobAmplitude = 0.3f;  // vertical bob size
    public float flyInBobFrequency = 3f;    // bob speed

    [Header("Fly To Center After Space")]
    public float flyToCenterDuration = 0.8f;

    // internal
    Vector3 playerTargetPos;
    Vector3 playerStartPos;

    void Start()
    {
        

        // HUD hidden
        if (hudGroup != null)
            hudGroup.alpha = 0f;

        if (statusText != null)
            statusText.text = "";

        if (spaceKeyHint != null)
            spaceKeyHint.SetActive(false);

        // Gun hidden / disabled until powerup
        if (gunRoot != null) gunRoot.SetActive(false);
        if (gunController != null) gunController.enabled = false;
        if (gunSpriteSwitcher != null)gunSpriteSwitcher.enabled = false;

        // Prepare player entrance
        if (player != null)
        {
            playerTargetPos = player.position; // where he should end up
            playerStartPos = playerTargetPos + playerStartOffset; // off-screen left
            player.position = playerStartPos;
        }

        // start cutscene
        StartCoroutine(CutsceneRoutine());
    }

    IEnumerator CutsceneRoutine()
    {
        // wait a bit
        yield return new WaitForSeconds(1.5f);

        // Player flies in with bobbing
        if (player != null) yield return StartCoroutine(PlayerFlyIn());

        // HUD flicker on
        yield return StartCoroutine(HUDFlicker());

        // Message 1
        yield return StartCoroutine(ShowLine("Infection Concentration CRITICAL."));

        // Message 2
        yield return StartCoroutine(ShowLine("Activate the Power of Friendship."));

        // Gun power-up pulse + enable shooting
        yield return StartCoroutine(GunPowerup());

        // Clear the last line text
        if (statusText != null) statusText.text = "";

        // Player flies to screen center
        if (player != null) yield return StartCoroutine(PlayerFlyToCenter());

        // final prompt to press Space
        if (spaceKeyHint != null) spaceKeyHint.SetActive(true);

        // Wait for N shots (space presses)
        yield return StartCoroutine(WaitForShots(requiredShots));

        // hide HUD while flying to center
        if (spaceKeyHint != null) spaceKeyHint.SetActive(false);

        SceneManager.LoadScene(nextSceneName);
    }

    // ------------ PLAYER ANIMATIONS ------------

    IEnumerator PlayerFlyIn()
    {
        float t = 0f;

        while (t < flyInDuration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / flyInDuration);

            // horizontal lerp
            Vector3 pos = Vector3.Lerp(playerStartPos, playerTargetPos, lerp);

            // vertical bob
            float bob = Mathf.Sin(lerp * Mathf.PI * flyInBobFrequency) * flyInBobAmplitude;
            pos.y = playerTargetPos.y + bob;

            player.position = pos;
            yield return null;
        }

        player.position = playerTargetPos;
    }

    IEnumerator PlayerFlyToCenter()
    {
        Vector3 start = player.position;
        Vector3 target = new Vector3(0f, start.y, start.z); // center on x

        float t = 0f;
        while (t < flyToCenterDuration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / flyToCenterDuration);

            player.position = Vector3.Lerp(start, target, lerp);
            yield return null;
        }

        player.position = target;
    }

    // ------------ HUD & TEXT ------------

    IEnumerator HUDFlicker()
    {
        if (hudGroup == null)
            yield break;

        float elapsed = 0f;
        bool on = false;

        while (elapsed < hudFlickerDuration)
        {
            on = !on;
            hudGroup.alpha = on ? 1f : 0f;

            float step = Random.Range(0.05f, 0.15f);
            elapsed += step;
            yield return new WaitForSeconds(step);
        }

        hudGroup.alpha = 1f; // stay on
    }

    IEnumerator ShowLine(string line)
    {
        if (statusText != null)
            statusText.text = line;

        PlayBeep();
        yield return new WaitForSeconds(textHoldTime);
    }

    // ------------ GUN POWERUP ------------

    IEnumerator GunPowerup()
    {
        // show the gun object
        if (gunRoot != null)
            gunRoot.SetActive(true);

        PlayPowerupSound();

        // pulse scale up and down
        if (gunRoot != null)
        {
            Vector3 originalScale = gunRoot.transform.localScale;
            float t = 0f;

            while (t < gunPulseDuration)
            {
                t += Time.deltaTime;
                float lerp = Mathf.Sin((t / gunPulseDuration) * Mathf.PI); // 0 -> 1 -> 0
                float scaleFactor = Mathf.Lerp(1f, gunPulseScale, lerp);

                gunRoot.transform.localScale = originalScale * scaleFactor;
                yield return null;
            }

            gunRoot.transform.localScale = originalScale;
        }

        // after the pulse, shooting becomes available
        if (gunController != null) gunController.enabled = true;
        if (gunSpriteSwitcher != null) gunSpriteSwitcher.enabled = true;
    }

    // ------------ AUDIO ------------

    void PlayBeep()
    {
        AudioManager.instance?.ButtonClick();
    }

    void PlayPowerupSound()
    {
        // mario powerup sound
        AudioManager.instance?.WeaponOnline();
    }

    // ------------ INPUT WAITING ------------
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

}
