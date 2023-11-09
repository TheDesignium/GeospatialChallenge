using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using NinevaStudios.GoogleMaps.Internal;
using UnityEngine;

namespace NinevaStudios.GoogleMaps
{
	/// <summary>
	/// A class to generate a color map from a given array of colors and the fractions that the colors represent by interpolating between their HSV values. This color map is to be used in the <see cref="HeatmapTileProvider"/>.
	/// </summary>
	[PublicAPI]
	public class HeatmapGradient
	{
		const string GradientClass = "com.google.maps.android.heatmaps.Gradient";

		static readonly Color[] DEFAULT_GRADIENT_COLORS =
		{
			new Color32(102, 225, 0, Byte.MaxValue),
			new Color32(255, 0, 0, Byte.MaxValue)
		};

		static readonly float[] DEFAULT_GRADIENT_START_POINTS =
		{
			0.2f, 1f
		};

		const int DEFAULT_COLOR_MAP_SIZE = 1000;

		public static readonly HeatmapGradient DefaultGradient =
			new HeatmapGradient(DEFAULT_GRADIENT_COLORS, DEFAULT_GRADIENT_START_POINTS, DEFAULT_COLOR_MAP_SIZE);

		readonly Color[] _colors;
		readonly float[] _startPoints;
		readonly int _colorMapSize;

		/// <summary>
		/// Creates a Gradient with the given colors and starting points which creates a colorMap of given size.
		/// </summary>
		/// <param name="colors">The colors to be used in the gradient</param>
		/// <param name="startPoints">The starting point for each color, given as a percentage of the maximum intensity</param>
		/// <param name="colorMapSize">Size of a color map for the heatmap</param>
		[PublicAPI]
		public HeatmapGradient(Color[] colors, float[] startPoints, int colorMapSize)
		{
			if (colors.Length != startPoints.Length)
			{
				throw new ArgumentException("colors and startPoints should be same length");
			}

			if (colors.Length == 0)
			{
				throw new ArgumentException("No colors have been defined");
			}

			for (int i = 1; i < startPoints.Length; i++)
			{
				if (startPoints[i] <= startPoints[i - 1])
				{
					throw new ArgumentException("startPoints should be in increasing order");
				}
			}

			_colors = colors;
			_startPoints = startPoints;
			_colorMapSize = colorMapSize;
		}

		public Dictionary<string, object> ToDictionary()
		{
			var result = new Dictionary<string, object>();

			var colors = new List<object>(_colors.Length);
			_colors.ToList().ForEach(x => colors.Add(x.ToDictionary()));
			result["colors"] = colors;

			var startPoints = new List<object>(_startPoints.Length);
			_startPoints.ToList().ForEach(x => startPoints.Add(x));
			result["startPoints"] = startPoints;

			result["colorMapSize"] = _colorMapSize;

			return result;
		}

		public AndroidJavaObject ToAJO()
		{
			int[] colors = _colors.ToList().ConvertAll(x => x.ToAndroidColor()).ToArray();
			return GoogleMapUtils.IsAndroid ? new AndroidJavaObject(GradientClass, colors, _startPoints) : null;
		}
	}
}