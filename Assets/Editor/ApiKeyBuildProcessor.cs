using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

using System.IO;

using Google.XR.ARCoreExtensions.Samples.Geospatial;

public class ApiKeyBuildProcessor : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }

    directionsPOI geo;
    travelAPI travel;

    public void OnPreprocessBuild(BuildReport report)
    {
        string filePath = @"C:\Users\designium_tower\Documents\Hackathon\apiKey.txt";
        string geoPath = @"C:\Users\designium_tower\Documents\Hackathon\geoapify.txt";

        if (!File.Exists(filePath))
        {
            Debug.LogError("Google API Key file not found at: " + filePath);
            return;
        }
        if (!File.Exists(geoPath))
        {
            Debug.LogError("Geoapify API Key file not found at: " + filePath);
            return;
        }

        string apiKey = File.ReadAllText(filePath).Trim();
        string geoKey = File.ReadAllText(geoPath).Trim();

        directionsPOI geo = GameObject.FindObjectOfType<directionsPOI>();
        travelAPI travel = GameObject.FindObjectOfType<travelAPI>();

        if (geo != null && travel != null )
        {
            geo.setAPI(apiKey);
            travel.setAPI(geoKey);

            Debug.Log(apiKey + " & " + geoKey);
        }
        else
        {
          Debug.LogError("scripts not found");
        }
    }
}
