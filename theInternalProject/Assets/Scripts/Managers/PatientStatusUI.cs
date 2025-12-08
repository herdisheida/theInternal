using UnityEngine;
using UnityEngine.UI;

public class PatientStatusUI : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void RefreshVisual(PatientData data, Image img)
    {
        switch (data.status)
        {
            case PatientStatus.None:
                img.sprite = data.unselected;
                break;
            case PatientStatus.Infected:
                img.sprite = data.infected;
                break;
            case PatientStatus.Dead:
                img.sprite = data.dead;
                break;
            case PatientStatus.Saved:
                img.sprite = data.saved;
                break;
        }
    }
}
