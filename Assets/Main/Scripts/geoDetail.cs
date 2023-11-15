using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

using Google.XR.ARCoreExtensions;
using Google.XR.ARCoreExtensions.Samples.Geospatial;

#if UNITY_ANDROID
    using UnityEngine.XR.ARCore;
#endif // UNITY_ANDROID

using NinevaStudios.GoogleMaps;
using NinevaStudios.GoogleMaps.Internal;

using TMPro;

public class geoDetail : MonoBehaviour
{

  public AREarthManager _armanager;
  public ARAnchorManager _anchors;
  EarthState EarthState;
  TrackingState EarthTrackingState;
  public GameObject GeospatialAssetPrefab;
  public GameObject GeospatialNaviPrefab;
  public List<GameObject> objects = new List<GameObject>();
  public Material[] materials;
  lookAt look;
  int counter;

    void Start()
    {

    }

    public void clearAll()
    {
        foreach (var anchor in objects)
        {
            Destroy(anchor);
        }
        objects.Clear();
        counter = 1;
     }

     public void setAnchorLatLng(LatLng ll, int catindex)
     {
       setAnchorDouble(ll.Latitude, ll.Longitude, catindex);
     }

      public void setAnchorDouble(double latitude, double longitude, int catindex)
        {
    #if !UNITY_EDITOR
          var earthTrackingState = _armanager.EarthTrackingState;
          if (earthTrackingState == TrackingState.Tracking)
          {
            Quaternion quaternion = Quaternion.identity;
            var anchor = _anchors.ResolveAnchorOnTerrain(latitude, longitude, 0, quaternion);
            var anchoredAsset = Instantiate(GeospatialAssetPrefab, anchor.transform);
            anchoredAsset.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<Renderer>().material = materials[catindex];
            objects.Add(anchoredAsset);
          }
    #endif
    #if UNITY_EDITOR
          var anchoredAsset = Instantiate(GeospatialAssetPrefab, transform);
          anchoredAsset.transform.GetChild(0).GetChild(0).GetChild(0).gameObject.GetComponent<Renderer>().material = materials[catindex];
          objects.Add(anchoredAsset);
    #endif
        }


        public void setAnchorNavi(float latitude, float longitude)
        {
#if !UNITY_EDITOR
          var earthTrackingState = _armanager.EarthTrackingState;

          if (earthTrackingState == TrackingState.Tracking)
          {
            Quaternion quaternion = Quaternion.identity;

            var anchor = _anchors.ResolveAnchorOnTerrain(latitude, longitude, 0, quaternion);
            var anchoredAsset = Instantiate(GeospatialNaviPrefab, anchor.transform);
            objects.Add(anchoredAsset);
            if(look != null)
            {
              look.thePlayer = anchoredAsset.transform;
            }
            look = anchoredAsset.transform.GetChild(0).GetComponent<lookAt>();
            anchoredAsset.transform.GetChild(1).GetChild(0).gameObject.GetComponent<TMP_Text>().text = counter.ToString();
          }
#endif
#if UNITY_EDITOR
          Vector3 v3 = new Vector3(Random.Range(-20,20),0,Random.Range(10,40));
          var anchoredAsset = Instantiate(GeospatialNaviPrefab, v3, Quaternion.identity);
          objects.Add(anchoredAsset);
          if(look != null)
          {
            look.thePlayer = anchoredAsset.transform;
          }
          look = anchoredAsset.transform.GetChild(0).GetComponent<lookAt>();
          anchoredAsset.transform.GetChild(1).GetChild(0).gameObject.GetComponent<TMP_Text>().text = counter.ToString();
#endif
          counter += 1;
        }
}
