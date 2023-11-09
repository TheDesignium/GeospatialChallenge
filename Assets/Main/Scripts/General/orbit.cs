using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class orbit : MonoBehaviour
{

    public Vector3 dir;
    public float speed;
    public bool randomspeed;

    void Start()
    {
      if(randomspeed == true)
      {
        speed = UnityEngine.Random.Range(speed * 0.8f, speed * 1.2f);
      }
    }

    void FixedUpdate()
    {
        transform.Rotate(dir * speed);
    }
}
