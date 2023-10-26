using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;

using TMPro;

public class setAddress : MonoBehaviour
{

    public TMP_Text addressTxt;
    public List<string> geocodeList = new List<string>();
    public List<string> addressList = new List<string>();

    public string debugString;

    void Update()
    {
      if(Input.GetKeyUp(KeyCode.Alpha7))
      {
        googleGeocodefromPlace(debugString);
      }
    }

    public void googleGeocode(float latitude, float longitude)
    {
     StartCoroutine(GetGeocode(latitude,longitude));
    }

    public void googleGeocodefromPlace(string name)
    {
     StartCoroutine(GetGeocodefromPlace(name));
    }

    IEnumerator GetGeocode(float latitude, float longitude)
    {

    var debuglat = latitude;
    var debuglon = longitude;

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
         //Debug.Log(www.downloadHandler.text);
         geocodeList.Clear();
         addressList.Clear();
         yield return new WaitForEndOfFrame();
         geocodeList = www.downloadHandler.text.Split('\n').ToList();

         foreach(string s in geocodeList)
         {
           if(s.Contains("formatted_address"))
           {
             addressList.Add(s);
           }
         }

         Debug.Log(addressList[0]);

         geocodeList.Clear();
         geocodeList = addressList[0].Split('\"').ToList();
         addressTxt.text = geocodeList[3];

         yield return new WaitForEndOfFrame();
         geocodeList.Clear();
         addressList.Clear();
         Resources.UnloadUnusedAssets();
       }
     }

     IEnumerator GetGeocodefromPlace(string name)
     {

      yield return new WaitForEndOfFrame();

      UnityWebRequest www = UnityWebRequest.Get("https://maps.googleapis.com/maps/api/geocode/json?address=" + name + "&key=AIzaSyCLoCRs3w-AGrm7_AwCjrCq9W9gNQLOC6c");

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
          yield return new WaitForEndOfFrame();
          geocodeList = www.downloadHandler.text.Split('\n').ToList();

          foreach(string s in geocodeList)
          {
            if(s.Contains("formatted_address"))
            {
              addressList.Add(s);
            }
          }

          Debug.Log(addressList[0]);

          //geocodeList.Clear();
          //geocodeList = addressList[0].Split('\"').ToList();
          //addressTxt.text = geocodeList[3];

          yield return new WaitForEndOfFrame();
          //geocodeList.Clear();
          //addressList.Clear();
          Resources.UnloadUnusedAssets();
        }
      }

}
