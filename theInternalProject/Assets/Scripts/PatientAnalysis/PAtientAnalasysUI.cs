using UnityEngine;


public class PatientAnalasysUI : MonoBehaviour
{
    public PatientImageSwitch werewolfSwitch; // Switch depending on condition
    public PatientImageSwitch vampireSwitch;
    public PatientImageSwitch zombieSwitch;

    public GameObject WerewolfContainer; // Keeps all the data per patient
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

        string patientName = patient.patientName; // Get analyzed patient
        bool isSaved = patient.status == PatientStatus.Saved;

        WerewolfContainer.SetActive(false); // Disable all containers before enabling the accuret/correct ones
        VampireContainer.SetActive(false);
        ZombieContainer.SetActive(false);
        activeSwitch = null;
        
        if (patientName == "Zombie") // Activate the correct UI based on the patient type
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

        if (isSaved) // Checks condition and shows the patients correct portait
        {
            activeSwitch.SavedImage();
        }
        else
        {
            activeSwitch.NotSavedImage();
        }
    }
}
