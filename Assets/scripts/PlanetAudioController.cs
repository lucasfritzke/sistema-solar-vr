using UnityEngine;
using System;

public class PlanetAudioController : MonoBehaviour
{
    public AudioSource audioSource;
    public event Action OnAudioFinished;

    void Start()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    public void PlayAudio()
    {
        if (audioSource != null)
        {
            audioSource.Play();
            Invoke(nameof(NotifyAudioFinished), audioSource.clip.length);
        }
    }

    private void NotifyAudioFinished()
    {
        OnAudioFinished?.Invoke();
    }
}
