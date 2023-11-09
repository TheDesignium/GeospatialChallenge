using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

using System;
using System.IO;
using System.Xml;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using NinevaStudios.GoogleMaps;

using Google.XR.ARCoreExtensions.Samples.Geospatial;

using TMPro;

public class LoadKML : MonoBehaviour {

    public GoogleMapsDemo _google;
    public GeospatialController geo;
    public Geo _geo;
    public getDistance distance;

    public string kmlFilePath; // Path to the KML file
    public string kmlFileName;
    public string api;

    public string[] coordinateStrings;
    public List<string> coordinates = new List<string>();
    public List<string> fileNames = new List<string>();
    public List<string> tempLatLon = new List<string>();
    public List<string> prevLatLon = new List<string>();
	  public List<string> extractedStrings = new List<string>();

    public int numButtons;
    public GameObject buttonPrefab;
    public float buttonWidth;
    public Canvas targetCanvas;

    public List<GameObject> buttons = new List<GameObject>();
    public List<string> nameList = new List<string>();
    public List<string> latLonList = new List<string>();
    public List<string> centerList = new List<string>();
    public List<string> pluscodeList = new List<string>();
    public List<string> visitedpluscodeList = new List<string>();
    public List<string> anchorpluscodeList = new List<string>();

    List<string> tempPlusList  = new List<string>();

    public bool maponly;
    public bool brute;

    public string stringA; // The first string that marks the start of the text we want to extract
    public string stringB; // The second string that marks the end of the text we want to extract
    public string stringC; // The first string that marks the start of the text we want to extract
    public string stringD; // The second string that marks the end of the text we want to extract

	public string first;
	public string mid;
	public string last;

    public LatLng southwest;
    public LatLng northeast;

	public string mapLvl;
	public string mapLvl3;
	public string mapLvl4;

	public float fourzoomlimit;
	public float fourzoomlimitTwo;
	public float threezoomlimit;
	public float threezoomlimitTwo;

  int gridlvl;

  public GameObject mapUItypes;
  public GameObject[] mapUIs;
  public TMP_Text idText;
  public TMP_Text idTextGreen;
  public TMP_Text idTextBlue;

  public Image[] imgGreen;
  public Image[] imgBlue;
  Image[] imgTemp;

    void Start()
    {

    }

    void Update ()
    {
      if(_google.movingMap == true)
      {
            if(_google.idleMap == true)
            {
              _google.movingMap = false;
              searchArea();
            }
      }
    }

    public void searchArea()
    {
      StartCoroutine(Search());
    }

