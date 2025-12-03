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
        // play button click sound
        AudioManager.instance?.ButtonClick();

        // then load the next scene
        if (!string.IsNullOrEmpty(sceneToLoadOnStart))
        {
            SceneManager.LoadScene(sceneToLoadOnStart);
        }
        else
        {
            Debug.LogWarning("PatientPatientSelection: sceneToLoad is not set!");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
