using UnityEngine;
using UnityEngine.UI;

public class PatientStatusUI : MonoBehaviour
{
    [Header("References")]
    public PatientData patientData;
    public Image displayImage;

    private PatientStatus lastStatus;

    void Start()
    {
        // get current patient data
        if (patientData == null && GameManager.instance != null) { patientData = GameManager.instance.currentPatient; }

        if (patientData == null || displayImage == null)
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
        if (patientData == null || displayImage == null)
            return;

        switch (patientData.status)
        {
            case PatientStatus.Infected:
                displayImage.sprite = patientData.infected;
                break;

            case PatientStatus.Dead:
                displayImage.sprite = patientData.dead;
                break;
        }
    }
}