    IEnumerator Search()
    {
#if !UNITY_EDITOR
        VisibleRegion vr = _google._map.Projection.GetVisibleRegion();
        southwest = vr.LatLngBounds._southwest;
        northeast = vr.LatLngBounds._northeast;
#endif
#if UNITY_EDITOR
		    LatLng southwest = new LatLng(35.62258854475747, 139.7198324407011);
        LatLng northeast = new LatLng(35.62627625246758, 139.72957429959416);
#endif

		float dist = distance.CalculateDistance((float)southwest.Latitude,(float)northeast.Latitude,(float)southwest.Longitude,(float)northeast.Longitude);

		string url = "";

		if(dist > fourzoomlimit && dist < fourzoomlimitTwo)
		{
			url = "https://grid.plus.codes/kml?level=" + mapLvl4 + "&BBOX=" + southwest.Longitude.ToString() + "," + southwest.Latitude.ToString() + "," + northeast.Longitude.ToString() + "," + northeast.Latitude.ToString();
      gridlvl = 4;
		}
		else if(dist > threezoomlimit && dist < threezoomlimitTwo)
		{
			url = "https://grid.plus.codes/kml?level=" + mapLvl3 + "&BBOX=" + southwest.Longitude.ToString() + "," + southwest.Latitude.ToString() + "," + northeast.Longitude.ToString() + "," + northeast.Latitude.ToString();
      gridlvl = 3;
  	}
		else
		{
			url = "https://grid.plus.codes/kml?BBOX=" + southwest.Longitude.ToString() + "," + southwest.Latitude.ToString() + "," + northeast.Longitude.ToString() + "," + northeast.Latitude.ToString();
      gridlvl = 2;
		}

        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(www.error);
        }
        else
        {
            // Show results as text
            string str = www.downloadHandler.text;
      			if(str.Contains("<MultiGeometry>"))
      			{
      				int index = str.IndexOf("<MultiGeometry>");
      				string result = str.Substring(0, index);
      				ExtractStrings(result, stringA, stringB);
      			}
      			else
      			{
      				ExtractStrings(str, stringA, stringB);
      			}
        }
    }

    public void readKML()
    {
        latLonList.Clear();

#if !UNITY_EDITOR
        _google.OnClearMapClick();
        //_geo.clearAnchors();
        kmlFilePath = Application.persistentDataPath + "/" + kmlFileName;
#endif
#if UNITY_EDITOR
      kmlFilePath = Application.streamingAssetsPath + "/" + kmlFileName;
#endif

        // Load the KML file as a string
        string kmlString = File.ReadAllText(kmlFilePath);
        // Parse the KML string into a list of lat lon coordinates
        if(brute == false)
        {
          latLonList = ParseKMLString(kmlString);
#if !UNITY_EDITOR
          _google.setRouteStrings(latLonList);
#endif
        }
        else
        {
          ExtractStrings(kmlString, stringA, stringB);
        }

    }

    void ButtonClicked(string buttonname)
    {
      //Debug.Log(buttonname);
      kmlFileName = buttonname;
      readKML();
    }

    public List<string> ParseKMLString(string kmlString)
    {
        coordinates.Clear();

        XmlDocument doc = new XmlDocument();
        doc.LoadXml(kmlString);

        XmlNamespaceManager nsmgr = new XmlNamespaceManager(doc.NameTable);
        nsmgr.AddNamespace("kml", "http://www.opengis.net/kml/2.2");

        XmlNodeList coordinatesNodes = doc.SelectNodes("//kml:coordinates", nsmgr);

        foreach (XmlNode coordinatesNode in coordinatesNodes)
        {
			       //Debug.Log(coordinatesNode.InnerText);
          if(coordinatesNode.InnerText.Contains(" "))
          {
            coordinateStrings = coordinatesNode.InnerText.Trim().Split(new[] { '\n', '\r', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
          }

            foreach (string coordinateString in coordinateStrings)
            {
        				if(coordinateString.Contains(","))
        				{
        					string[] latLonAlt = coordinateString.Split(',');
        					string ll = latLonAlt[1] + "," + latLonAlt[0];
        					ll = ll.Replace(" ","");
        					coordinates.Add(ll);
        				}
            }
        }

        return coordinates;
    }


    // Function to get a list of all files in the persistent data path
    List<string> GetFileNames()
    {
        List<string> fileNames = new List<string>();

        // Get the path to the persistent data folder
        string dataPath = Application.persistentDataPath;

    	  #if UNITY_EDITOR
    		 dataPath = Application.streamingAssetsPath;
    	  #endif

        //Debug.Log(dataPath);
        // Get a list of all files in the folder
        DirectoryInfo directoryInfo = new DirectoryInfo(dataPath);
        FileInfo[] fileInfos = directoryInfo.GetFiles("*.kml");

        // Add the name of each file to the list
        foreach (FileInfo fileInfo in fileInfos)
        {
            fileNames.Add(fileInfo.Name);
        }

        return fileNames;
    }

    void ExtractStrings(string inputString, string stringA, string stringB)
    {
        nameList.Clear();
        centerList.Clear();
        coordinates.Clear();
		    extractedStrings.Clear();
        _google.OnClearMapClick();

        // Create a regular expression that matches any text between stringA and stringB
        Regex regex = new Regex(stringA + "(.*?)" + stringB);
        MatchCollection matches = regex.Matches(inputString);
        extractedStrings = new List<string>();

        // Loop through all the matches and extract the captured group (the text between stringA and stringB)
        foreach (Match match in matches)
        {
            extractedStrings.Add(match.Groups[1].Value);
        }

        Regex regexCD = new Regex(stringC + "(.*?)" + stringD);
        MatchCollection matchesCD = regexCD.Matches(inputString);

        // Loop through all the matches and extract the captured group (the text between stringA and stringB)
        foreach (Match match in matchesCD)
        {
          if(match.Groups[1].Value.Contains("+"))
          {
            nameList.Add(match.Groups[1].Value);
          }
        }

        if(gridlvl == 4)
        {
          Regex regexCenter = new Regex(@"Center point \(lat,lng\):\s*(.*?)&lt;br\/&gt;&lt;br\/&gt;&#xA;Bounding");
          MatchCollection matchesCenter = regexCenter.Matches(inputString);

          // Loop through all the matches and extract the captured group (the text between stringA and stringB)
          foreach (Match match in matchesCenter)
          {
              centerList.Add(match.Groups[1].Value);
          }
        }

        int count = 0;
        foreach (string extractedString in extractedStrings)
        {
          //coordinates.Clear();

          if(extractedString.Contains(" "))
          {
            coordinateStrings = extractedString.Trim().Split(new[] { '\n', '\r', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
          }

    		  int pointcount = 0;
    		  //Debug.Log("pointcount");
          foreach (string coordinateString in coordinateStrings)
          {
			  		pointcount += 1;

      				if(coordinateString.Contains(","))
      				{
    						if(pointcount < 4)
    						{
    						    string[] latLonAlt = coordinateString.Split(',');
      							string ll = latLonAlt[1] + "," + latLonAlt[0];
      							ll = ll.Replace(" ","");
      							coordinates.Add(ll);
    						}
      				}
          }

          count += 1;
		      if(count == 1)
          {
            first = extractedString;
          }
          if(count == (extractedStrings.Count/2))
          {
            mid = extractedString;
          }
		      if(count == extractedStrings.Count)
          {
            last = extractedString;
          }
        }

        coordinateStrings = first.Trim().Split(new[] { '\n', '\r', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
    		string[] latLonAltd = coordinateStrings[0].Split(',');
    		string lld = latLonAltd[1] + "," + latLonAltd[0];
    		lld = lld.Replace(" ","");

    		string southwestLat = latLonAltd[1];
    		string southwestLon = latLonAltd[0];

        coordinateStrings = last.Trim().Split(new[] { '\n', '\r', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
    		latLonAltd = coordinateStrings[2].Split(',');
    		lld = latLonAltd[1] + "," + latLonAltd[0];
    		lld = lld.Replace(" ","");

    		string northeastLat = latLonAltd[1];
    		string northeastLon = latLonAltd[0];

    		string ne = northeastLat + "," + northeastLon;
    		string nw = northeastLat + "," + southwestLon;
    		string sw = southwestLat + "," + southwestLon;

    		coordinates.Add(ne); //35.6225,139.71975
    		coordinates.Add(nw); //35.626375,139.71975
    		coordinates.Add(sw); //35.626375,139.729625

#if !UNITY_EDITOR
        _google.setRouteStrings(coordinates);
        if(gridlvl == 4)
        {
          StopCoroutine("plusCodesSale");
          StartCoroutine("plusCodesSale", centerList);
        }
#endif
        Resources.UnloadUnusedAssets();
    }

  IEnumerator plusCodesSale(List<string> plusList)
  {
	   //_geo.clearAnchors();
     List<double> currentposition = geo.returnLatLonDbl();

    yield return new WaitForEndOfFrame();
    _google.clearDictionary();
    yield return new WaitForEndOfFrame();

  	coordinateStrings = first.Trim().Split(new[] { '\n', '\r', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

  	string[] latLonSW = coordinateStrings[0].Split(',');
  	string[] latLonSE = coordinateStrings[1].Split(',');
  	string[] latLonNE = coordinateStrings[2].Split(',');

  	float lat1 = 0f;
  	float lon1 = 0f;
  	float lat2 = 0f;
  	float lon2 = 0f;

  	float.TryParse(latLonSW[1], out lat1);
  	float.TryParse(latLonSW[0], out lon1);
  	float.TryParse(latLonSE[1], out lat2);
  	float.TryParse(latLonSE[0], out lon2);

    float ewDistance1 = distance.CalculateDistance(lat1,lat2,lon1,lon2);

  	float.TryParse(latLonNE[1], out lat1);
  	float.TryParse(latLonNE[0], out lon1);

    float nsDistance1 = distance.CalculateDistance(lat1,lat2,lon1,lon2);

    yield return new WaitForEndOfFrame();

  	coordinateStrings = mid.Trim().Split(new[] { '\n', '\r', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

  	latLonSW = coordinateStrings[0].Split(',');
  	latLonSE = coordinateStrings[1].Split(',');
  	latLonNE = coordinateStrings[2].Split(',');

  	lat1 = 0f;
  	lon1 = 0f;
  	lat2 = 0f;
  	lon2 = 0f;

  	float.TryParse(latLonSW[1], out lat1);
  	float.TryParse(latLonSW[0], out lon1);
  	float.TryParse(latLonSE[1], out lat2);
  	float.TryParse(latLonSE[0], out lon2);

    float ewDistance2 = distance.CalculateDistance(lat1,lat2,lon1,lon2);

  	float.TryParse(latLonNE[1], out lat1);
  	float.TryParse(latLonNE[0], out lon1);

    float nsDistance2 = distance.CalculateDistance(lat1,lat2,lon1,lon2);

    yield return new WaitForEndOfFrame();

  	coordinateStrings = last.Trim().Split(new[] { '\n', '\r', ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);

  	latLonSW = coordinateStrings[0].Split(',');
  	latLonSE = coordinateStrings[1].Split(',');
  	latLonNE = coordinateStrings[2].Split(',');

  	lat1 = 0f;
  	lon1 = 0f;
  	lat2 = 0f;
  	lon2 = 0f;

  	float.TryParse(latLonSW[1], out lat1);
  	float.TryParse(latLonSW[0], out lon1);
  	float.TryParse(latLonSE[1], out lat2);
  	float.TryParse(latLonSE[0], out lon2);

    float ewDistance3 = distance.CalculateDistance(lat1,lat2,lon1,lon2);

  	float.TryParse(latLonNE[1], out lat1);
  	float.TryParse(latLonNE[0], out lon1);

    float nsDistance3 = distance.CalculateDistance(lat1,lat2,lon1,lon2);
  	float avEWDistance = ((ewDistance1 + ewDistance2 + ewDistance3)/3) - 4f;
  	float avNSDistance = ((nsDistance1 + nsDistance2 + nsDistance3)/3) - 4f;

    int counting = 0;

    foreach(string s in plusList)
    {
  		float latpc = 0f;
  		float lonpc = 0f;
  		string[] latlonpc = s.Split(',');
  		float.TryParse(latlonpc[0], out latpc);
  		float.TryParse(latlonpc[1], out lonpc);

      if(visitedpluscodeList.Contains(s))
      {
        _google.setOverLaySimple(latpc,lonpc,avNSDistance,avEWDistance,1,nameList[counting]);
      }

      float dist = distance.CalculateDistance((float)latpc,(float)currentposition[0],(float)lonpc,(float)currentposition[1]);

      if(dist < 30)
      {
        if(visitedpluscodeList.Contains(s))
        {
          if(!anchorpluscodeList.Contains(s))
          {
            _geo.setAnchorPlus(latpc,lonpc,true,s);
            anchorpluscodeList.Add(s);
            Debug.Log("anchor placed");
            yield return new WaitForSeconds(0.5f);
          }
          else
          {
            GameObject g = GameObject.Find(s);
            Debug.Log("Found " + s + " changed to visited");
            g.GetComponent<setStatus>().setCubeStatus(true);
          }
        }
        else
        {
          if(!anchorpluscodeList.Contains(s))
          {
            _geo.setAnchorPlus(latpc,lonpc,false,s);
            anchorpluscodeList.Add(s);
            Debug.Log("anchor placed");
            yield return new WaitForSeconds(0.5f);
          }
          else
          {
            GameObject g = GameObject.Find(s);
            Debug.Log("Found " + s + " no need to change");
          }
        }
      }
      else
      {
        Debug.Log("do not place, out of range");
      }

      counting += 1;
    }

  }

//    float ewDistance2 = distance.CalculateDistance(lat1,lat2,lon1,lon2);

  /*

  foreach(string s in plusList)
  {
    float latpc = 0f;
    float lonpc = 0f;
    string[] latlonpc = s.Split(',');
    float.TryParse(latlonpc[0], out latpc);
    float.TryParse(latlonpc[1], out lonpc);

    if(visitedpluscodeList.Contains(s))
    {
      _geo.setAnchorPlus(latpc,lonpc,true);
    }
    else
    {
      _geo.setAnchorPlus(latpc,lonpc,false);
    }
    yield return new WaitForSeconds(0.5f);
  }

  */

  public string cleanPlusCode(string s)
  {
    var stripped = Regex.Replace(s, "[^0-9.]", "");
    return stripped;
  }

  public void kmlUI(string plusid, int available)
  {
    idText.text = plusid;
    idTextGreen.text = plusid;
    idTextBlue.text = plusid;

    foreach(GameObject g in mapUIs)
    {
      g.SetActive(false);
    }
    mapUIs[available].SetActive(true);
    idText.gameObject.SetActive(true);
    //available 0 = red, 1 = blue, 2 = yellow, 4 = green
  }

  public void fadeUIGreen(bool off)
  {
	  StartCoroutine(fadeUI(true, off));
  }
    public void fadeUIBlue(bool off)
  {
	  StartCoroutine(fadeUI(false, off));
  }

  IEnumerator fadeUI(bool green, bool off)
  {
	  Color c = Color.white;
	  Color b = Color.black;
	  c.a = 0;
	  c.a = 0;
	  float dir = 0.05f;
	  if(green == true)
	  {
		imgTemp =  imgGreen;
	  }
	  else
	  {
		imgTemp = imgBlue;
	  }

	  if(off == true)
	  {
		dir = -0.1f;
		c.a = 1;
		b.a = 1;
	  }
	  else
	  {
		 yield return new WaitForSeconds(1);
	  }

	  	foreach(Image img in imgTemp)
		{
		  	img.color = c;
			img.gameObject.SetActive(true);
		}

	  	if(green == true)
		{
			idTextGreen.color =  b;
			idTextGreen.gameObject.SetActive(true);
		}
		else
		{
			idTextBlue.color =  b;
			idTextBlue.gameObject.SetActive(true);
		}

	  while(true)
	  {
		c.a += dir;
		b.a += dir;
		foreach(Image img in imgTemp)
		{
		  	img.color = c;
		}

		if(green == true)
		{
			idTextGreen.color =  b;
		}
		else
		{
			idTextBlue.color =  b;
		}

		yield return new WaitForEndOfFrame();
		if(off == true && c.a < 0)
		{
			foreach(Image img in imgTemp)
			{
				img.gameObject.SetActive(false);
			}
			idTextGreen.gameObject.SetActive(false);
			idTextBlue.gameObject.SetActive(false);
			yield break;
		}
		else if(off == false && c.a > 1)
		{
			yield break;
		}
	  }
  }

}

/*
<kml>
 <Document>
  <StyleMap id="stylemap_level_0">
   <Pair>
    <key>normal</key>
    <styleUrl>#normal_style_level_0</styleUrl>
   </Pair>
   <Pair>
    <key>highlight</key>
    <styleUrl>#highlight_style_level_0</styleUrl>
   </Pair>
  </StyleMap>
  <StyleMap id="stylemap_level_1">
   <Pair>
    <key>normal</key>
    <styleUrl>#normal_style_level_1</styleUrl>
   </Pair>
   <Pair>
    <key>highlight</key>
    <styleUrl>#highlight_style_level_1</styleUrl>
   </Pair>
  </StyleMap>
  <StyleMap id="stylemap_level_2">
   <Pair>
    <key>normal</key>
    <styleUrl>#normal_style_level_2</styleUrl>
   </Pair>
   <Pair>
    <key>highlight</key>
    <styleUrl>#highlight_style_level_2</styleUrl>
   </Pair>
  </StyleMap>
  <StyleMap id="stylemap_level_3">
   <Pair>
    <key>normal</key>
    <styleUrl>#normal_style_level_3</styleUrl>
   </Pair>
   <Pair>
    <key>highlight</key>
    <styleUrl>#highlight_style_level_3</styleUrl>
   </Pair>
  </StyleMap>
  <StyleMap id="stylemap_level_4">
   <Pair>
    <key>normal</key>
    <styleUrl>#normal_style_level_4</styleUrl>
   </Pair>
   <Pair>
    <key>highlight</key>
    <styleUrl>#highlight_style_level_4</styleUrl>
   </Pair>
  </StyleMap>
  <StyleMap id="stylemap_level_5">
   <Pair>
    <key>normal</key>
    <styleUrl>#normal_style_level_5</styleUrl>
   </Pair>
   <Pair>
    <key>highlight</key>
    <styleUrl>#highlight_style_level_5</styleUrl>
   </Pair>
  </StyleMap>
  <StyleMap id="stylemap_level_6">
   <Pair>
    <key>normal</key>
    <styleUrl>#normal_style_level_6</styleUrl>
   </Pair>
   <Pair>
    <key>highlight</key>
    <styleUrl>#highlight_style_level_6</styleUrl>
   </Pair>
  </StyleMap>
  <Style id="normal_style_level_0">
   <BalloonStyle>
    <text>&#xA;&lt;div style=&#34;color:#202124;padding:20px;min-width:20em;&#34;&gt;$[description]&lt;br/&gt;&lt;br/&gt;&lt;a href=&#34;https://plus.codes&#34;&gt;&lt;img style=&#34;width:150px;&#34; src=&#34;https://plus.codes/static/about/images/utils/pc-logo.svg&#34;&gt;&lt;/a&gt;&lt;/div&gt;&#xA;</text>
   </BalloonStyle>
   <LineStyle>
    <color>#7fa039f4</color>
    <width>1</width>
   </LineStyle>
   <PolyStyle>
    <color>#7fa039f4</color>
    <fill>0</fill>
    <outline>1</outline>
   </PolyStyle>
  </Style>
  <Style id="highlight_style_level_0">
   <BalloonStyle>
    <text>&#xA;&lt;div style=&#34;color:#202124;padding:20px;min-width:20em;&#34;&gt;$[description]&lt;br/&gt;&lt;br/&gt;&lt;a href=&#34;https://plus.codes&#34;&gt;&lt;img style=&#34;width:150px;&#34; src=&#34;https://plus.codes/static/about/images/utils/pc-logo.svg&#34;&gt;&lt;/a&gt;&lt;/div&gt;&#xA;</text>
   </BalloonStyle>
   <LineStyle>
    <color>#ffa039f4</color>
    <width>3</width>
   </LineStyle>
   <PolyStyle>
    <color>#7fa039f4</color>
    <fill>1</fill>
    <outline>1</outline>
   </PolyStyle>
  </Style>
  <Style id="gridline_level_0">
   <LineStyle>
    <color>#ffa039f4</color>
    <width>5</width>
   </LineStyle>
  </Style>
  <Style id="normal_style_level_1">
   <BalloonStyle>
    <text>&#xA;&lt;div style=&#34;color:#202124;padding:20px;min-width:20em;&#34;&gt;$[description]&lt;br/&gt;&lt;br/&gt;&lt;a href=&#34;https://plus.codes&#34;&gt;&lt;img style=&#34;width:150px;&#34; src=&#34;https://plus.codes/static/about/images/utils/pc-logo.svg&#34;&gt;&lt;/a&gt;&lt;/div&gt;&#xA;</text>
   </BalloonStyle>
   <LineStyle>
    <color>#7f3543ea</color>
    <width>1</width>
   </LineStyle>
   <PolyStyle>
    <color>#7f3543ea</color>
    <fill>0</fill>
    <outline>1</outline>
   </PolyStyle>
  </Style>
  <Style id="highlight_style_level_1">
   <BalloonStyle>
    <text>&#xA;&lt;div style=&#34;color:#202124;padding:20px;min-width:20em;&#34;&gt;$[description]&lt;br/&gt;&lt;br/&gt;&lt;a href=&#34;https://plus.codes&#34;&gt;&lt;img style=&#34;width:150px;&#34; src=&#34;https://plus.codes/static/about/images/utils/pc-logo.svg&#34;&gt;&lt;/a&gt;&lt;/div&gt;&#xA;</text>
   </BalloonStyle>
   <LineStyle>
    <color>#ff3543ea</color>
    <width>3</width>
   </LineStyle>
   <PolyStyle>
    <color>#7f3543ea</color>
    <fill>1</fill>
    <outline>1</outline>
   </PolyStyle>
  </Style>
  <Style id="gridline_level_1">
   <LineStyle>
    <color>#ff3543ea</color>
    <width>5</width>
   </LineStyle>
  </Style>
  <Style id="normal_style_level_2">
   <BalloonStyle>
    <text>&#xA;&lt;div style=&#34;color:#202124;padding:20px;min-width:20em;&#34;&gt;$[description]&lt;br/&gt;&lt;br/&gt;&lt;a href=&#34;https://plus.codes&#34;&gt;&lt;img style=&#34;width:150px;&#34; src=&#34;https://plus.codes/static/about/images/utils/pc-logo.svg&#34;&gt;&lt;/a&gt;&lt;/div&gt;&#xA;</text>
   </BalloonStyle>
   <LineStyle>
    <color>#7f177bfa</color>
    <width>1</width>
   </LineStyle>
   <PolyStyle>
    <color>#7f177bfa</color>
    <fill>0</fill>
    <outline>1</outline>
   </PolyStyle>
  </Style>
  <Style id="highlight_style_level_2">
   <BalloonStyle>
    <text>&#xA;&lt;div style=&#34;color:#202124;padding:20px;min-width:20em;&#34;&gt;$[description]&lt;br/&gt;&lt;br/&gt;&lt;a href=&#34;https://plus.codes&#34;&gt;&lt;img style=&#34;width:150px;&#34; src=&#34;https://plus.codes/static/about/images/utils/pc-logo.svg&#34;&gt;&lt;/a&gt;&lt;/div&gt;&#xA;</text>
   </BalloonStyle>
   <LineStyle>
    <color>#ff177bfa</color>
    <width>3</width>
   </LineStyle>
   <PolyStyle>
    <color>#7f177bfa</color>
    <fill>1</fill>
    <outline>1</outline>
   </PolyStyle>
  </Style>
  <Style id="gridline_level_2">
   <LineStyle>
    <color>#ff177bfa</color>
    <width>5</width>
   </LineStyle>
  </Style>
  <Style id="normal_style_level_3">
   <BalloonStyle>
    <text>&#xA;&lt;div style=&#34;color:#202124;padding:20px;min-width:20em;&#34;&gt;$[description]&lt;br/&gt;&lt;br/&gt;&lt;a href=&#34;https://plus.codes&#34;&gt;&lt;img style=&#34;width:150px;&#34; src=&#34;https://plus.codes/static/about/images/utils/pc-logo.svg&#34;&gt;&lt;/a&gt;&lt;/div&gt;&#xA;</text>
   </BalloonStyle>
   <LineStyle>
    <color>#7f04bcfb</color>
    <width>1</width>
   </LineStyle>
   <PolyStyle>
    <color>#7f04bcfb</color>
    <fill>0</fill>
    <outline>1</outline>
   </PolyStyle>
  </Style>
  <Style id="highlight_style_level_3">
   <BalloonStyle>
    <text>&#xA;&lt;div style=&#34;color:#202124;padding:20px;min-width:20em;&#34;&gt;$[description]&lt;br/&gt;&lt;br/&gt;&lt;a href=&#34;https://plus.codes&#34;&gt;&lt;img style=&#34;width:150px;&#34; src=&#34;https://plus.codes/static/about/images/utils/pc-logo.svg&#34;&gt;&lt;/a&gt;&lt;/div&gt;&#xA;</text>
   </BalloonStyle>
   <LineStyle>
    <color>#ff04bcfb</color>
    <width>3</width>
   </LineStyle>
   <PolyStyle>
    <color>#7f04bcfb</color>
    <fill>1</fill>
    <outline>1</outline>
   </PolyStyle>
  </Style>
  <Style id="gridline_level_3">
   <LineStyle>
    <color>#ff04bcfb</color>
    <width>5</width>
   </LineStyle>
  </Style>
  <Style id="normal_style_level_4">
   <BalloonStyle>
    <text>&#xA;&lt;div style=&#34;color:#202124;padding:20px;min-width:20em;&#34;&gt;$[description]&lt;br/&gt;&lt;br/&gt;&lt;a href=&#34;https://plus.codes&#34;&gt;&lt;img style=&#34;width:150px;&#34; src=&#34;https://plus.codes/static/about/images/utils/pc-logo.svg&#34;&gt;&lt;/a&gt;&lt;/div&gt;&#xA;</text>
   </BalloonStyle>
   <LineStyle>
    <color>#7f53a834</color>
    <width>1</width>
   </LineStyle>
   <PolyStyle>
    <color>#7f53a834</color>
    <fill>0</fill>
    <outline>1</outline>
   </PolyStyle>
  </Style>
  <Style id="highlight_style_level_4">
   <BalloonStyle>
    <text>&#xA;&lt;div style=&#34;color:#202124;padding:20px;min-width:20em;&#34;&gt;$[description]&lt;br/&gt;&lt;br/&gt;&lt;a href=&#34;https://plus.codes&#34;&gt;&lt;img style=&#34;width:150px;&#34; src=&#34;https://plus.codes/static/about/images/utils/pc-logo.svg&#34;&gt;&lt;/a&gt;&lt;/div&gt;&#xA;</text>
   </BalloonStyle>
   <LineStyle>
    <color>#ff53a834</color>
    <width>3</width>
   </LineStyle>
   <PolyStyle>
    <color>#7f53a834</color>
    <fill>1</fill>
    <outline>1</outline>
   </PolyStyle>
  </Style>
  <Style id="gridline_level_4">
   <LineStyle>
    <color>#ff53a834</color>
    <width>5</width>
   </LineStyle>
  </Style>
  <Style id="normal_style_level_5">
   <BalloonStyle>
    <text>&#xA;&lt;div style=&#34;color:#202124;padding:20px;min-width:20em;&#34;&gt;$[description]&lt;br/&gt;&lt;br/&gt;&lt;a href=&#34;https://plus.codes&#34;&gt;&lt;img style=&#34;width:150px;&#34; src=&#34;https://plus.codes/static/about/images/utils/pc-logo.svg&#34;&gt;&lt;/a&gt;&lt;/div&gt;&#xA;</text>
   </BalloonStyle>
   <LineStyle>
    <color>#7fe0c124</color>
    <width>1</width>
   </LineStyle>
   <PolyStyle>
    <color>#7fe0c124</color>
    <fill>0</fill>
    <outline>1</outline>
   </PolyStyle>
  </Style>
  <Style id="highlight_style_level_5">
   <BalloonStyle>
    <text>&#xA;&lt;div style=&#34;color:#202124;padding:20px;min-width:20em;&#34;&gt;$[description]&lt;br/&gt;&lt;br/&gt;&lt;a href=&#34;https://plus.codes&#34;&gt;&lt;img style=&#34;width:150px;&#34; src=&#34;https://plus.codes/static/about/images/utils/pc-logo.svg&#34;&gt;&lt;/a&gt;&lt;/div&gt;&#xA;</text>
   </BalloonStyle>
   <LineStyle>
    <color>#ffe0c124</color>
    <width>3</width>
   </LineStyle>
   <PolyStyle>
    <color>#7fe0c124</color>
    <fill>1</fill>
    <outline>1</outline>
   </PolyStyle>
  </Style>
  <Style id="gridline_level_5">
   <LineStyle>
    <color>#ffe0c124</color>
    <width>5</width>
   </LineStyle>
  </Style>
  <Style id="normal_style_level_6">
   <BalloonStyle>
    <text>&#xA;&lt;div style=&#34;color:#202124;padding:20px;min-width:20em;&#34;&gt;$[description]&lt;br/&gt;&lt;br/&gt;&lt;a href=&#34;https://plus.codes&#34;&gt;&lt;img style=&#34;width:150px;&#34; src=&#34;https://plus.codes/static/about/images/utils/pc-logo.svg&#34;&gt;&lt;/a&gt;&lt;/div&gt;&#xA;</text>
   </BalloonStyle>
   <LineStyle>
    <color>#7ff48542</color>
    <width>1</width>
   </LineStyle>
   <PolyStyle>
    <color>#7ff48542</color>
    <fill>0</fill>
    <outline>1</outline>
   </PolyStyle>
  </Style>
  <Style id="highlight_style_level_6">
   <BalloonStyle>
    <text>&#xA;&lt;div style=&#34;color:#202124;padding:20px;min-width:20em;&#34;&gt;$[description]&lt;br/&gt;&lt;br/&gt;&lt;a href=&#34;https://plus.codes&#34;&gt;&lt;img style=&#34;width:150px;&#34; src=&#34;https://plus.codes/static/about/images/utils/pc-logo.svg&#34;&gt;&lt;/a&gt;&lt;/div&gt;&#xA;</text>
   </BalloonStyle>
   <LineStyle>
    <color>#fff48542</color>
    <width>3</width>
   </LineStyle>
   <PolyStyle>
    <color>#7ff48542</color>
    <fill>1</fill>
    <outline>1</outline>
   </PolyStyle>
  </Style>
  <Style id="gridline_level_6">
   <LineStyle>
    <color>#fff48542</color>
    <width>5</width>
   </LineStyle>
  </Style>
  <Folder>
   <name>Grid (level 3)</name>
   <description></description>
   <Placemark>
    <name>8Q7XJPF9+</name>
    <description>&#xA;&lt;b&gt;&lt;font color=&#34;#80868B&#34; size=&#34;+3&#34;&gt;8Q7X&lt;/font&gt;&lt;font size=&#34;+3&#34;&gt;JPF9+&lt;/font&gt;&lt;/b&gt;&lt;br/&gt;&lt;br/&gt;&#xA;Code length: 8 digits&lt;br/&gt;&#xA;Center point (lat,lng): 35.62375,139.71875&lt;br/&gt;&lt;br/&gt;&#xA;Bounding box&lt;br/&gt;SW (lat,lng): 35.6225,139.7175&lt;br/&gt;NE (lat,lng): 35.625,139.72&lt;br/&gt;&lt;br/&gt;&#xA;Resolution&lt;br/&gt;Lat: 0.0025&lt;br/&gt;Lng: 0.0025&#xA;</description>
    <Polygon>
     <tessellate>1</tessellate>
     <outerBoundaryIs>
      <LinearRing>
       <coordinates>139.7175,35.6225,0 139.72,35.6225,0 139.72,35.625,0 139.7175,35.625,0 139.7175,35.6225,0</coordinates>
      </LinearRing>
     </outerBoundaryIs>
    </Polygon>
    <styleUrl>#stylemap_level_3</styleUrl>
   </Placemark>
   <Placemark>
    <name>8Q7XJPFC+</name>
    <description>&#xA;&lt;b&gt;&lt;font color=&#34;#80868B&#34; size=&#34;+3&#34;&gt;8Q7X&lt;/font&gt;&lt;font size=&#34;+3&#34;&gt;JPFC+&lt;/font&gt;&lt;/b&gt;&lt;br/&gt;&lt;br/&gt;&#xA;Code length: 8 digits&lt;br/&gt;&#xA;Center point (lat,lng): 35.62375,139.72125&lt;br/&gt;&lt;br/&gt;&#xA;Bounding box&lt;br/&gt;SW (lat,lng): 35.6225,139.72&lt;br/&gt;NE (lat,lng): 35.625,139.7225&lt;br/&gt;&lt;br/&gt;&#xA;Resolution&lt;br/&gt;Lat: 0.0025&lt;br/&gt;Lng: 0.0025&#xA;</description>
    <Polygon>
     <tessellate>1</tessellate>
     <outerBoundaryIs>
      <LinearRing>
       <coordinates>139.72,35.6225,0 139.7225,35.6225,0 139.7225,35.625,0 139.72,35.625,0 139.72,35.6225,0</coordinates>
      </LinearRing>
     </outerBoundaryIs>
    </Polygon>
    <styleUrl>#stylemap_level_3</styleUrl>
   </Placemark>
   <Placemark>
    <name>8Q7XJPFF+</name>
    <description>&#xA;&lt;b&gt;&lt;font color=&#34;#80868B&#34; size=&#34;+3&#34;&gt;8Q7X&lt;/font&gt;&lt;font size=&#34;+3&#34;&gt;JPFF+&lt;/font&gt;&lt;/b&gt;&lt;br/&gt;&lt;br/&gt;&#xA;Code length: 8 digits&lt;br/&gt;&#xA;Center point (lat,lng): 35.62375,139.72375&lt;br/&gt;&lt;br/&gt;&#xA;Bounding box&lt;br/&gt;SW (lat,lng): 35.6225,139.7225&lt;br/&gt;NE (lat,lng): 35.625,139.725&lt;br/&gt;&lt;br/&gt;&#xA;Resolution&lt;br/&gt;Lat: 0.0025&lt;br/&gt;Lng: 0.0025&#xA;</description>
    <Polygon>
     <tessellate>1</tessellate>
     <outerBoundaryIs>
      <LinearRing>
       <coordinates>139.7225,35.6225,0 139.725,35.6225,0 139.725,35.625,0 139.7225,35.625,0 139.7225,35.6225,0</coordinates>
      </LinearRing>
     </outerBoundaryIs>
    </Polygon>
    <styleUrl>#stylemap_level_3</styleUrl>
   </Placemark>
   <Placemark>
    <name>8Q7XJPFG+</name>
    <description>&#xA;&lt;b&gt;&lt;font color=&#34;#80868B&#34; size=&#34;+3&#34;&gt;8Q7X&lt;/font&gt;&lt;font size=&#34;+3&#34;&gt;JPFG+&lt;/font&gt;&lt;/b&gt;&lt;br/&gt;&lt;br/&gt;&#xA;Code length: 8 digits&lt;br/&gt;&#xA;Center point (lat,lng): 35.62375,139.72625&lt;br/&gt;&lt;br/&gt;&#xA;Bounding box&lt;br/&gt;SW (lat,lng): 35.6225,139.725&lt;br/&gt;NE (lat,lng): 35.625,139.7275&lt;br/&gt;&lt;br/&gt;&#xA;Resolution&lt;br/&gt;Lat: 0.0025&lt;br/&gt;Lng: 0.0025&#xA;</description>
    <Polygon>
     <tessellate>1</tessellate>
     <outerBoundaryIs>
      <LinearRing>
       <coordinates>139.725,35.6225,0 139.7275,35.6225,0 139.7275,35.625,0 139.725,35.625,0 139.725,35.6225,0</coordinates>
      </LinearRing>
     </outerBoundaryIs>
    </Polygon>
    <styleUrl>#stylemap_level_3</styleUrl>
   </Placemark>
   <Placemark>
    <name>8Q7XJPFH+</name>
    <description>&#xA;&lt;b&gt;&lt;font color=&#34;#80868B&#34; size=&#34;+3&#34;&gt;8Q7X&lt;/font&gt;&lt;font size=&#34;+3&#34;&gt;JPFH+&lt;/font&gt;&lt;/b&gt;&lt;br/&gt;&lt;br/&gt;&#xA;Code length: 8 digits&lt;br/&gt;&#xA;Center point (lat,lng): 35.62375,139.72875&lt;br/&gt;&lt;br/&gt;&#xA;Bounding box&lt;br/&gt;SW (lat,lng): 35.6225,139.7275&lt;br/&gt;NE (lat,lng): 35.625,139.73&lt;br/&gt;&lt;br/&gt;&#xA;Resolution&lt;br/&gt;Lat: 0.0025&lt;br/&gt;Lng: 0.0025&#xA;</description>
    <Polygon>
     <tessellate>1</tessellate>
     <outerBoundaryIs>
      <LinearRing>
       <coordinates>139.7275,35.6225,0 139.73,35.6225,0 139.73,35.625,0 139.7275,35.625,0 139.7275,35.6225,0</coordinates>
      </LinearRing>
     </outerBoundaryIs>
    </Polygon>
    <styleUrl>#stylemap_level_3</styleUrl>
   </Placemark>
   <Placemark>
    <name>8Q7XJPG9+</name>
    <description>&#xA;&lt;b&gt;&lt;font color=&#34;#80868B&#34; size=&#34;+3&#34;&gt;8Q7X&lt;/font&gt;&lt;font size=&#34;+3&#34;&gt;JPG9+&lt;/font&gt;&lt;/b&gt;&lt;br/&gt;&lt;br/&gt;&#xA;Code length: 8 digits&lt;br/&gt;&#xA;Center point (lat,lng): 35.62625,139.71875&lt;br/&gt;&lt;br/&gt;&#xA;Bounding box&lt;br/&gt;SW (lat,lng): 35.625,139.7175&lt;br/&gt;NE (lat,lng): 35.6275,139.72&lt;br/&gt;&lt;br/&gt;&#xA;Resolution&lt;br/&gt;Lat: 0.0025&lt;br/&gt;Lng: 0.0025&#xA;</description>
    <Polygon>
     <tessellate>1</tessellate>
     <outerBoundaryIs>
      <LinearRing>
       <coordinates>139.7175,35.625,0 139.72,35.625,0 139.72,35.6275,0 139.7175,35.6275,0 139.7175,35.625,0</coordinates>
      </LinearRing>
     </outerBoundaryIs>
    </Polygon>
    <styleUrl>#stylemap_level_3</styleUrl>
   </Placemark>
   <Placemark>
    <name>8Q7XJPGC+</name>
    <description>&#xA;&lt;b&gt;&lt;font color=&#34;#80868B&#34; size=&#34;+3&#34;&gt;8Q7X&lt;/font&gt;&lt;font size=&#34;+3&#34;&gt;JPGC+&lt;/font&gt;&lt;/b&gt;&lt;br/&gt;&lt;br/&gt;&#xA;Code length: 8 digits&lt;br/&gt;&#xA;Center point (lat,lng): 35.62625,139.72125&lt;br/&gt;&lt;br/&gt;&#xA;Bounding box&lt;br/&gt;SW (lat,lng): 35.625,139.72&lt;br/&gt;NE (lat,lng): 35.6275,139.7225&lt;b<message truncated>

*/
