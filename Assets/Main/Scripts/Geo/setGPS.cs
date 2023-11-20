using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

using SimpleJSON;

using Google.XR.ARCoreExtensions.Samples.Geospatial;

using NinevaStudios.GoogleMaps;
using NinevaStudios.GoogleMaps.Internal;

using TMPro;

using GoogleTextToSpeech.Scripts.Example;

public class setGPS : MonoBehaviour
{

    public GoogleMapsDemo _google;
    public Geo _geo;
    public GeospatialController _gc;
	  public GPSGrid grid;
    public LoadKML kml;
	  public SquareAPI square;
    public TextToSpeechExample text2speech;

    public string debugString;

    public string lat;
    public string lon;
    public string radius;
    public string api;
    public string heading;

	  float latf;
	  float lonf;

    public bool enableMinimap;

    public List<string> pluscodeList = new List<string>();
    public List<string> geocodeList = new List<string>();
    public List<string> addressList = new List<string>();

  	public double gridDistance;


  	public TMP_Text plusCodeTxt;
    public TMP_Text detailTxt;
    public TMP_Text rewardText;

    public Material debugMaterial;

    public GameObject plusdebug;

    void Start()
    {
      #if UNITY_EDITOR
        latf = 35.625626f;
        lonf = 139.719521f;
      #endif

       StartCoroutine(GetLocation());
    }

    // Update is called once per frame
    void Update()
    {
      if(Input.GetKeyUp(KeyCode.T))
  		{
        getPlusCodeVisited(lat,lon);
  		}
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
            // Access granted and location value could be retrieved
            Debug.Log("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);

            latf = Input.location.lastData.latitude;
            lonf = Input.location.lastData.longitude;

            lat = latf.ToString();
            lon = lonf.ToString();

      			if(enableMinimap == true)
      			{
      				_google.setbyGPS(latf, lonf);
      			}

            yield return new WaitForEndOfFrame();

      			if(enableMinimap == true)
      			{
      				_google.gameObject.SetActive(true);
      			}
        }
    }

    public void checkPlusCode()
    {
      StartCoroutine(GetPlusCode());
    }

