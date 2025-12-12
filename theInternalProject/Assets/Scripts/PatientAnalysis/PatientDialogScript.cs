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
    [TextArea] public string[] ZombieInfectedCharcterLines;
    [TextArea] public string[] WerewolfInfectedCharcterLines;
    [TextArea] public string[] VampireInfectedCharacterLines;

    [Header("Charcter names for patients and doctor saved")]
    [TextArea] public string[] ZombieSavedcharacterLines;
    [TextArea] public string[] WerewolfSavedCharacterLines;
    [TextArea] public string[] VampireSavedCharacterLines;

    [Header("Timings")]
    public float fadeDuration = 0.2f;
    public float holdDuration = 3f;
    public bool loopLines = false; 
    public PatientData analizedPatient;
    public bool isSaved; 

    void Start() {
        if (GameManager.instance == null)
        {
            Debug.LogError("No GameManager in scene!");
            return;
        }
        analizedPatient = GameManager.instance.currentPatient;

        if (analizedPatient == null)
        {
            Debug.LogError("GameManager.currentPatient is null");
            return;
        }
        CharacterName.text = analizedPatient.patientName;
        string[] linesToUse = null;
        string[] characterName = null;

        if (analizedPatient.status == PatientStatus.Saved)
        {
            switch (analizedPatient.patientType) 
            {
                case PatientType.Zombie:
                    linesToUse = ZombieSavedLines;
                    characterName = ZombieInfectedCharcterLines;
                    break;
                case PatientType.Werewolf:
                    linesToUse = WerewolfSavedLines;
                    characterName = WerewolfInfectedCharcterLines;
                    break;
                case PatientType.Vampire:
                    linesToUse = VampireSavedLines;
                    characterName = VampireInfectedCharacterLines;
                    break;
            }
        }
        else
            {
                switch (analizedPatient.patientType) 
            {
                case PatientType.Zombie:
                    linesToUse = ZombieSavedLines;
                    characterName = ZombieSavedcharacterLines;
                    break;
                case PatientType.Werewolf:
                    linesToUse = WerewolfSavedLines;
                    characterName = WerewolfSavedCharacterLines;
                    break;
                case PatientType.Vampire:
                    linesToUse = VampireSavedLines;
                    characterName = VampireSavedCharacterLines;
                    break;
            }
        }
            TemplateText.text = (linesToUse != null && linesToUse.Length > 0) 
            ? linesToUse[0]
            : "";
    }
}
