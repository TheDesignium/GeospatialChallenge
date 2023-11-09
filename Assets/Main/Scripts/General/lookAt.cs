using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class lookAt : MonoBehaviour
{

  public Transform thePlayer;

	public bool fixY;

	public bool blockFind;

    void Start()
    {
      if(thePlayer == null && blockFind == false)
      {
        thePlayer = Camera.main.transform;
      }
    }

    void Update()
    {
        if (thePlayer != null)
        {
    			if(fixY == true)
    			{
    				Vector3 playerPosition = new Vector3(thePlayer.position.x, transform.position.y, thePlayer.position.z);
    				transform.LookAt(playerPosition);
    			}
    			else
    			{
    				transform.LookAt(thePlayer.position);
    			}
        }
    }
}
