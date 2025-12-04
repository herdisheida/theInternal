using UnityEngine;

// this class manages overall audio functionalities
// background music, sound effects, etc
public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Sources")]
    public AudioSource musicSource; // drag and drop AudioSource component in inspector (looping music)
    public AudioSource sfxSource; // drag and drop another AudioSource component in inspector


    [Header("Background Music Clips")]
    public AudioClip menuScreenMusic;
    public AudioClip hospitalLobbyMusic;
    public AudioClip obstacleGameplayMusic;
    public AudioClip bossBattleMusic;

    public AudioClip goodEndingMusic;    // save all patients
    public AudioClip badEndingMusic;     // save no patients
    public AudioClip partialEndingMusic; // save some patients but not all

    public AudioClip creditsMusic; // credits music


    [Header("Sound Effect Clips")]
    // menu-related
    public AudioClip buttonClickClip; // start, quit buttons

    // player-related
    public AudioClip damageTakenClip;
    public AudioClip dyingClip;
    public AudioClip attackClip;        // medicine bullet shooting

    // shoot patient-related
    public AudioClip shootPatientClip;    // shoot infected patient
    public AudioClip heavyBreathingClip;  // breath heavily while shooting
    public AudioClip deepExhaleClip;      // exhale after shooting

    // enemy-related
    public AudioClip zombieChompClip;
    public AudioClip enemyDeathClip;
 
    // ambient-related


    void Awake()
    {
        // singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
    }

    void Update()
    {
        
    }


    // Play a specific music clip (chosen from Inspector or passed in).
    // Example: AudioManager.instance?.PlayMusic(AudioManager.instance.bossBattleMusic);
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource == null || clip == null) return;

        // if already playing the same clip, do nothing
        if (musicSource.isPlaying && musicSource.clip == clip) return;

        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void StopMusic()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }
    }

    void PlaySFX(AudioClip clip)
    {
        if (sfxSource == null || clip == null) return;
        if (!sfxSource.isActiveAndEnabled) return; // prevents errors if AudioSource is disabled
        sfxSource.PlayOneShot(clip);
    }

    public void StopSFX()
    {
        if (sfxSource != null)
        {
            sfxSource.Stop();
        }
    }




    // helper methods for playing background music
    // Example: AudioManager.instance?.PlayHospitalLobbyMusic();
    public void PlayMenuScreenMusic()        => PlayMusic(menuScreenMusic);
    public void PlayHospitalLobbyMusic()     => PlayMusic(hospitalLobbyMusic);
    public void PlayObstacleGameplayMusic()  => PlayMusic(obstacleGameplayMusic);
    public void PlayBossBattleMusic()        => PlayMusic(bossBattleMusic);

    public void PlayGoodEndingMusic()        => PlayMusic(goodEndingMusic);
    public void PlayBadEndingMusic()         => PlayMusic(badEndingMusic);
    public void PlayPartialEndingMusic()     => PlayMusic(partialEndingMusic);

    public void PlayCredits()                => PlayMusic(creditsMusic);


    // helper methods for SFX
    // Example: AudioManager.instance?.ButtonClick();
    public void ButtonClick()    => PlaySFX(buttonClickClip);
    
    public void DamageTaken()    => PlaySFX(damageTakenClip);
    public void Dying()          => PlaySFX(dyingClip);
    public void Attack()         => PlaySFX(attackClip);

    public void ShootPatient()   => PlaySFX(shootPatientClip);
    public void HeavyBreathing()    => PlaySFX(heavyBreathingClip);
    public void DeepExhale()         => PlaySFX(deepExhaleClip);

    public void ZombieChomp()    => PlaySFX(zombieChompClip);
    public void EnemyDeath()     => PlaySFX(enemyDeathClip);
}
