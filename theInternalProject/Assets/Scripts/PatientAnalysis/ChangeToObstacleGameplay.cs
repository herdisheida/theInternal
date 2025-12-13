using UnityEngine.SceneManagement;
using UnityEngine;
using System.Collections;

public class ChangeScene : MonoBehaviour
{
    public void Update()
    {
        PatientData patient = GameManager.instance?.currentPatient;
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

    IEnumerator GoBackToSelection()
    {
        yield return new WaitForSeconds(5f); // after saving patient, wait 5 seconds
        SceneManager.LoadScene("PatientSelection");
    }

    IEnumerator GoToEnding()
    {
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("EndingScene");
    }
}
