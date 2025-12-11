using UnityEngine;
using UnityEngine.UI;

public class PatientStatusUI : MonoBehaviour
{
    [Header("References")]
    public PatientData patientData;
    public Image patientImage;

    [Header("Background References")]
    public Image backgroundImagePlaceholder;
    public Sprite zombieBg;
    public Sprite werewolfBg;
    public Sprite vampireBg;




    private PatientStatus lastStatus;

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

        switch (patientData.status)
        {
            case PatientStatus.Infected:
                patientImage.sprite = patientData.infected;
                break;

            case PatientStatus.Dead:
                patientImage.sprite = patientData.dead;
                break;
        }

        if (backgroundImagePlaceholder != null)
        {
            switch (patientData.patientName)
            {
                case PatientStatus.Zombie:
                    backgroundImagePlaceholder.sprite = zombieBg;
                    break;

                case PatientStatus.Werewolf:
                    backgroundImagePlaceholder.sprite = werewolfBg;
                    break;

                case PatientStatus.Vampire:
                    backgroundImagePlaceholder.sprite = vampireBg;
                    break;
            }
        }
    }
}
