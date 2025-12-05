using UnityEngine.SceneManagement;
using UnityEngine;

public class ChangeScene : MonoBehaviour
{
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            AudioManager.instance?.StopMusic();
            SceneManager.LoadScene("ObstacleGameplay");
        }
    }
}
