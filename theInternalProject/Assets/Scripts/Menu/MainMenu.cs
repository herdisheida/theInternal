using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Tooltip("Name of the scene to load when player starts the game.")]
    public string sceneToLoadOnStart;

    void Start()
    {
        AudioManager.instance?.PlayMenuScreenMusic();
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
            AudioManager.instance?.StopMusic();
            AudioManager.instance?.PlayHospitalLobbyMusic(); // TODO HERDIS: move this to the lobby scene (so it starts every time we load the lobby)
            SceneManager.LoadScene(sceneToLoadOnStart);
        }
        else
        {
            Debug.LogWarning("PatientSelection: sceneToLoad is not set!");
        }
    }

    public void QuitGame()
    {
        AudioManager.instance?.ButtonClick();
        Application.Quit();
    }

}
