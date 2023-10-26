using UnityEngine;
using System.Collections.Generic;
using JetBrains.Annotations;
using NinevaStudios.GoogleMaps;

public class OpenGoogleMapsHelperDemo : MonoBehaviour
{
	#region search

	[UsedImplicitly]
	public void OnLocationSearchByPlaceName()
	{
		OpenGoogleMapHelper.OpenSearch("CenturyLink Field");
	}

	[UsedImplicitly]
	public void OnLocationSearchByPlaceId()
	{
		OpenGoogleMapHelper.OpenSearch("47.5951518,-122.3316393", "ChIJKxjxuaNqkFQR3CK6O1HNNqY");
	}

	[UsedImplicitly]
	public void OnLocationSearchByCoordinates()
	{
		OpenGoogleMapHelper.OpenSearch("47.5951518,-122.3316393");
	}

	[UsedImplicitly]
	public void OnLocationSearchByCategory()
	{
		OpenGoogleMapHelper.OpenSearch("pizza seattle wa");
	}

	#endregion

	#region directions

	[UsedImplicitly]
	public void OnDirectionsSearchByPlaceNames()
	{
		var directionParams = new DirectionsParams();
		directionParams.SetOrigin("Space Needle Seattle");
		directionParams.SetDestination("Pike Place Marker Seattle");
		directionParams.SetTravelMode(DirectionsParams.TravelModeType.Bicycling);

		OpenGoogleMapHelper.OpenDirections(directionParams);
	}

	[UsedImplicitly]
	public void OnDirectionsSearchByPlaceIds()
	{
		var directionParams = new DirectionsParams();
		directionParams.SetOrigin("Google Pyrmont NSW");
		directionParams.SetDestination("QVB");
		directionParams.SetDestinationPlaceId("ChIJISz8NjyuEmsRFTQ9Iw7Ear8");
		directionParams.SetTravelMode(DirectionsParams.TravelModeType.Transit);

		OpenGoogleMapHelper.OpenDirections(directionParams);
	}

	[UsedImplicitly]
	public void OnDirectionsSearchByCoordinates()
	{
		var directionParams = new DirectionsParams();
		directionParams.SetOrigin("47.5951518,-122.3316393");
		directionParams.SetDestination("47.5989513,-122.3374653");
		directionParams.SetTravelMode(DirectionsParams.TravelModeType.Driving);

		OpenGoogleMapHelper.OpenDirections(directionParams);
	}

	[UsedImplicitly]
	public void OnDirectionsSearchWithWaypoints()
	{
		var directionParams = new DirectionsParams();
		directionParams.SetOrigin("CenturyLink Field");
		directionParams.SetDestination("City Hall Park Seattle");
		directionParams.SetTravelMode(DirectionsParams.TravelModeType.Walking);

		var waypoints = new List<string>
		{
			"Klondike Gold Rush National Historical Park Seattle",
			"Smith Tower",
			"Seattle Passport Agency"
		};

		directionParams.SetWaypoints(waypoints);

		OpenGoogleMapHelper.OpenDirections(directionParams);
	}

	[UsedImplicitly]
	public void OnDirectionsSearchWithWaypointIds()
	{
		var directionParams = new DirectionsParams();
		directionParams.SetOrigin("CenturyLink Field");
		directionParams.SetDestination("City Hall Park Seattle");
		directionParams.SetTravelMode(DirectionsParams.TravelModeType.Walking);

		var waypoints = new List<string>
		{
			"Klondike Gold Rush National Historical Park Seattle",
			"Smith Tower",
			"Seattle Passport Agency"
		};

		var waypointIds = new List<string>
		{
			"ChIJBbqpNLtqkFQRyLNYioGRcF8",
			"ChIJ6x3yk7pqkFQRW2zXQJUlScA",
			"ChIJvZiGw-V3CEER2qnjz1yUdW4"
		};

		directionParams.SetWaypoints(waypoints);
		directionParams.SetWaypointPlaceIds(waypointIds);

		OpenGoogleMapHelper.OpenDirections(directionParams);
	}

	#endregion

	#region display map

	[UsedImplicitly]
	public void OnOpenMapDefault()
	{
		OpenGoogleMapHelper.DisplayMapDefault();
	}

	[UsedImplicitly]
	public void OnOpenMapCustom()
	{
		OpenGoogleMapHelper.DisplayMap(new LatLng(-33.8569, 151.2152), 10, OpenGoogleMapHelper.MapType.Satellite, OpenGoogleMapHelper.MapLayer.Bicycling);
	}

	#endregion

	#region street view

	[UsedImplicitly]
	public void OnDisplayStreetViewPanaroma()
	{
		OpenGoogleMapHelper.DisplayStreetViewPanorama("tu510ie_z4ptBZYo2BGEJg", new LatLng(48.8578261, 2.2952414), -45, 38, 80);
	}

	#endregion
}