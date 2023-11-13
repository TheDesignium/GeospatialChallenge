using UnityEngine;

public class FPSCounter : MonoBehaviour
{
    private float deltaTime = 0.0f;
    private GUIStyle style = new GUIStyle();

    private void Awake()
    {
        // Define the style for the FPS counter
        style.alignment = TextAnchor.MiddleCenter;
        style.fontSize = 48;
        style.normal.textColor = Color.white;
    }

    private void Update()
    {
        // Calculate deltaTime
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
    }

    private void OnGUI()
    {
        // Calculate frames per second
        float fps = 1.0f / deltaTime;

        // Display FPS counter on the screen
        GUI.Label(new Rect(10, 10, 200, 100), "FPS: " + Mathf.Ceil(fps).ToString(), style);
    }
}
