using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using System.Collections;

public class IntroTextSequence : MonoBehaviour
{
    [Header("Text")]
    public TextMeshProUGUI dialogueText;

    [TextArea(3, 5)]
    public string[] lines;

    [Header("Timing")]
    public float timeBetweenLines = 3f;

    [Header("Scene")]
    public string nextSceneName = "PatientSelection";

    private int currentIndex = 0;

    void Start()
    {
        AudioManager.instance?.PlayHospitalLobbyMusic();
        if (dialogueText == null || lines.Length == 0)
        {
            Debug.LogWarning("IntroTextSequence: Missing text or lines.");
            return;
        }

        dialogueText.text = "";
        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        while (currentIndex < lines.Length)
        {
            dialogueText.text = lines[currentIndex];
            currentIndex++;

            yield return new WaitForSeconds(timeBetweenLines);
        }

        // after last line
        SceneManager.LoadScene(nextSceneName);
    }
}
