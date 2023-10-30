using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class texFlip : MonoBehaviour
{

    public float waittime;

    public Material mat;
    public Texture2D tex1;
    public Texture2D tex2;

    bool matone;

    void Start()
    {
        StartCoroutine(textureLoop());
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator textureLoop()
    {
      while(true)
      {
        yield return new WaitForSeconds(waittime);
        if(matone == true)
        {
          matone = false;
          mat.mainTexture = tex1;
        }
        else if(matone == false)
        {
          matone = true;
          mat.mainTexture = tex2;
        }
      }
    }
}
