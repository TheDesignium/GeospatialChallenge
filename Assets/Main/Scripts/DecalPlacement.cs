using UnityEngine;

public class DecalPlacement : MonoBehaviour
{
    public GameObject decalPrefab; // The prefab of the decal/plane you want to place
    public float minPlacementInterval = 1.0f; // Minimum time interval between placements
    private float lastPlacementTime;

    void OnCollisionEnter(Collision collision)
    {
        float currentTime = Time.time;

        // Check if enough time has passed since the last placement
        if (currentTime - lastPlacementTime >= minPlacementInterval)
        {
            PlaceDecal(collision.contacts[0].point, collision.contacts[0].normal);
            lastPlacementTime = currentTime; // Update the last placement time
        }
    }

    void PlaceDecal(Vector3 position, Vector3 normal)
    {
        // Instantiate the decalPrefab at the collision point with the proper rotation
        GameObject decal = Instantiate(decalPrefab, position, Quaternion.identity);

        // Rotate the decal to match the hit normal
        decal.transform.forward = -normal;

        // Adjust the scale and position of the decal as needed (e.g., scale it down)
        // decal.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

        // Optionally, you can parent the decal to the collided object
        // decal.transform.parent = collision.transform;
    }
}
