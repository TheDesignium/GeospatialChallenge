using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Networking;

using TMPro;

public class positionInformation : MonoBehaviour
{
    public TMP_Text _nametext;
    public TMP_Text _ratingtext;

  	public Shader shader;
  	public Shader shaderphoto;

  	public Renderer rend;
  	public Renderer rendphoto;

  	public Texture2D mask;
    public Texture2D[] randomImg;

    public void setUp(string name, string rating, string icon, string photo)
    {
      _nametext.text = name;
      _ratingtext.text = rating;
	  StartCoroutine(LoadTextureFromWeb(icon, photo));
    }

	IEnumerator LoadTextureFromWeb(string url, string photo)
    {
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
        {
            Debug.LogError("Error: " + www.error);
        }
        else
        {
            Texture2D loadedTexture = DownloadHandlerTexture.GetContent(www);
            Material m = new Material(shader);
      			m.SetTexture("_MainTex", loadedTexture);
      			rend.material = m;
        }

		yield return new WaitForEndOfFrame();

		string photourl = "https://maps.googleapis.com/maps/api/place/photo?maxwidth=1200&photo_reference=" + photo + "&key=AIzaSyCLoCRs3w-AGrm7_AwCjrCq9W9gNQLOC6c";

		  UnityWebRequest wwww = UnityWebRequestTexture.GetTexture(photourl);
        yield return wwww.SendWebRequest();

        if (wwww.isNetworkError || wwww.isHttpError)
        {
            Debug.LogError("Error: " + wwww.error);
            Material m = new Material(shaderphoto);
            m.SetTexture("_MainTex", randomImg[Random.Range(0,6)]);
            rendphoto.material = m;
        }
        else
        {
            Texture2D loadedTexture = DownloadHandlerTexture.GetContent(wwww);
            Material m = new Material(shaderphoto);
      			m.SetTexture("_MainTex", loadedTexture);
      			m.SetTexture("_MaskTex", mask);
      			rendphoto.material = m;

      			float _divide = 6;

      			if(loadedTexture.width > loadedTexture.height)
      			{
      				float _debug = (float)loadedTexture.width/(float)loadedTexture.height;
      				//float _debug = (float)loadedTexture.height/(float)loadedTexture.width;
      				//rendphoto.transform.localScale = new Vector3(_divide, _debug * _divide, 1);
      				rendphoto.transform.localScale = new Vector3(_debug * _divide, _divide, 1);
      			}
      			else
      			{
      				//float _debug = (float)loadedTexture.width/(float)loadedTexture.height;
      				//rendphoto.transform.localScale = new Vector3(_debug * _divide, _divide, 1);
      				float _debug = (float)loadedTexture.height/(float)loadedTexture.width;
      				rendphoto.transform.localScale = new Vector3(_divide, _debug * _divide, 1);
      			}
        }
    }
}
