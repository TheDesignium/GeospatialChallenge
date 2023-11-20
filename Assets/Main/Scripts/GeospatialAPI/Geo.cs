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

    public GameObject GeospatialAssetPrefab;
	   public GameObject GeospatialAssetPrefabVisited;

    float lat;
    float lon;

    bool _tapped;

    public List<GameObject> objects = new List<GameObject>();

    public void nowStart()
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
          }
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
