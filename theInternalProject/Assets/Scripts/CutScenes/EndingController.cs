using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;


// TOOD HERDIS muna setja þetta í patient selection screen til að checka hvort allir patientar hafa verið spilaðir
// if (GameManager.instance != null && AllPatientsResolved())
// {
//     UnityEngine.SceneManagement.SceneManager.LoadScene("EndingScene");
// }



public class EndingScreen : MonoBehaviour
{
    [Header("UI References")]
    public Image endingImage;                // big picture on screen
    public TextMeshProUGUI endingText;       // text that fades in/out

    [Header("Text Lines For Each Ending")]
    [TextArea] public string[] goodLines;
    [TextArea] public string[] partialyGoodLines;
    [TextArea] public string[] partialyBadLines;
    [TextArea] public string[] badLines;

    [Header("Timings")]
    public float fadeDuration = 0.8f;   // how long to fade in/out
    // public float holdDuration = 3f;     // how long text stays fully visible
    public bool loopLines = false;      // if true, keep cycling the lines
    private EndingType ending;
    private int savedCount;

    void Start()
    {
        // default to BAD if something goes wrong
        Debug.Log("Parti");
        ending = EndingType.PartialyGood;

        if (GameManager.instance != null)
        {
            ending = GameManager.instance.GetEndingType();
            savedCount = GameManager.instance.GetSavedCount();
        }

        // choose sprite + music + lines based on ending
        string[] lines = SetupVisualsForEnding(ending);

        // start text fade routine
        if (endingText != null && lines != null && lines.Length > 0)
        {
            StartCoroutine(ShowLinesRoutine(lines));
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (savedCount == 0) {
            SceneManager.LoadScene("Suicide");
        }
            else
            {
                SceneManager.LoadScene("Credits");
            }
        }
    }

    string[] SetupVisualsForEnding(EndingType ending)
    {
        string[] linesToUse = null;

        switch (ending)
        {
            case EndingType.Good:
                if (endingImage != null) 

                linesToUse = goodLines;
                AudioManager.instance?.PlayGoodEndingMusic();
                break;

            case EndingType.PartialyGood:
                if (endingImage != null)

                linesToUse = partialyGoodLines;
                AudioManager.instance?.PlayPartialEndingMusic();
                break;
            
            case EndingType.PartialyBad:
                if (endingImage != null)

                linesToUse = partialyBadLines;
                AudioManager.instance?.PlayPartialEndingMusic();
                break;

            case EndingType.Bad:
            default:
                if (endingImage != null)
                linesToUse = badLines;
                AudioManager.instance?.PlayBadEndingMusic();
                break;
        }

        // make sure text starts invisible
        if (endingText != null)
        {
            var c = endingText.color;
            c.a = 0f;
            endingText.color = c;
        }
        return linesToUse;
    }

    IEnumerator ShowLinesRoutine(string[] lines)
    {
        yield return new WaitForSeconds(2f); // initial delay before starting
        
        int index = 0;

        while (true)
        {
            string line = lines[index];

            // set text
            endingText.text = line;

            // fade IN
            yield return StartCoroutine(FadeTextAlpha(0f, 1f, fadeDuration));

            // // hold
            // yield return new WaitForSeconds(holdDuration);

            // // fade OUT
            // yield return StartCoroutine(FadeTextAlpha(1f, 0f, fadeDuration));

            // next line
            index++;

            if (index >= lines.Length)
            {
                if (loopLines)
                {
                    index = 0; // start over
                }
                // else
                // {
                //     // go to credits after last line
                //     StartCoroutine(GoToCreditsAfterDelay());
                //     yield break;
                // }
            }
        }
    }

    IEnumerator FadeTextAlpha(float start, float end, float duration)
    {
        if (endingText == null || duration <= 0f)
            yield break;

        float t = 0f;
        Color c = endingText.color;

        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / duration);
            c.a = Mathf.Lerp(start, end, lerp);
            endingText.color = c;
            yield return null;
        }

        // make sure final alpha is exact
        c.a = end;
        endingText.color = c;
    }

    // IEnumerator GoToCreditsAfterDelay()
    // {
    //     yield return new WaitForSeconds(0.2f);
    //     savedCount = GameManager.instance != null 
    //         ? GameManager.instance.GetSavedCount()
    //         : 0;
        
    // }
}
