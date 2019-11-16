using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MEC;
using MovementEffects;
using Object = UnityEngine.Object;

namespace Utils
{
	public static class UnityExtension
	{
		public static void DestroyAllChildren(this Transform transform)
		{
			for (int i = transform.childCount - 1; i >= 0; i--)
			{
				Object.Destroy(transform.GetChild(i).gameObject);
			}
		}

		public static void TraversalChildren(this Transform transform, UnityAction<Transform> action)
		{
			for (int i = 0; i < transform.childCount; i++)
			{
				action.Invoke(transform.GetChild(i));
			}
		}
		
		#region AudioSource extension
		/// <summary>
		/// A sugar for audioSource play by check audioSource and volume state
		/// </summary>
		public static bool Play(this AudioSource audioSource, AudioClip audioClip = null, float volume = 1f)
		{
			if (audioSource == null || audioClip == null || volume < 0f)
			{
				return false;
			}

			audioSource.clip = audioClip;
			audioSource.volume = volume;
			audioSource.Play();

			return true;
		}

		public static IEnumerator<float> PlayWithFadeIn(this AudioSource audioSource,
			AudioClip audioClip,
			float targetVolume = 1f,
			float duration = 0.1f,
			UnityAction compCallback = null)
		{
			if (targetVolume <= 0f || !audioSource.Play(audioClip, 1f))
			{
				yield break;
			}

			var fadeIn = new Effect<AudioSource, float>
			{
				Duration = duration,
				RetrieveStart = (source, lastEnd) => source.volume,
				RetrieveEnd = x => targetVolume,
				OnUpdate = OnUpdateVolume,
				OnDone = source => compCallback?.Invoke() 
			};

			Movement.Run(audioSource, fadeIn);
		}

		public static IEnumerator<float> StopWithFadeOut(this AudioSource audioSource, float duration = 0.1f, UnityAction compCallback = null)
		{
			if (!audioSource.isPlaying)
			{
				yield break;
			}

			if (duration <= 0f)
			{
				audioSource.volume = 0f;
				audioSource.Stop();
				yield break;
			}

			var fadeOut = new Effect<AudioSource, float>
			{
				Duration = duration,
				RetrieveEnd = x => 0f,
				OnUpdate = OnUpdateVolume
			};

			var sequence = new Sequence<AudioSource, float>()
			{
				RetrieveSequenceStart = source => source.volume,
				Reference = audioSource,
				OnComplete = source =>
				{
					source.Stop();
					compCallback?.Invoke();
				}
			};
			
			sequence.Add(fadeOut);

			Movement.Run(sequence);
		}

		public static IEnumerator<float> PlayWithCompCallBack(this AudioSource audioSource, AudioClip audioClip,
			float volume = 1f, UnityAction compCallback = null)
		{
			if (!audioSource.Play(audioClip, volume))
			{
				yield break;
			}

			float timer = 0f;
			while (timer < audioClip.length)
			{
				timer += Timing.DeltaTime;
				yield return Timing.WaitForOneFrame;
			}
			
			compCallback?.Invoke();
		}

		private static void OnUpdateVolume(AudioSource source, float value)
		{
			source.volume = Mathf.Clamp01(value);
		}
		
		public static bool IsPlayingClip(this AudioSource source, string clipName)
		{
			return source.IsCurrentClip(clipName) && source.isPlaying;
		}

		public static bool IsCurrentClip(this AudioSource source, string clipName)
		{
			return source.clip != null && source.clip.name == clipName;
		}
		#endregion
	}
}