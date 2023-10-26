namespace NinevaStudios.GoogleMaps
{
	using System;
	using System.Diagnostics.CodeAnalysis;
	using System.Runtime.InteropServices;
	using Internal;
	using JetBrains.Annotations;
	using DeadMosquito.JniToolkit;
	using MiniJSON;
	using UnityEngine;

	/// <summary>
	/// A ground overlay is an image that is fixed to a map. 
	/// 
	/// For more details visit: https://developers.google.com/android/reference/com/google/android/gms/maps/model/GroundOverlay
	/// </summary>
	[PublicAPI]
	[SuppressMessage("ReSharper", "HeuristicUnreachableCode")]
	public sealed class GroundOverlay
	{
		bool _wasRemoved;

		readonly AndroidJavaObject _ajo;

#pragma warning disable 0414
		readonly IntPtr _overlayPtr;
		readonly IntPtr _mapPtr;
#pragma warning restore 0414

		public GroundOverlay(AndroidJavaObject ajo)
		{
			_ajo = ajo;
		}

		public GroundOverlay()
		{
		}

		public GroundOverlay(IntPtr overlayPtr, IntPtr mapPtr)
		{
			_overlayPtr = overlayPtr;
			_mapPtr = mapPtr;
		}

		/// <summary>
		/// Gets this ground overlay's id. The id will be unique amongst all GroundOverlays on a map.
		/// </summary>
		/// <value>The ground overlay identifier.</value>
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
				return _googleMapsViewOverlayGetId(_overlayPtr);
#endif

#pragma warning disable 0162
				return GetValueIfAndroid<string>("getId");
#pragma warning restore 0162
			}
		}

		/// <summary>
		/// Gets the height of the ground overlay.
		/// </summary>
		[PublicAPI]
		[GoogleMapsAndroidOnly]
		public float Height
		{
			get
			{
				if (GoogleMapUtils.IsNotAndroid)
				{
					return 0f;
				}

				return GetValueIfAndroid<float>("getHeight");
			}
		}

		/// <summary>
		/// Gets the width of the ground overlay.
		/// </summary>
		[PublicAPI]
		[GoogleMapsAndroidOnly]
		public float Width
		{
			get
			{
				if (GoogleMapUtils.IsNotAndroid)
				{
					return 0f;
				}

				return GetValueIfAndroid<float>("getWidth");
			}
		}

		/// <summary>
		/// Gets/sets the bearing of the ground overlay (the direction that the vertical axis of the ground overlay points) in degrees clockwise from north. 
		/// The rotation is performed about the anchor point.
		/// </summary>
		[PublicAPI]
		public float Bearing
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return 0;
				}

				CheckIfRemoved();

 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				return (float) _googleMapsViewOverlayGetBearing(_overlayPtr);
#endif

#pragma warning disable 0162
				return GetValueIfAndroid<float>("getBearing");
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
				_googleMapsViewOverlaySetBearing(_overlayPtr, value);
#endif

				SetValueIfAndroid("setBearing", value);
			}
		}

		/// <summary>
		/// Gets the bounds for the ground overlay. This ignores the rotation of the ground overlay.
		/// </summary>
		[PublicAPI]
		public LatLngBounds Bounds
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return LatLngBounds.Zero;
				}

				CheckIfRemoved();

 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				var boundsJson = _googleMapsViewOverlayGetBounds(_overlayPtr);
				return LatLngBounds.FromJson(boundsJson);
#endif

#pragma warning disable 0162
				return LatLngBounds.FromAJO(GetValueIfAndroid<AndroidJavaObject>("getBounds"));
#pragma warning restore 0162
			}
			set
			{
				SetPositionFromBounds(value);
			}
		}

		/// <summary>
		/// Returns the position as a <see cref="LatLng"/>.
		/// </summary>
		/// <value>The geographic position as a <see cref="LatLng"/></value>
		[PublicAPI]
		public LatLng Position
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return LatLng.Zero;
				}

				CheckIfRemoved();

 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				var positionJson = _googleMapsViewOverlayGetPosition(_overlayPtr);
				return LatLng.FromJson(positionJson);
