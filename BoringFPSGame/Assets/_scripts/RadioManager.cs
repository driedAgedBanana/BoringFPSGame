using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioManager : MonoBehaviour
{
    public AudioSource audioSource; // The AudioSource component that will play the music
    public AudioClip[] musicClips;  // Array to store different music clips
    private bool isPlaying = false; // Boolean to track if the radio is playing

    public float interactionDistance = 3f; // The distance within which the player can interact with the radio

    [SerializeField] private bool isRadioOn = false; // Tracks if the radio is currently turned on

    void Update()
    {
        // Check if the player presses the "G" key
        if (Input.GetKeyDown(KeyCode.G))
        {
            // If the radio is already playing, stop the music
            if (isPlaying)
            {
                isRadioOn = false;
                StopMusic();
            }
            else
            {
                // Cast a ray from the player's camera to detect interaction with the radio
                Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
                RaycastHit hit;

                // If the raycast hits something within the interaction distance
                if (Physics.Raycast(ray, out hit, interactionDistance))
                {
                    // Check if the object hit by the raycast is this radio
                    if (hit.collider.gameObject == gameObject)
                    {
                        PlayRandomMusic(); // Play a random music clip
                        isRadioOn = true;  // Set the radio state to "on"
                    }
                }
            }
        }
    }

    // Function to play a random music clip from the array
    void PlayRandomMusic()
    {
        if (musicClips.Length == 0) return; // If there are no music clips, exit the function

        // Pick a random index from the musicClips array
        int randomIndex = Random.Range(0, musicClips.Length);
        audioSource.clip = musicClips[randomIndex]; // Assign the selected clip to the AudioSource
        audioSource.Play(); // Play the selected clip
        isPlaying = true;   // Update the state to show that music is playing
    }

    // Function to stop the music
    public void StopMusic()
    {
        audioSource.Stop(); // Stop the current audio clip
        isPlaying = false;  // Update the state to show that music is not playing
    }
}
