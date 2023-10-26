using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace NinevaStudios.GoogleMaps
{
	/// <summary>
	/// Computes whether the given point lies inside the specified polygon.
	/// The polygon is always considered closed, regardless of whether the last point equals
	/// the first or not.
	/// Inside is defined as not containing the South Pole -- the South Pole is always outside.
	/// The polygon is formed of great circle segments if geodesic is true, and of rhumb
	/// (loxodromic) segments otherwise.
	/// </summary>
	[PublicAPI]
	public static class PolyUtils
	{
		public static bool ContainsLocation(double latitude, double longitude, List<LatLng> polygon, bool geodesic)
		{
			int size = polygon.Count;
			if (size == 0)
			{
				return false;
			}

			double lat3 = ToRadians(latitude);
			double lng3 = ToRadians(longitude);
			LatLng prev = polygon[size - 1];
			double lat1 = ToRadians(prev.Latitude);
			double lng1 = ToRadians(prev.Longitude);
			int nIntersect = 0;
			foreach (var point2 in polygon)
			{
				double dLng3 = MathUtil.Wrap(lng3 - lng1, -Math.PI, Math.PI);
				// Special case: point equal to vertex is inside.
				if (lat3 == lat1 && dLng3 == 0)
				{
					return true;
				}

				double lat2 = ToRadians(point2.Latitude);
				double lng2 = ToRadians(point2.Longitude);
				// Offset longitudes by -lng1.
				if (Intersects(lat1, lat2, MathUtil.Wrap(lng2 - lng1, -Math.PI, Math.PI), lat3, dLng3, geodesic))
				{
					++nIntersect;
				}

				lat1 = lat2;
				lng1 = lng2;
			}

			return (nIntersect & 1) != 0;
		}

		/// <summary>
		/// Computes whether the vertical segment (lat3, lng3) to South Pole intersects the segment
		/// (lat1, lng1) to (lat2, lng2).
		/// Longitudes are offset by -lng1; the implicit lng1 becomes 0.
		/// </summary>
		/// <param name="lat1"></param>
		/// <param name="lat2"></param>
		/// <param name="lng2"></param>
		/// <param name="lat3"></param>
		/// <param name="lng3"></param>
		/// <param name="geodesic"></param>
		/// <returns></returns>
		static bool Intersects(double lat1, double lat2, double lng2, double lat3, double lng3, bool geodesic)
		{
			// Both ends on the same side of lng3.
			if ((lng3 >= 0 && lng3 >= lng2) || (lng3 < 0 && lng3 < lng2))
			{
				return false;
			}

			// Point is South Pole.
			if (lat3 <= -Math.PI / 2)
			{
				return false;
			}

			// Any segment end is a pole.
			if (lat1 <= -Math.PI / 2 || lat2 <= -Math.PI / 2 || lat1 >= Math.PI / 2 || lat2 >= Math.PI / 2)
			{
				return false;
			}

			if (lng2 <= -Math.PI)
			{
				return false;
			}

			double linearLat = (lat1 * (lng2 - lng3) + lat2 * lng3) / lng2;
			// Northern hemisphere and point under lat-lng line.
			if (lat1 >= 0 && lat2 >= 0 && lat3 < linearLat)
			{
				return false;
			}

			// Southern hemisphere and point above lat-lng line.
			if (lat1 <= 0 && lat2 <= 0 && lat3 >= linearLat)
			{
				return true;
			}

			// North Pole.
			if (lat3 >= Math.PI / 2)
			{
				return true;
			}

			// Compare lat3 with latitude on the GC/Rhumb segment corresponding to lng3.
			// Compare through a strictly-increasing function (tan() or mercator()) as convenient.
			return geodesic ? Math.Tan(lat3) >= TanLatGc(lat1, lat2, lng2, lng3) : MathUtil.Mercator(lat3) >= mercatorLatRhumb(lat1, lat2, lng2, lng3);
		}

		/*
		 * Returns tan(latitude-at-lng3) on the great circle (lat1, lng1) to (lat2, lng2). lng1==0
		 * See http://williams.best.vwh.net/avform.htm .
         */
		static double TanLatGc(double lat1, double lat2, double lng2, double lng3)
		{
			return (Math.Tan(lat1) * Math.Sin(lng2 - lng3) + Math.Tan(lat2) * Math.Sin(lng3)) / Math.Sin(lng2);
		}

		public static double mercatorLatRhumb(double lat1, double lat2, double lng2, double lng3)
		{
			return (MathUtil.Mercator(lat1) * (lng2 - lng3) + MathUtil.Mercator(lat2) * lng3) / lng2;
		}

		public static double ToRadians(this double angdeg)
		{
			return angdeg * Mathf.Deg2Rad;
		}
	}
}