using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "NewPatientData", menuName = "Patients/Patient Data")]
public class PatientData : ScriptableObject
{
    public string patientName;
    public PatientType patientType;


    public Sprite selected;
    public Sprite unselected;
    public Sprite infected;
    public Sprite dead;
    public Sprite analysisInfected;
    public Sprite analysisSafe;
    public PatientStatus status;
    public string bossSceneName;
    public Sprite infectionSprite;
    public RuntimeAnimatorController infectionAnimator;
    public string infectionAnimationStringName;


}

public enum PatientStatus
{
    None,       // not visited yet
    Infected,   // player died before before killing boss
    Dead,       // player shot them (after shooting patient)
    Saved       // player beat boss
}

public enum PatientType
{
    Zombie,
    Werewolf,
    Vampire
}