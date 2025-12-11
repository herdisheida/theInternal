using UnityEngine;
using UnityEngine.SceneManagement;

public class TunnelPortal : MonoBehaviour
{

    [Header("Scene Flow")]
    private string weaponCutScene = "WeaponOnline";
    private string bossBattleScene;


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (GameManager.instance == null)
        {
            Debug.LogWarning("TunnelPortal: GameManager instance not found.");
            return;
        }
        if (GameManager.instance?.currentPatient == null)
        {
            Debug.LogWarning("TunnelPortal: currentPatient is null in GameManager.");
            return;
        }

        if (GameManager.instance?.patientLevelsPlayed == 0)
        {
            AudioManager.instance?.FadeOutMusic(3f);
            SceneManager.LoadScene(weaponCutScene);
        } else {
            bossBattleScene = GameManager.instance?.currentPatient.bossSceneName;
            AudioManager.instance?.StopMusic();
            SceneManager.LoadScene(bossBattleScene);
        }
    }
}
