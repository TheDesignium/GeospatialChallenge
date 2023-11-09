using System;
using System.Runtime.InteropServices;
using DeadMosquito.JniToolkit;
using JetBrains.Annotations;
using NinevaStudios.GoogleMaps.Internal;
using NinevaStudios.GoogleMaps.MiniJSON;
using UnityEngine;

namespace NinevaStudios.GoogleMaps
{
	/// <summary>
	/// A projection is used to translate between on screen location
	/// and geographic coordinates on the surface of the Earth (LatLng).
	/// Screen location is in screen pixels (not display pixels) with respect
	/// to the top left corner of the map (and not necessarily of the whole screen).
	/// </summary>
	[PublicAPI]
	public class Projection
	{
		readonly AndroidJavaObject _ajo;
#pragma warning disable 0414
		readonly IntPtr _projectionPointer = IntPtr.Zero;
#pragma warning restore 0414
		internal Projection(AndroidJavaObject ajo)
		{
			_ajo = ajo;
		}

		internal Projection(IntPtr projectionPtr)
		{
			_projectionPointer = projectionPtr;
		}
		
		/// <summary>
		/// Returns the geographic location that corresponds to a screen location.
		/// </summary>
		[PublicAPI]
		public LatLng FromScreenLocation(Vector2Int point)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return LatLng.Zero;
			}
			
#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			var latLngJson = _googleMapViewProjectionFromScreenLocation(_projectionPointer, point.x, point.y);
			return LatLng.FromJson(latLngJson);
#endif
#pragma warning disable 0162
			var javaPoint = new AndroidJavaObject("android.graphics.Point", point.x, point.y);
			var latLngJava = _ajo.CallAJO("fromScreenLocation", javaPoint);
			return LatLng.FromAJO(latLngJava);
#pragma warning restore 0162
		}

		/// <summary>
		/// Gets a projection of the viewing frustum for converting between screen coordinates and geo-latitude/longitude coordinates.
		/// </summary>
		[PublicAPI]
		public VisibleRegion GetVisibleRegion()
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return new VisibleRegion();
			}
			
#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			var regionJson = _googleMapViewProjectionGetVisibleRegion(_projectionPointer);
			return VisibleRegion.FromJson(regionJson);
#endif
#pragma warning disable 0162
			var javaRegion = _ajo.CallAJO("getVisibleRegion");
			return VisibleRegion.FromAjo(javaRegion);
#pragma warning restore 0162
		}

		/// <summary>
		/// Returns a screen location that corresponds to a geographical coordinate (LatLng).
		/// </summary>
		[PublicAPI]
		public Vector2 ToScreenLocation(LatLng location)
		{
			if (GoogleMapUtils.IsPlatformNotSupported)
			{
				return Vector2.zero;
			}
#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
			var array = _googleMapViewProjectionToScreenLocation(_projectionPointer,
				Json.Serialize(location.ToDictionary())).Split(';');
			return new Vector2(float.Parse(array[0]), float.Parse(array[1]));
#endif
#pragma warning disable 0162
			var pointJava = _ajo.CallAJO("toScreenLocation", location.ToAJO());
			return new Vector2(pointJava.Get<int>("x"), pointJava.Get<int>("y"));
#pragma warning restore 0162
		}
		
#if UNITY_IOS && !DISABLE_IOS_GOOGLE_MAPS
		[DllImport("__Internal")]
		static extern string _googleMapViewProjectionFromScreenLocation(IntPtr projectionPointer, int x, int y);
		
		[DllImport("__Internal")]
		static extern string _googleMapViewProjectionGetVisibleRegion(IntPtr projectionPointer);
		
		[DllImport("__Internal")]
		static extern string _googleMapViewProjectionToScreenLocation(IntPtr projectionPointer, string latLngJson);
#endif
	}
}