using UnityEngine;


public enum EndingType
{
    Bad,
    Partial,
    Good
}


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("All Patients in this run")]
    public PatientData[] allPatients;   // assign your 3 PatientData assets


    [Header("Current Patient")]
    public PatientData currentPatient;   // who we’re working on right now



    void Awake()
    {
        // singleton pattern
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
    // FX: GameManager.instance?.SetCurrentPatient(patient);
    public void SetCurrentPatient(PatientData patient)
    {
        currentPatient = patient;
        Debug.Log($"GameManager: current patient set to {patient.patientName}");
    }





    // ------------------ STATE CHANGERS ------------------




    // --------- Player dies in boss or obstacle fight ---------
    // GameManager.instance?.MarkPatientInfected();
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
    // GameManager.instance?.MarkPatientSaved();
    public void MarkPatientSaved()
    {
        if (currentPatient == null) return;

        currentPatient.status = PatientStatus.Saved;
        currentPatient.isSaved = true;
        Debug.Log($"Patient {currentPatient.patientName} marked SAVED.");
    }






    // ------------------ PATIENT SUMMARY ------------------

    public int GetSavedCount()
    {
        int count = 0;
        if (allPatients == null) return 0;

        foreach (var p in allPatients)
        {
            if (p != null && p.status == PatientStatus.Saved)
                count++;
        }
        return count;
    }

    public int GetTotalPatients()
    {
        return (allPatients != null) ? allPatients.Length : 0;
    }

    public EndingType GetEndingType()
    {
        int total = GetTotalPatients();
        int saved = GetSavedCount();

        if (total <= 0) return EndingType.Bad; // no patients, default to Bad

        if (saved == 0) return EndingType.Bad;
        else if (saved == total) return EndingType.Good;
        else return EndingType.Partial;
    }
}

