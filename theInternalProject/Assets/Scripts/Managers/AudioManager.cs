using System.Collections;
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

    public AudioClip zombieBossBattleMusic;
    public AudioClip werewolfBossBattleMusic;
    public AudioClip vampireBossBattleMusic;


    public AudioClip goodEndingMusic;    // save all patients
    public AudioClip badEndingMusic;     // save no patients
    public AudioClip partialEndingMusic; // save some patients but not all

    public AudioClip creditsMusic; // credits music


    [Header("Sound Effect Clips")]
    [Header("Menu SFX")]
    public AudioClip buttonClickClip; // start, quit buttons

    [Header("Cut Scene SFX")]
    [Header("Weapon Online SFX")]
    public AudioClip weaponOnlineClip; // gun power-up appears

    [Header("Bloodstream Intro SFX")]
    public AudioClip shrinkPodClip;    // pod shrinking
    public AudioClip closeDoorClip;    // close pod door
    public AudioClip walkingClip;      // doctor walking
    public AudioClip jumpClip;         // jump to pod clip
    public AudioClip podFlyingOffClip;         // pod flies off



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
    public AudioClip zombieRoarClip; // phase 2
    public AudioClip zombieDeathClip;
 
    [Header("Werewolf Enemy SFX")]
    public AudioClip werewolfHowlingClip; // phase 2
    public AudioClip werewolfGrowlClip;
    public AudioClip werewolfBarkClip;
    public AudioClip werewolfChompClip;

    [Header("Vampire Enemy SFX")]
    public AudioClip vampireScreamClip;
    public AudioClip vampireGrowlClip;



    [Header("Volumes")]
    [Header("Music Volumes")]
    [Range(0f, 1f)] public float obstacleMusicVol = 0.7f;
    [Range(0f, 1f)] public float vampireMusicVol = 0.7f;
    [Header("SFX Volumes")]
    [Range(0f, 1f)] public float uiVol = 0.8f;


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
    public void PlayMusic(AudioClip clip, float volume = 1f, bool loop = true)
    {
        if (musicSource == null || clip == null) return;

        // if already playing the same clip, do nothing
        if (musicSource.isPlaying && musicSource.clip == clip) return;

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.volume = Mathf.Clamp01(volume);
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

    void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (sfxSource == null || clip == null) return;
        if (!sfxSource.isActiveAndEnabled) return; // prevents errors if AudioSource is disabled
        
        sfxSource.PlayOneShot(clip, Mathf.Clamp01(volume));
    }

    public void StopSFX()
    {
        if (sfxSource != null)
        {
            sfxSource.Stop();
        }
    }

    public void FadeOutSFX(float duration = 0.5f)
    {
        if (sfxSource != null)
            StartCoroutine(FadeOutSFXRoutine(duration));
    }

    private IEnumerator FadeOutSFXRoutine(float duration)
    {
        if (sfxSource == null || !sfxSource.isPlaying)
            yield break;

        float startVolume = sfxSource.volume;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            sfxSource.volume = Mathf.Lerp(startVolume, 0f, t / duration);
            yield return null;
        }

        sfxSource.Stop();
        sfxSource.volume = startVolume; // restore for next SFX
    }
    

    // ---------------------- helper methods ----------------------

    // helper methods for playing background music
    // Example: AudioManager.instance?.PlayHospitalLobbyMusic();
    public void PlayMenuScreenMusic()        => PlayMusic(menuScreenMusic);
    public void PlayHospitalLobbyMusic()     => PlayMusic(hospitalLobbyMusic);
    public void PlayObstacleGameplayMusic()  => PlayMusic(obstacleGameplayMusic, obstacleMusicVol);

    public void PlayZombieBossBattleMusic()        => PlayMusic(zombieBossBattleMusic);
    public void PlayWerewolfBossBattleMusic()      => PlayMusic(werewolfBossBattleMusic);
    public void PlayVampireBossBattleMusic()       => PlayMusic(vampireBossBattleMusic, vampireMusicVol);

    public void PlayGoodEndingMusic()        => PlayMusic(goodEndingMusic);
    public void PlayBadEndingMusic()         => PlayMusic(badEndingMusic);
    public void PlayPartialEndingMusic()     => PlayMusic(partialEndingMusic);

    public void PlayCredits()                => PlayMusic(creditsMusic);


    // helper methods for SFX
    // Example: AudioManager.instance?.ButtonClick();
    public void ButtonClick()        => PlaySFX(buttonClickClip);

    public void WeaponOnline()       => PlaySFX(weaponOnlineClip);

    public void ShrinkPod()          => PlaySFX(shrinkPodClip);
    public void CloseDoor()          => PlaySFX(closeDoorClip);
    public void Walking()            => PlaySFX(walkingClip);
    public void JumpToPod()          => PlaySFX(jumpClip);
    public void PodFlyingOff()       => PlaySFX(podFlyingOffClip);
    
    public void DamageTaken()        => PlaySFX(damageTakenClip);
    public void Death()              => PlaySFX(dyingClip);
    public void Attack()             => PlaySFX(attackClip);

    public void ShootPatient()       => PlaySFX(shootPatientClip);
    public void HeavyBreathing()     => PlaySFX(heavyBreathingClip);
    public void DeepExhale()         => PlaySFX(deepExhaleClip);

    public void ZombieChomp()        => PlaySFX(zombieChompClip);
    public void ZombieRoar()         => PlaySFX(zombieRoarClip);
    public void ZombieDeath()        => PlaySFX(zombieDeathClip);

    public void WerewolfHowling()    => PlaySFX(werewolfHowlingClip);
    public void WerewolfGrowl()      => PlaySFX(werewolfGrowlClip);
    public void WerewolfBark()       => PlaySFX(werewolfBarkClip);
    public void WerewolfChomp()      => PlaySFX(werewolfChompClip);

    public void VampireScream()      => PlaySFX(vampireScreamClip);
    public void VampireGrowl()       => PlaySFX(vampireGrowlClip);
}