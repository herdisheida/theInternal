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
    public bool isSaved;

    public PatientStatus status;

}

public enum PatientStatus
{
    None,       // not visited yet
    Infected,   // player died before before killing boss
    Dead,       // player shot them (after shooting patient)
    Saved       // player beat boss
}