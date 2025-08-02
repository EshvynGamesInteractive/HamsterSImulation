using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField]private AudioSource musicSource, effectsSource, loopSource;

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
        effectsSource.PlayOneShot(clip);
    }


    public void ToggleSound()
    {
        musicSource.mute = !musicSource.mute;
        effectsSource.mute = !effectsSource.mute;
    }
}