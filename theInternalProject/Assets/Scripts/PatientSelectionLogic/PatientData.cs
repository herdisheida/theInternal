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
    public Sprite saved;
    public Sprite analysisInfected;
    public Sprite analysisSafe;
    public Sprite Background;

    public bool isSaved;

    public PatientStatus status;
    public Sprite infectionSprite;

}

public enum PatientStatus
{
    None,       // not visited yet
    Infected,   // player died before before killing boss
    Dead,       // player shot them (after shooting patient)
    Saved       // player beat boss
}