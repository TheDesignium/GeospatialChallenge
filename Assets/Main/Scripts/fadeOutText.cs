using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using TMPro;

public class fadeOutText : MonoBehaviour
{

  public float maxDistance = 10f; // Adjust this value as needed
  private Camera mainCamera;
  public TMP_Text fadeText;
  public Color startColor;

    void Start()
    {
      mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
      if (mainCamera != null)
      {
        float distanceToCamera = Vector3.Distance(mainCamera.transform.position, transform.position);
        float alpha = 1f - Mathf.Clamp01(distanceToCamera / maxDistance);

        // Apply the faded color to the object's material
        Color fadedColor = new Color(startColor.r, startColor.g, startColor.b, alpha);
        fadeText.color = fadedColor;
      }
    }
}
