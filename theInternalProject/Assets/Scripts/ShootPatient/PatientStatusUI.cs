using UnityEngine;
using UnityEngine.UI;

public class PatientStatusUI : MonoBehaviour
{
    [Header("References")]
    public PatientData patientData;
    public Image patientImage;

    private PatientStatus lastStatus;

    [Header("Background References")]
    public Image backgroundImagePlaceholder;
    public Sprite zombieBg;
    public Sprite werewolfBg;
    public Sprite vampireBg;


    void Start()
    {
        // get current patient data
        if (patientData == null && GameManager.instance != null) { patientData = GameManager.instance.currentPatient; }

        if (patientData == null || patientImage == null)
        {
            Debug.LogWarning($"{name}: PatientStatusUI missing references.");
            enabled = false;
            return;
        }

        lastStatus = patientData.status;
        RefreshVisual();
    }

    void Update()
    {
        if (patientData == null && GameManager.instance != null) { patientData = GameManager.instance.currentPatient; }

        // only update when status changes
        if (patientData.status != lastStatus)
        {
            lastStatus = patientData.status;
            RefreshVisual();
        }
    }


    public void RefreshVisual()
    {
        if (patientData == null || patientImage == null)
            return;

        // get infected patient
        patientImage.sprite = patientData.infected;

        // scale patient based on type
         RectTransform rt = patientImage.rectTransform;

        // reset before applying new values
        rt.localScale = Vector3.one;
        rt.anchoredPosition = Vector2.zero;

        // update based on patient type
        switch (patientData.patientType)
        {
            case PatientType.Zombie:
                rt.localScale = new Vector3(2.4f, 2.4f, 2.4f);  // size
                rt.anchoredPosition = new Vector2(-110f, -24f);  // pos
                backgroundImagePlaceholder.sprite = zombieBg;
                break;

            case PatientType.Werewolf:
                rt.localScale = new Vector3(2.9f, 2.9f, 2.9f);  // size
                rt.anchoredPosition = new Vector2(-58f, 9f);  // pos
                backgroundImagePlaceholder.sprite = werewolfBg;
                break;

            case PatientType.Vampire:
                rt.localScale = new Vector3(2.4f, 2.4f, 2.4f);  // size
                rt.anchoredPosition = new Vector2(-79f, 13f);  // pos
                backgroundImagePlaceholder.sprite = vampireBg;
                break;
        }
    }
}
