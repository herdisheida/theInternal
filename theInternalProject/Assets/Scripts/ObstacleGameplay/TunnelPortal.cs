using UnityEngine;
using UnityEngine.SceneManagement;

public class TunnelPortal : MonoBehaviour
{

    [Tooltip("Name of the scene to load when the player enters the tunnel.")]
    public string sceneToLoad;

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
            SceneManager.LoadScene(sceneToLoad);
        }
        else
        {
            Debug.LogWarning("TunnelPortal: sceneToLoad is not set!");
        }
    }
}
