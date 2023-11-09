using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.ARFoundation.Samples;

using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using SimpleJSON;
using Newtonsoft.Json;

public class VisionAPI : MonoBehaviour {

  public ARCameraManager arCameraManager;
  public puzzleControl puzzle;

	private string url = "https://vision.googleapis.com/v1/images:annotate?key=";
	public string apiKey = "";
	public float captureIntervalSeconds = 5.0f;
	public int requestedWidth = 640;
	public int requestedHeight = 480;
	public FeatureType featureType = FeatureType.FACE_DETECTION;
	public int maxResults = 10;
  public List<string> objectNames = new List<string>();

  public bool captureNow;
  public bool _cleanStart;

  public Material debugMat;

	public Texture2D texture2D;
  public Sprite lastPhoto;
  Texture2D m_CameraTexture;

	Dictionary<string, string> headers;

  byte[] jpg;

	public enum FeatureType {
		TYPE_UNSPECIFIED,
		FACE_DETECTION,
		LANDMARK_DETECTION,
		LOGO_DETECTION,
		LABEL_DETECTION,
		TEXT_DETECTION,
		SAFE_SEARCH_DETECTION,
		IMAGE_PROPERTIES,
		OBJECT_LOCALIZATION
	}

  [System.Serializable]
  public class AnnotateImageRequests {
    public List<AnnotateImageRequest> requests;
  }

  [System.Serializable]
    public class AnnotateImageRequest {
    public Image image;
    public List<Feature> features;
  }

  [System.Serializable]
  public class Image {
    public string content;
  }

  [System.Serializable]
  public class Feature {
    public string type;
    public int maxResults;
  }

  XRCpuImage.Transformation m_Transformation = XRCpuImage.Transformation.MirrorY;

  public string filename = "V1.txt";
  public string path;

	void Start () {
		headers = new Dictionary<string, string>();
		headers.Add("Content-Type", "application/json; charset=UTF-8");

		if (apiKey == null|| apiKey == "")
    {
      Debug.LogError("No API key. Please set your API key into the \"Web Cam Texture To Cloud Vision(Script)\" component.");
    }

      arCameraManager.frameReceived += OnCameraFrameReceived;

      path = BaseDir() + Separator() + filename;

      if(_cleanStart == true)
      {
        if(File.Exists(path))
        {
          File.Delete(path);
        }
      }
	}

	// Update is called once per frame
	void Update ()
	{
		if(Input.GetKeyUp(KeyCode.L))
		{
			requestVision();
		}
    if(Input.GetKeyUp(KeyCode.K))
    {
      StartCoroutine("Capture");
    }
	}

  private void OnCameraFrameReceived(ARCameraFrameEventArgs eventArgs)
  {
    if(captureNow == true)
    {
      UpdateCameraImage();
      captureNow = false;
    }

  }

  unsafe void UpdateCameraImage()
  {
      //do some screen flash or something
      puzzle.cameraUI.SetActive(false);
      puzzle.botImage.sprite = puzzle.thinkingSprite;
      puzzle.audio.PlayOneShot(puzzle.clip,1f);

      if (!arCameraManager.TryAcquireLatestCpuImage(out XRCpuImage image))
      {
          return;
      }
      // Get the camera texture from the ARCameraFrameEventArgs
      var format = TextureFormat.RGBA32;

      if (m_CameraTexture == null || m_CameraTexture.width != image.width || m_CameraTexture.height != image.height)
      {
          m_CameraTexture = new Texture2D(image.width, image.height, format, false);
      }

      // Convert the image to format, flipping the image across the Y axis.
      // We can also get a sub rectangle, but we'll get the full image here.
      var conversionParams = new XRCpuImage.ConversionParams(image, format, m_Transformation);

      // Texture2D allows us write directly to the raw texture data
      // This allows us to do the conversion in-place without making any copies.
      var rawTextureData = m_CameraTexture.GetRawTextureData<byte>();
      try
      {
          image.Convert(conversionParams, new IntPtr(rawTextureData.GetUnsafePtr()), rawTextureData.Length);
      }
      finally
      {
          // We must dispose of the XRCpuImage after we're finished
          // with it to avoid leaking native resources.
          image.Dispose();
      }

      // Apply the updated texture data to our texture
      m_CameraTexture.Apply();

      texture2D = RotateTexture(m_CameraTexture, -90);

      debugMat.mainTexture = texture2D;

      StartCoroutine("Capture");
  }

	public void requestVision()
	{
		captureNow = true;
	}

	private IEnumerator Capture()
  {

			if (this.apiKey == null)
				yield return null;

      yield return new WaitForEndOfFrame();

      //texture2D = thetexture;

      yield return new WaitForEndOfFrame();

			jpg = texture2D.EncodeToJPG();
			string base64 = System.Convert.ToBase64String(jpg);

			AnnotateImageRequests requests = new AnnotateImageRequests();
			requests.requests = new List<AnnotateImageRequest>();

			AnnotateImageRequest request = new AnnotateImageRequest();
			request.image = new Image();
			request.image.content = base64;
			request.features = new List<Feature>();

			Feature feature = new Feature();
			feature.type = this.featureType.ToString();
			feature.maxResults = this.maxResults;

			request.features.Add(feature);

			requests.requests.Add(request);

			string jsonData = JsonUtility.ToJson(requests, false);
			if (jsonData != string.Empty) {
				string url = this.url + this.apiKey;
				byte[] postData = System.Text.Encoding.Default.GetBytes(jsonData);
				using(WWW www = new WWW(url, postData, headers)) {
					yield return www;
					if (string.IsNullOrEmpty(www.error))
					{
						Debug.Log(www.text.Replace("\n", "").Replace(" ", ""));
						parseJsonNow(www.text);

					} else {
						Debug.Log("Error: " + www.error);
					}
				}
			}
	}

