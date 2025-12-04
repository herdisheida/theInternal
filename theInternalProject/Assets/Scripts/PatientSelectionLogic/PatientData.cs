using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPatientData", menuName = "Patients/Patient Data")]
public class PatientData : ScriptableObject
{
    public string patientName;
    public Sprite selected;
    public Sprite unselected;
    public Sprite infected;
    public Sprite dead;

}
