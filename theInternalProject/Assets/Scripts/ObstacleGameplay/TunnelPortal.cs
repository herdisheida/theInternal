using UnityEngine;
using UnityEngine.SceneManagement;

public class TunnelPortal : MonoBehaviour
{

    [Header("Scene Flow")]
    private string weaponCutScene = "WeaponOnline";
    private string bossBattleScene;

    void Start()
    {
        gm = GameManager.instance;
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (gm == null)
        {
            Debug.LogWarning("TunnelPortal: GameManager instance not found.");
            return;
        }
        if (gm.currentPatient == null)
        {
            Debug.LogWarning("TunnelPortal: currentPatient is null in GameManager.");
            return;
        }

        if (gm.patientLevelsPlayed == 0)
        {
            AudioManager.instance?.FadeOutMusic(3f);
            SceneManager.LoadScene(weaponCutScene);
        } else {
            bossBattleScene = gm.currentPatient.bossSceneName;
            AudioManager.instance?.StopMusic();
            SceneManager.LoadScene(bossBattleScene);
        }
    }
}
