using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Google.OpenLocationCode;

public class plusCodeController : MonoBehaviour
{

    public string debugstring;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
      if(Input.GetKeyUp(KeyCode.Q))
      {
        OpenLocationCode encodedCode = new OpenLocationCode(35.62258854475747, 139.7198324407011);
        string encodedCodeStr = encodedCode.Code;
      }
    }

    void getLatLon(string s)
    {
      CodeArea codeArea = OpenLocationCode.Decode(s);

      GeoPoint areaCenter = codeArea.Center;

      Debug.Log(areaCenter.Latitude.ToString() + ":" + areaCenter.Longitude.ToString());
    }

    void createObject()
    {
        // From a valid full code
        OpenLocationCode code = new OpenLocationCode("7JVW52GR+2V");

        // From a valid short code
        OpenLocationCode.ShortCode shortCode = new OpenLocationCode.ShortCode("52GR+2V");

        // From coordinates encoded with default normal precision (~ 14x14 meters)
        OpenLocationCode encodedCode = new OpenLocationCode(27.175063, 78.042188);
        string encodedCodeStr = encodedCode.Code; // "7JVW52GR+2V"

        // From coordinates encoded with extra precision (~ 2x3 meters)
        OpenLocationCode encodedCode2 = new OpenLocationCode(27.175063, 78.042188, 11);
        string encodedCodeStr2 = encodedCode2.Code; // "7JVW52GR+2VG"

        // From coordinates encoded with low precision (~ 5560x5560 meters)
        OpenLocationCode encodedCode3 = new OpenLocationCode(27.175063, 78.042188, codeLength: 6);
        string encodedCodeStr3 = encodedCode3.Code; // "7JVW5200+"
        encodedCode3.IsPadded(); // true
    }

    void decodeObject()
    {
      OpenLocationCode code = new OpenLocationCode("7JVW52GR+2V");

      CodeArea codeArea = code.Decode();

      GeoPoint areaMin = codeArea.Min;
      GeoPoint areaMax = codeArea.Max;
      GeoPoint areaCenter = codeArea.Center;
      // Alternative properties
      double areaMinLat = codeArea.SouthLatitude; // codeArea.Min.Latitude
      double areaMaxLng = codeArea.EastLongitude; // codeArea.Max.Longitude
      // Check point containment
      bool areaContainsPoint = codeArea.Contains(areaCenter);
    }

    void shortCode()
    {
      OpenLocationCode code = new OpenLocationCode("7JVW52GR+2V");

      OpenLocationCode.ShortCode shortCode = code.Shorten(27.1, 78.0);

      string shortCodeStr = shortCode.Code; // "GR+2V"
    }

    void recoverCode()
    {
      //OpenLocationCode recoveredCode = OpenLocationCode.ShortCode.RecoverNearest(27.1, 78.0);
      //string recoveredCodeStr = recoveredCode.Code; // "7JVW52GR+2V"
    }

    void isCodeValid()
    {
      OpenLocationCode.IsValid("7JVW52GR+2V"); // true
      OpenLocationCode.IsValid("GR+2V"); // true
      OpenLocationCode.IsValid("12345678+"); // false

      OpenLocationCode.IsFull("7JVW52GR+2V"); // true
      OpenLocationCode.IsFull("GR+2V"); // false
      OpenLocationCode.IsFull("12345678+"); // false

      OpenLocationCode.IsShort("7JVW52GR+2V"); // false
      OpenLocationCode.IsShort("GR+2V"); // true
      OpenLocationCode.IsShort("12345678+"); // false
    }

    void altCommands()
    {
      string code = OpenLocationCode.Encode(27.175063, 78.042188);

      CodeArea codeArea = OpenLocationCode.Decode("7JVW52GR+2V");

      GeoPoint areaCenter = codeArea.Center;

      OpenLocationCode.ShortCode shortCode = OpenLocationCode.Shorten("7JVW52GR+2V", 27.1, 78.0);

      OpenLocationCode recoveredCode = OpenLocationCode.ShortCode.RecoverNearest("52GR+2V", 27.1, 78.0);
    }
}
