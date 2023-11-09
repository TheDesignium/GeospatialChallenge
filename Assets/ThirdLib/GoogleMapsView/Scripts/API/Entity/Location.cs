
using DeadMosquito.JniToolkit;
using NinevaStudios.GoogleMaps.Internal;

namespace NinevaStudios.GoogleMaps
{
	using UnityEngine;
	using System;
	using System.Collections.Generic;
	using JetBrains.Annotations;
	using MiniJSON;

	/// <summary>
	/// GPS Location.
	/// </summary>
	[PublicAPI]
	public sealed class Location
	{
		/// <summary>
		/// Gets the latitude.
		/// </summary>
		/// <value>The latitude.</value>
		[PublicAPI]
		public double Latitude { get; }

		/// <summary>
		/// Gets the Longitude.
		/// </summary>
		/// <value>The Longitude.</value>
		[PublicAPI]
		public double Longitude { get; }

		/// <summary>
		/// Gets a value indicating whether this instance has accuracy.
		/// </summary>
		/// <value><c>true</c> if this instance has accuracy; otherwise, <c>false</c>.</value>
		[PublicAPI]
		public bool HasAccuracy { get; }

		/// <summary>
		/// Gets the location accuracy.
		/// </summary>
		/// <value>The location accuracy.</value>
		[PublicAPI]
		public float Accuracy { get; }

		/// <summary>
		/// True if this location has a speed.
		/// </summary>
		[PublicAPI]
		public bool HasSpeed { get; set; }

		/// <summary>
		/// Get the speed if it is available, in meters/second over ground.
		/// 
		// If this location does not have a speed then 0.0 is returned.
		/// </summary>
		[PublicAPI]
		public float Speed { get; set; }

		/// <summary>
		/// 
		/// </summary>
		[PublicAPI]
		public bool HasBearing { get; set; }

		/// <summary>
		/// Get the bearing, in degrees.
		// Bearing is the horizontal direction of travel of this device, and is not related to the device orientation.It is guaranteed to be in the range (0.0, 360.0] if the device has a bearing.
		// If this location does not have a bearing then 0.0 is returned.
		/// </summary>
		[PublicAPI]
		public float Bearing { get; set; }

		/// <summary>
		/// WARNING! This API was added in API level 18. It will always return false if Android version is below 18;
		///
		/// Returns true if the Location came from a mock provider.
		/// </summary>
		[PublicAPI]
		public bool IsFromMockProvider { get; set; }


		/// <summary>
		/// Return the UTC time of this fix, in milliseconds since January 1, 1970.
		/// </summary>
		[PublicAPI]
		public long Timestamp { get; }

		[PublicAPI]
		public Location(double latitude, double longitude, bool hasAccuracy, float accuracy, long timestamp)
		{
			Latitude = latitude;
			Longitude = longitude;
			HasAccuracy = hasAccuracy;
			Accuracy = accuracy;
			Timestamp = timestamp;
		}

		public static Location FromAJO(/*Location*/ AndroidJavaObject locationAJO)
		{
			if (locationAJO.IsJavaNull())
			{
				return null;
			}

			using (locationAJO)
			{
				var latitude = locationAJO.Call<double>("getLatitude");
				var longitude = locationAJO.Call<double>("getLongitude");
				var hasAccuracy = locationAJO.Call<bool>("hasAccuracy");
				var accuracy = locationAJO.Call<float>("getAccuracy");
				long time = locationAJO.Call<long>("getTime");

				var hasSpeed = locationAJO.CallBool("hasSpeed");
				var speed = locationAJO.Call<float>("getSpeed");
				var hasBearing = locationAJO.CallBool("hasBearing");
				var bearing = locationAJO.Call<float>("getBearing");


				var result = new Location(latitude, longitude, hasAccuracy, accuracy, time);

				if (hasSpeed)
				{
					result.HasSpeed = true;
					result.Speed = speed;
				}
				if (hasBearing)
				{
					result.HasBearing = true;
					result.Bearing = bearing;
				}

				bool isFromMockProvider = false;
				try
				{
					isFromMockProvider = locationAJO.CallBool("isFromMockProvider");
				}
				catch (Exception)
				{
					// Ignore
				}
				result.IsFromMockProvider = isFromMockProvider;

				return result;
			}
		}

		public static Location FromJson(string json)
		{
			var dict = Json.Deserialize(json) as Dictionary<string,object>;
			var lat = dict.GetDouble("lat");
			var lng = dict.GetDouble("lng");
			var timestamp = (long) dict["timestamp"];
			var accuracy = dict.GetDouble("horizontalAccuracy");
			var speed = dict.GetFloat("speed");
			var course = dict.GetFloat("course");
			
			var result = new Location(lat, lng, accuracy > 0, (float) accuracy, timestamp * 1000)
			{
				IsFromMockProvider = false,
				HasSpeed = speed >= 0,
				Speed = speed,
				HasBearing = course >= 0,
				Bearing = course
			};

			return result;
		}

		public override string ToString()
		{
			return $"[Location: Latitude={Latitude}, Longitude={Longitude}, " + $"HasAccuracy={HasAccuracy}, Accuracy={Accuracy}, " + $"Timestamp={Timestamp}, " +
			       $"HasSpeed={HasSpeed}, Speed={Speed}, " + $"HasBearing={HasBearing}, Bearing={Bearing}, " + $"IsFromMockProvider={IsFromMockProvider}]";
		}
	}
}