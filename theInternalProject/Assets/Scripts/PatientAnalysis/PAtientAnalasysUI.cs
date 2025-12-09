using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;


public class PatientAnalasysUI : MonoBehaviour
{
    public Image patientImage;
    public Image backgroundImage;

    public GameObject WerewolfContainer;
    public GameObject VampireContainer;
    public GameObject ZombieContainer;

    public void Start()
    {
        if (GameManager.instance == null)
        {
            Debug.LogError("instance is null!");
        }
        PatientData patient = GameManager.instance?.currentPatient;
        if (patient == null)
        {
            Debug.LogError("currentPatient is null!");
        }

        string patientName = patient.patientName;
        bool isSaved = patient.isSaved || patient.status == PatientStatus.Saved;

        WerewolfContainer.SetActive(false);
        VampireContainer.SetActive(false);
        ZombieContainer.SetActive(false);
        
        if (patientName == "Zombie")
        {
            ZombieContainer.SetActive(true);
        }
        else if (patientName == "Werewolf") {
            WerewolfContainer.SetActive(true);
        }
        else
        {
            VampireContainer.SetActive(true);
        }

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
    IEnumerator GoToCreditsAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("Credits");
    }
}
