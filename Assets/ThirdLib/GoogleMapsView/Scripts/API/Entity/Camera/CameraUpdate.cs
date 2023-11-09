using DeadMosquito.JniToolkit;

namespace NinevaStudios.GoogleMaps
{
	using System.Collections.Generic;
	using Internal;
	using JetBrains.Annotations;
	using UnityEngine;

	/// <summary>
	/// Defines a camera move. An object of this type can be used to modify a map's camera by calling animateCamera(CameraUpdate), animateCamera(CameraUpdate, GoogleMap.CancelableCallback) or moveCamera(CameraUpdate).
	/// To obtain a <see cref="CameraUpdate"/> use the factory methods in <see cref="CameraUpdate"/>.
	/// </summary>
	[PublicAPI]
	public sealed class CameraUpdate
	{
		const string CameraUpdateFactoryClass = "com.google.android.gms.maps.CameraUpdateFactory";
		const string NewCameraPositionMethodName = "newCameraPosition";
		const string NewLatLngMethodName = "newLatLng";
		const string NewLatLngBoundsMethodName = "newLatLngBounds";
		const string NewLatLngZoomMethodName = "newLatLngZoom";
		const string ScrollByMethodName = "scrollBy";
		const string ZoomByMethodName = "zoomBy";
		const string ZoomInMethodName = "zoomIn";
		const string ZoomOutMethodName = "zoomOut";
		const string ZoomToMethodName = "zoomTo";
		const string CameraUpdateTypeKey = "type";

		readonly AndroidJavaObject _ajo;
		readonly Dictionary<string, object> _dic;

		static readonly CameraUpdate DummyCameraUpdate = new CameraUpdate();

		CameraUpdate()
		{
		}

		CameraUpdate(AndroidJavaObject androidJavaObject)
		{
			_ajo = androidJavaObject;
		}

		CameraUpdate(Dictionary<string, object> dictionary)
		{
			_dic = dictionary;
		}

		static AndroidJavaObject CreateCameraUpdateAJO(string methodName, params object[] args)
		{
			if (GoogleMapUtils.IsNotAndroid)
			{
				return null;
			}

			using (var c = new AndroidJavaClass(CameraUpdateFactoryClass))
			{
				return c.CallStaticAJO(methodName, args);
			}
		}

		internal AndroidJavaObject ToAJO()
		{
			return GoogleMapUtils.IsNotAndroid ? null : _ajo;
		}

		internal Dictionary<string, object> ToDictionary()
		{
			return _dic;
		}

		/// <summary>
		/// Returns a <see cref="CameraUpdate"/> that moves the camera to a specified <see cref="CameraPosition"/>. 
		/// In effect, this creates a transformation from the <see cref="CameraPosition"/> object's latitude, longitude, zoom level, bearing and tilt.
		/// </summary>
		/// <param name="cameraPosition">Camera position</param>
		/// <returns>a <see cref="CameraUpdate"/> containing the transformation.</returns>
		[PublicAPI]
		public static CameraUpdate NewCameraPosition(CameraPosition cameraPosition)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return DummyCameraUpdate;
			}

			if (GoogleMapUtils.IsAndroid)
			{
				return new CameraUpdate(CreateCameraUpdateAJO(NewCameraPositionMethodName, cameraPosition.ToAJO()));
			}

