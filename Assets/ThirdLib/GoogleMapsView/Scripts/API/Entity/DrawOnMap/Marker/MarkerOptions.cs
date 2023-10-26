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
	/// Defines MarkerOptions for a marker.
	/// Refer to https://developers.google.com/android/reference/com/google/android/gms/maps/model/MarkerOptions for more detailed documentation
	/// </summary>
	[PublicAPI]
	[SuppressMessage("ReSharper", "RedundantDefaultMemberInitializer")]
	public sealed class MarkerOptions
	{
		const string MarkerOptionsClass = "com.google.android.gms.maps.model.MarkerOptions";
		const string PositionMethodName = "position";
		const string ZIndexMethodName = "zIndex";
		const string TitleMethodName = "title";
		const string SnippetMethodName = "snippet";
		const string DraggableMethodName = "draggable";
		const string VisibleMethodName = "visible";
		const string FlatMethodName = "flat";
		const string RotationMethodName = "rotation";
		const string AlphaMethodName = "alpha";
		const string AnchorMethodName = "anchor";
		const string InfoWindowAnchorMethodName = "infoWindowAnchor";
		const string IconMethodName = "icon";

		float _zIndex = 0f;
		bool _draggable = false;
		bool _visible = true;
		bool _flat = false;
		float _rotation = 0.0F;
		float _alpha = 1.0F;
		ImageDescriptor _imageDescriptor = ImageDescriptor.DefaultMarker();

		float _anchorU = 0.5f;
		float _anchorV = 1.0f;

		float _infoWindowAnchorU = 0.5f;
		float _infoWindowAnchorV = 0.0f;

		public LatLng MarkerPosition { get; private set; }

		public string MarkerTitle { get; private set; } = string.Empty;

		public string MarkerSnippet { get; private set; } = string.Empty;

		/// <summary>
		/// Sets the location for the marker.
		/// </summary>
		/// <param name="latLng">The location for the marker.</param>
		[PublicAPI]
		public MarkerOptions Position(LatLng latLng)
		{
			if (latLng == null)
			{
				throw new ArgumentNullException(nameof(latLng));
			}

			MarkerPosition = latLng;
			return this;
		}

		/// <summary>
		/// Sets the zIndex for the marker.
		/// </summary>
		/// <param name="zIndex">Sets the zIndex for the marker.</param>
		[PublicAPI]
		public MarkerOptions ZIndex(float zIndex)
		{
			_zIndex = zIndex;
			return this;
		}

		/// <summary>
		/// Sets the icon for the marker.
		/// </summary>
		/// <param name="imageDescriptor">Image descriptor. If null, the default marker is used.</param>
		[PublicAPI]
		public MarkerOptions Icon(ImageDescriptor imageDescriptor)
		{
			_imageDescriptor = imageDescriptor;
			return this;
		}

		/// <summary>
		/// Specifies the anchor to be at a particular point in the marker image.
		/// The anchor specifies the point in the icon image that is anchored to the marker's position on the Earth's surface.
		/// </summary>
		/// <param name="u">u-coordinate of the anchor, as a ratio of the image width (in the range [0, 1])</param>
		/// <param name="v">v-coordinate of the anchor, as a ratio of the image height (in the range [0, 1])</param>
		[PublicAPI]
		public MarkerOptions Anchor(float u, float v)
		{
			_anchorU = Mathf.Clamp01(u);
			_anchorV = Mathf.Clamp01(v);
			return this;
		}

		/// <summary>
		/// Specifies the anchor point of the info window on the marker image. This is specified in the same coordinate system as the anchor.
		/// </summary>
		/// <param name="u">u-coordinate of the info window anchor, as a ratio of the image width (in the range [0, 1])</param>
		/// <param name="v">v-coordinate of the info window anchor, as a ratio of the image height (in the range [0, 1])</param>
		[PublicAPI]
		public MarkerOptions InfoWindowAnchor(float u, float v)
		{
			_infoWindowAnchorU = Mathf.Clamp01(u);
			_infoWindowAnchorV = Mathf.Clamp01(v);
			return this;
		}

		/// <summary>
		/// Sets the title for the marker.
		/// </summary>
		/// <param name="title">Title of the marker.</param>
		[PublicAPI]
		public MarkerOptions Title(string title)
		{
			MarkerTitle = title;
			return this;
		}

		/// <summary>
		/// Sets the snippet for the marker.
		/// </summary>
		/// <param name="snippet">Snippet for the marker.</param>
		[PublicAPI]
		public MarkerOptions Snippet(string snippet)
		{
			MarkerSnippet = snippet;
			return this;
		}

		/// <summary>
		/// Sets the draggability for the marker.
		/// </summary>
		/// <param name="draggable">If set to <c>true</c> draggable.</param>
		[PublicAPI]
		public MarkerOptions Draggable(bool draggable)
		{
			_draggable = draggable;
			return this;
		}

		/// <summary>
		/// Sets the visibility for the marker.
		/// </summary>
		/// <param name="visible">If set to <c>true</c> visible.</param>
		[PublicAPI]
		public MarkerOptions Visible(bool visible)
		{
			_visible = visible;
			return this;
		}

		/// <summary>
		/// Sets whether this marker should be flat against the map <c>true</c> or a billboard facing the camera <c>false</c>.
		/// The default value is <c>false</c>.
		/// </summary>
		/// <param name="flat">If set to <c>true</c> flat.</param>
		[PublicAPI]
		public MarkerOptions Flat(bool flat)
		{
			_flat = flat;
			return this;
		}

		/// <summary>
		/// Sets the rotation of the marker in degrees clockwise about the marker's anchor point.
		/// </summary>
		/// <param name="rotation">Rotation of the marker in degrees clockwise about the marker's anchor point./param>
		[PublicAPI]
		public MarkerOptions Rotation(float rotation)
		{
			_rotation = rotation;
			return this;
		}

		/// <summary>
		/// Sets the alpha (opacity) of the marker. This is a value from 0 to 1, where 0 means the marker is completely transparent and 1 means the marker is completely opaque.
		/// </summary>
		/// <param name="alpha">Alpha (opacity) of the marker.</param>
		[PublicAPI]
		public MarkerOptions Alpha(float alpha)
		{
			_alpha = Mathf.Clamp01(alpha);
			return this;
		}

		public AndroidJavaObject ToAJO()
		{
			if (GoogleMapUtils.IsNotAndroid)
			{
				return null;
			}

			var ajo = new AndroidJavaObject(MarkerOptionsClass);
			if (MarkerPosition != null)
			{
				ajo.CallAJO(PositionMethodName, MarkerPosition.ToAJO());
			}

			ajo.CallAJO(ZIndexMethodName, _zIndex);
			if (MarkerTitle != null)
			{
				ajo.CallAJO(TitleMethodName, MarkerTitle);
			}

			if (MarkerSnippet != null)
			{
				ajo.CallAJO(SnippetMethodName, MarkerSnippet);
			}

			ajo.CallAJO(DraggableMethodName, _draggable);
			ajo.CallAJO(VisibleMethodName, _visible);
			ajo.CallAJO(FlatMethodName, _flat);
			ajo.CallAJO(RotationMethodName, _rotation);
			ajo.CallAJO(AlphaMethodName, _alpha);
			ajo.CallAJO(AnchorMethodName, _anchorU, _anchorV);
			ajo.CallAJO(InfoWindowAnchorMethodName, _infoWindowAnchorU, _infoWindowAnchorV);

			if (_imageDescriptor != null)
			{
				ajo.CallAJO(IconMethodName, _imageDescriptor.ToAJO());
			}

			return ajo;
		}

		public Dictionary<string, object> ToDictionary()
		{
			var result = new Dictionary<string, object>();
			if (MarkerPosition != null)
			{
				result[PositionMethodName] = MarkerPosition.ToDictionary();
			}

			result[ZIndexMethodName] = _zIndex;
			
			result[TitleMethodName] = MarkerTitle;
			result[SnippetMethodName] = MarkerSnippet;

			result[DraggableMethodName] = _draggable;
			result[VisibleMethodName] = _visible;
			result[FlatMethodName] = _flat;
			result[RotationMethodName] = _rotation;
			result[AlphaMethodName] = _alpha;
			result[AnchorMethodName + "U"] = _anchorU;
			result[AnchorMethodName + "V"] = _anchorV;
			result[InfoWindowAnchorMethodName + "U"] = _infoWindowAnchorU;
			result[InfoWindowAnchorMethodName + "V"] = _infoWindowAnchorV;

			GoogleMapUtils.AddImage(result, _imageDescriptor);

			return result;
		}
	}
}