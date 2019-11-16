using System;
using Managers;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace PUNLobby
{
	public class LoginPanel : MonoBehaviour
	{
		[SerializeField]
		private TMP_InputField nameInputField;

		private const string LastLogin = "/last_login.txt";
		
		private void OnEnable()
		{
			var lastLoginName = SerializeUtility.LoadContentOrDefault(Application.persistentDataPath + LastLogin, "");
			nameInputField.text = lastLoginName.Trim();
			SoundManager.Instance.PlayBgm(BgmId.Login);
		}

		public void Login()
		{
			SoundManager.Instance.PlaySe(SeId.Tick);
			var launcher = Launcher.Instance;
			var playerName = nameInputField.text.Trim();
			if (string.IsNullOrEmpty(playerName))
			{
				launcher.PanelManager.warningPanel.Show(400, 200, "Please input a player name.");
				return;
			}

			launcher.Connect(playerName);
			SerializeUtility.SaveContent(Application.persistentDataPath + LastLogin, playerName);
			launcher.PanelManager.infoPanel.Show(400, 200, "Connecting...");
		}

		public void ExitGame()
		{
			Debug.Log("Quit game...");
			Application.Quit();
		}
	}
}