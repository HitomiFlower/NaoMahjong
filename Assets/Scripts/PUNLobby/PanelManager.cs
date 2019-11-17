﻿using System.Collections.Generic;
using Managers;
using Photon.Realtime;
using UnityEngine;

namespace PUNLobby
{
	public class PanelManager : MonoBehaviour
	{
		public RectTransform LoginPanel;
		public RectTransform LobbyPanel;

		[SerializeField]
		private RoomListPanel roomListPanel;

		[SerializeField]
		private CreateRoomPanel createPanel;

		public WarningPanel warningPanel;
		public WarningPanel infoPanel;
		private RectTransform currentPanel;

		public void Awake()
		{
			LoginPanel.gameObject.SetActive(true);
			LobbyPanel.gameObject.SetActive(false);
			warningPanel.gameObject.SetActive(false);
			infoPanel.gameObject.SetActive(false);
		}

		public void ChangeTo(RectTransform newPanel, BgmId bgmId = BgmId.Same)
		{
			if (currentPanel != null)
			{
				currentPanel.gameObject.SetActive(false);
			}

			if (newPanel != null)
			{
				newPanel.gameObject.SetActive(true);
			}

			currentPanel = newPanel;

			if (bgmId != BgmId.Same)
			{
				AudioManager.Instance.PlayBgm(bgmId);
			}
		}

		public void ShowCreateRoomPanel()
		{
			createPanel.gameObject.SetActive(true);
		}

		public void SetRoomList(IList<RoomInfo> rooms)
		{
			roomListPanel.SetRoomList(rooms);
		}
	}
}