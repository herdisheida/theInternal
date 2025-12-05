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

        // load the next scene
        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            AudioManager.instance?.FadeOutMusic(1.5f);
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("TunnelPortal: sceneToLoad is not set!");
        }
    }
}
