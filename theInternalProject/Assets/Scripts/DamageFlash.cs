using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Color flashColor = Color.red;
    public float flashDuration = 0.1f;

    private Color originalColor;
    private bool isFlashing = false;

    void Start()
    {
        originalColor = spriteRenderer.color;
    }

    public void Flash()
    {
        if (!isFlashing)
            StartCoroutine(FlashRoutine());
    }

    private System.Collections.IEnumerator FlashRoutine()
    {
        isFlashing = true;

        spriteRenderer.color = flashColor;         // turn red
        yield return new WaitForSeconds(flashDuration);

        spriteRenderer.color = originalColor;      // return to normal
        isFlashing = false;
    }
}
