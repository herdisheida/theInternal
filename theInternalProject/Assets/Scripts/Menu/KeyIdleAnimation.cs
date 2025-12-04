using UnityEngine;
using UnityEngine.UI;


public class KeyIdleAnimation : MonoBehaviour
{
    public Sprite unpressedSprite;
    public Sprite pressedSprite;

    public float swapInterval = 0.5f; // how fast it toggles

    private Image img;
    private float timer;
    private bool showingPressed = false;

    void Start()
    {
        img = GetComponent<Image>();

        // default sprite
        if (unpressedSprite != null)
            img.sprite = unpressedSprite;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= swapInterval)
        {
            timer = 0f;
            showingPressed = !showingPressed;

            img.sprite = showingPressed ? pressedSprite : unpressedSprite;
        }
    }
}
