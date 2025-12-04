using UnityEditor.SearchService;
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
            MoveRight();
            UpdateSprite(currentIndex);
            MoveArrowToCurrent();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow)|| Input.GetKeyDown(KeyCode.A))
        {
            MoveLeft();
            UpdateSprite(currentIndex);
            MoveArrowToCurrent();
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LoadPatientScreen();
        }
    }

    private void MoveRight()
    {
        int nextIndex = currentIndex;

        do
        {
            nextIndex++;
            if (nextIndex >= PatientSlots.Length) 
                return;
        } while (!PatientSlots[nextIndex].IsSelectable());

        currentIndex = nextIndex;
    }

    private void MoveLeft()
    {
        int prevIndex = currentIndex;

        do
        {
            prevIndex--;
            if (prevIndex < 0)
                return;
        } while (!PatientSlots[prevIndex].IsSelectable());

        currentIndex = prevIndex;
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
    }

    private void LoadPatientScreen()
    {
        string sceneToLoad = PatientSlots[currentIndex].sceneName;

        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
        }
    }
}
