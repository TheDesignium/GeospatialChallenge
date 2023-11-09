using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setPoints : MonoBehaviour
{
	
	public GameObject point;
    
	lookAt look;
	
	public float div;

    void Start()
    {
		look = gameObject.GetComponent<lookAt>();
        StartCoroutine(placePoints());
    }

    void Update()
    {
        
    }
	
	IEnumerator placePoints()
	{
		yield return new WaitForSeconds(2f);
		if(look.thePlayer == null)
		{
			look.gameObject.SetActive(false);
			yield break;
		}
		float dist = Vector3.Distance(transform.position, look.thePlayer.transform.position);
		float steps = dist/div;
		steps = steps - 3;
		//Debug.Log(dist + ":" + steps);
		Vector3 localposition = new Vector3(0,0,div * 2);
		for(int i=0; i<steps; i++)
		{
			if(localposition.z < dist)
			{
				var apoint = Instantiate(point, transform.position, transform.rotation);
				apoint.transform.parent = transform;
				apoint.transform.localPosition = localposition;
				localposition.z += div;
				yield return new WaitForSeconds(0.1f);
			}
			else 
			{
				yield break;
			}
		}
	}
}
