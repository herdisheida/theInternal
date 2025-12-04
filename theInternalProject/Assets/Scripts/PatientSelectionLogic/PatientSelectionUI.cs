using UnityEngine;
using UnityEngine.UI;

public class PatientSelectionUI : MonoBehaviour
{
    [Header("Data")]
    public PatientSlot[] PatientSlots; 

    private int currentIndex = 0;

    [Header("Arrow")]
    public Image arrow;
    public float arrowOffset = 30f;



    public void Start()
    {

        UpdateSprite(currentIndex);
        MoveArrowToCurrent();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow)|| Input.GetKeyDown(KeyCode.D))
        {
            currentIndex = Mathf.Min(currentIndex +1, PatientSlots.Length -1);
            UpdateSprite(currentIndex);
            MoveArrowToCurrent();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow)|| Input.GetKeyDown(KeyCode.A))
        {
            currentIndex = Mathf.Max(currentIndex -1, 0);
            UpdateSprite(currentIndex);
            MoveArrowToCurrent();
        }
    }

    private void UpdateSprite(int index)
    {
        
        for (int i = 0; i < PatientSlots.Length; i++)
        {
            bool isSelected = (index == i);
            PatientSlots[i].Refresh(isSelected);
        }
    }

    private void MoveArrowToCurrent()
    {
        if (arrow == null || PatientSlots[currentIndex] == null) return;

        RectTransform targetPatient = PatientSlots[currentIndex].GetComponent<RectTransform>();
        RectTransform arrowPoint = arrow.rectTransform;


        Vector3[] corners = new Vector3[4];
        targetPatient.GetWorldCorners(corners);

        Vector3 topCenter = (corners[1] + corners[2]) * 0.5f;

        arrowPoint.position = topCenter + new Vector3(0, arrowOffset, 0);
        // float x = targetPatient.anchoredPosition.x;
        // float y = targetPatient.anchoredPosition.y + (targetPatient.rect.height * 0.2f) + arrowOffset;
        
        // arrowPoint.anchoredPosition = new Vector2(x,y);
    }
}
