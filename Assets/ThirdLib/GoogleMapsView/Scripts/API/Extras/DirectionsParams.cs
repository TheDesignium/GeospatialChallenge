using System.Collections.Generic;
using JetBrains.Annotations;

namespace NinevaStudios.GoogleMaps
{
	/// <summary>
	/// Class to build parameters to open directions
	/// </summary>
	[PublicAPI]
	public sealed class DirectionsParams
	{
		[PublicAPI]
		public enum TravelModeType
		{
			Default,
			Driving,
			Walking,
			Bicycling,
			Transit
		}

		public bool LaunchNavigation { get; set; }

		public string Origin { get; set; }

		public string OriginPlaceId { get; set; }

		public string Destination { get; set; }

		public string DestinationPlaceId { get; set; }

		public TravelModeType TravelMode { get; set; }

		public List<string> Waypoints { get; set; }

		public List<string> WaypointPlaceIds { get; set; }

		/// <summary>
		/// Defines the starting point from which to display directions.
		/// Defaults to most relevant starting location, such as user location, if available.
		/// If none, the resulting map may provide a blank form to allow a user to enter the origin.
		/// The value can be either a place name, address, or comma-separated latitude/longitude coordinates.
		/// </summary>
		/// <param name="origin">The starting point from which to display directions.</param>
		/// <returns>The same <see cref="DirectionsParams"/> instance.</returns>
		public DirectionsParams SetOrigin(string origin)
		{
			Origin = origin;
			return this;
		}

		/// <summary>
		/// A place ID is a textual identifier that uniquely identifies a place from which to display directions.
		/// If you are trying to definitively specify an establishment, using a place ID is the best guarantee that you will link to the right place.
		/// To use this parameter you must also provide an origin.
		/// </summary>
		/// <param name="originPlaceId">The textual identifier that uniquely identifies a place from which to display directions.</param>
		/// <returns>The same <see cref="DirectionsParams"/> instance.</returns>
		public DirectionsParams SetOriginPlaceId(string originPlaceId)
		{
			OriginPlaceId = originPlaceId;
			return this;
		}

		/// <summary>
		/// Defines the endpoint of the directions.
		/// If none, the resulting map may provide a blank form to allow the user to enter the destination.
		/// The value can be either a place name, address, or comma-separated latitude/longitude coordinates.
		/// </summary>
		/// <param name="destination">The endpoint to which to display directions.</param>
		/// <returns>The same <see cref="DirectionsParams"/> instance.</returns>
		public DirectionsParams SetDestination(string destination)
		{
			Destination = destination;
			return this;
		}

		/// <summary>
		/// A place ID is a textual identifier that uniquely identifies a place to which to display directions.
		/// If you are trying to definitively specify an establishment, using a place ID is the best guarantee that you will link to the right place.
		/// To use this parameter you must also provide a destination.
		/// </summary>
		/// <param name="destinationPlaceId">The textual identifier that uniquely identifies a place to which to display directions.</param>
		/// <returns>The same <see cref="DirectionsParams"/> instance.</returns>
		public DirectionsParams SetDestinationPlaceId(string destinationPlaceId)
		{
			DestinationPlaceId = destinationPlaceId;
			return this;
		}

		/// <summary>
		/// Defines the method of travel.
		/// Options are driving, walking (which prefers pedestrian paths and sidewalks, where available), bicycling (which routes via bike paths and preferred streets where available), or transit.
		/// If no travel mode is specified, the Google Map shows one or more of the most relevant modes for the specified route and/or user preferences.
		/// </summary>
		/// <param name="travelMode">The method of travel.</param>
		/// <returns>The same <see cref="DirectionsParams"/> instance.</returns>
		public DirectionsParams SetTravelMode(TravelModeType travelMode)
		{
			TravelMode = travelMode;
			return this;
		}

		/// <summary>
		/// Defines whether to launch either turn-by-turn navigation or route preview to the specified destination.
		/// If the user specifies an origin and it is not close to the user's current location, or the user's current location is unavailable, the map launches a route preview.
		/// If the user does not specify an origin (in which case the origin defaults to the user's current location), or the origin is close to the user's current location,
		/// the map launches turn-by-turn navigation.
		/// Note that navigation is not available on all Google Maps products and/or between all destinations; in those cases this parameter will be ignored.
		/// </summary>
		/// <param name="launchNavigation">The flag indicating whether to launch either turn-by-turn navigation or route preview to the specified destination.</param>
		/// <returns>The same <see cref="DirectionsParams"/> instance.</returns>
		public DirectionsParams SetLaunchNavigation(bool launchNavigation)
		{
			LaunchNavigation = launchNavigation;
			return this;
		}

		/// <summary>
		/// Specifies one or more intermediary places to route directions through between the origin and destination.
		/// The number of waypoints allowed varies by the platform where the link opens, with up to three waypoints supported on mobile browsers, and a maximum of nine waypoints supported otherwise.
		/// Waypoints are displayed on the map in the same order they are listed.
		/// Each waypoint can be either a place name, address, or comma-separated latitude/longitude coordinates.
		/// </summary>
		/// <param name="waypoints">The list of intermediary places to route directions through between the origin and destination.</param>
		/// <returns>The same <see cref="DirectionsParams"/> instance.</returns>
		public DirectionsParams SetWaypoints(List<string> waypoints)
		{
			Waypoints = waypoints;
			return this;
		}

		/// <summary>
		/// Allows to provide a list of place IDs to match the list of waypoints.
		/// If you are trying to definitively specify certain establishments, place IDs are the best guarantee that you will link to the right places.
		/// To use this parameter you must also provide waypoints.
		/// </summary>
		/// <param name="waypointPlaceIds">The list of place IDs to match the list of waypoints.</param>
		/// <returns>The same <see cref="DirectionsParams"/> instance.</returns>
		public DirectionsParams SetWaypointPlaceIds(List<string> waypointPlaceIds)
		{
			WaypointPlaceIds = waypointPlaceIds;
			return this;
		}

		public string GetTravelMode()
		{
			if (TravelMode == TravelModeType.Default)
			{
				return string.Empty;
			}

			return TravelMode.ToString().ToLower();
		}
	}
}