using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PatientAnalysisScreen : MonoBehaviour
{
    public PatientData patientData;

    public Image SavedImage;
    public Image InfectedImage;

    private void UpdateAnalysisUI()
    {
        if (patientData == null)
        {
            Debug.Log("Patient Data not assigned");
            return;
        }
        bool isSaved = patientData.isSaved || patientData.status == PatientStatus.Saved;

        if (isSaved)
        {
            SavedImage.gameObject.SetActive(true);
            InfectedImage.gameObject.SetActive(false);
            if (patientData.analysisSafe != null)
                {
                SavedImage.sprite = patientData.analysisSafe; 
                }
        }
        else
        {
            InfectedImage.gameObject.SetActive(true);
            SavedImage.gameObject.SetActive(false);
            if (patientData.analysisInfected != null)
                {
                InfectedImage.sprite = patientData.analysisInfected; 
                }
        }
    }

    IEnumerator GoToCreditsAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("Credits");
    }
}
