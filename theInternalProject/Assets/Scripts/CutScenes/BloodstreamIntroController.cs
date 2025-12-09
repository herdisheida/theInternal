using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BloodstreamIntroController : MonoBehaviour
{
    [Header("Doctor & Pod")]
    public Transform doctor;              // doctor sprite
    public Transform pod;                 // pod sprite
    public SpriteRenderer podSpriteRenderer;
    public Sprite podOpenSprite;
    public Sprite podClosedSprite;

    [Header("Controls Hint UI")]
    public GameObject controlsKeyHint;

    [Header("Positions")]
    public Transform doctorStartPoint;    // off-screen left
    public Transform doctorGroundPoint;   // in front of pod
    public Transform doctorOnPodPoint;    // on top of/opening
    public Transform podFlyOffTarget;     // off-screen right target

    [Header("Doctor Hop Settings")]
    public float smallHopDuration = 0.3f;
    public float smallHopHeight = 0.3f;
    public int smallHopCount = 8;

    public float bigHopDuration = 0.9f;
    public float bigHopHeight = 1.5f;
    public float bigHopRotation = -360f;   // full spin while jumping

    [Header("Pod Shrink")]
    public float shrinkDuration = 1.2f;
    public float shrinkScale = 0.4f;

    [Header("Pod Fly Off")]
    public float podFlyDuration = 1.5f;

    [Header("Blood Cell Spawning")]
    public GameObject bloodCellPrefab;     // prefab with SpriteRenderer + BloodCellMover
    public Sprite[] bloodCellSprites;      // different cell sprites

    public float spawnX = 10f;             // just off the right side
    public float spawnYMin = -3.9f;
    public float spawnYMax = 3.9f;

    public float spawnIntervalMin = 0.1f;
    public float spawnIntervalMax = 0.18f;
    public float spawnDuration = 8f; // how long to keep spawning in this cutscene

    [Header("Background Transition")]
    public Image startBackground;            // the first, non-scrolling BG (room / neutral)
    public RawImage bloodBackground;         // the fleshy scrolling BG (RawImage)
    public BackgroundScroller bloodScroller; // the script on that BG
    public float backgroundFadeDuration = 1.5f;

    [Header("Scene Flow")]
    public string nextSceneName = "ObstacleGameplay"; // bloodstream level

    void Start()
    {
        if (controlsKeyHint != null)
            controlsKeyHint.SetActive(false);

        // safety
        if (doctor == null || pod == null || doctorStartPoint == null ||
            doctorGroundPoint == null || doctorOnPodPoint == null || podFlyOffTarget == null)
        {
            Debug.LogWarning("BloodstreamIntroController: Missing references!");
            return;
        }

        // Place doctor at start, set pod open
        doctor.position = doctorStartPoint.position;

        if (podSpriteRenderer != null && podOpenSprite != null)
            podSpriteRenderer.sprite = podOpenSprite;

        // Start the cutscene
        StartCoroutine(CutsceneRoutine());
    }

    IEnumerator CutsceneRoutine()
    {
        // Small pause before doctor appears
        yield return new WaitForSeconds(0.5f);

        // small hops toward the pod
        yield return StartCoroutine(DoctorSmallHops());
        yield return new WaitForSeconds(1f);

        // big hop & rotation into the pod opening
        yield return StartCoroutine(DoctorBigHopIntoPod());
        yield return new WaitForSeconds(0.4f);

        // hide doctor (he's inside pod)
        doctor.gameObject.SetActive(false);

        // close the pod
        if (podSpriteRenderer != null && podClosedSprite != null) podSpriteRenderer.sprite = podClosedSprite;
        AudioManager.instance?.CloseDoor();
        yield return new WaitForSeconds(0.8f);

        // shrink the pod
        yield return StartCoroutine(ShrinkPodRoutine());

        // show controls hint (arrow/WASD keys)
        if (controlsKeyHint != null) controlsKeyHint.SetActive(true);

        // fade into the bloodstream background
        yield return StartCoroutine(FadeToBloodBackground());

        // start spawning blood cells
        StartCoroutine(SpawnBloodCellsRoutine());
        yield return new WaitForSeconds(6f);

        // pod flies off into the bloodstream
        yield return StartCoroutine(PodFlyOffRoutine());

        // 8) load the bloodstream gameplay scene
        SceneManager.LoadScene(nextSceneName);
    }

    // -------- Doctor animations --------

    IEnumerator DoctorSmallHops()
    {
        // We'll hop from start -> ground point with a few small hops.
        Vector3 start = doctorStartPoint.position;
        Vector3 end = doctorGroundPoint.position;

        for (int i = 0; i < smallHopCount; i++)
        {
            Vector3 hopStart = Vector3.Lerp(start, end, (float)i / smallHopCount);
            Vector3 hopEnd   = Vector3.Lerp(start, end, (float)(i + 1) / smallHopCount);

            yield return StartCoroutine(HopArc(doctor, hopStart, hopEnd, smallHopDuration, smallHopHeight, rotate: false, rotationAmount: 0f));
        }

        // snap to ground point just in case
        doctor.position = doctorGroundPoint.position;
    }

    IEnumerator DoctorBigHopIntoPod()
    {
        Vector3 start = doctorGroundPoint.position;
        Vector3 end   = doctorOnPodPoint.position;

        // big hop with rotation
        yield return StartCoroutine(HopArc(doctor, start, end, bigHopDuration, bigHopHeight, rotate: true, rotationAmount: bigHopRotation));

        doctor.position = doctorOnPodPoint.position;
    }

    /// <summary>
    /// Generic parabolic hop between two points, optional rotation.
    /// </summary>
    IEnumerator HopArc(Transform target, Vector3 start, Vector3 end,
                       float duration, float height, bool rotate, float rotationAmount)
    {
        float t = 0f;
        float startRotZ = target.eulerAngles.z;

        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / duration);

            // horizontal lerp
            Vector3 pos = Vector3.Lerp(start, end, lerp);

            // parabolic vertical hop (sine gives nice up/down arc)
            float yOffset = Mathf.Sin(lerp * Mathf.PI) * height;
            pos.y += yOffset;

            target.position = pos;

            if (rotate)
            {
                float rotZ = startRotZ + rotationAmount * lerp;
                target.rotation = Quaternion.Euler(0f, 0f, rotZ);
            }

            yield return null;
        }

        // final position
        target.position = end;
        if (rotate)
            target.rotation = Quaternion.Euler(0f, 0f, startRotZ + rotationAmount);
    }

    // -------- Pod shrink & fly --------

    IEnumerator ShrinkPodRoutine()
    {
        Vector3 originalScale = pod.localScale;
        Vector3 targetScale = originalScale * shrinkScale;

        // original and target positions (keep X/Z, only change Y)
        Vector3 originalPos = pod.position;
        Vector3 targetPos = new Vector3(originalPos.x, 1f, originalPos.z);

        float t = 0f;

        AudioManager.instance?.ShrinkPod();

        while (t < shrinkDuration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / shrinkDuration);

            // shrink
            pod.localScale = Vector3.Lerp(originalScale, targetScale, lerp);

            // move to center Y
            pod.position = Vector3.Lerp(originalPos, targetPos, lerp);

            yield return null;
        }

        // final snap
        pod.localScale = targetScale;
        pod.position = targetPos;
    }


    IEnumerator PodFlyOffRoutine()
    {
        Vector3 start = pod.position;
        Vector3 end = podFlyOffTarget.position;

        float t = 0f;

        while (t < podFlyDuration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / podFlyDuration);
            pod.position = Vector3.Lerp(start, end, lerp);
            yield return null;
        }

        pod.position = end;
    }

    // -------- Blood cell spawning --------

    IEnumerator SpawnBloodCellsRoutine()
    {
        float elapsed = 0f;

        while (elapsed < spawnDuration)
        {
            SpawnOneBloodCell();

            float wait = Random.Range(spawnIntervalMin, spawnIntervalMax);
            elapsed += wait;
            yield return new WaitForSeconds(wait);
        }
    }

    void SpawnOneBloodCell()
    {
        if (bloodCellPrefab == null) return;

        float y = Random.Range(spawnYMin, spawnYMax);
        Vector3 pos = new Vector3(spawnX, y, 0f);

        GameObject cell = Instantiate(bloodCellPrefab, pos, Quaternion.identity);

        // pick a random sprite for this cell
        if (bloodCellSprites != null && bloodCellSprites.Length > 0)
        {
            var sr = cell.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                int index = Random.Range(0, bloodCellSprites.Length);
                sr.sprite = bloodCellSprites[index];
            }
        }
    }


    // -------- Background fade --------
    IEnumerator FadeToBloodBackground()
    {
        if (bloodBackground == null)
            yield break;

        float t = 0f;

        // grab starting colors
        Color startBgColor = Color.white;
        if (startBackground != null)
            startBgColor = startBackground.color;

        Color bloodBgColor = bloodBackground.color;
        float startOldA = startBackground != null ? startBgColor.a : 1f;
        float startNewA = bloodBgColor.a; // should be 0

        while (t < backgroundFadeDuration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / backgroundFadeDuration);

            // fade OUT old BG
            if (startBackground != null)
            {
                startBgColor.a = Mathf.Lerp(startOldA, 0f, lerp);
                startBackground.color = startBgColor;
            }

            // fade IN bloodstream BG
            bloodBgColor.a = Mathf.Lerp(startNewA, 1f, lerp);
            bloodBackground.color = bloodBgColor;

            yield return null;
        }

        // final values
        if (startBackground != null)
        {
            startBgColor.a = 0f;
            startBackground.color = startBgColor;
            startBackground.gameObject.SetActive(false);     // optional: hide it completely
        }

        bloodBgColor.a = 1f;
        bloodBackground.color = bloodBgColor;

        // now start scrolling the bloodstream
        if (bloodScroller != null)
            bloodScroller.enabled = true;
    }
}
