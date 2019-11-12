using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Utils;
using MEC;
using UnityEngine.Assertions;

namespace Managers
{
    public class SoundManager : Singleton<SoundManager>
    {
        [SerializeField]
        private List<AudioSource> _audioSourceBgmList;
        
        [SerializeField]
        private AudioSource _audioSourceSe;
        
        public bool isPaused
        {
            get;
            private set;
        }

        public void PlayBgm(string clipName, UnityAction compCallback = null)
        {
            if (isPaused)
            {
                return;
            }

            var clip = Resources.Load<AudioClip>(@"AudioClips\BGM\" + clipName);
            if (clip != null)
            {
                var emptySource = _audioSourceBgmList.Find(s => s.isPlaying == false);

                var busySource = _audioSourceBgmList.Find(s => s.isPlaying == true);

                if (busySource != null)
                {
                    Timing.RunCoroutine(busySource.StopWithFadeOut(1.5f));
                }

                Timing.RunCoroutine(emptySource.PlayWithFadeIn(clip, 1.5f));
            }
        }

        public void Pause()
        {
            isPaused = true;
            
            _audioSourceSe.Pause();
        }
    }
}
