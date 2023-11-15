using System;
using System.Linq;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using UnityEngine;
using UnityEngine.Networking;

using Google.XR.ARCoreExtensions.Samples.Geospatial;

using NinevaStudios.GoogleMaps;
using NinevaStudios.GoogleMaps.Internal;

using TMPro;

public class directionsPOI : MonoBehaviour
{

	public GoogleMapsDemo google;
	public geoDetail geo;
	public getDistance distance;
	public travelAPI details;

	public GameObject loadingUI;

  public string transitMode;
  public string latfrom;
  public string lonfrom;
	public string latto;
  public string lonto;
  public string api;

	public float latF;
	public float lonF;
	public float latT;
	public float lonT;

	public List<LatLng> latlngList = new List<LatLng>();
	public List<LatLng> POIlatlngList = new List<LatLng>();

  public double intervalDistance = 20.0; // Interval distance in meters
  public double latitudeA = 40.7128; // Latitude of GPS coordinate A
  public double longitudeA = -74.0060; // Longitude of GPS coordinate A
  public double latitudeB = 37.7749; // Latitude of GPS coordinate B
  public double longitudeB = -122.4194; // Longitude of GPS coordinate B

	List<double> interpolatedLatitudes = new List<double>();
	List<double> interpolatedLongitudes = new List<double>();
	public List<double> allLatitudes = new List<double>();
	public List<double> allLongitudes = new List<double>();

	List<POIDetails> poiList = new List<POIDetails>();

	public float POIDistanceLimit;

	public int poiPoint;

	public bool miniMap;
	public bool ARNavigation;

	public TMP_Text resultText;
	public TMP_Text statusText;

    private void Start()
    {
			if(miniMap == true)
			{
				StartCoroutine(GetLocation(false));
			}
    }

	void Update()
	{
		if(Input.GetKeyUp(KeyCode.Z))
		{
			StartCoroutine(GetDirections(latfrom,lonfrom,latto,lonto));
		}
		if(Input.GetKeyUp(KeyCode.X))
		{
			startOnTap(latitudeB,longitudeB);
		}
	}

	public void startOnTap(double lt, double ln)
	{
		details.hasChecked = false;
		details.haveCheckedList.Clear();
		details.resetButton.interactable = false;
		latto = lt.ToString();
		lonto = ln.ToString();
		latT = (float)lt;
		lonT = (float)ln;
		loadingUI.SetActive(true);
		StartCoroutine(GetLocation(true));
	}

	public void cleanAll()
	{
		StopAllCoroutines();
		google.OnClearMapClick();
		geo.clearAll();
		details.hasChecked = false;
		details.haveCheckedList.Clear();
		details.resetButton.interactable = false;
	}

