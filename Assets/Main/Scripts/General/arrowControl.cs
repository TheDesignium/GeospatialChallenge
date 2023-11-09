using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class arrowControl : MonoBehaviour
{

    public Transform target;
    public Animator ani;

    public float smoothTime = 10F;
    Vector3 velocity = Vector3.zero;
    float speed;
    float mindist;

    bool off;

    void Start()
    {
      float distance = Vector3.Distance(transform.position, target.position);
      //smoothTime = distance * 0.75f;
      speed = distance / 20f;
      mindist = distance / 5f;
    }

    void Update()
    {
      if (target != null)
      {
          Vector3 playerPosition = new Vector3(target.position.x, transform.position.y, target.position.z);
          transform.LookAt(playerPosition);

          //var temp_pos = Vector3.SmoothDamp(transform.position, target.position, ref velocity, smoothTime);
          //transform.position = temp_pos;

          var step =  speed * Time.deltaTime;
          var temp_pos = Vector3.MoveTowards(transform.position, target.position, step);
          transform.position = temp_pos;

          float distance = Vector3.Distance(transform.position, target.position);

          //Debug.Log(distance);

          if(distance < mindist)
          {
            if(off == false)
            {
              off = true;
              //Debug.Log("turn off");
              ani.Play("arrowhide",0,0);
            }
          }
      }
    }

    public void killSelf()
    {
      //Debug.Log("killself");
      Destroy(gameObject);
    }
}
