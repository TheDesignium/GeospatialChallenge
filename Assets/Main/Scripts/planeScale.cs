using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class planeScale : MonoBehaviour
{

    public Transform pointA;
    public Transform pointB;
    public Transform pointC;
    public Transform pointD;

    public float div;

    Vector3 scale = new Vector3(0,0,0);

    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = (pointA.position + pointC.position)/2;
        scale.x = (Vector3.Distance(pointA.position,pointB.position))/div;
        scale.y = 1f;
        scale.z = (Vector3.Distance(pointA.position,pointD.position))/div;
        transform.localScale = scale;
        var look = (pointA.position + pointB.position)/2;
        look.y = transform.position.y;
        transform.LookAt(look);
    }
}
