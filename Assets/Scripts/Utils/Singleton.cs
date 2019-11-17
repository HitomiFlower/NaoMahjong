using UnityEngine;

namespace Utils
{
	public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
	{
		// Singleton placeholder
		private static T _instance;
		// Lock instance object for execution
		private static object _key = new object();
		// Is application up and running
		protected static bool alive = true;

		/// <summary>
		/// Current and only running instance of the AudioManager
		/// </summary>
		public static T Instance
		{
			// Singleton design pattern
			get
			{
				// Check if application is quitting and AudioManager is destroyed
				if (!alive)
				{
					Debug.LogWarning(typeof(T) + "' is already destroyed on application quit.");
					return null;
				}

				// Check if there is a saved instance
				if (_instance != null)
				{
					return _instance;
				}
				
				// Find from the list or hierrachy
				_instance = FindObjectOfType<T>();

				// If none exists in scene
				if (_instance == null)
				{
					// Lock so can't be used by another thread until release. Useful if two AudioManager instances were created simultaneosly
					lock (_key)
					{
						//GameObject clone = new GameObject();
						//clone.SetActive(false);
						//clone.AddComponent<AudioManager>();

						// Create a new gameobject and add the AudioManager component to the gameobject
						_instance = new GameObject().AddComponent<T>();
					}
				}

				return _instance;
			}
		}

		private void Awake()
		{
			CheckInstance();
		}

		protected bool CheckInstance()
		{
			if (_instance == null)
			{
				_instance = (T)this;
				return true;
			}

			if (_instance == this)
			{
				return true;
			}

			Destroy(this);
			return false;
		}
	}
}