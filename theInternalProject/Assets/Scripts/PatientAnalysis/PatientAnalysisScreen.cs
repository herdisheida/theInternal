using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class PatientAnalysisScreen : MonoBehaviour
{
    public static bool isSaved = false;

    public Image InfectedImage;
    public Image SavedImage;

    public Sprite InfectedSprite;
    public Sprite SavedSprite;

    void Start()
    {
        // If saved, start countdown to Credits
        if (isSaved)
        {
            StartCoroutine(GoToCreditsAfterDelay());
        }
    }

    void Update()
    {
        if (isSaved)
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

    IEnumerator GoToCreditsAfterDelay()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("Credits");
    }
}
