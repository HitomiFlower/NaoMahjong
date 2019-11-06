using Mahjong.Model;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PUNLobby
{
	public class RoomEntry : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI _roomNameText;

		[SerializeField]
		private RectTransform _qtjStatus;

		[SerializeField]
		private TextMeshProUGUI _playerStatusText;

		[SerializeField]
		private Button _checkRuleButton;

		[SerializeField]
		private Button _joinButton;

		public void SetRoom(RoomInfo info)
		{
			var setting = (GameSetting)info.CustomProperties[SettingKeys.SETTING];
			_roomNameText.text = info.Name;
			var isQTJ = setting != null && setting.GameMode == GameMode.QTJ;
			_qtjStatus.gameObject.SetActive(isQTJ);
			_playerStatusText.text = $"{info.PlayerCount}/{info.MaxPlayers}";
			_checkRuleButton.onClick.RemoveAllListeners();
			_checkRuleButton.onClick.AddListener(() => CheckRules(setting));
			_joinButton.onClick.RemoveAllListeners();
			_joinButton.onClick.AddListener(() => { Launcher.Instance.JoinRoom(info.Name); });
		}

		private void CheckRules(GameSetting setting)
		{
			Launcher.Instance.ShowRulePanel(setting);
		}
	}
}