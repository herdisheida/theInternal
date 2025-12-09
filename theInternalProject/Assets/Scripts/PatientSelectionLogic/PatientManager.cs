using UnityEngine;

public class PatientManager : MonoBehaviour
{
    public static PatientManager Instance {get; private set; }
    public PatientData[] patients;
    public PatientData selectedPatient;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    
}
