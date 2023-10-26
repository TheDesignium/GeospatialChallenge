using System;
using System.Collections.Generic;
using NinevaStudios.GoogleMaps.Internal;
using DeadMosquito.JniToolkit;
using JetBrains.Annotations;
using NinevaStudios.GoogleMaps.MiniJSON;
using UnityEngine;

namespace NinevaStudios.GoogleMaps
{
	/// <summary>
	/// Tile provider that creates heatmap tiles.
	/// </summary>
	[PublicAPI]
	public class HeatmapTileProvider : TileProvider
	{
		const string BuilderClass = "com.google.maps.android.heatmaps.HeatmapTileProvider$Builder";

		const string WeightedDataMethodName = "weightedData";
		const string RadiusMethodName = "radius";
		const string GradientMethodName = "gradient";
		const string OpacityMethodName = "opacity";

		List<WeightedLatLng> _data;
		int _radius;
		double _opacity;
		readonly HeatmapGradient _gradient;

		HeatmapTileProvider(List<WeightedLatLng> data, int radius, double opacity, HeatmapGradient gradient)
		{
			_data = data;
			_radius = radius;
			_opacity = opacity;
			_gradient = gradient;
		}

		public AndroidJavaObject AJO
		{
			get
			{
				using (var managerClass = new AndroidJavaClass(GoogleMapsView.ClassName))
				{
					var latLngData = new List<object>();
					_data.ForEach(x => latLngData.Add(x.ToDictionary()));
					return managerClass.CallStaticAJO("createHeatmapTileProvider", _radius, _gradient.ToAJO(), _opacity, Json.Serialize(latLngData));
				}
			}
		}

		public Dictionary<string, object> Dictionary
		{
			get
			{
				var dic = new Dictionary<string, object>();

				var latLngData = new List<object>();
				_data.ForEach(x => latLngData.Add(x.ToDictionary()));

				dic[WeightedDataMethodName] = latLngData;
				dic[RadiusMethodName] = _radius;
				dic[OpacityMethodName] = _opacity;
				dic[GradientMethodName] = _gradient.ToDictionary();

				return dic;
			}
		}

		/// <summary>
		/// Builder class for the <see cref="HeatmapTileProvider"/>.
		/// </summary>
		[PublicAPI]
		public class Builder
		{
			const int DEFAULT_RADIUS = 20;
			const double DEFAULT_OPACITY = 0.7;

			List<WeightedLatLng> _data;
			int _radius = DEFAULT_RADIUS;
			double _opacity = DEFAULT_OPACITY;
			HeatmapGradient _gradient = HeatmapGradient.DefaultGradient;

			/// <summary>
			/// Setter for data in builder. Must call this or weightedData
			/// </summary>
			/// <param name="data">Collection of LatLngs to put into quadtree. Should be non-empty.</param>
			/// <returns>updated builder object</returns>
			/// <exception cref="ArgumentNullException"></exception>
			[PublicAPI]
			public Builder Data([NotNull] List<LatLng> data)
			{
				if (data == null || data.Count == 0)
				{
					throw new ArgumentException("data must not be null or empty");
				}

				_data = data.ConvertAll(x => new WeightedLatLng(x));
				return this;
			}

			/// <summary>
			/// Setter for data in builder. Must call this or data
			/// </summary>
			/// <param name="weightedData">Collection of WeightedLatLngs to put into quadtree. Should be non-empty.</param>
			/// <returns></returns>
			/// <exception cref="ArgumentException"></exception>
			[PublicAPI]
			public Builder WeightedData([NotNull] List<WeightedLatLng> weightedData)
			{
				if (weightedData == null || weightedData.Count == 0)
				{
					throw new ArgumentException("weightedData must not be null or empty");
				}

				_data = weightedData;
				return this;
			}

			/// <summary>
			/// The size of the Gaussian blur applied to the heatmap, expressed in pixels. The default is 20. Must be between 10 and 50.
			/// </summary>
			/// <param name="radius">The size of the Gaussian blur applied to the heatmap, expressed in pixels.</param>
			/// <returns>updated builder object</returns>
			/// <exception cref="ArgumentOutOfRangeException">Must be between 10 and 50.</exception>
			[PublicAPI]
			public Builder Radius(int radius)
			{
				if (radius <= 10 || radius >= 50)
				{
					throw new ArgumentOutOfRangeException(nameof(radius));
				}

				_radius = radius;
				return this;
			}

			/// <summary>
			/// Setter for gradient in builder
			/// </summary>
			/// <param name="gradient"></param>
			/// <returns>updated builder object</returns>
			[PublicAPI]
			public Builder Gradient([NotNull] HeatmapGradient gradient)
			{
				_gradient = gradient;
				return this;
			}

			/// <summary>
			/// Setter for opacity in builder
			/// </summary>
			/// <param name="opacity">Opacity of the entire heatmap in range [0, 1]</param>
			/// <returns>updated builder object</returns>
			[PublicAPI]
			public Builder Opacity(double opacity)
			{
				_opacity = opacity;
				return this;
			}

			/// <summary>
			/// Build the <see cref="HeatmapTileProvider"/> instance. 
			/// </summary>
			/// <returns>The <see cref="HeatmapTileProvider"/> instance.</returns>
			[PublicAPI]
			public HeatmapTileProvider Build()
			{
				return new HeatmapTileProvider(_data, _radius, _opacity, _gradient);
			}
		}
	}
}