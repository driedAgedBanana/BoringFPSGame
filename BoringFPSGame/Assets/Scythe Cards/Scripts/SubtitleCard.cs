using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Scythe.Accessibility.Data
{
    [CreateAssetMenu(menuName = "Subtitle Card Data")]
    public class SubtitleCard : ScriptableObject
    {
        [SerializeField] [TextArea] public string[] dialogue;
        [SerializeField] public float[] pauseUntilNextLine;
    }
}

// First we create a scriptable object by removing 'MonoBehavior' and adding 'ScriptableObject'
// We also add [CreateAssetMenu()] so we can create the scriptable object.
