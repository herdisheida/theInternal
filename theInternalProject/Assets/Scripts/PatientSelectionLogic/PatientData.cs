using UnityEngine;

[CreateAssetMenu(fileName = "NewPatientData", menuName = "Patients/Patient Data")]
public class PatientData : ScriptableObject
{
    public string patientName;
    public Sprite alive;
    public Sprite dlackAndWhite;
    public Sprite infected;
    public Sprite dead;
}
