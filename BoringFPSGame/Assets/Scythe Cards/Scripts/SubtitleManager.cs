using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Scythe.Accessibility.Data;
using UnityEngine.Events;

namespace Scythe.Accessibility
{
    public class SubtitleManager : MonoBehaviour
    {
        public static SubtitleManager instance;

        [Tooltip("The text used for the subtitles, can be any text object")]
        [SerializeField] public TextMeshProUGUI subtitleText;

        [Tooltip("If false, does not interrupt the current playing subtitle")]
        public bool subtitlesInterrupt;

        private Coroutine _subControl;
        private bool _subtitleSequenceRunning;

        public UnityEvent BeforeSubtitleBegins;
        public UnityEvent OnSubtitleFinished;

        void Awake()
        {
            instance = this;
        }

        public void CueSubtitle(SubtitleCard subCard, AudioClip[] audioClips)
        {
            if (!subtitlesInterrupt && _subtitleSequenceRunning)
            {
                return;
            }
            else
            {
                if (_subtitleSequenceRunning)
                {
                    StopCoroutine(_subControl);
                }
            }
            _subControl = StartCoroutine(SubtitleControl(subCard, audioClips));
        }

        IEnumerator SubtitleControl(SubtitleCard subCard, AudioClip[] audioClips)
        {
            BeforeSubtitleBegins.Invoke();
            _subtitleSequenceRunning = true;

            for (int i = 0; i < subCard.dialogue.Length; i++)
            {
                subtitleText.text = subCard.dialogue[i];

                // Play the corresponding audio clip for the current dialogue line
                if (i < audioClips.Length)
                {
                    AudioManager.instance.PlayAudioClip(audioClips[i]);
                }

                // Wait for the audio to finish playing
                while (AudioManager.instance.IsAudioPlaying())
                {
                    yield return null;
                }

                yield return new WaitForSeconds(subCard.pauseUntilNextLine[i]);
            }

            subtitleText.text = "";
            _subtitleSequenceRunning = false;
            OnSubtitleFinished.Invoke();
            Debug.Log("Subtitle Sequence finished");
        }
    }
}
