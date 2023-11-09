using NinevaStudios.GoogleMaps;
using UnityEngine;

public class GoogleMapsMinimalDemo : MonoBehaviour
{
	void Start()
	{
		var cameraPosition = new CameraPosition(
			new LatLng(51.5285582f, -0.2416799f), 10, 0, 0);
		var options = new GoogleMapsOptions()
			.Camera(cameraPosition);

		GoogleMapsView.CreateAndShow(options, new Rect(0, 0, Screen.width / 2, Screen.height / 2), OnMapReady);
	}

	static void OnMapReady(GoogleMapsView map)
	{
		Debug.Log("The map is ready!");
		
		// now that the map is ready we can add new things
		var londonMarkerOptions = new MarkerOptions()
			.Position(new LatLng(51.5285582f, -0.2416799))
			.Title("My marker in London");
		var markerInLondon = map.AddMarker(londonMarkerOptions);
		
		Debug.Log("Here is my marker in London: " + markerInLondon);
	}
}