using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using UnityEngine;

namespace PUNLobby
{
	public class RoomListPanel : MonoBehaviour
	{
		public RectTransform contentParent;
		public GameObject roomEntryPrefab;
		private const float height = 60;

		public void SetRoomList(IList<RoomInfo> rooms)
		{
			// Resize the panel height by the room count
			var size = contentParent.sizeDelta;
			contentParent.sizeDelta = new Vector2(size.x, rooms.Count * height);
			
			for (int i = 0; i < rooms.Count; i++)
			{
				RoomEntry entry;
				if (i < contentParent.childCount)
				{
					// Active the room item if has more child
					var t = contentParent.GetChild(i);
					t.gameObject.SetActive(true);
					entry = t.GetComponent<RoomEntry>();
				}
				else
				{
					// Add new room item if no more child
					var obj = Instantiate(roomEntryPrefab, contentParent);
					entry = obj.GetComponent<RoomEntry>();
				}

				entry.SetRoom(rooms[i]);
			}

			// Inactive room item more than room count 
			for (int i = rooms.Count; i < contentParent.childCount; i++)
			{
				var t = contentParent.GetChild(i);
				t.gameObject.SetActive(false);
			}
		}
	}
}