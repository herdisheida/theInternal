using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

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
    }

    // void Update()
    // {
    //     CharacterName.text = analizedPatient.patientName;
    //     if (analizedPatient.status == PatientStatus.Saved)
    //     {
    //         TemplateText.text = GetSavedLinesFor(analizedPatient);
    //     }
    //     else
    //     {
    //         TemplateText.text = 
    //     }
    // }
}
