using System.IO;
using GetSocialSdk.Editor.Android.Manifest.GoogleMaps;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace NinevaStudios.GoogleMaps.Internal
{
	public class AndroidManifestPreprocessor : IPreprocessBuildWithReport
	{
		static readonly UsesPermission LocationPermission = new UsesPermission(UsesPermission.LocationPermission);
		static readonly UsesApacheHttpLibrary UsesApacheHttpLibrary = new UsesApacheHttpLibrary();

		const string MainManifestPath = "Plugins/Android/AndroidManifest.xml";

		public int callbackOrder
		{
			get { return 2000; }
		}

		public void OnPreprocessBuild(BuildReport report)
		{
#if UNITY_ANDROID
			Debug.Log("Google Maps View is now preprocessing your AndroidManifest.xml");
			SetupApiKey();
#endif
		}

		void SetupApiKey()
		{
			var manifestPath = Path.Combine(Application.dataPath, MainManifestPath);
			EnsureManifestExists(manifestPath);

			var changed = false;
			var apiKey = new MetaData("com.google.android.geo.API_KEY", GoogleMapsViewSettings.AndroidApiKey.Trim());
			var manifest = new AndroidManifest(manifestPath);

			if (!manifest.Contains(apiKey))
			{
				manifest.RemoveSimilar(apiKey);
				manifest.Add(apiKey);
				changed = true;
			}

			// https://developers.google.com/maps/documentation/android-sdk/config#specify_requirement_for_apache_http_legacy_library
			if (!manifest.Contains(UsesApacheHttpLibrary))
			{
				manifest.RemoveSimilar(UsesApacheHttpLibrary);
				manifest.Add(UsesApacheHttpLibrary);
				changed = true;
			}

			if (GoogleMapsViewSettings.AddLocationPermission && !manifest.Contains(LocationPermission))
			{
				manifest.Add(LocationPermission);
				changed = true;
			}

			if (changed)
			{
				manifest.Save();
			}
		}

		void EnsureManifestExists(string manifestPath)
		{
			if (File.Exists(manifestPath))
			{
				return;
			}

			var pluginPath = GoogleMapsViewSettings.GetPluginPath();
			Debug.Log("Plugin path: " + pluginPath);
			var backupManifestPath = Path.Combine(pluginPath, "Editor/Libs/Android/ManifestBackup/AndroidManifest.xml");

			var manifestDirectoryPath = Path.GetDirectoryName(manifestPath);
			if (!Directory.Exists(manifestDirectoryPath))
			{
				Directory.CreateDirectory(manifestDirectoryPath);
			}

			File.Copy(backupManifestPath, manifestPath);
		}
	}
}