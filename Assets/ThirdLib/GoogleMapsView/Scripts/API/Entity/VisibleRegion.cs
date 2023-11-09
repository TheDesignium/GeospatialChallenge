using System.Collections.Generic;
using DeadMosquito.JniToolkit;
using JetBrains.Annotations;
using NinevaStudios.GoogleMaps.Internal;
using NinevaStudios.GoogleMaps.MiniJSON;
using UnityEngine;

namespace NinevaStudios.GoogleMaps
{
	/// <summary>
	/// Contains the four points defining the four-sided polygon that is visible in a map's camera.
	/// This polygon can be a trapezoid instead of a rectangle, because a camera can have tilt.
	/// If the camera is directly over the center of the camera, the shape is rectangular,
	/// but if the camera is tilted, the shape will appear to be a trapezoid
	/// whose smallest side is closest to the point of view.
	/// </summary>
	[PublicAPI]
	public class VisibleRegion
	{
		const string FarLeftKey = "farLeft";
		const string FarRightKey = "farRight";
		const string NearLeftKey = "nearLeft";
		const string NearRightKey = "nearRight";
		const string LatLngBoundsKey = "latLngBounds";

		/// <summary>
		/// Far left corner of the camera.
		/// </summary>
		[PublicAPI]
		public LatLng FarLeft = LatLng.Zero;

		/// <summary>
		/// Far right corner of the camera.
		/// </summary>
		[PublicAPI]
		public LatLng FarRight = LatLng.Zero;

		/// <summary>
		/// Bottom left corner of the camera.
		/// </summary>
		[PublicAPI]
		public LatLng NearLeft = LatLng.Zero;

		/// <summary>
		/// Bottom right corner of the camera.
		/// </summary>
		[PublicAPI]
		public LatLng NearRight = LatLng.Zero;

		/// <summary>
		/// The smallest bounding box that includes the visible region defined in this class.
		/// </summary>
		[PublicAPI]
		public LatLngBounds LatLngBounds = LatLngBounds.Zero;

		internal static VisibleRegion FromAjo(AndroidJavaObject ajo)
		{
			return new VisibleRegion
			{
				FarLeft = LatLng.FromAJO(ajo.GetAJO(FarLeftKey)),
				FarRight = LatLng.FromAJO(ajo.GetAJO(FarRightKey)),
				NearLeft = LatLng.FromAJO(ajo.GetAJO(NearLeftKey)),
				NearRight = LatLng.FromAJO(ajo.GetAJO(NearRightKey)),
				LatLngBounds = LatLngBounds.FromAJO(ajo.GetAJO(LatLngBoundsKey))
			};
		}

		internal static VisibleRegion FromJson(string json)
		{
			if (string.IsNullOrEmpty(json))
			{
				return null;
			}

			var dictionary = Json.Deserialize(json) as Dictionary<string, object>;
			return new VisibleRegion
			{
				FarLeft = LatLng.FromJson(dictionary.GetStr(FarLeftKey)),
				FarRight = LatLng.FromJson(dictionary.GetStr(FarRightKey)),
				NearLeft = LatLng.FromJson(dictionary.GetStr(NearLeftKey)),
				NearRight = LatLng.FromJson(dictionary.GetStr(NearRightKey)),
				LatLngBounds = LatLngBounds.FromJson(dictionary.GetStr(LatLngBoundsKey))
			};
		}

		public override string ToString() => $"Far left : {FarLeft}, far right : {FarRight}, near left: {NearLeft}, near right : {NearRight}, bounds: {LatLngBounds}";
	}
}