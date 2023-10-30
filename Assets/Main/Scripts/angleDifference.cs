using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class angleDifference : MonoBehaviour
{

    public Transform _parent;
    public Transform _child;
    public Transform _debug;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
      float _camheading = RoundAngle2(_parent.localEulerAngles.y);
      float _localheading = RoundAngle2(_child.localEulerAngles.y);
      float _heading = 0;
      _heading = _camheading + _localheading;
       _heading = RoundAngle2(_heading);
       Vector3 v3 = new Vector3(0,_heading,0);
       _debug.localEulerAngles = v3;
    }

    float RoundAngle2(float angle)
    {
        // Make sure that we get value between (-360, 360], we cannot use here module of 180 and call it a day, because we would get wrong values
        angle %= 360;
        if (angle > 180)
        {
            // If we get number above 180 we need to move the value around to get negative between (-180, 0]
            return angle - 360;
        }
        else if (angle < -180)
        {
            // If we get a number below -180 we need to move the value around to get positive between (0, 180]
            return angle + 360;
        }
        else
        {
            // We are between (-180, 180) so we just return the value
            return angle;
        }
    }
}
