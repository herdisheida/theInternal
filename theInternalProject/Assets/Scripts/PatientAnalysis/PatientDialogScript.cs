using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;
using System.Reflection;

public class PatientDialogScript : MonoBehaviour
{
    [Header("UI References")]
    public Image TextBox;
    public TextMeshProUGUI CharacterName;
    public TextMeshProUGUI TemplateText;

    [Header("Text lines for each patient Infected")]
    [TextArea] public string[] ZombieInfectedLines;
    [TextArea] public string[] WerewolfInfectedLines;
    [TextArea] public string[] VampireInfectedLines;

    [Header("Text lines for each patient Saved")]
    [TextArea] public string[] ZombieSavedLines;
    [TextArea] public string[] WerewolfSavedLines;
    [TextArea] public string[] VampireSavedLines;

    [Header("Charcter names for patients and doctor infected")]
    [TextArea] public string[] ZombieInfectedCharacterLines;
    [TextArea] public string[] WerewolfInfectedCharacterLines;
    [TextArea] public string[] VampireInfectedCharacterLines;

    [Header("Charcter names for patients and doctor saved")]
    [TextArea] public string[] ZombieSavedCharcterLines;
    [TextArea] public string[] WerewolfSavedCharcterLines;
    [TextArea] public string[] VampireSavedCharacterLines;

    [Header("Timings")]
    public float fadeDuration = 0.2f;
    public float holdDuration = 3f;
    public bool loopLines = false; 
    public PatientData analizedPatient;
    public bool isSaved; 
    public int dialogNumber;
    public bool started;

    void Start() {
        if (GameManager.instance == null)
        {
            Debug.LogError("No GameManager in scene!");
            return;
        }
        GameManager.instance.dialogOver = false; // Initialize per call to AnalysisScreen
        started = false;
        dialogNumber = 0;

        if (GameManager.instance.currentPatient.status == PatientStatus.Saved)
        {
            AudioManager.instance?.PlayPatientSavedMusic();
        }
    }

    void Update()
    {
        analizedPatient = GameManager.instance.currentPatient;

        if (analizedPatient == null)
        {
            Debug.LogError("GameManager.currentPatient is null");
            return;
        }
        string[] linesToUse = null; // Kepps the current lines and names when in the Analysis Screen
        string[] characterName = null;

        if (analizedPatient.status == PatientStatus.Saved) // makes sure to add the correct lines and names for each instance
        {
                        

            switch (analizedPatient.patientType) 
            {
                case PatientType.Zombie:
                    linesToUse = ZombieSavedLines;
                    characterName = ZombieSavedCharcterLines;
                    break;
                case PatientType.Werewolf:
                    linesToUse = WerewolfSavedLines;
                    characterName = WerewolfSavedCharcterLines;
                    break;
                case PatientType.Vampire:
                    linesToUse = VampireSavedLines;
                    characterName = VampireSavedCharacterLines;
                    break;
            }
        }
        else
        {
                // Infected lines (before entering patient)

            switch (analizedPatient.patientType) 
            {
                case PatientType.Zombie:
                    linesToUse = ZombieInfectedLines;
                    characterName = ZombieInfectedCharacterLines;
                    break;
                case PatientType.Werewolf:
                    linesToUse = WerewolfInfectedLines;
                    characterName = WerewolfInfectedCharacterLines;
                    break;
                case PatientType.Vampire:
                    linesToUse = VampireInfectedLines;
                    characterName = VampireInfectedCharacterLines;
                    break;
            }
        }
        if (!started) // Initialize so that the dialog starts when entering the patient
        {
            started = true;
            dialogNumber = 0;
            TemplateText.text = linesToUse[0];
            CharacterName.text = characterName[0];
            dialogNumber = 1;
        }
        if (Input.GetKeyDown(KeyCode.Space) && dialogNumber < linesToUse.Length && linesToUse != null) { 
            TemplateText.text = (linesToUse != null && linesToUse.Length > 0) // All lines 
            ? linesToUse[dialogNumber] 
            : "";
            CharacterName.text = (characterName != null && characterName.Length > 0) // All character names desplayed
            ? characterName[dialogNumber]
            : "";
            dialogNumber++;
            if (dialogNumber >= linesToUse.Length)
            {
                StartCoroutine(SetDialogOverNextFrame()); 
            }
        }
    }
    
    IEnumerator SetDialogOverNextFrame() // Makes sure to start game after next button press
    {
        yield return null;
        GameManager.instance.dialogOver = true;

        if (GameManager.instance.dialogOver = true) AudioManager.instance?.StopMusic();
    }
}
