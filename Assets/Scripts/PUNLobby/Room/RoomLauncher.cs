using System.Collections;
using System.Collections.Generic;
using Managers;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using Utils;
using MEC;

namespace PUNLobby.Room
{
	public class RoomLauncher : MonoBehaviourPunCallbacks
	{
		public static RoomLauncher Instance
		{
			get;
			private set;
		}

		public SceneField lobbyScene;
		public SceneField mahjongScene;
		public RoomPanelManager roomPanelManager;

		public override void OnEnable()
		{
			base.OnEnable();
			Instance = this;
		}

		private void Start()
		{
			//In case we started this demo with the wrong scene being active, simply load the menu scene
			if (PhotonNetwork.CurrentRoom == null)
			{
				Debug.Log("Is not in the room, returning back to Lobby");
				UnityEngine.SceneManagement.SceneManager.LoadScene(lobbyScene);
				return;
			}

			// We're in a room, set its title
			roomPanelManager.SetTitle(PhotonNetwork.CurrentRoom.Name);
			roomPanelManager.SetPlayers(PhotonNetwork.PlayerList);
		}

		public override void OnLeftRoom()
		{
			Debug.Log("Back to the lobby");
			//We have left the Room, return back to the GameLobby
			Timing.RunCoroutine(CoLeftRoom());
		}

		public override void OnMasterClientSwitched(Player newMasterClient)
		{
			roomPanelManager.SetPlayers(PhotonNetwork.PlayerList);
			roomPanelManager.CheckButtonForMaster();
		}

		public override void OnPlayerEnteredRoom(Player newPlayer)
		{
			roomPanelManager.SetPlayers(PhotonNetwork.PlayerList);
		}

		public override void OnPlayerLeftRoom(Player otherPlayer)
		{
			roomPanelManager.SetPlayers(PhotonNetwork.PlayerList);
		}

		public void GameStart()
		{
			Timing.RunCoroutine(GameStartCoroutine());
		}

		private IEnumerator<float> GameStartCoroutine()
		{
			photonView.RPC("RpcGameStart", RpcTarget.All, new object[0]);
			yield return Timing.WaitForSeconds(1.25f);
			PhotonNetwork.LoadLevel(mahjongScene);
		}

		private IEnumerator<float> CoLeftRoom()
		{
			var transition = FindObjectOfType<SceneTransitionManager>();
			transition.FadeOut();
			yield return Timing.WaitForSeconds(1f);
			UnityEngine.SceneManagement.SceneManager.LoadScene(lobbyScene);
		}

		[PunRPC]
		public void RpcGameStart()
		{
			var transition = FindObjectOfType<SceneTransitionManager>();
			transition.FadeOut();
		}
	}
}