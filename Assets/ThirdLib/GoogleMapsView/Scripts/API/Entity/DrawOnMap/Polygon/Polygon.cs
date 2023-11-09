namespace NinevaStudios.GoogleMaps
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Linq;
	using System.Runtime.InteropServices;
	using System.Text;
	using Internal;
	using JetBrains.Annotations;
	using DeadMosquito.JniToolkit;
	using MiniJSON;
	using UnityEngine;

	/// <summary>
	/// A polygon on the earth's surface. A polygon can be convex or concave, it may span the 180 meridian and it can have holes that are not filled in. It has the following properties:
	/// 
	/// See: https://developers.google.com/android/reference/com/google/android/gms/maps/model/Polygon
	/// </summary>
	[PublicAPI]
	[SuppressMessage("ReSharper", "RedundantDefaultMemberInitializer")]
	[SuppressMessage("ReSharper", "HeuristicUnreachableCode")]
	[SuppressMessage("ReSharper", "RedundantNameQualifier")]
	public sealed class Polygon
	{
		bool _wasRemoved = false;
		readonly AndroidJavaObject _ajo;
		
#pragma warning disable 0414
		readonly IntPtr _polygonPtr;
		readonly IntPtr _mapPtr;
#pragma warning restore 0414

		public Polygon(AndroidJavaObject ajo)
		{
			_ajo = ajo;
		}

		public Polygon()
		{
		}

		public Polygon(IntPtr polygonPtr, IntPtr mapPtr)
		{
			_polygonPtr = polygonPtr;
			_mapPtr = mapPtr;
		}
		
		/// <summary>
		/// Gets this polygons's id. The id will be unique amongst all Polygons on a map.
		/// </summary>
		/// <value>The polygon identifier.</value>
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
				return _googleMapsViewPolygonGetId(_polygonPtr);
#endif

#pragma warning disable 0162
				return GetValueIfAndroid<string>("getId");
#pragma warning restore 0162
			}
		}

		/// <summary>
		/// The outline is specified by a list of vertices in clockwise or counterclockwise order. 
		/// It is not necessary for the start and end points to coincide; if they do not, the polygon will be automatically closed. 
		/// Line segments are drawn between consecutive points in the shorter of the two directions (east or west).
		/// </summary>
		[PublicAPI]
		public List<LatLng> Points
		{
			set
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return;
				}
				
				CheckIfRemoved();

 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				var pointsJson = Json.Serialize(LatLng.ToJsonList(value));
				_googleMapsViewPolygonSetPoints(_polygonPtr, pointsJson);
#endif
				
				if (GoogleMapUtils.IsAndroid)
				{
					var listAJO = value.ToJavaList(latLng => latLng.ToAJO());
					SetValueIfAndroid("setPoints", listAJO);
				}
			}
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return new List<LatLng>();
				}
				
				CheckIfRemoved();

 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				var pointsJson = _googleMapsViewPolygonGetPoints(_polygonPtr);
				return LatLng.ListFromJson(pointsJson);
#endif
				
#pragma warning disable 0162
				var listAJO = GetValueIfAndroid<AndroidJavaObject>("getPoints");
				return listAJO.FromJavaList<LatLng>(LatLng.FromAJO);
#pragma warning restore 0162
			}
		}

		/// <summary>
		/// Gets/sets holes of the polygon
		/// </summary>
		[PublicAPI]
		public List<List<LatLng>> Holes
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return new List<List<LatLng>>();
				}
				
				CheckIfRemoved();

 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				var holesJson = _googleMapsViewPolygonGetHoles(_polygonPtr);
				return LatLng.HolesListFromJson(holesJson);
#endif

#pragma warning disable 0162
				if (GoogleMapUtils.IsAndroid)
				{
					var listAJO = GetValueIfAndroid<AndroidJavaObject>("getHoles");
					var holesAJO = listAJO.FromJavaList<AndroidJavaObject>();
					return holesAJO.Select(holeAJO => holeAJO.FromJavaList<LatLng>(LatLng.FromAJO)).ToList();
				}
				
				return new List<List<LatLng>>();
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
				value = value ?? new List<List<LatLng>>(); 
				_googleMapsViewPolygonSetHoles(_polygonPtr, Json.Serialize(LatLng.ToJsonList(value)));
