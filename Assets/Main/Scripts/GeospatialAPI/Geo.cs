using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using Google.XR.ARCoreExtensions;
using Google.XR.ARCoreExtensions.Samples.Geospatial;

using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

#if UNITY_ANDROID
    using UnityEngine.XR.ARCore;
#endif // UNITY_ANDROID

public class Geo : MonoBehaviour
{
    public AREarthManager _armanager;
    public ARAnchorManager _anchors;
    public GeospatialController geoControl;
  	public setGPS gps;

    EarthState EarthState;
    TrackingState EarthTrackingState;

    public GameObject GeospatialAssetPrefab;
	   public GameObject GeospatialAssetPrefabVisited;

    float lat;
    float lon;

    bool _tapped;

    public List<GameObject> objects = new List<GameObject>();

    public void nowStart()
    {
      if(_armanager == null)
      {
        _armanager = new AREarthManager();
      }
      if(_anchors == null)
      {
        _anchors = new ARAnchorManager();
      }
      var earthTrackingState = _armanager.EarthTrackingState;
      if (earthTrackingState == TrackingState.Tracking)
      {
         var cameraGeospatialPose = _armanager.CameraGeospatialPose;
      }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void checkPosition()
    {
      var _a = _armanager.IsGeospatialModeSupported(GeospatialMode.Enabled);

      Debug.Log(_a);

      var _b = _armanager.EarthState;

      Debug.Log(_b);

      var _c = _armanager.CameraGeospatialPose;

      Debug.Log(_c);

      var earthTrackingState = _armanager.EarthTrackingState;

      Debug.Log(earthTrackingState);

      if (earthTrackingState == TrackingState.Tracking)
      {
         // Values obtained by the Geospatial API are valid as long as
         // earthTrackingState is TrackingState.Tracking.
         // Use Geospatial APIs in this block.
         var cameraGeospatialPose = _armanager.CameraGeospatialPose;
         Debug.Log("Altitude: " + cameraGeospatialPose.Altitude);
         Debug.Log("Heading: " + cameraGeospatialPose.Heading);
         Debug.Log("HeadingAccuracy: " + cameraGeospatialPose.HeadingAccuracy);
         Debug.Log("HorizontalAccuracy: " + cameraGeospatialPose.HorizontalAccuracy);
         Debug.Log("Latitude: " + cameraGeospatialPose.Latitude);
         Debug.Log("Longitude: " + cameraGeospatialPose.Longitude);
         Debug.Log("VerticalAccuracy: " + cameraGeospatialPose.VerticalAccuracy);
      }
    }

    public void startLocation()
    {
      StartCoroutine(GetLocation());
    }

    public IEnumerator GetLocation()
    {
          yield return new WaitForSeconds(1);

          Input.location.Start();

          // First, check if user has location service enabled
          if (!Input.location.isEnabledByUser)
              yield break;

          // Start service before querying location
          Input.location.Start();

          // Wait until service initializes
          int maxWait = 20;
          while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
          {
              yield return new WaitForSeconds(1);
              maxWait--;
          }

          // Service didn't initialize in 20 seconds
          if (maxWait < 1)
          {
              Debug.Log("Timed out");
              yield break;
          }

          // Connection has failed
          if (Input.location.status == LocationServiceStatus.Failed)
          {
              Debug.Log("Unable to determine device location");
              yield break;
          }
          else
          {
              lat = Input.location.lastData.latitude;
              lon = Input.location.lastData.longitude;
              Debug.Log("Location: " + lat + ":" + lon);
          }
    }

    public void parseSetAnchor(string _s)
    {
      string output = _s.Split('(', ')')[1];
      string lat = output.Remove(output.LastIndexOf(','));
      string lon = output.Substring(output.LastIndexOf(',') + 1);
      float _la = 0;
      float _lo = 0;
      float.TryParse(lat, out _la);
      float.TryParse(lon, out _lo);
    }

    public void clearAnchors()
    {
      foreach(GameObject _g in objects)
      {
        Destroy(_g);
      }
      objects.Clear();
    }


	public void setAnchorPlus(float latitude, float longitude, bool visited, string s)
  {
		StartCoroutine(plusAnchors(latitude,longitude, visited, s));
	}

	IEnumerator plusAnchors(float latitude, float longitude, bool visited, string s)
	{
		var earthTrackingState = _armanager.EarthTrackingState;

		var pose = _armanager.CameraGeospatialPose;

		yield return new WaitForEndOfFrame();

		if (earthTrackingState == TrackingState.Tracking)
		{
			Quaternion quaternion = Quaternion.identity;
			var anchor = _anchors.ResolveAnchorOnTerrain(latitude, longitude, 0, quaternion);
			if(visited == true)
			{
				var anchoredAsset = Instantiate(GeospatialAssetPrefabVisited, anchor.transform);
        anchoredAsset.name = s;
				objects.Add(anchoredAsset);
			}
			else
			{
				var anchoredAsset = Instantiate(GeospatialAssetPrefab, anchor.transform);
        anchoredAsset.name = s;
				objects.Add(anchoredAsset);
			}
		}

    }
}
