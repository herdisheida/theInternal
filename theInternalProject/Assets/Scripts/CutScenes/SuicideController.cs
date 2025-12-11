using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class SuicideController : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(Shooting());
    }

    IEnumerator Shooting()
    {
        AudioManager.instance?.ShootPatient(); 
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("Credits");
    }
}