	void parseJsonNow(string json)
	{
      objectNames.Clear();
			Debug.Log(json);

      JSONNode jsonData = JSON.Parse(json);

      JSONNode responses = jsonData["responses"];

      if (responses != null && responses.Count > 0)
      {
          // Access the "localizedObjectAnnotations" array
          JSONNode annotations = responses[0]["localizedObjectAnnotations"];

          foreach (JSONNode annotation in annotations)
          {
              string objectName = annotation["name"];
              objectNames.Add(objectName);
          }

          // Print the object names (optional)
          foreach (string name in objectNames)
          {
              Debug.Log("Object Name: " + name);
          }
          if(objectNames.Count == 1 && objectNames[0] == "")
          {
            puzzle.noResults();
          }
          else
          {
            puzzle.haveResults(objectNames);
          }
      }
      else
      {
          Debug.LogError("No 'localizedObjectAnnotations' array found in JSON.");
          puzzle.noResults();
      }
  }

  public void saveImage(string imgname)
  {
    string savePath = Path.Combine(Application.persistentDataPath, imgname + ".jpg");

    File.WriteAllBytes(savePath, jpg);

    Debug.Log("Texture saved to: " + savePath);

    string commaadd = imgname + ",";

    WriteToFile(commaadd, path);
  }

  public Sprite makeSuccessSprite()
  {
    Sprite sprite = Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), Vector2.one * 0.5f);
    return sprite;
  }

  public Texture2D ToTexture2D(Texture sourceTexture)
  {
    if (sourceTexture != null)
    {
        // Create a new Texture2D with the same dimensions as the source texture
        Texture2D texture2D = new Texture2D(sourceTexture.width, sourceTexture.height);
        RenderTexture renderTexture = RenderTexture.GetTemporary(sourceTexture.width, sourceTexture.height);
        Graphics.Blit(sourceTexture, renderTexture);
        RenderTexture.active = renderTexture;
        texture2D.ReadPixels(new Rect(0, 0, sourceTexture.width, sourceTexture.height), 0, 0);
        texture2D.Apply();
        RenderTexture.active = null;
        RenderTexture.ReleaseTemporary(renderTexture);
        Debug.Log("Texture conversion successful.");

        return texture2D;
    }
    else
    {
        Debug.LogError("Source texture is not assigned.");
        return null;
    }
  }


  private Texture2D RotateTexture(Texture2D originalTexture, int angle)
  {
      int width = originalTexture.width;
      int height = originalTexture.height;

      // Create a new texture with swapped dimensions if the angle is 90 or 270 degrees
      Texture2D rotatedTexture = new Texture2D(angle % 180 == 0 ? width : height, angle % 180 == 0 ? height : width);

      for (int x = 0; x < rotatedTexture.width; x++)
      {
          for (int y = 0; y < rotatedTexture.height; y++)
          {
              Color pixel = originalTexture.GetPixel(
                  angle % 180 == 0 ? x : width - y - 1,
                  angle % 180 == 0 ? height - y - 1 : x
              );

              rotatedTexture.SetPixel(x, y, pixel);
          }
      }

      rotatedTexture.Apply(); // Apply changes to the rotatedTexture

      return rotatedTexture;
  }
	/////////////////////////////////////////////////

	public string cleanString(string s)
    {
      var stripped = Regex.Replace(s, "[^0-9.]", "");
      return stripped;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the ARCameraManager's FrameReceived event
        arCameraManager.frameReceived -= OnCameraFrameReceived;
    }

    public void WriteToFile(string strings, string thepath)
    {
        string _s = thepath;
        string _string = strings;
        StreamWriter sw = new StreamWriter(_s,true);
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
    /*

   ,animal, dog, cat, bird, fish, insect, reptile, amphibian,
   ,vehicle ,car, truck, bus, train, airplane, boat,
   ,food, pizza, hamburger, hotdog, ice cream, cake, fruit, vegetable,
   ,furniture, bed, table, chair, sofa, desk, lamp,
   ,clothing, shirt, pants, dress, shoes, hat, coat,
   ,electronics, phone, laptop, tablet, TV, camera,
   ,plant, tree, flower, bush, grass,
   ,other, person, bicycle, motorcycle, skateboard,
   surfboard, ball, cup, bottle, book, chair, table, building, street,
   city, beach, forest, mountain, river, lake, ocean,

    */

}

[System.Serializable]
public class Response
{
    public List<LocalizedObjectAnnotation> localizedObjectAnnotations;
}

[System.Serializable]
public class LocalizedObjectAnnotation
{
    public string name;
}
