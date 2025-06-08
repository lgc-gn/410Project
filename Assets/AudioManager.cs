using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;

    private AudioSource audioSource;

    void Awake()
    {
        // Singleton pattern
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        audioSource = GetComponent<AudioSource>();
    }

    public void PlayMusic(AudioClip musicClip)
    {
        if (musicClip == null) return;

        if (audioSource.clip != musicClip)
        {
            audioSource.clip = musicClip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }

    public void StopMusic()
    {
        audioSource.Stop();
    }

    public void SetVolume(float volume)
    {
        audioSource.volume = Mathf.Clamp01(volume);
    }

    public AudioClip battleTheme;

    void Start()
    {
        PlayMusic(battleTheme);
    }

}
