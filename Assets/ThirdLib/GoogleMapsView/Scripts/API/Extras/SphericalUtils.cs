using System.Collections.Generic;
using NinevaStudios.GoogleMaps;
using JetBrains.Annotations;
using UnityEngine;

namespace NinevaStudios.GoogleMaps
{
	[PublicAPI]
	public class SphericalUtils
	{
		const int EarthRadius = 6371009;

		/// <summary>
		/// Returns the heading from one LatLng to another LatLng.
		/// Headings are expressed in degrees clockwise from North within the range [-180,180).
		/// </summary>
		public static float ComputeHeading(LatLng from, LatLng to)
		{
			var fromLat = ToRadians(from.Latitude);
			var fromLng = ToRadians(from.Longitude);
			var toLat = ToRadians(to.Latitude);
			var toLng = ToRadians(to.Longitude);
			var dLng = toLng - fromLng;
			var heading = Mathf.Atan2(Mathf.Sin(dLng) * Mathf.Cos(toLat), Mathf.Cos(fromLat) * Mathf.Sin(toLat) - Mathf.Sin(fromLat) * Mathf.Cos(toLat) * Mathf.Cos(dLng));

			return Wrap(ToDegrees(heading), -180f, 180f);
		}

		/// <summary>
		/// Returns the LatLng resulting from moving a distance from an origin in the specified heading (expressed in degrees clockwise from north).
		/// </summary>
		/// <param name="from">The LatLng from which to start.</param>
		/// <param name="distance">The distance to travel.</param>
		/// <param name="heading">The heading in degrees clockwise from north.</param>
		public static LatLng ComputeOffset(LatLng from, float distance, float heading)
		{
			distance /= EarthRadius;
			heading = ToRadians(heading);
			var fromLat = ToRadians(from.Latitude);
			var fromLng = ToRadians(from.Longitude);
			var cosDistance = Mathf.Cos(distance);
			var sinDistance = Mathf.Sin(distance);
			var sinFromLat = Mathf.Sin(fromLat);
			var cosFromLat = Mathf.Cos(fromLat);
			var sinLat = cosDistance * sinFromLat + sinDistance * cosFromLat * Mathf.Cos(heading);
			var dLng = Mathf.Atan2(sinDistance * cosFromLat * Mathf.Sin(heading), cosDistance - sinFromLat * sinLat);

			return new LatLng(ToDegrees(Mathf.Asin(sinLat)), ToDegrees(fromLng + dLng));
		}

		/// <summary>
		/// Returns the location of origin when provided with a LatLng destination, meters travelled and original heading.
		/// Headings are expressed in degrees clockwise from North. This function returns null when no solution is available.
		/// </summary>
		/// <param name="to">The destination LatLng.</param>
		/// <param name="distance">The distance travelled, in meters.</param>
		/// <param name="heading">The heading in degrees clockwise from north.</param>
		public static LatLng ComputeOffsetOrigin(LatLng to, float distance, float heading)
		{
			heading = ToRadians(heading);
			distance /= EarthRadius;
			var n1 = Mathf.Cos(distance);
			var n2 = Mathf.Sin(distance) * Mathf.Cos(heading);
			var n3 = Mathf.Sin(distance) * Mathf.Sin(heading);
			var n4 = Mathf.Sin(ToRadians(to.Latitude));
			var n12 = n1 * n1;
			var discriminant = n2 * n2 * n12 + n12 * n12 - n12 * n4 * n4;
			if (discriminant < 0)
			{
				return null;
			}

			var b = n2 * n4 + Mathf.Sqrt(discriminant);
			b /= n1 * n1 + n2 * n2;
			var a = (n4 - n2 * b) / n1;
			var fromLatRadians = Mathf.Atan2(a, b);
			if (fromLatRadians < -Mathf.PI / 2 || fromLatRadians > Mathf.PI / 2)
			{
				b = n2 * n4 - Mathf.Sqrt(discriminant);
				b /= n1 * n1 + n2 * n2;
				fromLatRadians = Mathf.Atan2(a, b);
			}

			if (fromLatRadians < -Mathf.PI / 2 || fromLatRadians > Mathf.PI / 2)
			{
				return null;
			}

			var fromLngRadians = ToRadians(to.Longitude) - Mathf.Atan2(n3, (n1 * Mathf.Cos(fromLatRadians) - n2 * Mathf.Sin(fromLatRadians)));
			return new LatLng(ToDegrees(fromLatRadians), ToDegrees(fromLngRadians));
		}

