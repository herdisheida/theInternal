using UnityEngine;

public class BloodSplatterController : MonoBehaviour
{
    

    void Start()
    {
        if (GameManager.instance = null)
        {
            Debug.LogWarning("BloodSplatterController: GameManager instance not found.");
        }

        bool showBlood = GameManager.instance.GetSavedCount() == 0;
        gameObject.SetActive(showBlood);
    }
}
