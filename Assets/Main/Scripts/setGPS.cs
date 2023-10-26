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

public class setGPS : MonoBehaviour
{

    public GoogleMapsDemo _google;
    public Geo _geo;
    public GeospatialController _gc;
    public getDistance distance;

    public string debugString;

    public string lat;
    public string lon;
    public string radius;
    public string api;
    public string heading;

	float latf;
    float lonf;

    public bool _setOnStart;
	public bool useGoogleAlt;
    public bool useGeographiclib;
    public bool enableMinimap;
    public bool useConvert;

    public enum Types {accounting,
          airport,
          amusement_park,
          aquarium,
          art_gallery,
          atm,
          bakery,
          bank,
          bar,
          beauty_salon,
          bicycle_store,
          book_store,
          bowling_alley,
          bus_station,
          cafe,
          campground,
          car_dealer,
          car_rental,
          car_repair,
          car_wash,
          casino,
          cemetery,
          church,
          city_hall,
          clothing_store,
          convenience_store,
          courthouse,
          dentist,
          department_store,
          doctor,
          drugstore,
          electrician,
          electronics_store,
          embassy,
          fire_station,
          florist,
          funeral_home,
          furniture_store,
          gas_station,
          gym,
          hair_care,
          hardware_store,
          hindu_temple,
          home_goods_store,
          hospital,
          insurance_agency,
          jewelry_store,
          laundry,
          lawyer,
          library,
          light_rail_station,
          liquor_store,
          local_government_office,
          locksmith,
          lodging,
          meal_delivery,
          meal_takeaway,
          mosque,
          movie_rental,
          movie_theater,
          moving_company,
          museum,
          night_club,
          painter,
          park,
          parking,
          pet_store,
          pharmacy,
          physiotherapist,
          plumber,
          police,
          post_office,
          primary_school,
          real_estate_agency,
          restaurant,
          roofing_contractor,
          rv_park,
          school,
          secondary_school,
          shoe_store,
          shopping_mall,
          spa,
          stadium,
          storage,
          store,
          subway_station,
          supermarket,
          synagogue,
          taxi_stand,
          tourist_attraction,
          train_station,
          transit_station,
          travel_agency,
          university,
          veterinary_care,
          zoo}

    public Types _types;

    public List<string> _bigList = new List<string>();
    public List<string> _nameList = new List<string>();
    public List<string> _locationList = new List<string>();
    public List<string> _ratingList = new List<string>();
    public List<string> pluscodeList = new List<string>();
	public List<string> stepsList = new List<string>();
    public List<string> latList = new List<string>();
    public List<string> lonList = new List<string>();
    public List<string> pointsList = new List<string>();
    public List<string> elevationList = new List<string>();
    public List<string> geocodeList = new List<string>();
    public List<string> addressList = new List<string>();

    public List<LatLng> latlngList = new List<LatLng>();

    public Dropdown _drop;

  	public double gridDistance;

  	public Texture2D loadedTexture;

    public string transitMode;

	public TMP_Text debugTxt;
	public TMP_Text plusCodeTxt;
	public TMP_Text detailTxt;

	public float myElevation;
	public float myAltitude;
    public float modifier;

    public Material debugMaterial;

    public Image SVimage;
    public GameObject plusdebug;

    void Start()
    {
      #if UNITY_EDITOR
        //lat = "35.625626";
        //lon = "139.719521";
        //home 35.695242,139.624534
		//work 35.625626,139.719521
        latf = 35.625626f;
        lonf = 139.719521f;
      #endif

       StartCoroutine(GetLocation());

       if(_drop != null)
       {
         _drop.options.Clear ();
         string[] TypeNames = System.Enum.GetNames (typeof(Types));
         for(int i = 0; i < TypeNames.Length; i++){
             //Debug.Log (TypeNames[i]);
             string _str = TypeNames[i].ToString();
             _str = _str.Replace("_", " ");
             _drop.options.Add (new Dropdown.OptionData() {text=_str});
         }
         if(_setOnStart == true)
         {
           _drop.value = 25;
         }
       }
    }

