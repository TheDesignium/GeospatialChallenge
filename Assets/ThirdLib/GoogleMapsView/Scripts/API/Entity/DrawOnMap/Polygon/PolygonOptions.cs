namespace NinevaStudios.GoogleMaps
{
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using System.Linq;
	using Internal;
	using JetBrains.Annotations;
	using DeadMosquito.JniToolkit;
	using UnityEngine;

	/// <summary>
	/// Defines options for a polygon.
	/// 
	/// See: https://developers.google.com/android/reference/com/google/android/gms/maps/model/PolygonOptions
	/// </summary>
	[PublicAPI]
	[SuppressMessage("ReSharper", "RedundantDefaultMemberInitializer")]
	public sealed class PolygonOptions
	{
		const string PolygonOptionsClass = "com.google.android.gms.maps.model.PolygonOptions";
		const string ZIndexMethodName = "zIndex";
		const string IsVisibleMethodName = "visible";
		const string IsClickableMethodName = "clickable";
		const string FillColorMethodName = "fillColor";
		const string StrokeColorMethodName = "strokeColor";
		const string StrokeWidthMethodName = "strokeWidth";
		const string IsGeodesicMethodName = "geodesic";

		readonly List<LatLng> _points;
		readonly List<List<LatLng>> _holes;

		bool _clickable = false;
		Color _fillColor = Color.black;
		Color _strokeColor = Color.black;
		float _zIndex = 0f;
		bool _geodesic = false;
		bool _visible = true;
		JointType _strokeJointType = JointType.Default;
		float _strokeWidth = 10f;

		[PublicAPI]
		public PolygonOptions()
		{
			_points = new List<LatLng>();
			_holes = new List<List<LatLng>>();
		}

		/// <summary>
		/// Adds vertices to the end of the polygon being built.
		/// </summary>
		[PublicAPI]
		public PolygonOptions Add(params LatLng[] points)
		{
			_points.AddRange(points);
			return this;
		}

		/// <summary>
		/// Adds a vertex to the end of the polygon being built.
		/// </summary>
		[PublicAPI]
		public PolygonOptions Add(LatLng point)
		{
			_points.Add(point);
			return this;
		}

		/// <summary>
		/// Adds vertices to the end of the polygon being built.
		/// </summary>
		[PublicAPI]
		public PolygonOptions Add(IEnumerable<LatLng> points)
		{
			_points.AddRange(points);
			return this;
		}

		/// <summary>
		/// Adds a hole to the polygon being built.
		/// </summary>
		/// <param name="points">Hole to add to the polygon</param>
		[PublicAPI]
		public PolygonOptions AddHole(IEnumerable<LatLng> points)
		{
			_holes.Add(points.ToList());
			return this;
		}

		/// <summary>
		/// Specifies whether this polygon is clickable. The default setting is <code>false</code>
		/// </summary>
		[PublicAPI]
		public PolygonOptions Clickable(bool clickable)
		{
			_clickable = clickable;
			return this;
		}

		/// <summary>
		/// Sets the fill color of the polygon. The default color is black <see cref="UnityEngine.Color.black"/>.
		/// </summary>
		[PublicAPI]
		public PolygonOptions FillColor(Color color)
		{
			_fillColor = color;
			return this;
		}

		/// <summary>
		/// Sets the stroke color of the polygon. The default color is black <see cref="UnityEngine.Color.black"/>.
		/// </summary>
		[PublicAPI]
		public PolygonOptions StrokeColor(Color color)
		{
			_strokeColor = color;
			return this;
		}

		/// <summary>
		/// Specifies the polygon's stroke width, in display pixels. The default width is 10.
		/// </summary>
		[PublicAPI]
		public PolygonOptions StrokeWidth(float width)
		{
			_strokeWidth = width;
			return this;
		}

		/// <summary>
		/// Specifies whether to draw each segment of this polygon as a geodesic. The default setting is <code>false</code>
		/// </summary>
		[PublicAPI]
		public PolygonOptions Geodesic(bool geodesic)
		{
			_geodesic = geodesic;
			return this;
		}

		/// <summary>
		/// Specifies the joint type for all vertices of the polygon's outline.
		/// 
		/// See <see cref="JointType"/> for allowed values. The default value <see cref="JointType.Default"/> will be used if joint type is undefined or is not one of the allowed values.
		/// </summary>
		[PublicAPI]
		public PolygonOptions StrokeJointType(JointType jointType)
		{
			_strokeJointType = jointType;
			return this;
		}

		/// <summary>
		/// Specifies the visibility for the polygon. The default visibility is <code>true</code>.
		/// </summary>
		[PublicAPI]
		public PolygonOptions Visible(bool visible)
		{
			_visible = visible;
			return this;
		}

		/// <summary>
		/// Specifies the polygon's zIndex, i.e., the order in which it will be drawn.
		/// </summary>
		[PublicAPI]
		public PolygonOptions ZIndex(float zIndex)
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

			var ajo = new AndroidJavaObject(PolygonOptionsClass);

			foreach (var point in _points)
			{
				ajo.CallAJO("add", point.ToAJO());
			}

			ajo.CallAJO(ZIndexMethodName, _zIndex);
			ajo.CallAJO(IsVisibleMethodName, _visible);
			ajo.CallAJO(IsClickableMethodName, _clickable);
			ajo.CallAJO(FillColorMethodName, _fillColor.ToAndroidColor());
			ajo.CallAJO(StrokeColorMethodName, _strokeColor.ToAndroidColor());
			ajo.CallAJO(StrokeWidthMethodName, _strokeWidth);
			ajo.CallAJO(IsGeodesicMethodName, _geodesic);
			ajo.CallAJO("strokeJointType", (int) _strokeJointType);

			foreach (var hole in _holes)
			{
				ajo.CallAJO("addHole", hole.ToJavaList(latLng => latLng.ToAJO()));
			}

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
				[FillColorMethodName] = _fillColor.ToDictionary(),
				[StrokeColorMethodName] = _strokeColor.ToDictionary(),
				[IsGeodesicMethodName] = _geodesic,
				[StrokeWidthMethodName] = _strokeWidth,
				["holes"] = LatLng.ToJsonList(_holes)
			};

			return result;
		}
	}
}