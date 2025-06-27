using UnityEngine;

public class PanelScript : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Update()
    {
        Transform cameraTransform = Camera.main.transform;

        if (cameraTransform == null)
        {
            Debug.LogWarning("Main Camera not found.");
            return;
        }

        Vector3 userPosition = cameraTransform.position;

        // Set quad's position at eye height and slightly in front
        transform.position = new Vector3(
            userPosition.x,
            userPosition.y,
            userPosition.z + 1.0f
        );
    }
}
