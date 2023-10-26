using System.Collections.Generic;
using NinevaStudios.GoogleMaps.MiniJSON;

namespace NinevaStudios.GoogleMaps
{
	using System;
	using System.Diagnostics.CodeAnalysis;
	using System.Runtime.InteropServices;
	using Internal;
	using JetBrains.Annotations;
	using DeadMosquito.JniToolkit;
	using UnityEngine;

	/// <summary>
	/// An icon placed at a particular point on the map's surface. A marker icon is drawn oriented against the device's screen rather than the map's surface; i.e., it will not necessarily change orientation due to map rotations, tilting, or zooming.
	/// 
	/// For more details visit: https://developers.google.com/android/reference/com/google/android/gms/maps/model/Marker
	/// </summary>
	[PublicAPI]
	[SuppressMessage("ReSharper", "HeuristicUnreachableCode")]
	public sealed class Marker
	{
		bool _wasRemoved;

		public Marker()
		{
		}

		readonly AndroidJavaObject _ajo;

#pragma warning disable 0414
		readonly IntPtr _markerPtr;
		readonly IntPtr _mapPtr;
#pragma warning restore 0414

		public Marker(AndroidJavaObject ajo)
		{
			_ajo = ajo;
		}

		public Marker(IntPtr markerPtr, IntPtr mapPtr)
		{
			_markerPtr = markerPtr;
			_mapPtr = mapPtr;
		}

		/// <summary>
		/// Gets this marker's id. The id will be unique amongst all Markers on a map.
		/// </summary>
		/// <value>The marker identifier.</value>
		[PublicAPI]
		public string Id
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return string.Empty;
				}

				CheckIfRemoved();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				return _googleMapsViewMarkerGetId(_markerPtr);
#endif

#pragma warning disable 0162
				return GetValueIfAndroid<string>("getId");
#pragma warning restore 0162
			}
		}

		/// <summary>
		/// Returns the position as a <see cref="LatLng"/>.
		/// </summary>
		/// <value>The geographic position as a <see cref="LatLng"/></value>
		[PublicAPI]
		public LatLng Position
		{
			set
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return;
				}

				CheckIfRemoved();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				_googleMapsViewMarkerSetPosition(_markerPtr, value.Latitude, value.Longitude);
#endif

				SetValueIfAndroid("setPosition", value.ToAJO());
			}
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return LatLng.Zero;
				}

				CheckIfRemoved();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				var posJson = _googleMapsViewMarkerGetPosition(_markerPtr);
				return LatLng.FromJson(posJson);
#endif

#pragma warning disable 0162
				return LatLng.FromAJO(GetValueIfAndroid<AndroidJavaObject>("getPosition"));
#pragma warning restore 0162
			}
		}

		/// <summary>
		/// Gets or sets the opacity of the marker. Defaults to 1.0.
		/// </summary>
		/// <value>The opacity of the marker.</value>
		[PublicAPI]
		public float Alpha
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return 0f;
				}

				CheckIfRemoved();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				return _googleMapsViewMarkerGetTransparency(_markerPtr);
#endif

#pragma warning disable 0162
				return GetValueIfAndroid<float>("getAlpha");
#pragma warning restore 0162
			}
			set
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return;
				}

				CheckIfRemoved();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				_googleMapsViewMarkerSetTransparency(_markerPtr, value);
#endif

				SetValueIfAndroid("setAlpha", value);
			}
		}

		/// <summary>
		/// See https://developers.google.com/android/reference/com/google/android/gms/maps/model/Marker.html setAnchor method for more detailed description
		/// 
		/// Sets the anchor point for the marker. The anchor specifies the point in the icon image that is anchored to the marker's position on the Earth's surface.
		/// </summary>
		/// <param name="anchorU">u-coordinate of the anchor, as a ratio of the image width (in the range [0, 1]).</param>
		/// <param name="anchorV">v-coordinate of the anchor, as a ratio of the image height (in the range [0, 1]).</param>
		[PublicAPI]
		public void SetAnchor(float anchorU, float anchorV)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return;
			}

			CheckIfRemoved();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			_googleMapsViewMarkerSetAnchor(_markerPtr, anchorU, anchorV);
