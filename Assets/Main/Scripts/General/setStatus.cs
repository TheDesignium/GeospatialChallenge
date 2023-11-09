using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class setStatus : MonoBehaviour
{

    public GameObject standard;
    public GameObject visited;

    public void setCubeStatus(bool b)
    {
      if(b == true)
      {
        standard.SetActive(false);
        visited.SetActive(true);
      }
      else
      {
        standard.SetActive(true);
        visited.SetActive(false);
      }
    }
}
