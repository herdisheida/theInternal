using UnityEngine;
using UnityEngine.UI;

public class PatientImageSwitch : MonoBehaviour
{
    public Image portraitImage;
    public PatientData patient;

    public void NotSavedImage()
    {
        portraitImage.sprite = patient.analysisInfected;
    }
    public void SavedImage()
    {
        portraitImage.sprite = patient.analysisSafe;
    }
}
