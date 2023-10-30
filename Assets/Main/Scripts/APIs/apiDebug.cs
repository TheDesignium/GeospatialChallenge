using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using UnityEngine;
using UnityEngine.Networking;

using TMPro;

using Google.XR.ARCoreExtensions.Samples.Geospatial;

public class apiDebug : MonoBehaviour
{

    public string elevationKey = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJjcmVkZW50aWFsX2lkIjoiY3JlZGVudGlhbHx2M0tsUlI3UzRYdjJBR1NlMktOWFBzQk9NUTVQIiwiYXBwbGljYXRpb25faWQiOiJhcHBsaWNhdGlvbnxlM25sZzNHdWRlWlF4Z2MyYnd4MkdpeGduMlE2Iiwib3JnYW5pemF0aW9uX2lkIjoiZGV2ZWxvcGVyfGwwTlEzZ0dIeExsWEQyaHowUExiUWNMYkpnUVkiLCJpYXQiOjE2Nzg2ODI4NjB9.iyYIQY3M3Px2BokYwDLzEIz1S_acBggUW3CiY6HOY5c";

    public string debuglat = "35.625634";
    public string debuglon = "139.719513";
    public double debuglatDbl = 35.625634;
    public double debuglonDbl = 139.719513;
    public double debugDistance = 1;

    public List<string> _bigList = new List<string>();

    public TMP_Text debugTxt;
    public TMP_Text debugTxtOpo;

    public bool savedata;

    public GPSGrid grid;
    // Replace with your actual API key
    public string apiKey = "AIzaSyAjN_-X0lX3NM-drGHXFAgCBIVGSRzGuQc";

    public string apiUrl = "https://airquality.googleapis.com/v1/currentConditions:lookup?key=";

    void Start()
    {
      apiUrl = "https://airquality.googleapis.com/v1/currentConditions:lookup?key=" + apiKey;
    }


    void Update()
    {
      if(Input.GetKeyUp(KeyCode.Alpha0))
      {
        StartCoroutine(airqualityLoop());
      }
      if(Input.GetKeyUp(KeyCode.Alpha9))
      {
        StartNow();
      }
    }

    IEnumerator GetText() {

      //https://solar.googleapis.com/v1/buildingInsights:findClosest?location.latitude=37.4450&location.longitude=-122.1390&requiredQuality=HIGH&key=

    //UnityWebRequest www = UnityWebRequest.Get("https://solar.googleapis.com/v1/buildingInsights:findClosest?location.latitude=" + debuglat + "&location.longitude=" + debuglon + "&key=" + elevationKey);

    UnityWebRequest www = UnityWebRequest.Get("https://solar.googleapis.com/v1/buildingInsights:findClosest?location.latitude=" + debuglat + "&location.longitude=" + debuglon + "&key=" + elevationKey);

    yield return www.SendWebRequest();

        if (www.isNetworkError) {
            Debug.Log(www.error);
        } else {
            // Show results as text
            Debug.Log(www.downloadHandler.text);
        }
    }

    void StartNow()
    {
        StartCoroutine(SendAirQualityRequest());
    }

    IEnumerator SendAirQualityRequest()
    {
        // Create a JSON object to send in the request
        string jsonRequestBody = "{\"location\": {\"latitude\": " + debuglat + ",\"longitude\": " + debuglon + "}}";

        // Create a UnityWebRequest
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");

        // Set the request method and headers
        request.method = UnityWebRequest.kHttpVerbPOST;
        request.SetRequestHeader("Content-Type", "application/json");

        // Attach the JSON data to the request
        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonRequestBody);
        request.uploadHandler = new UploadHandlerRaw(jsonBytes);
        request.downloadHandler = new DownloadHandlerBuffer();

        // Send the request
        yield return request.SendWebRequest();

        // Check for errors
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            // Parse and use the response data (request.downloadHandler.text)
            Debug.Log("Response: " + request.downloadHandler.text);
        }
    }

    IEnumerator airqualityLoop()
    {
      List<double> latlng = new List<double>();
      List<double> debuglatlng = new List<double>();
      latlng.Add(debuglatDbl);
      latlng.Add(debuglonDbl);
      yield return new WaitForSeconds(1f);
      debuglatlng = grid.ToNorthPosition(latlng, debugDistance);
      StartCoroutine(SendAirQualityRequestTwo(debuglatlng[0], debuglatlng[1]));
      yield return new WaitForSeconds(1f);
      debuglatlng = grid.ToEastPosition(latlng, debugDistance);
      StartCoroutine(SendAirQualityRequestTwo(debuglatlng[0], debuglatlng[1]));
      yield return new WaitForSeconds(1f);
      debuglatlng = grid.ToSouthPosition(latlng, debugDistance);
      StartCoroutine(SendAirQualityRequestTwo(debuglatlng[0], debuglatlng[1]));
      yield return new WaitForSeconds(1f);
      debuglatlng = grid.ToWestPosition(latlng, debugDistance);
      StartCoroutine(SendAirQualityRequestTwo(debuglatlng[0], debuglatlng[1]));
    }

    IEnumerator SendAirQualityRequestTwo(double lat, double lng)
    {
        Debug.Log(lat + ":" + lng);
        // Create a JSON object to send in the request
        string jsonRequestBody = "{\"location\": {\"latitude\": " + lat.ToString() + ",\"longitude\": " + lng.ToString() + "}," +
            "\"extra_computations\": [" +
            "\"DOMINANT_POLLUTANT_CONCENTRATION\"," +
            "\"POLLUTANT_ADDITIONAL_INFO\"" +
            "]," +
            "\"language_code\": \"en\"" +
            "}";

        // Create a UnityWebRequest
        UnityWebRequest request = new UnityWebRequest(apiUrl, "POST");

        // Set the request method and headers
        request.method = UnityWebRequest.kHttpVerbPOST;
        request.SetRequestHeader("Content-Type", "application/json");
        request.SetRequestHeader("Accept", "application/json");

        // Attach the JSON data to the request
        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(jsonRequestBody);
        request.uploadHandler = new UploadHandlerRaw(jsonBytes);
        request.downloadHandler = new DownloadHandlerBuffer();

        // Send the request
        yield return request.SendWebRequest();

        // Check for errors
        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError("Error: " + request.error);
        }
        else
        {
            // Parse and use the response data (request.downloadHandler.text)
            Debug.Log("Response: " + request.downloadHandler.text);
        }
    }

}
