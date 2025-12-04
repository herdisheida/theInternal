using UnityEngine;

public class CreditScroll : MonoBehaviour
{
    public float scrollSpeed = 40f; // adjust the speed of the scroll
    private RectTransform rectTransform;

    void Start()
    {
        // get the RectTransform component
        rectTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        // move the credits upward
        rectTransform.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
    }
}
