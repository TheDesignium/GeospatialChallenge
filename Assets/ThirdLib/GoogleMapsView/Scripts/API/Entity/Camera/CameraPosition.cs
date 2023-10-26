using DeadMosquito.JniToolkit;

namespace NinevaStudios.GoogleMaps
{
	using System.Collections.Generic;
	using Internal;
	using JetBrains.Annotations;
	using MiniJSON;
	using UnityEngine;

	/// <summary>
	/// An immutable class that aggregates all camera position parameters such as location, zoom level, tilt angle, and bearing. 
	/// </summary>
	[PublicAPI]
	public sealed class CameraPosition
	{
		const string CameraPositionClass = "com.google.android.gms.maps.model.CameraPosition";

		/// <summary>
		/// The location that the camera is pointing at.
		/// </summary>
		[PublicAPI]
		public LatLng LatitudeLongitude { get; }

		/// <summary>
		/// Zoom level near the center of the screen.
		/// </summary>
		[PublicAPI]
		public float Zoom { get; }

		/// <summary>
		/// The angle, in degrees, of the camera angle from the nadir (directly facing the Earth).
		/// </summary>
		[PublicAPI]
		public float Tilt { get; }

		/// <summary>
		/// Direction that the camera is pointing in, in degrees clockwise from north.
		/// </summary>
		[PublicAPI]
		public float Bearing { get; }

		/// <summary>
		/// An immutable class that aggregates all camera position parameters such as location, zoom level, tilt angle, and bearing.
		/// </summary>
		/// <param name="target">Target.</param>
		/// <param name="zoom">Zoom.</param>
		/// <param name="tilt">Tilt.</param>
		/// <param name="bearing">Bearing.</param>
		[PublicAPI]
		public CameraPosition(LatLng target, float zoom, float tilt, float bearing)
		{
			Bearing = bearing;
			Tilt = tilt;
			Zoom = zoom;
			LatitudeLongitude = target;
		}

		public AndroidJavaObject ToAJO()
		{
			if (GoogleMapUtils.IsNotAndroid)
			{
				return null;
			}

			return new AndroidJavaObject(CameraPositionClass, LatitudeLongitude.ToAJO(), Zoom, Tilt, Bearing);
		}

		public static CameraPosition FromAJO(AndroidJavaObject ajo)
		{
			if (GoogleMapUtils.IsNotAndroid)
			{
				return new CameraPosition(LatLng.Zero, 0, 0, 0);
			}

			var latLng = LatLng.FromAJO(ajo.GetAJO("target"));
			var zoom = ajo.GetFloat("zoom");
			var tilt = ajo.GetFloat("tilt");
			var bearing = ajo.GetFloat("bearing");
			return new CameraPosition(latLng, zoom, tilt, bearing);
		}

		public static CameraPosition FromJson([NotNull] string json)
		{
			if (string.IsNullOrEmpty(json))
			{
				Debug.LogError("Cannot deserialize CameraPosition");
				return new CameraPosition(LatLng.Zero, 0, 0, 0);
			}
			
			var dict = Json.Deserialize(json) as Dictionary<string,object>;

			var latLng = new LatLng(dict.GetDouble("lat"), dict.GetDouble("lng"));
			var zoom = dict.GetFloat("zoom");
			var tilt = dict.GetFloat("tilt");
			var bearing = dict.GetFloat("bearing");
			
			return new CameraPosition(latLng, zoom, tilt, bearing);
		}

		public Dictionary<string, object> ToDictionary()
		{
			var result = new Dictionary<string, object>
			{
				["cameraPositionLat"] = LatitudeLongitude.Latitude,
				["cameraPositionLng"] = LatitudeLongitude.Longitude,
				["cameraPositionZoom"] = Zoom,
				["cameraPositionTilt"] = Tilt,
				["cameraPositionBearing"] = Bearing
			};
			return result;
		}

		public override string ToString()
		{
			return $"[CameraPosition: LatitudeLongitude={LatitudeLongitude}, Zoom={Zoom}, Tilt={Tilt}, Bearing={Bearing}]";
		}
	}
}