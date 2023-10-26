using System;
using DeadMosquito.JniToolkit;
using UnityEngine.Assertions;

namespace NinevaStudios.GoogleMaps
{
	using System.Collections.Generic;
	using Internal;
	using MiniJSON;
	using UnityEngine;
	using JetBrains.Annotations;

	/// <summary>
	/// An immutable class representing a latitude/longitude aligned rectangle.
	/// </summary>
	[PublicAPI]
	public sealed class LatLngBounds
	{
		const string SWLat = "latLngBoundsSouthWestLat";
		const string SWLng = "latLngBoundsSouthWestLng";
		const string NELat = "latLngBoundsNorthEastLat";
		const string NELng = "latLngBoundsNorthEastLng";

		public static readonly LatLngBounds Zero = new LatLngBounds(LatLng.Zero, LatLng.Zero);

		readonly LatLng _southwest;
		readonly LatLng _northeast;

		/// <summary>
		/// Creates a new bounds based on a southwest and a northeast corner.
		/// </summary>
		/// <param name="southwest">Southwest corner.</param>
		/// <param name="northeast">Northeast corner.</param>
		public LatLngBounds(LatLng southwest, LatLng northeast)
		{
			_southwest = southwest;
			_northeast = northeast;
		}

		/// <summary>
		/// Returns the center of this LatLngBounds.
		/// </summary>
		public LatLng Center
		{
			get
			{
				var latCenter = (_southwest.Latitude + _northeast.Latitude) / 2.0D;
				double lngCenter;
				if (_southwest.Longitude <= _northeast.Longitude)
				{
					lngCenter = (_northeast.Longitude + _southwest.Longitude) / 2.0D;
				}
				else
				{
					lngCenter = (_northeast.Longitude + 360.0D + _southwest.Longitude) / 2.0D;
				}

				return new LatLng(latCenter, lngCenter);
			}
		}

		public AndroidJavaObject ToAJO()
		{
			if (GoogleMapUtils.IsNotAndroid)
			{
				return null;
			}

			return new AndroidJavaObject("com.google.android.gms.maps.model.LatLngBounds", _southwest.ToAJO(),
				_northeast.ToAJO());
		}

		public static LatLngBounds FromAJO(AndroidJavaObject ajo)
		{
			if (GoogleMapUtils.IsNotAndroid)
			{
				return new LatLngBounds(LatLng.Zero, LatLng.Zero);
			}

			var northeast = LatLng.FromAJO(ajo.GetAJO("northeast"));
			var southwest = LatLng.FromAJO(ajo.GetAJO("southwest"));
			return new LatLngBounds(southwest, northeast);
		}

		public override string ToString()
		{
			return $"[LatLngBounds SW: {_southwest}, NE: {_northeast}]";
		}

		public Dictionary<string, object> ToDictionary()
		{
			var result = new Dictionary<string, object>
			{
				[SWLat] = _southwest.Latitude,
				[SWLng] = _southwest.Longitude,
				[NELat] = _northeast.Latitude,
				[NELng] = _northeast.Longitude
			};
			return result;
		}

		public static LatLngBounds FromJson(string boundsJson)
		{
			var dic = Json.Deserialize(boundsJson) as Dictionary<string, object>;
			var latSW = dic.GetDouble(SWLat);
			var lngSW = dic.GetDouble(SWLng);
			var latNE = dic.GetDouble(NELat);
			var lngNE = dic.GetDouble(NELng);

			return new LatLngBounds(new LatLng(latSW, lngSW), new LatLng(latNE, lngNE));
		}

		#region equality

		bool Equals(LatLngBounds other)
		{
			return Equals(_southwest, other._southwest) && Equals(_northeast, other._northeast);
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

			return obj is LatLngBounds && Equals((LatLngBounds) obj);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((_southwest != null ? _southwest.GetHashCode() : 0) * 397) ^ (_northeast != null ? _northeast.GetHashCode() : 0);
			}
		}

		public static bool operator ==(LatLngBounds left, LatLngBounds right)
		{
			return Equals(left, right);
		}

		public static bool operator !=(LatLngBounds left, LatLngBounds right)
		{
			return !Equals(left, right);
		}

		#endregion

		/// <summary>
		/// This is a builder that is able to create a minimum bound based on a set of LatLng points.
		/// </summary>
		[PublicAPI]
		public class Builder
		{
			double _south = 1.0D / 0.0;
			double _north = -1.0D / 0.0;
			double _west = 0.0D / 0.0;
			double _east = 0.0D / 0.0;

			/// <summary>
			/// Includes this point for building of the bounds. The bounds will be extended in a minimum way to include this point.
			/// More precisely, it will consider extending the bounds both in the eastward and westward directions (one of which may cross the antimeridian) and choose the smaller of the two. In the case that both directions result in a LatLngBounds of the same size, this will extend it in the eastward direction. For example, adding points (0, -179) and (1, 179) will create a bound crossing the 180 longitude.
			/// </summary>
			/// <param name="point">A <see cref="LatLng"/> to be included in the bounds.</param>
			/// <returns></returns>
			public Builder Include(LatLng point)
			{
				_south = Math.Min(_south, point.Latitude);
				_north = Math.Max(_north, point.Latitude);
				if (double.IsNaN(_west))
				{
					_west = point.Longitude;
				}
				else
				{
					if (_west <= _east ? _west <= point.Longitude && point.Longitude <= _east : _west <= point.Longitude || point.Longitude <= _east)
					{
						return this;
					}

					if ((_west - point.Longitude + 360.0D) % 360.0D < (point.Longitude - _east + 360.0D) % 360.0D)
					{
						_west = point.Longitude;
						return this;
					}
				}

				_east = point.Longitude;
				return this;
			}

			/// <summary>
			/// Creates the <see cref="LatLng"/> bounds.
			/// </summary>
			/// <returns></returns>
			public LatLngBounds Build()
			{
				Assert.IsTrue(!double.IsNaN(_west), "no included points");
				var southwest = new LatLng(_south, _west);
				var northeast = new LatLng(_north, _east);
				return new LatLngBounds(southwest, northeast);
			}
		}
	}
}