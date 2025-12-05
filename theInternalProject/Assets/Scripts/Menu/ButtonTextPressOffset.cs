using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonTextPressOffset : MonoBehaviour,
    IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    [Header("Text to move")]
    public RectTransform textTransform;    // child TextMeshPro object (text i want to move when button is pressed)

    [Header("Movement")]
    public float pressDistance = -4f;      // negative Y = move down

    private Vector2 originalAnchoredPos;

    void Awake()
    {
        if (textTransform == null)
        {
            // try to auto-find a TMP text in children
            var tmp = GetComponentInChildren<TMPro.TextMeshProUGUI>();
            if (tmp != null)
                textTransform = tmp.GetComponent<RectTransform>();
        }

        if (textTransform != null)
            originalAnchoredPos = textTransform.anchoredPosition;
    }

    void OnEnable()
    {
        // make sure it resets correctly when re-enabled
        if (textTransform != null) { textTransform.anchoredPosition = originalAnchoredPos; }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (textTransform == null) return;

        // move text down a bit
        textTransform.anchoredPosition = originalAnchoredPos + new Vector2(0f, pressDistance);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        ResetTextPosition();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // if the mouse/finger leaves the button while pressed
        ResetTextPosition();
    }

    private void ResetTextPosition()
    {
        if (textTransform != null)
            textTransform.anchoredPosition = originalAnchoredPos;
    }
}
