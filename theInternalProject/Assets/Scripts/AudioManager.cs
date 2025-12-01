using UnityEngine;

// this class manages overall audio functionalities
// background music, sound effects, etc
public class AudioManager : MonoBehaviour
{
    [Header("Sources")]
    public AudioSource musicSource; // drag and drop AudioSource component in inspector (looping music)
    public AudioSource sfxSource; // drag and drop another AudioSource component in inspector


    [Header("Background Music Clips")]
    public AudioClip hospitalLobbyMusic;
    public AudioClip obstacleGameplayMusic;
    public AudioClip bossBattleMusic;

    public AudioClip goodEndingMusic; // save all patients
    public AudioClip badEndingMusic; // save no patients
    public AudioClip partialEndingMusic; // save some patients but not all


    [Header("Sound Effect Clips")]
    // menu-related
    public AudioClip buttonClickClip; // start, quit buttons

    // ambient-related

    // player-related
    public AudioClip damageTakenClip;
    public AudioClip dyingClip;
    public AudioClip attackClip; // medicine bullet shooting
    public AudioClip shootPatientClip; // shoot infected patient

    // enemy-related
    public AudioClip enemyAttackClip;
    public AudioClip enemyDeathClip;


    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }


    public void PlayMusic()
    {
        if (musicSource == null || backgroundMusic == null) return;

        if (musicSource.isPlaying) return; // don’t restart if already playing

        musicSource.clip = backgroundMusic;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null)
            musicSource.Stop();
    }

    void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null) return;
        sfxSource.PlayOneShot(clip);
    }



    // helper methods
    public void buttonClick()    => PlaySFX(buttonClickClip);
    
    public void damageTaken()    => PlaySFX(damageTakenClip);
    public void dying()          => PlaySFX(dyingClip);
    public void attack()         => PlaySFX(attackClip);
    public void shootPatient()   => PlaySFX(shootPatientClip);

    public void enemyAttack()    => PlaySFX(enemyAttackClip);
    public void enemyDeath()     => PlaySFX(enemyDeathClip);
}
