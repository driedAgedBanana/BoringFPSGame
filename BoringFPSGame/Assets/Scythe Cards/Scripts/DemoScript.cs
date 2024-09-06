using UnityEngine;
using Scythe.Accessibility;
using Scythe.Accessibility.Data;

public class DemoScript : MonoBehaviour
{
    [SerializeField] SubtitleCard subtitleCard;
    [SerializeField] AudioClip[] audioClips;

    public bool isPlayerInTriggerZone = false;

    void Update()
    {
        if (isPlayerInTriggerZone)
        {
            SubtitleManager.instance.CueSubtitle(subtitleCard, audioClips);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTriggerZone = true;
            Debug.Log("Player entered trigger zone");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTriggerZone = false;
            Debug.Log("Player exited trigger zone");
        }
    }

}
