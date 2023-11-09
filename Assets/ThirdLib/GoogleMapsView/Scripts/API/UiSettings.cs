using DeadMosquito.JniToolkit;

namespace NinevaStudios.GoogleMaps
{
	using System;
	using System.Diagnostics.CodeAnalysis;
	using System.Runtime.InteropServices;
	using Internal;
	using JetBrains.Annotations;
	using UnityEngine;

	/// <summary>
	/// Settings for the user interface of a GoogleMap. To obtain this object, call <see cref="GoogleMapsView.UiSettings"/>.
	/// </summary>
	[PublicAPI]
	[SuppressMessage("ReSharper", "HeuristicUnreachableCode")]
	public sealed class UiSettings
	{
		readonly AndroidJavaObject _settingsAjo;
		
#pragma warning disable 0414
		readonly IntPtr _mapPtr;
#pragma warning restore 0414

		internal UiSettings()
		{
		}

		internal UiSettings(IntPtr mapPtr)
		{
			_mapPtr = mapPtr;
		}

		internal UiSettings(AndroidJavaObject settingsAjo)
		{
			_settingsAjo = settingsAjo;
		}

		/// <summary>
		/// Gets/sets whether the compass is enabled/disabled.
		/// </summary>
		[PublicAPI]
		public bool IsCompassEnabled
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return false;
				}

 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				return _googleMapsViewSettingsIsCompassEnabled(_mapPtr);
#endif

#pragma warning disable 0162
				return GetBoolAndroid("isCompassEnabled");
#pragma warning restore 0162
			}
			set
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return;
				}

				SetBoolIfAndroid("setCompassEnabled", value);

 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				_googleMapsViewSettingsSetCompassEnabled(_mapPtr, value);
#endif
			}
		}

		/// <summary>
		/// Gets/sets whether the indoor level picker is enabled/disabled.
		/// </summary>
		[PublicAPI]
		public bool IsIndoorLevelPickerEnabled
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return false;
				}

 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				return _googleMapsViewSettingsIsIndoorPickerEnabled(_mapPtr);
#endif
				
#pragma warning disable 0162
				return GetBoolAndroid("isIndoorLevelPickerEnabled");
#pragma warning restore 0162
			}
			set
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return;
				}

				SetBoolIfAndroid("setIndoorLevelPickerEnabled", value);

 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				_googleMapsViewSettingsSetIndoorPickerEnabled(_mapPtr, value);
#endif
			}
		}

		/// <summary>
		/// Gets/sets whether the Map Toolbar is enabled/disabled. Has no effect on iOS
		/// </summary>
		[PublicAPI]
		[GoogleMapsAndroidOnly]
		public bool IsMapToolbarEnabled
		{
			get
			{
				if (GoogleMapUtils.IsNotAndroid)
				{
					return false;
				}

				return GetBoolAndroid("isMapToolbarEnabled");
			}
			set
			{
				if (GoogleMapUtils.IsNotAndroid)
				{
					return;
				}

				SetBoolIfAndroid("setMapToolbarEnabled", value);
			}
		}

		/// <summary>
		/// Gets/sets whether the my-location button is enabled/disabled.
		/// </summary>
		[PublicAPI]
		public bool IsMyLocationButtonEnabled
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return false;
				}

 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				return _googleMapsViewSettingsIsMyLocationEnabled(_mapPtr);
#endif

#pragma warning disable 0162
				return GetBoolAndroid("isMyLocationButtonEnabled");
#pragma warning restore 0162
			}
			set
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return;
				}

				SetBoolIfAndroid("setMyLocationButtonEnabled", value);

 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				_googleMapsViewSettingsSetMyLocationEnabled(_mapPtr, value);
#endif
			}
		}

		/// <summary>
		/// Gets/sets whether rotate gestures are enabled/disabled.
		/// </summary>
		[PublicAPI]
		public bool IsRotateGesturesEnabled
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return false;
				}
 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				return _googleMapsViewSettingsIsRotateGesturesEnabled(_mapPtr);
#endif

#pragma warning disable 0162
				return GetBoolAndroid("isRotateGesturesEnabled");
#pragma warning restore 0162
			}
			set
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return;
				}

				SetBoolIfAndroid("setRotateGesturesEnabled", value);
				
 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				_googleMapsViewSettingsSetRotateGesturesEnabled(_mapPtr, value);
#endif
			}
		}

		/// <summary>
		/// Gets/sets whether scroll gestures are enabled/disabled.
		/// </summary>
		[PublicAPI]
		public bool IsScrollGesturesEnabled
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return false;
				}
 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				return _googleMapsViewSettingsIsScrollGesturesEnabled(_mapPtr);
#endif
				
#pragma warning disable 0162
				return GetBoolAndroid("isScrollGesturesEnabled");
#pragma warning restore 0162
			}
			set
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return;
				}

				SetBoolIfAndroid("setScrollGesturesEnabled", value);
				
 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				_googleMapsViewSettingsSetScrollGesturesEnabled(_mapPtr, value);
#endif
			}
		}

		/// <summary>
		/// Gets whether tilt gestures are enabled/disabled.
		/// </summary>
		[PublicAPI]
		public bool IsTiltGesturesEnabled
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return false;
				}
				
 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				return _googleMapsViewSettingsIsTiltGesturesEnabled(_mapPtr);
#endif
				
