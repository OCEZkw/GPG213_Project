using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AudioSource))]
public class UISoundEffect : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public AudioClip hoverSound;
    public AudioClip clickSound;

    private static AudioSource audioSource;

    private void Awake()
    {
        // If we don't have an AudioSource yet, create one
        if (audioSource == null)
        {
            // Create a new GameObject to hold our persistent AudioSource
            GameObject audioSourceObj = new GameObject("UI_SoundEffect_AudioSource");
            audioSource = audioSourceObj.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;

            // Make sure it persists across scene loads
            DontDestroyOnLoad(audioSourceObj);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlaySound(hoverSound);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PlaySound(clickSound);
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
