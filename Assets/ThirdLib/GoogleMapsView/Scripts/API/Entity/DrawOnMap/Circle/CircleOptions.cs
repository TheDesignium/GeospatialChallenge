namespace NinevaStudios.GoogleMaps
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics.CodeAnalysis;
	using Internal;
	using JetBrains.Annotations;
	using DeadMosquito.JniToolkit;
	using UnityEngine;

	/// <summary>
	/// Circle options. 
	/// 
	/// For detailed documentation please see https://developers.google.com/android/reference/com/google/android/gms/maps/model/CircleOptions.html
	/// </summary>
	[PublicAPI]
	[SuppressMessage("ReSharper", "RedundantDefaultMemberInitializer")]
	public sealed class CircleOptions
	{
		const string CircleOptionsClass = "com.google.android.gms.maps.model.CircleOptions";
		const string CenterMethodName = "center";
		const string RadiusMethodName = "radius";
		const string StrokeWidthMethodName = "strokeWidth";
		const string StrokeColorMethodName = "strokeColor";
		const string FillColorMethodName = "fillColor";
		const string ZIndexMethodName = "zIndex";
		const string IsVisibleMethodName = "visible";
		const string IsClickableMethodName = "clickable";

		LatLng _latLng = new LatLng(0, 0);
		double _radius = 0.0D;
		float _strokeWidth = GoogleMapUtils.IsAndroid ? 10.0F : 1.0f;
		Color _strokeColor = Color.black;
		Color _fillColor = Color.clear;
		float _zIndex = 0F;
		bool _visible = true;
		bool _clickable = false;

		/// <summary>
		/// Sets the center using a <see cref="LatLng"/>.
		/// The center must not be null. This method is mandatory because there is no default center.
		/// </summary>
		/// <param name="latLng">The geographic center as a <see cref="LatLng"/></param>
		[PublicAPI]
		public CircleOptions Center(LatLng latLng)
		{
			if (latLng == null)
			{
				throw new ArgumentNullException(nameof(latLng));
			}

			_latLng = latLng;
			return this;
		}

		/// <summary>
		/// Sets the radius in meters.
		/// The radius must be zero or greater. The default radius is zero.
		/// </summary>
		/// <param name="radius">Circle radius.</param>
		[PublicAPI]
		public CircleOptions Radius(double radius)
		{
			_radius = radius;
			return this;
		}

		/// <summary>
		/// Sets the stroke width.
		// The stroke width is the width (in screen pixels) of the circle's outline. It must be zero or greater. If it is zero then no outline is drawn.
		/// The default width is 10 pixels.
		/// </summary>
		/// <param name="strokeWidth">Stroke width.</param>
		[PublicAPI]
		public CircleOptions StrokeWidth(float strokeWidth)
		{
			_strokeWidth = strokeWidth;
			return this;
		}

		/// <summary>
		/// Sets the stroke color.
		// The stroke color is the color of this circle's outline. If TRANSPARENT is used then no outline is drawn.
		// By default the stroke color is black.
		/// </summary>
		/// <param name="strokeColor">Stroke color.</param>
		[PublicAPI]
		public CircleOptions StrokeColor(Color strokeColor)
		{
			_strokeColor = strokeColor;
			return this;
		}

		/// <summary>
		/// Sets the fill color.
		/// The fill color is the color inside the circle. If TRANSPARENT is used then no fill is drawn.
		/// By default the fill color is transparent.
		/// </summary>
		/// <param name="fillColor">Fill color.</param>
		[PublicAPI]
		public CircleOptions FillColor(Color fillColor)
		{
			_fillColor = fillColor;
			return this;
		}

		/// <summary>
		/// Sets the zIndex.
		// Overlays (such as circles) with higher zIndices are drawn above those with lower indices.
		// By default the zIndex is 0.0
		/// </summary>
		/// <param name="zIndex">Z index.</param>
		[PublicAPI]
		public CircleOptions ZIndex(float zIndex)
		{
			_zIndex = zIndex;
			return this;
		}

		/// <summary>
		/// Sets the visibility.
		/// If this circle is not visible then it is not drawn, but all other state is preserved.
		/// </summary>
		/// <param name="visible">If set to <c>true</c> visible.</param>
		[PublicAPI]
		public CircleOptions Visible(bool visible)
		{
			_visible = visible;
			return this;
		}

		/// <summary>
		/// Specifies whether this circle is clickable. The default setting is <code>false</code>.
		/// </summary>
		/// <param name="clickable">Whether the circle is cickable.</param>
		[PublicAPI]
		public CircleOptions Clickable(bool clickable)
		{
			_clickable = clickable;
			return this;
		}

		public AndroidJavaObject ToAJO()
		{
			if (GoogleMapUtils.IsNotAndroid)
			{
				return null;
			}

			var ajo = new AndroidJavaObject(CircleOptionsClass);
			
			ajo.CallAJO(CenterMethodName, _latLng.ToAJO());
			ajo.CallAJO(RadiusMethodName, _radius);
			ajo.CallAJO(StrokeWidthMethodName, _strokeWidth);
			ajo.CallAJO(StrokeColorMethodName, _strokeColor.ToAndroidColor());
			ajo.CallAJO(FillColorMethodName, _fillColor.ToAndroidColor());
			ajo.CallAJO(ZIndexMethodName, _zIndex);
			ajo.CallAJO(IsVisibleMethodName, _visible);
			ajo.CallAJO(IsClickableMethodName, _clickable);

			return ajo;
		}

		public Dictionary<string, object> ToDictionary()
		{
			var result = new Dictionary<string, object>
			{
				[CenterMethodName] = _latLng.ToDictionary(),
				[RadiusMethodName] = _radius,
				[StrokeWidthMethodName] = _strokeWidth,
				[StrokeColorMethodName] = _strokeColor.ToDictionary(),
				[FillColorMethodName] = _fillColor.ToDictionary(),
				[ZIndexMethodName] = _zIndex,
				[IsVisibleMethodName] = _visible,
				[IsClickableMethodName] = _clickable
			};

			return result;
		}
	}
}