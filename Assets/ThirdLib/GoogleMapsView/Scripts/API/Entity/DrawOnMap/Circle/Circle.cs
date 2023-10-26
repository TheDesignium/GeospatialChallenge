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
	/// A circle on the earth's surface (spherical cap).
	/// 
	/// For more details visit: https://developers.google.com/android/reference/com/google/android/gms/maps/model/Circle
	/// </summary>
	[PublicAPI]
	[SuppressMessage("ReSharper", "HeuristicUnreachableCode")]
	public sealed class Circle
	{
		bool _wasRemoved;

		public Circle()
		{
		}

		readonly AndroidJavaObject _ajo;
		
#pragma warning disable 0414
		readonly IntPtr _circlerPtr;
		readonly IntPtr _mapPtr;
#pragma warning restore 0414

		public Circle(IntPtr circlerPtr, IntPtr mapPtr)
		{
			_circlerPtr = circlerPtr;
			_mapPtr = mapPtr;
		}

		public Circle(AndroidJavaObject ajo)
		{
			_ajo = ajo;
		}

		/// <summary>
		/// Gets this circle's id. The id will be unique amongst all Circles on a map.
		/// </summary>
		/// <value>The circle identifier.</value>
		[PublicAPI]
		public string Id
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return null;
				}

				CheckIfRemoved();

 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				return _googleMapsViewCircleGetId(_circlerPtr);
#endif

#pragma warning disable 0162
				return GetAndroidValue<string>("getId");
#pragma warning restore 0162
				
			}
		}

		/// <summary>
		/// Returns the center as a <see cref="LatLng"/>.
		/// </summary>
		/// <value>The geographic center as a <see cref="LatLng"/></value>
		[PublicAPI]
		public LatLng Center
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return LatLng.Zero;
				}

				CheckIfRemoved();

 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				var jsonLatLng = _googleMapsViewCircleGetCenter(_circlerPtr);
				return LatLng.FromJson(jsonLatLng);
#endif

#pragma warning disable 0162
				var ajo = GetAndroidValue<AndroidJavaObject>("getCenter");
				return LatLng.FromAJO(ajo);
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
				_googleMapsViewCircleSetCenter(_circlerPtr, value.Latitude, value.Longitude);
#endif

				SetValueIfAndroid("setCenter", value.ToAJO());
			}
		}

		/// <summary>
		/// Gets or sets the color of the fill.
		/// </summary>
		/// <value>The color of the fill.</value>
		[PublicAPI]
		public Color FillColor
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return Color.magenta;
				}

				CheckIfRemoved();

 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				return _googleMapsViewCircleGetFillColor(_circlerPtr).ColorFromJson();
#endif

#pragma warning disable 0162
				return GetAndroidValue<int>("getFillColor").ToUnityColor();
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
				_googleMapsViewCircleSetFillColor(_circlerPtr, value.ToFloatArr());
#endif

				SetValueIfAndroid("setFillColor", value.ToAndroidColor());
			}
		}

		/// <summary>
		/// Gets or sets the color of the stroke.
		/// </summary>
		/// <value>The color of the stroke.</value>
		[PublicAPI]
		public Color StrokeColor
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return Color.magenta;
				}

				CheckIfRemoved();

 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				return _googleMapsViewCircleGetStrokeColor(_circlerPtr).ColorFromJson();
#endif

#pragma warning disable 0162
				return GetAndroidValue<int>("getStrokeColor").ToUnityColor();
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
				_googleMapsViewCircleSetStrokeColor(_circlerPtr, value.ToFloatArr());
#endif

				SetValueIfAndroid("setStrokeColor", value.ToAndroidColor());
			}
		}

		/// <summary>
		/// Gets or sets the width of the stroke.
		/// </summary>
		/// <value>The width of the stroke.</value>
		[PublicAPI]
		public float StrokeWidth
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return 0;
				}

				CheckIfRemoved();

 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				return _googleMapsViewCircleGetStrokeWidth(_circlerPtr);
#endif

#pragma warning disable 0162
				return GetAndroidValue<float>("getStrokeWidth");
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
				_googleMapsViewCircleSetStrokeWidth(_circlerPtr, value);
#endif

				SetValueIfAndroid("setStrokeWidth", value);
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
				return _googleMapsViewCircleGetZIndex(_circlerPtr);
#endif

