using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class smoothOut : MonoBehaviour {

	public Transform target;

	public float smoothTime = 0.3F;
	public float rotspeed = 0.2f;

	float moveLimit = 0.01f;
	float journeyLength;
	float startTime;
	float startTimeR;

	Vector3 velocity = Vector3.zero;
	Vector3 velocity2 = Vector3.zero;
	Vector3 startpoint;
	Vector3 lookpoint;

	Vector3 scalev3;

	Quaternion startrotation;
	Quaternion targetrotation;

	public bool _active;

	void Start ()
	{

	}

	void Update () {

		if(_active == true)
		{
			journeyLength = Vector3.Distance(transform.position, target.position);

			targetrotation = target.rotation;
			//targetrotation.x = 0;
			//targetrotation.z = 0;

			if (journeyLength > moveLimit)
			{
				var temp_pos = Vector3.SmoothDamp(transform.position, target.position, ref velocity, smoothTime);
				transform.position = temp_pos;
			}


			if (targetrotation != startrotation)
			{
					float timeProgressed = (Time.time - startTimeR) / rotspeed;
					transform.rotation = Quaternion.Slerp(startrotation, targetrotation, timeProgressed);
			}

			startTimeR = Time.time;
			startrotation = transform.rotation;
			startpoint = transform.position;
		}
	}
}
