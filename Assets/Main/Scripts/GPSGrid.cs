using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
	
using Google.XR.ARCoreExtensions;
using Google.XR.ARCoreExtensions.Samples.Geospatial;

public class GPSGrid : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
	
	//lat dist 13.54
	//lon dist 11.05

    public List<double>ToNorthPosition(List<double>canterlatlng, double northDistance)
    {
		List<double>newlatlng = new List<double>();
        double r_earth = 6378;
        var pi = Math.PI;
        var new_latitude = canterlatlng[0] + (northDistance / r_earth) * (180 / pi);
		newlatlng.Add(new_latitude);
		newlatlng.Add(canterlatlng[1]);
		return newlatlng;
    }

    public List<double>ToEastPosition(List<double>canterlatlng, double eastDistance)
    {
		List<double>newlatlng = new List<double>();
        double r_earth = 6378;
        var pi = Math.PI;
        var new_longitude = canterlatlng[1] + (eastDistance / r_earth) * (180 / pi) / Math.Cos(canterlatlng[0] * pi / 180);
		newlatlng.Add(canterlatlng[0]);
		newlatlng.Add(new_longitude);
		return newlatlng;
    }

    public List<double>ToSouthPosition(List<double>canterlatlng, double southDistance)
    {
		List<double>newlatlng = new List<double>();
        double r_earth = 6378;
        var pi = Math.PI;
        var new_latitude = canterlatlng[0] - (southDistance / r_earth) * (180 / pi);
		newlatlng.Add(new_latitude);
		newlatlng.Add(canterlatlng[1]);
		return newlatlng;
    }

    public List<double>ToWestPosition(List<double>canterlatlng, double westDistance)
    {
		List<double>newlatlng = new List<double>();
        double r_earth = 6378;
        var pi = Math.PI;
        var new_longitude = canterlatlng[1] - (westDistance / r_earth) * (180 / pi) / Math.Cos(canterlatlng[0] * pi / 180);
		newlatlng.Add(canterlatlng[0]);
		newlatlng.Add(new_longitude);
		return newlatlng;
    }
}