#endif

#pragma warning disable 0162
				return LatLng.FromAJO(GetValueIfAndroid<AndroidJavaObject>("getPosition"));
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
				_googleMapsViewOverlaySetPosition(_overlayPtr, value.Latitude, value.Longitude);
#endif
				
				SetValueIfAndroid("setPosition", value.ToAJO());
			}
		}

		/// <summary>
		/// Gets/sets the transparency of this ground overlay.
		/// Transparency of the ground overlay in the range [0..1] where 0 means the overlay is opaque and 1 means the overlay is fully transparent.
		/// </summary>
		/// <value>The transparency of the marker.</value>
		[PublicAPI]
		public float Transparency
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return 0;
				}

				CheckIfRemoved();

 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				return _googleMapsViewOverlayGetTransparency(_overlayPtr);
#endif

#pragma warning disable 0162
				return GetValueIfAndroid<float>("getTransparency");
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
				_googleMapsViewOverlaySetTransparency(_overlayPtr, value);
#endif
				
				SetValueIfAndroid("setTransparency", value);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this ground overlay is visible.
		/// If this ground overlay is not visible then it will not be drawn. All other state is preserved. Ground overlays are visible by default.
		/// </summary>
		/// <value><c>true</c> if this ground overlay is visible; otherwise, <c>false</c>.</value>
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
				return _googleMapsViewOverlayIsVisible(_overlayPtr);
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
				_googleMapsViewOverlaySetVisible(_overlayPtr, _mapPtr, value);
#endif
				
				SetValueIfAndroid("setVisible", value);
			}
		}

		/// <summary>
		/// Gets/sets the clickability of the ground overlay.
		/// </summary>
		[PublicAPI]
		public bool IsClickable
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return false;
				}

				CheckIfRemoved();

 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				return _googleMapsViewOverlayIsClickable(_overlayPtr);
#endif
				
#pragma warning disable 0162
				return GetValueIfAndroid<bool>("isClickable");
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
				_googleMapsViewOverlaySetClickable(_overlayPtr, value);
#endif
				
				SetValueIfAndroid("setClickable", value);
			}
		}

		/// <summary>
		/// Gets or sets the index of the Z. Overlays (such as circles) with higher zIndices are drawn above those with lower indices.
		/// </summary>
		/// <value>The index of the Z.</value>
		[PublicAPI]
		public float ZIndex
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return 0;
				}

				CheckIfRemoved();

 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				return _googleMapsViewOverlayGetZIndex(_overlayPtr);
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
				_googleMapsViewOverlaySetZIndex(_overlayPtr, (int) value);
#endif
				
				SetValueIfAndroid("setZIndex", value);
			}
		}

		/// <summary>
		/// Sets the width of the ground overlay. The height of the ground overlay will be adapted accordingly to preserve aspect ratio.
		/// </summary>
		/// <param name="width">width in meters</param>
		[PublicAPI]
		[GoogleMapsAndroidOnly]
		public void SetDimensions(float width)
		{
			if (GoogleMapUtils.IsNotAndroid)
			{
				return;
			}

			CheckIfRemoved();

			SetValueIfAndroid("setDimensions", width);
		}

		/// <summary>
		/// Sets the dimensions of the ground overlay. The image will be stretched to fit the dimensions.
		/// </summary>
		/// <param name="width">width in meters</param>
		/// <param name="height">height in meters</param>
		[PublicAPI]
		[GoogleMapsAndroidOnly]
		public void SetDimensions(float width, float height)
		{
			if (GoogleMapUtils.IsNotAndroid)
			{
				return;
			}

			CheckIfRemoved();

			SetValueIfAndroid("setDimensions", width, height);
		}

		/// <summary>
		/// Sets the image for the Ground Overlay. The new image will occupy the same bounds as the old image.
		/// </summary>
		/// <param name="imageDescriptor">Image descriptor</param>
		[PublicAPI]
		public void SetImage(ImageDescriptor imageDescriptor)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return;
			}

			CheckIfRemoved();

 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			if (string.IsNullOrEmpty(imageDescriptor.AssetName))
			{
				Debug.LogError("On iOS image descriptor asset name can't be null or empty");
				return;
			}

			_googleMapsViewOverlaySetImage(_overlayPtr, imageDescriptor.AssetName.ToFullStreamingAssetsPath());
