using UnityEngine;

public class PatientSelectionUI : MonoBehaviour
{
    [Header("Data")]
    // public PatientData[] patients; 
    public PatientSlot[] slots; 
    private int currentIndex = 0;

    public void Start()
    {

        UpdateSprite(currentIndex);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow)|| Input.GetKeyDown(KeyCode.D))
        {
            currentIndex = Mathf.Min(currentIndex +1, slots.Length -1);
            UpdateSprite(currentIndex);
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow)|| Input.GetKeyDown(KeyCode.A))
        {
            currentIndex = Mathf.Max(currentIndex -1, 0);
            UpdateSprite(currentIndex);
        }
    }

    private void UpdateSprite(int index)
    {
        
        for (int i = 0; i < slots.Length; i++)
        {
            bool isSelected = (index == i);
            slots[i].Refresh(isSelected);
        }
    }
}
