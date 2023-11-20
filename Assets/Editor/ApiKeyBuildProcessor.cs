using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

using System.IO;

using Google.XR.ARCoreExtensions.Samples.Geospatial;

public class ApiKeyBuildProcessor : IPreprocessBuildWithReport
{
    public int callbackOrder { get { return 0; } }

    public GeospatialController geo;

    public void OnPreprocessBuild(BuildReport report)
    {
        string filePath = @"C:\Users\designium_tower\Documents\Hackathon\apiKey.txt";

        if (!File.Exists(filePath))
        {
            Debug.LogError("API Key file not found at: " + filePath);
            return;
        }

        string apiKey = File.ReadAllText(filePath).Trim();

        GeospatialController geo = GameObject.FindObjectOfType<GeospatialController>();
        if (geo != null)
        {
            geo.setAPI(apiKey);
            Debug.Log(apiKey);
        }
        else
        {
          Debug.LogError("GeospatialController not found");
        }
    }
}