#endif

			SetValueIfAndroid("setImage", imageDescriptor.ToAJO());
		}

		/// <summary>
		/// Sets the position of the ground overlay by fitting it to the given <see cref="LatLngBounds"/>. 
		/// This method will ignore the rotation (bearing) of the ground overlay when positioning it, but the bearing will still be used when drawing it.
		/// </summary>
		/// <param name="bounds">a <see cref="LatLngBounds"/> in which to place the ground overlay</param>
		[PublicAPI]
		public void SetPositionFromBounds(LatLngBounds bounds)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return;
			}
			
			CheckIfRemoved();

 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			_googleMapsViewOverlaySetBounds(_overlayPtr, Json.Serialize(bounds.ToDictionary()));
#endif

			SetValueIfAndroid("setPositionFromBounds", bounds.ToAJO());
		}

		/// <summary>
		/// Removes this circle from the map.
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
			_googleMapsViewOverlayRemove(_overlayPtr);
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
				Debug.LogError(
					"This circle was already removed from the map. You can't perform any more operations on it.");
			}
		}

		public override string ToString()
		{
			return
				$"[GroundOverlay: Id={Id}, Height={Height}, Width={Width}, Bearing={Bearing}, Bounds={Bounds}, Position={Position}, Transparency={Transparency}, IsVisible={IsVisible}, IsClickable={IsClickable}, ZIndex={ZIndex}]";
		}

 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
		[DllImport("__Internal")]
		static extern string _googleMapsViewOverlayGetId(IntPtr overlayPtr);

		[DllImport("__Internal")]
		static extern int _googleMapsViewOverlayGetZIndex(IntPtr overlayPtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewOverlaySetZIndex(IntPtr overlayPtr, int zIndex);

		[DllImport("__Internal")]
		static extern void _googleMapsViewOverlaySetImage(IntPtr overlayPtr, string imagePath);

		[DllImport("__Internal")]
		static extern double _googleMapsViewOverlayGetBearing(IntPtr overlayPtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewOverlaySetBearing(IntPtr overlayPtr, double bearing);

		[DllImport("__Internal")]
		static extern float _googleMapsViewOverlayGetTransparency(IntPtr overlayPtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewOverlaySetTransparency(IntPtr overlayPtr, float transparency);

		[DllImport("__Internal")]
		static extern string _googleMapsViewOverlayGetBounds(IntPtr overlayPtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewOverlaySetBounds(IntPtr overlayPtr, string bounds);

		[DllImport("__Internal")]
		static extern string _googleMapsViewOverlayGetPosition(IntPtr overlayPtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewOverlaySetPosition(IntPtr overlayPtr, double lat, double lng);

		[DllImport("__Internal")]
		static extern bool _googleMapsViewOverlayIsVisible(IntPtr overlayPtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewOverlaySetVisible(IntPtr overlayPtr, IntPtr mapPtr, bool visible);

		[DllImport("__Internal")]
		static extern bool _googleMapsViewOverlayIsClickable(IntPtr overlayPtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewOverlaySetClickable(IntPtr overlayPtr, bool clickable);

		[DllImport("__Internal")]
		static extern void _googleMapsViewOverlayRemove(IntPtr overlayPtr);
#endif
	}
}