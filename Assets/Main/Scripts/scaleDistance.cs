using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

public class scaleDistance : MonoBehaviour
{

  public TMP_Text thetext;
  public float sizeOnScreen;
  public Camera camera;

    void Start()
    {
      if(camera == null)
      {
        camera = Camera.main;
      }
    }

    void Update () {
        Vector3 a = camera.WorldToScreenPoint(transform.position);
        Vector3 b = new Vector3(a.x, a.y + sizeOnScreen, a.z);

        Vector3 aa = camera.ScreenToWorldPoint(a);
        Vector3 bb = camera.ScreenToWorldPoint(b);

        transform.localScale = Vector3.one * (aa - bb).magnitude;

        float total_dist = Vector3.Distance(camera.transform.position,transform.position);

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
