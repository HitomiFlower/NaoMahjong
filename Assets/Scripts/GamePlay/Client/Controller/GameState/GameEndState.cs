using System.Collections;
using System.Collections.Generic;
using Managers;
using MEC;
using UnityEngine;

namespace GamePlay.Client.Controller.GameState
{
	public class GameEndState : ClientState
	{
		public string[] PlayerNames;
		public int[] Points;
		public int[] Places;

		public override void OnClientStateEnter()
		{
			controller.GameEndPanelManager.SetPoints(PlayerNames, Points, Places, () =>
			{
				Timing.RunCoroutine(BackToLobby());
				// todo -- record points (maybe)?
			});
		}

		private IEnumerator<float> BackToLobby()
		{
			Debug.Log("Back to lobby");
			var transition = GameObject.FindObjectOfType<SceneTransitionManager>();
			transition.FadeOut();
			yield return Timing.WaitForOneFrame;
			// todo -- button for "back to lobby" or "back to room"
		}

		public override void OnClientStateExit()
		{
		}

		public override void OnStateUpdate()
		{
		}
	}
}