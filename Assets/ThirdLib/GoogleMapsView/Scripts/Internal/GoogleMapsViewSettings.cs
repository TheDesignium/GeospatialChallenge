using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace NinevaStudios.GoogleMaps.Internal
{
	public class GoogleMapsViewSettings : ScriptableObject
	{
		public const string IOS_KEY_PLACEHOLDER = "AIzaSyD2kIFxZi5oDnMisuPPwkxfF27W7HhnRc4";
		public const string ANDROID_KEY_PLACEHOLDER = "AIzaSyCKo5_MdP3liUmFSCWhq--SsbFvRg8VbJ8";
		
		const string SettingsAssetName = "GoogleMapsViewSettings";
		const string SettingsAssetPath = "Resources/";
		
		static GoogleMapsViewSettings _instance;
		
		[SerializeField] string _apiKeyIos = IOS_KEY_PLACEHOLDER;
		[SerializeField] string _apiKeyAndroid = ANDROID_KEY_PLACEHOLDER;
		[SerializeField] bool _addLocationPermission = true;
		[SerializeField] string _iosUsageDescription = "To display your location on the map we would need you to allow location access";
		
		public static string IosApiKey
		{
			get => Instance._apiKeyIos;
			set
			{
				Instance._apiKeyIos = value;
				MarkAssetDirty();
			}
		}
		
		public static string AndroidApiKey
		{
			get => Instance._apiKeyAndroid;
			set
			{
				Instance._apiKeyAndroid = value;
				MarkAssetDirty();
			}
		}
		
		public static bool AddLocationPermission
		{
			get => Instance._addLocationPermission;
			set
			{
				Instance._addLocationPermission = value;
				MarkAssetDirty();
			}
		}
		
		public static string IosUsageDescription
		{
			get => Instance._iosUsageDescription;
			set
			{
				Instance._iosUsageDescription = value;
				MarkAssetDirty();
			}
		}
		
		public static GoogleMapsViewSettings Instance
		{
			get
			{
				if (_instance == null)
				{
					_instance = Resources.Load(SettingsAssetName) as GoogleMapsViewSettings;
					if (_instance == null)
					{
						_instance = CreateInstance<GoogleMapsViewSettings>();

						SaveAsset(Path.Combine(GetPluginPath(), SettingsAssetPath), SettingsAssetName);
					}
				}

				return _instance;
			}
		}
		
		public static string GetPluginPath()
		{
			return GetAbsolutePluginPath().Replace("\\", "/").Replace(Application.dataPath, "Assets");
		}

		static string GetAbsolutePluginPath()
		{
			return Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(FindEditor(Application.dataPath))));
		}

		static string FindEditor(string path)
		{
			foreach (var d in Directory.GetDirectories(path))
			{
				foreach (var f in Directory.GetFiles(d))
				{
					if (f.Contains("GoogleMapsViewSettingsEditor.cs"))
					{
						return f;
					}
				}

				var rec = FindEditor(d);
				if (rec != null)
				{
					return rec;
				}
			}

			return null;
		}
		
		static void SaveAsset(string directory, string name)
		{
#if UNITY_EDITOR
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}

			AssetDatabase.CreateAsset(Instance, directory + name + ".asset");
			AssetDatabase.Refresh();
#endif
		}
		
		static void MarkAssetDirty()
		{
#if UNITY_EDITOR
			EditorUtility.SetDirty(Instance);
#endif
		}
	}
}