	IEnumerator checkPOIs()
	{
		statusText.gameObject.SetActive(true);
		statusText.text = "Loading . . .";
#if !UNITY_EDITOR
		google.OnClearMapClick();
		geo.clearAll();
#endif
		poiList.Clear();
		POIlatlngList.Clear();
		poiPoint = 0;

		float total = distance.CalculateDistance(latF, latT, lonF, lonT);

		yield return new WaitForEndOfFrame();

		details.radius = total.ToString();

		statusText.text = "Getting Directions . . .";
		yield return StartCoroutine(GetDirections(latfrom, lonfrom, latto, lonto));

		yield return new WaitForEndOfFrame();

		details.lat = allLatitudes[allLatitudes.Count()/2].ToString();
		details.lon = allLongitudes[allLatitudes.Count()/2].ToString();

		yield return new WaitForEndOfFrame();

		statusText.text = "Getting Points of Interest . . .";

		yield return details.GetDetails(false);

		yield return new WaitForEndOfFrame();

		if(details.latitudesList.Count() == 0)
		{
			UnityEngine.Debug.Log("No POIs");
			StartCoroutine(fadeOutText("No points of interest foundalong this route."));
			loadingUI.SetActive(false);
			yield break;
		}

		List<double> detaillatitudesList = new List<double>();
		List<double> detaillongitudesList = new List<double>();
		detaillatitudesList = details.latitudesList;
		detaillongitudesList = details.longitudesList;

		yield return new WaitForEndOfFrame();

		float mindist = 999999999999999f;
		int poicounter = 0;

		for (int i = 0; i < detaillatitudesList.Count; i++)
      {
				for (int o = 0; o < allLatitudes.Count; o++)
				{
					float d = distance.CalculateDistance((float)detaillatitudesList[i], (float)allLatitudes[o], (float)detaillongitudesList[i], (float)allLongitudes[o]);

					if(d < POIDistanceLimit)
					{
						if(d < mindist)
						{
							mindist = d;
							poiPoint = poicounter;
						}
						POIDetails poid = new POIDetails();
						poid.distance = d;
						poid.routePoint = new LatLng(allLatitudes[o],allLongitudes[o]);
						poid.POI = new LatLng(detaillatitudesList[i],detaillongitudesList[i]);
						poiList.Add(poid);
					}
					poicounter += 1;
				}
				//yield return new WaitForEndOfFrame();
      }

		if(poiList.Count != 0)
		{
			foreach(POIDetails p in poiList)
			{
				if(!POIlatlngList.Contains(p.POI))
				{
					POIlatlngList.Add(p.POI);
				}
			}
			foreach(LatLng ll in POIlatlngList)
			{
				UnityEngine.Debug.Log(ll);
				geo.setAnchorLatLng(ll, details.catindex);
#if !UNITY_EDITOR
				google.setPOI(ll, details.catindex);
#endif
			}
		}
		else
		{
			UnityEngine.Debug.Log("No POIs within range");
			StartCoroutine(fadeOutText("No results within range of the route."));
			loadingUI.SetActive(false);
		}

		Resources.UnloadUnusedAssets();
		loadingUI.SetActive(false);
		statusText.gameObject.SetActive(false);
		StartCoroutine(fadeOutText("Complete"));

	}

	public void recheck()
	{
		StopCoroutine("recheckPOIs");
		StartCoroutine("recheckPOIs");
	}

	IEnumerator recheckPOIs()
	{
		loadingUI.SetActive(true);

		yield return details.GetDetails(false);

		yield return new WaitForEndOfFrame();

		if(details.latitudesList.Count() == 0)
		{
			UnityEngine.Debug.Log("No POIs");
			StartCoroutine(fadeOutText("No results found in this area."));
			loadingUI.SetActive(false);
			yield break;
		}

		List<double> detaillatitudesList = new List<double>();
		List<double> detaillongitudesList = new List<double>();
		detaillatitudesList = details.latitudesList;
		detaillongitudesList = details.longitudesList;

		yield return new WaitForEndOfFrame();

		float mindist = 999999999999999f;
		int poicounter = 0;

		for (int i = 0; i < detaillatitudesList.Count; i++)
			{
				for (int o = 0; o < allLatitudes.Count; o++)
				{
					float d = distance.CalculateDistance((float)detaillatitudesList[i], (float)allLatitudes[o], (float)detaillongitudesList[i], (float)allLongitudes[o]);

					if(d < POIDistanceLimit)
					{
						if(d < mindist)
						{
							mindist = d;
							poiPoint = poicounter;
						}
						POIDetails poid = new POIDetails();
						poid.distance = d;
						poid.routePoint = new LatLng(allLatitudes[o],allLongitudes[o]);
						poid.POI = new LatLng(detaillatitudesList[i],detaillongitudesList[i]);
						poiList.Add(poid);
					}
					poicounter += 1;
				}
				yield return new WaitForEndOfFrame();
			}

		if(poiList.Count != 0)
		{
			foreach(POIDetails p in poiList)
			{
				if(!POIlatlngList.Contains(p.POI))
				{
					POIlatlngList.Add(p.POI);
				}
			}
			foreach(LatLng ll in POIlatlngList)
			{
				UnityEngine.Debug.Log(ll);
				geo.setAnchorLatLng(ll, details.catindex);
	#if !UNITY_EDITOR
				google.setPOI(ll, details.catindex);
	#endif
			}
		}
		else
		{
			UnityEngine.Debug.Log("No POIs within range");
			StartCoroutine(fadeOutText("No results within range of the route."));
			loadingUI.SetActive(false);
		}

		Resources.UnloadUnusedAssets();
		loadingUI.SetActive(false);
	}

	public void setRadius(float f)
	{
		POIDistanceLimit = f;
	}

