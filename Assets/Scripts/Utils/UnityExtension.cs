using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using MEC;
using MovementEffects;

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
			if (audioClip == null || volume < 0f)
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
			float duration = 0.1f)
		{
			if (targetVolume <= 0f || !audioSource.Play(audioClip, 0f))
			{
				yield break;
			}

			var fadeIn = new Effect<AudioSource, float>
			{
				Duration = duration,
				RetrieveStart = (source, lastEnd) => source.volume,
				RetrieveEnd = x => targetVolume,
				OnUpdate = (source, value) => source.volume = value
			};

			Movement.Run(audioSource, fadeIn);

//			while (audioSource.volume < targetVolume)
//			{
//				float tempVolume = audioSource.volume + (Timing.DeltaTime / duration);
//
//				audioSource.volume = Mathf.Min(tempVolume, targetVolume);
//
//				yield return Timing.WaitForOneFrame;
//			}
		}

		public static IEnumerator<float> StopWithFadeOut(this AudioSource audioSource, float duration = 0.1f)
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
				OnUpdate = (source, value) => source.volume = value
			};

			Movement.Run(audioSource, fadeOut, source => source.volume);
			
//			var sequence = new Sequence<AudioSource, float>();
//			sequence.Reference = audioSource;
//			sequence.Add(fadeOut);
//
//			while (audioSource.volume > 0f)
//			{
//				audioSource.volume -= Timing.DeltaTime / duration;
//				yield return Timing.WaitForOneFrame;
//			}
			
			audioSource.Stop();
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
		#endregion
	}
}