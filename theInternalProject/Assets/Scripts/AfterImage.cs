using UnityEngine;

public class AfterImageGenerator : MonoBehaviour
{
    public SpriteRenderer source;
    public GameObject afterImagePrefab;
    public float spawnInterval = 0.03f;

    private float timer;
    private bool active = false;

    public void StartAfterImages()
    {
        active = true;
        timer = 0f;
    }

    public void StopAfterImages()
    {
        active = false;
    }

    void Update()
    {
        if (!active) return;

        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            SpawnAfterImage();
            timer = spawnInterval;
        }
    }

    void SpawnAfterImage()
    {
        GameObject img = Instantiate(afterImagePrefab, source.transform.position, Quaternion.identity);
        SpriteRenderer sr = img.GetComponent<SpriteRenderer>();
        sr.sprite = source.sprite;
        sr.flipX = source.flipX;
    }
}
