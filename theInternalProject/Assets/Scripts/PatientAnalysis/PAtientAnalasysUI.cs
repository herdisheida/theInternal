using UnityEngine;
using UnityEngine.UI;

public class PatientAnalasysUI : MonoBehaviour
{
    public Image patientImage;
    public Image backgroundImage;

    public void Start()
    {
        PatientData patient = PatientManager.Instance.selectedPatient;
        bool isSaved = patient.isSaved || patient.status == PatientStatus.Saved;

        if (isSaved)
        {
            if (patient.analysisSafe != null) 
                patientImage.sprite = patient.analysisSafe;
        }
        else
        {
            if (patient.analysisInfected != null)
                patientImage.sprite = patient.analysisInfected;
        }

        if (backgroundImage != null && patient.Background != null)
        {
            backgroundImage.sprite = patient.Background;
        }
    }
}
