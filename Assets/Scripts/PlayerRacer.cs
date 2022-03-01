using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    [SerializeField]
    private float maxAcceleration = 20.0f;
    [SerializeField]
    private float maxVelocity = 20.0f;
    [SerializeField]
    private float maxForce = 10.0f;
    [SerializeField]
    private float rotationSpeed = 1.5f;
    [SerializeField]
    private float vehicleWidth = 2.0f;
    [SerializeField]
    private float vehicleHeight = 2.0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        RacerManager.AddRacer(rb);
    }

    void Update() {
        if (Input.GetKeyDown(KeyCode.Space)) {    // TODO: do this with a fancier input manager
            ignition = true;
            rb.useGravity = false;
        }

        float horizontalTilt = Input.GetAxis("Horizontal");
        float verticalTilt = Input.GetAxis("Vertical");

        deltaX += horizontalTilt * Time.deltaTime;
        deltaY += verticalTilt * Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (ignition) {
            Vector3 force = BasicAI.VelocityToForce(rb.velocity + transform.up * maxVelocity, rb, Time.fixedDeltaTime, maxForce);
            rb.AddForce(force);

            rb.velocity = BasicAI.ClampVectorMagnitude(rb.velocity, maxVelocity);
        }

        // TODO: Not totally happy with rotating a transform directly, but Quaternion methods didn't get the desired result
        transform.RotateAround(transform.position, Camera.main.transform.right, deltaY * rotationSpeed);
        transform.RotateAround(transform.position, Camera.main.transform.up, deltaX * rotationSpeed);

        deltaX = 0.0f;
        deltaY = 0.0f;
    }
}