		/// <summary>
		/// Returns the LatLng which lies the given fraction of the way between the origin LatLng and the destination LatLng.
		/// </summary>
		/// <param name="from">The LatLng from which to start.</param>
		/// <param name="to">The LatLng toward which to travel.</param>
		/// <param name="fraction">A fraction of the distance to travel.</param>
		public static LatLng Interpolate(LatLng from, LatLng to, float fraction)
		{
			var fromLat = ToRadians(from.Latitude);
			var fromLng = ToRadians(from.Longitude);
			var toLat = ToRadians(to.Latitude);
			var toLng = ToRadians(to.Longitude);
			var cosFromLat = Mathf.Cos(fromLat);
			var cosToLat = Mathf.Cos(toLat);
			var angle = ComputeAngleBetween(from, to);
			var sinAngle = Mathf.Sin(angle);
			if (sinAngle < 1E-6)
			{
				return new LatLng(from.Latitude + fraction * (to.Latitude - from.Latitude),
					from.Longitude + fraction * (to.Longitude - from.Longitude));
			}

			var a = Mathf.Sin((1 - fraction) * angle) / sinAngle;
			var b = Mathf.Sin(fraction * angle) / sinAngle;
			var x = a * cosFromLat * Mathf.Cos(fromLng) + b * cosToLat * Mathf.Cos(toLng);
			var y = a * cosFromLat * Mathf.Sin(fromLng) + b * cosToLat * Mathf.Sin(toLng);
			var z = a * Mathf.Sin(fromLat) + b * Mathf.Sin(toLat);
			var lat = Mathf.Atan2(z, Mathf.Sqrt(x * x + y * y));
			var lng = Mathf.Atan2(y, x);
			return new LatLng(ToDegrees(lat), ToDegrees(lng));
		}

		/// <summary>
		/// Returns the distance between two LatLngs, in meters.
		/// </summary>
		/// <param name="from">The LatLng from which to start.</param>
		/// <param name="to">The LatLng toward which to travel.</param>
		public static float ComputeDistanceBetween(LatLng from, LatLng to)
		{
			return ComputeAngleBetween(from, to) * EarthRadius;
		}

		/// <summary>
		/// Returns the length of the given path, in meters, on Earth.
		/// </summary>
		public static float ComputeLength(List<LatLng> path)
		{
			if (path.Count < 2)
			{
				return 0;
			}

			var length = 0f;
			var prev = path[0];
			var prevLat = ToRadians(prev.Latitude);
			var prevLng = ToRadians(prev.Longitude);
			foreach (var point in path)
			{
				var lat = ToRadians(point.Latitude);
				var lng = ToRadians(point.Longitude);
				length += DistanceRadians(prevLat, prevLng, lat, lng);
				prevLat = lat;
				prevLng = lng;
			}

			return length * EarthRadius;
		}

		/// <summary>
		/// Returns the area of a closed path on Earth in square meters.
		/// </summary>
		public static float ComputeArea(List<LatLng> path)
		{
			return Mathf.Abs(ComputeSignedArea(path));
		}

		/// <summary>
		/// Returns the signed area of a closed path on Earth in square meters.
		/// The sign of the area may be used to determine the orientation of the path.
		/// "inside" is the surface that does not contain the South Pole.
		/// </summary>
		public static float ComputeSignedArea(List<LatLng> path)
		{
			return ComputeSignedArea(path, EarthRadius);
		}

		#region private

		static float ToRadians(double value)
		{
			return (float) value / 180f * Mathf.PI;
		}

		static float ToDegrees(float value)
		{
			return value * 180f / Mathf.PI;
		}

		static float Wrap(float value, float min, float max)
		{
			return value >= min && value < max ? value : Mod(value - min, max - min) + min;
		}

		static float Mod(float operand, float modulus)
		{
			return (operand % modulus + modulus) % modulus;
		}

		static float ComputeAngleBetween(LatLng from, LatLng to)
		{
			return DistanceRadians(ToRadians(from.Latitude), ToRadians(from.Longitude),
				ToRadians(to.Latitude), ToRadians(to.Longitude));
		}

		static float DistanceRadians(float lat1, float lng1, float lat2, float lng2)
		{
			return ArcHav(HavDistance(lat1, lat2, lng1 - lng2));
		}

		static float ArcHav(float x)
		{
			return 2 * Mathf.Asin(Mathf.Sqrt(x));
		}

		static float HavDistance(float lat1, float lat2, float dLng)
		{
			return Hav(lat1 - lat2) + Hav(dLng) * Mathf.Cos(lat1) * Mathf.Cos(lat2);
		}

		static float Hav(float x)
		{
			var sinHalf = Mathf.Sin(x * 0.5f);
			return sinHalf * sinHalf;
		}

		static float ComputeSignedArea(List<LatLng> path, float radius)
		{
			var size = path.Count;
			if (size < 3)
			{
				return 0;
			}

			var total = 0f;
			var prev = path[size - 1];
			var prevTanLat = Mathf.Tan((Mathf.PI / 2 - ToRadians(prev.Latitude)) / 2);
			var prevLng = ToRadians(prev.Longitude);
			foreach (var point in path)
			{
				var tanLat = Mathf.Tan((Mathf.PI / 2 - ToRadians(point.Latitude)) / 2);
				var lng = ToRadians(point.Longitude);
				total += PolarTriangleArea(tanLat, lng, prevTanLat, prevLng);
				prevTanLat = tanLat;
				prevLng = lng;
			}

			return total * (radius * radius);
		}

		static float PolarTriangleArea(float tan1, float lng1, float tan2, float lng2)
		{
			var deltaLng = lng1 - lng2;
			var t = tan1 * tan2;
			return 2 * Mathf.Atan2(t * Mathf.Sin(deltaLng), 1 + t * Mathf.Cos(deltaLng));
		}

		#endregion
	}
}