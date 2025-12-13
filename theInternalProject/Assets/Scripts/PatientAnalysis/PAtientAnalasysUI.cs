using UnityEngine;


public class PatientAnalasysUI : MonoBehaviour
{
    public PatientImageSwitch werewolfSwitch;
    public PatientImageSwitch vampireSwitch;
    public PatientImageSwitch zombieSwitch;

    public GameObject WerewolfContainer;
    public GameObject VampireContainer;
    public GameObject ZombieContainer;

    PatientImageSwitch activeSwitch;
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
        bool isSaved = patient.status == PatientStatus.Saved;

        WerewolfContainer.SetActive(false);
        VampireContainer.SetActive(false);
        ZombieContainer.SetActive(false);
        activeSwitch = null;
        
        if (patientName == "Zombie")
        {
            ZombieContainer.SetActive(true);
            activeSwitch = zombieSwitch;
        }
        else if (patientName == "Werewolf") {
            WerewolfContainer.SetActive(true);
            activeSwitch = werewolfSwitch;
        }
        else
        {
            VampireContainer.SetActive(true);
            activeSwitch = vampireSwitch;
        }

        activeSwitch.patient = patient;

        if (isSaved)
        {
            activeSwitch.SavedImage();
        }
        else
        {
            activeSwitch.NotSavedImage();
        }
    }
}
