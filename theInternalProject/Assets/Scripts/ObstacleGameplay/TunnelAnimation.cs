using UnityEngine;
using UnityEngine.UI;


public class TunnelAnimation : MonoBehaviour
{
    public Sprite tunnel01Sprite;
    public Sprite tunnel02Sprite;

    public float swapInterval = 0.5f; // how fast it toggles

    private SpriteRenderer tunnel;
    private float timer;
    private bool showingFirstSprite = false;

    void Start()
    {
        tunnel = GetComponent<SpriteRenderer>();

        // default sprite
        if (tunnel01Sprite != null)
            tunnel.sprite = tunnel01Sprite;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= swapInterval)
        {
            timer = 0f;
            showingFirstSprite = !showingFirstSprite;

            tunnel.sprite = showingFirstSprite ? tunnel02Sprite : tunnel01Sprite;
        }
    }
}
