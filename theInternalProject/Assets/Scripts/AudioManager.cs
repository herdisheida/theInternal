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
    public AudioClip hospitalLobbyMusic;
    public AudioClip obstacleGameplayMusic;
    public AudioClip bossBattleMusic;

    public AudioClip goodEndingMusic;    // save all patients
    public AudioClip badEndingMusic;     // save no patients
    public AudioClip partialEndingMusic; // save some patients but not all


    [Header("Default Music")]
    public AudioClip defaultMusic; // optional default music clip -- when PlayMusic() with no arguments is called
    private AudioClip currentMusic; // to keep track of currently playing music


    [Header("Sound Effect Clips")]
    // player-related
    public AudioClip damageTakenClip;
    public AudioClip dyingClip;
    public AudioClip attackClip;        // medicine bullet shooting
    public AudioClip shootPatientClip;  // shoot infected patient
    // enemy-related
    public AudioClip enemyAttackClip;
    public AudioClip enemyDeathClip;
    // menu-related
    public AudioClip buttonClickClip; // start, quit buttons
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
        currentMusic = defaultMusic;
    }

    void Start()
    {
        PlayMusic(); 
    }

    void Update()
    {
        
    }


    /// Play whatever is in currentMusic / defaultMusic.
    public void PlayMusic()
    {
        if (musicSource == null) return;

        // choose target clip: currentMusic > defaultMusic
        AudioClip targetClip = currentMusic != null ? currentMusic : defaultMusic;
        if (targetClip == null) return;

        // if we’re already playing this exact clip, don’t restart it
        if (musicSource.isPlaying && musicSource.clip == targetClip) return;

        musicSource.clip = targetClip;
        musicSource.loop = true;
        musicSource.Play();
    }



    // Play a specific music clip (chosen from Inspector or passed in).
    // Example: AudioManager.instance.PlayMusic(AudioManager.instance.bossBattleMusic);
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource == null || clip == null) return;

        // if already playing the same clip, do nothing
        if (musicSource.isPlaying && musicSource.clip == clip) return;

        currentMusic = clip;
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
        sfxSource.PlayOneShot(clip);
    }





    // helper methods for background music
    public void PlayHospitalLobbyMusic()     => PlayMusic(hospitalLobbyMusic);
    public void PlayObstacleGameplayMusic()  => PlayMusic(obstacleGameplayMusic);
    public void PlayBossBattleMusic()        => PlayMusic(bossBattleMusic);

    public void PlayGoodEndingMusic()        => PlayMusic(goodEndingMusic);
    public void PlayBadEndingMusic()         => PlayMusic(badEndingMusic);
    public void PlayPartialEndingMusic()     => PlayMusic(partialEndingMusic);


    // helper methods for SFX
    public void buttonClick()    => PlaySFX(buttonClickClip);
    
    public void damageTaken()    => PlaySFX(damageTakenClip);
    public void dying()          => PlaySFX(dyingClip);
    public void attack()         => PlaySFX(attackClip);
    public void shootPatient()   => PlaySFX(shootPatientClip);

    public void enemyAttack()    => PlaySFX(enemyAttackClip);
    public void enemyDeath()     => PlaySFX(enemyDeathClip);
}
