using UnityEngine;
using UnityEngine.UI;

public class PatientSlot: MonoBehaviour
{
    public Image portraitImage;
    public PatientData data;

    public void Refresh(bool isSelected)
    {
        if (data == null || portraitImage == null) return;
        
        if (data.status == PatientStatus.Dead) return;
        
        portraitImage.sprite = isSelected? data.selected: data.unselected;
    }

    public bool IsSelectable()
    {   
        if (portraitImage == null)
        {
            return false;
        }
        if (data == null)
        {
            return false;
        }
        if (data.status == PatientStatus.Saved)
        {
            return false;
        }
        
        Sprite currentSprite = portraitImage.sprite;

        if (currentSprite == data.infected) return false;
        if (currentSprite == data.dead) return false;

        return true;
    }
}