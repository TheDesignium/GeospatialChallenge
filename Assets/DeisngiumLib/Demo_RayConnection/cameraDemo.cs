using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MAOUtil.ARUtil
{
  public class cameraDemo : MonoBehaviour
  {
      public Transform cameraTransform = null;

      private float cameraTransformSensitivity = 360;
      private float translateSpeed = 50;
      private float climbSpeed = 4;
      private float normalMoveSpeed = 10;
      private float slowMoveFactor = 0.25f;
      private float fastMoveFactor = 3;

      private float rotationX = 0.0f;
      private float rotationY = 0.0f;

      private bool HoldingButtonDown = false;

      void Start()
      {
        if (cameraTransform == null)
          cameraTransform = Camera.main.transform;
      }

#if UNITY_EDITOR
      void Update()
      {
          cameraTransformControl();
      }
#endif

      private void cameraTransformControl()
      {
          float zoom = Input.GetAxis("Mouse ScrollWheel");
          if (zoom != 0)
          {
              cameraTransform.Translate(cameraTransform.transform.forward * 5 * zoom, Space.World);
          }

          if (Input.GetMouseButton(2))
          {
              cameraTransform.Translate(-Input.GetAxis("Mouse X") * translateSpeed * Time.deltaTime, -Input.GetAxis("Mouse Y") * translateSpeed * Time.deltaTime, 0, Space.Self);
          }

          if (Input.GetMouseButton(1))
          {
              rotationX += Input.GetAxis("Mouse X") * cameraTransformSensitivity * Time.deltaTime;
              rotationY += Input.GetAxis("Mouse Y") * cameraTransformSensitivity * Time.deltaTime;
              rotationY = Mathf.Clamp(rotationY, -90, 90);

              cameraTransform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
              cameraTransform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);

              float speed = normalMoveSpeed;
              if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift)) speed = normalMoveSpeed * fastMoveFactor;
              if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl)) speed = normalMoveSpeed * slowMoveFactor;
              cameraTransform.position += cameraTransform.forward * speed * Input.GetAxis("Vertical") * Time.deltaTime;
              cameraTransform.position += cameraTransform.right * speed * Input.GetAxis("Horizontal") * Time.deltaTime;

              if (Input.GetKey(KeyCode.Q)) { cameraTransform.position += cameraTransform.up * climbSpeed * Time.deltaTime; }
              if (Input.GetKey(KeyCode.E)) { cameraTransform.position -= cameraTransform.up * climbSpeed * Time.deltaTime; }
          }
      }
  }

}
