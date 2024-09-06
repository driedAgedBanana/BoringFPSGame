using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Tooltip("The audio source for playing the dialogue audio clips.")]
    public AudioSource audioSource;

    // List to store multiple audio clips
    [Tooltip("List of audio clips to be played in sequence.")]
    public List<AudioClip> audioClips;

    private int currentClipIndex = 0;

    void Awake()
    {
        // Set up the singleton instance
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Plays the next audio clip in the list.
    /// </summary>
    public void PlayNextAudioClip()
    {
        if (audioClips == null || audioClips.Count == 0)
        {
            Debug.LogWarning("Audio clip list is empty or null!");
            return;
        }

        if (currentClipIndex < audioClips.Count)
        {
            PlayAudioClip(audioClips[currentClipIndex]);
            currentClipIndex++;
        }
        else
        {
            Debug.Log("All audio clips have been played.");
        }
    }

    /// <summary>
    /// Plays a given audio clip.
    /// </summary>
    /// <param name="clip">The audio clip to play.</param>
    public void PlayAudioClip(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("Audio clip is null! Cannot play audio.");
            return;
        }

        audioSource.clip = clip;
        audioSource.Play();
    }

    /// <summary>
    /// Stops the currently playing audio clip.
    /// </summary>
    public void StopAudio()
    {
        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }

    /// <summary>
    /// Checks if the audio is still playing.
    /// </summary>
    /// <returns>True if audio is playing, otherwise false.</returns>
    public bool IsAudioPlaying()
    {
        return audioSource.isPlaying;
    }

    /// <summary>
    /// Resets the audio clip index to start from the beginning.
    /// </summary>
    public void ResetAudioSequence()
    {
        currentClipIndex = 0;
    }
}
