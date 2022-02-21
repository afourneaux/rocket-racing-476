using UnityEngine;

public class CameraController : MonoBehaviour
{
    // Attach this script to the object the main camera should focus on
    public float cameraSpeed;
    public float cameraDistance;

    void LateUpdate()
    {
        float dt = Time.deltaTime;
        float horizontal = Input.GetAxis("HorizontalLook");
        float vertical = Input.GetAxis("VerticalLook");

        Camera.main.transform.Rotate(new Vector3(0,1,0), horizontal * cameraSpeed * dt);
        Camera.main.transform.Rotate(new Vector3(1,0,0), vertical * cameraSpeed * dt);

        Camera.main.transform.position = transform.position - (Camera.main.transform.forward * cameraDistance);
    }
}
