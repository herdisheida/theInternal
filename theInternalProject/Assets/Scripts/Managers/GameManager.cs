using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Current Patient")]
    public PatientData currentPatient;   // who we’re working on right now

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    // Called from the PatientSelection scene when the player picks someone
    public void SetCurrentPatient(PatientData patient)
    {
        currentPatient = patient;
        Debug.Log($"GameManager: current patient set to {patient.patientName}");
    }





    // --------- STATE CHANGERS ---------

    // --------- Player died anywhere before finishing ---------
    public void MarkPatientInfected()
    {
        if (currentPatient == null) return;

        currentPatient.status = PatientStatus.Infected;
        currentPatient.isSaved = false;
        Debug.Log($"Patient {currentPatient.patientName} marked INFECTED.");
    }

    // --------- Player shoots patient ---------
    public void MarkPatientDead()
    {
        if (currentPatient == null) return;

        currentPatient.status = PatientStatus.Dead;
        currentPatient.isSaved = false;
        Debug.Log($"Patient {currentPatient.patientName} marked DEAD.");
    }

    // --------- Player defeated the boss ---------
    public void MarkPatientSaved()
    {
        if (currentPatient == null) return;

        currentPatient.status = PatientStatus.Saved;
        currentPatient.isSaved = true;
        Debug.Log($"Patient {currentPatient.patientName} marked SAVED.");
    }
}
