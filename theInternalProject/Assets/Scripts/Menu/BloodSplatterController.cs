using UnityEngine;

public class BloodSplatterController : MonoBehaviour
{
    void Update()
    {
        if (GameManager.instance == null)
        {
            Debug.LogWarning("BloodSplatterController: GameManager instance not found.");
            return; 
        }

        bool showBlood = GameManager.instance.GetSavedCount() == 0;
        gameObject.SetActive(showBlood);
    }
}

