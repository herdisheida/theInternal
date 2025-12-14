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
                // patient is saved
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (GameManager.instance?.patientLevelsPlayed == 3)
                    {
                        SceneManager.LoadScene("EndingScene");
                    }
                    else
                    {
                        SceneManager.LoadScene("PatientSelection");
                    }
                }
            }
        }
    }
}
