using System;
using System.IO;
using System.Text;
using System.Linq;
﻿using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

using Google.XR.ARCoreExtensions.Samples.Geospatial;

public class writeFile : MonoBehaviour
{

	public GeospatialController geo;

    public Transform _player;
	public Transform replayer;
    public GameObject recordIcon;

    public bool _cleanStart;
    public bool recording;
	public bool useGeoPose;

    public string filename = "L1.txt";
    public string path;

    float exp = 0f;
    float wyp = 0f;
    float zdp = 0f;
	float uwp = 0f;

    float lat = 0f;
    float lon = 0f;
    float alt = 0f;

    Vector3 _return;
	Quaternion _returnQ;

	public float readwritetimer;

	public List<string> tempList = new List<string>();
	public List<string> lineList = new List<string>();
	public List<Vector3> positionList = new List<Vector3>();
	public List<Vector3> rotationList = new List<Vector3>();
    List<string> vectors = new List<string>();

    //https://event-records.s3.ap-northeast-1.amazonaws.com/L1.txt

    void Start()
    {
      path = BaseDir() + Separator() + filename;

      if(_cleanStart == true)
      {
        if(File.Exists(path))
        {
          File.Delete(path);
        }
      }
    }

    void Update()
    {
      if(Input.GetKeyUp(KeyCode.L))
      {

      }
    }

    public void readFile()
    {
      StartCoroutine(delayRead());
    }

    IEnumerator delayRead()
    {
        yield return new WaitForSeconds(0.5f);
        //StartCoroutine(GetImageText());
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

    string ReadFileContent()
    {
        string content = "";
        if(File.Exists(path))
        {
          FileInfo fi = new FileInfo(path);
          try
          {
              using (StreamReader sr = new StreamReader(fi.OpenRead(), Encoding.UTF8))
              {
                  content = sr.ReadToEnd();
              }
          }
          catch (Exception e)
          {
              Debug.LogError(e);
              content = "";
          }
        }
        return content;
    }

    public void WriteToFile(string _strings)
    {
        string _s = path;
        string _string = _strings;
        StreamWriter sw = new StreamWriter(_s);
        sw.WriteLine(_string);
        sw.Close();
    }

    public void UpdateFile(string _strings)
    {
      StartCoroutine(UpdateFileLoop(_strings));
    }

    IEnumerator UpdateFileLoop(string _input)
    {
        yield return new WaitForEndOfFrame();

        string _string = "";
        string _s = path;

        _string = _string + _input;

        if(File.Exists(path))
        {
          _string = "/" + _string;
          StreamWriter sw = new StreamWriter(_s, append: true);
          sw.WriteLine(_string);
          sw.Close();
        }
        else
        {
          StreamWriter sw = new StreamWriter(_s);
          sw.WriteLine(_string);
          sw.Close();
        }

        yield return new WaitForSeconds(1);

        Resources.UnloadUnusedAssets();
    }

    public void autoUpdate()
    {
      readFile();
    }

	//^^^^^^^^^^^^^^^^^^^^^^^^^^^^^

	IEnumerator writeFileLoop()
    {
        yield return new WaitForEndOfFrame();

		while(recording == true)
		{
			string _string = "";

			if(useGeoPose == true)
			{
				_string = geo.returnGeoPose();
				Debug.Log(_string);
			}
			else
			{
				_string = _player.transform.localPosition.ToString("F2") + ":" + _player.transform.localEulerAngles.ToString("F2");
			}
			string _s = path;

			if(File.Exists(path))
			{
			  _string = "/" + _string;
			  StreamWriter sw = new StreamWriter(_s, append: true);
			  sw.WriteLine(_string);
			  sw.Close();
			}
			else
			{
			  StreamWriter sw = new StreamWriter(_s);
			  sw.WriteLine(_string);
			  sw.Close();
			}
			yield return new WaitForSeconds(readwritetimer);
		}

        yield return new WaitForSeconds(1);

        Resources.UnloadUnusedAssets();
    }

	Vector3 getV3(string _s)
    {
      vectors.Clear();
      vectors = _s.Split(',').ToList();
      exp = 0f;
      wyp = 0f;
      zdp = 0f;
      float.TryParse(vectors[0], out exp);
      float.TryParse(vectors[1], out wyp);
      float.TryParse(vectors[2], out zdp);
      _return.x = exp;
      _return.z = zdp;
      _return.y = wyp;
      return _return;
    }

	Quaternion getQ4(string _s)
    {
      vectors.Clear();
      vectors = _s.Split(',').ToList();
      exp = 0f;
      wyp = 0f;
      zdp = 0f;
	  uwp = 0f;
      float.TryParse(vectors[0], out exp);
      float.TryParse(vectors[1], out wyp);
      float.TryParse(vectors[2], out zdp);
	  float.TryParse(vectors[3], out uwp);
      _returnQ.x = exp;
      _returnQ.z = zdp;
      _returnQ.y = wyp;
	  _returnQ.w = uwp;
      return _returnQ;
    }

}
