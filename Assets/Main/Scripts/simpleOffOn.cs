using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class simpleOffOn : MonoBehaviour
{

  public float taptime;

  public GameObject[] _ui;

  bool _tapped;


    void Start()
    {
      doubleTapped();
    }

    // Update is called once per frame
    void Update()
    {
      if(Input.touchCount > 0)
      {
        Touch touch = Input.GetTouch(0);
        if (touch.phase == TouchPhase.Began)
        {
          if(_tapped == true)
          {
            doubleTapped();
          }
          else if(_tapped == false)
          {
            StopAllCoroutines();
            StartCoroutine(tapTimer());
          }
        }
      }
    }

    public void doubleTapped()
    {
      StopAllCoroutines();
      _tapped = false;
      if(_ui[0].activeSelf == true)
      {
        foreach(GameObject _g in _ui)
        {
          _g.SetActive(false);
        }
      }
      else if(_ui[0].activeSelf == false)
      {
        foreach(GameObject _g in _ui)
        {
          _g.SetActive(true);
        }
      }
    }

    public void offNow()
    {
      StopAllCoroutines();
      if(_ui[0].activeSelf == true)
      {
        foreach(GameObject _g in _ui)
        {
          _g.SetActive(false);
        }
      }
    }

    IEnumerator tapTimer()
    {
      _tapped = true;
      yield return new WaitForSeconds(taptime);
      _tapped = false;
    }
}
