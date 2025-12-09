using UnityEngine;
using UnityEngine.UI;
using System.Collections;


public class BackgroundScroller : MonoBehaviour
{
    [SerializeField] private RawImage _img;
    [SerializeField] private float _x;

    private bool _isScrolling = true;

    void Awake()
    {
        if (_img == null)
            _img = GetComponent<RawImage>();
    }

    void Update()
    {
        if (!_isScrolling) return;

        _img.uvRect = new Rect(
            new Vector2(_img.uvRect.x + _x * Time.deltaTime, 0f),
            _img.uvRect.size
        );
    }


    // stops the scrolling of the background
    public void StopScrolling()
    {
        _isScrolling = false;
    }


    // -------- OPTIONAL: fade in then start scrolling ----------
    public IEnumerator FadeInAndStart(float duration)
    {
        if (_img == null)
            yield break;


        Color c = _img.color;
        float originalAlpha = c.a;

        // make sure we start from 0 alpha for the fade
        c.a = 0f;
        _img.color = c;

        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float lerp = Mathf.Clamp01(t / duration);

            c.a = Mathf.Lerp(0f, 1f, lerp);
            _img.color = c;

            yield return null;
        }

        c.a = 1f;
        _img.color = c;
    }
}
