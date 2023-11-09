using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Networking;

namespace NinevaStudios.GoogleMaps
{
	/// <summary>
	/// See https://developers.google.com/maps/documentation/urls/guide for more information
	/// </summary>
	[PublicAPI]
	public static class OpenGoogleMapHelper
	{
		[PublicAPI]
		public enum MapType
		{
			Default,
			Satellite,
			Terrain
		}

		static Dictionary<MapType, string> _mapTypeParams = new Dictionary<MapType, string>
		{
			{MapType.Default, "roadmap"},
			{MapType.Satellite, "satellite"},
			{MapType.Terrain, "terrain"}
		};

		[PublicAPI]
		public enum MapLayer
		{
			None,
			Transit,
			Traffic,
			Bicycling
		}
		
		static Dictionary<MapLayer, string> _mapLayerParams = new Dictionary<MapLayer, string>
		{
			{MapLayer.None, "none"},
			{MapLayer.Transit, "transit"},
			{MapLayer.Traffic, "traffic"},
			{MapLayer.Bicycling, "bicycling"}
		};

		/// <summary>
		/// Launch a Google Map that displays a pin for a specific place, or perform a general search and launch a map to display the results
		/// </summary>
		/// <param name="query">
		/// Defines the place(s) to highlight on the map. The query parameter is required for all search requests.
		/// </param>
		/// <param name="queryPlaceId">
		/// (optional): A place ID is a textual identifier that uniquely identifies a place.
		/// For the search action, you must specify a <see cref="query"/>, but you may also specify a <see cref="queryPlaceId"/>.
		/// If you specify both parameters, the <see cref="query"/> is only used if Google Maps cannot find the place ID.
		/// If you are trying to definitively link to a specific establishment, the place ID is the best guarantee that you will link to the right place.
		/// It is also recommended to submit a <see cref="queryPlaceId"/> when you query for a specific location using latitude/longitude coordinates.
		/// </param>
		public static void OpenSearch([NotNull] string query, [CanBeNull] string queryPlaceId = null)
		{
			if (query == null)
			{
				throw new ArgumentNullException(nameof(query));
			}

			var uri = $"https://www.google.com/maps/search/?api=1&query={UnityWebRequest.EscapeURL(query)}";

			if (queryPlaceId != null)
			{
				uri += $"&query_place_id={UnityWebRequest.EscapeURL(queryPlaceId)}";
			}

			OpenUrl(uri);
		}

		/// <summary>
		/// Directions action displays the path between two or more specified points on the map, as well as the distance and travel time.
		/// </summary>
		/// <param name="directionsParams">Parameters for displaying directions.</param>
		public static void OpenDirections([NotNull] DirectionsParams directionsParams)
		{
			if (directionsParams == null)
			{
				throw new ArgumentNullException(nameof(directionsParams));
			}

			var uriBuilder = new StringBuilder("https://www.google.com/maps/dir/?api=1");

			uriBuilder.Append($"&origin={UnityWebRequest.EscapeURL(directionsParams.Origin)}");
			if (!string.IsNullOrEmpty(directionsParams.OriginPlaceId))
			{
				uriBuilder.Append($"&origin_place_id={UnityWebRequest.EscapeURL(directionsParams.OriginPlaceId)}");
			}

			uriBuilder.Append($"&destination={UnityWebRequest.EscapeURL(directionsParams.Destination)}");
			if (!string.IsNullOrEmpty(directionsParams.DestinationPlaceId))
			{
				uriBuilder.Append($"&destination_place_id={UnityWebRequest.EscapeURL(directionsParams.DestinationPlaceId)}");
			}

			var travelMode = directionsParams.GetTravelMode();
			if (!string.IsNullOrEmpty(travelMode))
			{
				uriBuilder.Append($"&travelmode={travelMode}");
			}

			if(directionsParams.LaunchNavigation)
			{
				uriBuilder.Append("&dir_action=navigate");
			}

			var waypoints = directionsParams.Waypoints;
			if (waypoints != null && waypoints.Any())
			{
				uriBuilder.Append("&waypoints=");
				uriBuilder.Append(UnityWebRequest.EscapeURL(string.Join("|", waypoints.ToArray())));
			}

			var waypointIds = directionsParams.WaypointPlaceIds;
			if (waypointIds != null && waypointIds.Any())
			{
				uriBuilder.Append("&waypoint_place_ids=");
				uriBuilder.Append(UnityWebRequest.EscapeURL(string.Join("|", waypointIds.ToArray())));
			}

			OpenUrl(uriBuilder.ToString());
		}

