using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

public class getDistance : MonoBehaviour {

	public bool debugging;
	public TMP_Text uitext2;

	List<string> _locationList = new List<string>();


	void Start ()
	{

	}

	void Update () {

	}

		public float parseMeasure(string from, string to)
		{
			_locationList.Clear();
			_locationList = from.Split(',').ToList();
			float frmlat = 0;
			float frmlon = 0;
			float tolat = 0;
			float tolon = 0;
			float.TryParse(_locationList[0], out frmlat);
			float.TryParse(_locationList[1], out frmlon);
			_locationList.Clear();
			_locationList = to.Split(',').ToList();
			float.TryParse(_locationList[0], out tolat);
			float.TryParse(_locationList[1], out tolon);
			float r = CalculateDistance(frmlat,tolat,frmlon,tolon);
			return r;
		}

		public float CalculateDistance(float lat_1, float lat_2, float long_1, float long_2)
		 {
				 int R = 6371;
				 var lat_rad_1 = Mathf.Deg2Rad * lat_1;
				 var lat_rad_2 = Mathf.Deg2Rad * lat_2;
				 var d_lat_rad = Mathf.Deg2Rad * (lat_2 - lat_1);
				 var d_long_rad = Mathf.Deg2Rad * (long_2 - long_1);
				 var a = Mathf.Pow(Mathf.Sin(d_lat_rad / 2), 2) + (Mathf.Pow(Mathf.Sin(d_long_rad / 2), 2) * Mathf.Cos(lat_rad_1) * Mathf.Cos(lat_rad_2));
				 var c = 2 * Mathf.Atan2(Mathf.Sqrt(a), Mathf.Sqrt(1 - a));
				 var total_dist = R * c; // convert to meters

				 var total_distM = total_dist * 1000f;

				 if(debugging == true)
				 {
					 if(total_dist < 1000){
						 uitext2.text = "Distance: " + total_distM.ToString("F1") + "m";
					 }
					 if(total_dist > 1000){
						 uitext2.text = "Distance: " + total_dist.ToString("F3") + "km";
					 }

				 }

				 return total_distM;
		 }

}
