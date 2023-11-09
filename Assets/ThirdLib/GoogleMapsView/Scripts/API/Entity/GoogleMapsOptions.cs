using DeadMosquito.JniToolkit;

namespace NinevaStudios.GoogleMaps
{
	using System;
	using System.Collections.Generic;
	using Internal;
	using JetBrains.Annotations;
	using UnityEngine;

	/// <summary>
	/// Defines configuration GoogleMapOptions for a GoogleMap. 
	/// 
	/// See https://developers.google.com/android/reference/com/google/android/gms/maps/GoogleMapOptions.html for reference.
	/// </summary>
	[PublicAPI]
	public class GoogleMapsOptions : IDisposable
	{
		const string GoogleMapsOptionsJavaClass = "com.google.android.gms.maps.GoogleMapOptions";

		const string AmbientEnabledMethodName = "ambientEnabled";
		const string CameraMethodName = "camera";
		const string CompassEnabledMethodName = "compassEnabled";
		const string LatLngBoundForCameraTargetMethodName = "latLngBoundsForCameraTarget";
		const string LiteModeMethodName = "liteMode";
		const string MapToolbarEnabledMethodName = "mapToolbarEnabled";
		const string MapTypeMethodName = "mapType";
		const string MaxZoomPreferenceMethodName = "maxZoomPreference";
		const string MinZoomPreferenceMethodName = "minZoomPreference";
		const string RotateGesturesEnabledMethodName = "rotateGesturesEnabled";
		const string ScrollGesturesEnabledMethodName = "scrollGesturesEnabled";
		const string TiltGesturesEnabledMethodName = "tiltGesturesEnabled";
		const string ZoomControlsEnabledMethodName = "zoomControlsEnabled";
		const string ZoomGesturesEnabledMethodName = "zoomGesturesEnabled";

		readonly Dictionary<string, object> _dic;

		static readonly Dictionary<string, object> DefaultOptionsDictionary = new Dictionary<string, object>
		{
			{CameraMethodName, new CameraPosition(new LatLng(0, 0), 6, 0, 0).ToDictionary()},
			{LatLngBoundForCameraTargetMethodName, null},
			{MapTypeMethodName, (int) GoogleMapType.Normal},
			
			{MaxZoomPreferenceMethodName, null},
			{MinZoomPreferenceMethodName, null},
			
			{CompassEnabledMethodName, true},
			{RotateGesturesEnabledMethodName, true},
			{ScrollGesturesEnabledMethodName, true},
			{TiltGesturesEnabledMethodName, true},
			{ZoomGesturesEnabledMethodName, true}
		};

		public AndroidJavaObject AJO { get; }

		/// <summary>
		/// Initializes a new instance of the <see cref="GoogleMapsOptions"/> class.
		/// </summary>
		[PublicAPI]
		public GoogleMapsOptions()
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return;
			}