    // Update is called once per frame
    void Update()
    {
      if(Input.GetKeyUp(KeyCode.L))
  		{
			     getPlusCodeTap("35.695242","139.624534");
  		}
      if(Input.GetKeyUp(KeyCode.K))
      {
		      StartCoroutine(LoadTextureFromWeb("https://maps.googleapis.com/maps/api/place/photo?maxwidth=400&photo_reference=Aap_uEA7vb0DDYVJWEaX3O-AtYp77AaswQKSGtDaimt3gt7QCNpdjp1BkdM6acJ96xTec3tsV_ZJNL_JP-lqsVxydG3nh739RE_hepOOL05tfJh2_ranjMadb3VoBYFvF0ma6S24qZ6QJUuV6sSRrhCskSBP5C1myCzsebztMfGvm7ij3gZT&key=AIzaSyCLoCRs3w-AGrm7_AwCjrCq9W9gNQLOC6c"));
      }
      if(Input.GetKeyUp(KeyCode.J))
      {
        StartCoroutine(streetTest());
      }
    }

    IEnumerator streetTest()
    {
      string baseurl = "https://maps.googleapis.com/maps/api/streetview?size=400x400&location=";
      string svurl = baseurl + "35.62561524780179,139.71949746616224" + "&fov=80&heading=" + heading + "&pitch=0&key=" + api; //&signature=YOUR_SIGNATURE

      UnityWebRequest www = UnityWebRequest.Get(svurl);
      yield return www.SendWebRequest();

      if (www.result != UnityWebRequest.Result.Success)
      {
          Debug.Log(www.error);
      }
      else
      {
          // Show results as text
          string str = www.downloadHandler.text;
          Debug.Log(str);
      }

      using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(svurl))
      {
        yield return uwr.SendWebRequest();

        if (uwr.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(uwr.error);
        }
        else
        {
            // Get downloaded asset bundle
            var texture = DownloadHandlerTexture.GetContent(uwr);
            debugMaterial.mainTexture = texture;
        }
      }
    }

    public void checkAPI()
    {
      StartCoroutine(GetResults());
    }

    IEnumerator GetText() {

		UnityWebRequest www = UnityWebRequest.Get("https://maps.googleapis.com/maps/api/elevation/json?locations=35.695242,139.624534&key=AIzaSyCLoCRs3w-AGrm7_AwCjrCq9W9gNQLOC6c");

    yield return www.SendWebRequest();

        if (www.isNetworkError) {
            Debug.Log(www.error);
        } else {
            // Show results as text
            Debug.Log(www.downloadHandler.text);
        }
    }

	IEnumerator LoadTextureFromWeb(string url)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError("Error: " + www.error);
        }
        else
        {
            loadedTexture = DownloadHandlerTexture.GetContent(www);
            //Material m = new Material(shader);
			//m.SetTexture("_MainTex", loadedTexture);
			//rend.material = m;
        }
    }

    IEnumerator GetResults() {

        string _url = "https://maps.googleapis.com/maps/api/place/search/json" + "?" + "location=" + lat + "," + lon + "&radius=" + radius + "&sensor=false&key=" + api + "&types=" + _types.ToString();
        UnityWebRequest www = UnityWebRequest.Get(_url);

        yield return www.SendWebRequest();

        if (www.isNetworkError) {
            Debug.Log(www.error);
        } else {
            // Show results as text
            Debug.Log(www.downloadHandler.text);

            // Or retrieve results as binary data
            //byte[] results = www.downloadHandler.data;
            string results = www.downloadHandler.text;

            var _edit = results.Replace("business_status", "^");

            _bigList.Clear();
            _nameList.Clear();
            _locationList.Clear();
            _ratingList.Clear();

            _bigList = _edit.Split('^').ToList();

            int _count = 0;

            foreach(var l in _bigList)
            {
                if(l.Contains("geometry"))
                {
                  //get GPS coords
                  //Debug.Log(l);
                  int pFrom = l.IndexOf("location") + "location".Length;
                  int pTo = l.LastIndexOf("viewport");
                  string result = l.Substring(pFrom, pTo - pFrom);
                  string str = new string(result.Where(c =>char.IsLetterOrDigit(c) || c == '.').ToArray());
                  pFrom = str.IndexOf("lat") + "lat".Length;
                  pTo = str.LastIndexOf("lng");
                  string latstr = str.Substring(pFrom, pTo - pFrom);
                  pFrom = str.IndexOf("lng") + "lng".Length;
                  pTo = str.Length;
                  string lonstr = str.Substring(pFrom, pTo - pFrom);
                  string _locstr = "(" + latstr + "," + lonstr + ")";
                  _locationList.Add(_locstr);
                  yield return new WaitForEndOfFrame();
                  //get name
                  pFrom = l.IndexOf("name") + "name".Length;
                  pTo = l.LastIndexOf("place_id");
                  result = l.Substring(pFrom, pTo - pFrom);

					          var name = "name";
                    if(l.Contains("name"))
                    {
                      pFrom = l.IndexOf("name") + "name".Length;
                      pTo = l.LastIndexOf("opening_hours");
                      result = l.Substring(pFrom, pTo - pFrom);
          					  name = result;
          					  name = cleanResults(name);
                      _nameList.Add(name);
					            //Debug.Log(name);
                    }
                    else
                    {
                      _nameList.Add("no name");
                      name = "no name";
                    }

                    //get rating
                    var rating = "rating";
                    if(l.Contains("rating"))
                    {
                      pFrom = l.IndexOf("rating") + "rating".Length;
                      pTo = l.LastIndexOf("reference");
                      result = l.Substring(pFrom, pTo - pFrom);
					            rating = result;
          				    rating = cleanPlusCode(rating);
                      _ratingList.Add(rating);
                      //Debug.Log(rating);
                    }
                    else
                    {
                      _ratingList.Add("no rating");
                      rating = "no rating";
                      //Debug.Log(name + " has no rating");
                    }

          					string iconURL = "icon";
          					if(l.Contains("icon"))
          					{
          					  pFrom = l.IndexOf("\"icon\" : ") + ("\"icon\" : ").Length;
          					  pTo = l.LastIndexOf("icon_background_color");
          					  iconURL = l.Substring(pFrom, pTo - pFrom);
          					  iconURL = cleanResults(iconURL);
          					  //Debug.Log(iconURL);
          					}
          					else
          					{
          					  //_ratingList.Add("no icon");
          					  iconURL = "noicon";
          					  //Debug.Log(name + " has no icon");
          					}

          					string photo = "photo";
          					if(l.Contains("photo"))
          					{
          					  pFrom = l.IndexOf("photo_reference") + ("photo_reference").Length;
          					  pTo = l.LastIndexOf("width");
          					  photo = l.Substring(pFrom, pTo - pFrom);
          					  photo = cleanResults(photo);
          					  //Debug.Log(photo);
          					}
          					else
          					{
          					  //_ratingList.Add("no icon");
          					  iconURL = "noicon";
          					  //Debug.Log(name + " has no icon");
          					}

                    createObjects(name, latstr, lonstr, rating, iconURL, photo);

					          _count += 1;
                }
            }
        }
    }


    void createObjects(string _name, string _lat, string _lon, string _rating, string icon, string photo)
    {
      //Debug.Log(_name + ":" + _lat + "," + _lon + ":" + _rating + ":" + icon);
      float _latflt = 0;
      float _lonflt = 0;
      float.TryParse(_lat, out _latflt);
      float.TryParse(_lon, out _lonflt);
      if(_latflt != 0)
      {
        #if !UNITY_EDITOR
        _google.setAPI(_latflt, _lonflt);
        _geo.setAnchorAPI(_latflt, _lonflt, _name, _rating, icon, photo);
        #endif
		#if UNITY_EDITOR
		    _geo.setAnchorDebug(_latflt, _lonflt, _name, _rating, icon, photo);
		#endif
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
            if(_geo != null)
            {
              _geo.nowStart();
            }

            yield return new WaitForEndOfFrame();

      			if(enableMinimap == true)
      			{
      				_google.gameObject.SetActive(true);
      			}

            yield return new WaitForEndOfFrame();

            if(_setOnStart == true)
            {
              StartCoroutine(GetResults());
            }
        }
    }

    public void changeType(int _i)
    {
      _google.OnClearMapClick();
      _geo.clearAnchors();
      _types = (Types)_i;
      StartCoroutine(GetResults());
      //Debug.Log("change");
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
            // Show results as text
            //Debug.Log(www.downloadHandler.text);

            //_geo.setAnchor
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


//northeast
// lat = 6
// lon = 7
//southwest
// lat = 10
// lon = 11
//location
// lat = 15
// lon = 16

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

    public void getDirections()
    {
      _google.OnClearMapClick();
      StartCoroutine(GetDirections(35.625629f, 139.719515f, 35.632221f, 139.712134f));
      //StartCoroutine(GetDirections(35.625629f, 139.719515f, 35.648209f, 139.710121f));
    }

	public void getDirectionsMapClick(string point)
    {
		string p = cleanPoint(point);
		List<string> userLocation = _gc.returnLatLon();
		_locationList.Clear();
		_locationList = p.Split(',').ToList();
		_google.OnClearMapClick();
		float fromLat = 0f; float fromLon = 0f; float toLat = 0f; float toLon = 0f;
		float.TryParse(userLocation[0], out fromLat);
		float.TryParse(userLocation[1], out fromLon);
		float.TryParse(_locationList[0], out toLat);
		float.TryParse(_locationList[1], out toLon);
		StartCoroutine(GetDirections(fromLat, fromLon, toLat, toLon));
    }



    IEnumerator GetDirections(float fromLat, float fromLon, float toLat, float toLon)
	{

    UnityWebRequest www = UnityWebRequest.Get("https://maps.googleapis.com/maps/api/directions/json?origin=" + fromLat.ToString() + "," + fromLon + "&destination=" + toLat + "," + toLon + "&mode=" + transitMode + "&key=" + api);

//mode driving walking bicycling transit
//transit_mode bus subway train tram rail
//https://maps.googleapis.com/maps/api/directions/json?
//origin=Adelaide,SA&destination=Adelaide,SA
//&waypoints=optimize:true|Barossa+Valley,SA|Clare,SA|Connawarra,SA|McLaren+Vale,SA

        yield return www.SendWebRequest();

        if (www.isNetworkError)
		{
            Debug.Log(www.error);
        }
		else
		{
            stepsList.Clear();
            latList.Clear();
            lonList.Clear();
            pointsList.Clear();

            stepsList = www.downloadHandler.text.Split('\n').ToList();
            foreach(string s in stepsList)
            {
              if(s.Contains("\"lat"))
              {
                latList.Add(cleanPlusCode(s));
              }
              else if(s.Contains("\"lng"))
              {
                lonList.Add(cleanPlusCode(s));
              }
              else if(s.Contains("\"points"))
              {
                string p = s.Replace("\"", "");
                p = p.Replace("points : ", "");
                p = p.Replace(" ", "");
                pointsList.Add(p);
              }
            }

            latlngList.Clear();
            latlngList = DecodePolylinePoints(pointsList[pointsList.Count() - 1]);
			LatLng previousLL = latlngList[0];

			//myAltitude = _gc.returnAlt();

			if(useGoogleAlt == true)
			{
				float googleelevation = 0;
				/*
				string templat = previousLL.Latitude.ToString();
				string templon = previousLL.Longitude.ToString();
				*/
				List<string> latlonlist = new List<string>();
				latlonlist = _gc.returnLatLon();

				UnityWebRequest wwwel = UnityWebRequest.Get("https://maps.googleapis.com/maps/api/elevation/json?locations=" + latlonlist[0] + "," + latlonlist[1] + "&key=AIzaSyCLoCRs3w-AGrm7_AwCjrCq9W9gNQLOC6c");

				yield return wwwel.SendWebRequest();

				if (wwwel.isNetworkError)
				{
				   Debug.Log(wwwel.error);
				}
				else
				{
					List<string> eList = new List<string>();
					eList = wwwel.downloadHandler.text.Split('\n').ToList();
					string elevation = cleanPlusCode(eList[3]);
					float.TryParse(elevation, out googleelevation);
					myElevation = googleelevation;
				}
				yield return new WaitForEndOfFrame();
				Resources.UnloadUnusedAssets();
			}

			Debug.Log("my altitude: " + myAltitude.ToString() + "// my elevation: " + myElevation.ToString());
#if UNITY_EDITOR

#endif
#if !UNITY_EDITOR
            _google.setRoute(latlngList);
            _geo.setAnchorNavi((float)previousLL.Latitude,(float)previousLL.Longitude, myAltitude);

			         yield return new WaitForEndOfFrame();

            int counter = 0;
            foreach(LatLng l in latlngList)
            {
              if(counter > 0)
              {
                float d = distance.CalculateDistance((float)previousLL.Latitude, (float)l.Latitude, (float)previousLL.Longitude, (float)l.Longitude);
                if(d > 5)
                {
          					float newElevation = 0;
          					float newAltitude = 0;

          					if(useGoogleAlt == true)
          					{
          						string templat = previousLL.Latitude.ToString();
          						string templon = previousLL.Longitude.ToString();

          						string theurl = "https://maps.googleapis.com/maps/api/elevation/json?locations=" + templat + "," + templon + "&key=AIzaSyCLoCRs3w-AGrm7_AwCjrCq9W9gNQLOC6c";

          						//Debug.Log(templat + ":" + templon);

          						UnityWebRequest wwww = UnityWebRequest.Get(theurl);

          						yield return wwww.SendWebRequest();

          						if (wwww.isNetworkError)
          						{
          						   Debug.Log(wwww.error);
          						}
          						else
          						{
          						   List<string> eList = new List<string>();
          						   eList = wwww.downloadHandler.text.Split('\n').ToList();
          						   string elevation = cleanPlusCode(eList[3]);
          						   float.TryParse(elevation, out newElevation);
                         float difference = 0;
/*
                         if(myElevation > newElevation)
                         {
                           difference = myElevation - newElevation;
                           newAltitude = myAltitude - difference;
                           Debug.Log("pin is lower" + "\n" +
                           templat.ToString() + ":" + templon.ToString() + "\n" +
                           "difference: " + difference.ToString() + " = " + myElevation.ToString() + " - " + newElevation.ToString() + "\n" +
                           "modified altitude: " + newAltitude.ToString() + " = " + myAltitude.ToString() + " - " + difference.ToString());
                         }
                         else if(myElevation < newElevation)
                         {
                           difference = newElevation - myElevation;
                           newAltitude = myAltitude + difference;
                           Debug.Log("pin is higher" + "\n" +
                           templat.ToString() + ":" + templon.ToString() + "\n" +
                           "difference: " + difference.ToString() + " = " + newElevation.ToString() + " - " + newElevation.ToString() + "\n" +
                           "modified altitude: " + newAltitude.ToString() + " = " + myAltitude.ToString() + " + " + difference.ToString());
                         }
*/
          						   yield return new WaitForEndOfFrame();
          						}
          					}


                    if(useConvert == true)
          					{
          						string templat = previousLL.Latitude.ToString();
          						string templon = previousLL.Longitude.ToString();

          						string theurl = "https://maps.googleapis.com/maps/api/elevation/json?locations=" + templat + "," + templon + "&key=AIzaSyCLoCRs3w-AGrm7_AwCjrCq9W9gNQLOC6c";

          						//Debug.Log(templat + ":" + templon);

          						UnityWebRequest wwww = UnityWebRequest.Get(theurl);

          						yield return wwww.SendWebRequest();

          						if (wwww.isNetworkError)
          						{
          						   Debug.Log(wwww.error);
          						}
          						else
          						{
          						   List<string> eList = new List<string>();
          						   eList = wwww.downloadHandler.text.Split('\n').ToList();
          						   string elevation = cleanPlusCode(eList[3]);
          						   float.TryParse(elevation, out newElevation);
                         newAltitude = ElevationToAltitude(newElevation);
          						   yield return new WaitForEndOfFrame();
          						}
          					}

                    //newAltitude = newAltitude + modifier;
                    //Debug.Log(newAltitude);

          					previousLL = latlngList[counter];
          					_geo.setAnchorNavi((float)previousLL.Latitude,(float)previousLL.Longitude, newAltitude);
                }
                else
                {
                  //Debug.Log("don't place anchor");
                }
              }
              counter += 1;
              yield return new WaitForEndOfFrame();
            }
#endif

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
                //Location p = new Location();
                //p.Latitude = Convert.ToDouble(currentLat) / 100000.0;
                //p.Longitude = Convert.ToDouble(currentLng) / 100000.0;

                //Location p = new Location();
                double latp = Convert.ToDouble(currentLat) / 100000.0;
                double lonp = Convert.ToDouble(currentLng) / 100000.0;
                LatLng p = new LatLng(latp,lonp);

                poly.Add(p);

            }
        }
        catch (Exception ex)
        {
            // logo it
        }
        return poly;
   }

   public void googleElevationAPI()
   {
	   StartCoroutine(GetElevation());
   }

   IEnumerator GetElevation()
   {

		var debuglat = lat;
		var debuglon = lon;

#if !UNITY_EDITOR
		var ll = _gc.returnLatLon();
		debuglat = ll[0];
		debuglon = ll[1];
#endif

	   yield return new WaitForEndOfFrame();

	   UnityWebRequest www = UnityWebRequest.Get("https://maps.googleapis.com/maps/api/elevation/json?locations=" + debuglat + "," + debuglon +"&key=AIzaSyCLoCRs3w-AGrm7_AwCjrCq9W9gNQLOC6c");

       yield return www.SendWebRequest();

       if (www.isNetworkError)
	     {
           Debug.Log(www.error);
       }
	   else
	   {
             // Show results as text
             // Debug.Log(www.downloadHandler.text);
  		   elevationList.Clear();
  		   elevationList = www.downloadHandler.text.Split('\n').ToList();
  		   string elevation = cleanPlusCode(elevationList[3]);
  		   float el = 0;
  		   float.TryParse(elevation, out el);
         Debug.Log(el);
  		   Debug.Log(ElevationToAltitude(el));
         Debug.Log(AltitudeToElevation(el));
         float _latflt = 0;
         float _lonflt = 0;
         float.TryParse(debuglat, out _latflt);
         float.TryParse(debuglon, out _lonflt);
         Debug.Log(Egm96ToWgs84((double)_latflt, (double)_lonflt, (double)el));

         /*
  		   debugTxt.text = "Google Elevation API: " + el.ToString() + "\n" +
  			"@ " + debuglat + "," + debuglon  + "\n\n" +
  			"LocationInfo Altitude: " + Input.location.lastData.altitude + "\n" +
  			"@ " + Input.location.lastData.latitude.ToString() + "," + Input.location.lastData.longitude.ToString();
        */
       }
   }

   public static double Egm96ToWgs84(double egm96Elevation, double latitude, double longitude)
   {
       // Calculate geocentric latitude from geographic latitude
       double phi = latitude * Math.PI / 180;
       double sinPhi = Math.Sin(phi);
       double cosPhi = Math.Cos(phi);
       double a = 6378137; // semi-major axis of WGS84 ellipsoid
       double f = 1 / 298.257223563; // flattening of WGS84 ellipsoid
       double b = a * (1 - f); // semi-minor axis of WGS84 ellipsoid
       double e2 = 1 - (b * b) / (a * a); // eccentricity squared of WGS84 ellipsoid
       double n = a / Math.Sqrt(1 - e2 * sinPhi * sinPhi);
       double x = (n + egm96Elevation) * cosPhi * Math.Cos(longitude * Math.PI / 180);
       double y = (n + egm96Elevation) * cosPhi * Math.Sin(longitude * Math.PI / 180);
       double z = ((1 - e2) * n + egm96Elevation) * sinPhi;

       // Convert geocentric (x, y, z) coordinates to WGS84 (latitude, longitude, elevation)
       double p = Math.Sqrt(x * x + y * y);
       double theta = Math.Atan2(z * a, p * b);
       double sinTheta = Math.Sin(theta);
       double cosTheta = Math.Cos(theta);
       double wgs84Latitude = Math.Atan2(z + (e2 * b * sinTheta * sinTheta * sinTheta), p - (e2 * a * cosTheta * cosTheta * cosTheta));
       double wgs84Longitude = Math.Atan2(y, x);
       double wgs84N = a / Math.Sqrt(1 - e2 * Math.Sin(wgs84Latitude) * Math.Sin(wgs84Latitude));
       double wgs84Elevation = p / Math.Cos(wgs84Latitude) - wgs84N;

       return wgs84Elevation;
   }

   public static float ElevationToAltitude(float elevation)
   {
       const float earthRadius = 6378137f; // in meters
       float altitude = (earthRadius * elevation) / (earthRadius - elevation);
       return altitude;
   }

   public static float AltitudeToElevation(float altitude)
   {
       const float earthRadius = 6378137f; // in meters
       float elevation = (altitude * earthRadius) / (earthRadius + altitude);
       return elevation;
   }


   public void googleGeocode()
   {
    StartCoroutine(GetGeocode());
   }

   IEnumerator GetGeocode()
   {

   var debuglat = lat;
   var debuglon = lon;

#if !UNITY_EDITOR
   var ll = _gc.returnLatLon();
   debuglat = ll[0];
   debuglon = ll[1];
#endif

    yield return new WaitForEndOfFrame();

    UnityWebRequest www = UnityWebRequest.Get("https://maps.googleapis.com/maps/api/geocode/json?address=" + debuglat + "," + debuglon + "&language=ja-JP" + "&key=AIzaSyCLoCRs3w-AGrm7_AwCjrCq9W9gNQLOC6c");

       yield return www.SendWebRequest();

       if (www.isNetworkError)
      {
           Debug.Log(www.error);
       }
    else
    {
             // Show results as text
        Debug.Log(www.downloadHandler.text);
        geocodeList.Clear();
        addressList.Clear();
        geocodeList = www.downloadHandler.text.Split('\n').ToList();

        foreach(string s in geocodeList)
        {
          if(s.Contains("formatted_address"))
          {
            addressList.Add(s);
          }
        }

        geocodeList.Clear();
        geocodeList = addressList[0].Split('\"').ToList();
        Debug.Log(geocodeList[3]);

                /*
        string elevation = cleanPlusCode(elevationList[3]);
        float el = 0;
        float.TryParse(elevation, out el);
         Debug.Log(el);
        Debug.Log(ElevationToAltitude(el));
         Debug.Log(AltitudeToElevation(el));
         float _latflt = 0;
         float _lonflt = 0;
         float.TryParse(debuglat, out _latflt);
         float.TryParse(debuglon, out _lonflt);
         Debug.Log(Egm96ToWgs84((double)_latflt, (double)_lonflt, (double)el));
*/
         /*
        debugTxt.text = "Google Elevation API: " + el.ToString() + "\n" +
       "@ " + debuglat + "," + debuglon  + "\n\n" +
       "LocationInfo Altitude: " + Input.location.lastData.altitude + "\n" +
       "@ " + Input.location.lastData.latitude.ToString() + "," + Input.location.lastData.longitude.ToString();
        */
       }
   }


   public void getPlusCodeTap(string codelat, string codelon)
   {
	   StartCoroutine(GetPlusCodeManual(codelat,codelon));
   }

      IEnumerator GetPlusCodeManual(string codelat, string codelon)
    {

        UnityWebRequest www = UnityWebRequest.Get("https://plus.codes/api?address=" + codelat + "," + codelon + "&ekey=" + api + "&email=" + "matt@designium.jp");

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

          _google.setAreaPlusCodes(nelatf,nelonf,swlatf,swlonf,loclatf,loclonf);

          //_gc.setAnchorPlusCode(nelatf,nelonf,0);
          //_gc.setAnchorPlusCode(swlatf,nelonf,1);
          //_gc.setAnchorPlusCode(swlatf,swlonf,2);
          //_gc.setAnchorPlusCode(nelatf,swlonf,3);
          //_gc.setAnchorPlusCode(loclatf,loclonf,4);

          heading = UnityEngine.Random.Range(0,360).ToString();
          string baseurl = "https://maps.googleapis.com/maps/api/streetview?size=400x400&location=";
          string svurl = baseurl + loclat + "," + loclonf + "&fov=80&heading=" + heading + "&pitch=0&key=" + api; //&signature=YOUR_SIGNATURE

          using (UnityWebRequest uwr = UnityWebRequestTexture.GetTexture(svurl))
          {
            yield return uwr.SendWebRequest();

            if (uwr.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(uwr.error);
            }
            else
            {
                // Get downloaded asset bundle
                var texture = DownloadHandlerTexture.GetContent(uwr);
                debugMaterial.mainTexture = texture;
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
                // Set sprite to Image component
                SVimage.sprite = sprite;
            }
          }

          //Color c = Color.white;
          //plusCodeTxt.color = c;
          plusCodeTxt.text = code;
          detailTxt.text = loclat.ToString() + ", " + loclonf.ToString();
          /*
          while(c.a > 0)
          {
            c.a -= 0.02f;
            plusCodeTxt.color = c;
            yield return new WaitForEndOfFrame();
          }
          */

          plusdebug.SetActive(true);
    		}
      }

    /*

		  if(useGeographiclib == true)
	  {

		string templat = previousLL.Latitude.ToString();
		string templon = previousLL.Longitude.ToString();

		UnityWebRequest wwwGlib = UnityWebRequest.Get("https://geographiclib.sourceforge.io/cgi-bin/GeoidEval?input=" + templat + "," + templon + "&option=Submit");

		yield return wwwGlib.SendWebRequest();

			if (wwwGlib.isNetworkError) {
				Debug.Log(wwwGlib.error);
			} else {
				// Show results as text
				//Debug.Log(wwwGlib.downloadHandler.text);
				string input = wwwGlib.downloadHandler.text;
				List<string> eList = new List<string>();
				eList = input.Split('\n').ToList();
				string EGM2008 = cleanString(eList[45]);
				string EGM96 = cleanString(eList[46]);
				string EGM84 = cleanString(eList[47]);
				float.TryParse(EGM2008, out altitude);
			}
	  }




    {
      "plus_code": {
        "global_code": "8Q7XJPG9+6Q",
        "geometry": {
          "bounds": {
            "northeast": {
              "lat": 35.625625,
              "lng": 139.71949999999998
            },
            "southwest": {
              "lat": 35.6255,
              "lng": 139.71937499999996
            }
          },
          "location": {
            "lat": 35.6255625,
            "lng": 139.71943749999997
          }
        },
        "local_code": "JPG9+6Q",
        "locality": {
          "local_address": "Shinagawa City, Tokyo, Japan"
        }
      },
      "status": "OK"
    }


    {
       "geocoded_waypoints" : [
          {
             "geocoder_status" : "OK",
             "place_id" : "ChIJXw_egOSKGGARR4MxvN6Z8g4",
             "types" : [ "street_address" ]
          },
          {
             "geocoder_status" : "OK",
             "place_id" : "ChIJp_0n2_SLGGARbL411gwLWOU",
             "types" : [ "establishment", "point_of_interest" ]
          }
       ],
       "routes" : [
          {
             "bounds" : {
                "northeast" : {
                   "lat" : 35.6321198,
                   "lng" : 139.7196375
                },
                "southwest" : {
                   "lat" : 35.6256439,
                   "lng" : 139.7122077
                }
             },
             "copyrights" : "Map data ©2023",
             "legs" : [
                {
                   "distance" : {
                      "text" : "1.0 km",
                      "value" : 1050
                   },
                   "duration" : {
                      "text" : "13 mins",
                      "value" : 786
                   },
                   "end_address" : "Taiko Bridge, 2-chōme-3 Shimomeguro, Meguro City, Tokyo 153-0064, Japan",
                   "end_location" : {
                      "lat" : 35.6321198,
                      "lng" : 139.7122077
                   },
                   "start_address" : "2-chōme-24-6 Nishigotanda, Shinagawa City, Tokyo 141-0031, Japan",
                   "start_location" : {
                      "lat" : 35.6256439,
                      "lng" : 139.7194602
                   },
                   "steps" : [
                      {
                         "distance" : {
                            "text" : "42 m",
                            "value" : 42
                         },
                         "duration" : {
                            "text" : "1 min",
                            "value" : 30
                         },
                         "end_location" : {
                            "lat" : 35.6259388,
                            "lng" : 139.7196375
                         },
                         "html_instructions" : "Head \u003cb\u003enortheast\u003c/b\u003e",
                         "polyline" : {
                            "points" : "gcmxEs}wsY{@c@"
                         },
                         "start_location" : {
                            "lat" : 35.6256439,
                            "lng" : 139.7194602
                         },
                         "travel_mode" : "WALKING"
                      },
                      {
                         "distance" : {
                            "text" : "1.0 km",
                            "value" : 1008
                         },
                         "duration" : {
                            "text" : "13 mins",
                            "value" : 756
                         },
                         "end_location" : {
                            "lat" : 35.6321198,
                            "lng" : 139.7122077
                         },
                         "html_instructions" : "Turn \u003cb\u003eleft\u003c/b\u003e",
                         "maneuver" : "turn-left",
                         "polyline" : {
                            "points" : "cemxEw~wsY_@rACN@?WbAMEADEAIV?@CFABMb@CJAFCHELKZCHGPGLg@pAEJIRO\\y@tBGHO`@[x@GJCBENIPGNENGLGNADEFMVEFILEDMNKFONQJ_@VGBIHu@d@e@XEBWPa@V_@TyA|@GFYPGDcAn@GDc@XWPQLGBIF_@VA@IFGD[TGDEBGFSNIFIFGDOJGDEDIDA@"
                         },
                         "start_location" : {
                            "lat" : 35.6259388,
                            "lng" : 139.7196375
                         },
                         "travel_mode" : "WALKING"
                      }
                   ],
                   "traffic_speed_entry" : [],
                   "via_waypoint" : []
                }
             ],
             "overview_polyline" : {
                "points" : "gcmxEs}wsY{@c@_@rACN@?WbAMEADEAIXEJW`A]dA_A~BaB~Dm@xA_@~@]t@c@j@mAz@mEpCkG~DiEzCYR"
             },
             "summary" : "",
             "warnings" : [
                "Walking directions are in beta. Use caution – This route may be missing sidewalks or pedestrian paths."
             ],
             "waypoint_order" : []
          }
       ],
       "status" : "OK"
    }

    UnityEngine.Debug:Log (object)
    setGPS/<GetDirections>d__43:MoveNext () (at Assets/Main/Scripts/setGPS.cs:623)
    UnityEngine.SetupCoroutine:InvokeMoveNext (System.Collections.IEnumerator,intptr)

    */
}
