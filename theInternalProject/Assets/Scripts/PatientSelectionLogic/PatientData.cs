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

    [Header("Dead Sprite position and size")]
    public Vector2 deadSpriteOffset;
    public Vector2 deadSpriteScale = Vector2.one;

    [Header("Selected Sprite position and size")]
    public Vector2 selectedSpriteOffset;
    public Vector2 selectedSpriteScale = Vector2.one;

    [Header("Unselected Sprite position and size")]
    public Vector2 unselectedSpriteOffset;
    public Vector2 unselectedSpriteScale = Vector2.one;
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