			if (GoogleMapUtils.IsAndroid)
			{
				AJO = new AndroidJavaObject(GoogleMapsOptionsJavaClass);
			}
			else
			{
				_dic = new Dictionary<string, object>();
				foreach (var entry in DefaultOptionsDictionary)
				{
					_dic.Add(entry.Key, entry.Value);
				}
			}
		}

		/// <summary>
		/// Specifies whether ambient-mode styling should be enabled. Has no effect on iOS
		/// </summary>
		/// <param name="enabled">Whether ambient-mode styling should be enabled.</param>
		[PublicAPI]
		[GoogleMapsAndroidOnly]
		public GoogleMapsOptions AmbientEnabled(bool enabled)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return this;
			}

			if (GoogleMapUtils.IsAndroid)
			{
				AJO.CallAJO(AmbientEnabledMethodName, enabled);
			}

			return this;
		}

		/// <summary>
		/// Specifies a the initial camera position for the map.
		/// </summary>
		/// <param name="camera">The initial camera position for the map.</param>
		[PublicAPI]
		public GoogleMapsOptions Camera([NotNull] CameraPosition camera)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return this;
			}

			if (GoogleMapUtils.IsAndroid)
			{
				AJO.CallAJO(CameraMethodName, camera.ToAJO());
			}
			else
			{
				_dic[CameraMethodName] = camera.ToDictionary();
			}

			return this;
		}

		/// <summary>
		/// Specifies whether the compass should be enabled. The default value is true.
		/// </summary>
		/// <param name="enabled">Whether the compass should be enabled. The default value is true.</param>
		[PublicAPI]
		public GoogleMapsOptions CompassEnabled(bool enabled)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return this;
			}

			if (GoogleMapUtils.IsAndroid)
			{
				AJO.CallAJO(CompassEnabledMethodName, enabled);
			}
			else
			{
				_dic[CompassEnabledMethodName] = enabled;
			}

			return this;
		}

		/// <summary>
		/// Specifies LatLngBounds used to constrain the camera target.
		/// </summary>
		/// <param name="llbounds">LatLngBounds used to constrain the camera target.</param>
		[PublicAPI]
		public GoogleMapsOptions LatLngBoundsForCameraTarget(LatLngBounds llbounds)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return this;
			}

			if (GoogleMapUtils.IsAndroid)
			{
				AJO.CallAJO(LatLngBoundForCameraTargetMethodName, llbounds.ToAJO());
			}
			else
			{
				_dic[LatLngBoundForCameraTargetMethodName] = llbounds.ToDictionary();
			}

			return this;
		}

		/// <summary>
		/// Specifies if lite mode enabled. Does not have effect on iOS
		/// </summary>
		/// <param name="enabled">If set to <c>true</c> lite mode will be enabled.</param>
		[PublicAPI]
		[GoogleMapsAndroidOnly]
		public GoogleMapsOptions LiteMode(bool enabled)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return this;
			}

			if (GoogleMapUtils.IsAndroid)
			{
				AJO.CallAJO(LiteModeMethodName, enabled);
			}

			return this;
		}

		/// <summary>
		/// Specifies if map toolbar enabled. Has no effect on iOS
		/// </summary>
		/// <returns>The toolbar enabled.</returns>
		/// <param name="enabled">If map toolbar enabled.</param>
		[PublicAPI]
		[GoogleMapsAndroidOnly]
		public GoogleMapsOptions MapToolbarEnabled(bool enabled)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return this;
			}

			if (GoogleMapUtils.IsAndroid)
			{
				AJO.CallAJO(MapToolbarEnabledMethodName, enabled);
			}

			return this;
		}

		/// <summary>
		/// Specifies the map type.
		/// </summary>
		/// <param name="mapType">Map type.</param>
		[PublicAPI]
		public GoogleMapsOptions MapType(GoogleMapType mapType)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return this;
			}

			if (GoogleMapUtils.IsAndroid)
			{
				AJO.CallAJO(MapTypeMethodName, (int) mapType);
			}
			else
			{
				_dic[MapTypeMethodName] = (int) mapType;
			}

			return this;
		}

		/// <summary>
		/// the maximum zoom level preference.
		/// </summary>
		/// <param name="maxZoomPreference">The maximum zoom level preference.</param>
		[PublicAPI]
		public GoogleMapsOptions MaxZoomPreference(float maxZoomPreference)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return this;
			}

			if (GoogleMapUtils.IsAndroid)
			{
				AJO.CallAJO(MaxZoomPreferenceMethodName, maxZoomPreference);
			}
			else
			{
				_dic[MaxZoomPreferenceMethodName] = maxZoomPreference;
			}

			return this;
		}

		/// <summary>
		/// the minimum zoom level preference.
		/// </summary>
		/// <param name="minZoomPreference">The minimum zoom level preference.</param>
		[PublicAPI]
		public GoogleMapsOptions MinZoomPreference(float minZoomPreference)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return this;
			}

			if (GoogleMapUtils.IsAndroid)
			{
				AJO.CallAJO(MinZoomPreferenceMethodName, minZoomPreference);
			}
			else
			{
				_dic[MinZoomPreferenceMethodName] = minZoomPreference;
			}

			return this;
		}

		/// <summary>
		/// Specifies if the rotate gestures are enabled.
		/// </summary>
		/// <param name="enabled">If the rotate gestures are enabled.</param>
		[PublicAPI]
		public GoogleMapsOptions RotateGesturesEnabled(bool enabled)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return this;
			}

			if (GoogleMapUtils.IsAndroid)
			{
				AJO.CallAJO(RotateGesturesEnabledMethodName, enabled);
			}
			else
			{
				_dic[RotateGesturesEnabledMethodName] = enabled;
			}

			return this;
		}

		/// <summary>
		/// Specifies if the scroll gestures are enabled.
		/// </summary>
		/// <param name="enabled">If the scroll gestures are enabled.</param>
		[PublicAPI]
		public GoogleMapsOptions ScrollGesturesEnabled(bool enabled)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return this;
			}

			if (GoogleMapUtils.IsAndroid)
			{
				AJO.CallAJO(ScrollGesturesEnabledMethodName, enabled);
			}
			else
			{
				_dic[ScrollGesturesEnabledMethodName] = enabled;
			}

			return this;
		}

		/// <summary>
		/// Specifies if the tilt gestures are enabled.
		/// </summary>
		/// <param name="enabled">If the tilt gestures are enabled.</param>
		[PublicAPI]
		public GoogleMapsOptions TiltGesturesEnabled(bool enabled)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return this;
			}

			if (GoogleMapUtils.IsAndroid)
			{
				AJO.CallAJO(TiltGesturesEnabledMethodName, enabled);
			}
			else
			{
				_dic[TiltGesturesEnabledMethodName] = enabled;
			}

			return this;
		}

		/// <summary>
		/// Specifies if the zoom controls are enabled. Has no effect on iOS
		/// </summary>
		/// <param name="enabled">If the zoom controls are enabled.</param>
		[PublicAPI]
		[GoogleMapsAndroidOnly]
		public GoogleMapsOptions ZoomControlsEnabled(bool enabled)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return this;
			}

			if (GoogleMapUtils.IsAndroid)
			{
				AJO.CallAJO(ZoomControlsEnabledMethodName, enabled);
			}

			return this;
		}

		/// <summary>
		/// Specifies if the zoom gestures are enabled.
		/// </summary>
		/// <param name="enabled">If the zoom gestures are enabled.</param>
		[PublicAPI]
		public GoogleMapsOptions ZoomGesturesEnabled(bool enabled)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return this;
			}

			if (GoogleMapUtils.IsAndroid)
			{
				AJO.CallAJO(ZoomGesturesEnabledMethodName, enabled);
			}
			else
			{
				_dic[ZoomGesturesEnabledMethodName] = enabled;
			}

			return this;
		}

		public override string ToString()
		{
			return "[GoogleMapsOptions]";
		}

		#region IDisposable implementation

		public void Dispose()
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return;
			}

			if (!AJO.IsJavaNull())
			{
				AJO.Dispose();
			}
		}

		public Dictionary<string, object> ToDictionary()
		{
			return _dic;
		}

		#endregion
	}
}