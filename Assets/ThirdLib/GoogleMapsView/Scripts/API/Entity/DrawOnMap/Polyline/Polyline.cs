namespace NinevaStudios.GoogleMaps
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Linq;
	using System.Runtime.InteropServices;
	using Internal;
	using JetBrains.Annotations;
	using DeadMosquito.JniToolkit;
	using MiniJSON;
	using UnityEngine;

	/// <summary>
	/// A polyline is a list of points, where line segments are drawn between consecutive points
	/// </summary>
	[PublicAPI]
	[SuppressMessage("ReSharper", "HeuristicUnreachableCode")]
	public sealed class Polyline
	{
		bool _wasRemoved;

		readonly AndroidJavaObject _ajo;
		
#pragma warning disable 0414
		readonly IntPtr _polylinePtr;
		readonly IntPtr _mapPtr;
#pragma warning restore 0414

		public Polyline(AndroidJavaObject ajo)
		{
			_ajo = ajo;
		}

		public Polyline(IntPtr polylinePtr, IntPtr mapPtr)
		{
			_polylinePtr = polylinePtr;
			_mapPtr = mapPtr;
		}

		[PublicAPI]
		public Polyline()
		{
		}

		/// <summary>
		/// Gets this polylines's id. The id will be unique amongst all Polylines on a map.
		/// </summary>
		/// <value>The polyline identifier.</value>
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
				return _googleMapsViewPolylineGetId(_polylinePtr);
#endif

#pragma warning disable 0162
				return GetValueIfAndroid<string>("getId");
#pragma warning restore 0162
			}
		}

		/// <summary>
		/// The vertices of the line. Line segments are drawn between consecutive points. A polyline is not closed by default; to form a closed polyline, the start and end points must be the same.
		/// </summary>
		[PublicAPI]
		public List<LatLng> Points
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return new List<LatLng>();
				}

				CheckIfRemoved();

 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				var pointsJson = _googleMapsViewPolylineGetPoints(_polylinePtr);
				return LatLng.ListFromJson(pointsJson);
#endif

#pragma warning disable 0162
				var listAJO = GetValueIfAndroid<AndroidJavaObject>("getPoints");
				return listAJO.FromJavaList<LatLng>(LatLng.FromAJO);
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
				var pointsJson = Json.Serialize(LatLng.ToJsonList(value));
				Debug.Log(pointsJson);
				_googleMapsViewPolylineSetPoints(_polylinePtr, pointsJson);
#endif

				if (GoogleMapUtils.IsAndroid)
				{
					var listAJO = value.ToJavaList(latLng => latLng.ToAJO());
					SetValueIfAndroid("setPoints", listAJO);
				}
			}
		}

		/// <summary>
		/// Gets/sets the polyline color
		/// </summary>
		[PublicAPI]
		public Color Color
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return Color.magenta;
				}

				CheckIfRemoved();

 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				return _googleMapsViewPolylineGetColor(_polylinePtr).ColorFromJson();
#endif

#pragma warning disable 0162
				return GetValueIfAndroid<int>("getColor").ToUnityColor();
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
				_googleMapsViewPolylineSetColor(_polylinePtr, value.ToFloatArr());
#endif

				SetValueIfAndroid("setColor", value.ToAndroidColor());
			}
		}

		/// <summary>
		/// Gets/sets the polyline width
		/// </summary>
		[PublicAPI]
		public float Width
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return 0;
				}

				CheckIfRemoved();

 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				return _googleMapsViewPolylineGetWidth(_polylinePtr);
#endif

#pragma warning disable 0162
				return GetValueIfAndroid<float>("getWidth");
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
				_googleMapsViewPolylineSetWidth(_polylinePtr, value);
#endif

				SetValueIfAndroid("setWidth", value);
			}
		}

		/// <summary>
		/// 
		/// NOTE: This property has no effect on iOS
		/// 
		/// Defines the shape to be used at the start of a polyline. Supported cap types: <see cref="ButtCap"/>, <see cref="SquareCap"/>, <see cref="RoundCap"/> (applicable for solid stroke pattern) and <see cref="CustomCap"/> (applicable for any stroke pattern). 
		/// Default for both start and end: <see cref="ButtCap"/>.
		/// </summary>
		[PublicAPI]
		[GoogleMapsAndroidOnly]
		public Cap StartCap
		{
			set
			{
				if (GoogleMapUtils.IsNotAndroid)
				{
					return;
				}

				CheckIfRemoved();

				SetValueIfAndroid("setStartCap", value.ToAJO());
			}
		}

		/// <summary>
		/// 
		/// NOTE: This property has no effect on iOS
		/// 
		/// Defines the shape to be used at the end of a polyline. Supported cap types: <see cref="ButtCap"/>, <see cref="SquareCap"/>, <see cref="RoundCap"/> (applicable for solid stroke pattern) and <see cref="CustomCap"/> (applicable for any stroke pattern). 
		/// Default for both start and end: <see cref="ButtCap"/>.
		/// </summary>
		[PublicAPI]
		[GoogleMapsAndroidOnly]
		public Cap EndCap
		{
			set
			{
				if (GoogleMapUtils.IsNotAndroid)
				{
					return;
				}

				CheckIfRemoved();

				SetValueIfAndroid("setStartCap", value.ToAJO());
			}
		}

		/// <summary>
		/// Sets/gets the joint type for all vertices of the polyline except the start and end vertices.
		/// </summary>
		[PublicAPI]
		[GoogleMapsAndroidOnly]
		public JointType JointType
		{
			get
			{
				if (GoogleMapUtils.IsNotAndroid)
				{
					return JointType.Default;
				}

				CheckIfRemoved();

				return (JointType) GetValueIfAndroid<int>("getJointType");
			}
			set
			{
				if (GoogleMapUtils.IsNotAndroid)
				{
					return;
				}

				CheckIfRemoved();

				SetValueIfAndroid("setJointType", (int) value);
			}
		}

		/// <summary>
		/// Gets or sets the index of the Z. Polylines with higher zIndices are drawn above those with lower indices.
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
				return _googleMapsViewPolylineGetZIndex(_polylinePtr);
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
				_googleMapsViewPolylineSetZIndex(_polylinePtr, (int) value);
