using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Music Clips")]
    public AudioClip[] musicTracks; 

    private AudioSource audioSource;
    public bool isMusicEnabled = true; 

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.Stop();
    }
    public void PlayRandomMusic()
    {
        if (!isMusicEnabled) return;
        if (musicTracks == null || musicTracks.Length == 0) return;

        int randomIndex = Random.Range(0, musicTracks.Length);
        audioSource.clip = musicTracks[randomIndex];
        audioSource.loop = true;
        audioSource.Play();
    }
    
    public void StopMusic()
    {
        audioSource.Stop();
    }

    public void SetMusicEnabled(bool enabled)
    {
        isMusicEnabled = enabled;
        if (!enabled)
        {
            audioSource.Stop();
        }
        else if (isGamePlaying)
        {
            PlayRandomMusic();
        }
    }

    private bool isGamePlaying = false;
    public void SetGameState(bool playing)
    {
        isGamePlaying = playing;
    }
}