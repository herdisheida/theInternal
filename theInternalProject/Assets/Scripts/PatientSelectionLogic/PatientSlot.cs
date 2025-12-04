using UnityEngine;
using UnityEngine.UI;

public class PatientSlot: MonoBehaviour
{
    public Image portraitImage;
    public PatientData data;
    public string sceneName;

    public void Refresh(bool isSelected)
    {
        if (data == null || portraitImage == null) return;
        portraitImage.sprite = isSelected? data.selected: data.unselected;
    }

    public bool IsSelectable()
    {   
        if (portraitImage == null)
        {
            return false;
        }
        if (data.isSaved)
        {
            return false;
        }
        Sprite currentSprite = portraitImage.sprite;

        if (currentSprite == data.dead) return false;
        if (currentSprite == data.infected) return false;

        return true;
    }
}