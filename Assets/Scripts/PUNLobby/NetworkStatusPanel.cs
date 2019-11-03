using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PUNLobby
{
    public class NetworkStatusPanel : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _statusText;

        private void Update()
        {
            if (_statusText == null) return;
            _statusText.text = PhotonNetwork.NetworkClientState.ToString();
        }
    }
}