#endif

				if (GoogleMapUtils.IsAndroid)
				{
					var holesAJO = value.ToJavaList(holes => holes.ToJavaList(latLng => latLng.ToAJO()));
					SetValueIfAndroid("setHoles", holesAJO);
				}
			}
		}

		/// <summary>
		/// Gets/sets the polygon stroke width
		/// </summary>
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
				return _googleMapsViewPolygonGetStrokeWidth(_polygonPtr);
#endif
				
#pragma warning disable 0162
				return GetValueIfAndroid<float>("getStrokeWidth");
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
				_googleMapsViewPolygonSetStrokeWidth(_polygonPtr, value);
#endif
				
				SetValueIfAndroid("setStrokeWidth", value);
			}
		}

		/// <summary>
		/// Gets/sets the polygon stroke color
		/// </summary>
		[PublicAPI]
		public Color StrokeColor
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return UnityEngine.Color.magenta;
				}
				
				CheckIfRemoved();

 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				return _googleMapsViewPolygonGetStrokeColor(_polygonPtr).ColorFromJson();
#endif
				
#pragma warning disable 0162
				return GetValueIfAndroid<int>("getStrokeColor").ToUnityColor();
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
				_googleMapsViewPolygonSetStrokeColor(_polygonPtr, value.ToFloatArr());
#endif
				
				SetValueIfAndroid("setStrokeColor", value.ToAndroidColor());
			}
		}

		[PublicAPI]
		[Obsolete("Use StrokeColor")]
		public Color Color
		{
			get => StrokeColor;
			set => StrokeColor = value;
		}

		/// <summary>
		/// Gets/sets the polygon fill color
		/// </summary>
		[PublicAPI]
		public Color FillColor
		{
			get
			{
				if (GoogleMapUtils.IsPlatformNotSupported)
				{
					return UnityEngine.Color.magenta;
				}
				
				CheckIfRemoved();

 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
				return _googleMapsViewPolygonGetFillColor(_polygonPtr).ColorFromJson();
#endif
				
#pragma warning disable 0162
				return GetValueIfAndroid<int>("getFillColor").ToUnityColor();
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
				_googleMapsViewPolygonSetFillColor(_polygonPtr, value.ToFloatArr());
#endif
				
				SetValueIfAndroid("setFillColor", value.ToAndroidColor());
			}
		}

		/// <summary>
		/// Gets/sets the joint type for all vertices of the polygon's outline.
		/// </summary>
		[PublicAPI]
		[GoogleMapsAndroidOnly]
		public JointType StrokeJointType
		{
			get
			{
				if (GoogleMapUtils.IsNotAndroid)
				{
					return JointType.Default;
				}
				
				CheckIfRemoved();
				
				return (JointType) GetValueIfAndroid<int>("getStrokeJointType");
			}
			set
			{
				if (GoogleMapUtils.IsNotAndroid)
				{
					return;
				}
				
				CheckIfRemoved();
				
				SetValueIfAndroid("setStrokeJointType", (int) value);
			}
		}

		/// <summary>
		/// Gets or sets the index of the Z. Polygons with higher zIndices are drawn above those with lower indices.
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
				return _googleMapsViewPolygonGetZIndex(_polygonPtr);
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
				_googleMapsViewPolygonSetZIndex(_polygonPtr, (int) value);
#endif
				
				SetValueIfAndroid("setZIndex", value);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this polygon is visible.
		/// If this polygon is not visible then it will not be drawn. All other state is preserved. Polygons are visible by default.
		/// </summary>
		/// <value><c>true</c> if this polygon is visible; otherwise, <c>false</c>.</value>
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
				return _googleMapsViewPolygonIsVisible(_polygonPtr);
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
				_googleMapsViewPolygonSetVisible(_polygonPtr, _mapPtr, value);
#endif
				
				SetValueIfAndroid("setVisible", value);
			}
		}

		/// <summary>
		/// Indicates whether the segments of the polygon should be drawn as geodesics, as opposed to straight lines on the Mercator projection. 
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
				return _googleMapsViewPolygonIsGeodesic(_polygonPtr);
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
				_googleMapsViewPolygonSetGeodesic(_polygonPtr, value);