    private void CalculateInterpolatedGPSPoints(ref List<double> latitudes, ref List<double> longitudes)
    {
        double totalDistance = DistanceBetweenGPSPoints(latitudeA, longitudeA, latitudeB, longitudeB); // Total distance between the two GPS points
        double currentDistance = 0.0; // Current distance traveled
        double currentInterval = intervalDistance; // Current interval distance

        while (currentDistance < totalDistance)
        {
            double interpolatedLatitude, interpolatedLongitude;
            InterpolateGPSPoint(latitudeA, longitudeA, latitudeB, longitudeB, currentDistance, out interpolatedLatitude, out interpolatedLongitude);
            latitudes.Add(interpolatedLatitude);
            longitudes.Add(interpolatedLongitude);

            currentDistance += intervalDistance;
            currentInterval += intervalDistance;
        }

				for (int i = 0; i < interpolatedLatitudes.Count; i++)
        {
            UnityEngine.Debug.Log("Interpolated GPS Point: (" + interpolatedLatitudes[i] + ", " + interpolatedLongitudes[i] + ")");
        }
    }

	private void CalculateInterpolatedGPSPointsDoubles(double latitudeF, double longitudeF, double latitudeT, double longitudeT)
    {
        double totalDistance = DistanceBetweenGPSPoints(latitudeF, longitudeF, latitudeT, longitudeT); // Total distance between the two GPS points
        double currentDistance = 0.0; // Current distance traveled
        double currentInterval = intervalDistance; // Current interval distance

        while (currentDistance < totalDistance)
        {
            double interpolatedLatitude, interpolatedLongitude;
            InterpolateGPSPoint(latitudeF, longitudeF, latitudeT, longitudeT, currentDistance, out interpolatedLatitude, out interpolatedLongitude);
            allLatitudes.Add(interpolatedLatitude);
            allLongitudes.Add(interpolatedLongitude);
            currentDistance += intervalDistance;
            currentInterval += intervalDistance;
        }
    }

    private double DistanceBetweenGPSPoints(double latitude1, double longitude1, double latitude2, double longitude2)
    {
        double dLat = Mathf.Deg2Rad * (float)(latitude2 - latitude1);
        double dLon = Mathf.Deg2Rad * (float)(longitude2 - longitude1);
        double a = Mathf.Sin((float)(dLat / 2.0)) * Mathf.Sin((float)(dLat / 2.0)) +
                   Mathf.Cos((float)(Mathf.Deg2Rad * latitude1)) * Mathf.Cos((float)(Mathf.Deg2Rad * latitude2)) *
                   Mathf.Sin((float)(dLon / 2.0)) * Mathf.Sin((float)(dLon / 2.0));
        double c = 2.0 * Mathf.Atan2(Mathf.Sqrt((float)a), Mathf.Sqrt((float)(1.0 - a)));
        double distance = 6371000.0 * c; // Earth radius in meters

        return distance;
    }

    private void InterpolateGPSPoint(double latitude1, double longitude1, double latitude2, double longitude2, double distance,
                                     out double interpolatedLatitude, out double interpolatedLongitude)
    {
        double bearing = Mathf.Atan2(Mathf.Sin((float)(Mathf.Deg2Rad * (float)(longitude2 - longitude1))) * Mathf.Cos((float)(Mathf.Deg2Rad * latitude2)),
                                    Mathf.Cos((float)(Mathf.Deg2Rad * latitude1)) * Mathf.Sin((float)(Mathf.Deg2Rad * latitude2)) -
                                    Mathf.Sin((float)(Mathf.Deg2Rad * latitude1)) * Mathf.Cos((float)(Mathf.Deg2Rad * latitude2)) *
                                    Mathf.Cos((float)(Mathf.Deg2Rad * (float)(longitude2 - longitude1))));

        double angularDistance = distance / 6371000.0; // Earth radius in meters
        double latitude = Mathf.Asin(Mathf.Sin((float)(Mathf.Deg2Rad * latitude1)) * Mathf.Cos((float)(angularDistance)) +
                                     Mathf.Cos((float)(Mathf.Deg2Rad * latitude1)) * Mathf.Sin((float)(angularDistance)) *
                                     Mathf.Cos((float)(bearing)));
        double longitude = Mathf.Deg2Rad * (float)longitude1 +
                           Mathf.Atan2(Mathf.Sin((float)(bearing)) * Mathf.Sin((float)(angularDistance)) *
                                       Mathf.Cos((float)(Mathf.Deg2Rad * latitude1)),
                                       Mathf.Cos((float)(angularDistance)) - Mathf.Sin((float)(Mathf.Deg2Rad * latitude1)) *
                                       Mathf.Sin((float)(latitude)));

        interpolatedLatitude = Mathf.Rad2Deg * (float)latitude;
        interpolatedLongitude = Mathf.Rad2Deg * (float)longitude;
    }

