using UnityEngine;
using UnityEngine.UI;

public class LerpImagePosition : MonoBehaviour
{
    public Image imageToMove;
    public Vector2 minPosition;
    public Vector2 maxPosition;
    public float lerpSpeed = 1.0f;
    public float minLerpDuration = 1.0f;
    public float maxLerpDuration = 3.0f;
    public float minRotationSpeed = 30.0f;
    public float maxRotationSpeed = 90.0f;

    private RectTransform imageRectTransform;
    private Vector2 targetPosition;
    private float lerpStartTime;
    private float lerpDuration;
    private Quaternion targetRotation;
    private float rotationSpeed;

    public float distancecheck;

    public float randomrotmin;
    public float randomrotmax;

    private void Start()
    {
        imageRectTransform = imageToMove.GetComponent<RectTransform>();
        SetNewTargetPosition();
    }

    private void Update()
    {
        // Move the image
        imageRectTransform.anchoredPosition = Vector2.Lerp(imageRectTransform.anchoredPosition, targetPosition, lerpSpeed * Time.deltaTime);

        // Rotate the image
        imageRectTransform.localRotation = Quaternion.RotateTowards(imageRectTransform.localRotation, targetRotation, rotationSpeed * Time.deltaTime);

        var distance = Vector2.Distance(imageToMove.transform.localPosition, targetPosition);
        if (distance < distancecheck)
        {
            // Set a new target position and lerp duration
            SetNewTargetPosition();
        }
    }

    private void SetNewTargetPosition()
    {
        // Generate a random target position within the specified bounds
        targetPosition = new Vector2(Random.Range(minPosition.x, maxPosition.x), Random.Range(minPosition.y, maxPosition.y));

        // Set a new random lerp duration
        lerpDuration = Random.Range(minLerpDuration, maxLerpDuration);

        // Record the start time of the lerp
        lerpStartTime = Time.time;

        // Generate a random target rotation
        targetRotation = Quaternion.Euler(0, 0, Random.Range(randomrotmin, randomrotmax));

        // Set a new random rotation speed
        rotationSpeed = Random.Range(minRotationSpeed, maxRotationSpeed);
    }
}
