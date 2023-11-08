using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

public class scaleDistance : MonoBehaviour
{

  public TMP_Text thetext;
  public float sizeOnScreen;
  public Camera thecamera;

    void Start()
    {
      if(thecamera == null)
      {
        thecamera = Camera.main;
      }
    }

    void Update () {
        Vector3 a = thecamera.WorldToScreenPoint(transform.position);
        Vector3 b = new Vector3(a.x, a.y + sizeOnScreen, a.z);

        Vector3 aa = thecamera.ScreenToWorldPoint(a);
        Vector3 bb = thecamera.ScreenToWorldPoint(b);

        transform.localScale = Vector3.one * (aa - bb).magnitude;

        float total_dist = Vector3.Distance(thecamera.transform.position,transform.position);

        if(thetext != null)
        {
          if(total_dist < 1000){
            thetext.text = total_dist.ToString("F0") + "m";
          }
          if(total_dist > 1000){
            thetext.text = (total_dist/1000).ToString("F2") + "km";
          }
        }
    }
}
