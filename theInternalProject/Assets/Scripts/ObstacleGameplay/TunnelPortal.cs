using UnityEngine;
using UnityEngine.SceneManagement;

public class TunnelPortal : MonoBehaviour
{

    [Header("Scene Flow")]
    public string nextSceneName = "WeaponOnline";

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        AudioManager.instance?.FadeOutMusic(1.0f);
        // load the next scene
        SceneManager.LoadScene(nextSceneName);
    }
}
