using UnityEngine;
using Scythe.Accessibility;
using Scythe.Accessibility.Data;

public class DemoScript : MonoBehaviour
{
    [SerializeField] SubtitleCard subtitleCard;
    void Start()
    {
        SubtitleManager.instance.CueSubtitle(subtitleCard);
    }

}
