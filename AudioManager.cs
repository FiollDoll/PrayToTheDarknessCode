using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    [SerializeField] private AudioSource speechSource;
    [SerializeField] private AudioSource musicSource;

    private void Awake() => Instance = this;

    public void PlayMusic(AudioClip music)
    {
        if (!music) return;
        musicSource.clip = music;
        musicSource.Play();
    }
    
    public void PlaySpeech(AudioClip clip)
    {
        speechSource.clip = clip;
        speechSource.Play();
    }

    public void StopSpeech() => speechSource.Stop();
}
