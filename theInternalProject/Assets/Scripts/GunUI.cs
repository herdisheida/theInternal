using UnityEngine;
using TMPro;
using System.Collections;

public class GunUI : MonoBehaviour
{
    public TextMeshProUGUI ammoText;
    public TextMeshProUGUI reloadText;
    public float flashDuration = 0.5f;

    private Coroutine flashCoroutine;

    public void UpdateAmmoUI(int current, int max)
    {
        ammoText.text = current + "/" + max;
    }

    public void showReloading()
    {
        reloadText.gameObject.SetActive(true);
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        flashCoroutine = StartCoroutine(FlashReloadText());
    }
    public void hideReloading()
    {
        if (flashCoroutine != null)
        {
            StopCoroutine(flashCoroutine);
        }
        reloadText.gameObject.SetActive(false);
    }

    private IEnumerator FlashReloadText()
    {
        Color original = reloadText.color;
        Color transparent = original;
        original.a = 1f;
        transparent.a = 0f;

        while (true)
        {
            
            float t = 0f;
            while (t < flashDuration)
            {
                t += Time.deltaTime;
                reloadText.color = Color.Lerp(transparent, original, t / flashDuration);
                yield return null;
            }
            t = 0f;
            while (t < flashDuration)
            {
                t += Time.deltaTime;
                reloadText.color = Color.Lerp(original, transparent, t / flashDuration);
                yield return null;
            }
        }
    }


}
