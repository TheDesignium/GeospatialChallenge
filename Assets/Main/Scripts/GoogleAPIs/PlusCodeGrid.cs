using UnityEngine;
using UnityEngine.Networking;

using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using Google.XR.ARCoreExtensions.Samples.Geospatial;

using NinevaStudios.GoogleMaps;
using NinevaStudios.GoogleMaps.Internal;

using TMPro;

public class PlusCodeGrid : MonoBehaviour
{

    public GeospatialController _gc;
    public GoogleMapsDemo _google;
    public Geo _geo;

    public string api;
    public double latitude = 37.4220;
    public double longitude = -122.0841;
    public double distanceInMeters = 1000;

    public double step = 0.000125;

    public List<Vector2> latLngsList = new List<Vector2>();
    public List<string> pluscodeList = new List<string>();
    public List<string> codeList = new List<string>();

    public TMP_Text distanceText;

    public bool saveCodes;

    public string filename = "codes.txt";
    public string path;

    void Start()
    {
      path = BaseDir() + Separator() + filename;
      Debug.Log(path);
    }

    public void CheckPlusCodes()
    {
      StopCoroutine("GetPlusCode");
      StartCoroutine("GetPlusCode");
    }

    IEnumerator GetPlusCode()
    {
      _google.OnClearMapClick();
      _geo.clearAnchors();
      latLngsList.Clear();
      codeList.Clear();

#if !UNITY_EDITOR
      List<double> list = _gc.returnLatLonDbl();
      latitude = list[0];
      longitude = list[1];
#endif

      //double degreeInMeters = 111319.9;
      double degreeInMeters = 111330;

      for (double lat = latitude - 0.001; lat <= latitude + 0.001; lat += 0.0001)
      {
          for (double lng = longitude - 0.001; lng <= longitude + 0.001; lng += 0.0001)
          {
              double latDistance = (lat - latitude) * degreeInMeters;
              double lngDistance = (lng - longitude) * degreeInMeters * Mathf.Cos((float)latitude * Mathf.Deg2Rad);
              double distance = Mathf.Sqrt((float)(latDistance * latDistance + lngDistance * lngDistance));

              if (distance <= distanceInMeters)
              {
                  Vector2 latLng = new Vector2((float)lat, (float)lng);
                  latLngsList.Add(latLng);
              }
              else
              {
                //Vector2 latLng = new Vector2((float)lat, (float)lng);
                //Debug.Log("did not add: " + latLng.ToString());
              }
          }
      }

      foreach (Vector2 latLng in latLngsList)
      {
          //Debug.Log(latLng.x + ", " + latLng.y);
#if !UNITY_EDITOR
          //_google.setAPI(latLng.x, latLng.y);
#endif

        UnityWebRequest www = UnityWebRequest.Get("https://plus.codes/api?address=" + latLng.x.ToString() + "," + latLng.y.ToString() + "&ekey=" + api + "&email=" + "matt@designium.jp");

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            //Debug.Log(www.downloadHandler.text);

            pluscodeList.Clear();
            pluscodeList = www.downloadHandler.text.Split('\n').ToList();

            string code = pluscodeList[2];
            code = code.Replace("\"", "");
            code = code.Replace("global_code:", "");
            code = code.Replace(",", "");
            code = code.Replace(" ", "");
            //    "global_code": "8Q7XJPG9+6V",
            if(!codeList.Contains(code))
            {
              codeList.Add(code);

              string nelat = cleanPlusCode(pluscodeList[6]);
              string nelon = cleanPlusCode(pluscodeList[7]);
              string swlat = cleanPlusCode(pluscodeList[10]);
              string swlon = cleanPlusCode(pluscodeList[11]);
              string loclat = cleanPlusCode(pluscodeList[15]);
              string loclon = cleanPlusCode(pluscodeList[16]);

              float nelatf = 0; float nelonf = 0; float swlatf = 0; float swlonf = 0; float loclatf = 0; float loclonf = 0;

              float.TryParse(nelat, out nelatf);
              float.TryParse(nelon, out nelonf);
              float.TryParse(swlat, out swlatf);
              float.TryParse(swlon, out swlonf);
              float.TryParse(loclat, out loclatf);
              float.TryParse(loclon, out loclonf);

  #if !UNITY_EDITOR
              _google.setAreaPlusCodes(nelatf,nelonf,swlatf,swlonf,loclatf,loclonf);
  #endif
              if(saveCodes == true)
              {
                WriteToFile(code + "," + nelat+ "," +nelon+ "," +swlat+ "," +swlon+ "," +loclat+ "," +loclon);
              }
            }
        }
        //yield return new WaitForEndOfFrame();
        Resources.UnloadUnusedAssets();
      }
      yield return new WaitForEndOfFrame();
    }

    public string cleanPlusCode(string s)
    {
      var stripped = Regex.Replace(s, "[^0-9.]+", "");
      return stripped;
    }

    public void setDistance(float i)
    {
      distanceInMeters = (double)i;
      distanceText.text = "Range: " + distanceInMeters.ToString() + "m";
    }

    char Separator()
    {
        return System.IO.Path.DirectorySeparatorChar;
    }

    string BaseDir()
    {
      var base_dir = Application.persistentDataPath ;

      #if UNITY_EDITOR
         base_dir = Application.streamingAssetsPath;
      #endif

        return base_dir;
    }

    public void WriteToFile(string _strings)
    {
        string _s = path;
        string _string = _strings + "\n";
        StreamWriter sw = new StreamWriter(_s, append: true);
        sw.WriteLine(_string);
        sw.Close();
    }
}