#endif

			SetValueIfAndroid("setAnchor", anchorU, anchorV);
		}

		/// <summary>
		/// Specifies the point in the marker image at which to anchor the info window when it is displayed.
		/// </summary>
		/// <param name="anchorU">u-coordinate of the info window anchor, as a ratio of the image width (in the range [0, 1]).</param>
		/// <param name="anchorV">v-coordinate of the info window anchor, as a ratio of the image height (in the range [0, 1]).</param>
		[PublicAPI]
		public void SetInfoWindowAnchor(float anchorU, float anchorV)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return;
			}

			CheckIfRemoved();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			_googleMapsViewMarkerSetInfoWindowAnchor(_markerPtr, anchorU, anchorV);
#endif

			SetValueIfAndroid("setInfoWindowAnchor", anchorU, anchorV);
		}

		/// <summary>
		/// A text string that's displayed in an info window when the user taps the marker. You can change this value at any time.
		/// </summary>
		/// <returns>The title of the marker</returns>
		[PublicAPI]
		public string Title
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return string.Empty;
				}

				CheckIfRemoved();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				return _googleMapsViewMarkerGetTitle(_markerPtr);
#endif

#pragma warning disable 0162
				return GetValueIfAndroid<string>("getTitle");
#pragma warning restore 0162
			}
			set
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return;
				}

				CheckIfRemoved();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				_googleMapsViewMarkerSetTitle(_markerPtr, value);
#endif

				SetValueIfAndroid("setTitle", value);
			}
		}

		/// <summary>
		/// Additional text that's displayed below the title. You can change this value at any time.
		/// </summary>
		/// <returns>Marker snippet text</returns>
		[PublicAPI]
		public string Snippet
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return string.Empty;
				}

				CheckIfRemoved();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				return _googleMapsViewMarkerGetSnippet(_markerPtr);
#endif

#pragma warning disable 0162
				return GetValueIfAndroid<string>("getSnippet");
#pragma warning restore 0162
			}
			set
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return;
				}

				CheckIfRemoved();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				_googleMapsViewMarkerSetSnippet(_markerPtr, value);
#endif

				SetValueIfAndroid("setSnippet", value);
			}
		}

		/// <summary>
		/// Sets the icon for the marker.
		/// </summary>
		/// <param name="imageDescriptor">Image descriptor. If null, the default marker is used.</param>
		[PublicAPI]
		public void SetIcon(ImageDescriptor imageDescriptor)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return;
			}

			CheckIfRemoved();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			var options = new Dictionary<string, object>();
			GoogleMapUtils.AddImage(options, imageDescriptor);
			_googleMapsViewMarkerSetIcon(_markerPtr, Json.Serialize(options));
#endif

			SetValueIfAndroid("setIcon", imageDescriptor.ToAJO());
		}

		/// <summary>
		/// If you want to allow the user to drag the marker, set this property to <code>true</code>. You can change this value at any time. The default is <code>false</code>
		/// </summary>
		[PublicAPI]
		[Obsolete("Use IsDraggable")]
		public bool DragStatus
		{
			get => IsDraggable;
			set => IsDraggable = value;
		}

		/// <summary>
		/// If you want to allow the user to drag the marker, set this property to <code>true</code>. You can change this value at any time. The default is <code>false</code>
		/// </summary>
		[PublicAPI]
		public bool IsDraggable
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return false;
				}

				CheckIfRemoved();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				return _googleMapsViewMarkerIsDraggable(_markerPtr);
#endif

#pragma warning disable 0162
				return GetValueIfAndroid<bool>("isDraggable");
#pragma warning restore 0162
			}
			set
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return;
				}

				CheckIfRemoved();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				_googleMapsViewMarkerSetDraggable(_markerPtr, value);
#endif

				SetValueIfAndroid("setDraggable", value);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this marker is visible.
		/// If this marker is not visible then it will not be drawn. All other state is preserved. Markers are visible by default.
		/// </summary>
		/// <value><c>true</c> if this marker is visible; otherwise, <c>false</c>.</value>
		[PublicAPI]
		public bool IsVisible
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return false;
				}

				CheckIfRemoved();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				return _googleMapsViewMarkerIsVisible(_markerPtr);
