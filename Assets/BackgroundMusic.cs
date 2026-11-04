using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    public static BackgroundMusic instance;
    private AudioSource audioSource;
    public AudioClip defaultMusic;
    public AudioClip winMusic;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            audioSource = GetComponent<AudioSource>();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public void PlayWinMusic()
    {
        if (audioSource == null) return;

        audioSource.Stop();

        if (winMusic != null)
        {
            audioSource.clip = winMusic;
            audioSource.loop = false;
            audioSource.Play();
        }
    }

    public void PlayDefaultMusic()
    {
        if (audioSource == null) return;

        if (audioSource.clip != defaultMusic)
        {
            audioSource.Stop();
            audioSource.clip = defaultMusic;
            audioSource.loop = true;
            audioSource.Play();
        }
        else
        {
            audioSource.UnPause();
            if (!audioSource.isPlaying) audioSource.Play();
        }
    }
}