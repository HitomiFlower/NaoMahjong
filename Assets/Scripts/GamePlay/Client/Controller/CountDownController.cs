using System.Collections;
using System.Collections.Generic;
using MEC;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GamePlay.Client.Controller
{
	public class CountDownController : MonoBehaviour
	{
		public NumberPanelController NumberController;
		private CoroutineHandle _currentTimerHandle;

		public bool IsCountingDown => Timing.IsRunning(_currentTimerHandle);

		private int mTimeLeft;

		public void StartCountDown(int countDown, UnityAction callback)
		{
			Timing.KillCoroutines(_currentTimerHandle);

			gameObject.SetActive(true);
			mTimeLeft = countDown;
			// SetTime(countDown);
			_currentTimerHandle = Timing.RunCoroutine(CountDown(callback));
		}

		public int StopCountDown()
		{
			Timing.KillCoroutines(_currentTimerHandle);

			NumberController.Close();
			return mTimeLeft;
		}

		private IEnumerator<float> CountDown(UnityAction callback)
		{
			for (; mTimeLeft > 0; mTimeLeft--)
			{
				SetTime(mTimeLeft);
				yield return Timing.WaitForSeconds(1f);
			}

			callback.Invoke();
			gameObject.SetActive(false);
		}

		private void SetTime(int timeLeft)
		{
			if (timeLeft == 0)
			{
				NumberController.gameObject.SetActive(false);
				return;
			}

			NumberController.SetNumber(timeLeft);
		}
	}
}