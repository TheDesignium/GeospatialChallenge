using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Google.XR.ARCoreExtensions;
using Google.XR.ARCoreExtensions.Samples.Geospatial;

public class lookAtCompass : MonoBehaviour
{

    public Geo _geo;

    public Transform theTarget;
    public Transform target;
    public Transform compass;
    public GameObject theparent;

    public bool onlylook;
    public bool isActive;
    public bool autoSwitch;

    int targetPoint;

    public float distanceLimit = 2f;
    public float dist;

    RaycastHit hit;

    void Start()
    {
      if(theTarget == null)
      {

      }
    }

    public void disableCompass()
    {
      isActive = true;
      theparent.SetActive(false);
    }

    public void setActiveNow()
    {
      targetPoint = 0;
      isActive = true;
      theparent.SetActive(true);
      theTarget = _geo.objects[0].transform;
    }

    void Update()
    {
      if(Input.touchCount == 1 && autoSwitch == true)
      {
          if (Input.GetTouch(0).phase == TouchPhase.Began)
          {
              Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position);
              if (Physics.Raycast(ray, out hit))
              {
                  Debug.Log(hit.transform.name + ":" + hit.transform.parent.parent.name + ":" + theTarget.name);

                  if(hit.transform.parent.parent == theTarget)
                  {
                    Debug.Log("cancel target");
                    if(_geo.objects.Count - 1 > targetPoint)
                    {
                      targetPoint += 1;
                      theTarget = _geo.objects[targetPoint].transform;
                    }
                  }
              }
          }
      }
    }

    void FixedUpdate()
    {

	if(autoSwitch == true)
	{
        if(_geo.objects.Count > 0 && isActive == true)
        {
          dist = Vector3.Distance(Camera.main.transform.position, theTarget.position);
          if(dist < distanceLimit)
          {
              if(_geo.objects.Count - 1 > targetPoint)
              {
                targetPoint += 1;
                theTarget = _geo.objects[targetPoint].transform;
              }
          }
        }
	}


        if (theTarget != null && isActive == true)
        {
            Vector3 playerPosition = new Vector3(theTarget.position.x, transform.position.y, theTarget.position.z);
            if(onlylook == false)
            {
                target.LookAt(playerPosition);
                Vector3 r = transform.localEulerAngles;
                r.x = 0;
                r.y = target.localEulerAngles.y;
                r.z = 0;
                transform.localEulerAngles = r;
                if(compass != null)
                {
                  compass.localEulerAngles = r;
                }
            }
            else
            {
                transform.LookAt(playerPosition);
            }
        }
    }
}
