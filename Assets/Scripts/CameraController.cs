using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Rigidbody rb;
    private Camera mainCamera;

    // Attach this script to the object the main camera should focus on
    [SerializeField]
    private float cameraSpeed = 120.0f;
    [SerializeField]
    private float cameraDistance = 40.0f;
    [SerializeField]
    private float cameraDeadAngle = 30.0f;
    [SerializeField]
    private float cameraDriftSpeed = 0.2f;
    [SerializeField]
    private bool invertCamera = true;

    void Start() {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        float dt = Time.deltaTime;
        float horizontal = Input.GetAxis("HorizontalLook");
        float vertical = Input.GetAxis("VerticalLook");
        float roll = Input.GetAxis("RollLook");
        
        if (invertCamera) {
            vertical = -vertical;
        }

        mainCamera.transform.Rotate(new Vector3(0,0,1), roll * cameraSpeed * dt);
        mainCamera.transform.Rotate(new Vector3(0,1,0), horizontal * cameraSpeed * dt);
        mainCamera.transform.Rotate(new Vector3(1,0,0), vertical * cameraSpeed * dt);

        float angleBetween = Vector3.SignedAngle(mainCamera.transform.forward, transform.up, Vector3.Cross(transform.up, mainCamera.transform.forward));

        if (Mathf.Abs(angleBetween) > cameraDeadAngle) {
            float angleToMove = angleBetween * Time.deltaTime * cameraDriftSpeed * (rb.velocity.magnitude / 10) * (Mathf.Deg2Rad * (Mathf.Abs(angleBetween) - cameraDeadAngle));
            mainCamera.transform.RotateAround(mainCamera.transform.position, Vector3.Cross(transform.up, mainCamera.transform.forward), angleToMove);
        }

        mainCamera.transform.position = transform.position - (mainCamera.transform.forward * cameraDistance);
    }
}
