using UnityEngine;

using System.Collections;
using System.Collections.Generic;

public class RotateUI : MonoBehaviour
{
    public float rotationSpeed = 50f;
    public bool randomChanges;
    public bool randomspeed;
    public bool randomscale;
    public float lerpSpeed = 1.0f;
    public Vector3 targetScale;
    public float minScale;
    public float maxScale;

    void Start()
    {
      targetScale = transform.localScale;
      if(randomspeed == true)
      {
        rotationSpeed = UnityEngine.Random.Range(rotationSpeed * 0.8f, rotationSpeed * 1.2f);
      }
      if(randomChanges == true)
      {
        StartCoroutine(randomLoop());
      }
    }

    void Update()
    {
        // Rotate the object around its center
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);

        if(randomscale == true)
        {
          transform.localScale = Vector3.Lerp(transform.localScale, targetScale, lerpSpeed * Time.deltaTime);
        }
    }

    IEnumerator randomLoop()
    {
      while(true)
      {
        rotationSpeed = UnityEngine.Random.Range(-20f, 20f);
        float randomtime = Random.Range(1f,20f);
        if(randomscale == true)
        {
          float rando = UnityEngine.Random.Range(minScale, maxScale);
          targetScale = new Vector3(rando,rando,rando);
        }
        yield return new WaitForSeconds(randomtime/2);
        if(randomscale == true)
        {
          float rando = UnityEngine.Random.Range(minScale, maxScale);
          targetScale = new Vector3(rando,rando,rando);
        }
        yield return new WaitForSeconds(randomtime/2);
      }
    }
}
