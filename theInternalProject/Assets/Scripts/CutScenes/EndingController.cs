using UnityEngine;
using UnityEngine.UI;


// TOOD HERDIS muna setja þetta í patient selection screen til að checka hvort allir patientar hafa verið spilaðir
// if (GameManager.instance != null && AllPatientsResolved())
// {
//     UnityEngine.SceneManagement.SceneManager.LoadScene("EndingScene");
// }




public class EndingController : MonoBehaviour
{
    [Header("UI")]
    public Text endingText;
    public Image endingImage;
    public Sprite goodSprite;
    public Sprite partialSprite;
    public Sprite badSprite;

    void Start()
    {
        var gameManager = GameManager.instance;

        EndingType ending = EndingType.Bad;
        int saved = 0;
        int total = 0;

        if (gameManager != null)
        {
            ending = gameManager.GetEndingType();
            saved = gameManager.GetSavedCount();
            total = gameManager.GetTotalPatients();
        }

        switch (ending)
        {
            case EndingType.Good:
                if (endingText != null)
                    endingText.text = $"All {total} patients were saved.";
                if (endingImage != null && goodSprite != null)
                    endingImage.sprite = goodSprite;
                AudioManager.instance?.PlayGoodEndingMusic();
                break;

            case EndingType.Partial:
                if (endingText != null)
                    endingText.text = $"{saved} out of {total} patients were saved.";
                if (endingImage != null && partialSprite != null)
                    endingImage.sprite = partialSprite;
                AudioManager.instance?.PlayPartialEndingMusic();
                break;

            case EndingType.Bad:
            default:
                if (endingText != null)
                    endingText.text = "None of the patients survived.";
                if (endingImage != null && badSprite != null)
                    endingImage.sprite = badSprite;
                AudioManager.instance?.PlayBadEndingMusic();
                break;
        }
    }
}