#pragma warning disable 0162
				return GetBoolAndroid("isTiltGesturesEnabled");
#pragma warning restore 0162
			}
			set
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return;
				}

				SetBoolIfAndroid("setTiltGesturesEnabled", value);
				
 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				_googleMapsViewSettingsSetTiltGesturesEnabled(_mapPtr, value);
#endif
			}
		}

		/// <summary>
		/// Gets/sets whether the zoom controls are enabled/disabled. Has no effect on iOS.
		/// </summary>
		[PublicAPI]
		[GoogleMapsAndroidOnly]
		public bool IsZoomControlsEnabled
		{
			get
			{
				if (GoogleMapUtils.IsNotAndroid)
				{
					return false;
				}

				return GetBoolAndroid("isZoomControlsEnabled");
			}
			set
			{
				if (GoogleMapUtils.IsNotAndroid)
				{
					return;
				}

				SetBoolIfAndroid("setZoomControlsEnabled", value);
			}
		}

		/// <summary>
		/// Gets/sets whether zoom gestures are enabled/disabled.
		/// </summary>
		[PublicAPI]
		public bool IsZoomGesturesEnabled
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return false;
				}
				
 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				return _googleMapsViewSettingsIsZoomGesturesEnabled(_mapPtr);
#endif
				
#pragma warning disable 0162
				return GetBoolAndroid("isZoomGesturesEnabled");
#pragma warning restore 0162
			}
			set
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return;
				}

				SetBoolIfAndroid("setZoomGesturesEnabled", value);
				
 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				_googleMapsViewSettingsSetZoomGesturesEnabled(_mapPtr, value);
#endif
			}
		}

		/// <summary>
		/// Sets the preference for whether all gestures should be enabled or disabled.
		/// </summary>
		/// <param name="enabled">Whether all gestures should be enabled</param>
		[PublicAPI]
		public void SetAllGesturesEnabled(bool enabled)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return;
			}

			if (GoogleMapUtils.IsAndroid)
			{
				SetBoolIfAndroid("setAllGesturesEnabled", enabled);
			}
			
 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			_googleMapsViewSettingsSetAllGesturesEnabled(_mapPtr, enabled);
#endif
		}

		public override string ToString()
		{
			return
				$"[UiSettings: IsCompassEnabled={IsCompassEnabled}, IsIndoorLevelPickerEnabled={IsIndoorLevelPickerEnabled}, IsMapToolbarEnabled={IsMapToolbarEnabled}, IsMyLocationButtonEnabled={IsMyLocationButtonEnabled}, IsRotateGesturesEnabled={IsRotateGesturesEnabled}, IsScrollGesturesEnabled={IsScrollGesturesEnabled}, IsTiltGesturesEnabled={IsTiltGesturesEnabled}, IsZoomControlsEnabled={IsZoomControlsEnabled}, IsZoomGesturesEnabled={IsZoomGesturesEnabled}]";
		}

		void SetBoolIfAndroid(string methodname, bool value)
		{
			if (GoogleMapUtils.IsAndroid)
			{
				_settingsAjo.MainThreadCall(methodname, value);
			}
		}

		bool GetBoolAndroid(string methodName)
		{
			return _settingsAjo.MainThreadCallBool(methodName);
		}

 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
		[DllImport("__Internal")]
		static extern bool _googleMapsViewSettingsIsCompassEnabled(IntPtr mapPtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewSettingsSetCompassEnabled(IntPtr mapPtr, bool isEnabled);

		[DllImport("__Internal")]
		static extern bool _googleMapsViewSettingsIsIndoorPickerEnabled(IntPtr mapView);

		[DllImport("__Internal")]
		static extern void _googleMapsViewSettingsSetIndoorPickerEnabled(IntPtr mapView, bool isEnabled);

		[DllImport("__Internal")]
		static extern bool _googleMapsViewSettingsIsMyLocationEnabled(IntPtr mapView);

		[DllImport("__Internal")]
		static extern void _googleMapsViewSettingsSetMyLocationEnabled(IntPtr mapView, bool isEnabled);

		[DllImport("__Internal")]
		static extern bool _googleMapsViewSettingsIsRotateGesturesEnabled(IntPtr mapView);

		[DllImport("__Internal")]
		static extern void _googleMapsViewSettingsSetRotateGesturesEnabled(IntPtr mapView, bool isEnabled);

		[DllImport("__Internal")]
		static extern bool _googleMapsViewSettingsIsScrollGesturesEnabled(IntPtr mapView);

		[DllImport("__Internal")]
		static extern void _googleMapsViewSettingsSetScrollGesturesEnabled(IntPtr mapView, bool isEnabled);

		[DllImport("__Internal")]
		static extern bool _googleMapsViewSettingsIsTiltGesturesEnabled(IntPtr mapView);

		[DllImport("__Internal")]
		static extern void _googleMapsViewSettingsSetTiltGesturesEnabled(IntPtr mapView, bool isEnabled);

		[DllImport("__Internal")]
		static extern bool _googleMapsViewSettingsIsZoomGesturesEnabled(IntPtr mapView);

		[DllImport("__Internal")]
		static extern void _googleMapsViewSettingsSetZoomGesturesEnabled(IntPtr mapView, bool isEnabled);

		[DllImport("__Internal")]
		static extern void _googleMapsViewSettingsSetAllGesturesEnabled(IntPtr mapView, bool isEnabled);
#endif
	}
}