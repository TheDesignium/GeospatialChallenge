using UnityEngine;

public class RotateAndFadeOut : MonoBehaviour
{
    public Transform target;
    private Material material;
    private float startTime;
    private float fadeDuration = 5.0f;
    private float rotationSpeed = 30.0f; // Adjust the rotation speed as needed

    private void Awake()
    {
        // Set the object's Z rotation to a random value
        target.rotation = Quaternion.Euler(0f, 0f, Random.Range(0f, 360f));
        material = target.gameObject.GetComponent<Renderer>().material;
    }

    private void Start()
    {
        startTime = Time.time;
    }

    private void Update()
    {
        // Rotate the object around its own Z-axis
        // target.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);

        // Calculate the time elapsed since Start
        float elapsedTime = Time.time - startTime;

        // If 10 seconds have passed, start fading out the material
        if (elapsedTime >= 10.0f)
        {
            // Calculate the normalized fade progress from 0 to 1
            float fadeProgress = Mathf.Clamp01((elapsedTime - 10.0f) / fadeDuration);

            // Reduce the alpha value of the material gradually
            Color color = material.color;
            color.a = 1.0f - fadeProgress;
            material.color = color;

            // If the fade is complete, you can destroy the object
            if (fadeProgress >= 1.0f)
            {
                Destroy(gameObject);
            }
        }
    }
}
