namespace NinevaStudios.GoogleMaps
{
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using Internal;
	using JetBrains.Annotations;
	using DeadMosquito.JniToolkit;
	using UnityEngine;

	/// <summary>
	/// Defines PolylineOptions for a polyline.
	/// 
	/// Refer to https://developers.google.com/android/reference/com/google/android/gms/maps/model/PolylineOptions for more detailed documentation
	/// </summary>
	[PublicAPI]
	[SuppressMessage("ReSharper", "RedundantDefaultMemberInitializer")]
	public sealed class PolylineOptions
	{
		const string PolylineOptionsClass = "com.google.android.gms.maps.model.PolylineOptions";
		const string ZIndexMethodName = "zIndex";
		const string IsVisibleMethodName = "visible";
		const string IsClickableMethodName = "clickable";
		const string ColorMethodName = "color";
		const string IsGeodesicMethodName = "geodesic";
		const string WidthMethodName = "width";

		readonly List<LatLng> _points;
		bool _clickable = false;
		Color _color = UnityEngine.Color.black;
		Cap _startCap = new ButtCap();
		Cap _endCap = new ButtCap();
		bool _geodesic = false;
		bool _visible = true;
		float _width = 10f;
		float _zIndex = 0f;
		JointType _jointType = GoogleMaps.JointType.Default;

		[PublicAPI]
		public PolylineOptions()
		{
			_points = new List<LatLng>();
		}

		/// <summary>
		/// Adds vertices to the end of the polyline being built.
		/// </summary>
		[PublicAPI]
		public PolylineOptions Add(params LatLng[] points)
		{
			_points.AddRange(points);
			return this;
		}

		/// <summary>
		/// Adds a vertex to the end of the polyline being built.
		/// </summary>
		[PublicAPI]
		public PolylineOptions Add(LatLng point)
		{
			_points.Add(point);
			return this;
		}

		/// <summary>
		/// Adds vertices to the end of the polyline being built.
		/// </summary>
		[PublicAPI]
		public PolylineOptions Add(IEnumerable<LatLng> points)
		{
			_points.AddRange(points);
			return this;
		}

		/// <summary>
		/// Specifies whether this polyline is clickable. The default setting is <code>false</code>
		/// </summary>
		[PublicAPI]
		public PolylineOptions Clickable(bool clickable)
		{
			_clickable = clickable;
			return this;
		}

		/// <summary>
		/// Sets the color of the polyline. The default color is black <see cref="UnityEngine.Color.black"/>.
		/// </summary>
		[PublicAPI]
		public PolylineOptions Color(Color color)
		{
			_color = color;
			return this;
		}

		/// <summary>
		/// NOTE: Has no effect on iOS
		/// 
		/// Sets the cap at the start vertex of the polyline. The default start cap is <see cref="ButtCap"/>.
		/// </summary>
		[PublicAPI]
		[GoogleMapsAndroidOnly]
		public PolylineOptions StartCap(Cap startCap)
		{
			_startCap = startCap;
			return this;
		}

		/// <summary>
		/// NOTE: Has no effect on iOS
		/// 
		/// Sets the cap at the end vertex of the polyline. The default start cap is <see cref="ButtCap"/>.
		/// </summary>
		[PublicAPI]
		[GoogleMapsAndroidOnly]
		public PolylineOptions EndCap(Cap endCap)
		{
			_endCap = endCap;
			return this;
		}

		/// <summary>
		/// NOTE: Has no effect on iOS
		/// 
		/// Sets the joint type for all vertices of the polyline except the start and end vertices.
		/// 
		/// See <see cref="GoogleMaps.JointType"/> for allowed values. The default value <see cref="GoogleMaps.JointType.Default"/> will be used if joint type is undefined or is not one of the allowed values.
		/// </summary>
		[PublicAPI]
		[GoogleMapsAndroidOnly]
		public PolylineOptions JointType(JointType jointType)
		{
			_jointType = jointType;
			return this;
		}

		/// <summary>
		/// Specifies whether to draw each segment of this polyline as a geodesic. The default setting is <code>false</code>
		/// </summary>
		[PublicAPI]
		public PolylineOptions Geodesic(bool geodesic)
		{
			_geodesic = geodesic;
			return this;
		}

		/// <summary>
		/// Specifies the visibility for the polyline. The default visibility is <code>true</code>.
		/// </summary>
		[PublicAPI]
		public PolylineOptions Visible(bool visible)
		{
			_visible = visible;
			return this;
		}

		/// <summary>
		/// Sets the width of the polyline in screen pixels. The default is 10.
		/// </summary>
		[PublicAPI]
		public PolylineOptions Width(float width)
		{
			_width = width;
			return this;
		}

		/// <summary>
		/// Specifies the polyline's zIndex, i.e., the order in which it will be drawn.
		/// </summary>
		[PublicAPI]
		public PolylineOptions ZIndex(float zIndex)
		{
			_zIndex = zIndex;
			return this;
		}

		public AndroidJavaObject ToAJO()
		{
			if (GoogleMapUtils.IsNotAndroid)
			{
				return null;
			}

			var ajo = new AndroidJavaObject(PolylineOptionsClass);

			foreach (var point in _points)
			{
				ajo.CallAJO("add", point.ToAJO());
			}

			ajo.CallAJO("startCap", _startCap.ToAJO());
			ajo.CallAJO("endCap", _endCap.ToAJO());
			ajo.CallAJO("jointType", (int) _jointType);

			ajo.CallAJO(ZIndexMethodName, _zIndex);
			ajo.CallAJO(IsVisibleMethodName, _visible);
			ajo.CallAJO(IsClickableMethodName, _clickable);
			ajo.CallAJO(ColorMethodName, _color.ToAndroidColor());
			ajo.CallAJO(IsGeodesicMethodName, _geodesic);
			ajo.CallAJO(WidthMethodName, _width);

			return ajo;
		}
		
		public Dictionary<string, object> ToDictionary()
		{
			var result = new Dictionary<string, object>
			{
				["points"] = LatLng.ToJsonList(_points),
				[ZIndexMethodName] = _zIndex,
				[IsVisibleMethodName] = _visible,
				[IsClickableMethodName] = _clickable,
				[ColorMethodName] = _color.ToDictionary(),
				[IsGeodesicMethodName] = _geodesic,
				[WidthMethodName] = _width
			};
			return result;
		}
	}
}