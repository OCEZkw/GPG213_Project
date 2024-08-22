using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum InteractionType
{
    LoadScene,
    OpenPanel,
    PlayAudio
}

public class InteractableObject : MonoBehaviour
{
    public InteractionType interactionType;
    public string sceneToLoad;
    public GameObject panelToOpen;
    public AudioClip interactionSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;
        audioSource.clip = interactionSound;
    }

    public void PlayInteractionSound()
    {
        if (audioSource != null && interactionSound != null)
        {
            // Create a new GameObject to play the audio
            GameObject audioPlayer = new GameObject("AudioPlayer");
            AudioSource newAudioSource = audioPlayer.AddComponent<AudioSource>();
            newAudioSource.clip = interactionSound;
            newAudioSource.Play();

            // Don't destroy the audio player when loading a new scene
            DontDestroyOnLoad(audioPlayer);

            // Destroy the audio player after the clip has finished playing
            Destroy(audioPlayer, interactionSound.length);
        }
    }
}