	IEnumerator GetDirections(string fromLat, string fromLon, string toLat, string toLon)
	{
		allLatitudes.Clear();
		allLongitudes.Clear();

		UnityEngine.Debug.Log("origin=" + fromLat.ToString() + "," + fromLon.ToString() + "&destination=" + toLat.ToString() + "," + toLon.ToString());

		UnityWebRequest www = UnityWebRequest.Get("https://maps.googleapis.com/maps/api/directions/json?origin=" + fromLat.ToString() + "," + fromLon.ToString() + "&destination=" + toLat.ToString() + "," + toLon.ToString() + "&mode=" + transitMode + "&key=" + api);

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.ConnectionError)
				{
            UnityEngine.Debug.Log(www.error);
        }
				else
				{
						UnityEngine.Debug.Log(www.downloadHandler.text);

						GoogleMapsResponse response = JsonUtility.FromJson<GoogleMapsResponse>(www.downloadHandler.text);

            latlngList.Clear();

						latlngList = ExtractAllLatLngSteps(www.downloadHandler.text);

						UnityEngine.Debug.Log(response.routes[0].legs[0].steps.Count());

						LatLng previousLL = latlngList[0];

						yield return new WaitForEndOfFrame();

#if !UNITY_EDITOR
            google.setRoute(latlngList);
#endif

						yield return new WaitForEndOfFrame();

            int counter = 0;
            foreach(LatLng l in latlngList)
            {
              if(counter > 0)
              {
                float d = distance.CalculateDistance((float)previousLL.Latitude, (float)l.Latitude, (float)previousLL.Longitude, (float)l.Longitude);
                if(d > intervalDistance)
                {
									CalculateInterpolatedGPSPointsDoubles(previousLL.Latitude, previousLL.Longitude, latlngList[counter].Latitude, latlngList[counter].Longitude);
   								previousLL = latlngList[counter];
									if(ARNavigation == true)
									{
										geo.setAnchorNavi((float)previousLL.Latitude,(float)previousLL.Longitude);
									}
                }
                else
                {
                  //UnityEngine.Debug.Log("don't place anchor");
                }
              }
              counter += 1;
              yield return new WaitForEndOfFrame();
            }

        }

    }

	public List<LatLng> DecodePolylinePoints(string encodedPoints)
    {
        if (encodedPoints == null || encodedPoints == "") return null;
        List<LatLng> poly = new List<LatLng>();
        char[] polylinechars = encodedPoints.ToCharArray();
        int index = 0;

        int currentLat = 0;
        int currentLng = 0;
        int next5bits;
        int sum;
        int shifter;

        try
        {
            while (index < polylinechars.Length)
            {
                // calculate next latitude
                sum = 0;
                shifter = 0;
                do
                {
                    next5bits = (int)polylinechars[index++] - 63;
                    sum |= (next5bits & 31) << shifter;
                    shifter += 5;
                } while (next5bits >= 32 && index < polylinechars.Length);

                if (index >= polylinechars.Length)
                    break;

                currentLat += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

                //calculate next longitude
                sum = 0;
                shifter = 0;
                do
                {
                    next5bits = (int)polylinechars[index++] - 63;
                    sum |= (next5bits & 31) << shifter;
                    shifter += 5;
                } while (next5bits >= 32 && index < polylinechars.Length);

                if (index >= polylinechars.Length && next5bits >= 32)
                    break;

                currentLng += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);
                double latp = Convert.ToDouble(currentLat) / 100000.0;
                double lonp = Convert.ToDouble(currentLng) / 100000.0;
                LatLng p = new LatLng(latp,lonp);

                poly.Add(p);

            }
        }
        catch (Exception ex)
        {
            UnityEngine.Debug.Log(ex);
        }
        return poly;
   }

	public List<LatLng> ExtractAllLatLngSteps(string json)
	{
			GoogleMapsResponse response = JsonUtility.FromJson<GoogleMapsResponse>(json);
			List<LatLng> allPoints = new List<LatLng>();

			if (response.status == "OK" && response.routes != null && response.routes.Length > 0)
			{
					foreach (var step in response.routes[0].legs[0].steps)
					{
						if(step.polyline != null)
						{
							UnityEngine.Debug.Log(step.polyline.points);
							List<LatLng> stepPoints = DecodePolylinePoints(step.polyline.points);
							allPoints.AddRange(stepPoints);
						}
					}
			}
			else
			{
					UnityEngine.Debug.LogWarning("No routes found or status is not OK.");
			}

			return allPoints;
	}

	 public IEnumerator GetLocation(bool routeRequest)
	 {
				 yield return new WaitForSeconds(1);
#if !UNITY_EDITOR
				 Input.location.Start();

			 if (!Input.location.isEnabledByUser)
					 yield break;

			 Input.location.Start();

			 int maxWait = 20;
			 while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
			 {
					 yield return new WaitForSeconds(1);
					 maxWait--;
			 }

			 if (maxWait < 1)
			 {
					 UnityEngine.Debug.Log("Timed out");
					 yield break;
			 }

			 if (Input.location.status == LocationServiceStatus.Failed)
			 {
					 UnityEngine.Debug.Log("Unable to determine device location");
					 yield break;
			 }
			 else
			 {
				 if(routeRequest == false)
				 {
					 google.setbyGPS(Input.location.lastData.latitude, Input.location.lastData.longitude);
						yield return new WaitForEndOfFrame();
					 google.gameObject.SetActive(true);
				 }
				 else
				{
					latfrom = Input.location.lastData.latitude.ToString();
					lonfrom = Input.location.lastData.longitude.ToString();
					latF = Input.location.lastData.latitude;
					lonF = Input.location.lastData.longitude;
					yield return new WaitForEndOfFrame();
					StartCoroutine(checkPOIs());
				}
			 }
#endif
#if UNITY_EDITOR
			if(routeRequest == true)
			{
				latfrom = latitudeA.ToString();
				lonfrom = longitudeB.ToString();
				latF = (float)latitudeA;
				lonF = (float)longitudeB;
				StartCoroutine(checkPOIs());
			}
#endif

	 }

	 IEnumerator fadeOutText(string s)
	 {
		 resultText.text = s;
		 var spin = Color.white;
		 float alph = spin.a;
		 resultText.color = spin;
		 resultText.gameObject.SetActive(true);
		 yield return new WaitForEndOfFrame();
		 while(alph > 0)
		 {
			 alph = spin.a;
			 alph -= 0.003f;
			 spin.a = alph;
			 resultText.color = spin;
			 yield return new WaitForEndOfFrame();
		 }
		 yield return new WaitForEndOfFrame();
		 resultText.gameObject.SetActive(false);
	 }
}

