using UnityEngine;

public class RotateUIup : MonoBehaviour
{
    public float rotationSpeed = 50f;

    // Update is called once per frame
    void Update()
    {
        // Rotate the object around its center
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
    }
}