			return new CameraUpdate(new Dictionary<string, object>
			{
				{CameraUpdateTypeKey, NewCameraPositionMethodName},
				{"cameraPosition", cameraPosition.ToDictionary()}
			});
		}

		/// <summary>
		/// Returns a <see cref="CameraUpdate"/> that moves the center of the screen to a latitude and longitude specified by a <see cref="LatLng"/> object.
		/// </summary>
		/// <param name="latLng">Latitude and longitude</param>
		/// <returns>a <see cref="CameraUpdate"/> containing the transformation.</returns>
		[PublicAPI]
		public static CameraUpdate NewLatLng(LatLng latLng)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return DummyCameraUpdate;
			}

			if (GoogleMapUtils.IsAndroid)
			{
				return new CameraUpdate(CreateCameraUpdateAJO(NewLatLngMethodName, latLng.ToAJO()));
			}
			
			return new CameraUpdate(new Dictionary<string, object>
			{
				{CameraUpdateTypeKey, NewLatLngMethodName},
				{"lat", latLng.Latitude},
				{"lng", latLng.Longitude}
			});
		}

		/// <summary>
		/// Returns a <see cref="CameraUpdate"/> that transforms the camera such that the specified latitude/longitude bounds are centered on screen at the greatest possible zoom level.
		/// </summary>
		/// <param name="bounds">Region to fit on screen</param>
		/// <param name="padding">Space (in px) to leave between the bounding box edges and the view edges. This value is applied to all four sides of the bounding box.</param>
		/// <returns>a <see cref="CameraUpdate"/> containing the transformation.</returns>
		[PublicAPI]
		public static CameraUpdate NewLatLngBounds(LatLngBounds bounds, int padding)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return DummyCameraUpdate;
			}

			if (GoogleMapUtils.IsAndroid)
			{
				return new CameraUpdate(CreateCameraUpdateAJO(NewLatLngBoundsMethodName, bounds.ToAJO(), padding));
			}
			
			return new CameraUpdate(new Dictionary<string, object>
			{
				{CameraUpdateTypeKey, NewLatLngBoundsMethodName},
				{"bounds", bounds.ToDictionary()},
				{"padding", padding}
			});
		}

		/// <summary>
		/// Returns a <see cref="CameraUpdate"/> that transforms the camera such that the specified latitude/longitude bounds are centered on screen within a bounding box of specified dimensions at the greatest possible zoom level.
		/// </summary>
		/// <param name="bounds">The region to fit in the bounding box</param>
		/// <param name="width">Bounding box width in pixels (px)</param>
		/// <param name="height">Bounding box height in pixels (px)</param>
		/// <param name="padding">Additional size restriction (in px) of the bounding box</param>
		/// <returns>a <see cref="CameraUpdate"/> containing the transformation.</returns>
		[PublicAPI]
		[GoogleMapsAndroidOnly]
		public static CameraUpdate NewLatLngBounds(LatLngBounds bounds, int width, int height, int padding)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return DummyCameraUpdate;
			}

			if (GoogleMapUtils.IsAndroid)
			{
				return new CameraUpdate(CreateCameraUpdateAJO(NewLatLngBoundsMethodName, bounds.ToAJO(), width, height, padding));
			}
			
			return new CameraUpdate(new Dictionary<string, object>
			{
				{CameraUpdateTypeKey, NewLatLngBoundsMethodName + "WithInsets"},
				{"bounds", bounds.ToDictionary()},
				{"padding", padding}
			});
		}

		/// <summary>
		/// Returns a <see cref="CameraUpdate"/> that moves the center of the screen to a latitude and longitude specified by a LatLng object, and moves to the given zoom level.
		/// </summary>
		/// <param name="latLng">a <see cref="LatLng"/> object containing the desired latitude and longitude.</param>
		/// <param name="zoom">The desired zoom level, in the range of 2.0 to 21.0. Values below this range are set to 2.0, and values above it are set to 21.0. Increase the value to zoom in. Not all areas have tiles at the largest zoom levels.</param>
		/// <returns>a <see cref="CameraUpdate"/> containing the transformation.</returns>
		[PublicAPI]
		public static CameraUpdate NewLatLngZoom(LatLng latLng, float zoom)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return DummyCameraUpdate;
			}

			if (GoogleMapUtils.IsAndroid)
			{
				return new CameraUpdate(CreateCameraUpdateAJO(NewLatLngZoomMethodName, latLng.ToAJO(), zoom));
			}
			
			return new CameraUpdate(new Dictionary<string, object>
			{
				{CameraUpdateTypeKey, NewLatLngZoomMethodName},
				{"lat", latLng.Latitude},
				{"lng", latLng.Longitude},
				{"zoom", zoom}
			});
		}

		/// <summary>
		/// Returns a <see cref="CameraUpdate"/> that scrolls the camera over the map, shifting the center of view by the specified number of pixels in the x and y directions.
		/// </summary>
		/// <param name="xPixel">The number of pixels to scroll horizontally. A positive value moves the camera to the right, with respect to its current orientation. A negative value moves the camera to the left, with respect to its current orientation.</param>
		/// <param name="yPixel">The number of pixels to scroll vertically. A positive value moves the camera downwards, with respect to its current orientation. A negative value moves the camera upwards, with respect to its current orientation.</param>
		/// <returns></returns>
		[PublicAPI]
		public static CameraUpdate ScrollBy(float xPixel, float yPixel)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return DummyCameraUpdate;
			}

			if (GoogleMapUtils.IsAndroid)
			{
				return new CameraUpdate(CreateCameraUpdateAJO(ScrollByMethodName, xPixel, yPixel));
			}
			
			return new CameraUpdate(new Dictionary<string, object>
			{
				{CameraUpdateTypeKey, ScrollByMethodName},
				{"x", xPixel},
				{"y", yPixel}
			});
		}

		/// <summary>
		/// Returns a <see cref="CameraUpdate"/> that shifts the zoom level of the current camera viewpoint.
		/// </summary>
		/// <param name="amount">Amount to change the zoom level. Positive values indicate zooming closer to the surface of the Earth while negative values indicate zooming away from the surface of the Earth.</param>
		/// <param name="x">X pixel location on the screen that is to remain fixed after the zooming process. The lat/long that was at that pixel location before the camera move will remain the same after the camera has moved.</param>
		/// <param name="y">Y pixel location on the screen that is to remain fixed after the zooming process. The lat/long that was at that pixel location before the camera move will remain the same after the camera has moved.</param>
		/// <returns>a <see cref="CameraUpdate"/> containing the transformation.</returns>
		[PublicAPI]
		public static CameraUpdate ZoomBy(float amount, int x, int y)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return DummyCameraUpdate;
			}

			if (GoogleMapUtils.IsAndroid)
			{
				var pointAjo = GoogleMapUtils.IsAndroid ? new AndroidJavaObject("android.graphics.Point", x, y) : null;
				return new CameraUpdate(CreateCameraUpdateAJO(ZoomByMethodName, amount, pointAjo));
			}
			
			return new CameraUpdate(new Dictionary<string, object>
			{
				{CameraUpdateTypeKey, ZoomByMethodName + "XY"},
				{"zoom", amount},
				{"x", x},
				{"y", y}
			});
		}

		/// <summary>
		/// Returns a <see cref="CameraUpdate"/> that shifts the zoom level of the current camera viewpoint.
		/// </summary>
		/// <param name="amount">Amount to change the zoom level. Positive values indicate zooming closer to the surface of the Earth while negative values indicate zooming away from the surface of the Earth.</param>
		/// <returns>a <see cref="CameraUpdate"/> containing the transformation.</returns>
		[PublicAPI]
		public static CameraUpdate ZoomBy(float amount)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return DummyCameraUpdate;
			}

			if (GoogleMapUtils.IsAndroid)
			{
				return new CameraUpdate(CreateCameraUpdateAJO(ZoomByMethodName, amount));
			}
			
			return new CameraUpdate(new Dictionary<string, object>
			{
				{CameraUpdateTypeKey, ZoomByMethodName},
				{"zoom", amount}
			});
		}

		/// <summary>
		/// Returns a <see cref="CameraUpdate"/> that zooms in on the map by moving the viewpoint's height closer to the Earth's surface. The zoom increment is 1.0.
		/// </summary>
		/// <returns>a <see cref="CameraUpdate"/> containing the transformation.</returns>
		[PublicAPI]
		public static CameraUpdate ZoomIn()
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return DummyCameraUpdate;
			}

			if (GoogleMapUtils.IsAndroid)
			{
				return new CameraUpdate(CreateCameraUpdateAJO(ZoomInMethodName));
			}
			
			return new CameraUpdate(new Dictionary<string, object>
			{
				{CameraUpdateTypeKey, ZoomInMethodName}
			});
		}

		/// <summary>
		/// Returns a <see cref="CameraUpdate"/> that zooms out on the map by moving the viewpoint's height farther away from the Earth's surface. The zoom increment is -1.0.
		/// </summary>
		/// <returns>a <see cref="CameraUpdate"/> containing the transformation.</returns>
		[PublicAPI]
		public static CameraUpdate ZoomOut()
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return DummyCameraUpdate;
			}

			if (GoogleMapUtils.IsAndroid)
			{
				return new CameraUpdate(CreateCameraUpdateAJO(ZoomOutMethodName));
			}
			
			return new CameraUpdate(new Dictionary<string, object>
			{
				{CameraUpdateTypeKey, ZoomOutMethodName}
			});
		}

		/// <summary>
		/// Returns a <see cref="CameraUpdate"/> that moves the camera viewpoint to a particular zoom level.
		/// </summary>
		/// <param name="zoom">the desired zoom level, in the range of 2.0 to 21.0. Values below this range are set to 2.0, and values above it are set to 21.0. Increase the value to zoom in. Not all areas have tiles at the largest zoom levels.</param>
		/// <returns>a <see cref="CameraUpdate"/> containing the transformation.</returns>
		[PublicAPI]
		public static CameraUpdate ZoomTo(float zoom)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return DummyCameraUpdate;
			}

			if (GoogleMapUtils.IsAndroid)
			{
				return new CameraUpdate(CreateCameraUpdateAJO(ZoomToMethodName, zoom));
			}
			
			return new CameraUpdate(new Dictionary<string, object>
			{
				{CameraUpdateTypeKey, ZoomToMethodName},
				{"zoom", zoom}
			});
		}
	}
}