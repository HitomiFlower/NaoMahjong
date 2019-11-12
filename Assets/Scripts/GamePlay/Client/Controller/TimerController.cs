using System.Collections;
using System.Collections.Generic;
using MEC;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace GamePlay.Client.Controller
{
	public class TimerController : MonoBehaviour
	{
		public Image PlusImage;
		public NumberPanelController BaseTimeController;
		public NumberPanelController BonusTimeController;

		private CoroutineHandle _currentTimerCoroutine;
		private int _mBaseTime;
		private int _mBonusTime;

		public bool IsCountingDown => Timing.IsRunning(_currentTimerCoroutine);

		/// <summary>
		/// Starts count down with the given time, invoke callback when time expires.
		/// </summary>
		/// <param name="baseTime"></param>
		/// <param name="bonusTime"></param>
		/// <param name="callback"></param>
		public void StartCountDown(int baseTime, int bonusTime, UnityAction callback)
		{
			Timing.KillCoroutines(_currentTimerCoroutine);

			gameObject.SetActive(true);
			_mBaseTime = baseTime;
			_mBonusTime = bonusTime;
			SetTime(_mBaseTime, _mBonusTime);
			_currentTimerCoroutine = Timing.RunCoroutine(CountDown(callback));
		}

		/// <summary>
		/// Stops the count down immediately, return the bonus turn time left.
		/// </summary>
		/// <returns>The bonus time left</returns>
		public int StopCountDown()
		{
			Timing.KillCoroutines(_currentTimerCoroutine);

			gameObject.SetActive(false);
			return _mBonusTime;
		}

		private IEnumerator<float> CountDown(UnityAction callback)
		{
			for (; _mBaseTime > 0; _mBaseTime--)
			{
				SetTime(_mBaseTime, _mBonusTime);
				yield return Timing.WaitForSeconds(1f);
			}

			if (_mBonusTime > 0)
				for (; _mBonusTime >= 0; _mBonusTime--)
				{
					SetTime(_mBaseTime, _mBonusTime);
					yield return Timing.WaitForSeconds(1f);
				}

			callback.Invoke();
			gameObject.SetActive(false);
		}

		private void SetTime(int baseTime, int bonusTime)
		{
			if (baseTime < 0) baseTime = 0;

			if (bonusTime < 0) bonusTime = 0;

			if (baseTime == 0)
			{
				BaseTimeController.gameObject.SetActive(false);
				PlusImage.gameObject.SetActive(false);
			}
			else
			{
				BaseTimeController.gameObject.SetActive(true);
				PlusImage.gameObject.SetActive(true);
				BaseTimeController.SetNumber(baseTime);
			}

			if (bonusTime == 0)
			{
				PlusImage.gameObject.SetActive(false);
				BonusTimeController.gameObject.SetActive(false);
				return;
			}

			BonusTimeController.SetNumber(bonusTime);
		}
	}
}