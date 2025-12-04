using UnityEngine;
using UnityEngine.UI;
public class PatientAnalysisScreen : MonoBehaviour
{
    public bool isSaved = false;
    public Image InfectedImage;
    public Image SavedImage;

    public Sprite InfectedSprite;
    public Sprite SavedSprite;

    public void Update()
    {
        if (isSaved == true)
        {
            SavedImage.gameObject.SetActive(true);
            InfectedImage.gameObject.SetActive(false);
            SavedImage.sprite = SavedSprite;

        }
        else
        {
            InfectedImage.gameObject.SetActive(true);
            SavedImage.gameObject.SetActive(false);
            InfectedImage.sprite = InfectedSprite;
        }
    }
}
