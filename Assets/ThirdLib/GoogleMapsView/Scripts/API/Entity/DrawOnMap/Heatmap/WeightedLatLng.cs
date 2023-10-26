using System.Collections.Generic;
using JetBrains.Annotations;
using NinevaStudios.GoogleMaps.Internal;
using UnityEngine;

namespace NinevaStudios.GoogleMaps
{
	/// <summary>
	/// A wrapper for <see cref="LatLng"/> with intensity
	/// </summary>
	[PublicAPI]
	public class WeightedLatLng
	{
		const string WeightedLatLngClass = "com.google.maps.android.heatmaps.WeightedLatLng";
		
		const double DefaultIntensity = 1;

		LatLng _latLng;
		double _intensity;

		/// <summary>
		/// 
		/// </summary>
		/// <param name="latLng">LatLng to add to wrapper.</param>
		/// <param name="intensity">Intensity to use: should be greater than 0. Default value is 1.</param>
		public WeightedLatLng(LatLng latLng, double intensity = DefaultIntensity)
		{
			_latLng = latLng;
			_intensity = intensity >= 0 ? intensity : DefaultIntensity;
		}
		
		public Dictionary<string, object> ToDictionary()
		{
			var result = new Dictionary<string, object>
			{
				["lat"] = _latLng.Latitude,
				["lng"] = _latLng.Longitude,
				["intensity"] = _intensity
			};
			return result;
		}
		
		public AndroidJavaObject ToAJO()
		{
			return GoogleMapUtils.IsAndroid
				? new AndroidJavaObject(WeightedLatLngClass, _latLng.ToAJO(), _intensity)
				: null;
		}
	}
}