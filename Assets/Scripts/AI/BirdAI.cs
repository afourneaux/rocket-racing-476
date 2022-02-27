using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BirdAI : MonoBehaviour
{
    [SerializeField]
    private float maxAcceleration = 20.0f;
    [SerializeField]
    private float maxVelocity = 20.0f;
    [SerializeField]
    private float rotationSpeed = 1.0f;

    [SerializeField]
    private float arrivalRadius = 0.5f;
    [SerializeField]
    private float slowDownRadius = 5.0f;
    [SerializeField]
    private float t2t = 1.0f;

    [SerializeField]
    private float shortestDistanceToRotate = 1.0f;



    private Vector3 startingPos;
    private Quaternion startingRotation;
    private Rigidbody rb;

    private Vector3 target;
    private bool resetting = true;

    private void Start()
    {
        startingPos = transform.position;
        startingRotation = transform.rotation;
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (!resetting)
        {
            rb.velocity = BasicAI.SteeringSeek(rb.position, rb.velocity, target, 
                maxAcceleration, maxVelocity, Time.fixedDeltaTime);

            float distanceToTarget = Vector3.Distance(rb.position, target);
            if (rb.velocity != Vector3.zero && Vector3.Distance(rb.position, target) > shortestDistanceToRotate)
            {
                rb.rotation = BasicAI.SteeringLookWhereYouAreGoing(rb.rotation, rb.velocity, rotationSpeed);
            }
        }
        else
        {
            ResetUpdate();
        }
    }

    public void SetDestination(Vector3 position)
    {
        target = position;
        resetting = false;
    }

    public void ResetPosition()
    {
        resetting = true;
    }

    private void ResetUpdate()
    {
        rb.velocity = BasicAI.SteeringArrive(rb.position, rb.velocity, startingPos,
            slowDownRadius, arrivalRadius, maxAcceleration, maxVelocity, t2t, Time.fixedDeltaTime);
        
        if (rb.velocity == Vector3.zero)
        {
            rb.rotation = Quaternion.Lerp(rb.rotation, startingRotation, Time.fixedDeltaTime);
        }
        else
        {
            rb.rotation = BasicAI.SteeringLookWhereYouAreGoing(rb.rotation, rb.velocity, rotationSpeed);
        }
    }
}
