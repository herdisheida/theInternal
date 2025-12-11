using UnityEngine;
using UnityEngine.UI;

public class PatientStatusUI : MonoBehaviour
{
    [Header("References")]
    public PatientData patientData;
    public Image patientImage;

    private PatientStatus lastStatus;

    [Header("Background References")]
    public Image backgroundImagePlaceholder;
    public Sprite zombieBg;
    public Sprite werewolfBg;
    public Sprite vampireBg;


    void Start()
    {
        // get current patient data
        if (patientData == null && GameManager.instance != null) { patientData = GameManager.instance.currentPatient; }

        if (patientData == null || patientImage == null)
        {
            Debug.LogWarning($"{name}: PatientStatusUI missing references.");
            enabled = false;
            return;
        }

        lastStatus = patientData.status;
        RefreshVisual();
    }

    void Update()
    {
        if (patientData == null && GameManager.instance != null) { patientData = GameManager.instance.currentPatient; }

        // only update when status changes
        if (patientData.status != lastStatus)
        {
            lastStatus = patientData.status;
            RefreshVisual();
        }
    }


    public void RefreshVisual()
    {
        if (patientData == null || patientImage == null)
            return;

        // update patient
        patientImage.sprite = patientData.infected;

        // update background
        switch (patientData.patientType)
        {
            case PatientType.Zombie:
                backgroundImagePlaceholder.sprite = zombieBg;
                break;

            case PatientType.Werewolf:
                backgroundImagePlaceholder.sprite = werewolfBg;
                break;

            case PatientType.Vampire:
                backgroundImagePlaceholder.sprite = vampireBg;
                break;
        }
    }
}