		/// <summary>
		/// Launch Google Maps with no markers or directions
		/// </summary>
		/// <param name="center">Defines the center of the map window, and accepts latitude/longitude coordinates as comma-separated values</param>
		/// <param name="zoom">Sets the initial zoom level of the map. Accepted values are whole integers ranging from 0 (the whole world) to 21 (individual buildings). The upper limit can vary depending on the map data available at the selected location. The default is 15.</param>
		/// <param name="mapType">Defines the type of map to display.</param>
		/// <param name="mapLayer">Defines an extra layer to display on the map, if any. </param>
		public static void DisplayMap([CanBeNull] LatLng center = null, int zoom = 15,
			MapType mapType = MapType.Default, MapLayer mapLayer = MapLayer.None)
		{
			var uriBuilder = new StringBuilder("https://www.google.com/maps/@?api=1&map_action=map");

			if (center != null)
			{
				uriBuilder.Append($"&center={center.Latitude},{center.Longitude}");
			}
			
			uriBuilder.Append($"&zoom={zoom}");
			uriBuilder.Append($"&basemap={_mapTypeParams[mapType]}");
			uriBuilder.Append($"&layer={_mapLayerParams[mapLayer]}");
			
			OpenUrl(uriBuilder.ToString());
		}

		/// <summary>
		/// Launch Google Maps, centered on the user’s current location and with default params
		/// </summary>
		public static void DisplayMapDefault()
		{
			OpenUrl("https://www.google.com/maps/@?api=1&map_action=map");
		}

		/// <summary>
		/// The pano action lets you launch a viewer to display Street View images as interactive panoramas. Each Street View panorama provides a full 360-degree view from a single location.
		/// 
		/// More about street view panorama: https://developers.google.com/maps/documentation/urls/guide#street-view-action
		/// </summary>
		/// <param name="panoramaId">
		/// The specific panorama ID of the image to display. If you specify a <see cref="panoramaId"/> you may also specify a <see cref="viewpoint"/>.
		/// The <see cref="viewpoint"/> is only used if Google Maps cannot find the panorama ID. If <see cref="panoramaId"/> is specified but not found, and a <see cref="viewpoint"/> is NOT specified, no panorama image is displayed.
		/// Instead, Google Maps opens in default mode, displaying a map centered on the user's current location.
		/// </param>
		/// <param name="viewpoint">
		/// The viewer displays the panorama photographed closest to the viewpoint location, specified as comma-separated latitude/longitude coordinates (for example 46.414382,10.013988).
		/// Because Street View imagery is periodically refreshed, and photographs may be taken from slightly different positions each time, it's possible that your location may snap to a different panorama when imagery is updated.
		/// </param>
		/// <param name="heading">
		/// Indicates the compass heading of the camera in degrees clockwise from North. Accepted values are from -180 to 360 degrees.
		/// If omitted, a default heading is chosen based on the viewpoint (if specified) of the query and the actual location of the image.
		/// </param>
		/// <param name="pitch">
		/// Specifies the angle, up or down, of the camera. The pitch is specified in degrees from -90 to 90. Positive values will angle the camera up, while negative values will angle the camera down.
		/// The default pitch of 0 is set based on on the position of the camera when the image was captured.
		/// Because of this, a pitch of 0 is often, but not always, horizontal. For example, an image taken on a hill will likely exhibit a default pitch that is not horizontal.
		/// </param>
		/// <param name="fov">
		/// Determines the horizontal field of view of the image. The field of view is expressed in degrees, with a range of 10 - 100. It defaults to 90.
		/// When dealing with a fixed-size viewport, the field of view is considered the zoom level, with smaller numbers indicating a higher level of zoom.
		/// </param>

		public static void DisplayStreetViewPanorama([CanBeNull] string panoramaId = null, [CanBeNull] LatLng viewpoint = null, int heading = 0, int pitch = 0, int fov = 90)
		{
			if (viewpoint == null && panoramaId == null)
			{
				throw new ArgumentException("Both viewpoint and panoramaId can't be null");
			}

			var uriBuilder = new StringBuilder("https://www.google.com/maps/@?api=1&map_action=pano");
			
			if (!string.IsNullOrEmpty(panoramaId))
			{
				uriBuilder.Append($"&pano={UnityWebRequest.EscapeURL(panoramaId)}");
			}

			if (viewpoint != null)
			{
				uriBuilder.Append($"&viewpoint={viewpoint.Latitude},{viewpoint.Longitude}");
			}
			
			uriBuilder.Append($"&heading={heading}");
			uriBuilder.Append($"&pitch={pitch}");
			uriBuilder.Append($"&fov={fov}");

			OpenUrl(uriBuilder.ToString());
		}

		static void OpenUrl(string uri)
		{
			if (Debug.isDebugBuild)
			{
				Debug.Log("Opening URL: " + uri);
			}

			Application.OpenURL(uri);
		}
	}
}