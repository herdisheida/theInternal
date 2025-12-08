using UnityEngine;
using UnityEngine.UI;

public class PatientStatusUI : MonoBehaviour
{
    [Header("References")]
    public PatientData patientData;
    public Image displayImage;


    void Start()
    {
        RefreshVisual();
    }

    void Update()
    {
        
    }

    public void RefreshVisual()
    {
        if (patientData == null || displayImage == null)
            return;

        switch (patientData.status)
        {
            case PatientStatus.None:
                displayImage.sprite = patientData.unselected;
                break;

            case PatientStatus.Infected:
                displayImage.sprite = patientData.infected;
                break;

            case PatientStatus.Dead:
                displayImage.sprite = patientData.dead;
                break;

            case PatientStatus.Saved:
                displayImage.sprite = patientData.saved;
                break;
        }
    }
}
