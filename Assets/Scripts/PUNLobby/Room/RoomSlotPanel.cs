using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PUNLobby.Room
{
	public class RoomSlotPanel : MonoBehaviour
	{
		[SerializeField]
		private Image _roomMaster;

		[SerializeField]
		private Image _readySign;

		[SerializeField]
		private TextMeshProUGUI _playerNameText;

		[SerializeField]
		private Image _charaImage;

		private bool _imageInitialized;
		private const string CharaImagePath = "Sprites/Chara/";

		public void Set(bool isMaster, string playerName, bool isReady, string charaName = "Haruna")
		{
			_roomMaster.gameObject.SetActive(isMaster);
			_readySign.gameObject.SetActive(isMaster || isReady);
			_playerNameText.text = playerName;

			if (_imageInitialized)
			{
				return;
			}

			var sprite = Resources.Load(CharaImagePath + charaName.ToLower(), typeof(Sprite)) as Sprite;
			if (sprite != null)
			{
				_charaImage.overrideSprite = sprite;
			}

			_imageInitialized = true;
		}
	}
}