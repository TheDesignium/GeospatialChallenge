using DeadMosquito.JniToolkit;

namespace NinevaStudios.GoogleMaps
{
	using System;
	using System.Collections.Generic;
	using System.Text;
	using Internal;
	using JetBrains.Annotations;
	using MiniJSON;
	using UnityEngine;

	/// <summary>
	/// https://developers.google.com/android/reference/com/google/android/gms/maps/model/LatLng
	/// 
	/// An immutable class representing a pair of latitude and longitude coordinates, stored as degrees.
	/// </summary>
	[PublicAPI]
	public sealed class LatLng
	{
		public static readonly LatLng Zero = new LatLng(0, 0);

		/// <summary>
		/// Latitude
		/// </summary>
		[PublicAPI]
		public double Latitude { get; }

		/// <summary>
		/// Longitude
		/// </summary>
		[PublicAPI]
		public double Longitude { get; }

		/// <summary>
		/// Constructs a <see cref="LatLng"/> with the other <see cref="LatLng"/> values
		/// </summary>
		/// <param name="latLng"><see cref="LatLng"/> to make a copy of</param>
		[PublicAPI]
		public LatLng(LatLng latLng)
		{
			Latitude = latLng.Latitude;
			Longitude = latLng.Longitude;
		}

		/// <summary>
		/// Constructs a <see cref="LatLng"/> with the given latitude and longitude, measured in degrees.
		/// </summary>
		/// <param name="latitude">Latitude.</param>
		/// <param name="longitude">Longitude.</param>
		[PublicAPI]
		public LatLng(double latitude, double longitude)
		{
			Latitude = Math.Max(-90.0D, Math.Min(90.0D, latitude));

			if (-180.0D <= longitude && longitude < 180.0D)
			{
				Longitude = longitude;
			}
			else
			{
				Longitude = ((longitude - 180.0D) % 360.0D + 360.0D) % 360.0D - 180.0D;
			}
		}

		#region equality

		bool Equals(LatLng other)
		{
			return Latitude.Equals(other.Latitude) && Longitude.Equals(other.Longitude);
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
			{
				return false;
			}

			if (ReferenceEquals(this, obj))
			{
				return true;
			}

			return obj is LatLng && Equals((LatLng) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return (Latitude.GetHashCode() * 397) ^ Longitude.GetHashCode();
			}
		}

		public static bool operator ==(LatLng left, LatLng right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(LatLng left, LatLng right)
		{
			return !Equals(left, right);
		}

		#endregion

		public override string ToString()
		{
			return new StringBuilder(60).Append("lat/lng: (").Append(Latitude).Append(",").Append(Longitude).Append(")")
				.ToString();
		}

		[NotNull]
		public static LatLng FromAJO(AndroidJavaObject ajo)
		{
			return GoogleMapUtils.IsAndroid
				? new LatLng(ajo.GetDouble("latitude"), ajo.GetDouble("longitude"))
				: new LatLng(0, 0);
		}

		public AndroidJavaObject ToAJO()
		{
			return GoogleMapUtils.IsAndroid
				? new AndroidJavaObject("com.google.android.gms.maps.model.LatLng", Latitude, Longitude)
				: null;
		}

		public static LatLng FromJson(string json)
		{
			if (string.IsNullOrEmpty(json))
			{
				return null;
			}
			
			var dic = Json.Deserialize(json) as Dictionary<string, object>;
			return FromDictionary(dic);
		}

		static LatLng FromDictionary(Dictionary<string, object> dic)
		{
			var lat = dic.GetDouble("lat");
			var lng = dic.GetDouble("lng");
			return new LatLng(lat, lng);
		}

		public Dictionary<string, object> ToDictionary()
		{
			var result = new Dictionary<string, object>
			{
				["lat"] = Latitude,
				["lng"] = Longitude
			};
			return result;
		}

		public static List<object> ToJsonList(List<LatLng> coords)
		{
			var result = new List<object>();
			foreach (var latLng in coords)
			{
				result.Add(latLng.ToDictionary());
			}
			return result;
		}
		
		public static List<object> ToJsonList(List<List<LatLng>> holes)
		{
			var result = new List<object>();

			foreach (var hole in holes)
			{
				var holeCoordsList = new List<object>();
				foreach (var coord in hole)
				{
					holeCoordsList.Add(coord.ToDictionary());
				}
				result.Add(holeCoordsList);
			}

			return result;
		}

		public static List<LatLng> ListFromJson(string pointsJson)
		{
			var points = Json.Deserialize(pointsJson) as List<object>;
			var result = new List<LatLng>();
			foreach (var point in points)
			{
				result.Add(FromDictionary(point as Dictionary<string, object>));
			}
			return result;
		}

		public static List<List<LatLng>> HolesListFromJson(string holesJson)
		{
			Debug.Log(holesJson);
			var holes = Json.Deserialize(holesJson) as List<object>;
			var result = new List<List<LatLng>>();
			foreach (var hole in holes)
			{
				var coordsOfHole = new List<LatLng>();
				foreach (var coord in (List<object>) hole)
				{
					var coordDic = coord as Dictionary<string, object>;
					coordsOfHole.Add(FromDictionary(coordDic));
				}
				result.Add(coordsOfHole);
			}

			return result;
		}
	}
}