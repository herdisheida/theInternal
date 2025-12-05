using UnityEngine;
using UnityEngine.UI;

public class BackgroundScroller : MonoBehaviour
{
    [SerializeField] private RawImage _img;
    [SerializeField] private float _x;

    private bool _isScrolling = true;

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
}
