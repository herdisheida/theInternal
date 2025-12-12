using UnityEngine;
using UnityEngine.UI;

public class PatientSlot: MonoBehaviour
{
    public Image portraitImage;
    public PatientData data;

    public void Refresh(bool isSelected)
    {   
        var patient = data;

        if (data == null || portraitImage == null) return;
        
        var rt = portraitImage.rectTransform;

        if (patient.status == PatientStatus.Saved)
        {
            portraitImage.gameObject.SetActive(false);
            return;
        }

        portraitImage.gameObject.SetActive(true);

        if (patient.status == PatientStatus.Dead)
        {
            if (patient.dead != null)
            {
                portraitImage.sprite = patient.dead;
                rt.anchoredPosition = patient.deadSpriteOffset;
                rt.localScale = new Vector3(
                    patient.deadSpriteScale.x,
                    patient.deadSpriteScale.y,
                    1f
                );
            }
        }
        else if (isSelected)
        {
            if (patient.selected != null)
            {
                portraitImage.sprite = patient.selected;
                rt.anchoredPosition = patient.selectedSpriteOffset;
                rt.localScale = new Vector3(
                    patient.selectedSpriteScale.x,
                    patient.selectedSpriteScale.y,
                    1f
                );
            }
        }
        else
        {
            if (patient.unselected != null)
            {
                portraitImage.sprite = patient.unselected;
                rt.anchoredPosition = patient.unselectedSpriteOffset;
                rt.localScale = new Vector3(
                    patient.unselectedSpriteScale.x,
                    patient.unselectedSpriteScale.y,
                    1f
                );
            }
        }
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