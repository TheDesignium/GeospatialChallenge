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
	/// Defines options for a ground overlay.
	/// 
	/// For detailed documentation visit: https://developers.google.com/android/reference/com/google/android/gms/maps/model/GroundOverlayOptions
	/// </summary>
	[SuppressMessage("ReSharper", "RedundantDefaultMemberInitializer")]
	[PublicAPI]
	public sealed class GroundOverlayOptions
	{
		const string GroundOverlayOptionsClass = "com.google.android.gms.maps.model.GroundOverlayOptions";
		const string ZIndexMethodName = "zIndex";
		const string VisibleMethodName = "visible";
		const string ClickableMethodName = "clickable";
		const string TransparencyMethodName = "transparency";
		const string AnchorMethodName = "anchor";
		const string BearingMethodName = "bearing";
		const string ImageMethodName = "image";

		#region position

		float _width;
		float _height;
		LatLng _latLng;
		LatLngBounds _latLngBounds;
		float _bearing;

		#endregion

		float _anchorU = 0.5f;
		float _anchorV = 0.5f;
		ImageDescriptor _imageDescriptor;
		bool _clickable = true;
		bool _visible = true;

		float _transparency = 0.0F;
		float _zIndex = 0f;

		SetPositionMethodAndroid _setPositionMethodAndroid = SetPositionMethodAndroid.NotSet;
		float _iosZoom = 6f;

		enum SetPositionMethodAndroid
		{
			NotSet,
			LatLngWidthOnly,
			LatLngWidthHeight,
			LatLngBounds
		}

		/// <summary>
		/// Specifies the anchor to be at a particular point in the ground overlay image.
		/// The anchor specifies the point in the icon image that is anchored to the ground overlay's position on the Earth's surface.
		/// </summary>
		/// <param name="u">u-coordinate of the anchor, as a ratio of the image width (in the range [0, 1])</param>
		/// <param name="v">v-coordinate of the anchor, as a ratio of the image height (in the range [0, 1])</param>
		[PublicAPI]
		public GroundOverlayOptions Anchor(float u, float v)
		{
			_anchorU = Mathf.Clamp01(u);
			_anchorV = Mathf.Clamp01(v);
			return this;
		}

		/// <summary>
		/// Specifies the bearing of the ground overlay in degrees clockwise from north. The rotation is performed about the anchor point. If not specified, the default is 0 (i.e., up on the image points north).
		///
		// Note that latitude-longitude bound applies before the rotation.
		/// </summary>
		/// <param name="bearing">the bearing in degrees clockwise from north. Values outside the range [0, 360) will be normalized.</param>
		[PublicAPI]
		public GroundOverlayOptions Bearing(float bearing)
		{
			_bearing = Mathf.Clamp(bearing, 0f, 360f);
			return this;
		}

		/// <summary>
		/// Sets the icon for the ground overlay.
		/// </summary>
		/// <param name="imageDescriptor">Image descriptor. If null, the default ground overlay is used.</param>
		[PublicAPI]
		public GroundOverlayOptions Image([CanBeNull] ImageDescriptor imageDescriptor)
		{
			_imageDescriptor = imageDescriptor;
			return this;
		}

		/// <summary>
		/// Specifies whether this ground overlay is clickable. The default setting is <code>false<code>/.
		/// </summary>
		/// <param name="clickable">Whether the ground overlay is cickable.</param>
		[PublicAPI]
		public GroundOverlayOptions Clickable(bool clickable)
		{
			_clickable = clickable;
			return this;
		}

		/// <summary>
		/// Sets the visibility for the ground overlay.
		/// </summary>
		/// <param name="visible">If set to <c>true</c> visible.</param>
		[PublicAPI]
		public GroundOverlayOptions Visible(bool visible)
		{
			_visible = visible;
			return this;
		}

		/// <summary>
		/// NOTE: This method will work only on Android. On iOS use <see cref="PositionFromBounds"/> or <see cref="PositionForIos"/>
		/// 
		/// Specifies the position for this ground overlay using an anchor point (a <see cref="LatLng"/>) and the width (in meters). 
		/// The height will be adapted accordingly to preserve aspect ratio.
		/// </summary>
		/// <param name="latLng">the location on the map <see cref="LatLng"/> to which the anchor point in the given image will remain fixed. The anchor will remain fixed to the position on the ground when transformations are applied (e.g., setDimensions, setBearing, etc.).</param>
		/// <param name="width">the width of the overlay (in meters). The height will be determined automatically based on the image aspect ratio.</param>
		[PublicAPI]
		[GoogleMapsAndroidOnly]
		public GroundOverlayOptions Position([JetBrains.Annotations.NotNull] LatLng latLng, float width)
		{
			if (latLng == null)
			{
				throw new ArgumentNullException(nameof(latLng));
			}

			_setPositionMethodAndroid = SetPositionMethodAndroid.LatLngWidthOnly;

			_latLng = latLng;
			_width = width;
			return this;
		}

		/// <summary>
		/// NOTE: This method will work only on Android. On iOS use <see cref="PositionFromBounds"/> or <see cref="PositionForIos"/>
		/// 
		/// Specifies the position for this ground overlay using an anchor point (a <see cref="LatLng"/>), width and height (both in meters). When rendered, the image will be scaled to fit the dimensions specified.
		/// </summary>
		/// <param name="latLng">the location on the map <see cref="LatLng"/> to which the anchor point in the given image will remain fixed. The anchor will remain fixed to the position on the ground when transformations are applied (e.g., setDimensions, setBearing, etc.).</param>
		/// <param name="width">the width of the overlay (in meters)</param>
		/// <param name="height">the height of the overlay (in meters)</param>
		[PublicAPI]
		[GoogleMapsAndroidOnly]
		public GroundOverlayOptions Position([JetBrains.Annotations.NotNull] LatLng latLng, float width, float height)
		{
			if (latLng == null)
			{
				throw new ArgumentNullException(nameof(latLng));
			}

			_setPositionMethodAndroid = SetPositionMethodAndroid.LatLngWidthHeight;

			_latLng = latLng;
			_width = width;
			_height = height;
			return this;
		}

		/// <summary>
		/// This is recommended way to set the position as it works both on Android and iOS
		/// 
		/// Specifies the position for this ground overlay.
		/// </summary>
		/// <param name="latLngBounds">a <see cref="LatLngBounds"/> in which to place the ground overlay</param>
		[PublicAPI]
		public GroundOverlayOptions PositionFromBounds([JetBrains.Annotations.NotNull] LatLngBounds latLngBounds)
		{
			if (latLngBounds == null)
			{
				throw new ArgumentNullException(nameof(latLngBounds));
			}

			_setPositionMethodAndroid = SetPositionMethodAndroid.LatLngBounds;

			_latLngBounds = latLngBounds;
			return this;
		}

		/// <summary>
		/// Renders the image at |position|, as if the image's actual size matches camera pixels at |zoomLevel|.
		/// </summary>
		[PublicAPI]
		[GoogleMapsIosOnly]
		public GroundOverlayOptions PositionForIos([JetBrains.Annotations.NotNull] LatLng latLng, float zoomLevel)
		{
			if (GoogleMapUtils.IsAndroid)
			{
				return this;
			}

			_latLng = latLng;
			_iosZoom = zoomLevel;
			return this;
		}

		/// <summary>
		/// Specifies the transparency of the ground overlay. The default transparency is 0 (opaque).
		/// </summary>
		/// <param name="transparency">A float in the range [0..1] where 0 means that the ground overlay is opaque and 1 means that the ground overlay is transparent.</param>
		[PublicAPI]
		public GroundOverlayOptions Transparency(float transparency)
		{
			_transparency = Mathf.Clamp01(transparency);
			return this;
		}

		/// <summary>
		/// Sets the zIndex for the ground overlay.
		/// </summary>
		/// <param name="zIndex">Sets the zIndex for the ground overlay.</param>
		[PublicAPI]
		public GroundOverlayOptions ZIndex(float zIndex)
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

			var ajo = new AndroidJavaObject(GroundOverlayOptionsClass);
			SetPosition(ajo);
			ajo.CallAJO(ZIndexMethodName, _zIndex);
			ajo.CallAJO(VisibleMethodName, _visible);
			ajo.CallAJO(ClickableMethodName, _clickable);
			ajo.CallAJO(TransparencyMethodName, _transparency);
			ajo.CallAJO(AnchorMethodName, _anchorU, _anchorV);
			ajo.CallAJO(BearingMethodName, _bearing);

			if (_imageDescriptor != null)
			{
				ajo.CallAJO(ImageMethodName, _imageDescriptor.ToAJO());
			}

			return ajo;
		}

		void SetPosition(AndroidJavaObject ajo)
		{
			switch (_setPositionMethodAndroid)
			{
				case SetPositionMethodAndroid.LatLngWidthOnly:
					ajo.CallAJO("position", _latLng.ToAJO(), _width);
					break;
				case SetPositionMethodAndroid.LatLngWidthHeight:
					ajo.CallAJO("position", _latLng.ToAJO(), _width, _height);
					break;
				case SetPositionMethodAndroid.LatLngBounds:
					ajo.CallAJO("positionFromBounds", _latLngBounds.ToAJO());
					break;
				case SetPositionMethodAndroid.NotSet:
					break;
				default:
					Debug.LogError("GroundOverlayOptions must have position set");
					break;
			}
		}

		public bool IsImageMissing => _imageDescriptor == null;

		public Dictionary<string, object> ToDictionary()
		{
			var result = new Dictionary<string, object>
			{
				[AnchorMethodName + "U"] = _anchorU,
				[AnchorMethodName + "V"] = _anchorV,
				[BearingMethodName] = _bearing,
				[TransparencyMethodName] = _transparency,
				[ZIndexMethodName] = _zIndex,
				[ClickableMethodName] = _clickable
			};

			if (_latLng == null && _latLngBounds == null)
			{
				Debug.LogError("Position for GroundOverlay was not provided. Errors ahead.");
			}

			if (_latLngBounds != null)
			{
				result["bounds"] = _latLngBounds.ToDictionary();
			}

			if (_latLng != null)
			{
				result["position"] = _latLng.ToDictionary();
				result["zoomLevel"] = _iosZoom;
			}

			GoogleMapUtils.AddImage(result, _imageDescriptor);

			return result;
		}
	}
}