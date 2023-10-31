using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

public class dataHolder : MonoBehaviour
{

    public string name;
    public TMP_Text thetext;

    void Awake()
    {
      if(thetext == null)
      {
        Transform childTransform = transform.Find("name");

        // If found, try to get the TMP_Text component
        if (childTransform != null)
        {
             thetext = childTransform.GetComponent<TMP_Text>();
        }
      }
    }

}
