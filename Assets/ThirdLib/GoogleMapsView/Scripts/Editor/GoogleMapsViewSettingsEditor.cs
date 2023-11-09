using NinevaStudios.GoogleMaps.Internal;
using UnityEditor;
using UnityEngine;

namespace NinevaStudios.GoogleMaps.Editor
{
	[CustomEditor(typeof(GoogleMapsViewSettings))]
	public class GoogleMapsViewSettingsEditor : UnityEditor.Editor
	{
		const string ApiKeyTooltip = "You must obtain an API key from Google in order to use Google Maps API";
		const string IosUsageDescriptionTooltip= "Text shown on the native iOS dialog requesting to access device location";

		[MenuItem("Window/Google Maps View/Edit Settings", false, 1000)]
		public static void Edit()
		{
			Selection.activeObject = GoogleMapsViewSettings.Instance;
		}

		public override void OnInspectorGUI()
		{
			using (new EditorGUILayout.VerticalScope("box"))
			{
				EditorGUILayout.HelpBox(
					"The plugin will modify your AndroidManifest.xml before the build starts",
					MessageType.Info);
				
				GUILayout.Label("Android Settings", EditorStyles.boldLabel);
				var androidApiKey = EditorGUILayout.TextField(new GUIContent("Android API Key [?]", ApiKeyTooltip),  GoogleMapsViewSettings.AndroidApiKey);
				CheckApiKey(androidApiKey, GoogleMapsViewSettings.ANDROID_KEY_PLACEHOLDER);
				GoogleMapsViewSettings.AndroidApiKey = androidApiKey;
				GoogleMapsViewSettings.AddLocationPermission =
					EditorGUILayout.Toggle("Automatically add location permission to AndroidManifest.xml? ", GoogleMapsViewSettings.AddLocationPermission);

				EditorGUILayout.Space();
				if (GUILayout.Button("Read how to get and setup Android API key"))
				{
					Application.OpenURL("https://docs.ninevastudios.com/#/unity-plugins/google-maps?id=setup-android");
				}
			}
			
			EditorGUILayout.Space();

			using (new EditorGUILayout.VerticalScope("box"))
			{
				GUILayout.Label("iOS Settings", EditorStyles.boldLabel);
				var iosApiKey = EditorGUILayout.TextField(new GUIContent("iOS API Key [?]", ApiKeyTooltip), GoogleMapsViewSettings.IosApiKey);
				CheckApiKey(iosApiKey, GoogleMapsViewSettings.IOS_KEY_PLACEHOLDER);
				GoogleMapsViewSettings.IosApiKey = iosApiKey;
				GoogleMapsViewSettings.IosUsageDescription = EditorGUILayout.TextField(new GUIContent("iOS Usage Description [?]", IosUsageDescriptionTooltip), GoogleMapsViewSettings.IosUsageDescription);

				EditorGUILayout.Space();
				if (GUILayout.Button("Read how to get and setup iOS API key"))
				{
					Application.OpenURL("https://docs.ninevastudios.com/#/unity-plugins/google-maps?id=setup-ios");
				}
			}
			
			EditorGUILayout.Space();
			
			EditorGUILayout.HelpBox(
				"If you are seeing a blank map after running on the device it means that the API key is misconfigured. Please go through the setup instructions in the documentation and double-check all the steps.",
				MessageType.Warning);

			using (new EditorGUILayout.HorizontalScope())
			{
				if (GUILayout.Button("Android Setup"))
				{
					Application.OpenURL("https://docs.ninevastudios.com/#/unity-plugins/google-maps?id=setup-android");
				}

				if (GUILayout.Button("iOS Setup"))
				{
					Application.OpenURL("https://docs.ninevastudios.com/#/unity-plugins/google-maps?id=setup-ios");
				}
			}

			EditorGUILayout.Space();

			using (new EditorGUILayout.HorizontalScope("box"))
			{
				if (GUILayout.Button("Read Documentation"))
				{
					Application.OpenURL("https://docs.ninevastudios.com/#/unity-plugins/google-maps");
				}
				if (GUILayout.Button("Ask us anything on Discord"))
				{
					Application.OpenURL("https://bit.ly/nineva_support_discord");
				}
			}
		}

		static void CheckApiKey(string key, string placeholder)
		{
			if (key == placeholder)
			{
				EditorGUILayout.HelpBox(
					"This is a placeholder API key! Please go through the setup section in the docs to setup your own.",
					MessageType.Error);
			}
		}
	}
}