using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Utils;
using MEC;
using UnityEngine.Assertions;

public enum BgmId
{
    None = -1,
    Same = 0,
    Login = 1,
    Lobby = 2,
    GameCommon = 3,
}

public enum SeId
{
    None = 0,
    Tick = 1,
    Dice = 2,
    Rong = 3
}

namespace Managers
{
    public class SoundManager : Singleton<SoundManager>
    {
        [SerializeField]
        private List<AudioSource> _BgmSourceList;
        
        [SerializeField]
        private AudioSource _primarySeSource;

        [SerializeField]
        private AudioSource _loopSeSource;

        private const string BgmPath = @"AudioClips\BGM\";
        private const string SePath = @"AudioClips\SE\";

        private const string BgmSourceName = "bgm_{0:000}";
        private const string SeSourceName = "se_{0}";

        private new void Awake()
        {
            base.Awake();
            foreach (var source in _BgmSourceList)
            {
                source.loop = true;
                source.Stop();
            }

            _primarySeSource.loop = false;
            _loopSeSource.loop = true;
            DontDestroyOnLoad(this);
        }

        public bool isPaused
        {
            get;
            private set;
        }

        public void PlayBgm(BgmId bgmId, UnityAction compCallback = null)
        {
            if (bgmId == BgmId.None || bgmId == BgmId.Same)
            {
                compCallback?.Invoke();
            }
            
            PlayBgmInternal(GetClipNameForBgm(bgmId), compCallback);
        }

        private void PlayBgmInternal(string clipName, UnityAction compCallback = null)
        {
            if (isPaused)
            {
                return;
            }
            
            var busySource = _BgmSourceList.Find(s => s.isPlaying);
            if (busySource != null && busySource.IsPlayingClip(clipName))
            {
                return;
            }

            var clip = Resources.Load<AudioClip>(BgmPath + clipName);
            if (clip != null)
            {
                var emptySource = _BgmSourceList.Find(s => s.isPlaying == false);

                if (busySource != null)
                {
                    Timing.RunCoroutine(busySource.StopWithFadeOut(1.5f));
                }

                Timing.RunCoroutine(emptySource.PlayWithFadeIn(clip, 1f, 1.5f, compCallback));
            }
        }

        public void PlaySe(SeId seId, UnityAction compCallback = null)
        {
            if (seId == SeId.None)
            {
                compCallback?.Invoke();
            }
            
            PlaySeInternal(GetClipNameForSe(seId), false, compCallback);
        }
        
        public void PlayLoopSe(SeId seId, UnityAction compCallback = null)
        {
            if (seId == SeId.None)
            {
                compCallback?.Invoke();
            }
            
            PlaySeInternal(GetClipNameForSe(seId), true, compCallback);
        }

        private void PlaySeInternal(string clipName, bool isLoopSe, UnityAction compCallback)
        {
            if (isPaused)
            {
                return;
            }

            AudioSource source = isLoopSe ? _loopSeSource : _primarySeSource;
            
            Assert.IsNotNull(source);
            if (source.IsPlayingClip(clipName))
            {
                return;
            }

            var clip = Resources.Load<AudioClip>(SePath + clipName);
            if (clip != null)
            {
                if (isLoopSe)
                {
                    source.Play(clip, 1f);
                }
                else
                {
                    Timing.RunCoroutine(source.PlayWithCompCallBack(clip, 1f, compCallback));
                }
            }
        }

        public void Pause()
        {
            isPaused = true;
            
            _primarySeSource.Pause();
        }

        private string GetClipNameForBgm(BgmId bgmId)
        {
            return string.Format(BgmSourceName, (int)bgmId);
        }

        private string GetClipNameForSe(SeId seId)
        {
            return string.Format(SeSourceName, seId);
        }
    }
}
