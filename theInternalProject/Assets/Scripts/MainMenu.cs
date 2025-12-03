using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Tooltip("Name of the scene to load when player starts the game.")]
    public string sceneToLoadOnStart;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void StartGame()
    {
        // load the next scene
        if (!string.IsNullOrEmpty(sceneToLoadOnStart))
        {
            Debug.Log("Start Game");
            SceneManager.LoadScene(sceneToLoadOnStart);
        }
        else
        {
            Debug.LogWarning("PatientPatientSelection: sceneToLoad is not set!");
        }
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

}
