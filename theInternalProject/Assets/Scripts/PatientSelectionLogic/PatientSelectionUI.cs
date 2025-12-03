using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Mono.Cecil.Cil;

public class PatientSelectionUI : MonoBehaviour
{
    [Header("Data")]
    public PatientData[] patients;

    [Header("UI References")]
    public Image portraitImage;
    
    private int currentIndex = 0;

    void Start()
    {
        ShowPatient(currentIndex);
    }

    public void NextPatient()
    {
        currentIndex = (currentIndex - 1 + patients.Length) % patients.Length;
        ShowPatient(currentIndex);
    }

    void ShowPatient(int index)
    {
        var data = patients[index];

        portraitImage.sprite = data.alive;
    }

    public void ConfirmSelection()
    {
        var selected = patients[currentIndex];
    } 
}
