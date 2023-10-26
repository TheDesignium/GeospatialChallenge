using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class minimapControl : MonoBehaviour
{

    public GoogleMapsDemo gmd;
    public Transform targetTransform; // The transform to use for rotation data
    public GameObject map;

    public float minPosition;
    public float maxPosition;
    float xRotation;

    public Image imgA;

    bool isfading;
    bool isenabled;

    public float fadespeed;

    void Start()
    {

    }

    void FixedUpdate()
    {
        xRotation = targetTransform.rotation.eulerAngles.x;

        if (xRotation < maxPosition && xRotation > minPosition)
        {
          if(isfading == false && isenabled == false)
          {
            StartCoroutine(enableMap());
          }
        }
        else
        {
          if(isfading == false && isenabled == true)
          {
            StartCoroutine(disableMap());
          }
        }
    }

    public void mapOn()
    {
      StartCoroutine(enableMap());
    }
    public void mapOff()
    {
      StartCoroutine(disableMap());
    }

    IEnumerator enableMap()
    {
      isfading = true;
      var bg = Color.black;
      bg.a = 0;
      imgA.color = bg;
      imgA.gameObject.SetActive(true);
      float alph = bg.a;
      yield return new WaitForEndOfFrame();
      while(alph < 1)
      {
        alph = bg.a;
        alph += fadespeed;
        bg.a = alph;
        imgA.color = bg;
        yield return new WaitForEndOfFrame();
      }
      yield return new WaitForEndOfFrame();
      map.SetActive(true);
      yield return new WaitForEndOfFrame();
      while(alph > 0)
      {
        alph = bg.a;
        alph -= fadespeed;
        bg.a = alph;
        imgA.color = bg;
        yield return new WaitForEndOfFrame();
      }
      yield return new WaitForEndOfFrame();
      isenabled = true;
      isfading = false;
      gmd.OnSetMapVisible();
      imgA.gameObject.SetActive(false);
    }

    IEnumerator disableMap()
    {
      gmd.screenShot();
      isfading = true;
      var bg = Color.black;
      bg.a = 0;
      imgA.color = bg;
      float alph = bg.a;
      imgA.gameObject.SetActive(true);
      yield return new WaitForEndOfFrame();
      while(alph < 1)
      {
        alph = bg.a;
        alph += fadespeed;
        bg.a = alph;
        imgA.color = bg;
        yield return new WaitForEndOfFrame();
      }
      yield return new WaitForEndOfFrame();
      //gmd.OnSetMapInvisible();
      map.SetActive(false);
      yield return new WaitForEndOfFrame();
      while(alph > 0)
      {
        alph = bg.a;
        alph -= fadespeed;
        bg.a = alph;
        imgA.color = bg;
        yield return new WaitForEndOfFrame();
      }
      yield return new WaitForEndOfFrame();
      isenabled = false;
      isfading = false;
      imgA.gameObject.SetActive(false);
    }
}
