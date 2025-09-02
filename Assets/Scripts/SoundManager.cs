using DG.Tweening;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] private AudioSource musicSource, effectsSource, loopSource;

    public AudioClip buttonClick,
        levelFail,
        levelComplete,
        taskComplete,
        ballLaunch,
        timeOut,
        dogBark,
        eat,
        interact,
        bubblePop,
        dogPawPrint,
        dropBucket,
        spillWater,
        lightSwitch,
        throwBlanket,
        pour,
        fireAlarm,
        spillTrash,
        throwItem,
        toppleDishes,
        lick,
        hens,
        goat,
        cageOpen,
        catMeow,
        balloonPop,
        doorOpen,
        doorClose,
        drawerOpen,
        drawerClose,
        chime,
        electrocute,
        partyPop;

    private void Awake()
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

    public void PlaySound(AudioClip clip)
    {
        if (clip != null)
            effectsSource.PlayOneShot(clip);
    }

    public void PlaySoundDelayed(AudioClip clip, float delay)
    {
        DOVirtual.DelayedCall(1, () => { effectsSource.PlayOneShot(clip); });
    }


    public void SoundOff()
    {
        musicSource.mute = true;
        effectsSource.mute = true;
        loopSource.mute = true;
    }

    public void SoundOn()
    {
        musicSource.mute = false;
        effectsSource.mute = false;
        loopSource.mute = false;
    }
}