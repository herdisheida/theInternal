using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void StartGame()
    {
        SceneManager.LoadScene("PatientSelection");
    }

    public void QuitGame()
    {
        Application.Quit();
    }

}
