using UnityEngine;
using System.Collections;


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
    public AudioClip zombieBossBattleMusic;

    public AudioClip goodEndingMusic;    // save all patients
    public AudioClip badEndingMusic;     // save no patients
    public AudioClip partialEndingMusic; // save some patients but not all

    public AudioClip creditsMusic; // credits music


    [Header("Sound Effect Clips")]
    [Header("Menu SFX")]
    public AudioClip buttonClickClip; // start, quit buttons

    [Header("Player SFX")]
    public AudioClip damageTakenClip;
    public AudioClip dyingClip;
    public AudioClip attackClip;        // medicine bullet shooting

    [Header("Player Shoot Patient SFX")]
    public AudioClip shootPatientClip;    // shoot infected patient
    public AudioClip heavyBreathingClip;  // breath heavily while shooting
    public AudioClip deepExhaleClip;      // exhale after shooting

    [Header("Zombie Enemy SFX")]
    public AudioClip zombieChompClip;
    public AudioClip zombieDeathClip;
 


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

    // ---------------------- music / soundtracks ----------------------


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

    public void FadeOutMusic(float duration = 1f)
    {
        if (musicSource != null)
            StartCoroutine(FadeOutMusicRoutine(duration));
    }

    private IEnumerator FadeOutMusicRoutine(float duration)
    {
        if (musicSource == null || !musicSource.isPlaying)
            yield break;

        float startVolume = musicSource.volume;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            musicSource.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }

        musicSource.Stop();
        musicSource.volume = startVolume; // restore for next track
    }


    // ---------------------- sound effects ----------------------

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



    // ---------------------- helper methods ----------------------

    // helper methods for playing background music
    // Example: AudioManager.instance?.PlayHospitalLobbyMusic();
    public void PlayMenuScreenMusic()        => PlayMusic(menuScreenMusic);
    public void PlayHospitalLobbyMusic()     => PlayMusic(hospitalLobbyMusic);
    public void PlayObstacleGameplayMusic()  => PlayMusic(obstacleGameplayMusic);
    public void PlayZombieBossBattleMusic()        => PlayMusic(zombieBossBattleMusic);

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
    public void ZombieDeath()     => PlaySFX(zombieDeathClip);
}
