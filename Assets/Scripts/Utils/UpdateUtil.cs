using System.Collections.Generic;
using MEC;
using UnityEngine;

public static class UpdateUtil
{
	/// <summary>
	/// Make the Update method more performance
	/// </summary>
	public static IEnumerator<float> EmulateUpdate(System.Action func, MonoBehaviour scr)
	{
		yield return Timing.WaitForOneFrame;
		while (scr.gameObject != null)
		{
			if (scr.gameObject.activeInHierarchy && scr.enabled)
			{
				func();
			}

			yield return Timing.WaitForOneFrame;
		}
	}
}