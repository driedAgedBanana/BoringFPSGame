using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RadioManager : MonoBehaviour
{
    public AudioSource audioSource; // The AudioSource component to play the music
    public AudioClip[] musicClips;  // Array to hold the music clips
    private bool isPlaying = false; // State to check if music is playing

    public float interactionDistance = 3f;

    [SerializeField] private bool isRadionOn = false;

    void Update()
    {
        // Check if the player presses the "G" key
        if (Input.GetKeyDown(KeyCode.G))
        {
            if (isPlaying)
            {
                isRadionOn = false;
                StopMusic();
            }
            else
            {
                Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, interactionDistance))
                {
                    // Check if the object hit by the raycast is this chest
                    if (hit.collider.gameObject == gameObject)
                    {
                        PlayRandomMusic();
                        isRadionOn = true;
                    }
                }
            }
        }
    }

    void PlayRandomMusic()
    {
        if (musicClips.Length == 0) return; // Check if the list is empty

        // Pick a random index from the list
        int randomIndex = Random.Range(0, musicClips.Length);
        audioSource.clip = musicClips[randomIndex];
        audioSource.Play();
        isPlaying = true;
    }

    public void StopMusic()
    {
        audioSource.Stop();
        isPlaying = false;
    }
}
