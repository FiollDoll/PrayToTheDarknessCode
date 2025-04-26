using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }
    [SerializeField] private AudioSource speechSource;

    private void Awake() => Instance = this;

    public void PlaySpeech(AudioClip clip)
    {
        speechSource.clip = clip;
        speechSource.Play();
    }

    public void StopSpeech() => speechSource.Stop();
}
