using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace NinevaStudios.GoogleMaps
{
	/// <summary>
	/// Item to be added to the cluster
	/// </summary>
	[PublicAPI]
	public class ClusterItem
	{
		const string ClusteredMarkerClass = "com.deadmosquitogames.gmaps.clustering.MyClusterItem";

		/// <summary>
		/// Position of the item
		/// </summary>
		[PublicAPI]
		public LatLng Position => MarkerOptions.MarkerPosition;

		/// <summary>
		/// Title of the item
		/// </summary>
		[PublicAPI]
		public string Title => MarkerOptions.MarkerTitle;

		/// <summary>
		/// Snippet of the item
		/// </summary>
		[PublicAPI]
		public string Snippet => MarkerOptions.MarkerSnippet;

		/// <summary>
		/// Marker Options to be rendered
		/// </summary>
		[PublicAPI]
		public MarkerOptions MarkerOptions { get; private set; }

		[PublicAPI]
		public ClusterItem(LatLng position, string title, string snippet)
		{
			MarkerOptions = new MarkerOptions()
				.Position(position)
				.Title(title)
				.Snippet(snippet);
		}

		[PublicAPI]
		public ClusterItem([NotNull] MarkerOptions markerOptions)
		{
			MarkerOptions = markerOptions;
		}

		[PublicAPI]
		public ClusterItem(LatLng position)
		{
			MarkerOptions = new MarkerOptions()
				.Position(position);
		}

		public AndroidJavaObject ToAJO()
		{
			return new AndroidJavaObject(ClusteredMarkerClass, MarkerOptions.ToAJO());
		}

		public Dictionary<string, object> ToDictionary()
		{
			return MarkerOptions.ToDictionary();
		}
	}
}