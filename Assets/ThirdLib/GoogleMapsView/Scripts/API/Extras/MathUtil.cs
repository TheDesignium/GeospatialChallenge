using System;
using JetBrains.Annotations;

namespace NinevaStudios.GoogleMaps
{
	[PublicAPI]
	public class MathUtil
	{
		public const double EarthRadius = 6371009;

		/**
		 * Restrict x to the range [low, high].
		 */
		static double Clamp(double x, double low, double high)
		{
			return x < low ? low : (x > high ? high : x);
		}

		/**
		 * Wraps the given value into the inclusive-exclusive interval between min and max.
		 * @param n   The value to wrap.
		 * @param min The minimum.
		 * @param max The maximum.
		 */
		public static double Wrap(double n, double min, double max)
		{
			return (n >= min && n < max) ? n : (Mod(n - min, max - min) + min);
		}

		/**
		 * Returns the non-negative remainder of x / m.
		 * @param x The operand.
		 * @param m The modulus.
		 */
		static double Mod(double x, double m)
		{
			return ((x % m) + m) % m;
		}

		/**
		 * Returns mercator Y corresponding to latitude.
		 * See http://en.wikipedia.org/wiki/Mercator_projection .
		 */
		public static double Mercator(double lat)
		{
			return Math.Log(Math.Tan(lat * 0.5 + Math.PI / 4));
		}

		/**
		 * Returns latitude from mercator Y.
		 */
		static double InverseMercator(double y)
		{
			return 2 * Math.Atan(Math.Exp(y)) - Math.PI / 2;
		}

		/**
		 * Returns haversine(angle-in-radians).
		 * hav(x) == (1 - cos(x)) / 2 == sin(x / 2)^2.
		 */
		static double Hav(double x)
		{
			double sinHalf = Math.Sin(x * 0.5);
			return sinHalf * sinHalf;
		}

		/**
		 * Computes inverse haversine. Has good numerical stability around 0.
		 * arcHav(x) == acos(1 - 2 * x) == 2 * asin(Math.Sqrt(x)).
		 * The argument must be in [0, 1], and the result is positive.
		 */
		static double ArcHav(double x)
		{
			return 2 * Math.Asin(Math.Sqrt(x));
		}

		// Given h==hav(x), returns sin(abs(x)).
		static double SinFromHav(double h)
		{
			return 2 * Math.Sqrt(h * (1 - h));
		}

		// Returns hav(asin(x)).
		static double HavFromSin(double x)
		{
			double x2 = x * x;
			return x2 / (1 + Math.Sqrt(1 - x2)) * .5;
		}

		// Returns sin(arcHav(x) + arcHav(y)).
		static double SinSumFromHav(double x, double y)
		{
			double a = Math.Sqrt(x * (1 - x));
			double b = Math.Sqrt(y * (1 - y));
			return 2 * (a + b - 2 * (a * y + b * x));
		}

		/**
		 * Returns hav() of distance from (lat1, lng1) to (lat2, lng2) on the unit sphere.
		 */
		static double HavDistance(double lat1, double lat2, double dLng)
		{
			return Hav(lat1 - lat2) + Hav(dLng) * Math.Cos(lat1) * Math.Cos(lat2);
		}
	}
}