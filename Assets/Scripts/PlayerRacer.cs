using UnityEngine;

[RequireComponent(typeof(Vehicle))]
public class PlayerRacer : MonoBehaviour
{
    private bool ignition = false;
    private Quaternion desiredAngle;
    private float deltaX = 0.0f;
    private float deltaY = 0.0f;
    private float velocity = 0.0f;
    private float acceleration = 5.0f; // TODO: This is what we'll modify if we implement a throttle. For now, keep constant.

    private Quaternion deltaRotation = Quaternion.identity;

    private Rigidbody rb;
    private Camera mainCamera;
    private Vehicle vehicleData;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        RacerManager.AddRacer(rb);
        vehicleData = GetComponent<Vehicle>();
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {
            ignition = !ignition;
            rb.useGravity = !rb.useGravity;
        }

        float horizontalTilt = Input.GetAxis("Horizontal");
        float verticalTilt = Input.GetAxis("Vertical");

        deltaX += horizontalTilt * Time.deltaTime;
        deltaY += verticalTilt * Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (ignition) {
            Vector3 force = BasicAI.VelocityToForce(rb.velocity + transform.up * vehicleData.GetMaxVelocity(), 
                rb, Time.fixedDeltaTime, vehicleData.GetMaxForce());
            rb.AddForce(force, ForceMode.Force);

            rb.velocity = BasicAI.ClampVectorMagnitude(rb.velocity, vehicleData.GetMaxVelocity());
        }

        rb.AddTorque(mainCamera.transform.right * deltaY * vehicleData.GetRotationSpeed(), ForceMode.Force);
        rb.AddTorque(mainCamera.transform.up * deltaX * vehicleData.GetRotationSpeed(), ForceMode.Force);

        deltaX = 0.0f;
        deltaY = 0.0f;
    }
}
