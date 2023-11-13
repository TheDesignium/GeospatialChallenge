using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class maintainScale : MonoBehaviour
{

    public Vector3 targetscale;
    Transform theparent;

    void Awake()
    {
      theparent = transform.parent;
    }

    void LateUpdate()
    {
      if(theparent != null)
      {
        transform.parent = null;
        transform.localScale = targetscale;
        transform.parent = theparent;
      }
    }
}