#endif

				SetValueIfAndroid("setZIndex", value);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this polyline is visible.
		/// If this polyline is not visible then it will not be drawn. All other state is preserved. Polylines are visible by default.
		/// </summary>
		/// <value><c>true</c> if this polyline is visible; otherwise, <c>false</c>.</value>
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
				return _googleMapsViewPolylineIsVisible(_polylinePtr);
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
				_googleMapsViewPolylineSetVisible(_polylinePtr, _mapPtr, value);
#endif

				SetValueIfAndroid("setVisible", value);
			}
		}

		/// <summary>
		/// Indicates whether the segments of the polyline should be drawn as geodesics, as opposed to straight lines on the Mercator projection. 
		/// A geodesic is the shortest path between two points on the Earth's surface. The geodesic curve is constructed assuming the Earth is a sphere
		/// </summary>
		[PublicAPI]
		public bool IsGeodesic
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return false;
				}

				CheckIfRemoved();

 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				return _googleMapsViewPolylineIsGeodesic(_polylinePtr);
#endif

#pragma warning disable 0162
				return GetValueIfAndroid<bool>("isGeodesic");
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
				_googleMapsViewPolylineSetGeodesic(_polylinePtr, value);
#endif

				SetValueIfAndroid("setGeodesic", value);
			}
		}

		/// <summary>
		/// Gets/sets the clickability of the polyline.
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
				return _googleMapsViewPolylineIsClickable(_polylinePtr);
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
				_googleMapsViewPolylineSetClickable(_polylinePtr, value);
#endif

				SetValueIfAndroid("setClickable", value);
			}
		}

		/// <summary>
		/// Removes this polyline from the map.
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
			_googleMapsViewPolylineRemove(_polylinePtr);
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
					"This polyline was already removed from the map. You can't perform any more operations on it.");
			}
		}

		public override string ToString()
		{
			var pointsStr = Points.ConvertAll(latLng => latLng.ToString());
			var points = pointsStr.Aggregate((acc, next) => acc + ", " + next);
			return
				$"[Polyline: Id={Id}, Points={points}, Color={Color}, Width={Width}, JointType={JointType}, ZIndex={ZIndex}, IsVisible={IsVisible}, IsGeodesic={IsGeodesic}, IsClickable={IsClickable}]";
		}

 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
		[DllImport("__Internal")]
		static extern string _googleMapsViewPolylineGetId(IntPtr polylinePtr);

		[DllImport("__Internal")]
		static extern string _googleMapsViewPolylineGetPoints(IntPtr polylinePtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewPolylineSetPoints(IntPtr polylinePtr, string pointsList);

		[DllImport("__Internal")]
		static extern string _googleMapsViewPolylineGetColor(IntPtr polylinePtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewPolylineSetColor(IntPtr polylinePtr, float[] color);

		[DllImport("__Internal")]
		static extern float _googleMapsViewPolylineGetWidth(IntPtr polylinePtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewPolylineSetWidth(IntPtr polylinePtr, float width);

		[DllImport("__Internal")]
		static extern int _googleMapsViewPolylineGetZIndex(IntPtr polylinePtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewPolylineSetZIndex(IntPtr polylinePtr, int zIndex);

		[DllImport("__Internal")]
		static extern bool _googleMapsViewPolylineIsVisible(IntPtr polylinePtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewPolylineSetVisible(IntPtr circlerPtr, IntPtr mapPtr, bool visible);

		[DllImport("__Internal")]
		static extern bool _googleMapsViewPolylineIsGeodesic(IntPtr polylinePtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewPolylineSetGeodesic(IntPtr circlerPtr, bool isGeodesic);

		[DllImport("__Internal")]
		static extern bool _googleMapsViewPolylineIsClickable(IntPtr polylinePtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewPolylineSetClickable(IntPtr polylinePtr, bool clickable);

		[DllImport("__Internal")]
		static extern void _googleMapsViewPolylineRemove(IntPtr polylinePtr);
#endif
	}
}