using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class scaleArrows : MonoBehaviour
{

    public Transform thePlane;

    lookAt look;
    Vector3 scale;

    public float scaleRate;

    public bool simpleStart;

    void OnEnable()
    {
      float z = 0;
      scale = thePlane.transform.localScale;
      scale.z = z;
      thePlane.transform.localScale = scale;
      look = gameObject.GetComponent<lookAt>();
      StopAllCoroutines();
      StartCoroutine(scaleNow());
    }

    IEnumerator scaleNow()
  	{
  		yield return new WaitForSeconds(2f);

  		if(look.thePlayer == null)
  		{
  			look.gameObject.SetActive(false);
  			yield break;
  		}
  		float dist = Vector3.Distance(transform.position, look.thePlayer.transform.position);
  		float z = 0;
      scale = thePlane.transform.localScale;
      scale.z = z;
      thePlane.transform.localScale = scale;
      yield return new WaitForEndOfFrame();
      thePlane.gameObject.SetActive(true);

      if(simpleStart == false)
      {
        while(z < dist)
        {
          z += scaleRate;
          scale.z = z;
          thePlane.transform.localScale = scale;
          yield return new WaitForEndOfFrame();
        }
      }
      else
      {
        scale.z = dist;
        thePlane.transform.localScale = scale;
      }
  	}
}
