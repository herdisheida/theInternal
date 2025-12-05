using UnityEngine;

public class CreditScroll : MonoBehaviour
{
    public float scrollSpeed = 40f; // adjust the speed of the scroll
    private RectTransform rectTransform;

    public float delayScrollTime = 2f; // delay before starting the scroll

    void Start()
    {
        // get the RectTransform component
        rectTransform = GetComponent<RectTransform>();
        AudioManager.instance?.PlayCredits();
    }

    void Update()
    {
        if (delayScrollTime > 0f)
        {
            delayScrollTime -= Time.deltaTime;
            return;
        }

        // move the credits upward
        rectTransform.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
    }
}
