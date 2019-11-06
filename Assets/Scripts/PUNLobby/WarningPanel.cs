using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PUNLobby
{
	public class WarningPanel : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI _title;

		[SerializeField]
		private RectTransform _window;

		[SerializeField]
		private TextMeshProUGUI _text;

		public void Show(int width, int height, string titleString, string content)
		{
			_title.text = titleString;
			_window.sizeDelta = new Vector2(width, height);
			_text.text = content;
			gameObject.SetActive(true);
		}

		public void Show(int width, int height, string content)
		{
			Show(width, height, "", content);
		}

		public void Close()
		{
			gameObject.SetActive(false);
		}
	}
}