public class POIDetails
{
	public float distance;
	public LatLng routePoint;
	public LatLng POI;
}

public class GoogleMapsResponse
{
    public GeocodedWaypoint[] geocoded_waypoints;
    public Route[] routes;
    public string status;
}

[System.Serializable]
public class GeocodedWaypoint
{
    public string geocoder_status;
    public string place_id;
    public string[] types;
}

[System.Serializable]
public class Route
{
    public string bounds;
    public string copyrights;
    public Leg[] legs;
    public EncodedPolyline overview_polyline;
    public string summary;
    public string[] warnings;
    public int[] waypoint_order;
}

[System.Serializable]
public class Location
{
    public double lat;
    public double lng;
}

[System.Serializable]
public class Leg
{
    public Distance distance;
    public Duration duration;
    public string end_address;
    public Location end_location;
    public string start_address;
    public Location start_location;
    public Step[] steps;
    public object[] traffic_speed_entry;
    public object[] via_waypoint;
}

[System.Serializable]
public class Distance
{
    public string text;
    public int value;
}

[System.Serializable]
public class Duration
{
    public string text;
    public int value;
}

[System.Serializable]
public class Step
{
    public Distance distance;
    public Duration duration;
    public Location end_location;
    public string html_instructions;
    public EncodedPolyline polyline;
    public Location start_location;
    public string travel_mode;
}

[Serializable]
public class EncodedPolyline
{
    public string points; // This is the string that contains the encoded polyline
}
