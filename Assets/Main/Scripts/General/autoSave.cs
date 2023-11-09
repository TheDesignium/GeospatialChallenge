using System;
using System.IO;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

public class autoSave : MonoBehaviour
{

    public TMP_Text geospatial;
    public TMP_Text elevation;
    public TMP_Text Geoid;
    public TMP_Text Opo;

    public string filename = "L1.txt";
    public string path;

    int count;

    void Start()
    {
      path = BaseDir() + Separator() + filename;
    }

    public void saveData()
    {
      StartCoroutine(delaySave());
    }

    IEnumerator delaySave()
    {
      yield return new WaitForSeconds(1);
      Debug.Log("take screenshot");
      ScreenCapture.CaptureScreenshot(count.ToString() + ".png");
      yield return new WaitForEndOfFrame();
      string s = count.ToString() + "\n";
      s = s + geospatial.text;
      s = s + "\n";
      s = s + elevation.text;
      s = s + "\n";
      s = s + Geoid.text;
      s = s + "\n";
      s = s + Opo.text;
      s = s + "\n\n\n";
      count += 1;
      Debug.Log(path);
      WriteToFile(s);
    }

    public void WriteToFile(string _strings)
    {
        string _s = path;
        string _string = _strings;
        StreamWriter sw = new StreamWriter(_s, true);
        sw.WriteLine(_string);
        sw.Close();
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
}
