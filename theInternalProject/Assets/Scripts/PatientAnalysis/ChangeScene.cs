using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

public class ChangeScene : MonoBehaviour
{
    public void Update()
    {
        PatientData patient = GameManager.instance?.currentPatient;
        if (GameManager.instance.dialogOver) {
            if (patient.status != PatientStatus.Saved)
            {
                if (Input.GetKeyDown(KeyCode.Space)) {
                    AudioManager.instance?.StopMusic();

                    if (GameManager.instance?.patientLevelsPlayed == 0) 
                    {
                        SceneManager.LoadScene("BloodstreamIntro");
                    }
                    else
                    {
                        HealthSystem.ResetSharedHealth(); // reset health for next patient
                        SceneManager.LoadScene("ObstacleGameplay");
                    }
                }
                else if (Input.GetKeyDown(KeyCode.Backspace))
                {
                    SceneManager.LoadScene("PatientSelection");
                }
            }
            else
            {
                if (GameManager.instance?.patientLevelsPlayed == 3)
                {
                    StartCoroutine(GoToEnding());
                }
                else
                { 
                    StartCoroutine(GoBackToSelection());
                }
            }
        }
    }

    IEnumerator GoBackToSelection()
    {
        AudioManager.instance?.StopMusic(); // stop music after boss battle
        AudioManager.instance?.PlayHospitalLobbyMusic(); // play lobby music while patient thanks player

        yield return new WaitForSeconds(7.5f); // feedback/msg from patient duration after being saved
        SceneManager.LoadScene("PatientSelection");
    }

    IEnumerator GoToEnding()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("EndingScene");
    }
}
