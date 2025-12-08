using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PatientAnalysisScreen : MonoBehaviour
{
    [Header("UI References")]
    public Image infectedImage;   // whole image for “infected” state
    public Image savedImage;      // whole image for “saved” state

    [Header("Sprites (optional overrides)")]
    public Sprite infectedSprite;
    public Sprite savedSprite;

    [Header("Flow")]
    public float delayToCredits = 5f;

    void Start()
    {
        // Safety checks
        if (infectedImage == null || savedImage == null)
        {
            Debug.LogWarning("PatientAnalysisScreen: Images not assigned in Inspector.");
            return;
        }

        bool isSaved = false;

        // Prefer the new GameManager system
        if (GameManager.instance != null && GameManager.instance.currentPatient != null)
        {
            var p = GameManager.instance.currentPatient;
            isSaved = (p.status == PatientStatus.Saved);

            Debug.Log($"PatientAnalysisScreen: showing analysis for {p.patientName}, status = {p.status}");
        }
        else
        {
            Debug.LogWarning("PatientAnalysisScreen: No current patient in GameManager - defaulting to unselected.");
        }

        // Apply optional sprite overrides
        if (infectedSprite != null)
            infectedImage.sprite = infectedSprite;
        if (savedSprite != null)
            savedImage.sprite = savedSprite;

        // Toggle which image is visible
        if (isSaved)
        {
            savedImage.gameObject.SetActive(true);
            infectedImage.gameObject.SetActive(false);
        }
        else
        {
            infectedImage.gameObject.SetActive(true);
            savedImage.gameObject.SetActive(false);
        }

        // Start timer to credits
        StartCoroutine(GoToCreditsAfterDelay());
    }

    IEnumerator GoToCreditsAfterDelay()
    {
        yield return new WaitForSeconds(delayToCredits);
        SceneManager.LoadScene("Credits");
    }
}