#endif
				
				SetValueIfAndroid("setGeodesic", value);
			}
		}

		/// <summary>
		/// Gets/sets the clickability of the polygon.
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
				return _googleMapsViewPolygonIsClickable(_polygonPtr);
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
				_googleMapsViewPolygonSetClickable(_polygonPtr, value);
#endif
				
				SetValueIfAndroid("setClickable", value);
			}
		}

		/// <summary>
		/// Removes this polygon from the map.
		/// </summary>
		[PublicAPI]
		public void Remove()
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return;
			}
			
			if (GoogleMapUtils.IsAndroid)
			{
				_ajo.MainThreadCall("remove");
			}
			
			CheckIfRemoved();

 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			_googleMapsViewPolygonRemove(_polygonPtr);
#endif

			_wasRemoved = true;
		}

		T GetValueIfAndroid<T>(string methodName)
		{
			CheckIfRemoved();
			return GoogleMapUtils.IsAndroid ? _ajo.MainThreadCall<T>(methodName) : default(T);
		}

		void SetValueIfAndroid(string methodName, params object[] args)
		{
			CheckIfRemoved();
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
					"This polygon was already removed from the map. You can't perform any more operations on it.");
			}
		}

		public override string ToString()
		{
			var pointsStr = Points.ConvertAll(latLng => latLng.ToString());
			var points = pointsStr.Aggregate((acc, next) => acc + ", " + next);

			var holesStringBuilder = new StringBuilder();
			Holes.ForEach(hole => hole.ForEach(point => holesStringBuilder.Append(point + ", ")));

			return
				$"[Polygon: Id={Id}, Points={points}, Holes={holesStringBuilder.ToString()}, StrokeWidth={StrokeWidth}, Color={StrokeColor}, FillColor={FillColor}, StrokeJointType={StrokeJointType}, ZIndex={ZIndex}, IsVisible={IsVisible}, IsGeodesic={IsGeodesic}, IsClickable={IsClickable}]";
		}
		
 #if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
		[DllImport("__Internal")]
		static extern string _googleMapsViewPolygonGetId(IntPtr polygonPtr);

		[DllImport("__Internal")]
		static extern string _googleMapsViewPolygonGetPoints(IntPtr polygonPtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewPolygonSetPoints(IntPtr polygonPtr, string pointsList);
		
		[DllImport("__Internal")]
		static extern string _googleMapsViewPolygonGetHoles(IntPtr polygonPtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewPolygonSetHoles(IntPtr polygonPtr, string holes);

		[DllImport("__Internal")]
		static extern string _googleMapsViewPolygonGetStrokeColor(IntPtr polygonPtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewPolygonSetStrokeColor(IntPtr polygonPtr, float[] color);
		
		[DllImport("__Internal")]
		static extern string _googleMapsViewPolygonGetFillColor(IntPtr polygonPtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewPolygonSetFillColor(IntPtr polygonPtr, float[] color);

		[DllImport("__Internal")]
		static extern float _googleMapsViewPolygonGetStrokeWidth(IntPtr polygonPtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewPolygonSetStrokeWidth(IntPtr polygonPtr, float width);

		[DllImport("__Internal")]
		static extern int _googleMapsViewPolygonGetZIndex(IntPtr polygonPtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewPolygonSetZIndex(IntPtr polygonPtr, int zIndex);

		[DllImport("__Internal")]
		static extern bool _googleMapsViewPolygonIsVisible(IntPtr polygonPtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewPolygonSetVisible(IntPtr circlerPtr, IntPtr mapPtr, bool visible);

		[DllImport("__Internal")]
		static extern bool _googleMapsViewPolygonIsGeodesic(IntPtr polygonPtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewPolygonSetGeodesic(IntPtr circlerPtr, bool isGeodesic);

		[DllImport("__Internal")]
		static extern bool _googleMapsViewPolygonIsClickable(IntPtr polygonPtr);

		[DllImport("__Internal")]
		static extern void _googleMapsViewPolygonSetClickable(IntPtr polygonPtr, bool clickable);

		[DllImport("__Internal")]
		static extern void _googleMapsViewPolygonRemove(IntPtr polygonPtr);
#endif
	}
}