using System.Collections;
using UnityEngine;

public class PanelScript : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(PositionPanelCoroutine());

    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    IEnumerator PositionPanelCoroutine()
    {
        yield return new WaitForSecondsRealtime(1.0f);
        Transform cameraTransform = Camera.main.transform;

        Vector3 userPosition = cameraTransform.position;

        // Set quad's position at eye height and slightly in front
        transform.position = new Vector3(
            userPosition.x,
            userPosition.y,
            userPosition.z + 1.0f
        );
    }
}
