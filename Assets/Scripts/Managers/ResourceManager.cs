using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Mahjong.Model;
using UnityEngine;
using Utils;
using MEC;

namespace Managers
{
	public class ResourceManager : MonoBehaviour
	{
		private static readonly string[] TileSuits = {"m", "s", "p", "z"};

		public static ResourceManager Instance
		{
			get;
			private set;
		}

		private Sprite[] tileSprites;
		private readonly IDictionary<string, Texture2D> textureDict = new Dictionary<string, Texture2D>();
		private IDictionary<string, Sprite> spriteDict;

		private void Awake()
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
			Timing.RunCoroutine(LoadSpritesAsync());
		}

		private IEnumerator<float> LoadSpritesAsync()
		{
			LoadDefaultSettings();
			yield return Timing.WaitForOneFrame;
			tileSprites = Resources.LoadAll<Sprite>("Textures/UIElements/tile_ui");
			yield return Timing.WaitForOneFrame;
			spriteDict = tileSprites.ToDictionary(sprite => sprite.name);
			for (int i = 0; i < TileSuits.Length; i++)
			{
				for (int rank = 0; rank <= 9; rank++)
				{
					var key = $"{rank}{TileSuits[i]}";
					var texture = Resources.Load<Texture2D>($"Textures/TileTextures/{key}");
					if (texture != null) textureDict.Add(key, texture);
				}

				yield return Timing.WaitForOneFrame;
			}
		}

		public Texture2D GetTileTexture(Tile tile)
		{
			var key = GetTileName(tile);
			return textureDict[key];
		}

		public Sprite GetTileSprite(Tile tile)
		{
			if (tileSprites == null)
			{
				Debug.LogError("tileSprite is null, something is wrong, please wait and try again.");
				return null;
			}

			var key = GetTileName(tile);
			return spriteDict[key];
		}

		public Sprite GetTileSpriteByName(string name)
		{
			return spriteDict[name];
		}

		public static string GetTileName(Tile tile)
		{
			int index = tile.IsRed ? 0 : tile.Rank;
			return index + tile.Suit.ToString().ToLower();
		}

		private const string DefaultSettings2 = "Data/default_settings_2";
		private const string DefaultSettings3 = "Data/default_settings_3";
		private const string DefaultSettings4 = "Data/default_settings_4";
		private const string LastSettings = "/settings.json";
		private readonly IDictionary<GamePlayers, string> _defaultSettings = new Dictionary<GamePlayers, string>();

		private void LoadDefaultSettings()
		{
			_defaultSettings.Clear();
			_defaultSettings.Add(GamePlayers.Two, Resources.Load<TextAsset>(DefaultSettings2).text);
			_defaultSettings.Add(GamePlayers.Three, Resources.Load<TextAsset>(DefaultSettings3).text);
			_defaultSettings.Add(GamePlayers.Four, Resources.Load<TextAsset>(DefaultSettings4).text);
		}

		public void LoadSettings(out GameSetting gameSetting)
		{
			gameSetting = SerializeUtility.Load<GameSetting>(LastSettings, _defaultSettings[GamePlayers.Four]);
		}

		public void SaveSettings(object setting, string path)
		{
			setting.Save(path);
		}

		public void SaveSettings(GameSetting gameSetting)
		{
			SaveSettings(gameSetting, LastSettings);
		}

		public void ResetSettings(GameSetting gameSetting)
		{
			Debug.Log("Reset to corresponding default settings");
			var setting = _defaultSettings[gameSetting.GamePlayers];
			JsonUtility.FromJsonOverwrite(setting, gameSetting);
		}
	}
}