#endif

#pragma warning disable 0162
				return GetValueIfAndroid<bool>("isVisible");
#pragma warning restore 0162
			}
			set
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return;
				}

				CheckIfRemoved();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				_googleMapsViewMarkerSetVisible(_markerPtr, _mapPtr, value);
#endif

				SetValueIfAndroid("setVisible", value);
			}
		}


		/// <summary>
		/// Gets or sets the flat setting of the Marker.
		/// </summary>
		/// <returns><code>true</code> the marker is flat against the map; <code>false</code> if the marker should face the camera.</returns>
		[PublicAPI]
		public bool Flat
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return false;
				}

				CheckIfRemoved();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				return _googleMapsViewMarkerIsFlat(_markerPtr);
#endif

#pragma warning disable 0162
				return GetValueIfAndroid<bool>("isFlat");
#pragma warning restore 0162
			}
			set
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return;
				}

				CheckIfRemoved();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				_googleMapsViewMarkerSetFlat(_markerPtr, value);
#endif

				SetValueIfAndroid("setFlat", value);
			}
		}

		/// <summary>
		/// Gets or sets the rotation of the marker.
		/// </summary>
		/// <returns>The rotation of the marker in degrees clockwise from the default position.</returns>
		[PublicAPI]
		public float Rotation
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return 0f;
				}

				CheckIfRemoved();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				return (float) _googleMapsViewMarkerGetRotation(_markerPtr);
#endif

#pragma warning disable 0162
				return GetValueIfAndroid<float>("getRotation");
#pragma warning restore 0162
			}
			set
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return;
				}

				CheckIfRemoved();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				_googleMapsViewMarkerSetRotation(_markerPtr, value);
#endif

				SetValueIfAndroid("setRotation", value);
			}
		}

		/// <summary>
		/// Gets or sets the index of the Z. Overlays (such as markers) with higher zIndices are drawn above those with lower indices.
		/// </summary>
		/// <value>The index of the Z.</value>
		[PublicAPI]
		public float ZIndex
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return 0f;
				}

				CheckIfRemoved();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				return _googleMapsViewMarkerGetZIndex(_markerPtr);
#endif

#pragma warning disable 0162
				return GetValueIfAndroid<float>("getZIndex");
#pragma warning restore 0162
			}
			set
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return;
				}

				CheckIfRemoved();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				_googleMapsViewMarkerSetZIndex(_markerPtr, (int) value);
#endif

				SetValueIfAndroid("setZIndex", value);
			}
		}

		/// <summary>
		/// Shows the info window of this marker on the map, if this marker <see cref="IsVisible"/>.
		/// </summary>
		[PublicAPI]
		public void ShowInfoWindow()
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return;
			}

			CheckIfRemoved();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			_googleMapsViewMarkerShowInfoWindow(_markerPtr, _mapPtr);
#endif

			if (GoogleMapUtils.IsAndroid)
			{
				_ajo.MainThreadCall("showInfoWindow");
			}
		}

		/// <summary>
		/// Hides the info window of this marker on the map, if this marker <see cref="IsVisible"/>.
		/// </summary>
		[PublicAPI]
		public void HideInfoWindow()
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return;
			}

			CheckIfRemoved();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			_googleMapsViewMarkerHideInfoWindow(_mapPtr);
#endif

			if (GoogleMapUtils.IsAndroid)
			{
				_ajo.MainThreadCall("hideInfoWindow");
			}
		}

		/// <summary>
		/// Gets whether the marker info window on the map is shown.
		/// </summary>
		public bool IsInfoWindowShown
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return false;
				}

				CheckIfRemoved();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				return _googleMapsViewMarkerIsInfoWindowShown(_markerPtr, _mapPtr);
#endif

#pragma warning disable 0162
				return _ajo.MainThreadCallBool("isInfoWindowShown");
