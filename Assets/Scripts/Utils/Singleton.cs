using UnityEngine;

namespace Utils
{
	public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
	{
		private static T _instance;

		public static T Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = (T)FindObjectOfType(typeof(T));

					if (_instance == null)
					{
						Debug.LogError(string.Format("{0} is not found.", typeof(T)));
					}
				}

				return _instance;
			}
		}

		protected void Awake()
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