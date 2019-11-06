using Photon.Pun;
using TMPro;
using UnityEngine;

namespace PUNLobby
{
	[RequireComponent(typeof(TextMeshProUGUI))]
	public class PlayerNameDisplayer : MonoBehaviour
	{
		private TextMeshProUGUI _text;

		private void Start()
		{
			_text = GetComponent<TextMeshProUGUI>();
		}

		private void Update()
		{
			_text.text = PhotonNetwork.NickName;
		}
	}
}