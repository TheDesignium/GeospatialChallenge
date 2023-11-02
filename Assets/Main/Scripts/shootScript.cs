using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

using TMPro;

public class shootScript : MonoBehaviour
{
  public GameObject ballPrefab;
  public float radius = 1f;
  public float mass = 1f;
  public float forceMagnitude = 100f;
  public float holdMagnitude = 100f;
  public PhysicMaterial pm;
  public Vector2 vec = new Vector2();
  public bool active;
  public bool touchcharge;

  //public TMP_Text debugtext;

  //public Image img;

  private void Update()
  {
    if(active == true)
    {
      if(Input.GetKeyUp(KeyCode.Q))
      {
        fireBall(vec);
      }


      //if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
      if (Input.touchCount > 0)
      {
        if(touchcharge == false)
        {
          fireBall(Input.GetTouch(0).position);
        }
        else
        {
          holdMagnitude = 0;
          //img.fillAmount = 0;
        }
      }

      if (Input.touchCount > 0)
      {
        if(touchcharge == true)
        {
          if(holdMagnitude < 2000)
          {
            holdMagnitude += 50;
            //if(debugtext != null)
            //{
            //  debugtext.text = holdMagnitude.ToString();
            //}
            //img.fillAmount = holdMagnitude/2000;
          }
        }
        if (Input.GetTouch(0).phase == TouchPhase.Ended)
        {
          if(touchcharge == true)
          {
            fireBall(Input.GetTouch(0).position);
            //img.fillAmount = 0;
          }
        }
      }
    }
  }

  public void setActive(bool b)
  {
    active = b;
  }

  void fireBall(Vector2 v2)
  {
      Ray ray = Camera.main.ScreenPointToRay(v2);
      Vector3 touchPosition = ray.GetPoint(1);
      Vector3 lookPosition = ray.GetPoint(2);
      //Vector3 touchPosition = Camera.main.ScreenToWorldPoint(Input.GetTouch(0).position);
      //touchPosition.z = 1f;

      // Create a new ball game object from the prefab at the touch position
      GameObject ball = Instantiate(ballPrefab, touchPosition, Quaternion.identity);
      ball.transform.LookAt(lookPosition);
      // Set the ball's Rigidbody properties
      Rigidbody rigidbody = ball.GetComponent<Rigidbody>();
      //rigidbody.mass = mass;
      //rigidbody.drag = 0f;
      //rigidbody.useGravity = true;

      // Set the ball's SphereCollider radius
      //SphereCollider collider = ball.GetComponent<SphereCollider>();
      //collider.radius = radius;
      //collider.material = pm;
      // Set the ball's tag to "Ball" (optional)
      //ball.tag = "Ball";

      if(touchcharge == false)
      {
        rigidbody.AddForce(ball.transform.forward * forceMagnitude);
      }
      else
      {
        rigidbody.AddForce(ball.transform.forward * holdMagnitude);
      }
  }
}
