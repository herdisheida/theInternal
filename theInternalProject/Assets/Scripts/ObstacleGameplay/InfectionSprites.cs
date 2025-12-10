using UnityEngine;

public class InfectionSprites : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    void Awake()
    {
        var patient = GameManager.instance?.currentPatient;
        if (patient == null && patient.infectionSprite != null)
        {
            spriteRenderer.sprite = patient.infectionSprite;
        }
    }   
}
