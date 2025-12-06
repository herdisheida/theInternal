using UnityEngine;

public class DamageFlash : MonoBehaviour
{
    public SpriteRenderer[] spriteRenderers;
    public Color flashColor = Color.red;
    public float flashDuration = 0.1f;

    private Color[] originalColors;
    private bool isFlashing = false;

    void Start()
    {
        if (spriteRenderers == null || spriteRenderers.Length == 0)
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();

        originalColors = new Color[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
            originalColors[i] = spriteRenderers[i].color;
    }

    public void Flash()
    {
        if (!isFlashing)
            StartCoroutine(FlashRoutine());
    }

    private System.Collections.IEnumerator FlashRoutine()
    {
        isFlashing = true;

        // turn red
        foreach (var sr in spriteRenderers)
            if (sr != null) sr.color = flashColor;

        yield return new WaitForSeconds(flashDuration);

        // back to normal
        for (int i = 0; i < spriteRenderers.Length; i++)
            if (spriteRenderers[i] != null) spriteRenderers[i].color = originalColors[i];

        isFlashing = false;
    }
}
