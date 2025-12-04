using UnityEngine;
using UnityEngine.UI;

public class PatientSlot: MonoBehaviour
{
    public Image portraitImage;
    public PatientData data;

    public void Refresh(bool isSelected)
    {
        if (data == null || portraitImage == null) return;
        portraitImage.sprite = isSelected? data.alive: data.unselected;
    }
}