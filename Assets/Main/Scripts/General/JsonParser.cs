using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using Newtonsoft.Json;

[System.Serializable]
public class AddressComponent
{
    public string long_name;
    public string short_name;
    public string[] types;
}

[System.Serializable]
public class Location
{
    public double lat;
    public double lng;
}

[System.Serializable]
public class Northeast
{
    public double lat;
    public double lng;
}

[System.Serializable]
public class Southwest
{
    public double lat;
    public double lng;
}

[System.Serializable]
public class Viewport
{
    public Northeast northeast;
    public Southwest southwest;
}

[System.Serializable]
public class Geometry
{
    public Location location;
    public string location_type;
    public Viewport viewport;
}

[System.Serializable]
public class PlusCode
{
    public string compound_code;
    public string global_code;
}

[System.Serializable]
public class Result
{
    public AddressComponent[] address_components;
    public string formatted_address;
    public Geometry geometry;
    public bool partial_match;
    public string place_id;
    public PlusCode plus_code;
    public string[] types;
}

[System.Serializable]
public class Root
{
    public Result[] results;
    public string status;
}

public class JsonParser : MonoBehaviour
{
    //public string json; // Input JSON string

    public void StartParse(string json)
    {
        // Parse the JSON string
        Root root = JsonConvert.DeserializeObject<Root>(json);

        // Access the parsed data
        Debug.Log("Status: " + root.status);
        Debug.Log("Formatted Address: " + root.results[0].formatted_address);
        Debug.Log("Latitude: " + root.results[0].geometry.location.lat);
        Debug.Log("Longitude: " + root.results[0].geometry.location.lng);
        Debug.Log("Place ID: " + root.results[0].place_id);
        Debug.Log("Compound Code: " + root.results[0].plus_code.compound_code);
        // Access other properties as needed
    }

    public string stringParse(string json)
    {
        // Parse the JSON string
        Root root = JsonConvert.DeserializeObject<Root>(json);

        string s = "null";

        if(root.status == "OK")
        {
          s = root.results[0].geometry.location.lat + "," + root.results[0].geometry.location.lng;
          //Debug.Log(s);
        }

        return s;
    }
	
	public List<string> stringParseList(string json)
    {
        // Parse the JSON string
        Root root = JsonConvert.DeserializeObject<Root>(json);

        List<string> s = new List<string>();

        if(root.status == "OK")
        {
          s.Add(root.results[0].geometry.location.lat.ToString()); 
		  s.Add(root.results[0].geometry.location.lng.ToString()); 
        }

        return s;
    }
	
	public string locParse(string json)
    {
        // Parse the JSON string
        Root root = JsonConvert.DeserializeObject<Root>(json);

        string s = "null";

        if(root.status == "OK")
        {
          s = root.results[0].formatted_address;
          //Debug.Log(s);
        }

        return s;
    }
}
