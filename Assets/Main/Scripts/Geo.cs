using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using Google.XR.ARCoreExtensions;
using Google.XR.ARCoreExtensions.Samples.Geospatial;
//using Google.XR.ARCoreExtensions.AREarthManager;

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

    EarthState EarthState;

    TrackingState EarthTrackingState;

    public GameObject GeospatialAssetPrefab;
    public GameObject APIGeospatialAssetPrefab;
    public GameObject debugPrefab;

    float lat;
    float lon;

    public bool placeOnStart;
    public bool useTerrain;
    public bool useAddress;
    public bool makeAnchorParent;

    bool _tapped;

    public GameObject theprefab;
    public GameObject[] GeospatialAssetPrefabs;

    public List<GameObject> objects = new List<GameObject>();
    public GameObject lastObject;

    lookAt look;

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
         // Values obtained by the Geospatial API are valid as long as
         // earthTrackingState is TrackingState.Tracking.
         // Use Geospatial APIs in this block.
         var cameraGeospatialPose = _armanager.CameraGeospatialPose;
         //Debug.Log(cameraGeospatialPose);
      }

      if(placeOnStart == true)
      {
        startLocation();
      }
    }

    // Update is called once per frame
    void Update()
    {

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

              setAnchor(lat,lon);
          }
    }

    public void setAnchor(float latitude, float longitude)
    {
      var earthTrackingState = _armanager.EarthTrackingState;

      Debug.Log(earthTrackingState);

      var pose = _armanager.CameraGeospatialPose;

      Debug.Log(pose);

      if (earthTrackingState == TrackingState.Tracking)
      {
        Quaternion quaternion = Quaternion.identity;

        var altitude = pose.Altitude - 1;

        var anchor =
            _anchors.AddAnchor(
                latitude,
                longitude,
                altitude,
                quaternion);
        var anchoredAsset = Instantiate(GeospatialAssetPrefab, anchor.transform);
      }
    }

	public void setAnchorDebug(float latitude, float longitude, string name, string rating, string icon, string photo)
    {
		    Vector3 v3 = new Vector3(UnityEngine.Random.Range(-10,10),0,UnityEngine.Random.Range(-10,10));
		    var anchoredAsset = Instantiate(APIGeospatialAssetPrefab, v3, transform.rotation);
	  }

    public void clearAnchors()
    {
      foreach(GameObject _g in objects)
      {
        Destroy(_g);
      }
      objects.Clear();
    }

    public void setAnchorNavi(float latitude, float longitude, float altitude)
    {
		    StartCoroutine(naviAnchors(latitude,longitude, altitude));
	}

	IEnumerator naviAnchors(float latitude, float longitude, float altitude)
	{
		var earthTrackingState = _armanager.EarthTrackingState;

		var pose = _armanager.CameraGeospatialPose;

		float alt = altitude;

		yield return new WaitForEndOfFrame();

		if (earthTrackingState == TrackingState.Tracking)
		{
			Quaternion quaternion = Quaternion.identity;

      if(useTerrain == true)
      {
        var anchor = _anchors.ResolveAnchorOnTerrain(latitude, longitude, 0, quaternion);
        var anchoredAsset = Instantiate(GeospatialAssetPrefab, anchor.transform);
        objects.Add(anchoredAsset);
        if(look != null)
        {
          look.thePlayer = anchoredAsset.transform;
        }
        look = anchoredAsset.transform.GetChild(0).GetComponent<lookAt>();
      }
      else
      {
        var anchor =
          _anchors.AddAnchor(
            latitude,
            longitude,
            alt,
            quaternion);
        var anchoredAsset = Instantiate(GeospatialAssetPrefab, anchor.transform);
        objects.Add(anchoredAsset);
        if(look != null)
        {
          look.thePlayer = anchoredAsset.transform;
        }
        look = anchoredAsset.transform.GetChild(0).GetComponent<lookAt>();
      }
		}
  }
}