#pragma warning restore 0162
			}
		}

		/// <summary>
		/// Removes this marker from the map.
		/// </summary>
		[PublicAPI]
		public void Remove()
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return;
			}

			CheckIfRemoved();

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			_googleMapsViewMarkerRemove(_markerPtr);
#endif

			if (GoogleMapUtils.IsAndroid)
			{
				_ajo.MainThreadCall("remove");
			}

			_wasRemoved = true;
		}

		T GetValueIfAndroid<T>(string methodName)
		{
			return GoogleMapUtils.IsAndroid ? _ajo.MainThreadCall<T>(methodName) : default(T);
		}

		void SetValueIfAndroid(string methodName, params object[] args)
		{
			if (GoogleMapUtils.IsAndroid)
			{
				_ajo.MainThreadCallNonBlocking(methodName, args);
			}
		}

		void CheckIfRemoved()
		{
			if (_wasRemoved)
			{
				Debug.LogError("This marker was already removed from the map. You can't perform any more operations on it.");
			}
		}

		[PublicAPI]
		public override string ToString()
		{
			return
				$"[Marker: Id={Id}, Position={Position}, Alpha={Alpha}, Title={Title}, Snippet={Snippet}, DragStatus={IsDraggable}, IsVisible={IsVisible}, Flat={Flat}, Rotation={Rotation}, ZIndex={ZIndex}, IsInfoWindowShown={IsInfoWindowShown}]";
		}

#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
		[DllImport("__Internal")]
		static extern string _googleMapsViewMarkerGetId(IntPtr markerPtr);

		[DllImport("__Internal")]
		static extern string _googleMapsViewMarkerGetPosition(IntPtr markerPtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewMarkerSetPosition(IntPtr markerPtr, double lat, double lng);

		[DllImport("__Internal")]
		static extern float _googleMapsViewMarkerGetTransparency(IntPtr markerPtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewMarkerSetTransparency(IntPtr markerPtr, float transparency);

		[DllImport("__Internal")]
		static extern void _googleMapsViewMarkerSetAnchor(IntPtr markerPtr, float x, float y);

		[DllImport("__Internal")]
		static extern void _googleMapsViewMarkerSetInfoWindowAnchor(IntPtr markerPtr, float x, float y);

		[DllImport("__Internal")]
		static extern string _googleMapsViewMarkerGetTitle(IntPtr markerPtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewMarkerSetTitle(IntPtr markerPtr, string title);

		[DllImport("__Internal")]
		static extern string _googleMapsViewMarkerGetSnippet(IntPtr markerPtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewMarkerSetSnippet(IntPtr markerPtr, string snippet);

		[DllImport("__Internal")]
		static extern void _googleMapsViewMarkerSetIcon(IntPtr markerPtr, string imageDescriptor);

		[DllImport("__Internal")]
		static extern bool _googleMapsViewMarkerIsDraggable(IntPtr markerPtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewMarkerSetDraggable(IntPtr markerPtr, bool draggable);

		[DllImport("__Internal")]
		static extern bool _googleMapsViewMarkerIsVisible(IntPtr markerPtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewMarkerSetVisible(IntPtr markerPtr, IntPtr mapPtr, bool visible);

		[DllImport("__Internal")]
		static extern bool _googleMapsViewMarkerIsFlat(IntPtr markerPtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewMarkerSetFlat(IntPtr markerPtr, bool isFlat);

		[DllImport("__Internal")]
		static extern double _googleMapsViewMarkerGetRotation(IntPtr markerPtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewMarkerSetRotation(IntPtr markerPtr, double rotation);

		[DllImport("__Internal")]
		static extern void _googleMapsViewMarkerShowInfoWindow(IntPtr markerPtr, IntPtr mapPtr);
	
		[DllImport("__Internal")]
		static extern void _googleMapsViewMarkerHideInfoWindow(IntPtr mapPtr);
	
		[DllImport("__Internal")]
		static extern bool _googleMapsViewMarkerIsInfoWindowShown(IntPtr markerPtr, IntPtr mapPtr);

		[DllImport("__Internal")]
		static extern int _googleMapsViewMarkerGetZIndex(IntPtr markerPtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewMarkerSetZIndex(IntPtr markerPtr, int zIndex);

		[DllImport("__Internal")]
		static extern void _googleMapsViewMarkerRemove(IntPtr markerPtr);
#endif
	}
}