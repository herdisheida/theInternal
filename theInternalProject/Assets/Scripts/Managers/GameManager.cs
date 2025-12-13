using UnityEngine;


public enum EndingType
{
    Bad,
    PartialyGood,
    PartialyBad,
    Good
}


public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("All Patients in this run")]
    public PatientData[] allPatients;   // assign your 3 PatientData assets


    [Header("Current Patient")]
    public PatientData currentPatient;   // who we’re working on right now

    [Header("Patients levels played and or saved")]
    public int patientsSaved = 0;
    public int patientLevelsPlayed = 0;




    void Awake()
    {
        // singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            ResetAllPatients();
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

        if (IsPatientResolved(currentPatient))
        {
            Debug.Log($"Cannot mark INFECTED: {currentPatient.patientName} is already {currentPatient.status}");
            return;
        }


        currentPatient.status = PatientStatus.Infected;
        patientLevelsPlayed++;
        Debug.Log($"Patient {currentPatient.patientName} marked INFECTED.");
    }

    // --------- Player shoots patient ---------
    public void MarkPatientDead()
    {
        if (currentPatient == null) return;

        currentPatient.status = PatientStatus.Dead;
        Debug.Log($"Patient {currentPatient.patientName} marked DEAD.");
    }

    // --------- Player defeated the boss ---------
    // GameManager.instance?.MarkPatientSaved();
    public void MarkPatientSaved()
    {
        if (currentPatient == null) return;

        if (IsPatientResolved(currentPatient))
        {
            Debug.Log($"Cannot mark SAVED: {currentPatient.patientName} is already {currentPatient.status}");
            return;
        }

        currentPatient.status = PatientStatus.Saved;
        patientsSaved++;
        patientLevelsPlayed++;
        Debug.Log($"Patient {currentPatient.patientName} marked SAVED.");
    }



    public void ResetAllPatients()
    {
        if (allPatients == null) return;

        allPatients[0].status = PatientStatus.None;
        allPatients[1].status = PatientStatus.None;
        allPatients[2].status = PatientStatus.None;
        Debug.Log($"All patients have been reset");
    }

    // Helper to check if a patient is resolved (saved or not saved [dead/infected])
        // used to error if boss died right after player or player right after boss
    private bool IsPatientResolved(PatientData p)
    {
        return p.status == PatientStatus.Saved
            || p.status == PatientStatus.Infected;
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
        else if (saved == 2) return EndingType.PartialyGood;
        else return EndingType.PartialyBad;
    }

    public bool AllPatientsResolved()
    {
        int total = GetTotalPatients();
        int resolved = 0;

        if (allPatients == null) return false;

        foreach (var p in allPatients)
        {
            if (p != null && (p.status == PatientStatus.Saved || p.status == PatientStatus.Dead || p.status == PatientStatus.Infected))
                resolved++;
        }

        return resolved == total;
    }
}