    IEnumerator GetPlusCode()
    {
      _google.OnClearMapClick();
      _geo.clearAnchors();

      List<string> list = _gc.returnLatLon();

        UnityWebRequest www = UnityWebRequest.Get("https://plus.codes/api?address=" + list[0] + "," + list[1] + "&ekey=" + api + "&email=" + "matt@designium.jp");

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
            pluscodeList.Clear();
            pluscodeList = www.downloadHandler.text.Split('\n').ToList();

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

            _google.setAreaPlusCodes(nelatf,nelonf,swlatf,swlonf,loclatf,loclonf);

            _gc.setAnchorPlusCode(nelatf,nelonf,0);
            _gc.setAnchorPlusCode(swlatf,nelonf,1);
            _gc.setAnchorPlusCode(swlatf,swlonf,2);
            _gc.setAnchorPlusCode(nelatf,swlonf,3);
			      _gc.setAnchorPlusCode(loclatf,loclonf,4);

        }
    }

    public string cleanPlusCode(string s)
    {
      var stripped = Regex.Replace(s, "[^0-9.]", "");
      return stripped;
    }
	string cleanResults(string s)
    {
      var stripped = s.Replace("\" : \"","");
  	  stripped = stripped.Replace("\",","");
  	  stripped = stripped.Replace(Environment.NewLine,"");
  	  stripped = stripped.Replace("  ","");
  	  stripped = stripped.Replace("\"","");
  	  stripped = stripped.Replace("\n","");
      return stripped;
    }
	string cleanPoint(string s)
    {
      var stripped = Regex.Replace(s, "[^0-9.,]", "");
      return stripped;
    }

    public string cleanString(string s)
    {
      var _tempList = Regex.Split(s, "blue");
      var stripped = Regex.Replace(_tempList[1], "[^0-9.]", "");
      return stripped;
    }

	public void testGrid()
	{
		StartCoroutine(GetGridLoop());
	}

	IEnumerator GetGridLoop()
	{
		UnityWebRequest www = UnityWebRequest.Get("https://plus.codes/api?address=" + lat + "," + lon + "&ekey=" + api + "&email=" + "matt@designium.jp");

        yield return www.SendWebRequest();

        if (www.isNetworkError)
        {
            Debug.Log(www.error);
        }
        else
        {
			pluscodeList.Clear();
            pluscodeList = www.downloadHandler.text.Split('\n').ToList();
			string loclat = cleanPlusCode(pluscodeList[15]);
            string loclon = cleanPlusCode(pluscodeList[16]);
			double loclatd = 0;
			double loclond = 0;
			double.TryParse(loclat, out loclatd);
            double.TryParse(loclon, out loclond);

			List<double>newlatlng = new List<double>();
			newlatlng.Add(loclatd);
			newlatlng.Add(loclond);
			StartCoroutine(GetGrid(newlatlng[0],newlatlng[1], "start"));
			List<double>checklatlng = new List<double>();
			//lat = 111/20
			checklatlng = grid.ToNorthPosition(newlatlng, gridDistance);
			StartCoroutine(GetGrid(checklatlng[0],checklatlng[1], "north"));
			checklatlng = grid.ToEastPosition(newlatlng, gridDistance);
			StartCoroutine(GetGrid(checklatlng[0],checklatlng[1], "east"));
			checklatlng = grid.ToSouthPosition(newlatlng, gridDistance);
			StartCoroutine(GetGrid(checklatlng[0],checklatlng[1], "south"));
			checklatlng = grid.ToWestPosition(newlatlng, gridDistance);
			StartCoroutine(GetGrid(checklatlng[0],checklatlng[1], "west"));
		}
	}

	IEnumerator GetGrid(double checklat, double checklon, string s)
	{
		UnityWebRequest www = UnityWebRequest.Get("https://plus.codes/api?address=" + checklat.ToString() + "," + checklon.ToString() + "&ekey=" + api + "&email=" + "matt@designium.jp");

        yield return www.SendWebRequest();

        if (www.isNetworkError) {
            Debug.Log(www.error);
        }
    		else
    		{
    	      List<string>stringlist = new List<string>();
            stringlist = www.downloadHandler.text.Split('\n').ToList();
        }
    }

      public void getPlusCodeVisited(string codelat, string codelon)
      {
        StartCoroutine(GetPlusCodeVisited(codelat,codelon));
      }

       IEnumerator GetPlusCodeVisited(string codelat, string codelon)
       {

           UnityWebRequest www = UnityWebRequest.Get("https://plus.codes/api?address=" + codelat + "," + codelon + "&ekey=" + api + "&email=" + "matt@designium.jp");

           yield return www.SendWebRequest();

           if (www.isNetworkError)
           {
               Debug.Log(www.error);
           }
           else
           {

             pluscodeList.Clear();
             pluscodeList = www.downloadHandler.text.Split('\n').ToList();

             string code = pluscodeList[2];
             code = code.Replace("\"", "");
             code = code.Replace("global_code:", "");
             code = code.Replace(",", "");
             code = code.Replace(" ", "");
             Debug.Log(code);
             //    "global_code": "8Q7XJPG9+6V",

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

             double dlat = 0;
             double dlon = 0;
             double.TryParse(loclat, out dlat);
             double.TryParse(loclon, out dlon);

             string newll = dlat.ToString("F7") + "," + dlon.ToString("F7");

             if(!kml.visitedpluscodeList.Contains(newll))
             {
               Debug.Log("new visit " + loclat + "," + loclon);

               kml.visitedpluscodeList.Add(newll);
               kml.searchArea();
			         yield return new WaitForEndOfFrame();
			         square.addBonus(2);
               Debug.Log("Added " + newll);
			         //some graphics to represent bonus
               rewardText.text = code;
               int rando = UnityEngine.Random.Range(0,10);
               if(rando == 0)
               {
                 text2speech.remoteRequest("You got another reward");
               }
               if(rando == 1)
               {
                 text2speech.remoteRequest("This area is now yours!");
               }
               if(rando == 2)
               {
                 text2speech.remoteRequest("Another code in the bag");
               }
               if(rando == 3)
               {
                 text2speech.remoteRequest("Reward collected for this zone");
               }
               if(rando == 4)
               {
                 text2speech.remoteRequest("Great! Another code collected.");
               }
             }
             else
             {
               Debug.Log("already been here");
             }

           }
         }


}
