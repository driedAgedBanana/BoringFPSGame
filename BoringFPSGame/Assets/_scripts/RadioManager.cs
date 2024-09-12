using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RadioManager : MonoBehaviour
{
    public AudioSource audioSource; // The AudioSource component that will play the music
    public AudioClip[] musicClips;  // Array to store different music clips
    private bool isPlaying = false; // Boolean to track if the radio is playing

    public float interactionDistance = 3f; // The distance within which the player can interact with the radio
    [SerializeField] private bool isRadioOn = false; // Tracks if the radio is currently turned on

    public TextMeshProUGUI songTitleText; // Reference to the TextMeshPro component for displaying the song name

    private PlayerMovementScript playerMovement; // Reference to the player's movement script

    private void Start()
    {
        // Find the PlayerMovementScript component in the scene (or through other methods if necessary)
        playerMovement = FindObjectOfType<PlayerMovementScript>();
    }

    void Update()
    {
        // Check if the player presses the "G" key
        if (Input.GetKeyDown(KeyCode.V))
        {
            PlayRandomMusic(); // Play a random music clip
            isRadioOn = true;  // Set the radio state to "on"
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            StopMusic();
            isRadioOn = false;
        }
    }

    // Function to play a random music clip from the array
    public void PlayRandomMusic()
    {
        if (musicClips.Length == 0) return; // If there are no music clips, exit the function

        // Pick a random index from the musicClips array
        int randomIndex = Random.Range(0, musicClips.Length);
        audioSource.clip = musicClips[randomIndex]; // Assign the selected clip to the AudioSource
        audioSource.Play(); // Play the selected clip
        isPlaying = true;   // Update the state to show that music is playing

        // Update both the in-game song title display and HUD display
        songTitleText.text = "Now Playing: " + audioSource.clip.name;

        if (playerMovement != null && playerMovement.MusicLists != null)
        {
            playerMovement.MusicLists.text = "Now Playing: " + audioSource.clip.name; // Update the HUD
        }

        Debug.Log($"Now playing: " + audioSource.clip.name);
    }

    // Function to stop the music
    public void StopMusic()
    {
        audioSource.Stop(); // Stop the current audio clip
        isPlaying = false;  // Update the state to show that music is not playing
        songTitleText.text = ""; // Clear the song title when music stops

        // Clear the HUD as well
        if (playerMovement != null && playerMovement.MusicLists != null)
        {
            playerMovement.MusicLists.text = ""; // Clear the HUD display
        }
    }
}