#pragma warning disable 0162
				return GetAndroidValue<float>("getZIndex");
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
				_googleMapsViewCircleSetZIndex(_circlerPtr, (int) value);
#endif

				SetValueIfAndroid("setZIndex", value);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this circle is visible.
		/// If this circle is not visible then it will not be drawn. All other state is preserved. Circles are visible by default.
		/// </summary>
		/// <value><c>true</c> if this circle is visible; otherwise, <c>false</c>.</value>
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
				return _googleMapsViewCircleIsVisible(_circlerPtr);
#endif

#pragma warning disable 0162
				return GetAndroidValue<bool>("isVisible");
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
				_googleMapsViewCircleSetVisible(_circlerPtr, _mapPtr, value);
#endif

				SetValueIfAndroid("setVisible", value);
			}
		}

		/// <summary>
		/// Gets/sets the clickability of the circle.
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
				return _googleMapsViewCircleIsClickable(_circlerPtr);
#endif

#pragma warning disable 0162
				return GetAndroidValue<bool>("isClickable");
#pragma warning restore 0162
			}
			set
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return;
				}

 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				_googleMapsViewCircleSetClickable(_circlerPtr, value);
#endif

				SetValueIfAndroid("setClickable", value);
			}
		}

		/// <summary>
		/// Gets or sets the radius of the circle.
		/// </summary>
		/// <value>The radius of the circle.</value>
		[PublicAPI]
		public double Radius
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return 0;
				}

				CheckIfRemoved();

 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				return _googleMapsViewCircleGetRadius(_circlerPtr);
#endif

#pragma warning disable 0162
				return GetAndroidValue<double>("getRadius");
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
				_googleMapsViewCircleSetRadius(_circlerPtr, value);
#endif

				SetValueIfAndroid("setRadius", value);
			}
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

			if (GoogleMapUtils.IsAndroid)
			{
				_ajo.MainThreadCall("remove");
			}

 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			_googleMapsViewCircleCircleRemove(_circlerPtr);
#endif

			_wasRemoved = true;
		}

		T GetAndroidValue<T>(string methodName)
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
				Debug.LogError("This circle was already removed from the map. You can't perform any more operations on it.");
			}
		}

		public override string ToString()
		{
			return
				$"[Circle: Id={Id}, Center={Center}, FillColor={FillColor}, StrokeColor={StrokeColor}, StrokeWidth={StrokeWidth}, ZIndex={ZIndex}, IsVisible={IsVisible}, IsClickable={IsClickable}, Radius={Radius}]";
		}

 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
		[DllImport("__Internal")]
		static extern string _googleMapsViewCircleGetId(IntPtr circlePtr);

		[DllImport("__Internal")]
		static extern string _googleMapsViewCircleGetCenter(IntPtr circlePtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewCircleSetCenter(IntPtr circlePtr, double lat, double lng);

		[DllImport("__Internal")]
		static extern string _googleMapsViewCircleGetFillColor(IntPtr circlePtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewCircleSetFillColor(IntPtr circlePtr, float[] color);

		[DllImport("__Internal")]
		static extern string _googleMapsViewCircleGetStrokeColor(IntPtr circlePtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewCircleSetStrokeColor(IntPtr circlePtr, float[] color);

		[DllImport("__Internal")]
		static extern float _googleMapsViewCircleGetStrokeWidth(IntPtr circlePtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewCircleSetStrokeWidth(IntPtr circlePtr, float width);

		[DllImport("__Internal")]
		static extern int _googleMapsViewCircleGetZIndex(IntPtr circlePtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewCircleSetZIndex(IntPtr circlePtr, int zIndex);

		[DllImport("__Internal")]
		static extern bool _googleMapsViewCircleIsVisible(IntPtr circlePtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewCircleSetVisible(IntPtr circlerPtr, IntPtr mapPtr, bool visible);

		[DllImport("__Internal")]
		static extern bool _googleMapsViewCircleIsClickable(IntPtr circlePtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewCircleSetClickable(IntPtr circlePtr, bool clickable);

		[DllImport("__Internal")]
		static extern void _googleMapsViewCircleCircleRemove(IntPtr circlePtr);

		[DllImport("__Internal")]
		static extern double _googleMapsViewCircleGetRadius(IntPtr circlePtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewCircleSetRadius(IntPtr circlePtr, double radius);
#endif
	}
}