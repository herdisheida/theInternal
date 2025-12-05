using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class WeaponOnlineController : MonoBehaviour
{
    [Header("UI References")]
    public Image fadeImage;            // full-screen black image
    public CanvasGroup hudGroup;       // panel that flickers on
    public TextMeshProUGUI statusText; // main HUD text
    public GameObject spaceKeyHint;    // the space-bar image + text container

    [Header("Gun Glow")]
    public DamageFlash gunFlash;       // DamageFlash on your GunIdle object
    public float glowDelay = 0.3f;

    [Header("Timings")]
    public float darkenDuration = 0.5f;
    public float flickerDuration = 0.6f;
    public float textHoldTime = 1.5f;

    [Header("Scene Flow")]
    public string nextSceneName = "BossBattle";

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
