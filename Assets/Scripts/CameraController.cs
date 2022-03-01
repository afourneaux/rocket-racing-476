using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Rigidbody rb;

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
    }

    void LateUpdate()
    {
        float dt = Time.deltaTime;
        float horizontal = Input.GetAxis("HorizontalLook");
        float vertical = Input.GetAxis("VerticalLook");
        
        if (invertCamera) {
            vertical = -vertical;
        }

        Camera.main.transform.Rotate(new Vector3(0,1,0), horizontal * cameraSpeed * dt);
        Camera.main.transform.Rotate(new Vector3(1,0,0), vertical * cameraSpeed * dt);

        float angleBetween = Vector3.SignedAngle(Camera.main.transform.forward, transform.up, Vector3.Cross(Camera.main.transform.forward, transform.up));

        if (Mathf.Abs(angleBetween) > cameraDeadAngle) {
            Vector3 cameraForward = Camera.main.transform.forward;
            Vector3 rt = (Vector3.RotateTowards(cameraForward, transform.up, cameraDriftSpeed * (angleBetween - cameraDeadAngle) * (rb.velocity.magnitude / 100) * dt, 0.0f));
            Camera.main.transform.rotation = Quaternion.LookRotation(rt);
        }

        Camera.main.transform.position = transform.position - (Camera.main.transform.forward * cameraDistance);
    }
}
